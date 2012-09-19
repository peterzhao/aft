using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Utilities
{
    public class LogTrimmer
    {
        public void TrimImporterLogsOlderThan(int days)
        {
            var daysAgo = DateTime.Now.AddDays(days * -1).ToString("yyyy-MM-dd");
            using(var db = new AftDbContext())
            {
                var sql = string.Format("delete from importerlogs where time_stamp < '{0}'", daysAgo);
                Logger.Debug("Delete importerlogs generated before " + daysAgo);
                db.Database.ExecuteSqlCommand(sql);
            }
        }

        public void TrimOldNonErrorSyncEvents(int currentSessionContextId)
        {
            using (var db = new AftDbContext())
            {
                var sql = string.Format("delete from syncEvents where sessionContext_id<{0} and eventType<>'error'", currentSessionContextId);
                Logger.Debug("Delete non-error syncEvents except for those of current session:" + currentSessionContextId);
                db.Database.ExecuteSqlCommand(sql);
            }
        }
    }
}
