using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Interfaces.Consumers;

namespace OsuMemoryEventSource
{
    public class PatternsDispatcher
    {
        public List<IHighFrequencyDataConsumer> HighFrequencyDataConsumers { get; set; }
        public BlockingCollection<IOutputPattern> OutputPatterns = new BlockingCollection<IOutputPattern>();

        public void SetOutputPatterns(IList<IOutputPattern> patterns)
        {
            lock (OutputPatterns)
            {
                while (OutputPatterns.TryTake(out _) || OutputPatterns.Count != 0) { }

                foreach (var f in patterns)
                {
                    OutputPatterns.Add(f);
                }
            }
        }

        public void TokensUpdated(OsuStatus status)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();

            lock (OutputPatterns)
            {
                if (OutputPatterns.Count > 0)
                {
                    foreach (var pattern in OutputPatterns)
                    {
                        if (!pattern.IsMemoryFormat)
                            continue;

                        var formattedPattern = pattern.GetFormatedPattern(((status & pattern.SaveEvent) != 0) ? OsuStatus.All : status);
                        if (string.IsNullOrEmpty(formattedPattern))
                            formattedPattern = " "; //workaround: OBS plugin doesn't seem to react to empty strings

                        output[pattern.Name] = formattedPattern;
                        SetOutput(pattern, formattedPattern);
                    }

                    var json = JsonConvert.SerializeObject(output);
                    HighFrequencyDataConsumers.ForEach(h => h.Handle(json));
                }
            }
        }

        private void SetOutput(IOutputPattern p, string value)
        {
            void WriteToHandlers(string name, string content)
            {
                HighFrequencyDataConsumers.ForEach(h => h.Handle(name, content));
            }

            //Standard output
            WriteToHandlers(p.Name, value.Replace("\r", ""));

            //ingameOverlay
            if (p.ShowInOsu)
            {
                var configName = "conf-" + p.Name;
                var valueName = "value-" + p.Name;
                var config = $"{p.XPosition} {p.YPosition} {p.Color.R} {p.Color.G} {p.Color.B} {p.Color.A} {p.FontName.Replace(' ', '/')} {p.FontSize} {p.Aligment}";
                WriteToHandlers(configName, config);
                WriteToHandlers(valueName, value);
            }
        }
    }
}