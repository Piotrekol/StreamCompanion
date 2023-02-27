using System;
using System.Threading;
using System.Threading.Tasks;

namespace StreamCompanion.Common
{
    public sealed class CancelableAsyncLazy<T>
    {
        private readonly Func<CancellationToken, Task<T>> _valueFactory;
        private volatile bool _memoized;
        private readonly SemaphoreSlim _mutex;
        private T _value;

        public CancelableAsyncLazy(Func<CancellationToken, Task<T>> valueFactory)
        {
            _valueFactory = valueFactory
                            ?? throw new ArgumentNullException(nameof(valueFactory));
            _mutex = new SemaphoreSlim(1, 1);
        }

        public T Value => GetValueAsync().Result;

        public async Task<T> GetValueAsync() => await GetValueAsync(CancellationToken.None);

        public async Task<T> GetValueAsync(CancellationToken cancellationToken)
        {
            await _mutex.WaitAsync(cancellationToken);
            try
            {
                if (!_memoized)
                {
                    _value = await _valueFactory(cancellationToken).ConfigureAwait(false);
                    _memoized = true;
                    // at this point, the value is available, however, the caller
                    // might have indicated to cancel the operation; I favor 
                    // checking for cancellation one last time here, but you
                    // might decide against it and return the result anyway
                    cancellationToken.ThrowIfCancellationRequested();
                }
                return _value;
            }
            finally
            {
                _mutex.Release();
            }
        }
    }
}