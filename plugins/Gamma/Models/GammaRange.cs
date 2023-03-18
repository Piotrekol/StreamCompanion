namespace Gamma.Models
{
    public class GammaRange
    {
        public double MinAr { get; set; } = 10.01;
        public double MaxAr { get; set; } = 10.33;
        public float? Gamma { get; set; }
        public int UserGamma { get; set; } = 90;

        public override string ToString() => $"AR: {MinAr:00.00}-{MaxAr:00.00} Gamma: {UserGamma}";
    }
}