using System.Collections.Generic;

namespace osu_StreamCompanion.Code.Helpers
{
    public static class ListExtensions
    {

        public static bool StartsAnywhereWith(this List<string> list,string val)
        {
            foreach (var variable in list)
            {
                if (variable.StartsWith(val))
                    return true;
            }
            return false;
        }
        public static int AnyStartsWith(this IReadOnlyList<string> list, string val)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].StartsWith(val))
                    return i;
            }
            return -1;
        }
    }
}
