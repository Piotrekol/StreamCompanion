using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.IngameOverlay
{
    class IngameOverlay : IModule, ISettingsProvider, ISaveRequester, IMapDataGetter, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private Settings _settings;
        private ISaver _saver;
        public bool Started { get; set; }
        public string SettingGroup { get; } = "General";
        private const string FilesFolder = "Dlls";
        private IngameOverlaySettings _overlaySettings;
        private ILogger _logger;

        private Thread _workerThread;
        private Process _currentOsuProcess;
        private bool _pauseProcessTracking;
        private bool _injectedAtleastOnce = false;

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;

            if (_settings.Get<bool>(_names.EnableIngameOverlay))
            {
                CopyFreeType();
                _workerThread = new Thread(WatchForProcessStart);
                _workerThread.Start();
            }
        }

        public void WatchForProcessStart()
        {
            try
            {
                while (true)
                {
                    if (_currentOsuProcess == null || _currentOsuProcess.SafeHasExited())
                    {
                        _currentOsuProcess = null;
                        foreach (var process in Process.GetProcesses())
                        {
                            if (process.ProcessName == "osu!")
                            {
                                _currentOsuProcess = process;
                                break;
                            }
                        }
                        if (_currentOsuProcess != null)
                        {
                            if (Inject(!_injectedAtleastOnce))
                                _injectedAtleastOnce = true;
                        }
                    }

                    while (_pauseProcessTracking)
                    {
                        Thread.Sleep(1000);
                    }
                    Thread.Sleep(2000);
                }
            }
            catch (ThreadAbortException ex)
            {
            }
        }

        private void CopyFreeType()
        {
            var osuFolderDirectory = _settings.Get<string>(_names.MainOsuDirectory);
            if (Directory.Exists(osuFolderDirectory))
            {
                var newFreeTypeLocation = Path.Combine(osuFolderDirectory, "FreeType.dll");
                if (File.Exists(newFreeTypeLocation))
                    return;

                File.Copy(GetFullFreeTypeLocation(), newFreeTypeLocation);
            }
        }

        private bool Inject(bool showErrors = false)
        {
            DllInjector dllInjector = DllInjector.GetInstance;
            var result = dllInjector.Inject("osu!", GetFullDllLocation());
            if (result != DllInjectionResult.Success)
            {
                string message = "";
                switch (result)
                {
                    case DllInjectionResult.GameProcessNotFound:
                        message = "osu! is not running yet - restart StreamCompanion after starting it!";
                        break;
                    case DllInjectionResult.DllNotFound:
                        message =
                            "Could not find osuOverlay file to add to osu!... this shouldn't happen, if it does(you see this message) please report this.";
                        break;
                    case DllInjectionResult.InjectionFailed:
                        message =
                            "Could not add overlay to osu! most likely SC doesn't have enough premissions - restart SC as administrator and try again. If that doesn't solve it - please report ";
                        break;
                }
                if (showErrors && result != DllInjectionResult.GameProcessNotFound)
                    MessageBox.Show(message, "StreamCompanion - ingameOverlay Error!", MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                _logger?.Log(message, LogLevel.Basic);
                return false;
            }

            return true;
        }

        private string GetFilesFolder() => Path.Combine(_saver.SaveDirectory, FilesFolder);

        private string GetFullDllLocation() => Path.Combine(GetFilesFolder(), "osuOverlay.dll");

        private string GetFullFreeTypeLocation() => Path.Combine(GetFilesFolder(), "FreeType.dll");

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }

        public void Free()
        {
            _overlaySettings?.Dispose();
        }

        public UserControl GetUiSettings()
        {
            if (_overlaySettings == null || _overlaySettings.IsDisposed)
            {
                _overlaySettings = new IngameOverlaySettings(_settings);
            }
            return _overlaySettings;
        }

        public void SetNewMap(MapSearchResult map)
        {
            try
            {
                _pauseProcessTracking = map.Action == OsuStatus.Playing;
            }
            catch
            {
                // ignored
            }
        }

        public void Dispose()
        {
            _currentOsuProcess?.Dispose();
            _overlaySettings?.Dispose();
            _workerThread?.Abort();
        }
    }
}