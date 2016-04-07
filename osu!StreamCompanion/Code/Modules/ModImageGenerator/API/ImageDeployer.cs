using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Resources;
using osu_StreamCompanion.Properties;

namespace osu_StreamCompanion.Code.Modules.ModImageGenerator.API
{
    public class ImageDeployer
    {
        public string ImagesDirectory { get; set; }

        public ImageDeployer(string imagesDirectory)
        {
            this.ImagesDirectory = imagesDirectory;
        }
        
        public void DeployImages()
        {
            var load = Resources.DT;

            ResourceSet resources = Resources.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, false, false);
            if (!Directory.Exists(ImagesDirectory))
                Directory.CreateDirectory(ImagesDirectory);
            foreach (DictionaryEntry resource in resources)
            {
                string resourceName = (string)resource.Key;

                //Mod images are either 2 or 3 characters
                if (resourceName.Length != 2 && resourceName.Length != 3)
                    continue;

                string resourceFullPath = Path.Combine(ImagesDirectory, resourceName + ".png");
                if (File.Exists(resourceFullPath)) continue;

                object obj = resource.Value;
                Image image = (Image)obj;
                if (image != null)
                {
                    image.Save(resourceFullPath, ImageFormat.Png);
                    image.Dispose();
                }
            }

        }

        public void CreateReadMe()
        {
            if (!Directory.Exists(ImagesDirectory))
                Directory.CreateDirectory(ImagesDirectory);
            else
            {
                string readmeFullPath = Path.Combine(ImagesDirectory, "ReadMe.txt");
                if (!File.Exists(readmeFullPath))
                    File.WriteAllLines(readmeFullPath, new[] { "All Mod Image files must have:", "\tUPPERCASE name", "\tsame dimensions(width and height).", "", "If mod image cannot be found in folder then mod is ignored." });
            }
        }
    }
}