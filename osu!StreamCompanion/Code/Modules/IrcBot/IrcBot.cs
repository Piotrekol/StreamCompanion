using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Windows;


namespace osu_StreamCompanion.Code.Modules.IrcBot
{
    class IrcBot : IModule, ISettingsProvider, ISqliteUser, IMapDataGetter, IMainWindowUpdater,IDisposable
    {
        private SqliteControler _sqliteHandle;
        private Settings _settings;
        private Thread BotThread;
        private Bot.IrcBot Bot;
        private MainWindowUpdater _mainWindowHandle;

        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            var username = _settings.Get("IrcUsername", "").ToLower();
            var channel = _settings.Get("IrcChannel", "").ToLower().Trim('#');
            var password = _settings.Get("IrcPassword", "");
            var a = new IrcDotNet.IrcUserRegistrationInfo()
            {
                NickName = username,
                Password = password,
                UserName = username
            };
            BotThread = new Thread(() =>
            {
                Bot = new Bot.IrcBot(logger);
                Bot.GetMainWindowHandle(_mainWindowHandle);
                Bot.Start(channel, a);
            });
            BotThread.Start();
        }

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public string SettingGroup { get; } = "IRC Bot";
        public void Free()
        {
            return;
        }

        public UserControl GetUiSettings()
        {
            return new UserControl();
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
