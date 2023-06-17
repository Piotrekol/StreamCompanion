using System;
using System.IO;

namespace osu_StreamCompanion.Code.Helpers
{
    public static class StringExtensions
    {
        public static bool TryFormat(this string str, out string result, params string[] args)
        {
            try
            {
                result = string.Format(str, args);
                return true;
            }
            catch (Exception)
            {
                result = null;
                return false;
            }
        }

        public static string RemoveInvalidFileNameChars(this string fileName)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                if (fileName.Contains(c))
                    fileName = fileName.Replace(c.ToString(), "");
            }

            return fileName;
        }
    }
}