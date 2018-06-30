namespace StreamCompanionTypes.DataTypes
{
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