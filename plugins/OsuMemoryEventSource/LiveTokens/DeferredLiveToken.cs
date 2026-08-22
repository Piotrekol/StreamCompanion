using System;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;

namespace OsuMemoryEventSource.LiveTokens
{
    public class DeferredLiveToken : BaseLiveToken
    {
        public DeferredLiveToken(IToken token, Func<object> updater)
            : base(token, updater) { }

        public override void Update(OsuStatus status = OsuStatus.All)
        {
            if (Token is DeferredToken deferred)
            {
                deferred.Recompute(Updater, status);
                return;
            }

            if (!CanUpdate(status))
            {
                Token.Reset();
                return;
            }

            Token.Value = Updater();
        }
    }
}
