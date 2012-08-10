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
            _salsa = new SalsaClient();

            _salsa.Login();
        }

        public void CreateCustomColumns()
        {

            Logger.Debug("Creating custom fields of string...");
            for (int i = 0; i < 10; i++)
            {
                string name = String.Format("CustomString{0}", i);
                string value = String.Format("Custom String {0}", i);

                if (0 == _salsa.CountObjectsMatchingQuery("custom_column", "name", Comparator.Equality, name) )
                {
                    var customColumn = new NameValueCollection
                                          {
                                              {"name", name},
                                              {"label", value}, 
                                              {"type", "varchar"}                                           };
                    _salsa.CreateSupporterCustomColumn(customColumn); 
                }

                
                
            }
            for (int i = 0; i < 10; i++)
            {
                string name = String.Format("CustomBoolean{0}", i);
                string label = String.Format("Custom Boolean {0}", i);
                if(0 == _salsa.CountObjectsMatchingQuery("custom_column", "name", Comparator.Equality, name ))
                {

                    var customColumn = new NameValueCollection
                                           {
                                               {"name", name},
                                               {"label", label}, 
                                               {"type", "bool"}
                                           };
                    _salsa.CreateSupporterCustomColumn(customColumn); 
                }
                 
            }
            Logger.Debug("Creating custom fields of integer...");
            for (int i = 0; i < 5; i++)
            {
                string name = String.Format("CustomInteger{0}", i);
                string value = String.Format("Custom Integer {0}", i);
                if (0 == _salsa.CountObjectsMatchingQuery("custom_column", "name", Comparator.Equality, name))
                {
                    var customColumn = new NameValueCollection
                                           {
                                               {"name", name},
                                               {"label", value},
                                               {"type", "int"}
                                           };
                    _salsa.CreateSupporterCustomColumn(customColumn);
                }
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
