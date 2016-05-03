using System;
using System.ComponentModel;

namespace osu_StreamCompanion.Code.Windows
{
    /// <summary>
    /// display information for main window
    /// </summary>
    public class MainWindowUpdater : INotifyPropertyChanged
    {
        private string _beatmapsLoaded;
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public string BeatmapsLoaded
        {
            get { return _beatmapsLoaded; }
            set
            {

                _beatmapsLoaded = value;
                RaisePropertyChanged("BeatmapsLoaded");
            }
        }

        private string _nowPlaying;
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public string NowPlaying
        {
            get { return _nowPlaying; }
            set
            {
                _nowPlaying = value;
                RaisePropertyChanged("NowPlaying");
            }
        }

        private string _updateText;
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public string UpdateText
        {
            get { return _updateText; }
            set
            {
                _updateText = value;
                RaisePropertyChanged("UpdateText");
            }
        }

        private string _ircStatus;
        [System.Reflection.Obfuscation(Feature = "renaming")]
        public string IrcStatus
        {
            get { return _ircStatus; }
            set
            {
                _ircStatus = value;
                RaisePropertyChanged("IrcStatus");
            }
        }
        public MainWindowUpdater()
        {
            BeatmapsLoaded = "Beatmaps not loaded";
            NowPlaying = "Nothing is playing at the moment";
            UpdateText = string.Format("No updates found ({0})", "v121212.13");
            IrcStatus = "Twitch bot not connected";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event EventHandler OnUpdateTextClicked;
        public void UpdateTextClicked(object sender, EventArgs e)
        {
            if (OnUpdateTextClicked != null) OnUpdateTextClicked(sender, null);
        }
    }
}
