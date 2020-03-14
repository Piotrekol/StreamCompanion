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
            "https://2a3c77450ec84295b6a6d426b2fdd9b5@sentry.io/107853";
        public static RavenClient RavenClient { get; } = new RavenClient(RavenDsn);

        public static Dictionary<string,string> ContextData { get; } = new Dictionary<string, string>();
        public SentryLogger()
        {
            RavenClient.Release = Program.ScVersion;
        }
        public void Log(object logMessage, LogLevel loglvevel, params string[] vals)
        {
            if (loglvevel == LogLevel.Error)
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

                sentryEvent.Extra = ContextData;
                RavenClient.Capture(sentryEvent);
            }
        }

        public void SetContextData(string key, string value)
        {
            ContextData[key] = value;
        }
    }
}