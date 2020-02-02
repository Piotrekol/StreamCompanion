using System;
using StreamCompanionTypes.DataTypes;

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

        public void Update()
        {
            if (Updater != null)
                Token.Value = Updater();
        }
    }
}