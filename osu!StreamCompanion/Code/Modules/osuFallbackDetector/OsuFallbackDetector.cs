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
        //LastVersion = b20160403.6
        private const string LAST_FALLBACK_VERSION = "b20160403.6";

        private Settings _settings;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            string FilePath = GetConfigFilePath();
            
            if (!isValidWindowsPath(FilePath))
            {
                logger.Log("WARNING: Path to osu! config location isn't valid. Tried: \"{0}\"", LogLevel.Basic, FilePath);
                return;
            }
            if (!File.Exists(FilePath))
            {
                logger.Log("WARNING: Could not get correct osu! config location. Tried: \"{0}\"", LogLevel.Basic, FilePath);
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
            string filename = string.Format("osu!.{0}.cfg", StripInvalidCharacters(Environment.UserName));
            return Path.Combine(LoadOsuDir(), filename);
        }

        private string StripInvalidCharacters(string name)
        {
            foreach (var invalidChar in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(invalidChar.ToString(), string.Empty);
            }
            return name.Replace(".", string.Empty);
        }
        private bool isValidWindowsPath(string path)
        {
            bool isValid = true;
            try
            {
                Path.GetFullPath(path);
            }
            catch
            {
                isValid = false;
            }
            return isValid;
        }
        private string LoadOsuDir()
        {
            return _settings.Get<string>(_names.MainOsuDirectory);
        }
    }
}
