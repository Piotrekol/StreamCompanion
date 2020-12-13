using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StreamCompanion.Common;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;

namespace ClickCounter
{
    public class ClickCounter : ApplicationContext, IPlugin, ISettingsSource, IDisposable, ITokensSource
    {
        private readonly SettingNames _names = SettingNames.Instance;
        public static ConfigEntry ResetKeyCountsOnEachPlay = new ConfigEntry("ResetKeyCountsOnEachPlay", false);
        private ISettings _settings;
        private ClickCounterSettings _frmSettings;
        private KeyboardListener _keyboardListener;
        private MouseListener _mouseListener;
        private ISaver _saver;
        private int _rightMouseCount;
        private int _leftMouseCount;
        private bool disableSavingToDisk = false;
        private readonly Dictionary<int, bool> _keyPressed = new Dictionary<int, bool>();
        private readonly List<int> _keyList = new List<int>();
        private readonly IDictionary<int, int> _keyCount = new Dictionary<int, int>();
        private readonly IDictionary<int, string> _filenames = new Dictionary<int, string>();
        private List<IHighFrequencyDataConsumer> _highFrequencyDataConsumers;
        private Tokens.TokenSetter _tokenSetter;
        private Thread _hooksThread = null;

        KeysPerX _keysPerX = new KeysPerX();
        private ILogger _logger;

        public string SettingGroup { get; } = "Click counter";

        public string Description { get; } = "";
        public string Name { get; } = "Click Counter";
        public string Author { get; } = "Piotrekol";
        public string Url { get; } = "";
        public string UpdateUrl { get; } = "";

        public ClickCounter(ILogger logger, ISaver saver, ISettings settings, IEnumerable<IHighFrequencyDataConsumer> consumers)
        {
            _logger = logger;
            _saver = saver;
            _settings = settings;
            _highFrequencyDataConsumers = consumers.ToList();

            disableSavingToDisk = _settings.Get<bool>(_names.DisableClickCounterWrite);
            Load();

            if (_settings.Get<bool>(_names.ResetKeysOnRestart))
                ResetKeys();
            HookAll();
            if (_settings.Get<bool>(_names.CfgEnableKpx))
            {
                _keysPerX.Start();
            }

            _tokenSetter = Tokens.CreateTokenSetter(Name);
        }

        public void HookAll()
        {
            _hooksThread = new Thread(() =>
            {
                HookKeyboard();
                HookMouse();
                Application.Run(this);
            });
            _hooksThread.IsBackground = false;
            _hooksThread.Priority = ThreadPriority.Highest;
            _hooksThread.Start();

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

        private void UpdateValue(string name, int value)
        {
            if (!disableSavingToDisk)
                _saver.Save(name + ".txt", value.ToString());

            _tokenSetter($"{getTokenName(name)}.txt", value, TokenType.Live);

            _highFrequencyDataConsumers.ForEach(h =>
                h.Handle(name, value.ToString())
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
            UpdateValue("m1", _rightMouseCount);
        }

        private void _mouseListener_OnLeftMouseDown(object sender, EventArgs e)
        {
            _leftMouseCount++;
            UpdateValue("m2", _leftMouseCount);
        }

        private void UnHookAll()
        {
            _logger.Log("Unhooking devices", LogLevel.Debug);
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
                    UpdateValue(name, _keyCount[args.VKCode]);

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
#pragma warning disable 618 // deprecated
            var keys = _settings.Geti(_names.KeyList.Name);
            var keyfilenames = _settings.Get(_names.KeyNames.Name);
            var keyCounts = _settings.Geti(_names.KeyCounts.Name);
#pragma warning restore 618

            _rightMouseCount = Convert.ToInt32(_settings.Get<long>(_names.RightMouseCount));
            _leftMouseCount = Convert.ToInt32(_settings.Get<long>(_names.LeftMouseCount));
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
        public object GetUiSettings()
        {
            SaveKeysToSettings();
            if (_frmSettings == null || _frmSettings.IsDisposed)
            {
                _frmSettings = new ClickCounterSettings(_settings);
                _frmSettings.checkBox_ResetOnRestart.CheckedChanged += CheckBox_ResetOnRestart_CheckedChanged;
                _frmSettings.checkBox_EnableKPX.CheckedChanged += CheckBox_EnableKPX_CheckedChanged;
                _frmSettings.checkBox_enableMouseHook.CheckedChanged += CheckBox_enableMouseHook_CheckedChanged;
                _frmSettings.checkBox_resetOnPlay.CheckedChanged += CheckBox_resetOnPlayOnCheckedChanged;
            }
            _frmSettings.checkBox_ResetOnRestart.Checked = _settings.Get<bool>(_names.ResetKeysOnRestart);
            _frmSettings.checkBox_enableMouseHook.Checked = _settings.Get<bool>(_names.EnableMouseHook);
            _frmSettings.checkBox_resetOnPlay.Checked = _settings.Get<bool>(ResetKeyCountsOnEachPlay);
            _frmSettings.SetLeftMouseCount(_leftMouseCount);
            _frmSettings.SetRightMouseCount(_rightMouseCount);
            _frmSettings.RefreshDataGrid();
            _frmSettings.KeysChanged += _frmSettings_KeysChanged;
            return _frmSettings;
        }

        private void CheckBox_resetOnPlayOnCheckedChanged(object sender, EventArgs e)
        {
            _settings.Add(ResetKeyCountsOnEachPlay.Name, _frmSettings.checkBox_resetOnPlay.Checked);
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

        private void SaveKeysToSettings()
        {
            var keyCounts = new List<int>();
            foreach (var k in _keyCount)
            {
                keyCounts.Add(k.Value);
            }
#pragma warning disable 618 // deprecated
            _settings.Add(_names.KeyCounts.Name, keyCounts);
#pragma warning restore 618
            _settings.Add(_names.RightMouseCount.Name, _rightMouseCount);
            _settings.Add(_names.LeftMouseCount.Name, _leftMouseCount);
            _logger.Log($"Saved: {string.Join(",", keyCounts)}; RM:{_rightMouseCount} LM:{_leftMouseCount}", LogLevel.Debug);
        }

        public void SetSaveHandle(ISaver saver)
        {
            _saver = saver;
        }

        public new void Dispose()
        {
            UnHookAll();
            SaveKeysToSettings();
        }
        private string getTokenName(string keyName) => $"key-{keyName}".ToLowerInvariant().RemoveWhitespace();
        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if ((map.SearchArgs.EventType == OsuEventType.MapChange ||
                 map.SearchArgs.EventType == OsuEventType.PlayChange) &&
                _settings.Get<bool>(ResetKeyCountsOnEachPlay))
            {
                ResetKeys();
            }

            for (int i = 0; i < _keyList.Count; i++)
            {
                _tokenSetter(getTokenName(_filenames[_keyList[i]]), _keyCount[_keyList[i]], TokenType.Live);
            }

            _tokenSetter(getTokenName("m1.txt"), _rightMouseCount, TokenType.Live);
            _tokenSetter(getTokenName("m2.txt"), _leftMouseCount, TokenType.Live);
            
            return Task.CompletedTask;
        }

    }
}
