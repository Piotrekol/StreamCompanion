using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Windows;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core.Loggers;
using SharpRaven.Data;

namespace osu_StreamCompanion
{
    static class Program
    {
        public static string ScVersion ="v190824.21";
        private static Initializer _initializer;
        private const bool AllowMultiInstance = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);

            string settingsProfileName = GetSettingsProfileNameFromArgs(args);

            if (AllowMultiInstance)
#pragma warning disable 162
                Run(settingsProfileName);
#pragma warning restore 162
            else
                using (var mutex = new Mutex(false, mutexId))
                {

                    var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
                    var securitySettings = new MutexSecurity();
                    securitySettings.AddAccessRule(allowEveryoneRule);
                    mutex.SetAccessControl(securitySettings);

                    var hasHandle = false;
                    try
                    {
                        try
                        {
                            hasHandle = mutex.WaitOne(2000, false);
                            if (hasHandle == false)
                            {
                                MessageBox.Show("osu!StreamCompanion is already running.", "Error");
                                return;
                            }
                        }
                        catch (AbandonedMutexException)
                        {
                            hasHandle = true;
                        }

                        Run(settingsProfileName);

                    }
                    finally
                    {
                        if (hasHandle)
                            mutex.ReleaseMutex();
                    }
                }
        }

        private static string GetSettingsProfileNameFromArgs(string[] args)
        {
            const string argPrefix = "--settings-profile=";
            int argIndex = args.AnyStartsWith(argPrefix);
            return argIndex == -1 ? null : args[argIndex].Substring(argPrefix.Length);
        }

        private static void Run(string settingsProfileName)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += Application_ThreadException;
            _initializer = new Initializer(settingsProfileName);
            _initializer.Start();
            Application.Run(_initializer);
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        static void HandleNonLoggableException(NonLoggableException ex)
        {
            MessageBox.Show(ex.CustomMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }
        public static void SafeQuit()
        {
            try
            {
                _initializer?.Exit();
            }
            catch
            {
            }
            Quit();
        }

        private static void Quit()
        {
            if (System.Windows.Forms.Application.MessageLoop)
            {
                System.Windows.Forms.Application.Exit();
            }
            else
            {
                System.Environment.Exit(1);
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is NonLoggableException)
            {
                var ex = (NonLoggableException)e.ExceptionObject;
                HandleNonLoggableException(ex);
            }
            else
            {
#if DEBUG
                throw (Exception)e.ExceptionObject;
#endif
#pragma warning disable 162
                Exception ex = null;
                try
                {
                    ex = (Exception)e.ExceptionObject;
                }
                finally
                {
                }
                HandleException(ex);
#pragma warning restore 162
            }
        }
        
        private static (bool SendReport, string Message) GetErrorData()
        {
            // ReSharper disable ConditionIsAlwaysTrueOrFalse
            bool sendReport = false;

#if WITHSENTRY
            sendReport = true;
#endif

            if (sendReport)
                return (true, $"{errorNotificationFormat} {needToExit} {messageWasSent}");

            return (false, $"{errorNotificationFormat} {needToExit} {privateBuildMessageNotSent}");

            // ReSharper restore ConditionIsAlwaysTrueOrFalse
        }

        private static string errorNotificationFormat = @"There was unhandled problem with a program";
        private static string needToExit = @"and it needs to exit.";

        private static string messageWasSent = "Error report was sent to Piotrekol.";
        private static string privateBuildMessageNotSent = "This is private build, so error report WAS NOT SENT.";

        public static void HandleException(Exception ex)
        {
            try
            {

                ex.Data.Add("netFramework", GetDotNetVersion.Get45PlusFromRegistry());

                var errorConsensus = GetErrorData();

#if WITHSENTRY
                if (errorConsensus.SendReport)
                {
                    var ravenClient = SentryLogger.RavenClient;
                    ravenClient.Release = ScVersion;
                    var sentryEvent = new SentryEvent(ex);
                    sentryEvent.Extra = SentryLogger.ContextData;
                    ravenClient.Capture(sentryEvent);
                }
#endif

                MessageBox.Show(errorConsensus.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                var form = new Error(ex.Message + Environment.NewLine + ex.StackTrace, null);
                form.ShowDialog();
            }
            finally
            {
                try
                {
                    SafeQuit();
                }
                catch
                {
                    _initializer.ExitThread();
                }
            }
        }
    }
}
