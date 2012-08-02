using System;
using SalsaImporter.Aft;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public class SyncLog: ISyncLog
    {
        private readonly AftDbContext _db;
        private readonly SyncRun _currentSyncRun;

        public SyncLog(AftDbContext db, DateTime startTime)
        {
            _db = db;
            _currentSyncRun = _db.SyncRuns.Any(s => !s.Complete) 
                ? _db.SyncRuns.First(s => !s.Complete) 
                : NewSyncRun(startTime);
        }

        public DateTime LastPullDateTime
        {
            get { return _currentSyncRun.LastUpdatedMinimum; } 
            
        }
        public int LastPulledKey
        {
            get { return _currentSyncRun.CurrentRecord; }
            set { _currentSyncRun.CurrentRecord = value; }
        }

        public void PullingCompleted()
        {
            _currentSyncRun.Complete = true;
            _db.SaveChanges();
        }

        private SyncRun NewSyncRun(DateTime startTime)
        {
            var completedSyncRuns = _db.SyncRuns.Where(s => s.Complete);
            
            SyncRun syncRun = new SyncRun
                                  {
                                      StartTime = startTime,
                                      CurrentRecord = 0,
                                      LastUpdatedMinimum =
                                        completedSyncRuns.Any()
                                              ? completedSyncRuns.Max(s => s.StartTime)
                                              : DateTime.MinValue
                                  };

            _db.SyncRuns.Add(syncRun);

            return syncRun;
        }

        public DateTime LastPushDateTime 
        { 
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            } 
        }

        public int LastPushedKey
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}