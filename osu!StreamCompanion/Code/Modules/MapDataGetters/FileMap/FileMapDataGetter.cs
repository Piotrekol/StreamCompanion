using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1;

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.FileMap
{
    public class FileMapDataGetter : IModule, IMapDataGetter
    {
        public bool Started { get; set; }
        private readonly FileMapManager _fileMapManager = new FileMapManager();
        public void Start(ILogger logger)
        {
            Started = true;
        }

        public void SetNewMap(MapSearchResult map)
        {
            var ingamePatterns = new List<string>();
            foreach (var s in map.FormatedStrings)
            {
                if (s.IsMemoryFormat) continue;//memory pattern saving is handled elsewhere, not in this codebase.

                string valueToWrite = (s.SaveEvent & map.Action) != 0 ? s.GetFormatedPattern() : "    ";
                var name = $"SC-{s.Name}";

                if (s.ShowInOsu)
                {
                    ingamePatterns.Add(name);
                    var configName = "conf-" + name;
                    var valueName = "value-" + name;
                    var config = $"{s.XPosition} {s.YPosition} {s.Color.R} {s.Color.G} {s.Color.B} {s.FontName}";
                    _fileMapManager.Write(configName, config);
                    _fileMapManager.Write(valueName, valueToWrite);
                }
                _fileMapManager.Write(name, valueToWrite);
            }
            _fileMapManager.Write("Sc-ingamePatterns", string.Join(" ", ingamePatterns)+" ");
        }
    }
}