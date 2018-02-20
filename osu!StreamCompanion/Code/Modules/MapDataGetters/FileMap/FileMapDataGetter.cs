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
                if(s.IsMemoryFormat) continue;//memory pattern saving is handled elsewhere, not in this codebase.
                var name = s.Name;

                if ((s.SaveEvent & map.Action) != 0)
                    _fileMapManager.Write("SC-" + name, s.GetFormatedPattern());
                else
                    _fileMapManager.Write("SC-" + name, "    ");//spaces so object rect displays on obs preview window.

            }
        }
    }
}