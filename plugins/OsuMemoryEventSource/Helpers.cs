using System.Diagnostics;
using System.IO;
using CollectionManager.DataTypes;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using Beatmap = StreamCompanionTypes.DataTypes.Beatmap;

namespace OsuMemoryEventSource
{
    internal static class Helpers
    {
        public static bool IsInvalidCombination(Mods mods)
        {
            //There can be only one KMod active at the time.
            int c = 0;
            c += (mods & Mods.K9) > 0 ? 1 : 0;
            c += (mods & Mods.K8) > 0 ? 1 : 0;
            if (c > 1) return true;
            c += (mods & Mods.K7) > 0 ? 1 : 0;
            if (c > 1) return true;
            c += (mods & Mods.K6) > 0 ? 1 : 0;
            if (c > 1) return true;
            c += (mods & Mods.K5) > 0 ? 1 : 0;
            if (c > 1) return true;
            c += (mods & Mods.K4) > 0 ? 1 : 0;
            if (c > 1) return true;
            c += (mods & Mods.K3) > 0 ? 1 : 0;
            if (c > 1) return true;
            c += (mods & Mods.K2) > 0 ? 1 : 0;
            if (c > 1) return true;
            c += (mods & Mods.K1) > 0 ? 1 : 0;
            if (c > 1) return true;

            //Check mod combinations

            //DT HT
            c = 0;
            c += (mods & Mods.Dt) > 0 ? 1 : 0;
            c += (mods & Mods.Ht) > 0 ? 1 : 0;
            if (c > 1) return true;

            //HR EZ
            c = 0;
            c += (mods & Mods.Hr) > 0 ? 1 : 0;
            c += (mods & Mods.Ez) > 0 ? 1 : 0;
            if (c > 1) return true;

            return false;
        }

        public static OsuStatus Convert(this OsuMemoryStatus status)
        {
            switch (status)
            {
                case OsuMemoryStatus.EditingMap:
                    return OsuStatus.Editing;

                case OsuMemoryStatus.ResultsScreen:
                    return OsuStatus.Listening2;
                case OsuMemoryStatus.MainMenu:
                case OsuMemoryStatus.SongSelect:
                case OsuMemoryStatus.SongSelectEdit:
                case OsuMemoryStatus.MultiplayerRoom:
                case OsuMemoryStatus.MultiplayerResultsscreen:
                case OsuMemoryStatus.MultiplayerRooms:
                case OsuMemoryStatus.MultiplayerSongSelect:
                case OsuMemoryStatus.RankingTagCoop:
                case OsuMemoryStatus.RankingTeam:
                case OsuMemoryStatus.Tourney:
                case OsuMemoryStatus.OsuDirect:
                    return OsuStatus.Listening;
                case OsuMemoryStatus.Playing:
                    return OsuStatus.Playing;
                default:
                    return OsuStatus.Null;

            }
        }


        #region copied from BeatmapPpReplacements\BeatmapHelpers.cs
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
        #endregion
    }
}