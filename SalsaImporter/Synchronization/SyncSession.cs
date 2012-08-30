using System;
using System.Collections.Generic;
using System.Linq;
using SalsaImporter.Exceptions;

namespace SalsaImporter.Synchronization
{
    public class SessionState
    {
        public static string New = "New";
        public static string Start = "Start";
        public static string InProgress = "In Progress";
        public static string Finished = "Finished";
        public static string Aborted = "Aborted";
        public static string Canceled = "Canceled";
    }

    public class SyncSession
    {
        private readonly List<ISyncJob> _jobs;
        private SessionContext _currentContext;
        private readonly AftDbContext _db;

        public SyncSession()
        {
            _jobs = new List<ISyncJob>();
            _db = new AftDbContext();
            Initialize();
        }

        private void Initialize()
        {
            var lastContext = _db.SessionContexts.Include("JobContexts").OrderByDescending(s => s.Id).FirstOrDefault();
            if (lastContext == null)
            {
                _currentContext = new SessionContext()
                                      {MinimumModifiedDate = new DateTime(1991, 1, 1), State = SessionState.New};
                _db.SessionContexts.Add(_currentContext);
            }
            else
            {
                if (lastContext.State != SessionState.Finished)
                {
                    _currentContext = lastContext;
                    lastContext.State = SessionState.InProgress;
                }
                else
                {
                    _currentContext = new SessionContext()
                                          {MinimumModifiedDate = lastContext.StartTime.Value, State = SessionState.New};
                    _db.SessionContexts.Add(_currentContext);
                }
            }
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

            if (CurrentContext.JobContexts == null) CurrentContext.JobContexts = new List<JobContext>();
            if (CurrentContext.JobContexts.All(j => j.JobName != job.Name))
                CurrentContext.JobContexts.Add(new JobContext {JobName = job.Name});
            _db.SaveChanges();

            return this;
        }

        public void Start()
        {
            Logger.Info("Start new sync session...");

            CurrentContext.State = SessionState.Start;
            CurrentContext.StartTime = DateTime.Now;
            _db.SaveChanges();

            try
            {
                foreach (var job in _jobs)
                {
                    var jobContext = CurrentContext.JobContexts.First(j => j.JobName == job.Name);
                    jobContext.JobContextChanged += (obj, arg) => _db.SaveChanges();
                    jobContext.StartTime = DateTime.Now;
                    Logger.Info("Start job " + job.Name);
                    _db.SaveChanges();

                    job.Start(jobContext);

                    jobContext.FinishedTime = DateTime.Now;
                    _db.SaveChanges();
                }
                CurrentContext.State = SessionState.Finished;
                CurrentContext.FinishedTime = DateTime.Now;
                _db.SaveChanges();

                Logger.Info("Finished sync session.");
            }
            catch (Exception ex)
            {
                var message = "Encourtered unexpected error. Sync aborted. Please try to resume this session later.";
                Logger.Fatal(message, ex);
                CurrentContext.State = SessionState.Aborted;
                _db.SaveChanges();
                throw new SyncAbortedException(message, ex);
            }
        }
    }
}