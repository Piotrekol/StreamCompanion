using System;

namespace StreamCompanion.Common.Models
{
    public class WebOverlayRecommendedSettings
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public string ToUserReadableString()
        {
            var nl = Environment.NewLine;
            return $"Width: {Width}{nl}Height: {Height}";
        }
    }
}
