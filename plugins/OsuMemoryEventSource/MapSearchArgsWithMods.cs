using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;

namespace OsuMemoryEventSource
{
    public class MemoryMapSearchArgs : MapSearchArgs
    {
        public int Mods { get; set; }

        public MemoryMapSearchArgs(OsuEventType eventType) : base("OsuMemory", eventType)
        {
        }
    }
}