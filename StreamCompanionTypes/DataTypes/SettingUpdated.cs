using System;

namespace StreamCompanionTypes.DataTypes
{
    public class SettingUpdated : EventArgs
    {
        public SettingUpdated(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}