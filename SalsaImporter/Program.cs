using System;

namespace SalsaImporter
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowUsage();
                return 1;
            }

            SetEnvironment(args);
            Logger.Info("Start Salsa importer...");
            var begin = DateTime.Now;

            try
            {
                var sync = new Sync();
                switch (args[0])
                {
                    case "push":
                        sync.PushNewSupportsToSalsa();
                        break;
                    case "count":
                        sync.CountSupportOnSalsa();
                        break;
                    case "delete":
                        sync.DeleteAllSupporters();
                        break;
                    case "customcolumns":
                        new CreateTestingCustomColumns().CreateCustomColumns();
                        break;
                    default:
                        ShowUsage();
                        break;
                }
            }
            catch(Exception e)
            {
                Logger.Fatal("Encounter unexpected error.", e);
                return 1;
            }
            var finished = DateTime.Now;
            Logger.Info("finished:" + (finished - begin).TotalSeconds);
            return 0;
        }

      
        private static void ShowUsage()
        {
            Console.WriteLine("Usage: push|count|delete|customcolumns [environment]\n if no environment is specified, use dev.");
        }

        private static void SetEnvironment(string[] args)
        {
            Config.Environment = args.Length > 1 ? args[1] : Config.Dev;
            Logger.Info("Sync under environment:" + Config.Environment);
        }

        
    }
}