using System;
using System.ComponentModel;
using StreamCompanionTypes.Interfaces;

namespace osu_StreamCompanion.Code.Windows
{
    /// <summary>
    /// display information for main window
    /// </summary>
    public class MainWindowUpdater : INotifyPropertyChanged, IMainWindowModel
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

        private string _version;
        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                RaisePropertyChanged("Version");
            }
        }

        public MainWindowUpdater()
        {
            BeatmapsLoaded = "Beatmaps not loaded";
            NowPlaying = "Nothing is playing at the moment";
            UpdateText = string.Format("No updates found ({0})", "v121212.13");
            Version = Program.ScVersion;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event EventHandler OnUpdateTextClicked;
        public void UpdateTextClicked(object sender, EventArgs e)
        {
            OnUpdateTextClicked?.Invoke(sender, null);
        }
    }
}
