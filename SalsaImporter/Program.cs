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
            try
            {
                switch (args[0])
                {
                    case "push":
                        new Sync().PushNewSupportsToSalsa();
                        break;
                    case "count":
                        CountSupportOnSalsa();
                        break;
                    case "delete":
                        DeleteAllSupporters();
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
            return 0;
        }

        private static void DeleteAllSupporters()
        {
            var client = new SalsaClient();
            client.Authenticate();
            client.DeleteAllObjects("supporter", 100);
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: push|count|delete [environment]\n if no environment is specified, use dev.");
        }

        private static void SetEnvironment(string[] args)
        {
            Config.Environment = args.Length > 1 ? args[1] : Config.Dev;
            Logger.Info("Sync under environment:" + Config.Environment);
        }

        private static void CountSupportOnSalsa()
        {
            var client = new SalsaClient();
            client.Authenticate();
            int count = client.SupporterCount();
            Logger.Info("total supporter on salsa:" + count);
        }
    }
}