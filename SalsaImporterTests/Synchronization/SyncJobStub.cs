using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Synchronization;

namespace SalsaImporterTests.Synchronization
{
    public class SyncJobStub:ISyncJob
    {
        private readonly Action<JobContext> _startAction;

        public string Name { get; private set; }
        public string ObjectType { get { return ""; } }

        public SyncJobStub(string name, Action<JobContext> startAction)
        {
            _startAction = startAction;
            Name = name;
        }

        public void Start(IJobContext jobContext)
        {
            _startAction((JobContext) jobContext);
        }
    }
}
