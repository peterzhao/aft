
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Utilities
{
    public static class StringUtility
    {
        public static bool EqualsIgnoreCase(this string text, string other)
        {
            if (text == null || other == null) return text == other;
            return text.ToLower() == other.ToLower();
        }
        public static bool EqualsIncludingNullEmpty(string string1, string string2)
        {
            if (string.IsNullOrWhiteSpace(string1))
            {
                if (string.IsNullOrWhiteSpace(string2))
                {
                    return true;
                }
                return false;
            }
            if (string.IsNullOrWhiteSpace(string2))
            {
                return false;
            }
            // Both not null or whitespace...
            return string1.Trim().Equals(string2.Trim());
        }

        public static bool EqualsIncludingNullAndSpecifiedvalue(string string1, string string2, string ignoredValue)
        {
            if (string1 == ignoredValue) string1 = null;
            if (string2 == ignoredValue) string2 = null;
            return EqualsIncludingNullEmpty(string1, string2);
        }
    }
}
