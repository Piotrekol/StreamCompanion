using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.osuPathReslover
{
    public class OsuPathResolver : ISettingsProvider, IModule
    {
        private Process[] _processes;
        private Settings _settings;
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

        public void Start(ILogger logger)
        {
            _logger = logger;
            Started = true;
        }
        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;

            if (LoadOsuDir() == "" || !Directory.Exists(LoadOsuDir()))
            {
                string osuRunningDir = GetRunningOsuDir();
                SaveOsuDir(osuRunningDir);
            }
        }

        public void Free()
        {
            _frmSettings.Dispose();
        }

        public UserControl GetUiSettings()
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
                    _logger.Log("ERROR: could not get directory from running osu! | {0}",LogLevel.Advanced,e.Message);
                }
            }
            else
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
                    _logger.Log("ERROR: could not get directory from registry key | {0}", LogLevel.Advanced, e.Message);
                }
                
            }
            return string.Empty;
        }
        string GetManualOsuDir()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            //set description and base folder for browsing
            var description = "Where is your osu! folder located at?";
            dialog.ShowNewFolderButton = false;
            dialog.Description = description;
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(dialog.SelectedPath + @"\osu!.db"))
                {
                    return dialog.SelectedPath;
                }
                MessageBox.Show(@"This isn't osu! folder", @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;
        }

        private void SaveOsuDir(string dir)
        {
            if (dir != string.Empty)
                _settings.Add("MainOsuDirectory", dir);
        }

        private string LoadOsuDir()
        {
            return _settings.Get("MainOsuDirectory", "");
        }
    }
}
