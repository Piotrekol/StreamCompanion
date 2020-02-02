using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using CollectionManager.DataTypes;
using OsuMemoryDataProvider;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;

namespace OsuMemoryEventSource
{
    public partial class FirstRunMemoryCalibration : UserControl, IFirstRunUserControl
    {
        private readonly object _lockingObject = new object();

        private readonly List<Map> _maps = new List<Map>
        {
            new Map("Thunderclowns - Find The Memes In You", 314211,
                new List<int> {712858, 712859, 712847, 712945, 701254}),
            new Map("Bachelor Party - Kappa Kappa (Forsen Edition)", 312113, new List<int> {958367, 696895}),
            new Map("NOMA - Brain Power Long Version", 432822, new List<int> {933228}),
            new Map("Memme - Chinese Restaurant", 399404, new List<int> {869286, 875324}),
            new Map("Yuyoyuppe - Bad Apple!!", 299000, new List<int> {670892, 670894, 670893}),
            new Map("Kommisar - Bad Apple!! (Chiptune ver.)", 28222, new List<int> {94187, 94186, 94188, 94189}),
            new Map("Freezer feat. Kiichigo - Berry Go!!", 1034502,
                new List<int> {2162847, 2162848, 2164477, 2170342, 2170756, 2171102})
        };

        private readonly SettingNames _names = SettingNames.Instance;
        private readonly ISettingsHandler _settings;

        private readonly int ExpectedMods = 89; //HRNFDTHD 

        private readonly IOsuMemoryReader memoryReader;
        private readonly Random rnd = new Random();
        private Map _currentMap;
        private volatile bool Passed;

        public event EventHandler<FirstRunCompletedEventArgs> Completed;

        private Map CurrentMap
        {
            get => _currentMap;
            set
            {
                _currentMap = value;
                linkLabel_mapDL.Text = $"Click here for map download ({value.DownloadLink})";
                label_beatmapDL.Text =
                    "Enable following mods: HR,NF,DT,HD" + Environment.NewLine +
                    " and play this map:" + Environment.NewLine +
                    " \"" + value.MapName + "\" with any difficulty in osu! mode";
            }
        }

        public FirstRunMemoryCalibration(IOsuMemoryReader reader, ISettingsHandler settings)
        {
            memoryReader = reader;
            _settings = settings;

            InitializeComponent();
            pictureBox1.Image = StreamCompanionHelper.StreamCompanionLogo();

            CurrentMap = _maps[rnd.Next(0, _maps.Count)];

            _settings.Add(_names.EnableMemoryScanner.Name, true);
            _settings.Add(_names.EnableMemoryPooling.Name, true);
        }

        private void SetCalibrationText(string text)
        {
            if (label_CalibrationResult.InvokeRequired)
            {
                Invoke((MethodInvoker) delegate { SetCalibrationText(text); });
                return;
            }

            label_CalibrationResult.Text = text;
        }

        public void GotMemory(int mapId, OsuStatus status, string mapString)
        {
            if (!IsHandleCreated) return;
            lock (_lockingObject)
            {
                Invoke((MethodInvoker) delegate
                {
                    label_memoryStatus.Text =
                        $"{status}, id:{mapId}, valid mapset:{CurrentMap.BelongsToSet(mapId)}";
                });

                if (CurrentMap.BelongsToSet(mapId) && status == OsuStatus.Playing)
                {
                    Invoke((MethodInvoker) delegate
                    {
                        SetCalibrationText("Searching. It can take up to 20 seconds.");
                        button_Skip.Enabled = false;
                    });

                    var mods = Helpers.ExecWithTimeout(async token =>
                    {
                        if (token.IsCancellationRequested)
                            return -1;

                        //ugly workaround
                        SetCalibrationText("Initial search delay... waiting 3 seconds");
                        await Task.Delay(3000);

                        return memoryReader.GetMods();
                    }, 20000).Result;

                    Passed = mods == ExpectedMods;

                    Invoke((MethodInvoker) delegate
                    {
                        if (Passed)
                        {
                            button_Skip_Click(this, EventArgs.Empty);
                        }
                        else
                        {
                            var resultText =
                                $"Something went wrong(mods: {(Mods)mods}). Try again(start map again) or continue with DISABLED memory";
                            SetCalibrationText(resultText);
                            button_Skip.Enabled = true;
                        }
                    });
                }
            }
        }

        public void SetNextMap()
        {
            CurrentMap = _maps[(_maps.IndexOf(CurrentMap) + 1) % _maps.Count];
            SetCalibrationText("Waiting...");
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

        private void button_anotherMap_Click(object sender, EventArgs e)
        {
            SetNextMap();
        }

        private void button_Skip_Click(object sender, EventArgs e)
        {
            _settings.Add(_names.EnableMemoryScanner.Name, Passed);
            _settings.Add(_names.EnableMemoryPooling.Name, Passed);

            Completed?.Invoke(this, new FirstRunCompletedEventArgs {ControlCompletionStatus = FirstRunStatus.Ok});
        }

        private class Map
        {
            public string MapName { get; }
            public int MapSetId { get; }
            public List<int> Diffs { get; }
            public string DownloadLink => $"https://osu.ppy.sh/d/{MapSetId}";
            public string SetLink => $"https://osu.ppy.sh/s/{MapSetId}";

            public Map(string mapName, int mapSetId, List<int> diffs)
            {
                MapName = mapName;
                MapSetId = mapSetId;
                Diffs = diffs;
            }

            public bool BelongsToSet(int mapId)
            {
                return Diffs.Contains(mapId);
            }
        }
    }
}