using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Utilities
{
    public class StringUtility
    {
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
    }
}
