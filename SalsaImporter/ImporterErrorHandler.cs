using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace SalsaImporter
{
    public class ImporterErrorHandler
    {
        private ConcurrentDictionary<string, NameValueCollection> _failedRecordsToCreate = new ConcurrentDictionary<string, NameValueCollection>();
        private readonly int _abortCreateThreshold;

        public ImporterErrorHandler(int abortCreateThreshold)
        {
            _abortCreateThreshold = abortCreateThreshold;
        }

        public ConcurrentDictionary<string, NameValueCollection> FailedRecordsToCreate
        {
            get { return _failedRecordsToCreate; }
        }

        public void CanContinueToCreate(NameValueCollection data)
        {
            _failedRecordsToCreate[data["uid"]] = data;
            Logger.Error(string.Format("Failed to create supporter(id:{0})", data["uid"]));
            if(_abortCreateThreshold < _failedRecordsToCreate.Keys.Count)
            {
                string message = "Failure to create supporters exceeded the threshold. Process aborted. Threshold:" + _abortCreateThreshold;
                Logger.Fatal(message);
                throw new OperationCanceledException(message);
            }

        }
    }
}
