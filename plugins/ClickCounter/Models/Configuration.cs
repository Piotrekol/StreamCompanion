using System.ComponentModel;

namespace ClickCounter
{
    public class Configuration
    {
        public bool KeyboardEnabled { get; set; } = false;
        public bool MouseEnabled { get; set; } = false;
        public bool KPXEnabled { get; set; } = false;
        public bool ResetKeysOnStartup { get; set; } = false;
        public bool ResetKeyCountsOnEachPlay { get; set; } = false;
        public BindingList<KeyEntry> KeyEntries = new BindingList<KeyEntry>();
    }
}
