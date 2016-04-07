using System.Collections.Generic;
using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Interfeaces
{
    public interface IMapDataParser
    {
        /// <summary>
        /// Returns dictionary with formated outputs defined in settings by user.
        /// </summary>
        /// <param name="map">mapSearchResult entry with or without found maps</param>
        /// <returns>Filled dictionary with (mandatory)"np" key with value and (optional) filenames as Keys and Values as formated strings. In case there were no maps found, Values are filled with empty strings</returns>
        Dictionary<string, string> GetFormatedMapStrings(Dictionary<string,string> replacements,OsuStatus status);
    }
}