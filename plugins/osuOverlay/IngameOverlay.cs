using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;

namespace osuOverlay
{
    public class IngameOverlay : IPlugin, ISettingsProvider, IMapDataGetter, IDisposable
    {
        private ISettingsHandler _settings;
        public string SettingGroup { get; } = "General";
        private IngameOverlaySettings _overlaySettings;
        private ILogger _logger;

        private Thread _workerThread;
        private Process _currentOsuProcess;
        private bool _pauseProcessTracking;
        private bool _injectedAtleastOnce = false;


        public string Description { get; } = "";
        public string Name { get; } = nameof(IngameOverlay);
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public IngameOverlay(ILogger logger, ISettingsHandler settings, Delegates.Exit exiter)
        {
            _logger = logger;
            _settings = settings;

            try
            {
                SetNewMap(new MapSearchResult(new MapSearchArgs("dummy")));
            }
            catch (Exception)
            {
                MessageBox.Show(
                    $"IngameOverlay plugin version is not valid for this version of StreamCompanion. {Environment.NewLine} Either update or remove it from plugins folder",
                    "osu!StreamCompanion Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                exiter("plugin version is invalid for current StreamCompanion version.");
            }

            if (_settings.Get<bool>(PluginSettings.EnableIngameOverlay))
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

                    if (_currentOsuProcess == null || SafeHasExited(_currentOsuProcess))
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
            catch (ThreadAbortException)
            {
            }
        }

        private void CopyFreeType()
        {
            var osuFolderDirectory = _settings.Get<string>(PluginSettings.MainOsuDirectory);
            if (Directory.Exists(osuFolderDirectory))
            {
                var newFreeTypeLocation = Path.Combine(osuFolderDirectory, "FreeType.dll");
                if (File.Exists(newFreeTypeLocation))
                    return;

                File.Copy(GetFullFreeTypeLocation(), newFreeTypeLocation);
            }
        }
        public bool SafeHasExited(Process process)
        {

            try
            {
                return process.HasExited;
            }
            catch
            {
                return true;
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

        private string GetFilesFolder() => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "Dlls");

        private string GetFullDllLocation() => Path.Combine(GetFilesFolder(), "osuOverlay.dll");

        private string GetFullFreeTypeLocation() => Path.Combine(GetFilesFolder(), "FreeType.dll");

        public void Free()
        {
            _overlaySettings?.Dispose();
        }

        public object GetUiSettings()
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