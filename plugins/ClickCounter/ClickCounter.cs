using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ClickCounter
{
    public class ClickCounter : IPlugin, ISettingsProvider, ISaveRequester, IDisposable, ITokensProvider, IHighFrequencyDataSender
    {
        private readonly SettingNames _names = SettingNames.Instance;
        private ISettingsHandler _settings;
        private ClickCounterSettings _frmSettings;
        private KeyboardListener _keyboardListener;
        private MouseListener _mouseListener;
        private ISaver _saver;
        private long _rightMouseCount;
        private long _leftMouseCount;
        private bool disableSavingToDisk = false;
        private readonly Dictionary<int, bool> _keyPressed = new Dictionary<int, bool>();
        private readonly List<int> _keyList = new List<int>();
        private readonly IDictionary<int, int> _keyCount = new Dictionary<int, int>();
        private readonly IDictionary<int, string> _filenames = new Dictionary<int, string>();
        private List<IHighFrequencyDataHandler> _highFrequencyDataHandler;
        private Tokens.TokenSetter _tokenGetter;

        public string SettingGroup { get; } = "Click counter";

        public string Description { get; } = "";
        public string Name { get; } = "Click Counter";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";


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

        private void UpdateValue(string name, string value)
        {
            if (!disableSavingToDisk)
                _saver.Save(name + ".txt", value);

            _highFrequencyDataHandler.ForEach(h =>
                h.Handle(name, value)
            );
        }
        private void HookMouse()
        {
            if (_settings.Get<bool>(_names.EnableMouseHook) && _mouseListener == null)
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
            UpdateValue("M1", _rightMouseCount.ToString());
        }

        private void _mouseListener_OnLeftMouseDown(object sender, EventArgs e)
        {
            _leftMouseCount++;
            UpdateValue("M2", _rightMouseCount.ToString());
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

                    var name = _filenames[args.VKCode].Replace(".txt", "");
                    UpdateValue(name, _keyCount[args.VKCode].ToString());

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

        public void SetSettingsHandle(ISettingsHandler settings)
        {
            _settings = settings;
            disableSavingToDisk = _settings.Get<bool>(_names.DisableClickCounterWrite);
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
            var keys = _settings.Geti(_names.KeyList.Name);
            var keyfilenames = _settings.Get(_names.KeyNames.Name);
            var keyCounts = _settings.Geti(_names.KeyCounts.Name);

            _rightMouseCount = _settings.Get<long>(_names.RightMouseCount);
            _leftMouseCount = _settings.Get<long>(_names.LeftMouseCount);
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
            _frmSettings.checkBox_ResetOnRestart.Checked = _settings.Get<bool>(_names.ResetKeysOnRestart);
            _frmSettings.checkBox_enableMouseHook.Checked = _settings.Get<bool>(_names.EnableMouseHook);
            _frmSettings.SetLeftMouseCount(_leftMouseCount);
            _frmSettings.SetRightMouseCount(_rightMouseCount);
            _frmSettings.RefreshDataGrid();
            _frmSettings.KeysChanged += _frmSettings_KeysChanged;
            return _frmSettings;
        }

        private void CheckBox_enableMouseHook_CheckedChanged(object sender, EventArgs e)
        {
            var enabled = _frmSettings.checkBox_enableMouseHook.Checked;
            _settings.Add(_names.EnableMouseHook.Name, enabled);

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
            _settings.Add(_names.CfgEnableKpx.Name, enabled);

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
            _settings.Add(_names.ResetKeysOnRestart.Name, enabled);
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
            _logger = logger;
            if (_settings.Get<bool>(_names.ResetKeysOnRestart))
                ResetKeys();
            HookAll();
            if (_settings.Get<bool>(_names.CfgEnableKpx))
            {
                _keysPerX.Start();
            }

            _tokenGetter = Tokens.CreateTokenSetter(Name);
            Started = true;

        }

        private void SaveKeysToSettings()
        {
            var keyCounts = new List<int>();
            foreach (var k in _keyCount)
            {
                keyCounts.Add(k.Value);
            }
            _settings.Add(_names.KeyCounts.Name, keyCounts);
            _settings.Add(_names.RightMouseCount.Name, _rightMouseCount);
            _settings.Add(_names.LeftMouseCount.Name, _leftMouseCount);
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

        public void CreateTokens(MapSearchResult map)
        {
            for (int i = 0; i < _keyList.Count; i++)
            {
                _tokenGetter($"{_filenames[_keyList[i]]}", _keyCount[_keyList[i]]);
            }

            _tokenGetter("M1", _rightMouseCount);
            _tokenGetter("M2", _leftMouseCount);
        }

        public void SetHighFrequencyDataHandlers(List<IHighFrequencyDataHandler> handlers)
        {
            _highFrequencyDataHandler = handlers;
        }
    }
}
