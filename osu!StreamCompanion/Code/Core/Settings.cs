using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StreamCompanionTypes.Enums;

namespace osu_StreamCompanion.Code.Core
{

    public class Settings : ISettings
    {
        private readonly Dictionary<string, JToken> _rawSettingEntries = new Dictionary<string, JToken>();
        private readonly Dictionary<string, object> _deserializedSettingEntries = new Dictionary<string, object>();
        public IReadOnlyDictionary<string, object> SettingEntries { get; }
        private ILogger _logger;
        public EventHandler<SettingUpdated> SettingUpdated { get; set; }
        private string _saveLocation = AppDomain.CurrentDomain.BaseDirectory;
        public string ConfigFileName { get; set; } = "settings.json";
        public string FullConfigFilePath => Path.Combine(_saveLocation, ConfigFileName);
        private readonly object _lockingObject = new object();

        private readonly JsonSerializer _defaultJsonSerializer = new JsonSerializer()
        {
            ObjectCreationHandling = ObjectCreationHandling.Replace
        };

        public Settings(ILogger logger)
        {
            _logger = logger;
            SettingEntries = new ReadOnlyDictionary<string, object>(_deserializedSettingEntries);
        }


        protected virtual void OnSettingUpdated(string settingName)
        {
            SettingUpdated?.Invoke(this, new SettingUpdated(settingName));
        }

        public T Get<T>(ConfigEntry entry)
        {
            lock (_lockingObject)
                return this.Get<T>(entry.Name, entry.Default<T>());
        }

        public void Add<T>(string key, T value, bool raiseUpdate = false)
        {
            lock (_lockingObject)
            {
                _deserializedSettingEntries[key] = value;
                if (_rawSettingEntries.ContainsKey(key))
                    _rawSettingEntries.Remove(key);

                if (raiseUpdate)
                    OnSettingUpdated(key);
            }
        }

        public void Add<T>(ConfigEntry entry, T value, bool raiseUpdate = false)
            => Add<T>(entry.Name, value, raiseUpdate);

        public T Get<T>(string key, T defaultValue)
        {
            lock (_lockingObject)
            {
                if (_deserializedSettingEntries.TryGetValue(key, out var rawSettingValue) && rawSettingValue != null && rawSettingValue is T settingValue)
                    return settingValue;

                if (_rawSettingEntries.TryGetValue(key, out var rawValue) && (rawValue.HasValues || rawValue.Value<object>() != null))
                {
                    _deserializedSettingEntries[key] = settingValue = rawValue.ToObject<T>(_defaultJsonSerializer);
                }
                else
                {
                    _deserializedSettingEntries[key] = settingValue = defaultValue;
                }

                return settingValue;
            }
        }

        public bool Delete(ConfigEntry entry) => Delete(entry.Name);

        public bool Delete(string key)
        {
            var deleted = false;
            if (_rawSettingEntries.ContainsKey(key))
            {
                _rawSettingEntries.Remove(key);
                deleted = true;
            }

            if (_deserializedSettingEntries.ContainsKey(key))
            {
                _deserializedSettingEntries.Remove(key);
                deleted = true;
            }

            return deleted;
        }
        public void Save()
        {
            lock (_lockingObject)
            {
                var settingsCopy = new Dictionary<string, object>(_deserializedSettingEntries);
                foreach (var kvPair in _rawSettingEntries)
                {
                    if (settingsCopy.ContainsKey(kvPair.Key))
                        continue;

                    settingsCopy[kvPair.Key] = kvPair.Value;
                }

                var tempFilePath = $"{FullConfigFilePath}.tmp";
                File.WriteAllText(tempFilePath, JsonConvert.SerializeObject(settingsCopy, Formatting.Indented));
                File.Move(tempFilePath, FullConfigFilePath, true);
            }
        }

        public void Load()
        {
            lock (_lockingObject)
            {
                _rawSettingEntries.Clear();
                if (!File.Exists(FullConfigFilePath) && !TryConvertOldSettings())
                {
                    _logger.Log("Failed to convert or load settings, starting from scratch.", LogLevel.Warning);
                    return;
                }

                var contents = File.ReadAllText(FullConfigFilePath);
                var settings = JsonConvert.DeserializeObject<Dictionary<string, JToken>>(contents);
                if (settings == null)
                    return;

                foreach (var kvPair in settings)
                    _rawSettingEntries[kvPair.Key] = kvPair.Value;
            }
        }

        private bool TryConvertOldSettings()
        {
            var iniFileName = ConfigFileName.Replace(".json", ".ini");
            var oldSettingsFilePath = Path.Combine(_saveLocation, iniFileName);
            if (!File.Exists(oldSettingsFilePath))
                return false;

            var configLines = File.ReadAllLines(oldSettingsFilePath);
            var jObject = new JObject();
            foreach (var line in configLines)
            {
                var split = line.Split('=', 2, StringSplitOptions.TrimEntries);
                jObject.Add(split[0], split[1]);
            }

            File.WriteAllText(FullConfigFilePath, jObject.ToString());
            var backupOldSettingsFilePath = Path.Combine(_saveLocation, $"unused_{iniFileName.Replace(".ini", ".backup")}");
            File.Move(oldSettingsFilePath, backupOldSettingsFilePath, true);
            return true;
        }
    }
}
