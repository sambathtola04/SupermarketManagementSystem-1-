using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SupermarketManagementSystem.Helpers
{
    /// <summary>
    /// Central place for the SQL Server connection.
    /// Edit the connection string below to match your SSMS instance.
    /// </summary>
    public static class DBHelper
    {
        // Examples:
        //  Local default instance: "Server=.;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True;"
        //  Named instance (Express): "Server=.\\SQLEXPRESS;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True;"
        //  SQL login instead of Windows auth:
        //  "Server=.;Database=SupermarketDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
       public static string ConnectionString { get; set; } =
    @"Server=(localdb)\MSSQLLocalDB;Database=SupermarketDB;Trusted_Connection=True;";        public static SqlConnection GetConnection() => new SqlConnection(ConnectionString);

        public static bool TestConnection(out string message)
        {
            try
            {
                using var conn = GetConnection();
                conn.Open();
                message = "OK";
                return true;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return false;
            }
        }

        public static DataTable ExecuteQuery(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);

            var table = new DataTable();
            conn.Open();
            using var adapter = new SqlDataAdapter(cmd);
            adapter.Fill(table);
            return table;
        }

        public static int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public static object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            using var conn = GetConnection();
            using var cmd = new SqlCommand(sql, conn);
            if (parameters != null) cmd.Parameters.AddRange(parameters);
            conn.Open();
            return cmd.ExecuteScalar();
        }
    }
}
