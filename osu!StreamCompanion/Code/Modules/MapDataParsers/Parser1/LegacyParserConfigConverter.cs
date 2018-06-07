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

            List<int> patternShowInOsu = settings.Geti(names.PatternShowInOsu.Name);
            List<int> patternX = settings.Geti(names.PatternX.Name);
            List<int> patternY = settings.Geti(names.PatternY.Name);
            List<string> patternColor = settings.Get(names.PatternColor.Name);
            List<string> patternFontName = settings.Get(names.PatternFontName.Name);
            List<int> patternFontSize = settings.Geti(names.PatternFontSize.Name);

            var requiredCount = Math.Max(filenames.Count, Math.Max(patterns.Count, Math.Max(patternShowInOsu.Count, saveEvents.Count)));
            requiredCount = Math.Max(requiredCount, Math.Max(patternX.Count, Math.Max(patternY.Count, Math.Max(patternColor.Count, patternFontName.Count))));
            requiredCount = Math.Max(requiredCount, patternFontSize.Count);
            if (Helpers.Helpers.GetDateFromVersionString(lastRanVersion) <
                Helpers.Helpers.GetDateFromVersionString("v180501.16") && patternShowInOsu.Count == 0)
            {//New setting entrys added - fill with defaults.
                while (patternShowInOsu.Count < requiredCount)
                {
                    patternShowInOsu.Add(0);
                    patternX.Add(200);
                    patternY.Add(200);
                    patternColor.Add("255;0;0");
                    patternFontName.Add("Arial");
                    patternFontSize.Add(12);
                }
            }


            if (filenames.Count != requiredCount || filenames.Count != requiredCount ||
                patternShowInOsu.Count != requiredCount || patternX.Count != requiredCount ||
                patternY.Count != requiredCount || patternColor.Count != requiredCount ||
                patternFontName.Count != requiredCount || patternFontSize.Count != requiredCount)
            {

                var userResponse = MessageBox.Show("Your output patterns are broken and could not be loaded successfully" + Environment.NewLine +
                    "I can load them with missing data or just reset to default patterns." + Environment.NewLine +
                    "Do you want to try to load them?" + Environment.NewLine +
                    "Note that after this is done your patterns will be converted to new format with will get rid of this issue."
                    , "osu!StreamCompanion - User action required!", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
                if (userResponse == DialogResult.No)
                    return new List<OutputPattern>();

                var toFixCount = requiredCount * 9 - (filenames.Count + patterns.Count + saveEvents.Count + patternShowInOsu.Count + patternX.Count + patternY.Count + patternColor.Count + patternFontName.Count + patternFontSize.Count);
                while (filenames.Count < requiredCount)
                    filenames.Add(ParserSettings.GenerateFileName(filenames, "Recovered_"));
                while (patterns.Count < requiredCount)
                    patterns.Add("Recovered");
                while (saveEvents.Count < requiredCount)
                    saveEvents.Add((int)OsuStatus.All);
                while (patternShowInOsu.Count < requiredCount)
                    patternShowInOsu.Add(0);
                while (patternX.Count < requiredCount)
                    patternX.Add(200);
                while (patternY.Count < requiredCount)
                    patternY.Add(200);
                while (patternColor.Count < requiredCount)
                    patternColor.Add("255;0;0");
                while (patternFontName.Count < requiredCount)
                    patternFontName.Add("Arial");
                while (patternFontSize.Count < requiredCount)
                    patternFontSize.Add(12);

                MessageBox.Show("Finished recovering patterns" + Environment.NewLine +
                                toFixCount + " entrys were missing" + Environment.NewLine +
                                "Go to settings and check your patterns!!!", "osu!StreamCompanion - Done", MessageBoxButtons.OK);
            }

            var loadedPatterns = new List<OutputPattern>();
            for (int i = 0; i < filenames.Count; i++)
            {
                //Converting from ealier versions                    
                if (saveEvents[i] == 27)
                    saveEvents[i] = (int)OsuStatus.All;
                if (filenames[i].EndsWith(".txt"))
                    filenames[i] = filenames[i].Substring(0, filenames[i].LastIndexOf(".txt", StringComparison.Ordinal));

                var c = patternColor[i].Split(';').Select(Int32.Parse).ToList();
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
                    ShowInOsu = patternShowInOsu[i] == 1,
                    XPosition = patternX[i],
                    YPosition = patternY[i],
                    Color = Color.FromArgb(c[0], c[1], c[2]),
                    FontName = patternFontName[i],
                    FontSize = patternFontSize[i]
                });
            }

            return loadedPatterns;


        }

    }
}