using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.IngameOverlay
{
    class IngameOverlay : IModule, ISettingsProvider, ISaveRequester
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private Settings _settings;
        private ISaver _saver;
        public bool Started { get; set; }
        public string SettingGroup { get; } = "General";

        private const string FilesFolder = "Dlls";
        public void Start(ILogger logger)
        {
            Started = true;
            if (_settings.Get<bool>(_names.EnableIngameOverlay))
            {
                CopyFreeType();
                Inject();
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

        private void Inject()
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
                MessageBox.Show(message, "StreamCompanion - ingameOverlay Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
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

        private IngameOverlaySettings _overlaySettings;
        public UserControl GetUiSettings()
        {
            if (_overlaySettings == null || _overlaySettings.IsDisposed)
            {
                _overlaySettings = new IngameOverlaySettings(_settings);
            }
            return _overlaySettings;
        }
    }
}
