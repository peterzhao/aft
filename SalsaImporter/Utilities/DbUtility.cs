using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SalsaImporter.Utilities
{
    public class DbUtility
    {
        public static List<DataColumn>  GetColumnsInfo(string tableName)
        {
            var result = new List<DataColumn>();
            using (var dataAdaptor = new SqlDataAdapter(String.Format("SELECT * FROM {0}", tableName),
                                               Config.DbConnectionString))
            {
                var dataSet = new DataSet();
                dataAdaptor.Fill(dataSet);

                var table = dataSet.Tables[0];

                result.AddRange(table.Columns.Cast<DataColumn>());
            }
            return result;
        }

        public static bool IsTableExist(string tableName)
        {
            try
            {
                GetColumnsInfo(tableName);
                return true;
            }catch(Exception)
            {
                return false;
            }
        }
    }
}
