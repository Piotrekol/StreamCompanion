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
            "https://3187b2a91f23411ab7ec5f85ad7d80b8@sentry.pioo.space/2";
        public static SentryClient SentryClient { get; } = new SentryClient(new SentryOptions
        {
            Dsn = SentryDsn,
            Release = Program.ScVersion
        });

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