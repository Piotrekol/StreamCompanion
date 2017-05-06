using System;
using System.Collections.Generic;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;
using osu_StreamCompanion.Code.Modules.ModParser;
using Mods = osu_StreamCompanion.Code.Modules.ModsHandler.EMods;
namespace osu_StreamCompanion.Code.Modules.ModsHandler
{
    public class ModParser : IModule, IModParser, ISettingsProvider
    {
        private readonly SettingNames _names = SettingNames.Instance;
        public bool Started { get; set; }
        public void Start(ILogger logger)
        {
            Started = true;
        }

        private Dictionary<int, Tuple<Mods, string, string>> ModsConv = new Dictionary<int, Tuple<Mods, string, string>>()
        {
            {(int)Mods.Omod,new Tuple<Mods, string, string>(Mods.Omod,"None","None")},
            {(int)Mods.Nf,new Tuple<Mods, string, string>(Mods.Nf,"NF","NoFail")},
            {(int)Mods.Ez,new Tuple<Mods, string, string>(Mods.Ez,"EZ","Easy")},
            //{(int)Mods.//Nv,new Tuple<EMods, string, string>(Mods.//Nv,"NV","//NoVideo")},
            {(int)Mods.Hd,new Tuple<Mods, string, string>(Mods.Hd,"HD","Hidden")},
            {(int)Mods.Hr,new Tuple<Mods, string, string>(Mods.Hr,"HR","HardRock")},
            {(int)Mods.Sd,new Tuple<Mods, string, string>(Mods.Sd,"SD","SuddenDeath")},
            {(int)Mods.Dt,new Tuple<Mods, string, string>(Mods.Dt,"DT","DoubleTime")},
            {(int)Mods.RX,new Tuple<Mods, string, string>(Mods.RX,"RX","Relax")},
            {(int)Mods.Ht,new Tuple<Mods, string, string>(Mods.Ht,"HT","HalfTime")},
            {(int)Mods.Nc,new Tuple<Mods, string, string>(Mods.Nc,"NC","Nightcore")},
            {(int)Mods.Fl,new Tuple<Mods, string, string>(Mods.Fl,"FL","Flashlight")},
            {(int)Mods.Ap,new Tuple<Mods, string, string>(Mods.Ap,"AP","Autoplay")},
            {(int)Mods.So,new Tuple<Mods, string, string>(Mods.So,"SO","SpunOut")},
            {(int)Mods.Rx2,new Tuple<Mods, string, string>(Mods.Rx2,"RX2","Relax2")},
            {(int)Mods.Pf,new Tuple<Mods, string, string>(Mods.Pf,"PF","Perfect")},
            {(int)Mods.K4,new Tuple<Mods, string, string>(Mods.K4,"K4","Key4")},
            {(int)Mods.K5,new Tuple<Mods, string, string>(Mods.K5,"K5","Key5")},
            {(int)Mods.K6,new Tuple<Mods, string, string>(Mods.K6,"K6","Key6")},
            {(int)Mods.K7,new Tuple<Mods, string, string>(Mods.K7,"K7","Key7")},
            {(int)Mods.K8,new Tuple<Mods, string, string>(Mods.K8,"K8","Key8")},
            {(int)Mods.Fi,new Tuple<Mods, string, string>(Mods.Fi,"FI","FadeIn")},
            {(int)Mods.Rn,new Tuple<Mods, string, string>(Mods.Rn,"RN","Random")},
            {(int)Mods.Lm,new Tuple<Mods, string, string>(Mods.Lm,"LM","LastMod")},
            //{(int)Mods.//=,new Tuple<EMods, string, string>(Mods.//=,"--","--")},
            {(int)Mods.K9,new Tuple<Mods, string, string>(Mods.K9,"K9","Key9")},
            {(int)Mods.Coop,new Tuple<Mods, string, string>(Mods.Coop,"Coop","Coop")},
            {(int)Mods.K1,new Tuple<Mods, string, string>(Mods.K1,"K1","Key1")},
            {(int)Mods.K3,new Tuple<Mods, string, string>(Mods.K3,"K3","Key3")},
            {(int)Mods.K2,new Tuple<Mods, string, string>(Mods.K2,"K2","Key2")}
        };

        private Settings _settings;

        public string GetModsFromEnum(int modsEnum)
        {
            string noneText = _settings?.Get<string>(_names.NoModsDisplayText) ?? "None";
            if (noneText != ModsConv[0].Item2)
            {
                ModsConv[0] = new Tuple<Mods, string, string>(ModsConv[0].Item1, noneText, noneText);
                ModsConv[0] = new Tuple<Mods, string, string>(ModsConv[0].Item1, noneText, noneText);
            }
            return GetModsFromEnum(modsEnum, !_settings?.Get<bool>(_names.UseLongMods) ?? true);
        }

        public Mods GeteModsFromInt(int mods)
        {
            Mods eMods = Mods.Omod;
            foreach (var key in ModsConv.Keys)
            {
                if ((mods & key) > 0)
                {
                    eMods = eMods | ModsConv[key].Item1;
                }
            }
            return eMods;
        }
        public string GetModsFromEnum(int modsEnum, bool shortMod = false)
        {
            System.Text.StringBuilder modStr = new System.Text.StringBuilder();
            //Sanitize input int
            modsEnum = (int)GeteModsFromInt(modsEnum);

            foreach (var key in ModsConv.Keys)
            {
                if ((modsEnum & key) > 0)
                {
                    modStr.Append(shortMod ? ModsConv[key].Item2 : ModsConv[key].Item3);
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
                retVal = modStr.Append(shortMod ? ModsConv[0].Item2 : ModsConv[0].Item3).ToString();

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
