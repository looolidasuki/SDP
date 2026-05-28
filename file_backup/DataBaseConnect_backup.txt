using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

// System.Configuration is available via System.Configuration.dll reference
namespace Sales_user.Controllers
{
    public static class DatabaseConnect
    {
        private static string _connectionString;

        private static string GetConnectionString()
        {
            if (_connectionString != null) return _connectionString;
            try
            {
                _connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
            }
            catch { }
            if (string.IsNullOrEmpty(_connectionString))
                _connectionString = "Server=localhost;Port=3306;Database=furniture_erp_system;Uid=root;Pwd=;CharSet=utf8mb4;AllowPublicKeyRetrieval=True;SslMode=Disabled;";
            return _connectionString;
        }

        public static DataTable ExecuteQuery(string sql, MySqlParameter[] parameters = null)
        {
            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    var dt = new DataTable();
                    using (var adapter = new MySqlDataAdapter(cmd))
                        adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public static long ExecuteInsertReturnId(string sql, MySqlParameter[] parameters = null)
        {
            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    cmd.ExecuteNonQuery();
                    return cmd.LastInsertedId;
                }
            }
        }

        public static int ExecuteNonQuery(string sql, MySqlParameter[] parameters = null)
        {
            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static object ExecuteScalar(string sql, MySqlParameter[] parameters = null)
        {
            using (var conn = new MySqlConnection(GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }
    }
}