using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalsaImporter.Utilities
{
    public static class DateTimeUtility
    {
        public static bool EqualsIgnoreMillionSeconds(this DateTime? dt, DateTime? other)
        {
            if (dt == null && other == null) return true;
            if (dt != null && other == null) return false;
            if (dt == null) return false;
            var thisValue = dt.Value;
            var otherValue = other.Value;
            return thisValue.Year == otherValue.Year &&
                   thisValue.Month == otherValue.Month &&
                   thisValue.Day == otherValue.Day &&
                   thisValue.Hour == otherValue.Hour &&
                   thisValue.Minute == otherValue.Minute &&
                   thisValue.Second == otherValue.Second;
        }
    }
}
