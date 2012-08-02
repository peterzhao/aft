using System;
using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    public class Sync
    {
        private readonly SyncErrorHandler _errorHandler;
        private readonly SupporterMapper _mapper;
        private readonly SalsaClient _salsa;


        public Sync()
        {
            _errorHandler = new SyncErrorHandler(500, 500, 500);
            _salsa = new SalsaClient(_errorHandler);
            _mapper = new SupporterMapper();
            _salsa.Login();
        }

        public void PushToSalsa()
        {

            PrintFailedRecords();
        }

        public void DeleteAllSupporters()
        {
            _salsa.DeleteAllObjects("supporter", 100, true);
        }

        public void CountSupportOnSalsa()
        {
            int count = _salsa.SupporterCount();
            Logger.Info("total supporter on salsa:" + count);
        }


        public void PullFromSalsa()
        {

        }


        private void PrintFailedRecords()
        {
            List<ISyncObject> failedCreatedSupporterKeys = _errorHandler.PullingFailure.Keys.ToList();
            if (failedCreatedSupporterKeys.Count > 0)
            {
                var message = "";
                failedCreatedSupporterKeys.ForEach(k => message += k + " ");
                Logger.Error(String.Format("There are {0} supporters failed to push to Salsa. {1}",
                                           failedCreatedSupporterKeys.Count, message));
            }
        }

       
    }
}
