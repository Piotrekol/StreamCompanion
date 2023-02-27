using System;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;

namespace OsuMemoryEventSource.LiveTokens
{
    public class LazyLiveToken : BaseLiveToken
    {
        private readonly LazyToken<object> lazyToken;

        public LazyLiveToken(IToken token, Func<object> updater) : base(token, updater)
        {
            lazyToken = (LazyToken<object>) token;
        }

        public override void Update(OsuStatus status = OsuStatus.All)
        {
            if (!Token.CanSave(status) || Updater == null)
            {
                lazyToken.Reset();
                return;
            }

            if (lazyToken.IsValueCreated)
                Token.Value = new Lazy<object>(Updater);
        }
    }
}