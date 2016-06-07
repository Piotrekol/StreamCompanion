using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using osu_StreamCompanion.Code.Core.DataTypes;
using osu_StreamCompanion.Code.Helpers;
using osu_StreamCompanion.Code.Interfeaces;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Core
{
    public class Settings
    {
        private readonly Dictionary<string, object> _settingsEntries = new Dictionary<string, object>();
        private ILogger _logger;
        private readonly List<string> _rawLines = new List<string>();
        public EventHandler<SettingUpdated> SettingUpdated;
        private string saveLocation;
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
            return this.Get<T>(entry.Name, entry.Default<T>());
        }
        public void Add<T>(string key, T value, bool raiseUpdate = false)
        {
            _settingsEntries[key] = value;
            if (raiseUpdate)
                OnSettingUpdated(key);
        }

        public void Add<T>(string key, List<T> valueList, bool raiseUpdate = false)
        {
            string vals = string.Join("|,~", valueList);

            Add<string>(key, vals, raiseUpdate);
        }

        public List<string> Get(string key)
        {
            string savedValue = Get<string>(key, "");
            var ret = savedValue.Split(new[] { "|,~" }, StringSplitOptions.None).ToList();
            if (ret[0] == string.Empty)
                ret.RemoveAt(0);
            return ret;
        }
        public List<int> Geti(string key)
        {
            string savedValue = Get<string>(key, "");
            var ret = new List<int>();
            if (!string.IsNullOrEmpty(savedValue))
                ret =
                        savedValue.Split(new[] { "|,~" }, StringSplitOptions.None)
                            .ToList().ConvertAll(int.Parse);


            return ret;
        }
        private T Get<T>(string key, T defaultValue)
        {
            if (_settingsEntries.ContainsKey(key))
            {
                if (_settingsEntries[key] is T)
                {
                    return (T)_settingsEntries[key];
                }
                else {
                    try
                    {
                        return (T)Convert.ChangeType(_settingsEntries[key], typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        _logger.Log("Warning: Had to use default value on {0} (InvalidCastException){1}::{2}", LogLevel.Basic, key, _settingsEntries[key].GetType().FullName, typeof(T).FullName);
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
        public void Save()
        {
            if (_rawLines.Count > 0)
            {
                for (int i = 0; i < _rawLines.Count; i++)
                {
                    string[] splited = _rawLines[i].Split(new[] { '=' }, 2);
                    Add(splited[0].Trim(), Convert.ChangeType(splited[1].Trim(), typeof(string)));
                }
                _rawLines.Clear();
            }
            StringBuilder stringBuilder = new StringBuilder();
            foreach (var entry in _settingsEntries)
            {
                stringBuilder.AppendFormat("{0} = {1}{2}", entry.Key, entry.Value, Environment.NewLine);
            }

            using (var fileHandle = new StreamWriter(saveLocation))
            {
                fileHandle.Write(stringBuilder);
            }
        }
        public void Load(string fullPath)
        {
            if (File.Exists(fullPath))
                using (var fileHandle = new StreamReader(fullPath, true))
                {
                    while (!fileHandle.EndOfStream)
                    {
                        _rawLines.Add(fileHandle.ReadLine());
                    }
                }
        }

        public void SetSavePath(string path)
        {
            saveLocation = path;
        }
    }
}
