using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace SalsaImporter.Utilities
{
    public static class XElementExtension
    {
        public static string StringValueOrNull(this XElement root, string elementName)
        {
            XElement xElement = root.Element(elementName);
            return xElement == null? null: xElement.Value;
        }

        public static int? IntValueOrNull(this XElement root, string elementName)
        {
            int? result = null;
            XElement xElement = root.Element(elementName);
            if (xElement != null && !string.IsNullOrWhiteSpace(xElement.Value)) result = int.Parse(xElement.Value);
            return result;
        }

        public static float? FloatValueOrNull(this XElement root, string elementName)
        {
            float? result = null;
            XElement xElement = root.Element(elementName);
            if( xElement != null && !string.IsNullOrWhiteSpace(xElement.Value) ) result = float.Parse(xElement.Value);
            return result;
        }

        public static DateTime? DateTimeValueOrNull(this XElement root, string elementName)
        {
            DateTime? result = null;
            XElement xElement = root.Element(elementName);
            var formats = "ddd MMM dd yyyy HH:mm:ss 'GMT'zz'00'";
            if( xElement != null && !string.IsNullOrWhiteSpace(xElement.Value) )
            {
                string value = xElement.Value;
                result = DateTime.ParseExact(value.Remove(value.Length - 6, 6), formats, null);
            }
            return result;
        }

        
        
    }
}
