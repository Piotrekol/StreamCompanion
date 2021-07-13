using System;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;

namespace OsuMemoryEventSource.LiveTokens
{
    public class LiveToken : BaseLiveToken
    {
        public LiveToken(IToken token, Func<object> updater) : base(token, updater)
        {
        }

        public override void Update(OsuStatus status = OsuStatus.All)
        {
            if (!CanUpdate(status))
            {
                Token.Reset();
                return;
            }

            Token.Value = Updater();
        }
    }
}