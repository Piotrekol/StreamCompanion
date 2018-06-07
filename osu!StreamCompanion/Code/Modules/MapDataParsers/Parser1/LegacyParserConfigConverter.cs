using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Core;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.MapDataParsers.Parser1
{
    public class LegacyParserConfigConverter
    {
        private sealed class LegacyConfigEntries
        {
            private LegacyConfigEntries() { }
            public static LegacyConfigEntries Instance { get; } = new LegacyConfigEntries();

            public readonly ConfigEntry PatternFileNames = new ConfigEntry("PatternFileNames", new List<string>());
            public readonly ConfigEntry Patterns = new ConfigEntry("Patterns", new List<string>());
            public readonly ConfigEntry SaveEvents = new ConfigEntry("saveEvents", new List<int>());
            public readonly ConfigEntry PatternShowInOsu = new ConfigEntry("ShowInOsu", new List<int>());
            public readonly ConfigEntry PatternX = new ConfigEntry("PatternX", new List<int>());
            public readonly ConfigEntry PatternY = new ConfigEntry("PatternY", new List<int>());
            public readonly ConfigEntry PatternColor = new ConfigEntry("PatternColor", new List<string>());
            public readonly ConfigEntry PatternFontName = new ConfigEntry("PatternFontName", new List<string>());
            public readonly ConfigEntry PatternFontSize = new ConfigEntry("PatternFontSize", new List<int>());
            public readonly ConfigEntry PatternIsMemory = new ConfigEntry("PatternIsMemory", new List<int>());
        }

        private readonly SettingNames _names = SettingNames.Instance;

        public List<OutputPattern> Convert(Settings settings, bool removeLegacyEntries = true)
        {
            var patterns = Load(settings, settings.Get<string>(_names.LastRunVersion));
            if (removeLegacyEntries)
            {
                var n = LegacyConfigEntries.Instance;

                settings.Delete(n.PatternFileNames);
                settings.Delete(n.Patterns);
                settings.Delete(n.SaveEvents);
                settings.Delete(n.PatternShowInOsu);
                settings.Delete(n.PatternX);
                settings.Delete(n.PatternY);
                settings.Delete(n.PatternColor);
                settings.Delete(n.PatternFontName);
                settings.Delete(n.PatternFontSize);
                settings.Delete(n.PatternIsMemory);
            }
            return patterns;
        }
        private List<OutputPattern> Load(Settings settings, string lastRanVersion)
        {
            var names = LegacyConfigEntries.Instance;

            List<string> filenames = settings.Get(names.PatternFileNames.Name);
            if (filenames.Count == 1 && string.IsNullOrEmpty(filenames[0]))
                return new List<OutputPattern>();
            List<string> patterns = settings.Get(names.Patterns.Name);
            List<int> saveEvents = settings.Geti(names.SaveEvents.Name);


            var requiredCount = Math.Max(filenames.Count, Math.Max(patterns.Count, saveEvents.Count));
            
            if (filenames.Count != requiredCount || patterns.Count != requiredCount || saveEvents.Count!=requiredCount)
            {

                var userResponse = MessageBox.Show("Your output patterns are broken and could not be loaded successfully" + Environment.NewLine +
                    "I can load them with missing data or just reset to default patterns." + Environment.NewLine +
                    "Do you want to try to load them?" + Environment.NewLine +
                    "Note that after this is done your patterns will be converted to new format with will get rid of this issue."
                    , "osu!StreamCompanion - User action required!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (userResponse == DialogResult.No)
                    return new List<OutputPattern>();

                var toFixCount = requiredCount * 3 - (filenames.Count + patterns.Count + saveEvents.Count);
                while (filenames.Count < requiredCount)
                    filenames.Add(ParserSettings.GenerateFileName(filenames, "Recovered_"));
                while (patterns.Count < requiredCount)
                    patterns.Add("Recovered");
                while (saveEvents.Count < requiredCount)
                    saveEvents.Add((int)OsuStatus.All);

                MessageBox.Show("Finished recovering patterns" + Environment.NewLine +
                                toFixCount + " entrys were missing" + Environment.NewLine +
                                "Go to settings and check your patterns!!!", "osu!StreamCompanion - Done", MessageBoxButtons.OK);
            }

            var loadedPatterns = new List<OutputPattern>();
            for (int i = 0; i < filenames.Count; i++)
            {
                //Converting from ealier versions                    
                if (saveEvents[i] == 27 || saveEvents[i] == 31)
                    saveEvents[i] = (int)OsuStatus.All;
                if (filenames[i].EndsWith(".txt"))
                    filenames[i] = filenames[i].Substring(0, filenames[i].LastIndexOf(".txt", StringComparison.Ordinal));

                var c = Color.FromArgb(255, 0, 0);
                OsuStatus saveEvent;
                try
                {
                    saveEvent = (OsuStatus)saveEvents[i];
                }
                catch
                {
                    saveEvent = OsuStatus.All;
                }
                loadedPatterns.Add(new OutputPattern()
                {
                    Name = filenames[i],
                    Pattern = patterns[i],
                    SaveEvent = saveEvent,
                    ShowInOsu = false,
                    XPosition = 200,
                    YPosition = 200,
                    Color = c,
                    FontName = "Arial",
                    FontSize = 12
                });
            }

            return loadedPatterns;


        }

    }
}