using System;
using System.Diagnostics;

namespace StreamCompanionTypes
{
    public static class Helpers
    {
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
    }
}