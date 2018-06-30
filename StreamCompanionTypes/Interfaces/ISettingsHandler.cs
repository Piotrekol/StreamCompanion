using System;
using System.Collections.Generic;
using StreamCompanionTypes.DataTypes;

namespace StreamCompanionTypes.Interfaces
{
    public interface ISettingsHandler
    {
        EventHandler<SettingUpdated> SettingUpdated { get; set; }
        string FullConfigFilePath { get; }
        T Get<T>(ConfigEntry entry);
        void Add<T>(string key, T value, bool raiseUpdate = false);
        void Add<T>(string key, List<T> valueList, bool raiseUpdate = false);
        string GetRaw(string key, string defaultValue = "");
        List<string> Get(string key);
        List<int> Geti(string key);
        bool Delete(ConfigEntry entry);
        bool Delete(string key);
        void Save();
        void Load();
        void SetSavePath(string path);
    }
}