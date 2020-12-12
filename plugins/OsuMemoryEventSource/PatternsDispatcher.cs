using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Consumers;
using StreamCompanionTypes.Interfaces.Services;

namespace OsuMemoryEventSource
{
    public class PatternsDispatcher : IDisposable
    {
        private readonly ISettings _settings;
        private readonly ISaver _saver;
        public List<IHighFrequencyDataConsumer> HighFrequencyDataConsumers { get; set; }
        private BlockingCollection<IOutputPattern> OutputPatterns = new BlockingCollection<IOutputPattern>();
        public bool SaveLiveTokensOnDisk { get; private set; }
        private ManualResetEvent _updatingOutputPatterns = new ManualResetEvent(false);
        private ManualResetEvent _usingOutputPatterns = new ManualResetEvent(false);


        public PatternsDispatcher(ISettings settings, ISaver saver)
        {
            _settings = settings;
            _saver = saver;
            _settings.SettingUpdated += SettingUpdated;
            SettingUpdated(this, new SettingUpdated(OsuMemoryEventSourceBase.SaveLiveTokensOnDisk.Name));
        }

        public void SetOutputPatterns(IList<IOutputPattern> patterns)
        {
            while (_usingOutputPatterns.WaitOne(1)) { }

            _updatingOutputPatterns.Set();
            while (OutputPatterns.TryTake(out _) || OutputPatterns.Count != 0) { }
            foreach (var f in patterns)
            {
                OutputPatterns.Add(f);
            }

            _updatingOutputPatterns.Reset();
        }

        public void TokensUpdated(OsuStatus status)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            while (_updatingOutputPatterns.WaitOne(1)) { }

            _usingOutputPatterns.Set();

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

            _usingOutputPatterns.Reset();
        }

        public void Dispose()
        {
            _settings.SettingUpdated -= SettingUpdated;
        }

        private void SetOutput(IOutputPattern p, string value)
        {
            void WriteToHandlers(string name, string content)
            {
                HighFrequencyDataConsumers.ForEach(h => h.Handle(name, content));
            }

            //Standard output
            WriteToHandlers(p.Name, value.Replace("\r", ""));

            if (SaveLiveTokensOnDisk)
                _saver.Save($"{p.Name}.txt", value);

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

        private void SettingUpdated(object sender, SettingUpdated e)
        {
            if (e.Name == OsuMemoryEventSourceBase.SaveLiveTokensOnDisk.Name)
            {
                SaveLiveTokensOnDisk = _settings.Get<bool>(OsuMemoryEventSourceBase.SaveLiveTokensOnDisk);
            }
        }
    }
}