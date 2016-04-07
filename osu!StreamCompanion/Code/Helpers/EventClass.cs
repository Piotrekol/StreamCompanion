using System;

namespace osu_StreamCompanion.Code.Helpers
{
    public class SettingUpdated :EventArgs
    {
        public SettingUpdated(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
