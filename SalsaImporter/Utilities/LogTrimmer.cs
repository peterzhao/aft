using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Synchronization;

namespace SalsaImporter.Utilities
{
    public class LogTrimmer
    {
        public void TrimLogsOlderThan(int days)
        {
            var daysAgo = DateTime.Now.AddDays(days * -1).ToString("yyyy-MM-dd");
            using(var db = new AftDbContext())
            {
                var sql = string.Format("delete from importerlogs where time_stamp < '{0}'", daysAgo);
                Logger.Debug("Delete logs generated before " + daysAgo);
                db.Database.ExecuteSqlCommand(sql);
            }
        }
    }
}
