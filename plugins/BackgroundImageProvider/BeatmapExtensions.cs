using System.IO;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace BackgroundImageProvider
{
    public static class BeatmapExtensions
    {
        
        public static string GetImageLocation(this IBeatmap beatmap,ISettingsHandler settings)
        {
            var songsDirectory = BeatmapHelpers.GetFullSongsLocation(settings);
            var osuFileLocation = beatmap.FullOsuFileLocation(songsDirectory);
            string ImageLocation = string.Empty;
            using (StreamReader file = new StreamReader(osuFileLocation))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.ToLower().Contains(".jpg") || line.ToLower().Contains(".png"))
                    {
                        var splited = line.Split(',');
                        ImageLocation = Path.Combine(beatmap.BeatmapDirectory(songsDirectory), splited[2].Trim('"'));
                        if (!File.Exists(ImageLocation))
                        {
                            return string.Empty;
                        }
                        break;
                    }
                }
            }
            return ImageLocation;
        }
    }
}