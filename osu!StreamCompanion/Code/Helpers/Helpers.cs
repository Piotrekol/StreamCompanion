using System;

namespace osu_StreamCompanion.Code.Helpers
{
    public static class Helpers
    {
        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static DateTime GetDateFromVersionString(string version)
        {
            return DateTime.ParseExact(version.TrimStart('v'), "yyMMdd.HH", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
