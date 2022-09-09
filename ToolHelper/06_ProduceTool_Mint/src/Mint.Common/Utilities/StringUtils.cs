namespace Mint.Common
{
    using System;
    using System.Text.RegularExpressions;

    public static class StringUtils
    {
        public static bool EqualsIgnoreCase(string s1, string s2)
        {
            return string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool StartsWithIgnoreCase(string s, string v)
        {
            return s.StartsWith(v, StringComparison.OrdinalIgnoreCase);
        }

        public static bool EndsWithIgnoreCase(string s, string v)
        {
            return s.EndsWith(v, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(string s, string v)
        {
            return s.Contains(v, StringComparison.OrdinalIgnoreCase);
        }

        public static string ReplaceIgnoreCase(string s, string v, string r)
        {
            return Regex.Replace(s, Regex.Escape(v), r.Replace("$","$$"), RegexOptions.IgnoreCase);
        }
    }
}
