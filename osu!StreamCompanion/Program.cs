using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.Loggers;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Windows;
using SharpRaven;
using SharpRaven.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace osu_StreamCompanion
{
    static class Program
    {
        public static string ScVersion ="v190424.19";
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

            if (AllowMultiInstance)
                Run();
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

                        Run();

                    }
                    finally
                    {
                        if (hasHandle)
                            mutex.ReleaseMutex();
                    }
                }
        }

        private static void Run()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException +=
                new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.ThreadException += Application_ThreadException;
            _initializer = new Initializer();
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
                Exception ex = null;
                try
                {
                    ex = (Exception)e.ExceptionObject;
                }
                finally
                {
                }
                HandleException(ex);
            }
        }

        private static (bool ForceQuit, bool SendReport, string Message) GetErrorData()
        {
            if (Exceptions.Count <= 4)
                return (false, true, $"{errorNotificationFormat} {willAttemptToRun} {messageWasSent}");

            return (false, false, $"{errorNotificationFormat} {willAttemptToRun} {messageWasNotSent}");
        }

        private static string errorNotificationFormat = @"There was unhandled problem with a program";
        private static string needToExit = @"and it needs to exit.";
        private static string willAttemptToRun = @"but it will attempt to run.";
        private static string messageWasSent = "Error report was sent to Piotrekol.";
        private static string messageWasNotSent = "Any further errors will !not! be sent to Piotrekol until StreamCompanion restarts.";
        
        private static List<Exception> Exceptions { get; set; }
        public static void HandleException(Exception ex)
        {
            bool shouldQuit = true;
            try
            {
                ex.Data["ExceptionCount"] = Exceptions.Count;
                Exceptions.Add(ex);

                ex.Data.Add("netFramework", GetDotNetVersion.Get45PlusFromRegistry());

                var errorConsensus = GetErrorData();
                shouldQuit = errorConsensus.ForceQuit;

#if DEBUG
                if (errorConsensus.SendReport)
                {
                    var ravenClient = SentryLogger.RavenClient;
                    ravenClient.Release = ScVersion;
                    var sentryEvent = new SentryEvent(ex);
                    ravenClient.Capture(sentryEvent);
                }
#endif
                MessageBox.Show(errorConsensus.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                var form = new Error(ex.Message + Environment.NewLine + ex.StackTrace, !errorConsensus.ForceQuit ? "StreamCompanion will attempt to run" : null);
                form.ShowDialog();
            }
            finally
            {
                if (shouldQuit)
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
}
