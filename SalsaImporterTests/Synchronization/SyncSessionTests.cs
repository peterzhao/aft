using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SalsaImporter;
using SalsaImporter.Service;
using SalsaImporter.Synchronization;
using SalsaImporterTests.Utilities;

namespace SalsaImporterTests.Synchronization
{
    [TestFixture]
    public class SyncSessionTests
    {
        private JobContext _jobContext11;
        private JobContext _jobContext12;
        private JobContext _jobContext21;
        private JobContext _jobContext22;
        private SessionContext _context1;
        private SessionContext _context2;
        
        private NotificationService _notificationService;
        private readonly Mock<ISmtpClient> _mockMailer = new Mock<ISmtpClient>();

        [SetUp]
        public void SetUp()
        {
            Config.Environment = Config.Test;
            TestUtils.ClearAllSessions();
            _notificationService = new NotificationService(_mockMailer.Object);
        }

        [Test]
        public void ShouldRunGiveJobsAndUpdateJobStatusWhenThereIsNoSessionRunBefore()
        {
            var start = DateTime.Now.AddMinutes(-1);
            var session = new SyncSession(_notificationService);
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) => { job1Called = true; jobContext.SetCurrentRecord(200); });
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);

            session.Run(SessionRunningFlag.Default);

            Assert.IsTrue(job1Called);

            Assert.IsTrue(job2Called);
            var currentContext = Db(db => db.SessionContexts.Include("JobContexts").First(s => s.Id == session.CurrentContext.Id));
            Assert.AreEqual(SessionState.Finished, currentContext.State);
            Assert.IsTrue(currentContext.FinishedTime >= start);
            Assert.IsTrue(currentContext.StartTime >= start);
            Assert.IsTrue(currentContext.JobContexts.First().StartTime >= start);
            Assert.IsTrue(currentContext.JobContexts.Last().StartTime >= start);
            Assert.IsTrue(currentContext.JobContexts.First().FinishedTime >= start);
            Assert.IsTrue(currentContext.JobContexts.Last().FinishedTime >= start);
            Assert.AreEqual(200, currentContext.JobContexts.First().CurrentRecord);
            Assert.AreEqual(SyncSession.BaseModifiedDate, session.CurrentContext.MinimumModifiedDate);
        }

        [Test]
        public void ShouldResumeAbortedSessionButSkipFinishedJobWhenResumeSession()
        {
            CreateTwoSessions(SessionState.Aborted);
            var session = new SyncSession(_notificationService);
          
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) => { job1Called = true; });
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);

            session.Run(SessionRunningFlag.Default);

            Assert.IsFalse(job1Called);
            Assert.IsTrue(job2Called);
            Assert.AreEqual(_jobContext22.StartTime, session.CurrentContext.JobContexts.Last().StartTime);
        }

        [Test]
        public void ShouldRunGiveJobsAndUpdateJobStatusWhenStartNewSession()
        {
            var start = DateTime.Now;
            CreateTwoSessions(SessionState.Finished);
            var session = new SyncSession(_notificationService);
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) =>{job1Called = true; jobContext.SetCurrentRecord(200);});
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);

            session.Run(SessionRunningFlag.Default);

            Assert.IsTrue(job1Called);
         
            Assert.IsTrue(job2Called);
            var currentContext = Db(db => db.SessionContexts.Include("JobContexts").First(s => s.Id ==session.CurrentContext.Id));
            Assert.AreEqual(SessionState.Finished, currentContext.State);
            Assert.IsTrue(currentContext.FinishedTime >= start);
            Assert.IsTrue(currentContext.StartTime >= start);
            Assert.IsTrue(currentContext.JobContexts.First().StartTime >= start);
            Assert.IsTrue(currentContext.JobContexts.Last().StartTime >= start);
            Assert.IsTrue(currentContext.JobContexts.First().FinishedTime >= start);
            Assert.IsTrue(currentContext.JobContexts.Last().FinishedTime >= start);
            Assert.AreEqual(200, currentContext.JobContexts.First().CurrentRecord);
        }

       

        [Test]
        public void ShouldAbortSyncSessionAndNotifyWhenGotError()
        {
            var start = DateTime.Now;
            CreateTwoSessions(SessionState.Finished);

            var session = new SyncSession(_notificationService); 
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) =>{job1Called = true; throw new ApplicationException("error here");});
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);
            try
            {
                session.Run(SessionRunningFlag.Default);
            }catch(Exception){}

            Assert.IsTrue(job1Called);
            Assert.IsFalse(job2Called);
            Assert.AreEqual(SessionState.Aborted, session.CurrentContext.State);
            Assert.IsNull(session.CurrentContext.FinishedTime);
            Assert.IsTrue(session.CurrentContext.StartTime >= start);
            Assert.IsTrue(session.CurrentContext.JobContexts.First().StartTime >= start);
            Assert.IsNull(session.CurrentContext.JobContexts.Last().StartTime);
            Assert.IsNull(session.CurrentContext.JobContexts.First().FinishedTime);
            _mockMailer.Verify();

        }

        [Test]
        public void ShouldGetErrorWhenGivenJobHasNoName()
        {
            CreateTwoSessions(SessionState.Aborted);
            var session = new SyncSession(_notificationService);
            var job31 = new SyncJobStub("", (jobContext) => { });
            Assert.Throws <ApplicationException> (() => session.AddJob(job31));
        }

        [Test]
        public void ShouldGetErrorWhenGivenJobNameIsNotUnique()
        {
            CreateTwoSessions(SessionState.Aborted);
            var session = new SyncSession(_notificationService);
            var job31 = new SyncJobStub("job31", (jobContext) => { });
            var job32 = new SyncJobStub("job31", (jobContext) => { });
            session.AddJob(job31);
            Assert.Throws<ApplicationException>(() => session.AddJob(job32));
        }

        [Test]
        public void ShouldSendEmail()
        {
            CreateTwoSessions(SessionState.Finished);
            var session = new SyncSession(_notificationService);
            var job = new SyncJobStub("email_test_job", (jobcontest) => { });
            session.AddJob(job);
            session.Run(SessionRunningFlag.Default);
            _mockMailer.Verify();
            
        }

        [Test]
        public void ShouldRunSyncFrom1991IfRebase()
        {
            CreateTwoSessions(SessionState.Finished);
            var session = new SyncSession(_notificationService);
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) => { job1Called = true; jobContext.SetCurrentRecord(200); });
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);

            session.Run(SessionRunningFlag.Rebase);

            Assert.IsTrue(job1Called);
            Assert.IsTrue(job2Called);
            var currentContext = Db(db => db.SessionContexts.Include("JobContexts").First(s => s.Id == session.CurrentContext.Id));
            Assert.AreEqual(SessionState.Finished, currentContext.State);
            Assert.AreEqual(SyncSession.BaseModifiedDate, currentContext.MinimumModifiedDate);
        }



        [Test]
        public void ShouldRunSyncFrom1991IfRebaseForAbortedSession()
        {
            DateTime startTime = new DateTime();
            CreateTwoSessions(SessionState.Aborted, 100, 101);
            var session = new SyncSession(_notificationService);
            JobContext job1Context = null;
            JobContext job2Context = null;
            var job31 = new SyncJobStub("job21", (jobContext) => { job1Context = jobContext; });
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Context = jobContext; });
            session.AddJob(job31).AddJob(job32);

            session.Run(SessionRunningFlag.Rebase);

            var currentContext = Db(db => db.SessionContexts.Include("JobContexts").First(s => s.Id == session.CurrentContext.Id));

            Assert.IsTrue(job1Context.StartTime > startTime);
            Assert.IsTrue(job2Context.StartTime > startTime);

            Assert.AreEqual(0, job1Context.CurrentRecord);
            Assert.AreEqual(0, job2Context.CurrentRecord);
            Assert.AreEqual(SyncSession.BaseModifiedDate, currentContext.MinimumModifiedDate);
           
        }

        [Test]
        public void ShouldReRunLastSyncSession()
        {
            CreateTwoSessions(SessionState.Finished);
            var session = new SyncSession(_notificationService);
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) => { job1Called = true; jobContext.SetCurrentRecord(200); });
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);

            session.Run(SessionRunningFlag.RedoLast);

            Assert.IsTrue(job1Called);
            Assert.IsTrue(job2Called);
            var currentContext = Db(db => db.SessionContexts.Include("JobContexts").First(s => s.Id == session.CurrentContext.Id));
            Assert.AreEqual(SessionState.Finished, currentContext.State);
            Assert.AreEqual(_context2.MinimumModifiedDate, currentContext.MinimumModifiedDate);
        }

        [Test]
        public void ShouldReRunLastSessionSyncIfLastSessionWasAborted()
        {
            DateTime startTime = new DateTime();
            CreateTwoSessions(SessionState.Aborted, 100, 101);
            var session = new SyncSession(_notificationService);
            JobContext job1Context = null;
            JobContext job2Context = null;
            var job31 = new SyncJobStub("job21", (jobContext) => { job1Context = jobContext; });
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Context = jobContext; });
            session.AddJob(job31).AddJob(job32);

            session.Run(SessionRunningFlag.RedoLast);

            var currentContext = Db(db => db.SessionContexts.Include("JobContexts").First(s => s.Id == session.CurrentContext.Id));

            Assert.IsTrue(job1Context.StartTime > startTime);
            Assert.IsTrue(job2Context.StartTime > startTime);

            Assert.AreEqual(0, job1Context.CurrentRecord);
            Assert.AreEqual(0, job2Context.CurrentRecord);
            Assert.AreEqual(_context2.MinimumModifiedDate, currentContext.MinimumModifiedDate);

        }


      

        private void CreateTwoSessions(string stateOfSecondSession, int job1CurrentRecord = 0, int job2CurrentRecord = 0)
        {
            _jobContext11 = new JobContext { JobName = "job11", StartTime = new DateTime(2012, 6, 30), FinishedTime = new DateTime(2012, 6, 30) };
            _jobContext12 = new JobContext { JobName = "job12", StartTime = new DateTime(2012, 7, 1), FinishedTime = new DateTime(2012, 7, 1) };
            _jobContext21 = new JobContext { JobName = "job21", StartTime = new DateTime(2012, 7, 2), FinishedTime = new DateTime(2012, 7, 2)};
            _jobContext22 = new JobContext { JobName = "job22", StartTime = new DateTime(2012, 7, 2), FinishedTime = null };
            _jobContext21.SetCurrentRecord(job1CurrentRecord);
            _jobContext22.SetCurrentRecord(job2CurrentRecord);
            _context1 = new SessionContext
            {
                MinimumModifiedDate = new DateTime(1991, 1, 1),
                State = SessionState.Finished,
                StartTime = new DateTime(2012, 6, 30),
                FinishedTime = new DateTime(2012, 7, 1),
                JobContexts = new List<JobContext> { _jobContext11, _jobContext12 }
            };
            _context2 = new SessionContext
            {
                MinimumModifiedDate = new DateTime(2012, 7, 1),
                State = stateOfSecondSession,
                StartTime = new DateTime(2012, 7, 2),
                JobContexts = new List<JobContext> { _jobContext21, _jobContext22 }
            };

            SaveSessionContext(_context1);
            SaveSessionContext(_context2);
        }


       

        private void SaveSessionContext(SessionContext context)
        {
            Db(db =>
            {
                db.SessionContexts.Add(context);
                db.SaveChanges();
            });
        }

        private void Db(Action<AftDbContext> action)
        {
            using (var db = new AftDbContext())
            {
                action(db);
            }
        }

        private T Db<T>(Func<AftDbContext, T> func)
        {
            using (var db = new AftDbContext())
            {
                return func(db);
            }
        }
    }
}