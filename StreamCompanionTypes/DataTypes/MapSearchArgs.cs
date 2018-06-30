using System;

namespace StreamCompanionTypes.DataTypes
{
    public class MapSearchArgs : EventArgs
    {
        public string Artist { get; set; } = "";
        public string Title { get; set; } = "";
        public string Diff { get; set; } = "";
        public string Raw { get; set; } = "";
        public int MapId { get; set; } = 0;
        public OsuStatus Status { get; set; } = OsuStatus.Null;
        public string SourceName { get; }
        //TODO: enforce explicitly setting event type via ctor
        public OsuEventType EventType { get; set; } = OsuEventType.MapChange;

        public MapSearchArgs(string sourceName)
        {
            SourceName = sourceName;
        }
    }

    public enum OsuEventType
    {
        MapChange,//When map changes
        MapStatusChange,//When user map status changes(eg. listening->watching, playing->listening)
        SceneChange,//When osu! ingame scene changes(eg. user goes from main menu to song selection)
    }
}