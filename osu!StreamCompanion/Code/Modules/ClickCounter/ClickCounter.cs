using System;
using System.Collections.Generic;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.ClickCounter
{
    public class ClickCounter : ISettingsProvider, IModule, ISaveRequester, IDisposable, IMapDataReplacements
    { 
        private const string CfgEnableMouseHook = "HookMouse";
        private const string CfgKeyList = "keyList";
        private const string CfgKeyNames = "keyNames";
        private const string CfgKeyCounts = "keyCounts";
        private const string CfgRightMouse = "rightMouseCount";
        private const string CfgLeftMouse = "leftMouseCount";
        private const string CfgResetKeys = "ResetKeysOnRestart";
        private const string CfgEnableKpx = "EnableKPM";
        

        private Settings _settings;
        private ClickCounterSettings _frmSettings;
        private KeyboardListener _keyboardListener;
        private MouseListener _mouseListener;
        private ISaver _saver;
        private long _rightMouseCount;
        private long _leftMouseCount;
        private readonly Dictionary<int, bool> _keyPressed = new Dictionary<int, bool>();
        private readonly List<int> _keyList = new List<int>();
        private readonly IDictionary<int, int> _keyCount = new Dictionary<int, int>();
        private readonly IDictionary<int, string> _filenames = new Dictionary<int, string>();
        public string SettingGroup { get; } = "Click counter";

        public void HookAll()
        {
            HookKeyboard();
            HookMouse();
        }

        private void HookKeyboard()
        {
            if (_keyboardListener == null)
            {
                _keyboardListener = new KeyboardListener();
                _keyboardListener.KeyDown += _keyboardListener_KeyDown;
                _keyboardListener.KeyUp += _keyboardListener_KeyUp;
                _logger.Log(">Keyboard hooked!", LogLevel.Debug);
            }
        }

        private void HookMouse()
        {
            if (_settings.Get(CfgEnableMouseHook, false) && _mouseListener == null)
            {
                _mouseListener = new MouseListener();
                _mouseListener.OnLeftMouseDown += _mouseListener_OnLeftMouseDown;
                _mouseListener.OnRightMouseDown += MouseListener_OnRightMouseDown;
                _mouseListener.Hook();
                _logger.Log(">Mouse hooked!", LogLevel.Debug);
            }
        }

        private void MouseListener_OnRightMouseDown(object sender, EventArgs e)
        {
            _rightMouseCount++;
            _saver.Save("M1.txt", _rightMouseCount.ToString());
        }

        private void _mouseListener_OnLeftMouseDown(object sender, EventArgs e)
        {
            _leftMouseCount++;
            _saver.Save("M2.txt", _leftMouseCount.ToString());
        }

        private void UnHookAll()
        {
            UnHookKeyboard();
            UnHookMouse();
        }

        private void UnHookKeyboard()
        {
            if (_keyboardListener != null)
            {
                _keyboardListener.Dispose();
                _keyboardListener = null;
            }
        }

        private void UnHookMouse()
        {
            if (_mouseListener != null)
            {
                _mouseListener.Dispose();
                _mouseListener = null;
            }
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
            _frmSettings.checkBox_ResetOnRestart.CheckedChanged -= CheckBox_ResetOnRestart_CheckedChanged;
            _frmSettings.checkBox_EnableKPX.CheckedChanged -= CheckBox_EnableKPX_CheckedChanged;
            _frmSettings.checkBox_enableMouseHook.CheckedChanged -= CheckBox_enableMouseHook_CheckedChanged;
            _frmSettings.KeysChanged -= _frmSettings_KeysChanged;
            _frmSettings.Dispose();
        }

        private void Load()
        {
            var keys = _settings.Geti(CfgKeyList);
            var keyfilenames = _settings.Get(CfgKeyNames);
            var keyCounts = _settings.Geti(CfgKeyCounts);

            _rightMouseCount = _settings.Get(CfgRightMouse, 0L);
            _leftMouseCount = _settings.Get(CfgLeftMouse, 0L);
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
                _frmSettings = new ClickCounterSettings(_settings);
                _frmSettings.checkBox_ResetOnRestart.CheckedChanged += CheckBox_ResetOnRestart_CheckedChanged;
                _frmSettings.checkBox_EnableKPX.CheckedChanged += CheckBox_EnableKPX_CheckedChanged;
                _frmSettings.checkBox_enableMouseHook.CheckedChanged += CheckBox_enableMouseHook_CheckedChanged;
            }
            _frmSettings.checkBox_ResetOnRestart.Checked = _settings.Get(CfgResetKeys, false);
            _frmSettings.checkBox_enableMouseHook.Checked = _settings.Get(CfgEnableMouseHook, false);
            _frmSettings.SetLeftMouseCount(_leftMouseCount);
            _frmSettings.SetRightMouseCount(_rightMouseCount);
            _frmSettings.RefreshDataGrid();
            _frmSettings.KeysChanged += _frmSettings_KeysChanged;
            return _frmSettings;
        }

        private void CheckBox_enableMouseHook_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = _frmSettings.checkBox_enableMouseHook.Checked;
            _settings.Add(CfgEnableMouseHook, enabled);

            if (enabled)
            {
                HookMouse();
            }
            else
            {
                UnHookMouse();
            }
        }

        private void CheckBox_EnableKPX_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = _frmSettings.checkBox_EnableKPX.Checked;
            _settings.Add(CfgEnableKpx, enabled);

            if (enabled)
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
            var enabled = _frmSettings.checkBox_ResetOnRestart.Checked;
            _settings.Add(CfgResetKeys, enabled);
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
            if (_settings.Get(CfgResetKeys, false))
                ResetKeys();
            HookAll();
            if (_settings.Get(CfgEnableKpx, false))
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
            _settings.Add(CfgKeyCounts, keyCounts);
            _settings.Add(CfgRightMouse, _rightMouseCount);
            _settings.Add(CfgLeftMouse, _leftMouseCount);
        }
        
        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }

        public void Dispose()
        {
            UnHookAll();
            SaveKeysToSettings();
        }

        public Dictionary<string, string> GetMapReplacements(MapSearchResult map)
        {
            var ret = new Dictionary<string, string>();
            for (int i = 0; i < _keyList.Count; i++)
            {
                ret.Add($"!{_filenames[_keyList[i]]}!", _keyCount[_keyList[i]].ToString());
            }
            ret.Add("!M1!", _rightMouseCount.ToString());
            ret.Add("!M2!", _leftMouseCount.ToString());
            return ret;
        }
    }
}
