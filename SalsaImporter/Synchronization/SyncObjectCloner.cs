using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Aft
{
    public class SyncObjectCloner
    {
        public T Clone<T>(T input) where T : class, ISyncObject, new()
        {
            var cloned = new T();
            foreach (var property in input.GetType().GetProperties())
            {
                if (!property.CanWrite || !property.CanRead) continue;
                var value = property.GetValue(input, null);
                property.SetValue(cloned, value, null);
            }
            return cloned;
        }
    }
}
