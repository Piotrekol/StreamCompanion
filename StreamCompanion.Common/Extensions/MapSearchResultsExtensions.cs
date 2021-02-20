using System;
using System.Linq;
using System.Threading.Tasks;
using PpCalculatorTypes;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanion.Common
{
    public static class MapSearchResultsExtensions
    {
        public static async Task<IPpCalculator> GetPpCalculator(this IMapSearchResult mapSearchResult)
        {
            if (!(mapSearchResult.SharedObjects.FirstOrDefault(o => o.GetType() == typeof(Lazy<Task<IPpCalculator>>)) is Lazy<Task<IPpCalculator>> ppCalculatorTask)) 
                return null;

            var ppCalculator = await ppCalculatorTask.Value;

            if (ppCalculator == null)
                return null;

            ppCalculator = (IPpCalculator)ppCalculator.Clone();
            ppCalculator.Mods = (mapSearchResult.Mods?.WorkingMods ?? "").Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            return ppCalculator;
        }
    }
}