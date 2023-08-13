using System.Collections.Generic;
using System.ComponentModel;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public class PluginManagerConfiguration
    {
        public BindingList<LocalPluginEntry> Plugins { get; set; } = new();
        public string IndexFileUrl { get; set; } = "TODO";
    }
}
