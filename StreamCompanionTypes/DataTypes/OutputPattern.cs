using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using CollectionManager.Annotations;
using Newtonsoft.Json;

namespace StreamCompanionTypes.DataTypes
{
    public class OutputPattern : EventArgs, INotifyPropertyChanged, ICloneable
    {
        private static readonly List<string> _memoryFormatTokens = new List<string>
        {
            "!acc!", "!300!", "!100!", "!50!", "!miss!", "!time!", "!combo!", "!CurrentMaxCombo!", "!PpIfMapEndsNow!", "!PpIfRestFced!", "!AccIfRestFced!"
        };
        [IgnoreDataMember]
        public ReadOnlyCollection<string> MemoryFormatTokens => _memoryFormatTokens.AsReadOnly();
        private bool _isMemoryFormat;
        private OsuStatus _saveEvent = OsuStatus.All;
        private string _pattern = "Your pattern text";
        private string _name;
        [IgnoreDataMember]
        public Dictionary<string, string> Replacements;
        [DisplayName("Name")]
        [JsonProperty(PropertyName = "Name")]
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
        [JsonProperty(PropertyName = "Pattern")]
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
        [IgnoreDataMember]
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
        [JsonProperty(PropertyName = "ShowInOsu")]
        [Editable(false)]
        [DisplayName("Ingame")]
        public bool ShowInOsu { get; set; }

        [JsonProperty(PropertyName = "XPosition")]
        [Browsable(false)]
        public int XPosition { get; set; } = 200;

        [JsonProperty(PropertyName = "YPosition")]
        [Browsable(false)]
        public int YPosition { get; set; } = 200;

        [JsonProperty(PropertyName = "Color")]
        [Browsable(false)]
        public Color Color { get; set; } = Color.Red;

        [JsonProperty(PropertyName = "FontName")]
        [Browsable(false)]
        public string FontName { get; set; } = "Arial";

        [JsonProperty(PropertyName = "FontSize")]
        [Browsable(false)]
        public int FontSize { get; set; } = 12;

        [JsonProperty(PropertyName = "SaveEvent")]
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
        [IgnoreDataMember]
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
                string toFormat = this.Pattern ?? "";
                foreach (var r in Replacements)
                {
                    toFormat = toFormat.Replace(r.Key, r.Value, StringComparison.InvariantCultureIgnoreCase);
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