using System;
using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;
using SalsaImporter.Salsa;
using SalsaImporter.Service;
using SalsaImporter.Synchronization;
using SalsaImporter.Utilities;

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
        private NotificationService _notificationService;
        private LogTrimmer _logTrimmer;
        
        public Sync()
        {
            var mapperFactory = new MapperFactory();

            _errorHandler = new SyncErrorHandler(Config.ErrorToleranceThreshold);
            _salsaClient = new SalsaClient();
            _syncEventTracker = new SyncEventTracker();
            _salsaRepository = new SalsaRepository(_salsaClient, mapperFactory, _errorHandler);
            _queueRepository = new QueueRepository(mapperFactory);
            _notificationService = new NotificationService(new EmailService());
            _syncSession = new SyncSession(_notificationService);
            _logTrimmer = new LogTrimmer();
        }

        public void Start()
        {
           Run(SessionRunningFlag.Default); 
        }

        public void Redo()
        {
            Run(SessionRunningFlag.RedoLast);
        }

        public void Rebase()
        {
            Run(SessionRunningFlag.Rebase);
        }

        private void Run(string sessionRunningFlag)
        {
            SanityCheck();
            _logTrimmer.TrimImporterLogsOlderThan(2);
            _salsaClient.Login();//test if connection is good
            ConfigSync();
            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _salsaRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);
            _queueRepository.NotifySyncEvent += (sender, syncEventArgs) => _syncEventTracker.TrackEvent(syncEventArgs, _syncSession.CurrentContext);

            try
            {
                _syncSession.Run(sessionRunningFlag);
            }
            finally
            {
                NotifySyncEvents();
            }
            _logTrimmer.TrimOldNonErrorSyncEvents(_syncSession.CurrentContext.Id);
        }

     

        public  void SanityCheck()
        {
            Logger.Info("Verifying SyncConfig / FieldMappings...");
            var sanityChecker = new SanityChecker(_salsaClient);
            var errors = sanityChecker.VerifyQueues();
            errors.AddRange(sanityChecker.VerifySalsaFields());
            if(errors.Count > 0)
                throw new ApplicationException("SyncConfig / FieldMapping verification failed. " + string.Join(", ", errors));
            Logger.Info("SyncConfig / FieldMappings match queues and salsalabs.com");
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

            var batchSize = Config.BatchSize;

            if (syncConfig.SyncDirection.EqualsIgnoreCase(SyncDirection.Export))
                return new Exporter(_queueRepository, _salsaRepository, _errorHandler, batchSize, name, syncConfig.ObjectType, syncConfig.QueueName);
            else if (syncConfig.SyncDirection.EqualsIgnoreCase(SyncDirection.Import))
                return new Importer(_salsaRepository, _queueRepository, _errorHandler, batchSize, name, syncConfig.ObjectType, syncConfig.QueueName);
            else
                throw new ApplicationException(string.Format("Invalid SyncDirection '{0}' for this SyncConfigs record. Supported types include 'import' and 'export'.", syncConfig.SyncDirection));
        }

        private void NotifySyncEvents()
        {
            var messages = _syncSession.Jobs.GroupBy( job=>job.ObjectType).Select(g => PrintSyncEventsFor(g.Key)).ToList();
            _notificationService.SendNotification(string.Join("\n", messages));
        }

        private string PrintSyncEventsFor(string objectType)
        {
            int totalErrors = 0;
            int totalAddedToLocal = 0;
            int totalAddedToSalsa = 0;
            var currentContext = _syncSession.CurrentContext;
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalErrors = events.Count(e => e.EventType == SyncEventType.Error && e.ObjectType == objectType));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToLocal = events.Count(e => e.EventType == SyncEventType.Import && e.ObjectType == objectType));
            _syncEventTracker.SyncEventsForSession(currentContext, events => totalAddedToSalsa = events.Count(e => e.EventType == SyncEventType.Export && e.ObjectType == objectType));
            var message = string.Format("{0}: total imported from Salsa:{1} total exported to Salsa:{2} total errors: {3}", objectType.ToUpper(), totalAddedToLocal, totalAddedToSalsa, totalErrors);
            Logger.Info(message);
            return message;
        }

        public void DeleteAllSupporters()
        {
            if(Config.Environment == Config.Production)
                throw new InvalidOperationException("Cannot delete all supporters in production");
            _salsaClient.DeleteAllObjects("supporter", 100, true);
        }

        public void CountSupportOnSalsa()
        {
            int count = _salsaClient.CountObjects("supporter");
            Logger.Info("total supporter on salsa:" + count);
        }

       
       
    }
}
