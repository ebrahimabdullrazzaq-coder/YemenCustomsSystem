using System;
using System.Data;
using System.Data.SqlClient;

namespace WindowsFormsApp2 // ⚠️ تأكد أن هذا يطابق اسم مشروعك تماماً
{
    // ⚠️ مهمة: كلاس عادي ثابت، بدون partial، بدون Designer
    public static class DatabaseHelper
    {
        // سلسلة الاتصال المباشرة (عدّلها حسب سيرفرك)
        private static readonly string ConnectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=YemenCustomsDB;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }

        public static void ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (var conn = GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}