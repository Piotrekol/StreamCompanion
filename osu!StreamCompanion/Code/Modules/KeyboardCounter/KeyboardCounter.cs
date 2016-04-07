using System;
using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.KeyboardCounter
{
    public class KeyboardCounter : ISettingsProvider, IModule, ISaveRequester, IDisposable, IMapDataReplacements
    {
        private Settings _settings;
        private KeyboardCounterSettings _frmSettings;
        private KeyboardListener _keyboardListener;
        private ISaver _saver;
        private readonly Dictionary<int, bool> _keyPressed = new Dictionary<int, bool>();
        private readonly List<int> _keyList = new List<int>();
        private readonly IDictionary<int, int> _keyCount = new Dictionary<int, int>();
        private readonly IDictionary<int, string> _filenames = new Dictionary<int, string>();
        public string SettingGroup { get; } = "Keyboard";
        public void Hook()
        {
            if (_keyboardListener != null) return;
            _keyboardListener = new KeyboardListener();
            _keyboardListener.KeyDown += _keyboardListener_KeyDown;
            _keyboardListener.KeyUp += _keyboardListener_KeyUp;
        }

        private void Unhook()
        {
            if (_keyboardListener == null) return;
            _keyboardListener.Dispose();
            _keyboardListener = null;
        }
        private void _keyboardListener_KeyUp(object sender, RawKeyEventArgs args)
        {
            if (_keyList.Contains(args.VKCode))
            {
                if (_keyPressed[args.VKCode])
                {
                    _keyCount[args.VKCode]++;
                    _saver.Save(_filenames[args.VKCode], _keyCount[args.VKCode].ToString());
                    _keyPressed[args.VKCode] = false;
                    _keysPerX?.AddToKeys();
                }
            }
        }

        private void _keyboardListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            if (_keyList.Contains(args.VKCode))
            {
                _keyPressed[args.VKCode] = true;
            }
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;

            Load();
        }

        public void Free()
        {
            _frmSettings.Dispose();
        }

        private void Load()
        {
            var keys = _settings.Geti("keyList");
            var keyfilenames = _settings.Get("keyNames");
            var keyCounts = _settings.Geti("keyCounts");
            if (keys.Count > 0)
            {
                for (int i = 0; i < keys.Count; i++)
                {
                    int keyHash = keys[i];
                    if (!_keyList.Contains(keyHash))
                    {
                        int keyCount = keyCounts[i];
                        _keyList.Add(keyHash);
                        _keyCount[keyHash] = keyCount;
                        _keyPressed[keyHash] = false;
                        _filenames[keyHash] = keyfilenames[i];
                    }
                }
            }
        }

        private void ResetKeys()
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                _keyCount[_keyList[i]] = 0;
                _saver.Save(_filenames[_keyList[i]], "0");
            }

        }
        public UserControl GetUiSettings()
        {
            SaveKeysToSettings();
            if (_frmSettings == null || _frmSettings.IsDisposed)
            {
                _frmSettings = new KeyboardCounterSettings(_settings);
                _frmSettings.checkBox_ResetOnRestart.CheckedChanged += CheckBox_ResetOnRestart_CheckedChanged;
                _frmSettings.checkBox_EnableKPX.CheckedChanged += CheckBox_EnableKPX_CheckedChanged;
            }
            _frmSettings.checkBox_ResetOnRestart.Checked = _settings.Get("ResetKeysOnRestart", false);
            _frmSettings.RefreshDataGrid();
            _frmSettings.KeysChanged += _frmSettings_KeysChanged;
            return _frmSettings;
        }

        private void CheckBox_EnableKPX_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add("EnableKPM", _frmSettings.checkBox_EnableKPX.Checked);

            if (_settings.Get("EnableKPM", false))
            {
                _keysPerX.Start();
            }
            else
            {
                _keysPerX.Stop();
            }

        }

        private void CheckBox_ResetOnRestart_CheckedChanged(object sender, EventArgs e)
        {
            _settings.Add("ResetKeysOnRestart", _frmSettings.checkBox_ResetOnRestart.Checked);
        }

        private void _frmSettings_KeysChanged(object sender, EventArgs e)
        {
            Load();
        }

        public bool Started { get; set; }
        KeysPerX _keysPerX = new KeysPerX();
        private ILogger _logger;

        public void Start(ILogger logger)
        {
            Started = true;
            _logger = logger;
            if (_settings.Get("ResetKeysOnRestart", false))
                ResetKeys();
            Hook();
            logger.Log(">keyboard hooked!", LogLevel.Debug);
            if (_settings.Get("EnableKPM", false))
            {
                _keysPerX.Start();
            }

        }

        private void SaveKeysToSettings()
        {
            var keyCounts = new List<int>();
            foreach (var k in _keyCount)
            {
                keyCounts.Add(k.Value);
            }
            _settings.Add("keyCounts", keyCounts);
        }

        private void Save(string filename, int count)
        {
            _saver.Save(filename, count.ToString());
        }
        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }

        public void Dispose()
        {
            Unhook();
        }

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            var ret = new Dictionary<string, string>();
            for (int i = 0; i < _keyList.Count; i++)
            {
                ret.Add($"!{_filenames[_keyList[i]]}!", _keyCount[_keyList[i]].ToString());
            }
            return ret;
        }
    }
}
