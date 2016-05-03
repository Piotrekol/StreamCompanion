using System;
using IrcDotNet;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Windows;

namespace osu_StreamCompanion.Code.Modules.IrcBot.Bot
{
    public class IrcBot : IrcHandler, IMainWindowUpdater
    {
        private MapSearchResult _mapResult = new MapSearchResult();
        private MainWindowUpdater _mainWindowHandle;
        private bool displayingError = false;
        private ILogger _logger;

        public IrcBot(ILogger logger) : base()
        {
            _logger = logger;
        }


        public new void Start(string channel, IrcUserRegistrationInfo userInfo)
        {
            displayingError = false;
            _mainWindowHandle.IrcStatus = "IRC bot is connecting...";
            base.Start(channel, userInfo);
        }
        protected override void Error(string message, bool isCritical, params string[] vals)
        {
            displayingError = true;
            _logger.Log(message, LogLevel.Basic, vals);
            SetWindowText(string.Format(message, vals));
            if (isCritical)
            {
                base.CloseConnection();
            }
        }
        protected override void Info(string message, params string[] vals)
        {
            if (displayingError) return;
            _logger.Log(message, LogLevel.Basic, vals);
            SetWindowText(string.Format(message, vals));
        }

        private void SetWindowText(string text)
        {
            _mainWindowHandle.IrcStatus = text;

        }
        protected override void IrcClient_Channel_MessageReceived(object sender, IrcDotNet.IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;
            if (e.Source is IrcUser)
            {
                Console.WriteLine("[{0}]({1}): {2}", channel.Name, e.Source.Name, e.Text);

                var splited = e.Text.Split(new[] { ' ' }, 2);
                HandleCommand(channel, splited[0]);
            }
        }

        private void HandleCommand(IrcChannel source, string command)
        {
            if (_mapResult.Commands.ContainsKey(command))
            {
                SendMessage(source, _mapResult.Commands[command]);
            }
        }

        public void SetLastMapResult(MapSearchResult map)
        {
            lock (_mapResult)
            {
                _mapResult = map;
            }
        }

        public void GetMainWindowHandle(MainWindowUpdater mainWindowHandle)
        {
            _mainWindowHandle = mainWindowHandle;
        }


    }
}
