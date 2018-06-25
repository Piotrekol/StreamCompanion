using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface IMapDataParser
    {
        List<OutputPattern> GetFormatedPatterns(Dictionary<string,string> replacements,OsuStatus status);
    }
}