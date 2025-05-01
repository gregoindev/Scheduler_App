using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using MySql.Data.MySqlClient;

namespace gBruno_C969
{
    public static class Database
    {
        public static MySqlConnection Connection => new MySqlConnection(ConfigurationManager.ConnectionStrings["client_schedule"].ConnectionString);
    }
}
