using System;

namespace StreamCompanion.Common.Helpers
{
    public static class Retry
    {
        //We don't need Polly here..yet
        public static T RetryMe<T>(Func<T> func, Func<T, bool> isValid, int maxRetries)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                var result = func();

                if (isValid(result))
                    return result;
            }

            return default;
        }

        public static TV RetryMe<TV, T>(Func<T> func, Func<T, (bool, TV)> isValid, int maxRetries)
        {
            for (int i = 0; i < maxRetries; i++)
            {
                var result = func();

                var validResult = isValid(result);
                if (validResult.Item1)
                    return validResult.Item2;
            }

            return default;
        }
    }
}