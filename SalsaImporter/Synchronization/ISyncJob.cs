using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Synchronization
{
    public interface ISyncJob
    {
        string Name { get; }
        void Start(IJobContext jobContext);
    }
}
