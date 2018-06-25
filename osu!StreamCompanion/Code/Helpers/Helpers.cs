using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CollectionManager.DataTypes;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;
using Beatmap = OppaiSharp.Beatmap;

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
            if (version == "N/A")
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file">File to check</param>
        /// <param name="timeout">Timeout, in ms</param>
        /// <returns></returns>
        public static bool FileIsLocked(FileInfo file, int timeout)
        {
            var result = ExecWithTimeout(token =>
            {
                try
                {
                    while (FileIsLocked(file))
                    {
                        if (token.IsCancellationRequested)
                            return true;
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
        [DebuggerStepThrough()]
        public static T ExecWithTimeout<T>(Func<CancellationToken, T> function, int timeout = 10000, ILogger logger = null)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            var token = cancellationTokenSource.Token;
            var task = new Task<T>(() => function(token));
            task.Start();
            if (task.Wait(TimeSpan.FromMilliseconds(timeout)))
            {
                logger?.Log("task finished", LogLevel.Debug);
                return task.Result;
            }
            cancellationTokenSource.Cancel();
            logger?.Log("task aborted", LogLevel.Debug);
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
        

        public static float Lerp(float firstValue, float secondValue, float by)
        {
            return firstValue * by + secondValue * (1 - by);
        }
        public static double Lerp(double firstValue, float secondValue, float by)
        {
            return firstValue * by + secondValue * (1 - by);
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

        public static Beatmap GetOppaiSharpBeatmap(string mapLocation)
        {
            bool retry = true;
            Beatmap beatmap = null;
            do
            {
                try
                {
                    using (var stream = new FileStream(mapLocation, FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            beatmap = Beatmap.Read(reader);
                            retry = false;
                        }
                    }
                }
                catch
                {
                    if (!File.Exists(mapLocation))
                        return null;
                }
            } while (retry);

            return beatmap;
        }

        public static bool SafeHasExited(this Process process)
        {

            try
            {
                return process.HasExited;
            }
            catch
            {
                return true;
            }
        }

        public static void WaitForOsuFileLock(FileInfo file, ILogger logger = null, int Id = 0)
        {
            //If we acquire lock before osu it'll force "soft" beatmap reprocessing(no data loss, but time consuming).
            logger?.Log($"{Id}: osu release: wait start", LogLevel.Debug);
            var startTime = DateTime.Now;
            var isLocked = ExecWithTimeout(token =>
            {
                while (!FileIsLocked(file))
                {
                    if (token.IsCancellationRequested)
                        return false;
                    Thread.Sleep(1);
                }
                return true;
            }, 500, logger);
            var diff = (DateTime.Now - startTime).TotalMilliseconds;
            logger?.Log($"{Id}: osu release: wait end - {diff}ms", LogLevel.Debug);

            logger?.Log($"{Id}: isLocked:{isLocked}", LogLevel.Debug);

            if (isLocked)
            {
                startTime = DateTime.Now;
                int cycles = 0;
                while (FileIsLocked(file))
                {
                    cycles++;
                    Thread.Sleep(1);
                }
                diff = (DateTime.Now - startTime).TotalMilliseconds;
                logger?.Log($"{Id}: osu lock: released after {diff}ms, {cycles}loops", LogLevel.Debug);

            }
        }
    }
}
