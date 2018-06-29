using CollectionManager.DataTypes;
using OsuMemoryDataProvider;
using StreamCompanionTypes.DataTypes;

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
    }
}