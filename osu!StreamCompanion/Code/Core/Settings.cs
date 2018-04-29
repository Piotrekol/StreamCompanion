using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Core
{
    public class Settings
    {
        private readonly Dictionary<string, object> _settingsEntries = new Dictionary<string, object>();
        private ILogger _logger;
        private readonly List<string> _rawLines = new List<string>();
        private readonly List<string> _backupRawLines = new List<string>();
        public EventHandler<SettingUpdated> SettingUpdated;
        private string saveLocation;
        private readonly string configFileName = "settings.ini";
        public string FullConfigFilePath { get { return Path.Combine(saveLocation, configFileName); } }
        private static readonly object _lockingObject = new object();
        public Settings(ILogger logger)
        {
            _logger = logger;
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

        public string GetRaw(string key, string defaultValue="")
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
                            return (T)Convert.ChangeType(_settingsEntries[key], typeof(T));
                        }
                        catch (InvalidCastException)
                        {
                            _logger.Log("Warning: Had to use default value on {0} (InvalidCastException){1}::{2}",
                                LogLevel.Basic, key, _settingsEntries[key].GetType().FullName, typeof(T).FullName);
                            return default(T);
                        }
                    }
                }

                int idx = _rawLines.AnyStartsWith(key);
                if (idx > -1)
                {
                    string[] splited = _rawLines[idx].Split(new[] { '=' }, 2);
                    Add(key, Convert.ChangeType(splited[1].Trim(), typeof(T)));
                    _rawLines.RemoveAt(idx);
                }
                else
                    Add(key, defaultValue);
                return Get(key, defaultValue);
            }
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
                if (!Directory.Exists(saveLocation))
                {
                    Directory.CreateDirectory(saveLocation);
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

        public void SetSavePath(string path)
        {
            saveLocation = path;
        }
    }
}
