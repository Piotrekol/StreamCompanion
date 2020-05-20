using System.IO;
using StreamCompanionTypes.DataTypes;

namespace OsuSongsFolderWatcher
{
    public static class BeatmapHelpers
    {
        public static Beatmap ReadBeatmap(string fullPath)
            => OsuFileParser.Instance.ReadBeatmapData(fullPath);

        public static string GetDiffFromString(string msnString)
        {
            if (msnString.Contains("]") && msnString.Contains("["))
            {
                var openBr = 0;
                var closedBr = 0;
                var strPos = msnString.Length - 1;
                do
                {
                    var character = msnString[strPos];
                    switch (character)
                    {
                        case ']':
                            closedBr++;
                            break;
                        case '[':
                            openBr++;
                            break;
                    }
                    strPos--;
                } while (closedBr != openBr);

                return msnString.Substring(strPos + 2, msnString.Length - strPos - 3);
            }
            return string.Empty;
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
    }
}