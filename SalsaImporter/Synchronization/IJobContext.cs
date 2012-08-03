using System;

namespace SalsaImporter.Synchronization
{
    public interface IJobContext
    {
        DateTime MinimumModificationDate { get; }
        int CurrentRecord { get; set; }
        void MarkComplete(); //todo: remove it
    }
}