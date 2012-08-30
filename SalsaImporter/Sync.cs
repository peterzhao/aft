using System;
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
            var mapperFactory = new MapperFactory();
            _salsaRepository = new SalsaRepository(_salsaClient, mapperFactory, _errorHandler);
            _queueRepository = new QueueRepository(mapperFactory);
            _salsaClient.Login();

            _syncSession = new SyncSession();
        }

        public void Run()
        {
            ConfigSync();
            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _salsaRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _queueRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            
            _syncSession.Start();
            PrintSyncEvents();
        }

        private void ConfigSync()
        {
            using(var db = new AftDbContext())
            {
                var syncConfigs = db.SyncConfigs.OrderBy(c => c.Order).ToList();
                syncConfigs.ForEach(c => _syncSession.AddJob(CreateSyncJob(c)));
            }
        }

        private ISyncJob CreateSyncJob(SyncConfig syncConfig)
        {
            var name = string.Format("{0} {1}",syncConfig.SyncDirection.ToUpper(), syncConfig.ObjectType.ToUpper());

            const int batchSize = 100;

            switch (syncConfig.SyncDirection)
            {
                case "export":
                    {
                        var queueName = string.Format("AftToSalsaQueue_{0}", syncConfig.ObjectType);
                        return new Exporter(_queueRepository, _salsaRepository, batchSize, name, syncConfig.ObjectType, queueName);
                    }
                case "import":
                    {
                        var queueName = string.Format("SalsaToAftQueue_{0}", syncConfig.ObjectType);
                        return new QueuePusher(_salsaRepository, _queueRepository, batchSize, name, syncConfig.ObjectType, queueName);
                    }
                default:
                    throw new ApplicationException(string.Format("Invalid SyncDirection '{0}' for this SyncConfigs record. Supported types include 'import' and 'export'.", syncConfig.SyncDirection));
            }
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
