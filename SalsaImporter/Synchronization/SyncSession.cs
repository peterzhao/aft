using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SalsaImporter.Exceptions;
using SalsaImporter.Service;
using SalsaImporter.Utilities;

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
        private SessionContext _currentSessionContext;
        private JobContext _currentJobContext;
        private readonly AftDbContext _db;
        private NotificationService _notificationService;
        public readonly static DateTime BaseModifiedDate = new DateTime(1991, 1, 1);

        public SyncSession(NotificationService notificationService)
        {
            _notificationService = notificationService;
            _jobs = new List<ISyncJob>();
            _db = new AftDbContext();
        }

        public SessionContext CurrentSessionContext
        {
            get { return _currentSessionContext; }
        }

        public JobContext CurrentJobContext
        {
            get { return _currentJobContext; }
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
                    var jobContext = CurrentSessionContext.JobContexts.First(j => j.JobName == job.Name);
                    _currentJobContext = jobContext;
                    if (jobContext.FinishedTime == null)
                        StartJob(jobContext, job);
                }
                UpdateSessionStateToFinished();
            }
            catch (Exception ex)
            {
                var message = String.Format("Encountered error. Sync aborted. Please try to resume this session later. Exception: {0}", ex.Message);
                _notificationService.SendNotification(message);

                CurrentSessionContext.State = SessionState.Aborted;
                SaveContextToDb();
                throw new SyncAbortedException(message, ex);
            }
        }

        private void InitializeContext(string flag)
        {
            var lastContext = _db.SessionContexts.Include("JobContexts").OrderByDescending(s => s.Id).FirstOrDefault();
            if (lastContext == null || flag == SessionRunningFlag.Rebase)
                StartNewSession(BaseModifiedDate);
            else if (flag == SessionRunningFlag.RedoLast)
                StartNewSession(lastContext.MinimumModifiedDate);
            else if (lastContext.State.EqualsIgnoreCase(SessionState.Finished))
                StartNewSession(lastContext.StartTime.Value);
            else
                ResumeLastSession(lastContext);

            _jobs.ForEach(job =>
            {
                if (CurrentSessionContext.JobContexts.All(j => j.JobName != job.Name))
                    CurrentSessionContext.JobContexts.Add(new JobContext { JobName = job.Name });
            });

            SaveContextToDb();
        }

        private void StartNewSession(DateTime modifiedDate)
        {
                _currentSessionContext = new SessionContext { 
                    State = SessionState.New,
                    StartTime = DateTime.Now,
                    JobContexts = new List<JobContext>(),
                    MinimumModifiedDate = modifiedDate};
                Logger.Info("Start sync session...");
                _db.SessionContexts.Add(_currentSessionContext);
        }

        private void StartJob(JobContext jobContext, ISyncJob job)
        {
            jobContext.JobContextChanged += (obj, arg) => SaveContextToDb();
            if(jobContext.StartTime == null) jobContext.StartTime = DateTime.Now;
            Logger.Info(string.Format("Start job {0} at record {1}", job.Name, jobContext.CurrentRecord));
            SaveContextToDb();

            job.Start(jobContext);
            Logger.Trace("Job finished: " + job.Name);
            jobContext.FinishedTime = DateTime.Now;
            SaveContextToDb();
        }

        private void UpdateSessionStateToFinished()
        {
            Logger.Trace("Updating session state to finished...");
            CurrentSessionContext.State = SessionState.Finished;
            CurrentSessionContext.FinishedTime = DateTime.Now;
            SaveContextToDb();

            Logger.Info("Finished sync session.");
        }

        private void ResumeLastSession(SessionContext lastContext)
        {
            _currentSessionContext = lastContext; //resume
            CurrentSessionContext.State = SessionState.Resumed;
            Logger.Info("Resuming sync session...");
        }

        private void SaveContextToDb()
        {
            Logger.Trace("Saving context to db...");
            _db.SaveChanges();
            Logger.Trace("Context saved.");
        }
    }
}