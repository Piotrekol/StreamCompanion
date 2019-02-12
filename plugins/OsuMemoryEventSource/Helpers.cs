using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
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
        public static readonly ConfigEntry EnablePpSmoothing = new ConfigEntry("EnablePPSmoothing", true);
        public static readonly ConfigEntry ClearLiveTokensAfterResultScreenExit = new ConfigEntry("ClearLiveTokensAfterResultScreenExit", true);

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
        #endregion

        public static bool IsMD5(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return false;
            }

            return Regex.IsMatch(input, "^[0-9a-fA-F]{32}$", RegexOptions.Compiled);
        }

        [DebuggerStepThrough()]
        public static T ExecWithTimeout<T>(Func<CancellationToken, T> function, int timeout = 10000, ILogger logger = null)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var task = new Task<T>(() => function(token));
            task.Start();
            if (task.Wait(TimeSpan.FromMilliseconds(timeout)))
            {
                logger?.Log("task finished", LogLevel.Debug);
                return task.Result;
            }
            cancellationTokenSource.Cancel();
            logger?.Log("task aborted", LogLevel.Debug);
            return default(T);
        }
    }
}