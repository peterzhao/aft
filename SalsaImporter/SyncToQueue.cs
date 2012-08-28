using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    public class SyncToQueue
    {
        private readonly SyncSession _syncSession;
        private readonly ISyncErrorHandler _errorHandler;
        private readonly ISalsaClient _salsaClient;
        private readonly ISyncEventTracker _syncEventTracker;
        private readonly ISalsaRepository _salsaRepository;
        private readonly QueueRepository _queueRepository;

        public SyncToQueue()
        {
            _errorHandler = new SyncErrorHandler(10);
            _salsaClient = new SalsaClient();
            _syncEventTracker = new SyncEventTracker();
            _salsaRepository = new SalsaRepository(_salsaClient, new MapperFactory(), _errorHandler);
            _queueRepository = new QueueRepository(new AftDbContext());
            _salsaClient.Login();

            _syncSession = new SyncSession();

            _syncSession
                .AddJob(new QueuePusher(_salsaRepository, _queueRepository, 100, "SupportersFromSalsa", "Supporter"));
           }

        public void Run()
        {
            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _salsaRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _queueRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            
            _syncSession.Start();
            PrintSyncEvents();
        }

        private void PrintSyncEvents()
        {
            int totalErrors = 0;
            int totalAddedToLocal = 0;
            int totalUpdatedInLocal = 0;
            int totalAddedToSalsa = 0;
            int totalUpdatedinSalsa = 0;
            var currentContext = _syncSession.CurrentContext;
            var salsaDestination = _salsaRepository.GetType().Name;
            var queueDestination = _queueRepository.GetType().Name;
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalErrors = events.Count(e => e.EventType == SyncEventType.Error));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToLocal = events.Count(e => e.EventType == SyncEventType.Add && e.Destination == queueDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToSalsa = events.Count(e => e.EventType == SyncEventType.Add && e.Destination == salsaDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalUpdatedInLocal = events.Count(e => e.EventType == SyncEventType.Update && e.Destination == queueDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalUpdatedinSalsa = events.Count(e => e.EventType == SyncEventType.Update && e.Destination == salsaDestination));
            Logger.Info(string.Format("Total added to local:{0} Total updated in local:{1} Total added to Salsa:{2} Total updated in Salsa:{3} Total errors: {4}", totalAddedToLocal, totalUpdatedInLocal, totalAddedToSalsa, totalUpdatedinSalsa, totalErrors));
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
