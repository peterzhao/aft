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
            if (string.IsNullOrEmpty(string1) && string.IsNullOrEmpty(string2)) return true;
            return string1 != null && string1.Equals(string2);

        }
    }
}
