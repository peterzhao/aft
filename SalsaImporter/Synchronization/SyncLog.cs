using System;
using SalsaImporter.Aft;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public class SyncLog: ISyncLog
    {
        private readonly SyncRun _currentSyncRun;

        public SyncLog(DateTime now)
        {
            using (var db = new AftDbContext())
            {
                var incompleteRuns = db.SyncRuns.Where(s => !s.Complete);
                if (incompleteRuns.Any())
                {
                    _currentSyncRun = incompleteRuns.First();
                }
                else
                {
                    var completedRuns = db.SyncRuns.Where(s => s.Complete);
                    _currentSyncRun = new SyncRun
                                      {
                                          StartTime = now,
                                          CurrentRecord = 0,
                                          LastUpdatedMinimum =
                                              completedRuns.Any()
                                                  ? completedRuns.Max(s => s.StartTime)
                                                  : DateTime.MinValue
                                      };

                    db.SyncRuns.Add(_currentSyncRun);
                    db.SaveChanges();
                }
            }
        }

        public DateTime LastPullDateTime
        {
            get {return _currentSyncRun.LastUpdatedMinimum;} 
        }
        public int LastPulledKey
        {
            get { return _currentSyncRun.CurrentRecord; }
            set
            {
                using (var db = new AftDbContext())
                {
                    db.SyncRuns.Attach(_currentSyncRun);
                    _currentSyncRun.CurrentRecord = value;
                    db.SaveChanges();
                }
            }
        }

        public void PullingCompleted()
        {
            using (var db = new AftDbContext())
            {
                db.SyncRuns.Attach(_currentSyncRun);
                _currentSyncRun.Complete = true;
                db.SaveChanges();
            }
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