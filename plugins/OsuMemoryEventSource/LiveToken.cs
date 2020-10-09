using System;
using System.Diagnostics;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;

namespace OsuMemoryEventSource
{
    public class LiveToken
    {
        public IToken Token { get; set; }
        public Func<object> Updater;
        public bool IsLazy { get; set; }
        protected Lazy<object> Lazy = new Lazy<object>();
        public LiveToken(IToken token, Func<object> updater)
        {
            Token = token;
            Updater = updater;
            _ = Lazy.Value;
        }

        public void Update(OsuStatus status = OsuStatus.All)
        {
            if (!Token.CanSave(status) || Updater == null)
            {
                return;
            }

            if (IsLazy)
            {
                if (Lazy.IsValueCreated)
                {
                    Token.Value = Lazy = new Lazy<object>(Updater);
                }
            }
            else
            {
                Token.Value = Updater();
            }
        }
    }
}