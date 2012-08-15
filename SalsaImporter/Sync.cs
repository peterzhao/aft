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
        private readonly SyncSession _syncSession;
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

            _syncSession = new SyncSession();
            _syncSession
                .AddJob(new BatchOneWaySyncJob<Supporter>(_salsaRepository, _localConditionalUpdater, 100, "Pulling supporters"))
                .AddJob(new BatchOneWaySyncJob<Supporter>(_localRepository, _salsaConditionalUpdater, 100, "Push supporters"));
        }

        public void Run()
        {
            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _localRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _salsaRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);

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
            var localDestination = _localRepository.GetType().Name;
            _syncEventTracker.SyncEventsForSession(currentContext, events =>totalErrors = events.Count(e => e.EventType == SyncEventType.Error));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToLocal = events.Count(e => e.EventType == SyncEventType.Add && e.Destination == localDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToSalsa = events.Count(e => e.EventType == SyncEventType.Add && e.Destination == salsaDestination));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalUpdatedInLocal = events.Count(e => e.EventType == SyncEventType.Update && e.Destination == localDestination));
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
