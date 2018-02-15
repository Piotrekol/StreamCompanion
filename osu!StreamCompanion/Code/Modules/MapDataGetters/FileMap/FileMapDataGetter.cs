using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfaces;

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
            foreach (var s in map.FormatedStrings)
            {
                var name = s.Key.Replace(".txt", "");
                _fileMapManager.Write(name, s.Value);
            }
        }
        
    }
}