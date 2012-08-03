using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using SalsaImporter.Aft;
using SalsaImporter.Synchronization;
using SalsaImporterTests.utilities;

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
        [SetUp]
        public void SetUp()
        {
            TestUtils.ClearAllSessions();
        }

        [Test]
        public void ShouldStartANewSessionIfThereIsNoPreviousSessionBefore()
        {
            var session = new SyncSession();
            Assert.AreEqual(SessionState.New, session.CurrentContext.State);
            Assert.AreEqual(new DateTime(1991, 1, 1), session.CurrentContext.MinimumModifiedDate);
            //Assert.IsTrue(session.CurrentContext.StartTime >= start);
            Assert.IsNull(session.CurrentContext.JobContexts);
            Assert.IsNotNull(GetSessionContext(session.CurrentContext.Id));
        }

       


        [Test]
        public void ShouldResumeTheLastUnFinishedSessionAsCurrentContext()
        {
            CreateTowSessions(SessionState.Aborted);

            var session = new SyncSession();
            Assert.AreEqual(SessionState.InProgress, session.CurrentContext.State);
            Assert.AreEqual(new DateTime(2012, 7, 1), session.CurrentContext.MinimumModifiedDate);
            Assert.IsNotNull(session.CurrentContext.JobContexts);
            Assert.AreEqual("job21", session.CurrentContext.JobContexts.First().JobName);
            Assert.AreEqual("job22", session.CurrentContext.JobContexts.Last().JobName);
        }

        [Test]
        public void ShouldCreateANewContextWhenTheLastSessionWasFinished()
        {
            CreateTowSessions(SessionState.Finished);
            var start = DateTime.Now;
            var session = new SyncSession();
            Assert.AreEqual(SessionState.New, session.CurrentContext.State);
            Assert.AreEqual(_context2.StartTime, session.CurrentContext.MinimumModifiedDate);
            //Assert.IsTrue(session.CurrentContext.StartTime >= start);
            Assert.IsNull(session.CurrentContext.JobContexts);
        }

        [Test]
        public void ShouldCreateNewJobContextForNewJobIfThereIsNoExistingOne()
        {
            CreateTowSessions(SessionState.Finished);
            var session = new SyncSession();
            var job31 = new SyncJobStub("job21", (jobContext) => { });
            var job32 = new SyncJobStub("job22", (jobContext) => { });
            session.AddJob(job31).AddJob(job32);
            Assert.AreEqual(job31.Name, session.CurrentContext.JobContexts.First().JobName);
            Assert.AreEqual(job32.Name, session.CurrentContext.JobContexts.Last().JobName);
            Assert.AreNotEqual(_jobContext21.Id, session.CurrentContext.JobContexts.First().Id);
            Assert.AreNotEqual(_jobContext22.Id, session.CurrentContext.JobContexts.Last().Id);

        }

        [Test]
        public void ShouldUseExistingJobContextForJobIfThereIsExistingOne()
        {
            CreateTowSessions(SessionState.Aborted);
            var session = new SyncSession();
            var job31 = new SyncJobStub("job21", (jobContext) => { });
            var job32 = new SyncJobStub("job22", (jobContext) => { });
            session.AddJob(job31).AddJob(job32);
            Assert.AreEqual(job31.Name, session.CurrentContext.JobContexts.First().JobName);
            Assert.AreEqual(job32.Name, session.CurrentContext.JobContexts.Last().JobName);
            Assert.AreEqual(_jobContext21.Id, session.CurrentContext.JobContexts.First().Id);
            Assert.AreEqual(_jobContext22.Id, session.CurrentContext.JobContexts.Last().Id);
        }

        [Test]
        public void ShouldRunGiveJobsAndUpdateJobStatus()
        {
            var start = DateTime.Now;
            CreateTowSessions(SessionState.Finished);
            var session = new SyncSession();
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) =>{job1Called = true;});
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);

            try
            {
                session.Start();
            }catch(Exception){}

            Assert.IsTrue(job1Called);
         
            Assert.IsTrue(job2Called);
            Assert.AreEqual(SessionState.Finished, session.CurrentContext.State);
            Assert.IsTrue(session.CurrentContext.FinishedTime >= start);
            Assert.IsTrue(session.CurrentContext.StartTime >= start);
            Assert.IsTrue(session.CurrentContext.JobContexts.First().StartTime >= start);
            Assert.IsTrue(session.CurrentContext.JobContexts.Last().StartTime >= start);
            Assert.IsTrue(session.CurrentContext.JobContexts.First().FinishedTime >= start);
            Assert.IsTrue(session.CurrentContext.JobContexts.Last().FinishedTime >= start);

        }

        [Test]
        public void ShouldAbortSyncSessionWhenGotError()
        {
            var start = DateTime.Now;
            CreateTowSessions(SessionState.Finished);
            var session = new SyncSession();
            var job1Called = false;
            var job2Called = false;
            var job31 = new SyncJobStub("job21", (jobContext) =>{job1Called = true; throw new ApplicationException("error here");});
            var job32 = new SyncJobStub("job22", (jobContext) => { job2Called = true; });
            session.AddJob(job31).AddJob(job32);
            try
            {
                session.Start();
            }catch(Exception){}

            Assert.IsTrue(job1Called);
            Assert.IsFalse(job2Called);
            Assert.AreEqual(SessionState.Aborted, session.CurrentContext.State);
            Assert.IsNull(session.CurrentContext.FinishedTime);
            Assert.IsTrue(session.CurrentContext.StartTime >= start);
            Assert.IsTrue(session.CurrentContext.JobContexts.First().StartTime >= start);
            Assert.IsNull(session.CurrentContext.JobContexts.Last().StartTime);
            Assert.IsNull(session.CurrentContext.JobContexts.First().FinishedTime);

        }


        private void CreateTowSessions(string stateOfSecondSession)
        {
            _jobContext11 = new JobContext { JobName = "job11", StartTime = new DateTime(2012, 6, 30), FinishedTime = new DateTime(2012, 6, 30) };
            _jobContext12 = new JobContext { JobName = "job12", StartTime = new DateTime(2012, 7, 1), FinishedTime = new DateTime(2012, 7, 1) };
            _jobContext21 = new JobContext { JobName = "job21", StartTime = new DateTime(2012, 7, 2), FinishedTime = new DateTime(2012, 7, 2) };
            _jobContext22 = new JobContext { JobName = "job22", StartTime = new DateTime(2012, 7, 2), FinishedTime = null };
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


        private SessionContext GetSessionContext(int id)
        {
            return Db(db => db.SessionContexts.FirstOrDefault(s => s.Id == id));
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