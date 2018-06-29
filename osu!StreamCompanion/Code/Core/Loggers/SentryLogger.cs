using System;
using osu_StreamCompanion.Code.Helpers;
using SharpRaven;
using SharpRaven.Data;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Core.Loggers
{
    public class SentryLogger : ILogger
    {
        public static string RavenDsn =
            "https://2a3c77450ec84295b6a6d426b2fdd9b5@sentry.io/107853";
        public static RavenClient RavenClient { get; } = new RavenClient(RavenDsn);

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

                RavenClient.Capture(sentryEvent);
            }
        }
    }
}