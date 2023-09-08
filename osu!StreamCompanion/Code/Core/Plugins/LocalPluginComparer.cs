using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public class LocalPluginComparer : IComparer<LocalPluginEntry>
    {
        public int Compare(LocalPluginEntry x, LocalPluginEntry y)
        {
            if (x.EnabledForcefully && !y.EnabledForcefully)
                return 1;

            if (!x.EnabledForcefully && y.EnabledForcefully)
                return -1;

            return x.Name.CompareTo(y.Name);
        }
    }
}
