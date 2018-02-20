using System;
using System.Threading.Tasks;
using CollectionManager.DataTypes;

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

        public static OppaiSharp.Mods Convert(this Mods mods)
        {
            OppaiSharp.Mods result = OppaiSharp.Mods.NoMod;
            if ((Mods.Nf & mods) != 0)
                result |= OppaiSharp.Mods.NoFail;
            if ((Mods.Ez & mods) != 0)
                result |= OppaiSharp.Mods.Easy;
            //if ((Mods.TD & mods) != 0)
            //result |= OppaiSharp.Mods.TouchDevice;
            if ((Mods.Hd & mods) != 0)
                result |= OppaiSharp.Mods.Hidden;
            if ((Mods.Hr & mods) != 0)
                result |= OppaiSharp.Mods.Hardrock;
            if ((Mods.Dt & mods) != 0)
                result |= OppaiSharp.Mods.DoubleTime;
            if ((Mods.Ht & mods) != 0)
                result |= OppaiSharp.Mods.HalfTime;
            if ((Mods.Nc & mods) != 0)
                result |= OppaiSharp.Mods.Nightcore;
            if ((Mods.Fl & mods) != 0)
                result |= OppaiSharp.Mods.Flashlight;
            if ((Mods.So & mods) != 0)
                result |= OppaiSharp.Mods.SpunOut;
            return result;
        }
    }
}
