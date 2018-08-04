using System;
using OppaiSharp;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace OsuMemoryEventSource
{
    internal static class OppaiSharpBeatmapHelper
    {
        public static Func<StreamCompanionTypes.DataTypes.Beatmap> GetCurrentBeatmap;
        internal static int GetMaxComboSafe(this OppaiSharp.Beatmap beatmap, bool onlyCount300 = false)
        {
            try
            {
                return beatmap.GetMaxCombo(onlyCount300);
            }
            catch (Exception ex)
            {
                var logger = OsuMemoryEventSourceBase.Logger;
                if (logger == null)
                    throw;

                var scBeatmap = GetCurrentBeatmap?.Invoke();
                if (scBeatmap != null)
                {
                    var scMapData =
                        $"GetMaxComboSafe: {scBeatmap.ToString(true).Replace('{','[').Replace('}',']')} | {scBeatmap.Md5} | {scBeatmap.MapId} | {scBeatmap.MapSetId} | {onlyCount300}";
                    logger.Log(scMapData, LogLevel.Error);
                }
                else
                    throw;

            }
            return 1;
        }

    }
}