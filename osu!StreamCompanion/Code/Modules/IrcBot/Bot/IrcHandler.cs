using System;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Threading;
using IrcDotNet;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Interfeaces;

namespace osu_StreamCompanion.Code.Modules.IrcBot.Bot
{

    public abstract class IrcHandler : IDisposable
    {

        private TwitchIrcClient _client;
        protected IrcHandler()
        {

        }

        protected void SendMessage(IIrcMessageTarget ircTarget, string text)
        {
            _client.LocalUser.SendMessage(ircTarget, text);
        }
        private void IrcClient_Registered(object sender, EventArgs e)
        {
            var client = (IrcClient)sender;

            client.LocalUser.NoticeReceived += IrcClient_LocalUser_NoticeReceived;
            //client.LocalUser.MessageReceived += IrcClient_LocalUser_MessageReceived;
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
            Info("Twitch bot joined {0}", e.Channel.Name);
        }

        private void IrcClient_Channel_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var channel = (IrcChannel)sender;

            Console.WriteLine("[{0}] Notice: {1}.", channel.Name, e.Text);
        }



        private void IrcClient_LocalUser_NoticeReceived(object sender, IrcMessageEventArgs e)
        {
            var localUser = (IrcLocalUser)sender;
            Console.WriteLine("Notice: {0}.", e.Text);
        }


        private bool ConnectionDataIsValid(IrcUserRegistrationInfo userInfo)
        {
            return
                !(string.IsNullOrWhiteSpace(userInfo.NickName) || string.IsNullOrWhiteSpace(userInfo.Password) ||
                  string.IsNullOrWhiteSpace(userInfo.UserName));
        }
        protected void Start(string channel, IrcUserRegistrationInfo userInfo)
        {
            if (_client != null)
            {
                CloseConnection();
                _client.Dispose();
                _client = null;
            }
            if (!ConnectionDataIsValid(userInfo))
            {
                Error("Invalid Twitch connection data(check your login data in options)", true);
                return;
            }


            var server = "irc.twitch.tv";
            Info("Starting to connect to Twitch as {0}", userInfo.NickName);

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
                    if (!connectedEvent.Wait(5000))
                    {
                        Error("Connection to '{0}' timed out", true, server);
                        return;
                    }
                }

                if (!registeredEvent.Wait(5000))
                {
                    Error("Could not register to '{0}' (Make sure that your username and password is correct)", true, server);
                    return;
                }
            }
            Info("Now registered to '{0}' as '{1}'.. connecting to channel {2}", server, userInfo.NickName, "#" + channel);
            _client.SendRawMessage("JOIN " + "#" + channel);
        }

        protected abstract void IrcClient_Channel_MessageReceived(object sender, IrcMessageEventArgs e);
        protected abstract void Error(String message, bool isCritical, params string[] vals);
        protected abstract void Info(String message, params string[] vals);


        protected void CloseConnection()
        {
            if (_client != null)
                if (_client.IsConnected)
                    _client.Disconnect();
        }
        public void Dispose()
        {
            if (_client != null)
            {
                CloseConnection();

                (_client).Dispose();
            }
        }

    }
}
