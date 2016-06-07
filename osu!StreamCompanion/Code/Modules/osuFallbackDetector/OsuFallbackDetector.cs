using System;
using System.IO;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.osuFallbackDetector
{
    class OsuFallbackDetector : IModule, ISettings
    {
        private readonly SettingNames _names = SettingNames.Instance;
        //LastVersion = b20151228.3
        private const string LAST_FALLBACK_VERSION = "b20151228.3";

        private Settings _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            string FilePath = GetConfigFilePath();
            if (!Uri.IsWellFormedUriString(FilePath, UriKind.Absolute))
            {
                logger.Log("WARNING: Path to osu! config location isn't valid. Tried: \"{0}\"", LogLevel.Advanced, FilePath);
                return;
            }
            if (!File.Exists(FilePath))
            {
                logger.Log("WARNING: Could not get correct osu! config location. Tried: \"{0}\"", LogLevel.Advanced, FilePath);
                return;
            }
            bool isFallback = IsFallback(FilePath);

            _settings.Add(_names.OsuFallback.Name, isFallback);
            if (isFallback)
                logger.Log("Detected osu fallback version!", LogLevel.Basic);
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
            //TODO: Fix configuration filename being incorrect in some cases (eg. windows "email" usernames)
            string filename = string.Format("osu!.{0}.cfg", Environment.UserName);
            return Path.Combine(LoadOsuDir(), filename);
        }
        private string LoadOsuDir()
        {
            return _settings.Get<string>(_names.MainOsuDirectory);
        }
    }
}
