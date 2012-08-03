using System.Collections.Generic;

namespace SalsaImporter.Synchronization
{
    public interface IConditonalUpdater
    {
        void MaybeUpdate<T>(T sourceObject) where T : class, ISyncObject;
    }
}