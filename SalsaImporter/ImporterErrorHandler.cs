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

        public bool CanContinueToCreate(NameValueCollection data1)
        {
            _failedRecordsToCreate[data1["uid"]] = data1;
            Logger.Error(string.Format("Failed to create supporter(id:{0})", data1["uid"]));
            return _failedRecordsToCreate.Keys.Count <= _abortCreateThreshold;
        }
    }
}
