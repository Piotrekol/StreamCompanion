using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes
{
    public sealed class SettingNames
    {
        //main
        public readonly ConfigEntry MainOsuDirectory = new ConfigEntry("MainOsuDirectory", "");
        public readonly ConfigEntry SongsFolderLocation = new ConfigEntry("SongsFolderLocation", "Songs");
        public readonly ConfigEntry LogLevel = new ConfigEntry("LogLevel", StreamCompanionTypes.DataTypes.LogLevel.Disabled.GetHashCode());
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
        public readonly ConfigEntry DisableClickCounterWrite = new ConfigEntry("DisableClickCounterWrite", false);

        //Data parser
        public readonly ConfigEntry ActualPatterns = new ConfigEntry("ActualPatterns", "");//Json array of OutputPattern
        public readonly ConfigEntry DisableDiskPatternWrite = new ConfigEntry("DisableDiskPatternWrite", false);

        //memory scanner
        public readonly ConfigEntry EnableMemoryScanner = new ConfigEntry("EnableMemoryScanner", true);
        public readonly ConfigEntry EnableMemoryPooling = new ConfigEntry("EnableMemoryPooling", true);
        //ingame overlay
        public readonly ConfigEntry EnableIngameOverlay = new ConfigEntry("EnableIngameOverlay", false);

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

        //TcpSocket
        public readonly ConfigEntry tcpSocketEnabled = new ConfigEntry("tcpSocketEnabled", false);
        public readonly ConfigEntry tcpSocketIp = new ConfigEntry("tcpSocketIp", "127.0.0.1");
        public readonly ConfigEntry tcpSocketPort = new ConfigEntry("tcpSocketPort", 7839);
        public readonly ConfigEntry tcpSocketLiveMapDataPort = new ConfigEntry("tcpSocketLiveMapDataPort", 7840);


        private static readonly SettingNames _instance = new SettingNames();
        public static SettingNames Instance
        {
            get { return _instance; }
        }

        private SettingNames()
        { }
    }
}