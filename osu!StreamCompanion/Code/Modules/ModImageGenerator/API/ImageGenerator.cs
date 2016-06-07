using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;
using Point = System.Drawing.Point;

namespace osu_StreamCompanion.Code.Modules.ModImageGenerator.API
{
    public class ImageGenerator:ISaveRequester
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private Settings _settings;
        private ISaver _saver;
        public string ImagesFolderName { get; set; }
        public ImageGenerator(Settings settings, string imagesFolderName)
        {
            ImagesFolderName = imagesFolderName;
            _settings = settings;
        }

        private bool ModImageExists(string fullPath)
        {
            return File.Exists(fullPath);
        }
        public Bitmap GenerateImage(string[] modsList)
        {
            int imageWidth = _settings.Get<int>(_names.ImageWidth);
            int modHeight = _settings.Get<int>(_names.ModHeight);
            int modWidth = _settings.Get<int>(_names.ModWidth);
            int spacing = _settings.Get<int>(_names.ModImageSpacing);
            float opacity = (float)_settings.Get<int>(_names.ModImageOpacity) / 100;
            DrawSide drawSide = _settings.Get<bool>(_names.DrawOnRightSide) ? DrawSide.Right : DrawSide.Left;
            DrawDirection drawDirection = _settings.Get<bool>(_names.DrawFromRightToLeft) ? DrawDirection.FromRightToLeft : DrawDirection.FromLeftToRight;
            List<string> validModPaths = new List<string>();

            foreach (var mod in modsList)
            {
                string effectiveModPath = Path.Combine(_saver.SaveDirectory, ImagesFolderName, mod.ToUpper() + ".png");
                if (ModImageExists(effectiveModPath))
                {
                    validModPaths.Add(effectiveModPath);
                }
            }
            int widthOfGeneratedMods = GetWidthOfGeneratedMods(validModPaths.Count, modWidth, spacing);
            if (drawDirection == DrawDirection.FromRightToLeft)
                validModPaths.Reverse();
            Bitmap bitmap = new Bitmap(imageWidth, modHeight);
            bitmap.MakeTransparent();
            using (var g = Graphics.FromImage(bitmap))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                for (int i = 0; i < validModPaths.Count; i++)
                {
                    var singleModImage = Image.FromFile(validModPaths[i]);

                    using (var singleModImageWithOpacity = SetImageOpacity(singleModImage, opacity))
                    {
                        int xPos = GetCurrentXPostion(i, spacing, modWidth, widthOfGeneratedMods, imageWidth, drawSide,
                            drawDirection);
                        g.DrawImage(singleModImageWithOpacity, new Point(xPos, 0));
                    }
                    singleModImage.Dispose();
                }
                g.Save();
            }
            //bitmap.Save(ImageFullPath, ImageFormat.Png);
            return bitmap;
        }

        private int GetCurrentXPostion(int i, int spacing, int modImgWidth, int widthOfGeneratedMods,
            int imageWidth, DrawSide drawSide, DrawDirection drawDirection)
        {
            int xPos = i * (modImgWidth + spacing);
            if (drawSide == DrawSide.Left)
            {
                if (drawDirection == DrawDirection.FromRightToLeft)
                    xPos = widthOfGeneratedMods - xPos - modImgWidth;
                return xPos;
            }
            else
            {
                if (drawDirection == DrawDirection.FromRightToLeft)
                    xPos = imageWidth - xPos - modImgWidth;
                else
                    xPos += imageWidth - widthOfGeneratedMods;
                return xPos;
            }
        }

        private int GetWidthOfGeneratedMods(int numberOfMods, int modImgWidth, int spacing)
        {
            if (numberOfMods == 1)
                return modImgWidth;
            return numberOfMods * (modImgWidth + spacing) - spacing;
        }

        public Image SetImageOpacity(Image image, float opacity)
        {
            try
            {
                //create a Bitmap the size of the image provided  
                Bitmap bmp = new Bitmap(image.Width, image.Height);

                //create a graphics object from the image  
                using (Graphics gfx = Graphics.FromImage(bmp))
                {

                    //create a color matrix object  
                    ColorMatrix matrix = new ColorMatrix();

                    //set the opacity  
                    matrix.Matrix33 = opacity;

                    //create image attributes  
                    ImageAttributes attributes = new ImageAttributes();

                    //set the color(opacity) of the image  
                    attributes.SetColorMatrix(matrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

                    //now draw the image  
                    gfx.DrawImage(image, new Rectangle(0, 0, bmp.Width, bmp.Height), 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);
                }
                return bmp;
            }
            catch
            {
                return null;
            }
        }


        private enum DrawSide { Left = 0, Right = 1 }
        private enum DrawDirection { FromLeftToRight = 0, FromRightToLeft = 1 }

        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver; 
        }
    }

}