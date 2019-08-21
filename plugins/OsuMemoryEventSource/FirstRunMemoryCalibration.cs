using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

namespace OsuMemoryEventSource
{
    public partial class FirstRunMemoryCalibration : FirstRunUserControl
    {
        private class Map
        {
            public Map(string mapName, int mapSetId, List<int> diffs)
            {
                MapName = mapName;
                MapSetId = mapSetId;
                Diffs = diffs;
            }

            public string MapName { get; }
            public int MapSetId { get; }
            public List<int> Diffs { get; }
            public string DownloadLink => $"https://osu.ppy.sh/d/{MapSetId}";
            public string SetLink => $"https://osu.ppy.sh/s/{MapSetId}";

            public bool BelongsToSet(int mapId) => Diffs.Contains(mapId);
        }

        private List<Map> _maps = new List<Map>
        {
            new Map("Thunderclowns - Find The Memes In You",314211,new List<int>{712858,712859,712847,712945,701254}),
            new Map("Thunderclowns - Find The Memes In You",314211,new List<int>{712858,712859,712847,712945,701254}),
            new Map("Bachelor Party - Kappa Kappa (Forsen Edition)",312113,new List<int>{958367,696895}),
            new Map("Bachelor Party - Kappa Kappa (Forsen Edition)",312113,new List<int>{958367,696895}),
            new Map("Bachelor Party - Kappa Kappa (Forsen Edition)",312113,new List<int>{958367,696895}),
            new Map("NOMA - Brain Power Long Version",432822,new List<int>{933228}),
            new Map("Memme - Chinese Restaurant",399404,new List<int>{869286,875324}),
            new Map("Yuyoyuppe - Bad Apple!!",299000,new List<int>{ 670892, 670894,670893}),
            new Map("Kommisar - Bad Apple!! (Chiptune ver.)",28222,new List<int>{ 94187, 94186, 94188,94189}),

        };

        private IOsuMemoryReader memoryReader;
        private readonly ISettingsHandler _settings;
        private readonly SettingNames _names = SettingNames.Instance;
        private Map _currentMap;
        private int ExpectedMods = 89;//HRNFDTHD 
        Random rnd = new Random();
        private volatile bool Passed = false;
        private Map CurrentMap
        {
            get { return _currentMap; }
            set
            {
                _currentMap = value;
                linkLabel_mapDL.Text = $"Click here for map download ({value.DownloadLink})";
                label_beatmapDL.Text =
                    "Enable following mods: HR,NF,DT,HD" + Environment.NewLine +
                    " and play this map:" + Environment.NewLine +
                    " \"" + value.MapName + "\" with any difficulty";
            }
        }

        public FirstRunMemoryCalibration(IOsuMemoryReader reader, ISettingsHandler settings)
        {
            memoryReader = reader;
            _settings = settings;

            InitializeComponent();
            this.pictureBox1.Image = GetStreamCompanionLogo();

            CurrentMap = _maps[rnd.Next(0, _maps.Count)];

            _settings.Add(_names.EnableMemoryScanner.Name, true);
            _settings.Add(_names.EnableMemoryPooling.Name, true);
        }

        public void SetRandomMap(bool notSame = true)
        {
            Map newMap;
            do
            {
                newMap = _maps[rnd.Next(0, _maps.Count)];

            } while (notSame && newMap.MapSetId == CurrentMap.MapSetId);
            CurrentMap = newMap;
            this.button_Next.Enabled = false;
            SetCalibrationText("Waiting...");
        }

        private void SetCalibrationText(string text)
        {
            if (label_CalibrationResult.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    SetCalibrationText(text);
                });
                return;
            }
            this.label_CalibrationResult.Text = text;

        }

        private readonly object _lockingObject = new object();
        public void GotMemory(int mapId, OsuStatus status, string mapString)
        {
            if (!this.IsHandleCreated) return;
            lock (_lockingObject)
            {
                this.BeginInvoke((MethodInvoker)delegate
                {
                    label_memoryStatus.Text = $"{status}, id:{mapId}, valid mapset:{CurrentMap.BelongsToSet(mapId)}";
                });
                
                if (CurrentMap.BelongsToSet(mapId) && status == OsuStatus.Playing)
                {
                    this.BeginInvoke((MethodInvoker)delegate
                   {
                       SetCalibrationText("Searching. It can take up to 20 seconds.");
                       this.button_Next.Enabled = false;
                   });
                    var everythingIsOk = Helpers.ExecWithTimeout(token =>
                    {
                        Thread.Sleep(2000);
                        if (token.IsCancellationRequested)
                            return false;
                        int mods = memoryReader.GetMods();

                        if (mods == ExpectedMods)
                            return true;

                        return false;
                    }, 20000);

                    _settings.Add(_names.EnableMemoryScanner.Name, everythingIsOk);
                    _settings.Add(_names.EnableMemoryPooling.Name, everythingIsOk);
                    Passed = everythingIsOk;

                    this.BeginInvoke((MethodInvoker)delegate
                   {
                       var resultText = everythingIsOk
                           ? "Found right offset, Continue :)"
                           : "Something went wrong. Try again(start map again) or continue with DISABLED memory";
                       SetCalibrationText(resultText);
                       this.button_Next.Enabled = true;
                   });

                }
            }
        }

        private void linkLabel_mapDL_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(CurrentMap.DownloadLink);
            }
            catch (Win32Exception)
            {
                MessageBox.Show("Your system doesn't have a default browser set" + Environment.NewLine +
                                "Sadly there is nothing I can do to fix this" + Environment.NewLine +
                                "Either fix that(google can help), or write shown url in your browser window manually",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button_Next_Click(object sender, EventArgs e)
        {
            _settings.Add(_names.EnableMemoryScanner.Name, Passed);
            _settings.Add(_names.EnableMemoryPooling.Name, Passed);

            this.OnCompleted(status.Ok);
        }

        private void button_anotherMap_Click(object sender, EventArgs e)
        {
            SetRandomMap();
            var l = new List<string> { "No", "NO", "Nah", "Nope", "Why", "uh-uh", "nay", "negative", "" };//Last one is intentional
            button_anotherMap.Text = l[rnd.Next(0, l.Count)];
        }
    }
}
