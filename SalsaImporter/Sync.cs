using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    public class Sync
    {
        private readonly SyncSession _syncSession;
        private readonly ISyncErrorHandler _errorHandler;
        private readonly ISalsaClient _salsaClient;
        private readonly ISyncEventTracker _syncEventTracker;
        private readonly ISalsaRepository _salsaRepository;
        private readonly QueueRepository _queueRepository;

        public Sync()
        {
            _errorHandler = new SyncErrorHandler(10);
            _salsaClient = new SalsaClient();
            _syncEventTracker = new SyncEventTracker();
            _salsaRepository = new SalsaRepository(_salsaClient, new MapperFactory(), _errorHandler);
            _queueRepository = new QueueRepository();
            _salsaClient.Login();

            _syncSession = new SyncSession();

            _syncSession
                .AddJob(new QueuePusher(_salsaRepository, _queueRepository, 100, "SupportersFromSalsa", "supporter", "SalsaToAftQueue_Supporters"));
           }

        public void Run()
        {
//            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
//            _salsaRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
//            _queueRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            
            _syncSession.Start();
            PrintSyncEvents();
        }

        private void PrintSyncEvents()
        {
            int totalErrors = 0;
            int totalAddedToLocal = 0;
            int totalAddedToSalsa = 0;
            var currentContext = _syncSession.CurrentContext;
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalErrors = events.Count(e => e.EventType == SyncEventType.Error));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToLocal = events.Count(e => e.EventType == SyncEventType.Import));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToSalsa = events.Count(e => e.EventType == SyncEventType.Export));
            Logger.Info(string.Format("Total imported from Salsa:{0} Total exported to Salsa:{1} Total errors: {2}", totalAddedToLocal, totalAddedToSalsa, totalErrors));
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
