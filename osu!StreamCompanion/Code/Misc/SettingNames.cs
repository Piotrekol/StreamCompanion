using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Misc
{

    public sealed class SettingNames
    {//main
        public readonly ConfigEntry MainOsuDirectory = new ConfigEntry("MainOsuDirectory", "");
        public readonly ConfigEntry SongsFolderLocation = new ConfigEntry("SongsFolderLocation", "Songs");
        public readonly ConfigEntry LogLevel = new ConfigEntry("LogLevel", Core.DataTypes.LogLevel.Disabled.GetHashCode());
        public readonly ConfigEntry StartHidden = new ConfigEntry("StartHidden", false);
        public readonly ConfigEntry Console = new ConfigEntry("console", false);
        public readonly ConfigEntry LoadingRawBeatmaps = new ConfigEntry("LoadingRawBeatmaps", false);
        public readonly ConfigEntry LastRunVersion = new ConfigEntry("LastRunVersion", "N/A");
    
        //ClickCounter 
        public readonly ConfigEntry KeyList = new ConfigEntry("keyList", new List<string>());
        public readonly ConfigEntry KeyNames = new ConfigEntry("keyNames", new List<string>());
        public readonly ConfigEntry KeyCounts = new ConfigEntry("keyCounts", new List<int>());
        public readonly ConfigEntry FirstRun = new ConfigEntry("firstRun", true);
        public readonly ConfigEntry EnableMouseHook = new ConfigEntry("HookMouse", false);
        public readonly ConfigEntry CfgEnableKpx = new ConfigEntry("EnableKPM", false);
        public readonly ConfigEntry RightMouseCount = new ConfigEntry("rightMouseCount", 0L);
        public readonly ConfigEntry LeftMouseCount = new ConfigEntry("leftMouseCount", 0L);
        public readonly ConfigEntry ResetKeysOnRestart = new ConfigEntry("ResetKeysOnRestart", false);
        //Data parser
        public readonly ConfigEntry PatternFileNames = new ConfigEntry("PatternFileNames", new List<string>());
        public readonly ConfigEntry Patterns = new ConfigEntry("Patterns", new List<string>());
        public readonly ConfigEntry saveEvents = new ConfigEntry("saveEvents", new List<int>());
        public readonly ConfigEntry PatternIsMemory = new ConfigEntry("PatternIsMemory", new List<int>());
        public readonly ConfigEntry DisableDiskPatternWrite = new ConfigEntry("DisableDiskPatternWrite", false);
        //memory scanner
        public readonly ConfigEntry EnableMemoryScanner = new ConfigEntry("EnableMemoryScanner", true);
        //mod images
        public readonly ConfigEntry EnableModImages = new ConfigEntry("EnableModImages", true);
        public readonly ConfigEntry ImageWidth = new ConfigEntry("ImageWidth", 720);
        public readonly ConfigEntry ModHeight = new ConfigEntry("ModHeight", 64);
        public readonly ConfigEntry ModWidth = new ConfigEntry("ModWidth", 64);
        public readonly ConfigEntry ModImageSpacing = new ConfigEntry("ModImageSpacing", -25);
        public readonly ConfigEntry ModImageOpacity = new ConfigEntry("ModImageOpacity", 85);
        public readonly ConfigEntry DrawOnRightSide = new ConfigEntry("DrawOnRightSide", false);
        public readonly ConfigEntry DrawFromRightToLeft = new ConfigEntry("DrawFromRightToLeft", false);
        //Text mod generation
        public readonly ConfigEntry NoModsDisplayText = new ConfigEntry("NoModsDisplayText", "None");
        public readonly ConfigEntry UseLongMods = new ConfigEntry("UseLongMods", false);
        //osu!Fallback detector
        public readonly ConfigEntry OsuFallback = new ConfigEntry("OsuFallback", false);
        //osu!Post
        public readonly ConfigEntry osuPostLogin = new ConfigEntry("osuPostLogin", "");
        public readonly ConfigEntry osuPostPassword = new ConfigEntry("osuPostPassword", "");
        public readonly ConfigEntry osuPostEnabled = new ConfigEntry("osuPostEnabled", false);
        public readonly ConfigEntry osuPostEndpoint = new ConfigEntry("osuPostEndpoint", @"http://osupost.givenameplz.de/input.php?u=");
        







        private static readonly SettingNames _instance = new SettingNames();
        public static SettingNames Instance
        {
            get { return _instance; }
        }

        private SettingNames()
        { }
    }
    public class ConfigEntry
    {
        public ConfigEntry(string name, object value)
        {
            Name = name;
            _defaultValue = value;
        }
        public string Name { get; set; }
        private readonly object _defaultValue;
        public T Default<T>()
        {
            return (T)_defaultValue;
        }
    }
}
