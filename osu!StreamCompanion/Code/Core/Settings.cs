using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using osu_StreamCompanion.Code.Helpers;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using StreamCompanionTypes.Enums;
using StreamCompanionTypes.Interfaces.Services;

namespace osu_StreamCompanion.Code.Core
{

    public class Settings : ISettings
    {
        private readonly Dictionary<string, object> _settingsEntries = new Dictionary<string, object>();
        public IReadOnlyDictionary<string, object> SettingsEntries { get; }
        private ILogger _logger;
        private readonly List<string> _rawLines = new List<string>();
        private readonly List<string> _backupRawLines = new List<string>();
        public EventHandler<SettingUpdated> SettingUpdated { get; set; }
        private string _saveLocation = AppDomain.CurrentDomain.BaseDirectory;
        public string ConfigFileName { get; set; } = "settings.ini";
        public string FullConfigFilePath { get { return Path.Combine(_saveLocation, ConfigFileName); } }
        private static readonly object _lockingObject = new object();
        public Settings(ILogger logger)
        {
            _logger = logger;
            SettingsEntries = new ReadOnlyDictionary<string, object>(_settingsEntries);
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
                _settingsEntries[key] = value;
                if (raiseUpdate)
                    OnSettingUpdated(key);
            }
        }

        public void Add<T>(string key, List<T> valueList, bool raiseUpdate = false)
        {
            lock (_lockingObject)
            {
                string vals = string.Join("|,~", valueList);

                Add<string>(key, vals, raiseUpdate);
            }
        }

        public string GetRaw(string key, string defaultValue = "")
        {
            int idx = _backupRawLines.AnyStartsWith(key);
            if (idx > -1)
            {
                string[] splited = _backupRawLines[idx].Split(new[] { '=' }, 2);
                return splited[1].Trim();
            }
            return defaultValue;

        }
        public List<string> Get(string key)
        {
            lock (_lockingObject)
            {
                string savedValue = Get<string>(key, "");
                var ret = savedValue.Split(new[] { "|,~" }, StringSplitOptions.None).ToList();
                if (ret[0] == string.Empty)
                    ret.RemoveAt(0);
                return ret;
            }
        }
        public List<int> Geti(string key)
        {
            lock (_lockingObject)
            {
                string savedValue = Get<string>(key, "");
                var ret = new List<int>();
                if (!string.IsNullOrEmpty(savedValue))
                    ret =
                        savedValue.Split(new[] { "|,~" }, StringSplitOptions.None)
                            .ToList().ConvertAll(int.Parse);


                return ret;
            }
        }
        private T Get<T>(string key, T defaultValue)
        {
            lock (_lockingObject)
            {
                var nullableUnderlyingType = Nullable.GetUnderlyingType(typeof(T));
                if (_settingsEntries.ContainsKey(key))
                {
                    if (_settingsEntries[key] is T)
                    {
                        return (T)_settingsEntries[key];
                    }
                    else
                    {
                        try
                        {
                            if (nullableUnderlyingType != null && _settingsEntries[key] == null)
                                return default;

                            return (T)Convert.ChangeType(_settingsEntries[key], typeof(T));
                        }
                        catch (InvalidCastException)
                        {
                            _logger.Log("Warning: Had to use default value on {0} (InvalidCastException){1}::{2}",
                                LogLevel.Information, key, _settingsEntries[key].GetType().FullName, typeof(T).FullName);
                            return default(T);
                        }
                    }
                }

                int idx = _rawLines.AnyStartsWith(key);
                if (idx > -1)
                {
                    string[] splited = _rawLines[idx].Split(new[] { '=' }, 2);
                    if (nullableUnderlyingType != null && string.IsNullOrWhiteSpace(splited[1].Trim()))
                        Add(key, default(T));
                    else
                        Add(key, Convert.ChangeType(splited[1].Trim(), nullableUnderlyingType ?? typeof(T)));
                    _rawLines.RemoveAt(idx);
                }
                else
                    Add(key, defaultValue);
                return Get(key, defaultValue);
            }
        }

        public bool Delete(ConfigEntry entry) => Delete(entry.Name);

        public bool Delete(string key)
        {
            if (_settingsEntries.ContainsKey(key))
            {
                _settingsEntries.Remove(key);
                return true;
            }
            return false;
        }
        public void Save()
        {
            lock (_lockingObject)
            {
                if (_rawLines.Count > 0)
                {
                    foreach (string line in _rawLines)
                    {
                        string[] splited = line.Split(new[] { '=' }, 2);
                        if (splited.Length == 2)
                            Add(splited[0].Trim(), Convert.ChangeType(splited[1].Trim(), typeof(string)));
                    }
                    _rawLines.Clear();
                }
                StringBuilder stringBuilder = new StringBuilder();
                foreach (var entry in _settingsEntries)
                {
                    stringBuilder.AppendFormat("{0} = {1}{2}", entry.Key, entry.Value, Environment.NewLine);
                }
                if (!Directory.Exists(_saveLocation))
                {
                    Directory.CreateDirectory(_saveLocation);
                }
                using (var fileHandle = new StreamWriter(FullConfigFilePath))
                {
                    fileHandle.Write(stringBuilder);
                }
            }
        }
        public void Load()
        {
            lock (_lockingObject)
            {
                var filePath = FullConfigFilePath;
                if (File.Exists(filePath))
                    using (var fileHandle = new StreamReader(filePath, true))
                    {
                        while (!fileHandle.EndOfStream)
                        {
                            string line = fileHandle.ReadLine();
                            _rawLines.Add(line);
                            _backupRawLines.Add(line);
                        }
                    }
            }
        }
    }
}
