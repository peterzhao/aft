using System;
using System.Collections.Specialized;
using SalsaImporter.Salsa;
using SalsaImporter.Synchronization;

namespace SalsaImporter
{
    public class CreateTestingCustomColumns
    {
        private readonly SalsaClient _salsa;
        private readonly SyncErrorHandler _errorHandler;

        public CreateTestingCustomColumns()
        {
            _errorHandler = new SyncErrorHandler(500);
            _salsa = new SalsaClient(_errorHandler);

            _salsa.Login();
        }

        public void CreateCustomColumns()
        {
            _salsa.DeleteAllObjects("custom_column", 100, false);
            Logger.Debug("Creating custom fields of string...");
            for (int i = 0; i < 10; i++)
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
            Logger.Debug("Creating custom fields of integer...");
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

            for (int i = 0; i < 1; i++)
            {
                var customColumn = new NameValueCollection
                                       {
                                           {"name", String.Format("CustomDateTime{0}", i)},
                                           {"label", String.Format("Custom DateTime {0}", i)},
                                           {"type", "datetime"}
                                       };
                _salsa.CreateSupporterCustomColumn(customColumn);
            }

            Logger.Debug("Created custom fields.");
        }
    }
}
