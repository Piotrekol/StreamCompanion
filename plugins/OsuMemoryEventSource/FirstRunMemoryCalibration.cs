using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using CollectionManager.DataTypes;
using OsuMemoryDataProvider;
using OsuMemoryDataProvider.Models;
using StreamCompanion.Common;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Services;

namespace OsuMemoryEventSource
{
    public partial class FirstRunMemoryCalibration : UserControl, IFirstRunControl
    {
        private readonly object _lockingObject = new object();

        private readonly List<Map> _maps = new List<Map>
        {
            new Map("Thunderclowns - Find The Memes In You", 314211,
                new List<int> {712858, 712859, 712847, 712945, 701254}),
            new Map("Bachelor Party - Kappa Kappa (Alex TheSeal Edit)", 312113, new List<int> { 1909230, 696895}),
            new Map("NOMA - Brain Power Long Version", 432822, new List<int> {933228}),
            new Map("Memme - Chinese Restaurant", 399404, new List<int> {869286, 875324}),
            new Map("Yuyoyuppe - Bad Apple!!", 299000, new List<int> {670892, 670894, 670893}),
            new Map("Kommisar - Bad Apple!! (Chiptune ver.)", 28222, new List<int> {94187, 94186, 94188, 94189}),
            new Map("Freezer feat. Kiichigo - Berry Go!!", 1034502,
                new List<int> {2162847, 2162848, 2164477, 2170342, 2170756, 2171102})
        };

        private readonly SettingNames _names = SettingNames.Instance;
        private readonly ISettings _settings;

        private readonly int ExpectedMods = 89; //HRNFDTHD 

        private readonly StructuredOsuMemoryReader memoryReader;
        private readonly Random rnd = new Random();
        private Map _currentMap;
        private volatile bool Passed;
        private ILogger _logger;

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

        public FirstRunMemoryCalibration(StructuredOsuMemoryReader reader, ISettings settings, ILogger logger)
        {
            memoryReader = reader;
            _settings = settings;
            _logger = logger;

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
                Invoke((MethodInvoker)delegate { SetCalibrationText(text); });
                return;
            }

            label_CalibrationResult.Text = text;
        }

        public void GotMemory(int mapId, OsuStatus status, string mapString)
        {
            if (!IsHandleCreated)
                return;

            _logger.Log($"FirstRun: mapId: {mapId}, status: {status}, mapString: {mapString}", LogLevel.Debug);
            _logger.Log($"FirstRun: looking for: {CurrentMap}", LogLevel.Debug);
            lock (_lockingObject)
            {
                Invoke((MethodInvoker)delegate
               {
                   label_memoryStatus.Text =
                       $"{status}, id:{mapId}, is valid mapset:{CurrentMap.BelongsToSet(mapId)}";
               });

                if (CurrentMap.BelongsToSet(mapId) && status == OsuStatus.Playing)
                {
                    Invoke((MethodInvoker)delegate
                   {
                       SetCalibrationText("Searching. It can take up to 20 seconds.");
                   });

                    var mods = Helpers.ExecWithTimeout(async token =>
                    {
                        if (token.IsCancellationRequested)
                            return -1;

                        //ugly workaround
                        SetCalibrationText("Initial search delay... waiting 3 seconds");
                        await Task.Delay(3000);

                        return ((OsuMemoryDataProvider.Models.Memory.Mods)memoryReader.ReadProperty(memoryReader.OsuMemoryAddresses.Player, nameof(Player.Mods))).Value;
                    }, 20000).Result;

                    Passed = mods == ExpectedMods;

                    Invoke((MethodInvoker)delegate
                   {
                       if (Passed)
                       {
                           Completed?.Invoke(this, new FirstRunCompletedEventArgs { ControlCompletionStatus = FirstRunStatus.Ok });
                       }
                       else
                       {
                           var resultText =
                               $"Something went wrong(mods: {(Mods)mods}). Maybe try again with another map?";
                           SetCalibrationText(resultText);
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
                ProcessExt.OpenUrl(CurrentMap.DownloadLink);
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

        private class Map
        {
            public string MapName { get; }
            public int MapSetId { get; }
            public List<int> Diffs { get; }
            public string DownloadLink => $"https://osu.ppy.sh/beatmapsets/{MapSetId}/download";

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

            public override string ToString()
                => $"{MapName}; setId: {MapSetId}; diffIds:[{string.Join(",", Diffs)}]";
        }
    }
}