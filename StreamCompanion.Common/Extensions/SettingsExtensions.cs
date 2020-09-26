using System.IO;
using StreamCompanionTypes;
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
    }
}