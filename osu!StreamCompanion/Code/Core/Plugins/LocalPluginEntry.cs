using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public class LocalPluginEntry
    {
        public string Name { get; set; }
        public bool Enabled { get; set; } = true;

        [IgnoreDataMember]
        public Type Type { get; set; }
        public bool EnabledForcefully { get; set; }
        [IgnoreDataMember]
        public List<string> EnabledForcefullyByPlugins = new(0);
        [IgnoreDataMember]
        public List<PluginDependencyAttribute> Dependencies { get; set; }
        [IgnoreDataMember]
        public IPlugin Plugin { get; set; }

        public override string ToString()
        {
            return $"Name: {Name}; Enabled: {Enabled}; Type: {Type?.ToString() ?? ""}";
        }
    }
}
