using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using IrcDotNet;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.IrcBot.Bot
{

    class IrcBot : IDisposable
    {
        private Dictionary<string, string> _commands = new Dictionary<string, string>();

        private void OnCommandsChanged()
        {
            if (_commands.ContainsKey("!np"))
                _client.LocalUser.SendMessage(_client.Channels[0], _commands["!np"]);
        }

        private ILogger _loggger;
        private TwitchIrcClient _client;
        public IrcBot(ILogger logger)
        {
            _loggger = logger;

        }

        public void setCommands(Dictionary<string, string> commands)
        {
            lock (commands)
            {
                _commands = commands;
            }
            OnCommandsChanged();
        }

        private void IrcClient_Registered(object sender, EventArgs e)
        {
            var client = (IrcClient)sender;

            client.LocalUser.NoticeReceived += IrcClient_LocalUser_NoticeReceived;
            client.LocalUser.MessageReceived += IrcClient_LocalUser_MessageReceived;
            client.LocalUser.JoinedChannel += IrcClient_LocalUser_JoinedChannel;
            client.LocalUser.LeftChannel += IrcClient_LocalUser_LeftChannel;
        }

        private void IrcClient_LocalUser_LeftChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;

            //e.Channel.UserJoined -= IrcClient_Channel_UserJoined;
            //e.Channel.UserLeft -= IrcClient_Channel_UserLeft;
            e.Channel.MessageReceived -= IrcClient_Channel_MessageReceived;
            e.Channel.NoticeReceived -= IrcClient_Channel_NoticeReceived;

            Console.WriteLine("You left the channel {0}.", e.Channel.Name);
        }

        private void IrcClient_LocalUser_JoinedChannel(object sender, IrcChannelEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;

            //e.Channel.UserJoined += IrcClient_Channel_UserJoined;
            //e.Channel.UserLeft += IrcClient_Channel_UserLeft;
            e.Channel.MessageReceived += IrcClient_Channel_MessageReceived;
            e.Channel.NoticeReceived += IrcClient_Channel_NoticeReceived;

            Console.WriteLine("You joined the channel {0}.", e.Channel.Name);
        }

        private void IrcClient_Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;

            Console.WriteLine("[{0}] Notice: {1}.", channel.Name, e.Text);
        }

        private void IrcClient_Channel_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;
            if (e.Source is IrcUser)
            {
                // Read message.
                Console.WriteLine("[{0}]({1}): {2}.", channel.Name, e.Source.Name, e.Text);
                var splited = e.Text.Split(new[] { ' ' }, 2);
                var command = splited[0];
                if (_commands.ContainsKey(command))
                {
                    _client.LocalUser.SendMessage(channel, _commands[command]);
                }
            }
            else
            {
                Console.WriteLine("[{0}]({1}) Message: {2}.", channel.Name, e.Source.Name, e.Text);
            }
        }


        private void IrcClient_LocalUser_MessageReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;

            if (e.Source is IrcUser)
            {
                // Read message.
                Console.WriteLine("({0}): {1}.", e.Source.Name, e.Text);

            }
            else
            {
                Console.WriteLine("({0}) Message: {1}.", e.Source.Name, e.Text);
            }
        }

        private void IrcClient_LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;
            Console.WriteLine("Notice: {0}.", e.Text);
        }

        private void Error(String message, params string[] vals)
        {
            _loggger.Log(message, LogLevel.Basic, vals);
        }
        public void Start(string channel, IrcUserRegistrationInfo userInfo)
        {
            var server = "irc.twitch.tv";
            _loggger.Log("Starting to connect to twitch as {0}.", LogLevel.Basic, userInfo.NickName);

            _client = new TwitchIrcClient();
            _client.FloodPreventer = new IrcStandardFloodPreventer(4, 2000);
            _client.Registered += IrcClient_Registered;
            using (var registeredEvent = new ManualResetEventSlim(false))
            {
                using (var connectedEvent = new ManualResetEventSlim(false))
                {
                    _client.Connected += (sender2, e2) => connectedEvent.Set();
                    _client.Registered += (sender2, e2) => registeredEvent.Set();
                    _client.Connect(server, false, userInfo);
                    if (!connectedEvent.Wait(10000))
                    {
                        Error("Connection to '{0}' timed out.", server);
                        return;
                    }
                }
                _loggger.Log("Now connected to '{0}'.", LogLevel.Basic, server);
                if (!registeredEvent.Wait(10000))
                {
                    Error("Could not register to '{0}'.", server);
                    return;
                }
            }
            _loggger.Log("Now registered to '{0}' as '{1}'.", LogLevel.Basic, server, userInfo.NickName);
            _client.SendRawMessage("JOIN " + "#" + channel);
        }

        public void Dispose()
        {
            if (_client != null)
            {
                if (_client.IsConnected)
                    _client.Disconnect();

                (_client).Dispose();
            }
        }
    }
}
