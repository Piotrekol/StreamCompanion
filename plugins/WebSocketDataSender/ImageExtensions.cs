using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace WebSocketDataSender
{
    public static class ImageExtensions
    {
        /// <summary>
        /// Resize the image to the specified width and height
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <param name="atMinimum">Should resulting image be fit to just one of the dimensions to preserve aspect ratio(default), or should it be at least that big</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(this Image image, int? width, int? height, bool atMinimum = false)
        {
            if ((!width.HasValue || width == 0) && (!height.HasValue || height == 0))
                throw new ArgumentNullException(nameof(width), "Both width and height cannot be null. Provide value for at least one of them.");

            double ratioX = (double)(width ?? double.MaxValue) / (double)image.Width;
            double ratioY = (double)(height ?? double.MaxValue) / (double)image.Height;
            double ratio;

            if (atMinimum)
                // use whichever multiplier is smaller
                ratio = ratioX > ratioY ? ratioX : ratioY;
            else
                ratio = ratioX < ratioY ? ratioX : ratioY;

            // now we can get the new height and width
            int newHeight = Convert.ToInt32(image.Height * ratio);
            int newWidth = Convert.ToInt32(image.Width * ratio);

            var destRect = new Rectangle(0, 0, newWidth, newHeight);
            var destImage = new Bitmap(newWidth, newHeight);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        public static Bitmap ResizeAndCropBitmap(this Image image, int width, int height)
        {

            Rectangle cropRect = new Rectangle(0, 0, width, height);
            Bitmap target = new Bitmap(cropRect.Width, cropRect.Height);
            image = ResizeImage(image, width, height, true);


            using (Graphics g = Graphics.FromImage(target))
            {
                var xOffset = Convert.ToInt32(Math.Floor((double)(image.Width - width) / 2));
                var yOffset = Convert.ToInt32(Math.Floor((double)(image.Height - height) / 2));
                g.DrawImage(image, cropRect, new Rectangle(xOffset, yOffset, target.Width, target.Height),
                    GraphicsUnit.Pixel);
            }

            return target;
        }
    }
}