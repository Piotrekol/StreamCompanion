using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace StreamCompanion.Common.Extensions
{
    public static class CancellationTokenSourceExtensions
    {
        public static bool TryCancel(this CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                cancellationTokenSource.Cancel();
                return true;
            }
            catch (ObjectDisposedException)
            {
                return false;
            }
        }
    }
}
