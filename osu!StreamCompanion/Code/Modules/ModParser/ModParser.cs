using System;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.ModParser
{
    public class ModParser : IModule, IModParser, ISettingsProvider
    {
        private readonly SettingNames _names = SettingNames.Instance;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
            _modsEnumVals = new int[_modsLong.Length];
            for (var i = 0; i < _modsLong.Length; i++)
            {
                _modsEnumVals[i] = (int)Math.Pow(2, i - 1);
            }
        }
        private int[] _modsEnumVals;

        private string[] _modsLong =
        {
            "None", "NoFail", "Easy", "//NoVideo", "Hidden", "HardRock", "SuddenDeath", "DoubleTime", "Relax",
            "HalfTime", "Nightcore", "Flashlight", "Autoplay", "SpunOut", "Relax2", "Perfect", "Key4", "Key5", "Key6", "Key7",
            "Key8", "FadeIn", "Random", "LastMod","--", "Key9", "Coop", "Key1", "Key3", "Key2"
        };

        private string[] _modsShort =
        {
            "None", "NF", "EZ", "NV", "HD", "HR", "SD", "DT", "RX",
            "HT", "NC", "FL","AP", "SO", "RX2", "PF", "K4", "K5", "K6", "K7",
            "K8", "FI", "RN", "LM", "--", "K9", "Coop", "K1", "K3", "K2"
        };

        private Settings _settings;

        public string GetModsFromEnum(int modsEnum)
        {
            string noneText = _settings.Get<string>(_names.NoModsDisplayText);
            if (noneText != _modsShort[0])
            {
                _modsShort[0] = noneText;
                _modsLong[0] = noneText;
            }
            return GetModsFromEnum(modsEnum, !_settings.Get<bool>(_names.UseLongMods));
        }
        public string GetModsFromEnum(int modsEnum, bool shortMod = false)
        {
            System.Text.StringBuilder modStr = new System.Text.StringBuilder();
            for (int i = 0; i < _modsEnumVals.Length; i++)
            {
                if ((modsEnum & _modsEnumVals[i]) > 0)
                {
                    modStr.Append(shortMod ? _modsShort[i] : _modsLong[i]);
                    modStr.Append(",");
                }
            }

            string retVal;
            if (modStr.Length > 1)
            {
                modStr.Remove(modStr.Length - 1, 1);
                retVal = modStr.ToString();

                if (retVal.Contains("NC"))
                {
                    retVal = retVal.Replace("DT,", "");
                }
            }
            else
                retVal = modStr.Append(shortMod ? _modsShort[0] : _modsLong[0]).ToString();

            return retVal;
        }


        public string SettingGroup { get; } = "Map matching";

        public void SetSettingsHandle(Settings settings)
        {
            _settings = settings;
        }

        public void Free()
        {
            _modParserSettings.Dispose();
        }

        private ModParserSettings _modParserSettings;
        public System.Windows.Forms.UserControl GetUiSettings()
        {
            if (_modParserSettings == null || _modParserSettings.IsDisposed)
            {
                _modParserSettings = new ModParserSettings(_settings);
            }
            return _modParserSettings;
        }
    }
}
