namespace Mint.Common.Extensions
{
    using System;
    using System.Text.RegularExpressions;

    public static class StringExtension
    {
        /// <summary>
        /// Determines whether two specified strings have the same value.
        /// Case-insensitive.
        /// </summary>
        public static bool EqualsIgnoreCase(this string a, string? b)
        {
            return a == b
                ? true
                : string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the beginning of this string instance matches the specified string.
        /// Case-insensitive.
        /// </summary>
        public static bool StartsWithIgnoreCase(this string s, string v)
        {
            return s.StartsWith(v, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether the end of this string matches the specified string.
        /// Case-insensitive.
        /// </summary>
        public static bool EndsWithIgnoreCase(this string s, string v)
        {
            return s.EndsWith(v, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines whether a specified string occurs within this string.
        /// Case-insensitive.
        /// </summary>
        public static bool ContainsIgnoreCase(this string s, string v)
        {
            return s.Contains(v, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Replaces all strings that match a specified string with a specified replacement string.
        /// Case-insensitive.
        /// </summary>
        public static string ReplaceIgnoreCase(this string s, string? v, string? r)
        {
            return (v == null || r == null)
                ? s
                : Regex.Replace(s, Regex.Escape(v), r.Replace("$", "$$"), RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Splits an input string into an array of substrings.
        /// Case-insensitive.
        /// </summary>
        public static string[] SplitIgnoreCase(this string s, string? v)
        {
            return Regex.Split(s, Regex.Escape(v), RegexOptions.IgnoreCase);
        }
    }
}
