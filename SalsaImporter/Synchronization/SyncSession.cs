using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Service;

namespace SalsaImporter.Synchronization
{
    public class SessionState
    {
        public static string New = "New";
        public static string Started = "Started";
        public static string Resumed = "Resumed";
        public static string Finished = "Finished";
        public static string Aborted = "Aborted";
    }

    public class SessionRunningFlag
    {
        public static readonly string Default = "Default";
        public static readonly string Rebase = "Rebase";
        public static readonly string RedoLast = "RedoLast";
    }

    public class SyncSession
    {
        private readonly List<ISyncJob> _jobs;
        private SessionContext _currentContext;
        private readonly AftDbContext _db;
        private NotificationService _notificationService;
        public readonly static DateTime BaseModifiedDate = new DateTime(1991, 1, 1);

        public SyncSession(NotificationService notificationService)
        {
            _notificationService = notificationService;
            _jobs = new List<ISyncJob>();
            _db = new AftDbContext();
        }

        private void InitializeContext(string flag)
        {
            var lastContext = _db.SessionContexts.Include("JobContexts").OrderByDescending(s => s.Id).FirstOrDefault();
            if (lastContext == null || flag == SessionRunningFlag.Rebase)
                StartNewSession(BaseModifiedDate);
            else if(flag == SessionRunningFlag.RedoLast)
                StartNewSession(lastContext.MinimumModifiedDate);
            else if (lastContext.State == SessionState.Finished)
                StartNewSession(lastContext.StartTime.Value);
            else
                ResumeLastSession(lastContext);

            _jobs.ForEach(job =>{if (CurrentContext.JobContexts.All(j => j.JobName != job.Name))
                                      CurrentContext.JobContexts.Add(new JobContext { JobName = job.Name });
                              });
          
            _db.SaveChanges();
        }

      


        public SessionContext CurrentContext
        {
            get { return _currentContext; }
        }

        public List<ISyncJob> Jobs
        {
            get { return _jobs; }
        }

        public SyncSession AddJob(ISyncJob job)
        {
            if (string.IsNullOrWhiteSpace(job.Name))
                throw new ApplicationException("Job name cannot be empty");
            if (_jobs.Any(j => j.Name == job.Name))
                throw new ApplicationException("Job names must be unique.  Already have job: " + job.Name);

            _jobs.Add(job);
            return this;
        }

     

        public void Run(string sessionRunningFlag)
        {
            InitializeContext(sessionRunningFlag);
            try
            {
                foreach (var job in _jobs)
                {
                    var jobContext = CurrentContext.JobContexts.First(j => j.JobName == job.Name);
                    if(jobContext.FinishedTime == null)
                        StartJob(jobContext, job);
                }
                UpdateSessionStateToFinished();
            }
            catch (Exception ex)
            {
                var message = "Encountered unexpected error. Sync aborted. Please try to resume this session later.";
                Logger.Fatal(message, ex);

                message = String.Format(message + " Exception: {0}", ex.Message);
                _notificationService.SendNotification(message);

                CurrentContext.State = SessionState.Aborted;
                _db.SaveChanges();
                throw new SyncAbortedException(message, ex);
            }
        }

        private void StartNewSession(DateTime modifiedDate)
        {
                _currentContext = new SessionContext { 
                    State = SessionState.New,
                    StartTime = DateTime.Now,
                    JobContexts = new Collection<JobContext>(),
                    MinimumModifiedDate = modifiedDate};
                Logger.Info("Start sync session...");
                _db.SessionContexts.Add(_currentContext);
        }

        private void StartJob(JobContext jobContext, ISyncJob job)
        {
            jobContext.JobContextChanged += (obj, arg) => _db.SaveChanges();
            if(jobContext.StartTime == null) jobContext.StartTime = DateTime.Now;
            Logger.Info(string.Format("Start job {0} at record {1}", job.Name, jobContext.CurrentRecord));
            _db.SaveChanges();

            job.Start(jobContext);

            jobContext.FinishedTime = DateTime.Now;
            _db.SaveChanges();
        }

        private void UpdateSessionStateToFinished()
        {
            CurrentContext.State = SessionState.Finished;
            CurrentContext.FinishedTime = DateTime.Now;
            _db.SaveChanges();

            Logger.Info("Finished sync session.");
        }

        private void ResumeLastSession(SessionContext lastContext)
        {
            _currentContext = lastContext; //resume
            CurrentContext.State = SessionState.Resumed;
            Logger.Info("Resuming sync session...");
        }
    }
}