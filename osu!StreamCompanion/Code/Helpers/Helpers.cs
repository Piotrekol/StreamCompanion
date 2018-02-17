using System;
using System.Threading.Tasks;

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
            try
            {
                return DateTime.ParseExact(version.TrimStart('v'), "yyMMdd.HH",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
            catch
            {
                return DateTime.ParseExact(version.TrimStart('v'), "yyMMdd",
                    System.Globalization.CultureInfo.InvariantCulture);
            }
        }

        public static T ExecWithTimeout<T>(Func<T> function, int timeout = 10000)
        {
            var task = Task<T>.Factory.StartNew(function);
            if (task.Wait(TimeSpan.FromMilliseconds(timeout)))
                return task.Result;
            else
                return default(T);
        }
    }
}
