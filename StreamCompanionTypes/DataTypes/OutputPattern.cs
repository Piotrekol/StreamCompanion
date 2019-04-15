using CollectionManager.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;

namespace StreamCompanionTypes.DataTypes
{
    public class OutputPattern : EventArgs, INotifyPropertyChanged, ICloneable
    {
        private static readonly ObservableCollection<string> LiveTokenNames = new ObservableCollection<string>();

        private string _name;
        private string _pattern = "Your pattern text";
        private OsuStatus _saveEvent = OsuStatus.All;

        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreDataMember]
        public Tokens Replacements;

        private static readonly ReadOnlyObservableCollection<string> ReadOnlyLiveTokenNames = new ReadOnlyObservableCollection<string>(LiveTokenNames);

        [IgnoreDataMember]
        public ReadOnlyObservableCollection<string> MemoryFormatTokens => ReadOnlyLiveTokenNames;

        [DisplayName("Name")]
        [JsonProperty(PropertyName = "Name")]
        public string Name
        {
            get => _name;
            set
            {
                if (value == _name)
                {
                    return;
                }

                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        private static int counter = 0;
        [JsonProperty(PropertyName = "Pattern")]
        [DisplayName("Pattern")]
        public string Pattern
        {
            get => _pattern;
            set
            {
                if (value == _pattern)
                {
                    return;
                }

                _pattern = value;
                SetMemoryFormat();
               
                OnPropertyChanged(nameof(Pattern));
            }
        }

        private void SetMemoryFormat()
        {
            IsMemoryFormat = MemoryFormatTokens.Select(s => s.ToLower()).Any(_pattern.ToLower().Contains);
            if (IsMemoryFormat)
            {
                counter++;
                var a = 1;
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
            get => _saveEvent;
            set
            {
                if (value == _saveEvent)
                {
                    return;
                }

                _saveEvent = value;
                OnPropertyChanged(nameof(SaveEvent));
                OnPropertyChanged(nameof(SaveEventStr));
            }
        }

        [Browsable(false)]
        [IgnoreDataMember]
        [DisplayName("Memory format")]
        public bool IsMemoryFormat { get; private set; }
        
        public OutputPattern()
        {
            LiveTokenNames.CollectionChanged += (_, __) => SetMemoryFormat();
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string GetFormatedPattern()
        {
            if (Replacements != null)
            {
                var toFormat = Pattern ?? "";
                foreach (var r in Replacements)
                {
                    string replacement;
                    if (r.Value.Value is null)
                    {
                        replacement = "";
                    }
                    else
                    {
                        replacement = r.Value.FormatedValue;
                    }

                    toFormat = toFormat.Replace($"!{r.Key}!", replacement, StringComparison.InvariantCultureIgnoreCase);
                }

                return toFormat;
            }

            return string.Empty;
        }

        internal static void AddLiveToken(string tokenName)
        {
            tokenName = $"!{tokenName}!";
            if (!LiveTokenNames.Contains(tokenName))
            {
                LiveTokenNames.Add(tokenName);
            }
        }
    }
}