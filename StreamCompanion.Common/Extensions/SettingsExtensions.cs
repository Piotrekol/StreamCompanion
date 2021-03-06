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

        public static string GetFullSongsLocation(ISettings settings)
        {
            var dir = settings.Get<string>(_names.SongsFolderLocation);
            if (dir == _names.SongsFolderLocation.Default<string>())
            {
                dir = settings.Get<string>(_names.MainOsuDirectory);
                dir = Path.Combine(dir, "Songs\\");
            }
            return dir;
        }

        public static T GetConfiguration<T>(this ISettings settings, ConfigEntry configEntry) where T : new()
        {
            var rawConfiguration = settings.Get<string>(configEntry);
            return rawConfiguration == configEntry.Default<string>()
                ? new T()
                : JsonConvert.DeserializeObject<T>(rawConfiguration, new JsonSerializerSettings
                {
                    ObjectCreationHandling = ObjectCreationHandling.Replace
                });
        }

        public static void SaveConfiguration<T>(this ISettings settings, ConfigEntry configEntry, T configuration)
        {
            settings.Add(configEntry.Name, JsonConvert.SerializeObject(configuration));
        }
    }
}