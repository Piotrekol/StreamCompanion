using System;

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
    }
}