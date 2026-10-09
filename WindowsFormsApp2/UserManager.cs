using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;
using System.Data.SQLite;

namespace YemenCustomsSystem
{
    // ==================== ✅ كلاس صلاحيات الشاشة ====================
    public class ScreenPermission
    {
        public string ScreenName { get; set; }
        public string ScreenTag { get; set; }
        public bool Include { get; set; }
        public bool Print { get; set; }
        public bool ViewReport { get; set; }
        public bool AddFrom { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }
        public bool View { get; set; }
        public bool ViewList { get; set; }
        public bool SendTo { get; set; }
        public bool Review { get; set; }
        public bool Post { get; set; }
        public bool CancelPost { get; set; }

        public bool IsGranted()
        {
            return Include || Print || ViewReport || AddFrom || Add ||
                   Edit || Delete || View || ViewList || SendTo ||
                   Review || Post || CancelPost;
        }
    }

    public static class UserManager
    {
        private static User currentUser = null;

        private static Dictionary<int, List<ScreenPermission>> _userScreenPermissions =
            new Dictionary<int, List<ScreenPermission>>();

        public const string PERM_PORTS = "ports";
        public const string PERM_WAREHOUSES = "warehouses";
        public const string PERM_ITEMS = "items";
        public const string PERM_ITEMS_DATA = "items_data";
        public const string PERM_CLEARANCE = "clearance";
        public const string PERM_STOCK_TRANSFER = "stock_transfer";
        public const string PERM_WAREHOUSE_TRANSFER = "warehouse_transfer";
        public const string PERM_USERS = "users";
        public const string PERM_REPORTS = "reports";

        public class User
        {
            public int Id { get; set; }
            public string UserNumber { get; set; }
            public string Username { get; set; }
            public string PasswordHash { get; set; }
            public string FullName { get; set; }
            public string Role { get; set; }
            public bool IsActive { get; set; }
            public Dictionary<string, bool> Permissions { get; set; } = new Dictionary<string, bool>();
            public DateTime CreatedDate { get; set; }
            public DateTime? LastLogin { get; set; }
            public string CreatedBy { get; set; }
        }

        // ==================== ✅ التهيئة ====================
        public static void Initialize()
        {
            try
            {
                // ✅ 1. إنشاء جدول المستخدمين إذا لم يكن موجوداً (هذا هو السبب الرئيسي للمشكلة)
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Users (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserNumber TEXT NOT NULL,
                    Username TEXT NOT NULL UNIQUE,
                    Password TEXT NOT NULL,
                    FullName TEXT NOT NULL,
                    Role TEXT NOT NULL DEFAULT 'User',
                    PermissionsJson TEXT,
                    IsActive INTEGER DEFAULT 1,
                    CreatedBy TEXT,
                    CreatedDate TEXT,
                    ModifiedDate TEXT,
                    LastLogin TEXT
                )");

                // ✅ 2. التحقق من عدد المستخدمين في الجدول
                var countObj = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM Users");
                int count = countObj != null && countObj != DBNull.Value ? Convert.ToInt32(countObj) : 0;

                // ✅ 3. إنشاء المستخدم الافتراضي (Admin) إذا كان الجدول فارغاً
                if (count == 0)
                {
                    string defaultHash = HashPassword("admin");
                    string defaultPerms = JsonSerializer.Serialize(GetAllPermissions(true));

                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO Users (UserNumber, Username, Password, FullName, Role, PermissionsJson, IsActive, CreatedBy, CreatedDate, ModifiedDate) 
                        VALUES ('1', 'admin', @Password, 'المدير العام', 'Admin', @PermissionsJson, 1, 'System', datetime('now'), datetime('now'))",
                        DatabaseHelper.CreateParameter("@Password", defaultHash),
                        DatabaseHelper.CreateParameter("@PermissionsJson", defaultPerms)
                    );

                  
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تهيئة نظام المستخدمين:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== ✅ أدوار المستخدمين ====================
        public const string ROLE_ADMIN = "Admin";
        public const string ROLE_OFFICER = "CustomsOfficer";
        public const string ROLE_REVIEWER = "Reviewer";

        // ✅ قائمة الأدوار كما تظهر في القوائم المنسدلة
        public static List<string> GetRoleDisplays()
        {
            return new List<string>
    {
        "مدير (Admin)",
        "مخلص جمركي (CustomsOfficer)",
        "مراجع (Reviewer)"
    };
        }

        // ✅ تحويل النص المعروض إلى قيمة الدور المخزنة
        public static string GetRoleValueFromDisplay(string display)
        {
            if (string.IsNullOrEmpty(display)) return ROLE_OFFICER;
            if (display.Contains("Admin")) return ROLE_ADMIN;
            if (display.Contains("Reviewer")) return ROLE_REVIEWER;
            return ROLE_OFFICER;
        }

        // ✅ تحويل قيمة الدور المخزنة إلى النص المعروض
        public static string GetRoleDisplayWithCode(string role)
        {
            if (role == ROLE_ADMIN) return "مدير (Admin)";
            if (role == ROLE_REVIEWER) return "مراجع (Reviewer)";
            return "مخلص جمركي (CustomsOfficer)";
        }

        // ✅ الاسم العربي المختصر للدور (للعرض في الجداول)
        public static string GetRoleArabicName(string role)
        {
            if (role == ROLE_ADMIN) return "👑 مدير";
            if (role == ROLE_REVIEWER) return "🔍 مراجع";
            return "👤 مخلص";
        }

        // ✅ هل المستخدم الحالي مراجع؟
        public static bool IsReviewer()
        {
            if (currentUser == null) return false;
            return string.Equals(currentUser.Role?.Trim(), ROLE_REVIEWER, StringComparison.OrdinalIgnoreCase);
        }

        public static void LoadUsers() => Initialize();

        // ==================== ✅ تشفير كلمة المرور ====================
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                    builder.Append(bytes[i].ToString("x2"));
                return builder.ToString();
            }
        }

