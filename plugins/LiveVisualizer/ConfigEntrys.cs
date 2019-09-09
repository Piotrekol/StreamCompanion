using StreamCompanionTypes.DataTypes;

namespace LiveVisualizer
{
    internal static class ConfigEntrys
    {
        public const string Prefix = "LiveVisualizer_";

        public static readonly ConfigEntry LiveVisualizerConfig = new ConfigEntry($"{Prefix}Config", "{}");

    }
}