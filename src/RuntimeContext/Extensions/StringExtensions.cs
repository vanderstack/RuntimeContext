namespace Vanderstack.RuntimeContext.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Gets the substring which occurs before the first instance of the needle.
        /// </summary>
        public static string Before(this string haystack, string needle)
        {
            var needlePosition = haystack.IndexOf(needle);

            return needlePosition == -1
                ? ""
                : haystack.Substring(0, needlePosition);
        }

        /// <summary>
        /// Gets the substring which occurs before the last instance of the needle.
        /// </summary>
        public static string BeforeLast(this string haystack, string needle)
        {
            var needlePosition = haystack.LastIndexOf(needle);

            return needlePosition == -1
                ? ""
                : haystack.Substring(0, needlePosition);
        }

        /// <summary>
        /// Gets the substring which occurs after the last instance of the needle.
        /// </summary>
        public static string AfterLast(this string haystack, string needle)
        {
            var needlePosition = haystack.LastIndexOf(needle);
            if (needlePosition == -1) { return ""; }

            var startingIndex = needlePosition + needle.Length;
            var endingIndex = haystack.Length - startingIndex;

            return haystack.Substring(startingIndex, endingIndex);
        }
    }
}
