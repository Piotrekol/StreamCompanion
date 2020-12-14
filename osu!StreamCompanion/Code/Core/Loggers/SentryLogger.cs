using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Helpers;
using SharpRaven;
using SharpRaven.Data;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    public class SentryLogger : IContextAwareLogger
    {
        public static string RavenDsn =
            "https://3187b2a91f23411ab7ec5f85ad7d80b8@sentry.pioo.space/2";
        public static RavenClient RavenClient { get; } = new RavenClient(RavenDsn);

        public static Dictionary<string, string> ContextData { get; } = new Dictionary<string, string>();
        private object _lockingObject = new object();
        public SentryLogger()
        {
            RavenClient.Release = Program.ScVersion;
        }
        public void Log(object logMessage, LogLevel logLevel, params string[] vals)
        {
            if (logLevel == LogLevel.Critical)
            {
                SentryEvent sentryEvent;
                if (logMessage is Exception)
                {
                    if (logMessage is NonLoggableException)
                        return;
                    sentryEvent = new SentryEvent((Exception)logMessage);
                }
                else
                    sentryEvent = new SentryEvent(string.Format(logMessage.ToString(), vals));

                lock (_lockingObject)
                {
                    sentryEvent.Extra = ContextData;
                    RavenClient.Capture(sentryEvent);
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