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
using StreamCompanionTypes.Attributes;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;
using StreamCompanionTypes.Interfaces.Sources;
using System.Windows.Media.Animation;

namespace ClickCounter
{
    [SCPlugin(Name, "Counts keyboard and/or mouse clicks", Consts.SCPLUGIN_AUTHOR, Consts.SCPLUGIN_BASEURL)]
    public class ClickCounter : ApplicationContext, IPlugin, ISettingsSource, IDisposable, ITokensSource
    {
        public const string Name = "Click Counter";
        private readonly SettingNames _names = SettingNames.Instance;
        private ISettings _settings;
        private ClickCounterSettings _frmSettings;
        private KeyboardListener _keyboardListener;
        private MouseListener _mouseListener;
        private ISaver _saver;
        private List<Lazy<IHighFrequencyDataConsumer>> _highFrequencyDataConsumers;
        private Tokens.TokenSetter _tokenSetter;
        private Thread _hooksThread = null;
        private Configuration configuration;
        private KeyEntry RMouseKeyEntry;
        private KeyEntry LMouseKeyEntry;

        KeysPerX _keysPerX = new KeysPerX();
        private ILogger _logger;

        public string SettingGroup { get; } = "Click counter";

        public ClickCounter(ILogger logger, ISaver saver, ISettings settings, List<Lazy<IHighFrequencyDataConsumer>> consumers)
        {
            _logger = logger;
            _saver = saver;
            _settings = settings;
            _highFrequencyDataConsumers = consumers;
            Load();

            _tokenSetter = Tokens.CreateTokenSetter(Name);
            if (configuration.ResetKeysOnStartup)
                ResetKeys();
            HookAll();
            if (configuration.KPXEnabled)
            {
                _keysPerX.Start();
            }

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

        private void UpdateValue(KeyEntry key)
        {
            _tokenSetter(key.TokenName, key.Count, TokenType.Live);

            _highFrequencyDataConsumers.ForEach(h =>
                h.Value.Handle(key.Name, key.Count.ToString())
            );
        }

        private void HookMouse()
        {
            if (configuration.MouseEnabled && _mouseListener == null)
            {
                AddMouseKeys();
                _mouseListener = new MouseListener();
                _mouseListener.OnLeftMouseDown += _mouseListener_OnLeftMouseDown;
                _mouseListener.OnRightMouseDown += MouseListener_OnRightMouseDown;
                _mouseListener.Hook();
                _logger.Log(">Mouse hooked!", LogLevel.Debug);
            }
        }

        private void AddMouseKeys()
        {
            if (!configuration.KeyEntries.Any(k => k.Code == (int)MouseMessages.WM_LBUTTONDOWN))
            {
                configuration.KeyEntries.Add(new KeyEntry
                {
                    Code = (int)MouseMessages.WM_LBUTTONDOWN,
                    Name = "LeftMouseButton"
                });
            }
            if (!configuration.KeyEntries.Any(k => k.Code == (int)MouseMessages.WM_RBUTTONDOWN))
            {
                configuration.KeyEntries.Add(new KeyEntry
                {
                    Code = (int)MouseMessages.WM_RBUTTONDOWN,
                    Name = "RightMouseButton"
                });
            }
        }

        private void MouseListener_OnRightMouseDown(object sender, EventArgs e)
        {
            RMouseKeyEntry ??= configuration.KeyEntries.First(k => k.Code == (int)MouseMessages.WM_RBUTTONDOWN);
            RMouseKeyEntry.Count++;
            UpdateValue(RMouseKeyEntry);
        }

        private void _mouseListener_OnLeftMouseDown(object sender, EventArgs e)
        {
            LMouseKeyEntry ??= configuration.KeyEntries.First(k => k.Code == (int)MouseMessages.WM_LBUTTONDOWN);
            LMouseKeyEntry.Count++;
            UpdateValue(LMouseKeyEntry);
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
            var key = GetKeyByVKCode(args.VKCode);
            if (key != null && key.Pressed)
            {
                key.Count++;
                UpdateValue(key);
                key.Pressed = false;
                _keysPerX?.AddToKeys();
            }
        }

        private void _keyboardListener_KeyDown(object sender, RawKeyEventArgs args)
        {
            var key = GetKeyByVKCode(args.VKCode);
            if (key != null)
                key.Pressed = true;
        }

        public KeyEntry GetKeyByVKCode(int vkCode)
        {
            return configuration.KeyEntries.FirstOrDefault(k => k.Code == vkCode);
        }
        public void Free()
        {
            _frmSettings.checkBox_ResetOnRestart.CheckedChanged -= CheckBox_ResetOnRestart_CheckedChanged;
            _frmSettings.checkBox_EnableKPX.CheckedChanged -= CheckBox_EnableKPX_CheckedChanged;
            _frmSettings.checkBox_enableMouseHook.CheckedChanged -= CheckBox_enableMouseHook_CheckedChanged;
            _frmSettings.checkBox_enableKeyboardHook.CheckedChanged -= CheckBox_enableKeyboardHook_CheckedChanged;
            _frmSettings.Dispose();
        }
        private void Load()
        {
            configuration = _settings.GetConfiguration<Configuration>(_names.ClickCounter);
            return;
        }

        private void ResetKeys()
        {
            foreach (var keyEntry in configuration.KeyEntries)
            {
                keyEntry.Count = 0;
                keyEntry.Pressed = false;
                UpdateValue(keyEntry);
            }
        }
        public object GetUiSettings()
        {
            SaveKeysToSettings();
            if (_frmSettings == null || _frmSettings.IsDisposed)
            {
                _frmSettings = new ClickCounterSettings(configuration);
                _frmSettings.checkBox_ResetOnRestart.CheckedChanged += CheckBox_ResetOnRestart_CheckedChanged;
                _frmSettings.checkBox_EnableKPX.CheckedChanged += CheckBox_EnableKPX_CheckedChanged;
                _frmSettings.checkBox_enableMouseHook.CheckedChanged += CheckBox_enableMouseHook_CheckedChanged;
                _frmSettings.checkBox_enableKeyboardHook.CheckedChanged += CheckBox_enableKeyboardHook_CheckedChanged;
            }
            _frmSettings.checkBox_ResetOnRestart.Checked = configuration.ResetKeysOnStartup;
            _frmSettings.checkBox_enableMouseHook.Checked = configuration.MouseEnabled;
            _frmSettings.checkBox_resetOnPlay.Checked = configuration.ResetKeyCountsOnEachPlay;
            return _frmSettings;
        }

        private void CheckBox_enableKeyboardHook_CheckedChanged(object sender, EventArgs e)
        {
            configuration.KeyboardEnabled = _frmSettings.checkBox_enableKeyboardHook.Checked;
            if (configuration.KeyboardEnabled)
            {
                HookKeyboard();
            }
            else
            {
                UnHookKeyboard();
            }
        }

        private void CheckBox_enableMouseHook_CheckedChanged(object sender, EventArgs e)
        {
            configuration.MouseEnabled = _frmSettings.checkBox_enableMouseHook.Checked;
            if (configuration.MouseEnabled)
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
            configuration.KPXEnabled = _frmSettings.checkBox_EnableKPX.Checked;
            if (configuration.KPXEnabled)
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
            configuration.ResetKeysOnStartup = _frmSettings.checkBox_ResetOnRestart.Checked;
        }

        private void SaveKeysToSettings()
        {
            _settings.SaveConfiguration(_names.ClickCounter, configuration);
        }

        public new void Dispose()
        {
            UnHookAll();
            SaveKeysToSettings();
        }

        public Task CreateTokensAsync(IMapSearchResult map, CancellationToken cancellationToken)
        {
            if ((map.SearchArgs.EventType == OsuEventType.MapChange ||
                 map.SearchArgs.EventType == OsuEventType.PlayChange) &&
                configuration.ResetKeyCountsOnEachPlay)
            {
                ResetKeys();
            }

            foreach (var keyEntry in configuration.KeyEntries)
            {
                _tokenSetter(keyEntry.TokenName, keyEntry.Count, TokenType.Live);
            }

            return Task.CompletedTask;
        }

    }
}
