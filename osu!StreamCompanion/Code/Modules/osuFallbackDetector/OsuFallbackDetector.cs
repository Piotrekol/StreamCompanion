using System;
using System.IO;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.osuFallbackDetector
{
    class OsuFallbackDetector : IModule, ISettings
    {
        //LastVersion = b20151228.3
        private const string LAST_FALLBACK_VERSION = "b20151228.3";

        private Settings _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            string FilePath = GetConfigFilePath();
            if (File.Exists(FilePath))
            {
                logger.Log("WARNING: Could not get correct osu! config location. Tried: \"{0}\"",LogLevel.Advanced,FilePath);
                return;
            }
            bool isFallback = IsFallback(FilePath);
            
            _settings.Add("OsuFallback", isFallback);
            if(isFallback)
                logger.Log("Detected osu fallback version!",LogLevel.Basic);
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        private bool IsFallback(string configPath)
        {
            foreach (var cfgLine in File.ReadLines(configPath))
            {
                if (cfgLine.StartsWith("LastVersion") && cfgLine.Contains(LAST_FALLBACK_VERSION))
                    return true;
            }
            return false;
        }

        private string GetConfigFilePath()
        {
            string filename = string.Format("osu!.{0}.cfg", Environment.UserName);
            return Path.Combine(LoadOsuDir(), filename);
        }
        private string LoadOsuDir()
        {
            return _settings.Get("MainOsuDirectory", "");
        }
    }
}
