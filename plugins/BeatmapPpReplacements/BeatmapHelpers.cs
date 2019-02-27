using System.Diagnostics;
using System.IO;
using CollectionManager.DataTypes;
using StreamCompanionTypes;
using StreamCompanionTypes.Interfaces;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace BeatmapPpReplacements
{
    public static class BeatmapHelpers
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
        private static readonly SettingNames _names = SettingNames.Instance;

        public static string GetFullSongsLocation(ISettingsHandler settings)
        {
            var dir = settings.Get<string>(_names.SongsFolderLocation);
            if (dir == _names.SongsFolderLocation.Default<string>())
            {
                dir = settings.Get<string>(_names.MainOsuDirectory);
                dir = Path.Combine(dir, "Songs\\");
            }
            return dir;
        }
    }
}