using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Rulesets.Osu.Objects;
using osu.Game.Rulesets.Osu.Statistics;
using osu.Game.Scoring;
using osuTK;
using System;
using System.Linq;
using PpCalculatorTypes;

namespace PpCalculator
{
    public partial class OsuAccuracyHeatmap
    {
        /// <summary>
        /// Size of the inner circle containing the "hit" points, relative to the size of this <see cref="AccuracyHeatmap"/>.
        /// All other points outside of the inner circle are "miss" points.
        /// </summary>
        private const float inner_portion = 0.8f;

        /// <summary>
        /// Number of rows/columns of points.
        /// ~4px per point @ 128x128 size (the contents of the <see cref="AccuracyHeatmap"/> are always square). 1089 total points.
        /// </summary>
        private const int points_per_dimension = 33;

        private const float rotation = 45;

        private readonly ScoreInfo score;
        private readonly IBeatmap playableBeatmap;

        /// <summary>
        /// The highest count of any point currently being displayed.
        /// </summary>
        protected float PeakValue { get; private set; }


        public HitPoint[][] Points { get; private set; } = new HitPoint[points_per_dimension][];
        public OsuAccuracyHeatmap(ScoreInfo score, IBeatmap playableBeatmap)
        {
            this.score = score;
            this.playableBeatmap = playableBeatmap;
        }


        public void CalculateSlow(int msToKeep = 0)
        {
            Vector2 centre = new Vector2(points_per_dimension) / 2;
            float innerRadius = centre.X * inner_portion;

            for (int r = 0; r < points_per_dimension; r++)
            {
                Points[r] = new HitPoint[points_per_dimension];

                for (int c = 0; c < points_per_dimension; c++)
                {
                    HitPointType pointType = Vector2.Distance(new Vector2(c, r), centre) <= innerRadius
                        ? HitPointType.Hit
                        : HitPointType.Miss;

                    Points[r][c] = new HitPoint(pointType, pointType == HitPointType.Hit ? new PpCalculatorTypes.Color4(102, 255, 204, 255) : new PpCalculatorTypes.Color4(255, 102, 102, 255));
                }
            }

            if (score.HitEvents == null || score.HitEvents.Count == 0)
                return;

            // Todo: This should probably not be done like this.
            //float radius = OsuHitObject.OBJECT_RADIUS * (1.0f - 0.7f * (playableBeatmap.Difficulty.CircleSize - 5) / 5) / 2;
            var lastEventTime = score.HitEvents.Last().TimeOffset;
            var fromTime = msToKeep==0 
                ? 0 
                : lastEventTime - msToKeep;

            foreach (var e in score.HitEvents.Where(e => e.HitObject is HitCircle && !(e.HitObject is SliderTailCircle) && fromTime < e.TimeOffset))
            {
                if (e.LastHitObject == null || e.Position == null)
                    continue;

                var osuHitObject = (OsuHitObject)e.HitObject;
                AddPoint(((OsuHitObject)e.LastHitObject).StackedEndPosition, (osuHitObject).StackedEndPosition, e.Position.Value, (float)osuHitObject.Radius); //radius);
            }
        }

        protected void AddPoint(Vector2 start, Vector2 end, Vector2 hitPoint, float radius)
        {
            double angle1 = Math.Atan2(end.Y - hitPoint.Y, hitPoint.X - end.X); // Angle between the end point and the hit point.
            double angle2 = Math.Atan2(end.Y - start.Y, start.X - end.X); // Angle between the end point and the start point.
            double finalAngle = angle2 - angle1; // Angle between start, end, and hit points.
            float normalisedDistance = Vector2.Distance(hitPoint, end) / radius;

            // Consider two objects placed horizontally, with the start on the left and the end on the right.
            // The above calculated the angle between {end, start}, and the angle between {end, hitPoint}, in the form:
            //             +pi | 0
            //     O --------- O ----->      Note: Math.Atan2 has a range (-pi <= theta <= +pi)
            //             -pi | 0
            // E.g. If the hit point was directly above end, it would have an angle pi/2.
            //
            // It also calculated the angle separating hitPoint from the line joining {start, end}, that is anti-clockwise in the form:
            //               0 | pi
            //     O --------- O ----->
            //             2pi | pi
            //
            // However keep in mind that cos(0)=1 and cos(2pi)=1, whereas we actually want these values to appear on the left, so the x-coordinate needs to be inverted.
            // Likewise sin(pi/2)=1 and sin(3pi/2)=-1, whereas we actually want these values to appear on the bottom/top respectively, so the y-coordinate also needs to be inverted.
            //
            // We also need to apply the anti-clockwise rotation.
            double rotatedAngle = finalAngle - MathUtils.DegreesToRadians(rotation);
            var rotatedCoordinate = -1 * new Vector2((float)Math.Cos(rotatedAngle), (float)Math.Sin(rotatedAngle));

            Vector2 localCentre = new Vector2(points_per_dimension - 1) / 2;
            float localRadius = localCentre.X * inner_portion * normalisedDistance; // The radius inside the inner portion which of the heatmap which the closest point lies.
            Vector2 localPoint = localCentre + localRadius * rotatedCoordinate;

            // Find the most relevant hit point.
            int r = Math.Clamp((int)Math.Round(localPoint.Y), 0, points_per_dimension - 1);
            int c = Math.Clamp((int)Math.Round(localPoint.X), 0, points_per_dimension - 1);

            PeakValue = Math.Max(PeakValue, Points[r][c].Increment());
            Points[r][c].UpdateColour(PeakValue);
        }
    }
}
