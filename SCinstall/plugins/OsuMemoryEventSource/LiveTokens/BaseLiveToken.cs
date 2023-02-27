using System;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;

namespace OsuMemoryEventSource.LiveTokens
{
    public abstract class BaseLiveToken
    {
        public IToken Token { get; set; }
        public Func<object> Updater;
        public BaseLiveToken(IToken token, Func<object> updater)
        {
            Token = token;
            Updater = updater;
        }

        protected bool CanUpdate(OsuStatus status) => Token.CanSave(status) && Updater != null;
        public abstract void Update(OsuStatus status = OsuStatus.All);
    }
}