        public static bool VerifyPassword(string password, string hash) => HashPassword(password) == hash;

        // ==================== ✅ تحويل الصف إلى مستخدم ====================
        private static User MapRowToUser(DataRow row)
        {
            return new User
            {
                Id = Convert.ToInt32(row["Id"]),
                UserNumber = row["UserNumber"] != DBNull.Value ? row["UserNumber"].ToString() : "",
                Username = row["Username"].ToString(),
                PasswordHash = row["Password"].ToString(),
                FullName = row["FullName"] != DBNull.Value ? row["FullName"].ToString() : "",
                Role = row["Role"] != DBNull.Value ? row["Role"].ToString() : "",
                IsActive = row["IsActive"] != DBNull.Value && Convert.ToBoolean(row["IsActive"]),
                CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now,
                LastLogin = row["LastLogin"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["LastLogin"]) : null,
                CreatedBy = row["CreatedBy"] != DBNull.Value ? row["CreatedBy"].ToString() : null,
                Permissions = string.IsNullOrEmpty(row["PermissionsJson"]?.ToString())
                    ? GetAllPermissions(false)
                    : JsonSerializer.Deserialize<Dictionary<string, bool>>(row["PermissionsJson"].ToString())
            };
        }

        // ==================== ✅ جلب المستخدمين ====================
        public static List<User> GetAllUsers()
        {
            List<User> users = new List<User>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Users ORDER BY UserNumber");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                        users.Add(MapRowToUser(row));
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ جلب المستخدمين: {ex.Message}"); }
            return users;
        }

