using StreamCompanionTypes.Attributes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace osu_StreamCompanion.Code.Core.Plugins
{
    public class LocalPluginEntry
    {
        public string TypeName { get; set; }
        public bool Enabled { get; set; } = true;

        [IgnoreDataMember]
        public IPluginMetadata Metadata { get; set; }
        [IgnoreDataMember]
        public Type Type { get; set; }
        [IgnoreDataMember]
        public bool EnabledForcefully { get; set; }
        [IgnoreDataMember]
        public List<LocalPluginEntry> EnabledForcefullyByPlugins = new(0);
        [IgnoreDataMember]
        public List<SCPluginDependencyAttribute> Dependencies { get; set; }
        [IgnoreDataMember]
        public IPlugin Plugin { get; set; }

        [IgnoreDataMember]
        public string Name => Metadata?.Name ?? "ERROR(see logs): " + TypeName;
        [IgnoreDataMember]
        public string Description => Metadata?.Description;
        [IgnoreDataMember]
        public string Authors => Metadata?.Authors;
        [IgnoreDataMember]
        public string ProjectURL => Metadata?.ProjectURL;
        [IgnoreDataMember]
        public string WikiUrl => Metadata?.WikiUrl;

        public override string ToString()
        {
            return $"Name: {TypeName}; Enabled: {Enabled}; Type: {Type?.ToString() ?? ""}";
        }
    }

}
