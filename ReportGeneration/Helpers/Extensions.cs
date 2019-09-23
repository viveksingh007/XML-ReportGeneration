using System;

namespace ReportGeneration.Helpers
{
    /// <summary>
    /// Extensions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Determines whether this instance contains the object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="value">The value.</param>
        /// <param name="stringComparison">The string comparison.</param>
        /// <returns>
        ///   <c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains(this string source, string value, StringComparison stringComparison = StringComparison.CurrentCultureIgnoreCase)
        {
            return source.IndexOf(value, stringComparison) >= 0;
        }
    }
}
