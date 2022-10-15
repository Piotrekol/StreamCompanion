using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Helpers;
using Sentry;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    public class SentryLogger : IContextAwareLogger
    {
        public static string SentryDsn =
            "https://61b0ba522c24450a87fba347918f6364@glitchtip.pioo.space/1";
        public static SentryClient SentryClient { get; } = new SentryClient(new SentryOptions
        {
            Dsn = SentryDsn,
            Release = Program.ScVersion,
            SendDefaultPii = true,
            BeforeSend = BeforeSend
        });

        private static SentryEvent BeforeSend(SentryEvent arg)
        {
            arg.User.IpAddress = null;
            return arg;
        }

        public static Dictionary<string, string> ContextData { get; } = new Dictionary<string, string>();
        private object _lockingObject = new object();

        public void Log(object logMessage, LogLevel logLevel, params string[] vals)
        {
            if (logLevel == LogLevel.Critical && logMessage is Exception exception && !(exception is NonLoggableException))
            {
                var sentryEvent = new SentryEvent(exception);

                lock (_lockingObject)
                {
                    foreach (var contextKeyValue in ContextData)
                    {
                        sentryEvent.SetExtra(contextKeyValue.Key, contextKeyValue.Value);
                    }
                    SentryClient.CaptureEvent(sentryEvent);
                }
            }
        }

        public void SetContextData(string key, string value)
        {
            lock (_lockingObject)
                ContextData[key] = value;
        }
    }
}