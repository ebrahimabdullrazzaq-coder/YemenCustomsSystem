using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms; // ✅ تم إضافة هذا السطر لحل المشكلة

namespace YemenCustomsSystem
{
    /// <summary>
    /// كلاس للتعامل مع جدول المجموعات في قاعدة البيانات
    /// </summary>
    public class GroupsRepository
    {
        private string dbPath = Path.Combine(Application.StartupPath, "YemenCustomsDB.sqlite");

        public List<GroupData> GetAll()
        {
            var list = new List<GroupData>();
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Groups", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new GroupData
                        {
                            GroupNo = reader["GroupNo"].ToString(),
                            GroupName = reader["GroupName"].ToString(),
                            ForeignName = reader["ForeignName"]?.ToString() ?? "",
                            Description = reader["Description"]?.ToString() ?? "",
                            IsActive = Convert.ToBoolean(reader["IsActive"])
                        });
                    }
                }
            }
            return list;
        }

        public bool Add(GroupData group)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(@"
                    INSERT INTO Groups (GroupNo, GroupName, ForeignName, Description, IsActive) 
                    VALUES (@GroupNo, @GroupName, @ForeignName, @Description, @IsActive)", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", group.GroupNo);
                        cmd.Parameters.AddWithValue("@GroupName", group.GroupName);
                        cmd.Parameters.AddWithValue("@ForeignName", group.ForeignName ?? "");
                        cmd.Parameters.AddWithValue("@Description", group.Description ?? "");
                        cmd.Parameters.AddWithValue("@IsActive", group.IsActive ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public bool Update(GroupData group)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(@"
                    UPDATE Groups SET 
                        GroupName = @GroupName, 
                        ForeignName = @ForeignName, 
                        Description = @Description, 
                        IsActive = @IsActive 
                    WHERE GroupNo = @GroupNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", group.GroupNo);
                        cmd.Parameters.AddWithValue("@GroupName", group.GroupName);
                        cmd.Parameters.AddWithValue("@ForeignName", group.ForeignName ?? "");
                        cmd.Parameters.AddWithValue("@Description", group.Description ?? "");
                        cmd.Parameters.AddWithValue("@IsActive", group.IsActive ? 1 : 0);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public bool Delete(string groupNo)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("DELETE FROM Groups WHERE GroupNo = @GroupNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", groupNo);
                        cmd.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch { return false; }
        }

        public bool Exists(string groupNo, int excludeId = 0)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand("SELECT COUNT(*) FROM Groups WHERE GroupNo = @GroupNo", conn))
                    {
                        cmd.Parameters.AddWithValue("@GroupNo", groupNo);
                        return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch { return false; }
        }
    }
}