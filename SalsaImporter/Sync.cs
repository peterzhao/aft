using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Exceptions;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    public class Sync
    {
        private readonly SalsaClient _salsa;
        private readonly SyncErrorHandler _errorHandler;
        private readonly SupporterMapper _mapper;
        

        public Sync()
        {
            _errorHandler = new SyncErrorHandler(500, 500);
            _salsa = new SalsaClient(_errorHandler);
            _mapper = new SupporterMapper();
            _salsa.Login();
        }

        public void PushToSalsa()
        {
        
            var batchSize = 100;
            int? totalLimit = null;
            EachBatchOfSupportersFromAft(batchSize, totalLimit, supporters =>
            {
                var nameValuesList = supporters.Select(_mapper.ToNameValues).ToList();
                _salsa.CreateSupporters(nameValuesList);
                
                nameValuesList.ForEach(nameValues =>
                {
                    string supporterKey = nameValues["supporter_KEY"];
                    if (!string.IsNullOrEmpty(supporterKey))
                    {
                    var supporter = supporters.Find(s => s.Id == int.Parse(nameValues["uid"]));
                        supporter.supporter_KEY = int.Parse(supporterKey);
                    }
                });
            });
            PrintFailedRecords();
        }

        public void DeleteAllSupporters()
        {
            _salsa.DeleteAllObjects("supporter", 100, true);
        }

        public void CountSupportOnSalsa()
        {
            int count = _salsa.SupporterCount();
            Logger.Info("total supporter on salsa:" + count);
        }

        public void PullFromSalsa()
        {
            int batchSize = 100;
            Logger.Info("start pull changes from salsa....");
            _salsa.EachBatchOfObjects("supporter", batchSize, nameValueCollections =>
                                            {
                                                using(var db = new AftDbContext())
                                                {
                                                    foreach (var nameValues in nameValueCollections)
                                                    {
                                                        var supporter = _mapper.ToSupporter(nameValues);
                                                        if (supporter.uid != null)
                                                        {
                                                            var localSupporter = db.Supporters.Find(int.Parse(supporter.uid));
                                                            localSupporter.supporter_KEY = supporter.supporter_KEY;
                                                            Logger.Trace("one supporter updated. supporter_key:" + supporter.supporter_KEY);
                                                        }
                                                        else
                                                        {
                                                            db.Supporters.Add(supporter);
                                                            Logger.Trace("added a supporter from salsa. supporter_key:" + supporter.supporter_KEY);
                                                        }

                                                    }
                                                    db.SaveChanges();
                                                }
                                                Logger.Debug(string.Format("Finished {0} supporters.", batchSize));
                                            }, null);

            Logger.Info("Pulling from Salsa finished.");
        }


        private void PrintFailedRecords()
        {
            var failedCreatedSupporterKeys = _errorHandler.FailedRecordsToCreate.Keys.ToList();
            if (failedCreatedSupporterKeys.Count > 0)
            {
                var message = "";
                failedCreatedSupporterKeys.ForEach(k => message += k + " ");
                Logger.Error(String.Format("There are {0} supporters failed to push to Salsa. {1}",
                                           failedCreatedSupporterKeys.Count, message));
            }
        }

        private void EachBatchOfSupportersFromAft(int batchSize, int? totalLimit, Action<List<Supporter>> batchHandler)
        {
            int start = 0;
            int total = 0;
            int batchCount = 0;
            while (true)
            {
                using (var db = new AftDbContext())
                {
                    var supporters =
                        db.Supporters.OrderBy(s => s.Id).Where(s => s.Id > start && s.supporter_KEY == null).Take(
                            batchSize).ToList();
                    if (supporters.Count == 0) return;
                    Logger.Debug(String.Format("Pulling supporter from aft... batch:{0} start: {1} Get: {2}", batchCount,
                                              supporters.First().Id, supporters.Count));
                    start = supporters.Last().Id;
                    batchHandler(supporters);
                    db.SaveChanges();
                    if (supporters.Count < batchSize) return;
                    total += supporters.Count;
                    if (totalLimit != null && total >= totalLimit.Value) return;
                    batchCount += 1;
                }
            }
        }
    }
}
