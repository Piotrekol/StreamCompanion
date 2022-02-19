using System;
using System.Diagnostics;

namespace PpCalculatorTypes
{
    public struct HitPoint
    {
        /// <summary>
        /// The base colour which will be lightened/darkened depending on the value of this <see cref="HitPoint"/>.
        /// </summary>
        private Color4 BaseColour;
        private readonly HitPointType pointType;
        public Color4 Colour;
        public float Alpha;

        public int Count;
        private double LastHitTime;
        public HitPoint(HitPointType pointType, Color4 baseColour)
        {
            this.pointType = pointType;
            this.Colour = this.BaseColour = baseColour;
            LastHitTime = Count = 0;
            this.Alpha = 1;
            if (pointType == HitPointType.Miss)
                Count = 1;
        }

        public int Increment()
        {
            return ++Count;
        }

        public void UpdateColour(float peakValue)
        {
            // the point at which alpha is saturated and we begin to adjust colour lightness.
            const float lighten_cutoff = 0.95f;

            // the amount of lightness to attribute regardless of relative value to peak point.
            const float non_relative_portion = 0.2f;

            float amount = 0;

            // give some amount of alpha regardless of relative count
            amount += non_relative_portion * Math.Min(1, Count / 5f);

            // add relative portion
            amount += (1 - non_relative_portion) * (Count / peakValue);

            // apply easing
            amount = ApplyOutQuintEasing(Math.Min(1, amount));

            Debug.Assert(amount <= 1);

            Alpha = Math.Min(amount / lighten_cutoff, 1);
            if (pointType == HitPointType.Hit)
                Colour = BaseColour.Lighten(Math.Max(0, amount - lighten_cutoff));
        }

        private float ApplyOutQuintEasing(float time)
        {
            return --time * time * time * time * time + 1;
        }
    }

}
