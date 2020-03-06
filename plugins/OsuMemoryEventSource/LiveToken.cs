using System;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;

namespace OsuMemoryEventSource
{
    public class LiveToken
    {
        public IToken Token { get; set; }
        public Func<object> Updater;

        public LiveToken(IToken token, Func<object> updater)
        {
            Token = token;
            Updater = updater;
        }

        public void Update(OsuStatus status = OsuStatus.All)
        {
            if (Token.CanSave(status) && Updater != null)
                Token.Value = Updater();
        }
    }
}