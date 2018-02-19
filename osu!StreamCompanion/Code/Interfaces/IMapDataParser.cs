using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1;

namespace osu_StreamCompanion.Code.Interfaces
{
    public interface IMapDataParser
    {
        List<OutputPattern> GetFormatedPatterns(Dictionary<string,string> replacements,OsuStatus status);
    }
}