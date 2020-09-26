using System.IO;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces.Services;
using Beatmap = CollectionManager.DataTypes.Beatmap;

namespace StreamCompanion.Common
{
    public static class BeatmapExtensions
    {
        public static string BeatmapDirectory(this Beatmap beatmap, string songsDirectory)
        {
            return Path.Combine(songsDirectory, beatmap.Dir);
        }

        public static string FullOsuFileLocation(this Beatmap beatmap, string songsDirectory)
        {
            var beatmapDirectory = beatmap.BeatmapDirectory(songsDirectory);
            if (string.IsNullOrEmpty(beatmapDirectory) || string.IsNullOrEmpty(beatmap.OsuFileName))
                return "";
            return Path.Combine(beatmapDirectory, beatmap.OsuFileName);
        }

        public static string GetImageLocation(this IBeatmap beatmap, ISettings settings)
        {
            var songsDirectory = BeatmapHelpers.GetFullSongsLocation(settings);
            var osuFileLocation = beatmap.FullOsuFileLocation(songsDirectory);
            string ImageLocation = string.Empty;
            using (StreamReader file = new StreamReader(osuFileLocation))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.ToLower().Contains(".jpg") || line.ToLower().Contains(".png") || line.ToLower().Contains(".jpeg"))
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