using System;
using System.Linq;
using System.Threading.Tasks;
using PpCalculatorTypes;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanion.Common
{
    public static class MapSearchResultsExtensions
    {
        public static Task<IPpCalculator> GetPpCalculator(this IMapSearchResult mapSearchResult)
        {
            if (mapSearchResult.SharedObjects.FirstOrDefault(o => o.GetType() == typeof(Lazy<Task<IPpCalculator>>)) is Lazy<Task<IPpCalculator>> ppCalculator)
                return ppCalculator.Value;

            return null;
        }
    }
}