namespace StreamCompanionTypes.DataTypes
{
    public class MapSearchArgs
    {
        public string Artist { get; set; } = "";
        public string Title { get; set; } = "";
        public string Diff { get; set; } = "";
        public string Raw { get; set; } = "";
        public int MapId { get; set; } = 0;
        public OsuStatus Status { get; set; } = OsuStatus.Null;

    }
}