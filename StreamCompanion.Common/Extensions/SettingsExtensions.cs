using System.IO;
using Newtonsoft.Json;
using StreamCompanionTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces.Services;

namespace StreamCompanion.Common
{
    public static class SettingsExtensions
    {

        private static readonly SettingNames _names = SettingNames.Instance;

        public static string GetFullSongsLocation(this ISettings settings)
        {
            var dir = settings.Get<string>(_names.SongsFolderLocation);
            if (dir == _names.SongsFolderLocation.Default<string>())
            {
                dir = settings.Get<string>(_names.MainOsuDirectory);
                dir = Path.Combine(dir, "Songs\\");
            }
            return dir;
        }

        public static string GetFullSkinsLocation(this ISettings settings) => Path.Combine(settings.Get<string>(_names.MainOsuDirectory), "Skins\\");

        public static string GetFullOsuLocation(this ISettings settings) => settings.Get<string>(_names.MainOsuDirectory);

        public static T GetConfiguration<T>(this ISettings settings, ConfigEntry configEntry) where T : new()
        {
            T configValue = default;
            bool exception = false;
            try
            {
                configValue = settings.Get<T>(configEntry);
            }
            catch
            {
                exception = true;
            }

            if (configValue == null || exception)
            {
                var rawValue = settings.Get<string>(configEntry);
                if (!string.IsNullOrWhiteSpace(rawValue))
                {
                    configValue = JsonConvert.DeserializeObject<T>(rawValue);
                    //Replace json string blob with actual json object
                    if (configValue != null)
                        settings.Add(configEntry, configValue);
                }
            }

            if (configValue == null)
            {
                configValue = new T();
                settings.Add(configEntry, configValue);
            }

            return configValue;
        }

        public static void SaveConfiguration<T>(this ISettings settings, ConfigEntry configEntry, T configuration)
        {
            settings.Add(configEntry.Name, configuration);
        }
    }
}