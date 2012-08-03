using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    public class SyncJobStub:ISyncJob
    {
        private readonly Action<IJobContext> _startAction;

        public string Name { get; private set; }

        public SyncJobStub(string name, Action<IJobContext> startAction)
        {
            _startAction = startAction;
            Name = name;
        }

        public void Start(IJobContext jobContext)
        {
            _startAction(jobContext);
        }
    }
}
