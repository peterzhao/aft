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
                .AddJob(new QueuePusher(_salsaRepository, _queueRepository, 100, "Import Supporters", "supporter", "SalsaToAftQueue_Supporters"))
                .AddJob(new Exporter(_queueRepository, _salsaRepository, 100, "Export Supporters", "supporter", "AftToSalsaQueue_Supporters"));
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
            _syncSession.Jobs.GroupBy( job=>job.ObjectType).ToList().ForEach(g => PrintSyncEventsFor(g.Key));
        }

        private void PrintSyncEventsFor(string objectType)
        {
            int totalErrors = 0;
            int totalAddedToLocal = 0;
            int totalAddedToSalsa = 0;
            var currentContext = _syncSession.CurrentContext;
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalErrors = events.Count(e => e.EventType == SyncEventType.Error && e.ObjectType == objectType));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToLocal = events.Count(e => e.EventType == SyncEventType.Import && e.ObjectType == objectType));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToSalsa = events.Count(e => e.EventType == SyncEventType.Export && e.ObjectType == objectType));
            Logger.Info(string.Format("{0}: Total imported from Salsa:{1} Total exported to Salsa:{2} Total errors: {3}", objectType, totalAddedToLocal, totalAddedToSalsa, totalErrors));
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
