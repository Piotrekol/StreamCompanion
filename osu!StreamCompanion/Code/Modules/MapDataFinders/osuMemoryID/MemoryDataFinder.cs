using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.MapDataFinders.osuMemoryID
{

    /// <summary>
    /// memory-related code removed from opensource, THIS IS A DUMMY CLASS
    /// Finds mapID/other required data directly from osu! memory.
    /// </summary>
    public class MemoryDataFinder : IModule, IMapDataFinder, ISettingsProvider, ISqliteUser, IModParserGetter
    {
        private ILogger _logger;
        private MemoryDataFinderSettings _memoryDataFinderSettings;
        public string SettingGroup { get; } = "Map matching";
        private Settings _settings;
        private SqliteControler _sqLiteControler;
        public bool Started { get; set; }

        public OsuStatus SearchModes { get; } = OsuStatus.Playing;
        public string SearcherName { get; } = "Memory";
        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
        }

        public MapSearchResult FindBeatmap(Dictionary<string, string> mapDictionary)
        {
            var result = new MapSearchResult();
            
            
            return result;
        }



        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public void Free()
        {
            _memoryDataFinderSettings.Dispose();
        }

        public UserControl GetUiSettings()
        {
            if (_memoryDataFinderSettings == null || _memoryDataFinderSettings.IsDisposed)
            {
                _memoryDataFinderSettings = new MemoryDataFinderSettings(_settings);
            }
            return _memoryDataFinderSettings;
        }

        public void SetSqliteControlerHandle(SqliteControler sqLiteControler)
        {
            _sqLiteControler = sqLiteControler;
        }

        public void SetModParserHandle(List<IModParser> modParser)
        {
            
        }
    }
}