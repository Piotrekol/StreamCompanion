using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using CollectionManager.Annotations;
using osu_StreamCompanion.Code.Core.DataTypes;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class OutputPattern : EventArgs, INotifyPropertyChanged, ICloneable
    {
        private static readonly List<string> _memoryFormatTokens = new List<string>
        {
            "!acc!", "!300!", "!100!", "!50!", "!miss!", "!time!", "!combo!", "!comboMax!", "!PpIfMapEndsNow!", "!PpIfRestFced!", "!AccIfRestFced!"
        };

        public ReadOnlyCollection<string> MemoryFormatTokens => _memoryFormatTokens.AsReadOnly();
        private bool _isMemoryFormat;
        private OsuStatus _saveEvent;
        private string _pattern;
        private string _name;
        public Dictionary<string, string> Replacements;
        [DisplayName("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        [DisplayName("Pattern")]
        public string Pattern
        {
            get { return _pattern; }
            set
            {
                if (value == _pattern) return;
                _pattern = value;
                IsMemoryFormat = MemoryFormatTokens.Any(value.Contains);
                OnPropertyChanged(nameof(Pattern));
            }
        }

        [Editable(false)]
        [DisplayName("Event")]
        public string SaveEventStr
        {
            get
            {
                switch (SaveEvent)
                {
                    case OsuStatus.All: return "All";
                    case OsuStatus.Playing: return "Playing";
                    case OsuStatus.Editing: return "Editing";
                    case OsuStatus.Listening: return "Listening";
                    case OsuStatus.Watching: return "Watching";
                    case OsuStatus.Null: return "Never";
                    default: return "Unknown";
                }
            }
        }

        [Browsable(false)]
        public OsuStatus SaveEvent
        {
            get { return _saveEvent; }
            set
            {
                if (value == _saveEvent) return;
                _saveEvent = value;
                OnPropertyChanged(nameof(SaveEvent));
                OnPropertyChanged(nameof(SaveEventStr));
            }
        }
        [Browsable(false)]
        [DisplayName("Memory format")]
        public bool IsMemoryFormat { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetFormatedPattern()
        {
            if (Replacements != null)
            {
                string toFormat = this.Pattern;
                foreach (var r in Replacements)
                {
                    toFormat = toFormat.Replace(r.Key, r.Value);
                }
                return toFormat;
            }
            return string.Empty;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}