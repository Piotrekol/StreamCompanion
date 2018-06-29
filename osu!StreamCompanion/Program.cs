using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion
{
    static class Program
    {
        public static string ScVersion ="v180629.20";
        private static Initializer _initializer;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string appGuid = ((GuidAttribute)Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0)).Value.ToString();
            string mutexId = string.Format("Global\\{{{0}}}", appGuid);
            using (var mutex = new Mutex(false, mutexId))
            {

                var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
                var securitySettings = new MutexSecurity();
                securitySettings.AddAccessRule(allowEveryoneRule);
                mutex.SetAccessControl(securitySettings);

                // edited by acidzombie24
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
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    AppDomain.CurrentDomain.UnhandledException +=
                    new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                    Application.ThreadException += Application_ThreadException;
                    _initializer = new Initializer();
                    _initializer.Start();
                    Application.Run(_initializer);


                }
                finally
                {
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }



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
        private static string errorNotification = @"There was unhandled problem with a program and it needs to close." + Environment.NewLine + "Error report was sent to Piotrekol";

        public static void HandleException(Exception ex)
        {
            try
            {
                MessageBox.Show(errorNotification, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                var form = new Error(ex.Message + Environment.NewLine + ex.StackTrace);
                form.ShowDialog();
            }
            finally
            {
                _initializer.ExitThread();
            }
        }

    }
}
