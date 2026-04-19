using System;
using System.Configuration;
using System.Data.SqlClient;

namespace PengaduanPDAM
{
    public static class DatabaseConfig
    {
        public static string GetConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["DBPengaduan"].ConnectionString;
        }

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(GetConnectionString());
        }
    }
}
