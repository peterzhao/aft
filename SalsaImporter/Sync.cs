using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using SalsaImporter.Aft;

namespace SalsaImporter
{
    public class Sync
    {
        private readonly SalsaClient _salsa;

        public Sync()
        {
            _salsa = new SalsaClient();
            _salsa.Authenticate();
        }

        public void PushNewSupportsToSalsa()
        {
            Logger.Info("Start performance test...");
        
            var begin = DateTime.Now;
            var mapper = new SupporterMapper();
            var batchSize = 500;
            int? totalLimit = null; //100000;
            EachBatchOfSupportersFromAft(batchSize, totalLimit, supporters =>
            {
                var nameValuesList = supporters.Select(mapper.ToNameValues).ToList();
                _salsa.Authenticate();
                _salsa.CreateSupporters(nameValuesList);
                nameValuesList.ForEach(nameValues =>
                {
                    var supporter = supporters.Find(s => s.Id == int.Parse(nameValues["uid"]));
                    supporter.supporter_KEY = int.Parse(nameValues["supporter_KEY"]);
                });
            });

            var finished = DateTime.Now;
            Logger.Info("finished:" + (finished - begin).TotalSeconds);
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
                                              {"type", "varchar"}, 
                                              {"data_column", String.Format("VARCHAR{0}", i)}
                                          };
                _salsa.CreateSupporterCustomColumn(customColumn);
            }
            for (int i = 0; i < 10; i++)
            {
                var customColumn = new NameValueCollection
                                          {
                                              {"name", String.Format("CustomBoolean{0}", i)},
                                              {"label", String.Format("Custom Boolean {0}", i)}, 
                                              {"type", "bool"}, 
                                              {"data_column", String.Format("BOOL{0}", i)}
                                          };
                _salsa.CreateSupporterCustomColumn(customColumn);
            }
            for (int i = 0; i < 5; i++)
            {
                var customColumn = new NameValueCollection
                                          {
                                              {"name", String.Format("CustomInteger{0}", i)},
                                              {"label", String.Format("Custom Integer {0}", i)}, 
                                              {"type", "int"}, 
                                              {"data_column", String.Format("BOOL{0}", i)}
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
