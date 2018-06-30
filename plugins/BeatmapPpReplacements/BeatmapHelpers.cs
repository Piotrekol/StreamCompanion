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
        public static OppaiSharp.Beatmap GetOppaiSharpBeatmap(string mapLocation)
        {
            bool retry = true;
            OppaiSharp.Beatmap beatmap = null;
            do
            {
                try
                {
                    using (var stream = new FileStream(mapLocation, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            beatmap = OppaiSharp.Beatmap.Read(reader);
                            retry = false;
                        }
                    }
                }
                catch
                {
                    if (!File.Exists(mapLocation))
                        return null;
                }
            } while (retry);

            return beatmap;
        }

        [DebuggerStepThrough()]
        public static OppaiSharp.Mods Convert(this Mods mods)
        {
            OppaiSharp.Mods result = OppaiSharp.Mods.NoMod;
            if ((Mods.Nf & mods) != 0)
                result |= OppaiSharp.Mods.NoFail;
            if ((Mods.Ez & mods) != 0)
                result |= OppaiSharp.Mods.Easy;
            //if ((Mods.TD & mods) != 0)
            //result |= OppaiSharp.Mods.TouchDevice;
            if ((Mods.Hd & mods) != 0)
                result |= OppaiSharp.Mods.Hidden;
            if ((Mods.Hr & mods) != 0)
                result |= OppaiSharp.Mods.Hardrock;
            if ((Mods.Dt & mods) != 0)
                result |= OppaiSharp.Mods.DoubleTime;
            if ((Mods.Ht & mods) != 0)
                result |= OppaiSharp.Mods.HalfTime;
            if ((Mods.Nc & mods) != 0)
                result |= OppaiSharp.Mods.Nightcore;
            if ((Mods.Fl & mods) != 0)
                result |= OppaiSharp.Mods.Flashlight;
            if ((Mods.So & mods) != 0)
                result |= OppaiSharp.Mods.SpunOut;
            return result;
        }
    }
}