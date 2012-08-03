using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Mappers;
using SalsaImporter.Repositories;

namespace SalsaImporter.Synchronization
{
    public class PullSalsaToLocal
    {
        public void run()
        {
            var syncErrorHandler = new SyncErrorHandler(10, 10, 10);

            var localRepository = new LocalRepository();
            var salsaRepository = new SalsaRepository(new SalsaClient(syncErrorHandler), new MapperFactory());

            var syncLog = new SyncState(DateTime.Now);

            var localConditionalUpdater = new ConditionalUpdater(localRepository, syncErrorHandler);

            var batchProcess = new BatchOneWaySynchronize(salsaRepository, localConditionalUpdater, syncLog, 100);

            batchProcess.Synchronize<Supporter>();
        }
    }
}
