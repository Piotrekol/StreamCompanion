using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Windows;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core.Loggers;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using StreamCompanionTypes.Enums;
using TaskExtensions = StreamCompanion.Common.TaskExtensions;
#if DEBUG
using System.Diagnostics;
#endif
namespace osu_StreamCompanion
{
    static class Program
    {
        public static string ScVersion ="v210310.19";
        private static Initializer _initializer;

        private static bool AllowMultiInstance = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
#if False
            AllowMultiInstance = true;
#endif
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);

            string settingsProfileName = GetSettingsProfileNameFromArgs(args)?.Trim();
            if (!string.IsNullOrEmpty(settingsProfileName) && settingsProfileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                // settingsProfileName contains chars not valid for a filename
                MessageBox.Show(settingsProfileName + " is an invalid settings profile name", "Error");
                return;
            }

            if (!OperatingSystem.IsWindows() || AllowMultiInstance)
                Run(settingsProfileName);
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
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            TaskExtensions.GlobalExceptionHandler = HandleException;
            _initializer = new Initializer(settingsProfileName);
            _initializer.Start();
            Application.Run(_initializer);
        }

        private static void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
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
            catch (Exception ex)
            {
                MainLogger.Instance.Log(ex, LogLevel.Error);
                MessageBox.Show(
                    $"There was a problem while shutting down Stream Companion. Some of the settings might have not been saved.{Environment.NewLine}{Environment.NewLine}{ex}",
                    "Stream Companion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                System.Environment.Exit(0);
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
                WaitForDebugger((Exception)e.ExceptionObject);
                //throw (Exception)e.ExceptionObject;
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
#if DEBUG
        private static void WaitForDebugger(Exception ex)
        {
            var result = MessageBox.Show($"Unhandled error: attach debugger?{Environment.NewLine}" +
                                         $"press Yes to attach local debugger{Environment.NewLine}" +
                                         $"press No to wait for debugger (Application will freeze){Environment.NewLine}" +
                                         $"press cancel to ignore and continue error handling as usual{Environment.NewLine}" +
                                         $"{ex}", "Error - attach debugger?", MessageBoxButtons.YesNoCancel);
            switch (result)
            {
                case DialogResult.Cancel:
                    return;
                case DialogResult.Yes:
                    Debugger.Launch();
                    break;
                case DialogResult.No:
                    break;
            }

            while (!Debugger.IsAttached)
            {
                Thread.Sleep(100);
            }

            Debugger.Break();
        }
#endif
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
#if DEBUG
                WaitForDebugger(ex);
#endif
                var errorResult = GetExceptionText(ex);
                foreach (var d in errorResult.Data)
                {
                    ex.Data[d.Key] = d.Value;
                }

                //also reports to sentry if enabled
                MainLogger.Instance.Log(ex, LogLevel.Critical);

                MessageBox.Show(errorConsensus.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                var form = new Error(errorResult.Text, null);
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

        private static (string Text, IDictionary<string, object> Data) GetExceptionText(Exception ex)
        {
            var exceptionMessage = string.Empty;
            var dict = new Dictionary<string, object>();
            exceptionMessage +=
                $"{ex.GetType().Name}: {ex.Message}";
            if (ex is SocketException socketEx)
            {
                ex.Data["SocketErrorCode"] = socketEx.SocketErrorCode.ToString();
                ex.Data["ErrorCode"] = socketEx.ErrorCode.ToString();
                exceptionMessage += $"{Environment.NewLine}SocketErrorCode: {socketEx.SocketErrorCode}, ErrorCode: {socketEx.ErrorCode}";
            }

            exceptionMessage += $"{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}{Environment.NewLine}";
            if (ex is AggregateException aggEx)
            {
                foreach (var innerException in aggEx.InnerExceptions)
                {
                    var exResult = GetExceptionText(innerException);
                    exceptionMessage += exResult.Text;
                    foreach (var d in exResult.Data)
                    {
                        dict[d.Key] = d.Value;
                    }
                }
            }

            return (exceptionMessage, dict);
        }
    }
}
