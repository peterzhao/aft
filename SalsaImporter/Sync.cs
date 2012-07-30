using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;
using SalsaImporter.Exceptions;

namespace SalsaImporter
{
    public class Sync
    {
        private readonly SalsaClient _salsa;
        private ImporterErrorHandler _errorHandler;

        public Sync()
        {
            _salsa = new SalsaClient();
            _errorHandler = new ImporterErrorHandler(500);
            _salsa.Login();
        }

        public void PushNewSupportsToSalsa()
        {
        
            var mapper = new SupporterMapper();
            var batchSize = 100;
            int? totalLimit = null;
            EachBatchOfSupportersFromAft(batchSize, totalLimit, supporters =>
            {
                var nameValuesList = supporters.Select(mapper.ToNameValues).ToList();
                _salsa.CreateSupporters(nameValuesList, _errorHandler.CanContinueToCreate);
                
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

        public void EnsureTestingCustomColumnExist()
        {
            _salsa.DeleteAllObjects("custom_column", 100);
            for (int i=0;i < 10;i++)
            {
                var customColumn = new NameValueCollection
                                          {
                                              {"name", String.Format("CustomString{0}", i)},
                                              {"label", String.Format("Custom String {0}", i)}, 
                                              {"type", "varchar"}                                           };
                _salsa.CreateSupporterCustomColumn(customColumn);
            }
            for (int i = 0; i < 10; i++)
            {
                var customColumn = new NameValueCollection
                                          {
                                              {"name", String.Format("CustomBoolean{0}", i)},
                                              {"label", String.Format("Custom Boolean {0}", i)}, 
                                              {"type", "bool"}
                                          };
                _salsa.CreateSupporterCustomColumn(customColumn);
            }
            for (int i = 0; i < 5; i++)
            {
                var customColumn = new NameValueCollection
                                          {
                                              {"name", String.Format("CustomInteger{0}", i)},
                                              {"label", String.Format("Custom Integer {0}", i)}, 
                                              {"type", "int"}
                                          };
                _salsa.CreateSupporterCustomColumn(customColumn);
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
                    Logger.Info(String.Format("Pulling supporter from aft... batch:{0} start: {1} Get: {2}", batchCount,
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
