using System;
using System.Collections.Generic;

namespace KSynthLib.Common
{
    public static class StringExtensions
    {
        public static string[] Split(this string value, int desiredLength, bool strict = false)
        {
            if (value.Length == 0)
            {
                return new string[0];
            }

            int numberOfItems = value.Length / desiredLength;
            int remaining = (value.Length > numberOfItems * desiredLength) ? 1 : 0;

            var split = new List<string>(numberOfItems + remaining);

            for (var i = 0; i < numberOfItems; i++)
            {
                split.Add(value.Substring(i * desiredLength, desiredLength));
            }

            if (remaining != 0)
            {
                split.Add(value.Substring(numberOfItems * desiredLength));
            }

            return split.ToArray();
        }

        /// <summary>
        /// Receives string and returns the string with its characters in reverse order.
        /// </summary>
        public static string Reversed(this string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
}