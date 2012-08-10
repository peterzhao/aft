using System;
using System.Collections.Generic;
using System.Linq;
using NLog.Layouts;
using NLog.Targets;


namespace SalsaImporter
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            DateTime begin = DateTime.Now;

            try
            {
                if (args.Length < 1)
                {
                    ShowUsage();
                    return 1;
                }
                Config.Environment = args.Length > 1 ? args[1] : Config.Test;
                Logger.Info("Sync under environment:" + Config.Environment);
                Logger.Info("Start Salsa importer...");


                var sync = new Sync();
                switch (args[0])
                {
                    case "sync":
                        sync.Run();
                        break;
                    case "count":
                        sync.CountSupportOnSalsa();
                        break;
                    case "delete":
                        sync.DeleteAllSupporters();
                        break;
                    case "customcolumns":
                        new CreateTestingCustomColumns().CreateCustomColumns(new List<SupporterCustomColumnsRequest>()
                                                                                 {
                                                                                     new SupporterCustomColumnsRequest
                                                                                         {CustomColumnName = "String", HowManyToMake = 10, SalsaType = "varchar"},
                                                                                     new SupporterCustomColumnsRequest
                                                                                         {CustomColumnName = "Boolean", HowManyToMake = 10, SalsaType = "bool"},
                                                                                     new SupporterCustomColumnsRequest
                                                                                         {CustomColumnName = "Integer", HowManyToMake = 5, SalsaType = "int"},
                                                                                     new SupporterCustomColumnsRequest
                                                                                         {CustomColumnName = "DateTime", HowManyToMake = 1, SalsaType = "datetime"}
                                                                                 });
                        break;
                    default:
                        ShowUsage();
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.Fatal("Encounter unexpected error.", e);
                Console.WriteLine(e.Message);
                return 1;
            }
            DateTime finished = DateTime.Now;
            Logger.Info("finished:" + (finished - begin).TotalSeconds);
            return 0;
        }


        private static void ShowUsage()
        {
            Console.WriteLine("Usage: sync|count|delete|customcolumns [environment]\n if no environment is specified, use dev.");
        }
    }
}