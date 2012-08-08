using System;
using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    public class Sync
    {
        private readonly SyncErrorHandler _errorHandler;
        private readonly SalsaClient _salsa;


        public Sync()
        {
            _errorHandler = new SyncErrorHandler(500);
            _salsa = new SalsaClient(_errorHandler);
            _salsa.Login();
        }

        public void Run()
        {
            var syncErrorHandler = new SyncErrorHandler(10);

            var localRepository = new LocalRepository();
            var salsaRepository = new SalsaRepository(new SalsaClient(syncErrorHandler), new MapperFactory());

            var localConditionalUpdater = new LocalUpdater(localRepository, syncErrorHandler);
            var salsaConditionalUpdater = new ExternalUpdater(salsaRepository, localRepository, syncErrorHandler);

            var pullJob = new BatchOneWaySyncJob<Supporter>(salsaRepository, localConditionalUpdater, 100, "Pulling supporters");
            var pushJob = new BatchOneWaySyncJob<Supporter>(localRepository, salsaConditionalUpdater, 100, "Push supporters");

            var syncSession = new SyncSession();
            syncSession.AddJob(pullJob).AddJob(pushJob);
            syncSession.Start();
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

        private void PrintFailedRecords()
        {
            List<ISyncObject> failedCreatedSupporterKeys = _errorHandler.Failures.Keys.ToList();
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
