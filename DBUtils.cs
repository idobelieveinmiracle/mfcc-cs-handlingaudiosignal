using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TestSignal
{
    class DBUtils
    {
        public static SqlConnection GetDBConnection()
        {
            string datasource = @"QuocHung\SQLEXPRESS";

            string database = "mfcc";
            string username = "sa";
            string password = "123";

            return DBSQLServerUtils.GetDBConnection(datasource, database, username, password);
        }
    }
}
