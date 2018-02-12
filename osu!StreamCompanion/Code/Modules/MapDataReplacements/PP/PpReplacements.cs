using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;
using OppaiSharp;
using Beatmap = OppaiSharp.Beatmap;

namespace osu_StreamCompanion.Code.Modules.MapDataReplacements.PP
{
    public class PpReplacements : IModule, IMapDataReplacements, ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;

        DiffCalc diffCalculator = new DiffCalc();
        PPv2 _ppCalculator;
        Accuracy _accCalculator = new Accuracy();
        private Settings _settings;
        public bool Started { get; set; }

        public void Start(ILogger logger)
        {

        }

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            var ret = new Dictionary<string, string>
            {
                {"!FcPP!", "-1"},
                {"!99PP!", "-1"},
                {"!98PP!", "-1"},
                {"!95PP!", "-1"},
            };
            if (!map.FoundBeatmaps) return ret;

            var mapLocation = map.BeatmapsFound[0].FullOsuFileLocation(BeatmapHelpers.GetFullSongsLocation(_settings));
            if (!File.Exists(mapLocation)) return ret;
            FileInfo file = new FileInfo(mapLocation);
            while (FileIsLocked(file))
            {
                Thread.Sleep(1);
            }

            using (var stream = new FileStream(mapLocation, FileMode.Open))
            {
                using (var reader = new StreamReader(stream))
                {
                    var beatmap = Beatmap.Read(reader);

                    var beatmapCalc = diffCalculator.Calc(beatmap, Mods.NoMod);

                    ret["!FcPP!"] = Math.Round((new PPv2(beatmapCalc.Aim, beatmapCalc.Speed, beatmap)).Total, 2).ToString(CultureInfo.InvariantCulture);
                    ret["!99.9PP!"] = GetPp(beatmap, 99.9d).ToString(CultureInfo.InvariantCulture);
                    ret["!99PP!"] = GetPp(beatmap, 99d).ToString(CultureInfo.InvariantCulture);
                    ret["!98PP!"] = GetPp(beatmap, 98d).ToString(CultureInfo.InvariantCulture);
                    ret["!95PP!"] = GetPp(beatmap, 95d).ToString(CultureInfo.InvariantCulture);
                    ret["!90PP!"] = GetPp(beatmap, 90d).ToString(CultureInfo.InvariantCulture);
                }
            }
            return ret;
        }

        private double GetPp(Beatmap beatmap, double acc)
        {
            _accCalculator = new Accuracy(acc, beatmap.Objects.Count, 0);
            _ppCalculator = new PPv2(new PPv2Parameters(beatmap, _accCalculator.Count100, _accCalculator.Count50, _accCalculator.CountMiss, -1, _accCalculator.Count300));
            return Math.Round(_ppCalculator.Total, 2);
        }
        protected bool FileIsLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        private string GetOsuDir()
        {
            return _settings.Get<string>(_names.MainOsuDirectory);
        }

        public void SetSettingsHandle(Settings settings)
        {
            this._settings = settings;
        }

    }
}