        public static User GetUserByUsername(string username)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Users WHERE Username = @Username",
                    DatabaseHelper.CreateParameter("@Username", username));
                if (dt != null && dt.Rows.Count > 0) return MapRowToUser(dt.Rows[0]);
            }
            catch { }
            return null;
        }

        public static User GetUserByUserNumber(string userNumber)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Users WHERE UserNumber = @UserNumber",
                    DatabaseHelper.CreateParameter("@UserNumber", userNumber));
                if (dt != null && dt.Rows.Count > 0) return MapRowToUser(dt.Rows[0]);
            }
            catch { }
            return null;
        }

        public static User GetUserById(int id)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Users WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@Id", id));
                if (dt != null && dt.Rows.Count > 0) return MapRowToUser(dt.Rows[0]);
            }
            catch { }
            return null;
        }

        // ==================== ✅ التحقق من التكرار ====================
        public static bool UsernameExists(string username, int excludeId = 0)
        {
            try
            {
                var countObj = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Id != @ExcludeId",
                    DatabaseHelper.CreateParameter("@Username", username),
                    DatabaseHelper.CreateParameter("@ExcludeId", excludeId));
                return countObj != null && Convert.ToInt32(countObj) > 0;
            }
            catch { return false; }
        }

        public static bool UserNumberExists(string userNumber, int excludeId = 0)
        {
            try
            {
                var countObj = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Users WHERE UserNumber = @UserNumber AND Id != @ExcludeId",
                    DatabaseHelper.CreateParameter("@UserNumber", userNumber),
                    DatabaseHelper.CreateParameter("@ExcludeId", excludeId));
                return countObj != null && Convert.ToInt32(countObj) > 0;
            }
            catch { return false; }
        }

        // ==================== ✅ إضافة مستخدم جديد ====================
        public static User AddUser(string userNumber, string username, string password, string fullName, string role, bool isActive)
        {
            if (UserNumberExists(userNumber)) return null;
            if (UsernameExists(username)) return null;

            try
            {
                string permJson = JsonSerializer.Serialize(GetAllPermissions(false));
                string hashedPassword = HashPassword(password);

                var newId = DatabaseHelper.ExecuteScalar(@"INSERT INTO Users (UserNumber, Username, Password, FullName, Role, PermissionsJson, IsActive, CreatedBy, CreatedDate, ModifiedDate) 
                    VALUES (@UserNumber, @Username, @Password, @FullName, @Role, @PermissionsJson, @IsActive, @CreatedBy, datetime('now'), datetime('now'));
                    SELECT last_insert_rowid();",
                    DatabaseHelper.CreateParameter("@UserNumber", userNumber),
                    DatabaseHelper.CreateParameter("@Username", username),
                    DatabaseHelper.CreateParameter("@Password", hashedPassword),
                    DatabaseHelper.CreateParameter("@FullName", fullName),
                    DatabaseHelper.CreateParameter("@Role", role),
                    DatabaseHelper.CreateParameter("@PermissionsJson", permJson),
                    DatabaseHelper.CreateParameter("@IsActive", isActive ? 1 : 0),
                    DatabaseHelper.CreateParameter("@CreatedBy", currentUser != null ? currentUser.FullName : "System")
                );
                return GetUserById(Convert.ToInt32(newId));
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ إضافة مستخدم: {ex.Message}"); return null; }
        }

        // ==================== ✅ صيغة قديمة للتوافق مع الشاشات القديمة ====================
        public static User AddUser(string username, string password, string fullName, string role, Dictionary<string, bool> permissions, string createdBy)
        {
            // توليد رقم مستخدم جديد تلقائياً
            string userNumber = GenerateNextUserNumber();

            // استدعاء الصيغة الجديدة
            var newUser = AddUser(userNumber, username, password, fullName, role, true);

            // تطبيق الصلاحيات القديمة إن وجدت
            if (newUser != null && permissions != null)
            {
                UpdateUser(newUser.Id, fullName, role, permissions, true);
            }

            return newUser;
        }


        // ==================== ✅ التحقق من صلاحية المستخدم الحالي على شاشة ====================
        public static bool Can(string screenTag, string key)
        {
            if (currentUser == null) return false;
            if (currentUser.Role == "Admin") return true; // الأدمن يملك كل الصلاحيات

            if (!_userScreenPermissions.ContainsKey(currentUser.Id))
                LoadScreenPermissionsFromDatabase(currentUser.Id);

            var perms = GetScreenPermissions(currentUser.Id);
            var p = perms.FirstOrDefault(x => x.ScreenTag == screenTag);
            if (p == null) return false;

            return GetPermValue(p, key);
        }

        // ==================== ✅ إدارة صلاحيات المنافذ ====================
        private static Dictionary<int, List<PortPermission>> _userPortPermissions =
            new Dictionary<int, List<PortPermission>>();

        public static List<PortPermission> GetPortPermissions(int userId)
        {
            if (_userPortPermissions.ContainsKey(userId)) return _userPortPermissions[userId];
            var list = new List<PortPermission>();
            _userPortPermissions[userId] = list;
            return list;
        }

        public static void LoadPortPermissionsFromDatabase(int userId)
        {
            try
            {
                var result = DatabaseHelper.ExecuteScalar(
                    "SELECT PortPermissionsJson FROM Users WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@Id", userId));

                if (result != null && result != DBNull.Value && !string.IsNullOrEmpty(result.ToString()))
                {
                    _userPortPermissions[userId] = JsonSerializer.Deserialize<List<PortPermission>>(result.ToString());
                    return;
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ تحميل صلاحيات المنافذ: {ex.Message}"); }
            _userPortPermissions[userId] = new List<PortPermission>();
        }

        public static void SavePortPermissions(int userId, List<PortPermission> perms)
        {
            _userPortPermissions[userId] = perms;
            try
            {
                string json = JsonSerializer.Serialize(perms);
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Users SET PortPermissionsJson = @P WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@P", json),
                    DatabaseHelper.CreateParameter("@Id", userId));
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ حفظ صلاحيات المنافذ: {ex.Message}"); }
        }

        public static bool GetPortPermValue(PortPermission p, string key)
        {
            switch (key)
            {
                case "View": return p.View;
                case "Add": return p.Add;
                case "Edit": return p.Edit;
                case "Delete": return p.Delete;
                case "Print": return p.Print;
            }
            return false;
        }

        // ✅ هل المستخدم الحالي يملك صلاحية على منفذ معين؟
        public static bool CanPort(string portName, string key)
        {
            if (currentUser == null) return false;
            if (currentUser.Role == "Admin") return true;

            if (!_userPortPermissions.ContainsKey(currentUser.Id))
                LoadPortPermissionsFromDatabase(currentUser.Id);

            var p = GetPortPermissions(currentUser.Id).FirstOrDefault(x => x.PortName == portName);
            if (p == null) return false;
            return GetPortPermValue(p, key);
        }

        // ✅ قائمة المنافذ المسموحة للمستخدم الحالي
        public static List<string> GetAllowedPorts(string key = "View")
        {
            var names = new List<string>();
            if (currentUser == null) return names;

            if (currentUser.Role == "Admin")
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName FROM Ports ORDER BY PortName");
                foreach (DataRow r in dt.Rows) names.Add(r["PortName"].ToString());
                return names;
            }

            if (!_userPortPermissions.ContainsKey(currentUser.Id))
                LoadPortPermissionsFromDatabase(currentUser.Id);

            foreach (var p in GetPortPermissions(currentUser.Id))
            {
                if (GetPortPermValue(p, key)) names.Add(p.PortName);
            }
            return names;
        }


        // ==================== ✅ كلاس صلاحيات المنافذ (المدخلات) ====================
        public class PortPermission
        {
            public string PortName { get; set; }
            public bool View { get; set; }
            public bool Add { get; set; }
            public bool Edit { get; set; }
            public bool Delete { get; set; }
            public bool Print { get; set; }
        }
        public static bool GetPermValue(ScreenPermission p, string key)
        {
            switch (key)
            {
                case "Include": return p.Include;
                case "Print": return p.Print;
                case "ViewReport": return p.ViewReport;
                case "AddFrom": return p.AddFrom;
                case "Add": return p.Add;
                case "Edit": return p.Edit;
                case "Delete": return p.Delete;
                case "View": return p.View;
                case "ViewList": return p.ViewList;
                case "SendTo": return p.SendTo;
                case "Review": return p.Review;
                case "Post": return p.Post;
                case "CancelPost": return p.CancelPost;
            }
            return false;
        }
        // ==================== ✅ توليد رقم المستخدم التالي تلقائياً ====================
        private static string GenerateNextUserNumber()
        {
            int maxNum = 0;
            foreach (var u in GetAllUsers())
            {
                int n;
                if (int.TryParse(u.UserNumber, out n) && n > maxNum)
                    maxNum = n;
            }
            return (maxNum + 1).ToString();
        }


        // ==================== ✅ تحديث مستخدم ====================
        public static bool UpdateUser(int id, string fullName, string role, Dictionary<string, bool> permissions, bool isActive)
        {
            try
            {
                string permJson = JsonSerializer.Serialize(permissions ?? GetAllPermissions(false));
                DatabaseHelper.ExecuteNonQuery(@"UPDATE Users SET 
                    FullName = @FullName, Role = @Role, PermissionsJson = @PermissionsJson, 
                    IsActive = @IsActive, ModifiedDate = datetime('now') WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@FullName", fullName),
                    DatabaseHelper.CreateParameter("@Role", role),
                    DatabaseHelper.CreateParameter("@PermissionsJson", permJson),
                    DatabaseHelper.CreateParameter("@IsActive", isActive),
                    DatabaseHelper.CreateParameter("@Id", id)
                );

                if (currentUser != null && currentUser.Id == id)
                {
                    currentUser.FullName = fullName;
                    currentUser.Role = role;
                    currentUser.IsActive = isActive;
                    currentUser.Permissions = permissions ?? GetAllPermissions(false);
                }
                return true;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ تحديث مستخدم: {ex.Message}"); return false; }
        }

        // ==================== ✅ حذف مستخدم ====================
        public static bool DeleteUser(int id)
        {
            var user = GetUserById(id);
            if (user == null || user.Role == "Admin") return false;
            try
            {
                DatabaseHelper.ExecuteNonQuery("DELETE FROM Users WHERE Id = @Id", DatabaseHelper.CreateParameter("@Id", id));
                return true;
            }
            catch { return false; }
        }

        // ==================== ✅ تسجيل الدخول برقم المستخدم ====================
        public static User LoginByUserNumber(string userNumber, string password)
        {
            var user = GetUserByUserNumber(userNumber);
            if (user == null || !user.IsActive) return null;
            if (!VerifyPassword(password, user.PasswordHash)) return null;

            try { DatabaseHelper.ExecuteNonQuery("UPDATE Users SET LastLogin = datetime('now') WHERE Id = @Id", DatabaseHelper.CreateParameter("@Id", user.Id)); }
            catch { }

            user.LastLogin = DateTime.Now;
            currentUser = user;
            return user;
        }

        // ==================== ✅ تسجيل الدخول باسم المستخدم (احتياطي) ====================
        public static User Login(string username, string password)
        {
            var user = GetUserByUsername(username);
            if (user == null || !user.IsActive) return null;
            if (!VerifyPassword(password, user.PasswordHash)) return null;

            try { DatabaseHelper.ExecuteNonQuery("UPDATE Users SET LastLogin = datetime('now') WHERE Id = @Id", DatabaseHelper.CreateParameter("@Id", user.Id)); }
            catch { }

            user.LastLogin = DateTime.Now;
            currentUser = user;
            return user;
        }

        // ==================== ✅ الحالة الحالية ====================
        public static void Logout() => currentUser = null;
        public static User GetCurrentUser() => currentUser;
        public static bool IsLoggedIn() => currentUser != null;
        public static bool IsAdmin()
        {
            if (currentUser == null) return false;
            // ✅ التحقق مع تجاهل حالة الأحرف وإزالة المسافات الزائدة
            return string.Equals(currentUser.Role?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase);
        }

        public static bool HasPermission(string permission)
        {
            if (currentUser == null) return false;
            if (IsAdmin()) return true; // ✅ استخدام الدالة المحدثة هنا أيضاً
            return currentUser.Permissions.ContainsKey(permission) && currentUser.Permissions[permission];
        }

        public static Dictionary<string, bool> GetAllPermissions(bool value)
        {
            return new Dictionary<string, bool>
            {
                { PERM_PORTS, value }, { PERM_WAREHOUSES, value }, { PERM_ITEMS, value },
                { PERM_ITEMS_DATA, value }, { PERM_CLEARANCE, value }, { PERM_STOCK_TRANSFER, value },
                { PERM_WAREHOUSE_TRANSFER, value }, { PERM_USERS, value }, { PERM_REPORTS, value }
            };
        }

        public static Dictionary<string, string> GetPermissionsDisplayNames()
        {
            return new Dictionary<string, string>
            {
                { PERM_PORTS, "🚢 إدارة المنافذ" }, { PERM_WAREHOUSES, "🏢 إدارة المخازن" },
                { PERM_ITEMS, "📦 إدارة الأصناف" }, { PERM_ITEMS_DATA, "📊 بيانات الأصناف" },
                { PERM_CLEARANCE, "🛃 التخليص الجمركي" }, { PERM_STOCK_TRANSFER, "🔄 التحويل المخزني" },
                { PERM_WAREHOUSE_TRANSFER, "🔀 التحويل بين المخازن" }, { PERM_USERS, "👥 إدارة المستخدمين" },
                { PERM_REPORTS, "📈 التقارير" }
            };
        }

        // ==================== ✅ دوال صلاحيات الشاشات ====================
        public static List<ScreenPermission> GetScreenPermissions(int userId)
        {
            if (_userScreenPermissions.ContainsKey(userId))
                return _userScreenPermissions[userId];

            var permissions = new List<ScreenPermission>();
            _userScreenPermissions[userId] = permissions;
            return permissions;
        }

        public static void SaveScreenPermissions(int userId, List<ScreenPermission> permissions)
        {
            _userScreenPermissions[userId] = permissions;

            try
            {
                string json = JsonSerializer.Serialize(permissions);
                DatabaseHelper.ExecuteNonQuery(
                    "UPDATE Users SET ScreenPermissionsJson = @Permissions WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@Permissions", json),
                    DatabaseHelper.CreateParameter("@Id", userId)
                );
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في حفظ صلاحيات الشاشات: {ex.Message}");
            }
        }

        public static void LoadScreenPermissionsFromDatabase(int userId)
        {
            try
            {
                var result = DatabaseHelper.ExecuteScalar(
                    "SELECT ScreenPermissionsJson FROM Users WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@Id", userId)
                );

                if (result != null && result != DBNull.Value)
                {
                    string json = result.ToString();
                    if (!string.IsNullOrEmpty(json))
                    {
                        var permissions = JsonSerializer.Deserialize<List<ScreenPermission>>(json);
                        _userScreenPermissions[userId] = permissions;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل صلاحيات الشاشات: {ex.Message}");
            }
            _userScreenPermissions[userId] = new List<ScreenPermission>();
        }

        // ==================== ✅ قائمة شاشات النظام ====================
        public static List<ScreenPermission> GetAllSystemScreens()
        {
            return new List<ScreenPermission>
            {
                // 1. تهيئة النظام
                new ScreenPermission { ScreenName = "تهيئة العملات", ScreenTag = "Currencies" },
                new ScreenPermission { ScreenName = "مستويات الاعتماد", ScreenTag = "ApprovalLevels" },

                // 2. إدارة النظام
                new ScreenPermission { ScreenName = "بيانات المستخدمين", ScreenTag = "UsersData" },
                new ScreenPermission { ScreenName = "صلاحيات الشاشات", ScreenTag = "ScreensPermissions" },
                new ScreenPermission { ScreenName = "صلاحيات المدخلات", ScreenTag = "InputsPermissions" },
                new ScreenPermission { ScreenName = "تغيير كلمة السر", ScreenTag = "ChangePassword" },
                new ScreenPermission { ScreenName = "الأقفال الشهري", ScreenTag = "MonthlyLocks" },
                new ScreenPermission { ScreenName = "الأقفال السنوي", ScreenTag = "AnnualLocks" },
                new ScreenPermission { ScreenName = "اعدادات النسخ الاحتياطي", ScreenTag = "BackupSettings" },
                new ScreenPermission { ScreenName = "استرجاع النسخه الأحتياطيه", ScreenTag = "RestoreBackup" },


                // 3. أنظمة المخازن
                new ScreenPermission { ScreenName = "وحدات القياس", ScreenTag = "Units" },
                new ScreenPermission { ScreenName = "وحدات الأوزان", ScreenTag = "WeightUnits" },
                new ScreenPermission { ScreenName = "بيانات المخازن", ScreenTag = "Warehouses" },
                new ScreenPermission { ScreenName = "مجموعة الأصناف", ScreenTag = "ItemsGroup" },
                new ScreenPermission { ScreenName = "المجموعات الفرعية", ScreenTag = "SubGroups" }, // ✅ تمت الإضافة هنا
                new ScreenPermission { ScreenName = "تهيئة الأصناف", ScreenTag = "Items" },
                new ScreenPermission { ScreenName = "أنواع التحويل المخزني", ScreenTag = "TransferTypes" },
                new ScreenPermission { ScreenName = "تحويل من منفذ إلى مخزن", ScreenTag = "PortToWarehouseTransfer" },
                new ScreenPermission { ScreenName = "تحويل من مخزن إلى مخزن", ScreenTag = "WarehouseToWarehouseTransfer" },
                new ScreenPermission { ScreenName = "استلام التحويل المخزني", ScreenTag = "ReceiveWarehouseTransfer" },
                new ScreenPermission { ScreenName = "تسوية مخزون", ScreenTag = "InventorySettlement" },
                new ScreenPermission { ScreenName = "تقارير التحويل والاستلامات المخزنية", ScreenTag = "TransferReceiptsReports" },
                new ScreenPermission { ScreenName = "تقارير تسوية مخزون", ScreenTag = "InventorySettlementReports" },

                // 4. نظام التخليص الجمركي - A_التهيئة
                new ScreenPermission { ScreenName = "أنواع الترسيم الجمركي", ScreenTag = "CustomsTariffTypes" },
                new ScreenPermission { ScreenName = "العميل", ScreenTag = "ClientType" },
                new ScreenPermission { ScreenName = "الفرع", ScreenTag = "BranchType" },
                new ScreenPermission { ScreenName = "التسعيرة الجمركية", ScreenTag = "CustomsPricing" },
                new ScreenPermission { ScreenName = "مجموعة المنافذ", ScreenTag = "PortsGroup" },
                new ScreenPermission { ScreenName = "بيانات المنافذ", ScreenTag = "PortsData" },
                new ScreenPermission { ScreenName = "أنواع المبالغ الإضافية في البيان", ScreenTag = "AdditionalAmountsTypes" },
                new ScreenPermission { ScreenName = "المبالغ الإضافية في البيان", ScreenTag = "AdditionalAmounts" },
                new ScreenPermission { ScreenName = "بيانات العملاء والفروع", ScreenTag = "CustomersBranches" },

                              // 4. نظام التخليص الجمركي - B_العمليات
                new ScreenPermission { ScreenName = "طلب تخليص جمركي", ScreenTag = "ClearanceRequest" },
                new ScreenPermission { ScreenName = "تخليص جمركي", ScreenTag = "CustomsClearance" },
                
                // ✅ إضافة الشاشات الجديدة هنا
                new ScreenPermission { ScreenName = "طلب ترسيم", ScreenTag = "TariffRequest" },
                new ScreenPermission { ScreenName = "مراجعة طلبات الترسيم", ScreenTag = "ReviewTariffRequests" },

                new ScreenPermission { ScreenName = "التصاريح الزراعية", ScreenTag = "AgriculturalPermits" },
                new ScreenPermission { ScreenName = "إفراج المدفوعات", ScreenTag = "PaymentsRelease" },
                new ScreenPermission { ScreenName = "تسوية التصاريح والمدفوعات", ScreenTag = "PermitsPaymentsSettlement" },

                // 5. التقارير
                new ScreenPermission { ScreenName = "تقارير طلبات التخليص", ScreenTag = "ClearanceRequestsReports" },
                new ScreenPermission { ScreenName = "تقارير التخليص", ScreenTag = "ClearanceReports" },
                new ScreenPermission { ScreenName = "تقارير الرسوم والمبالغ الإضافية للبيانات", ScreenTag = "FeesAdditionalAmountsReports" },
                new ScreenPermission { ScreenName = "تقارير وحدة التصاريح والإفراجات", ScreenTag = "PermitsReleasesUnitReports" },
                new ScreenPermission { ScreenName = "تقارير حركة التصاريح والإفراجات", ScreenTag = "PermitsReleasesMovementReports" }
            };
        }
    }
}