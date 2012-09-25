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
        private readonly ISyncErrorTracker _syncErrorTracker;
        private readonly ISalsaRepository _salsaRepository;
        private readonly QueueRepository _queueRepository;
        private NotificationService _notificationService;
        private LogTrimmer _logTrimmer;
        
        public Sync()
        {
            var mapperFactory = new MapperFactory();

            _errorHandler = new SyncErrorHandler(Config.ErrorToleranceThreshold);
            _salsaClient = new SalsaClient();
            _syncErrorTracker = new SyncErrorTracker();
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
            HandleErrors();

            try
            {
                _syncSession.Run(sessionRunningFlag);
            }
            finally
            {
                NotifySyncEvents();
            }
        }

        private void HandleErrors()
        {
            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) =>
                                             _syncErrorTracker.TrackError(syncEventArgs, _syncSession.CurrentSessionContext);
            _errorHandler.NotifySyncEvent += (sender, syncEventArgs) => { if (_syncSession.CurrentJobContext != null) _syncSession.CurrentJobContext.CountError(); };
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
            var details = string.Join(Environment.NewLine, _syncSession.CurrentSessionContext.JobContexts.Select(c => string.Format("{0}: {1} success; {2} error", c.JobName, c.SuccessCount ?? 0, c.ErrorCount ?? 0)).ToList());
            Logger.Info(string.Format("Synchronization in {0}:{1}{2}", Config.Environment, Environment.NewLine, details));
            _notificationService.SendNotification( details);
        }

     

        public void DeleteAllSupporters()
        {
            if (Config.Environment == Config.Production || Config.SalsaApiUri == "https://hq-afl.salsalabs.com/")
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
