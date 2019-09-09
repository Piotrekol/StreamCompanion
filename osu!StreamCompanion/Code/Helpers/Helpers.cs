using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StreamCompanionTypes.DataTypes;
using StreamCompanionTypes.Interfaces;

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
