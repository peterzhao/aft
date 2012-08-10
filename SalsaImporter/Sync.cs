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
        private readonly ISyncErrorHandler _errorHandler;
        private readonly ISalsaClient _salsaClient;
        private readonly ISyncEventTracker _syncEventTracker;
        private readonly ISyncObjectRepository _localRepository;
        private readonly ISyncObjectRepository _salsaRepository;
        private readonly IObjectUpdater _localConditionalUpdater;
        private readonly IObjectUpdater _salsaConditionalUpdater;

        public Sync()
        {
            _errorHandler = new SyncErrorHandler(10);
            _salsaClient = new SalsaClient();
            _syncEventTracker = new SyncEventTracker();
            _localRepository = new LocalRepository();
            _salsaRepository = new SalsaRepository(_salsaClient, new MapperFactory());
            _localConditionalUpdater = new LocalUpdater(_localRepository, _errorHandler);
            _salsaConditionalUpdater = new ExternalUpdater(_salsaRepository, _localRepository, _errorHandler);

            _salsaClient.Login();
        }

        public void Run()
        {
            var syncSession = SyncSession.CurrentSession();
            var pullJob = new BatchOneWaySyncJob<Supporter>(_salsaRepository, _localConditionalUpdater, 100, "Pulling supporters");
            var pushJob = new BatchOneWaySyncJob<Supporter>(_localRepository, _salsaConditionalUpdater, 100, "Push supporters");
            syncSession.AddJob(pullJob).AddJob(pushJob);

            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, syncSession.CurrentContext);
            _localRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, syncSession.CurrentContext);
            _salsaRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, syncSession.CurrentContext);

            syncSession.Start();
            PrintSyncEvents();
        }

        private void PrintSyncEvents()
        {
            int totalError = 0;
            int totalAddedToLocal = 0;
            int totalUpdatedToLocal = 0;
            int totalAddedToSalsa = 0;
            int totalUpdatedToSalsa = 0;
            var currentContext = SyncSession.CurrentSession().CurrentContext;
            var salsaDestination = _salsaRepository.GetType().Name;
            var localDestination = _localRepository.GetType().Name;
            _syncEventTracker.SyncEventsForSession(currentContext, events =>totalError = events.Count(e => e.EventType == SyncEventType.Error));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToLocal = events.Count(e => e.EventType == SyncEventType.Add && e.Destination == localDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToSalsa = events.Count(e => e.EventType == SyncEventType.Add && e.Destination == salsaDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalUpdatedToLocal = events.Count(e => e.EventType == SyncEventType.Update && e.Destination == localDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalUpdatedToSalsa = events.Count(e => e.EventType == SyncEventType.Update && e.Destination == salsaDestination));
            Logger.Info(string.Format("Total added to local:{0} Total updated to local:{1} Total added to Salsa:{2} Total updated to Salsa:{3} Total errors: {4}", totalAddedToLocal, totalUpdatedToLocal, totalAddedToSalsa, totalUpdatedToSalsa, totalError));
        }

        public void DeleteAllSupporters()
        {
            _salsaClient.DeleteAllObjects("supporter", 100, true);
        }

        public void CountSupportOnSalsa()
        {
            int count = _salsaClient.CountObjects("supporter");
            Logger.Info("total supporter on salsa:" + count);
        }

       
       
    }
}
