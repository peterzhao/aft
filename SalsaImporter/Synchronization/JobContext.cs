using System;
using SalsaImporter.Aft;
using System.Linq;

namespace SalsaImporter.Synchronization
{
    public class JobContext: IJobContext
    {
        private readonly SyncRun _currentSyncRun;

        public JobContext(DateTime now)
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
                                          MinimumModificationDate =
                                              completedRuns.Any()
                                                  ? completedRuns.Max(s => s.StartTime)
                                                  : DateTime.MinValue
                                      };

                    db.SyncRuns.Add(_currentSyncRun);
                    db.SaveChanges();
                }
            }
        }

        public DateTime MinimumModificationDate
        {
            get {return _currentSyncRun.MinimumModificationDate;} 
        }

        public int CurrentRecord
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

        public void MarkComplete()
        {
            using (var db = new AftDbContext())
            {
                db.SyncRuns.Attach(_currentSyncRun);
                _currentSyncRun.Complete = true;
                db.SaveChanges();
            }
        }

    }
}