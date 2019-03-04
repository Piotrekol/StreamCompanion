using StreamCompanionTypes.DataTypes;
using System.Net.NetworkInformation;

namespace LiveVisualizer
{
    internal static class ConfigEntrys
    {
        public const string Prefix = "LiveVisualizer_";
        public static readonly ConfigEntry Enable = new ConfigEntry($"{Prefix}Enable", false);
        public static readonly ConfigEntry Font = new ConfigEntry($"{Prefix}Font", "Arial");
        public static readonly ConfigEntry ChartColor = new ConfigEntry($"{Prefix}ChartColor", "102;255;178;227");//66FFB2E3
        public static readonly ConfigEntry ChartProgressColor = new ConfigEntry($"{Prefix}ChartProgressColor", "178;255;178;227");//B2FFB2E3
        public static readonly ConfigEntry AutoSizeAxisY = new ConfigEntry($"{Prefix}AutoSizeAxisY", true);
        public static readonly ConfigEntry ManualAxisCutoffs = new ConfigEntry($"{Prefix}ManualAxisCutoffs", "30;60;100;200;350");
        public static readonly ConfigEntry ShowAxisYSeparator = new ConfigEntry($"{Prefix}ShowAxisYSeparator", true);
        public static readonly ConfigEntry AxisYSeparatorColor = new ConfigEntry($"{Prefix}AxisYSeparatorColor", "102;255;178;227");

    }
}