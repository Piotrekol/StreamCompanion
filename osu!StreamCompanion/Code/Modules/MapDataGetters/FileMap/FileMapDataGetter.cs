using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1;

namespace osu_StreamCompanion.Code.Modules.MapDataGetters.FileMap
{
    public class FileMapDataGetter : IModule, IMapDataGetter,IDisposable
    {
        public bool Started { get; set; }
        private readonly FileMapManager _fileMapManager = new FileMapManager();
        private ILogger _logger;

        public void Start(ILogger logger)
        {
            _logger = logger;
            Started = true;
        }

        public void SetNewMap(MapSearchResult map)
        {
            var ingamePatterns = new List<string>();
            foreach (var s in map.FormatedStrings)
            {
                var name = $"SC-{s.Name}";
                if (s.ShowInOsu)
                {
                    ingamePatterns.Add(name);
                }
                string valueToWrite = (s.SaveEvent & map.Action) != 0 ? s.GetFormatedPattern() : "    ";
                if (s.ShowInOsu)
                {
                    var configName = "conf-" + name;
                    var valueName = "value-" + name;
                    var config = $"{s.XPosition} {s.YPosition} {s.Color.R} {s.Color.G} {s.Color.B} {s.FontName.Replace(' ','/')} {s.FontSize}";
                    _fileMapManager.Write(configName, config);
                    if (!s.IsMemoryFormat)
                        _fileMapManager.Write(valueName, valueToWrite);
                }

                if (!s.IsMemoryFormat)
                    _fileMapManager.Write(name, valueToWrite);
            }
            _fileMapManager.Write("Sc-ingamePatterns", string.Join(" ", ingamePatterns)+" ");
        }

        public void Dispose()
        {
            _fileMapManager.Write("Sc-ingamePatterns", " ");

        }
    }
}