using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
        [DebuggerStepThrough()]
        public static DateTime GetDateFromVersionString(string version)
        {
            if(version=="N/A")
                return DateTime.MinValue;
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
        [DebuggerStepThrough()]
        public static T ExecWithTimeout<T>(Func<T> function, int timeout = 10000)
        {
            var task = Task<T>.Factory.StartNew(function);
            if (task.Wait(TimeSpan.FromMilliseconds(timeout)))
                return task.Result;
            else
                return default(T);
        }
        [DebuggerStepThrough()]
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
        /// <summary>
        /// Returns a new string in which all occurrences of a specified string in the current instance are replaced with another 
        /// specified string acording the type of search to use for the specified string.
        /// </summary>
        /// <param name="str">The string performing the replace method.</param>
        /// <param name="oldValue">The string to be replaced.</param>
        /// <param name="newValue">The string replace all occurrances of oldValue.</param>
        /// <param name="comparisonType">One of the enumeration values that specifies the rules for the search. </param>
        /// <returns>A string that is equivalent to the current string except that all instances of oldValue are replaced with newValue.
        ///  If oldValue is not found in the current instance, the method returns the current instance unchanged. </returns>
        [DebuggerStepThrough()]
        public static string Replace(this string str,
            string oldValue, string @newValue,
            StringComparison comparisonType)
        {

            //Check inputs
            //Same as original .NET C# string.Replace behaviour
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }
            if (str.Length == 0)
            {
                return str;
            }
            if (oldValue == null)
            {
                throw new ArgumentNullException(nameof(oldValue));
            }
            if (oldValue.Length == 0)
            {
                throw new ArgumentException("String cannot be of zero length.");
            }
            
            @newValue = @newValue ?? string.Empty;
            
            const int valueNotFound = -1;
            int foundAt, startSearchFromIndex = 0;
            while ((foundAt = str.IndexOf(oldValue, startSearchFromIndex, comparisonType)) != valueNotFound)
            {

                str = str.Remove(foundAt, oldValue.Length)
                    .Insert(foundAt, @newValue);
                
                startSearchFromIndex = foundAt + @newValue.Length;
                if (startSearchFromIndex == str.Length)
                {
                    break;
                }
            }

            return str;
        }

        public static float Lerp(float firstValue, float secondValue, float by)
        {
            return firstValue * by + secondValue * (1 - by);
        }
        public static double Lerp(double firstValue, float secondValue, float by)
        {
            return firstValue * by + secondValue * (1 - by);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">File to check</param>
        /// <param name="timeout">Timeout, in ms</param>
        /// <returns></returns>
        public static bool FileIsLocked(FileInfo file, int timeout)
        {
            var result = ExecWithTimeout(() =>
            {
                try
                {
                    while (FileIsLocked(file))
                    {
                        Thread.Sleep(1);
                    }
                }
                catch (DirectoryNotFoundException)
                { return true; }
                catch (FileNotFoundException)
                { return true; }
                return false;
            }, timeout);
            return result;

        }
        public static bool FileIsLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        public static void WaitForOsuFileLock(FileInfo file)
        {
            //If we acquire lock before osu it'll force "soft" beatmap reprocessing(no data loss, but time consuming).
            var isLocked = ExecWithTimeout(() =>
            {
                while (!FileIsLocked(file))
                {
                    Thread.Sleep(1);
                }
                return true;
            }, 50);

            if (isLocked)
            {
                while (FileIsLocked(file))
                {
                    Thread.Sleep(1);
                }
            }
        }
    }
}
