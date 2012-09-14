using System;
using System.Threading;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            using (var mutex = new Mutex(false, "SalsaImporter"))
            {
                if (!mutex.WaitOne(0, false))
                {
                    Logger.Error("There is another instance of SalsaImporter is already running! Cannot run multiple instances!");
                    return 1;
                }

                DateTime begin = DateTime.Now;

                try
                {
                    if (args.Length < 1)
                    {
                        ShowUsage();
                        return 1;
                    }
                    Config.Environment = args.Length > 1 ? args[1] : Config.Test;
                    Logger.Info(string.Format("Using environment: {0} ({1}{2} {3})",
                                              Config.Environment,
                                              Config.SalsaWritable ? "" : "READ ONLY ",
                                              Config.SalsaApiUri,
                                              Config.SalsaUserName));
                    
                    var sync = new Sync();
                    switch (args[0])
                    {
                        case "sync":
                            sync.Start();
                            break;
                        case "redo":
                            sync.Redo();
                            break;
                        case "rebase":
                            sync.Rebase();
                            break;
                        case "count":
                            sync.CountSupportOnSalsa();
                            break;
                        case "verify":
                            sync.SanityCheck();
                            break;
                        case "delete":
                            sync.DeleteAllSupporters();
                            break;
                        default:
                            ShowUsage();
                            break;
                    }
                }
                catch (Exception e)
                {
                    Logger.Fatal("Encounter the error:", e);
                    Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
                    return 1;
                }
                DateTime finished = DateTime.Now;
                Logger.Info("finished:" + (finished - begin).TotalSeconds);
                return 0;
            }
        }

        private static void ShowUsage()
        {
            Console.WriteLine("Usage: SalsaImporter sync|redo|rebase|count|delete [environment]");
            Console.WriteLine("");
            Console.WriteLine("     sync: will start a new session if last session was finished or resume last session from the broken point if last session was aborted.");
            Console.WriteLine("     redo: will re-run last session");
            Console.WriteLine("   rebase: will start a new session which will synchronize data from " + SyncSession.BaseModifiedDate);
            Console.WriteLine("    count: fetch the count of supporters on salsa");
            Console.WriteLine("   delete: delete all supporters on salsa (cannot be run on production environment)");
            Console.WriteLine("");
            Console.WriteLine("The default environment is test.");
        }
    }
}