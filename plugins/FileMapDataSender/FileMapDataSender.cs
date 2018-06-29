using System;
using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace FileMapDataSender
{
    public class FileMapDataSender : IPlugin, IMapDataGetter, IDisposable, IFileMapDataSender
    {
        public bool Started { get; set; }
        private readonly FileMapManager _fileMapManager = new FileMapManager();
        private ILogger _logger;

        public string Description { get; } = "";
        public string Name { get; } = "FileMapDataSender";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public void Start(ILogger logger)
        {
            _logger = logger;
            Started = true;
        }

        public void Save(string fileName, string content)
        {
            _fileMapManager.Write(fileName, content);
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
                    var config = $"{s.XPosition} {s.YPosition} {s.Color.R} {s.Color.G} {s.Color.B} {s.FontName.Replace(' ', '/')} {s.FontSize}";
                    _fileMapManager.Write(configName, config);
                    if (!s.IsMemoryFormat)
                        _fileMapManager.Write(valueName, valueToWrite.Replace("\r", ""));
                }

                if (!s.IsMemoryFormat)
                    _fileMapManager.Write(name, valueToWrite);
            }
            _fileMapManager.Write("Sc-ingamePatterns", string.Join(" ", ingamePatterns) + " ");
        }

        public void Dispose()
        {
            _fileMapManager.Write("Sc-ingamePatterns", " ");

        }

    }
}