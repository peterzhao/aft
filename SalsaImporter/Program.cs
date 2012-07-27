using System;

namespace SalsaImporter
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                ShowUsage();
                return;
            }

            SetEnvironment(args);
            switch (args[0])
            {
                case "push":
                    new Sync().PushNewSupportsToSalsa();
                    break;
                case "count":
                    CountSupportOnSalsa();
                    break;
                default:
                    ShowUsage();
                    break;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: push|count [environment]\n if no environment is specified, use production.");
        }

        private static void SetEnvironment(string[] args)
        {
            Config.Environment = args.Length > 1 ? args[1] : Config.Production;
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