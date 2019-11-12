using StreamCompanionTypes.DataTypes;

namespace osuOverlay
{
    public static class PluginSettings
    {
        //settings that plugin will be using

        public static readonly ConfigEntry EnableIngameOverlay = new ConfigEntry("EnableIngameOverlay", false);
        public static readonly ConfigEntry MainOsuDirectory = new ConfigEntry("MainOsuDirectory", "");
    }
}