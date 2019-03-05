using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace LiveVisualizer
{
    internal static class ColorHelpers
    {
        public static string GetArgbColor(ISettingsHandler settings, ConfigEntry entry)
        => string.Join("", GetArgbColorList(settings, entry).Select(n => n.ToString("X2")));

        public static List<int> GetArgbColorList(ISettingsHandler settings, ConfigEntry entry)
            => settings.Get<string>(entry).Split(new[] { ';' }, 4).Select(v => int.TryParse(v, out int num) ? num : 0).ToList();

        public static Color GetColor(ISettingsHandler settings, ConfigEntry entry)
        {
            var argb = GetArgbColorList(settings, entry);

            return Color.FromArgb(argb[0], argb[1], argb[2], argb[3]);
        }

        public static void SaveColor(ISettingsHandler settings, ConfigEntry entry, Color color)
        {
            var colors = BitConverter.GetBytes(color.ToArgb())
                .Reverse()
                .ToList();
            
            settings.Add(entry.Name, string.Join(";", colors.Select(v => v.ToString())), true);
        }
    }
}