using System;
using System.Collections.Generic;
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

        public void CreateCustomColumns(List<SupporterCustomColumnsRequest> list)
        {
            list.ForEach( CreateSupporterCustomColumns);

            Logger.Debug("Created custom fields.");
        }

        public void CreateSupporterCustomColumns(SupporterCustomColumnsRequest p)
        {

            Logger.Debug(String.Format("Creating custom fields of {0}...", p.SalsaType));
            for (int i = 0; i < p.HowManyToMake; i++)
            {
                var name = String.Format("Custom{0}{1}", p.CustomColumnName, i);
                var label = String.Format("Custom {0} {1}", p.CustomColumnName, i);

                if (0 == _salsa.CountObjectsMatchingQuery("custom_column", "name", Comparator.Equality, name))
                {
                    var customColumn = new NameValueCollection
                                           {
                                               {"name", name},
                                               {"label", label},
                                               {"type", p.SalsaType}
                                           };
                    _salsa.CreateSupporterCustomColumn(customColumn);
                }
            }
        }
    }
}
