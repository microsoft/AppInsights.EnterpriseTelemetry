using System.Linq;
using System.Collections.Generic;

namespace System
{
    public static class StringExtension
    {
        /// <summary>
        /// Splits a string into given chunk sizes
        /// </summary>
        /// <param name="str">Original string</param>
        /// <param name="chunkSize">Size of each</param>
        /// <returns>List of strings chunked from the original string</returns>
        public static List<string> Split(this string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(chunkIndex => str.Substring(chunkIndex * chunkSize, chunkSize))
                .ToList();
        }
    }
}
