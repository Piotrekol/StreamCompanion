using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PpCalculatorTypes;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanion.Common
{
    public static class MapSearchResultsExtensions
    {
        public static async Task<IPpCalculator> GetPpCalculator(this IMapSearchResult mapSearchResult, CancellationToken cancellationToken)
        {
            if (!(mapSearchResult.SharedObjects.FirstOrDefault(o => o is CancelableAsyncLazy<IPpCalculator>) is CancelableAsyncLazy<IPpCalculator> ppCalculatorLazy))
                return null;

            var ppCalculator = await ppCalculatorLazy.GetValueAsync(cancellationToken);

            if (ppCalculator == null)
                return null;

            ppCalculator = (IPpCalculator)ppCalculator.Clone();
            return ppCalculator;
        }
    }
}