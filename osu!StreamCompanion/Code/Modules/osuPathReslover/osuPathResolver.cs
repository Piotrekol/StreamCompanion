using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using osu_StreamCompanion.Code.Misc;
using StreamCompanionTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace osu_StreamCompanion.Code.Modules.osuPathReslover
{
    public sealed class OsuPathResolver : ISettingsSource, IModule, IDisposable
    {
        private readonly SettingNames _names = SettingNames.Instance;

        private Process[] _processes;
        private ISettings _settings;
        private OsuPathResolverSettings _frmSettings;
        private ILogger _logger;

        public string SettingGroup { get; } = "General";
        public bool OsuIsRunning
        {
            get
            {
                try
                {
                    _processes = Process.GetProcessesByName("osu!");
                    return _processes.Length > 0;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool Started { get; set; }

        public OsuPathResolver(ILogger logger, ISettings settings)
        {
            _logger = logger;
            _settings = settings;
            Start(logger);
        }

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;

            if (LoadOsuDir() == "" || !Directory.Exists(LoadOsuDir()))
            {
                string osuRunningDir = GetRunningOsuDir();
                if (Directory.Exists(osuRunningDir))
                    SaveOsuDir(osuRunningDir);
                else
                {
                    SaveOsuDir(GetManualOsuDir());
                }
            }
        }

        private void Log(string text, params string[] vals)
        {
            _logger.Log(text, LogLevel.Debug, vals);
        }
        public void Free()
        {
            _frmSettings.Dispose();
        }

        public object GetUiSettings()
        {
            if (_frmSettings == null || _frmSettings.IsDisposed)
            {
                _frmSettings = new OsuPathResolverSettings();
                _frmSettings.textBox_osuDir.Click += TextBox_osuDir_Click;
                _frmSettings.button_AutoDetect.Click += Button_AutoDetect_Click;
            }
            _frmSettings.textBox_osuDir.Text = LoadOsuDir();

            return _frmSettings;
        }

        private void Button_AutoDetect_Click(object sender, EventArgs e)
        {
            string dir = GetRunningOsuDir();
            _frmSettings.textBox_osuDir.Text = dir;
            SaveOsuDir(dir);

        }
        private void TextBox_osuDir_Click(object sender, EventArgs e)
        {
            string dir = GetManualOsuDir();
            _frmSettings.textBox_osuDir.Text = dir != string.Empty ? dir : _frmSettings.textBox_osuDir.Text;
            SaveOsuDir(dir);

        }
        public string GetRunningOsuDir()
        {
            if (OsuIsRunning)
            {
                try
                {
                    string dir = _processes[0].Modules[0].FileName;
                    dir = dir.Remove(dir.LastIndexOf('\\'));
                    return dir;
                }
                catch (Exception e) //Access denied
                {
                    Log("ERROR: could not get directory from running osu! | {0}", e.Message);
                }
            }
            else if (OperatingSystem.IsWindows())
            {
                try
                {
                    using (RegistryKey osureg = Registry.ClassesRoot.OpenSubKey("osu\\DefaultIcon"))
                    {
                        if (osureg != null)
                        {
                            string osukey = osureg.GetValue(null).ToString();
                            var osupath = osukey.Remove(0, 1);
                            osupath = osupath.Remove(osupath.Length - 11);
                            return osupath;
                        }
                    }
                }
                catch (Exception e)
                {
                    Log("ERROR: could not get directory from registry key | {0}", e.Message);
                }

            }
            return string.Empty;
        }
        private string GetManualOsuDir()
        {
            var directory = SelectDirectory("Where is your osu! folder located at?");

            if (directory == string.Empty || File.Exists(directory + @"\osu!.db"))
            {
                return directory;
            }
            MessageBox.Show(@"This isn't osu! folder", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return string.Empty;
        }

        private string SelectDirectory(string text)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            //set description and base folder for browsing

            dialog.ShowNewFolderButton = true;
            dialog.Description = text;
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            if (dialog.ShowDialog() == DialogResult.OK && Directory.Exists((dialog.SelectedPath)))
            {
                return dialog.SelectedPath;
            }
            return string.Empty;
        }

        private void SaveOsuDir(string dir)
        {
            if (dir != string.Empty)
                _settings.Add(_names.MainOsuDirectory.Name, dir, true);
        }

        private string LoadOsuDir()
        {
            return _settings.Get<string>(_names.MainOsuDirectory);
        }

        public void Dispose()
        {
            _frmSettings?.Dispose();
        }
    }
}
