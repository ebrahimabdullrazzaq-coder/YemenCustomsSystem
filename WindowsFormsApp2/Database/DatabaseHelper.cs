using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public static class DatabaseHelper
    {
        private static string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YemenCustomsDB.sqlite");
        private static string connectionString = $"Data Source={dbPath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(connectionString);
        }

        public static void TestConnection()
        {
            try
            {
                if (!File.Exists(dbPath))
                {
                    SQLiteConnection.CreateFile(dbPath);
                }
                using (var conn = GetConnection())
                {
                    conn.Open();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الاتصال بقاعدة البيانات:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void InitializeDatabase()
        {
            TestConnection();

            using (var conn = GetConnection())
            {
                conn.Open();




               

                // ✅ أضف هذا الكود لإنشاء جدول المنافذ
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Ports (
            PortId INTEGER PRIMARY KEY AUTOINCREMENT,
            PortNumber TEXT,
            PortName TEXT NOT NULL,
            PortNature TEXT,
            PortType TEXT,
            Classification TEXT,
            ExchangeRate TEXT,
            Currency TEXT,
            ExchangeRateType TEXT,
            ExchangeRateUSD REAL DEFAULT 0,
            MinRate REAL DEFAULT 0,
            MaxRate REAL DEFAULT 0,
            UserNumber TEXT,
            PaymentMethod TEXT,
            IsActive INTEGER DEFAULT 1,
            CreatedDate TEXT,
            ModifiedDate TEXT
        )", conn);

                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS PortsAuditLog (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PortName TEXT NOT NULL,
            PortNature TEXT,
            PortType TEXT,
            Classification TEXT,
            ExchangeRate TEXT,
            Currency TEXT,
            ExchangeRateType TEXT,
            UserNumber TEXT,
            ModifiedDate TEXT NOT NULL,
            OperationType TEXT NOT NULL
        )", conn);

          

        ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Items (
                    ItemCode TEXT PRIMARY KEY, GroupNo TEXT, GroupName TEXT, ItemName TEXT,
                    ItemType TEXT, SubGroupUnder TEXT, Branch TEXT, RefNo TEXT, Barcode TEXT,
                    ForeignName TEXT, ItemSpecs TEXT, InitialCost REAL DEFAULT 0, DefaultUnit TEXT,
                    ItemActivity TEXT, OpenCost REAL DEFAULT 0, AvailableQty REAL DEFAULT 0,
                    UnitsJson TEXT, WeightUnitsJson TEXT, CostsJson TEXT, ItemImage BLOB, ModifiedDate TEXT
                )", conn);

                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Groups (
                    GroupNo TEXT PRIMARY KEY, GroupName TEXT NOT NULL, ForeignName TEXT, Description TEXT, IsActive INTEGER DEFAULT 1
                )", conn);

                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS MeasurementUnits (
                    UnitCode TEXT PRIMARY KEY, UnitName TEXT NOT NULL, IsActive INTEGER DEFAULT 1
                )", conn);

                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS WeightUnits (
                    UnitName TEXT PRIMARY KEY, Weight REAL NOT NULL
                )", conn);

                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS TariffRequests (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StatementNo TEXT NOT NULL,
                    PortName TEXT NOT NULL,
                    TariffType TEXT,
                    ClientName TEXT,
                    CarNo TEXT,
                    StatementDate TEXT,
                    DriversJson TEXT,
                    AdditionalAmountsJson TEXT,
                    ItemsJson TEXT,
                    Status TEXT DEFAULT 'pending',
                    CreatedBy TEXT,
                    CreatedDate TEXT,
                    ReviewedBy TEXT,
                    ReviewedDate TEXT,
                    ReviewNotes TEXT
                )", conn);

                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS DocumentReviews (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StatementNo TEXT NOT NULL,
                    PortName TEXT NOT NULL,
                    ReviewStatus TEXT NOT NULL,
                    Notes TEXT,
                    UserName TEXT,
                    ReviewDate TEXT NOT NULL
                )", conn);
            }
        }

        public static int ExecuteNonQuery(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static int ExecuteNonQuery(string query, SQLiteConnection conn, params SQLiteParameter[] parameters)
        {
            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        public static object ExecuteScalar(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    return cmd.ExecuteScalar();
                }
            }
        }

        public static object ExecuteScalar(string query, SQLiteConnection conn, params SQLiteParameter[] parameters)
        {
            using (var cmd = new SQLiteCommand(query, conn))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }

        public static DataTable ExecuteQuery(string query, params SQLiteParameter[] parameters)
        {
            using (var conn = GetConnection())
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    if (parameters != null) cmd.Parameters.AddRange(parameters);
                    using (var adapter = new SQLiteDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public static void EnsureAllTablesExist()
        {
            try
            {
                // 1. جدول العملات
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Currencies (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            CurrencyNumber TEXT NOT NULL UNIQUE,
            CurrencyName TEXT NOT NULL,
            CurrencySymbol TEXT,
            CurrencyType TEXT DEFAULT 'محلية',
            ExchangeRate REAL DEFAULT 1.0,
            MinRate REAL DEFAULT 0.0,
            MaxRate REAL DEFAULT 0.0,
            ForeignName TEXT,
            SubUnit TEXT,
            DecimalPlaces INTEGER DEFAULT 2,
            IsStored INTEGER DEFAULT 0,
            IsActive INTEGER DEFAULT 1,
            CreatedDate TEXT,
            ModifiedDate TEXT
        )");

                // 2. جدول سجل مراجعة العملات (جدول سجلات، حذفه آمن)
                ExecuteNonQuery(@"DROP TABLE IF EXISTS CurrenciesAuditLog");
                ExecuteNonQuery(@"CREATE TABLE CurrenciesAuditLog (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            CurrencyId INTEGER,
            CurrencyName TEXT,
            Action TEXT NOT NULL,
            OldRate REAL,
            NewRate REAL,
            OldMinRate REAL,
            NewMinRate REAL,
            OldMaxRate REAL,
            NewMaxRate REAL,
            UserName TEXT,
            ChangeDate TEXT NOT NULL
        )");

                // 3. جدول المنافذ 
                // ⚠️ هام جداً: استخدمنا CREATE TABLE IF NOT EXISTS لمنع حذف بيانات المنافذ عند كل تشغيل للبرنامج
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Ports (
            PortId INTEGER PRIMARY KEY AUTOINCREMENT,
            PortNumber TEXT,
            PortName TEXT NOT NULL,
            PortNature TEXT,
            PortType TEXT,
            Classification TEXT,
            ExchangeRate TEXT,
            Currency TEXT,
            ExchangeRateType TEXT,
            ExchangeRateUSD REAL DEFAULT 0,
            MinRate REAL DEFAULT 0,
            MaxRate REAL DEFAULT 0,
            UserNumber TEXT,
            PaymentMethod TEXT,
            IsActive INTEGER DEFAULT 1,
            CreatedDate TEXT,
            ModifiedDate TEXT
        )");

                // جدول سجل مراجعة المنافذ
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS PortsAuditLog (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PortName TEXT NOT NULL,
            PortNature TEXT,
            PortType TEXT,
            Classification TEXT,
            ExchangeRate TEXT,
            Currency TEXT,
            ExchangeRateType TEXT,
            UserNumber TEXT,
            ModifiedDate TEXT NOT NULL,
            OperationType TEXT NOT NULL
        )");

                // جدول التخليص الجمركي
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS CustomsClearance (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PortName TEXT,
            StatementDate TEXT,
            StatementNo TEXT,
            TariffType TEXT,
            UnloadFrom TEXT,
            TariffRequestNo TEXT,
            ClientName TEXT,
            CarNo TEXT,
            ProductName TEXT,
            Unit TEXT,
            WeightUnit TEXT,
            Quantity REAL DEFAULT 0,
            Weight REAL DEFAULT 0,
            WeightTotal REAL DEFAULT 0,
            CustomsFeeTotal REAL DEFAULT 0,
            STTotal REAL DEFAULT 0,
            VATTotal REAL DEFAULT 0,
            PTTotal REAL DEFAULT 0,
            VFTotal REAL DEFAULT 0,
            TEFTotal REAL DEFAULT 0,
            MFFTotal REAL DEFAULT 0,
            Other REAL DEFAULT 0,
            Total REAL DEFAULT 0,
            AgriculturePermit REAL DEFAULT 0,
            PaymentRelease REAL DEFAULT 0,
            CountryOfOrigin TEXT,
            TransferredQty REAL DEFAULT 0,
            RemainingQty REAL DEFAULT 0,
            DriversJson TEXT,
            AdditionalAmountsJson TEXT,
            CreatedDate TEXT,
            ModifiedDate TEXT
        )");

                // 4. جدول المخازن
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Warehouses (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            WarehouseCode TEXT NOT NULL UNIQUE,
            WarehouseName TEXT NOT NULL,
            WarehouseType TEXT,
            PortLink TEXT,
            Address TEXT,
            Manager TEXT,
            Phone TEXT,
            Status TEXT DEFAULT 'نشط',
            Notes TEXT,
            IsActive INTEGER DEFAULT 1,
            CreatedDate TEXT,
            ModifiedDate TEXT
        )");

                // 5. جدول بيانات التسعير الجمركي للأصناف
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS ItemsPricingData (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ProductName TEXT NOT NULL,
            PortClassification TEXT NOT NULL,
            PortName TEXT NOT NULL,
            Unit TEXT,
            WeightUnit TEXT,
            Weight REAL DEFAULT 0,
            CustomsPriceUSD REAL DEFAULT 0,
            ExchangeRate REAL DEFAULT 0,
            CustomsPriceRY REAL DEFAULT 0,
            CustomsRatePct REAL DEFAULT 0,
            CustomsValue REAL DEFAULT 0,
            ST_Pct REAL DEFAULT 0,
            VAT_Pct REAL DEFAULT 0,
            PT_Pct REAL DEFAULT 0,
            VF_Pct REAL DEFAULT 0,
            TEF_Pct REAL DEFAULT 0,
            MFF_Pct REAL DEFAULT 0,
            TotalCustoms REAL DEFAULT 0,
            TotalST REAL DEFAULT 0,
            TotalVAT REAL DEFAULT 0,
            TotalPT REAL DEFAULT 0,
            TotalVF REAL DEFAULT 0,
            TotalTEF REAL DEFAULT 0,
            TotalMFF REAL DEFAULT 0,
            CountryOfOrigin TEXT,
            IsActive INTEGER DEFAULT 1,
            CreatedDate TEXT,
            ModifiedDate TEXT
        )");

                // 6. جدول الأصناف
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS Items (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            ItemCode TEXT NOT NULL UNIQUE,
            GroupNo TEXT,
            GroupName TEXT,
            ItemName TEXT NOT NULL,
            ItemType TEXT,
            SubGroupUnder TEXT,
            Branch TEXT,
            RefNo TEXT,
            Barcode TEXT,
            ForeignName TEXT,
            ItemSpecs TEXT,
            InitialCost REAL DEFAULT 0,
            DefaultUnit TEXT,
            ItemActivity TEXT DEFAULT 'نشط',
            OpenCost REAL DEFAULT 0,
            AvailableQty REAL DEFAULT 0,
            UnitsJson TEXT,
            WeightUnitsJson TEXT,
            CostsJson TEXT,
            CountryOfOrigin TEXT,
            AgriculturePermit INTEGER DEFAULT 0,
            PaymentRelease INTEGER DEFAULT 0,
            ItemImage BLOB,
            IsActive INTEGER DEFAULT 1,
            CreatedDate TEXT,
            ModifiedDate TEXT
        )");

                // ✅ إضافة الأعمدة الناقصة للجدول الموجود مسبقاً (كإجراء احتياطي آمن)
                try { ExecuteNonQuery("ALTER TABLE Items ADD COLUMN CountryOfOrigin TEXT"); } catch { }
                try { ExecuteNonQuery("ALTER TABLE Items ADD COLUMN AgriculturePermit INTEGER DEFAULT 0"); } catch { }
                try { ExecuteNonQuery("ALTER TABLE Items ADD COLUMN PaymentRelease INTEGER DEFAULT 0"); } catch { }


                // ✅ 7. جدول مجموعات المنافذ (مطلوب لشاشة الأصناف)
                ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS PortGroups (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    GroupName TEXT NOT NULL UNIQUE,
                    Description TEXT,
                    IsActive INTEGER DEFAULT 1,
                    CreatedDate TEXT
                )");

                // إضافة بيانات افتراضية إذا كان الجدول فارغاً تماماً
                try
                {
                    var count = ExecuteScalar("SELECT COUNT(*) FROM PortGroups");
                    if (Convert.ToInt32(count) == 0)
                    {
                        ExecuteNonQuery("INSERT INTO PortGroups (GroupName, Description, IsActive) VALUES ('مجموعة المنافذ الشمالية', 'منافذ المنطقة الشمالية', 1)");
                        ExecuteNonQuery("INSERT INTO PortGroups (GroupName, Description, IsActive) VALUES ('مجموعة المنافذ الجنوبية', 'منافذ المنطقة الجنوبية', 1)");
                        ExecuteNonQuery("INSERT INTO PortGroups (GroupName, Description, IsActive) VALUES ('مجموعة المنافذ الشرقية', 'منافذ المنطقة الشرقية', 1)");
                        ExecuteNonQuery("INSERT INTO PortGroups (GroupName, Description, IsActive) VALUES ('مجموعة المنافذ الغربية', 'منافذ المنطقة الغربية', 1)");
                    }
                }
                catch { }

                Console.WriteLine("✅ تم إنشاء/تحديث جميع الجداول بنجاح");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في إنشاء الجداول: {ex.Message}");
            }
        }
        public static SQLiteParameter CreateParameter(string name, object value)
        {
            return new SQLiteParameter(name, value ?? DBNull.Value);
        }
    }
}