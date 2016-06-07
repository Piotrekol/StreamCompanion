using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.IrcBot
{
    class IrcBot : IModule, ISettingsProvider, ISqliteUser, IMapDataGetter, IMainWindowUpdater, IDisposable
    {
        private const string CfgEnableTwitchBot = "EnableTwitchBot";
        private const string CfgTwitchUsername = "IrcUsername";
        private const string CfgTwitchPassword = "IrcPassword";
        private const string CfgTwitchChannel = "IrcChannel";

        private SqliteControler _sqliteHandle;
        private Settings _settings;
        private Thread BotThread;
        private Bot.IrcBot Bot;
        private MainWindowUpdater _mainWindowHandle;

        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            _logger = logger;
            if (_settings.Get(CfgEnableTwitchBot, false))
            {
                Connect();
            }
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public string SettingGroup { get; } = "IRC Bot";
        public void Free()
        {
            if (_ircBotSettings != null)
            {
                _ircBotSettings.button_reconnect.Click -= Button_reconnect_Click;
                _ircBotSettings.Dispose();
                _ircBotSettings = null;
            }
        }

        private IrcBotSettings _ircBotSettings = null;
        private ILogger _logger;

        public UserControl GetUiSettings()
        {
            if (_ircBotSettings == null || _ircBotSettings.IsDisposed)
            {
                _ircBotSettings = new IrcBotSettings(_settings);
                _ircBotSettings.button_reconnect.Click += Button_reconnect_Click;
            }

            return _ircBotSettings;
        }

        private void Button_reconnect_Click(object sender, EventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            var username = _settings.Get(CfgTwitchUsername, "").ToLower();
            var channel = _settings.Get(CfgTwitchChannel, "").ToLower().Trim('#');
            var password = _settings.Get(CfgTwitchPassword, "");
            var a = new IrcDotNet.IrcUserRegistrationInfo()
            {
                NickName = username,
                Password = password,
                UserName = username
            };
            if (BotThread != null)
            {
                Bot.Dispose();
                BotThread.Abort();
            }
            BotThread = new Thread(() =>
            {
                Bot = new Bot.IrcBot(_logger);
                Bot.GetMainWindowHandle(_mainWindowHandle);
                Bot.Start(channel, a);
            });
            BotThread.Start();
        }
        public void SetSqliteControlerHandle(SqliteControler sqLiteControler)
        {
            _sqliteHandle = sqLiteControler;
        }

        public void SetNewMap(MapSearchResult map)
        {
            Bot.SetLastMapResult(map);
        }

        public void Dispose()
        {
            Bot.Dispose();
            BotThread.Abort();
        }

        public void GetMainWindowHandle(MainWindowUpdater mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
        }
    }
}
