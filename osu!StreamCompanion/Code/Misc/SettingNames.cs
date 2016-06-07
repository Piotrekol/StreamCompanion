namespace osu_StreamCompanion.Code.Misc
{

    public sealed class SettingNames
    {
        public readonly ConfigEntry setting1 = new ConfigEntry("settingname", value:false);
        
        static readonly SettingNames _instance = new SettingNames();
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
