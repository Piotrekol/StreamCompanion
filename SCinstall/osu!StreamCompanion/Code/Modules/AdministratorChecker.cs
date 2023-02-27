using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace osu_StreamCompanion.Code.Modules
{
    internal class AdministratorChecker : IModule
    {
        private readonly ILogger _logger;

        [SupportedOSPlatform("windows")]
        private bool IsElevated =>
            WindowsIdentity.GetCurrent().Owner?
                .IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid) ?? false;

        public bool Started { get; set; }

        public AdministratorChecker(ILogger logger)
        {
            _logger = logger;
            Start(logger);
        }

        /// <summary>
        /// Detects correctly only if current process is not elevated
        /// </summary>
        /// <returns></returns>
        private bool IsOsuProcessElevated()
        {
            var osuProcess = Process.GetProcessesByName("osu!").FirstOrDefault();
            try
            {
                _ = osuProcess?.PriorityBoostEnabled;
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == 5)
            {
                //Access denied
                return true;
            }

            return false;
        }

        public void Start(ILogger logger)
        {
            if (!OperatingSystem.IsWindows() || IsElevated)
                return;

            if (IsOsuProcessElevated())
            {
                var message = "osu! is running as administrator while StreamCompanion is not. This will break StreamCompanion. Do not run osu! as administrator.";
                _logger.Log(message, LogLevel.Error);
                MessageBox.Show(message, "StreamCompanion - osu! running as administrator!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
