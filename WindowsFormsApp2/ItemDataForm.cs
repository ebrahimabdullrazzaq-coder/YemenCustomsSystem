using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Data.SQLite;

namespace YemenCustomsSystem
{
    public partial class ItemDataForm : Form
    {
        private List<ItemData> items = new List<ItemData>();
        private List<GroupData> groups = new List<GroupData>();
        private List<UnitsForm.UnitData> measurementUnits = new List<UnitsForm.UnitData>();
        private List<WeightUnitOption> weightUnitOptions = new List<WeightUnitOption>();
        private int currentIndex = -1;
        private string currentImagePath = null;
        private ComboBox cmbGroupSelect;
        private Button btnOpenGroups;
        private bool _suppressLinkSave = false;

        // ✅ أسماء الأعمدة النصية الجديدة (بدلاً من الأعمدة INTEGER القديمة)
        private const string COL_AGRI = "AgriLinkGroup";
        private const string COL_PAY = "PayLinkGroup";

        private string dataFilePath = Path.Combine(Application.StartupPath, "items_data.json");
        private string groupsDataFilePath = Path.Combine(Application.StartupPath, "groups_data.json");
        private string unitsDataFilePath = Path.Combine(Application.StartupPath, "units_data.json");
        private string weightUnitsFilePath = Path.Combine(Application.StartupPath, "weight_units_options.json");
        private bool isEditingMode = false;
        private bool hasChanges = false;
        private ItemData originalItem = null;

        public ItemDataForm()
        {
            InitializeComponent();
            this.KeyPreview = true;
            InitializeGrids();
            SetupAutoNumbering();
            MarkRequiredFields();
            InitializeGroupControls();
            AddEditButtonToToolbar();
            EnsurePortGroupsTable();
            EnsureItemsTableColumns(); // ✅ يجب أن يكون قبل LoadDataFromDatabase
            LoadDataFromDatabase();
            LoadGroupsFromDatabase();
            LoadMeasurementUnits();
            LoadWeightUnitOptions();
            LoadPortGroupsToDropdowns();
            ClearFields();
            UpdateNavigation();
            SetControlsState(false);
            LanguageManager.ApplyLanguage(this);
            LanguageManager.LanguageChanged += (s, ev) => RefreshAllTranslations();
        }

        // ==================== ✅ إنشاء جدول مجموعات المنافذ ====================
        private void EnsurePortGroupsTable()
        {
            try
            {
                var tableCheck = DatabaseHelper.ExecuteScalar(
                    "SELECT name FROM sqlite_master WHERE type='table' AND name='PortGroups'");

                if (tableCheck == null || string.IsNullOrEmpty(tableCheck.ToString()))
                {
                    DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE PortGroups (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        GroupNo TEXT UNIQUE NOT NULL,
                        GroupName TEXT NOT NULL,
                        IsActive INTEGER DEFAULT 1,
                        Description TEXT,
                        CreatedDate TEXT DEFAULT (datetime('now'))
                    )");

                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('1', 'مجموعة المنافذ الشمالية', 'منافذ المنطقة الشمالية', 1)");
                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('2', 'مجموعة المنافذ الجنوبية', 'منافذ المنطقة الجنوبية', 1)");
                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('3', 'مجموعة المنافذ الشرقية', 'منافذ المنطقة الشرقية', 1)");
                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('4', 'مجموعة المنافذ الغربية', 'منافذ المنطقة الغربية', 1)");

                    Console.WriteLine("✅ تم إنشاء جدول PortGroups وإضافة البيانات الافتراضية");
                }
                else
                {
                    try
                    {
                        DatabaseHelper.ExecuteNonQuery("ALTER TABLE PortGroups ADD COLUMN IsActive INTEGER DEFAULT 1");
                        DatabaseHelper.ExecuteNonQuery("ALTER TABLE PortGroups ADD COLUMN Description TEXT");
                        DatabaseHelper.ExecuteNonQuery("ALTER TABLE PortGroups ADD COLUMN CreatedDate TEXT DEFAULT (datetime('now'))");
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في EnsurePortGroupsTable: {ex.Message}");
            }
        }

        // ==================== ✅ التأكد من وجود أعمدة الربط النصية في جدول Items ====================
        private void EnsureItemsTableColumns()
        {
            try
            {
                DataTable schema = DatabaseHelper.ExecuteQuery("PRAGMA table_info(Items)");
                var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (schema != null)
                    foreach (DataRow r in schema.Rows)
                        cols.Add(r["name"].ToString());

                bool added = false;
                if (!cols.Contains(COL_AGRI))
                {
                    DatabaseHelper.ExecuteNonQuery($"ALTER TABLE Items ADD COLUMN {COL_AGRI} TEXT DEFAULT 'غير مربوط'");
                    added = true;
                }
                if (!cols.Contains(COL_PAY))
                {
                    DatabaseHelper.ExecuteNonQuery($"ALTER TABLE Items ADD COLUMN {COL_PAY} TEXT DEFAULT 'غير مربوط'");
                    added = true;
                }

                if (added)
                {
                    DatabaseHelper.ExecuteNonQuery($"UPDATE Items SET {COL_AGRI} = 'غير مربوط' WHERE {COL_AGRI} IS NULL OR {COL_AGRI} = ''");
                    DatabaseHelper.ExecuteNonQuery($"UPDATE Items SET {COL_PAY} = 'غير مربوط' WHERE {COL_PAY} IS NULL OR {COL_PAY} = ''");
                    MessageBox.Show("✅ تم إنشاء أعمدة الربط النصية الجديدة في جدول Items", "تحديث قاعدة البيانات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحديث جدول Items:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== ✅ تحميل مجموعات المنافذ إلى القوائم المنسدلة ====================
        private void LoadPortGroupsToDropdowns()
        {
            try
            {
                if (cmbAgriculturePermit == null || cmbPaymentRelease == null) return;

                _suppressLinkSave = true;
                try
                {
                    cmbAgriculturePermit.Items.Clear();
                    cmbPaymentRelease.Items.Clear();

                    cmbAgriculturePermit.Items.Add("غير مربوط");
                    cmbAgriculturePermit.Items.Add("الكل");

                    cmbPaymentRelease.Items.Add("غير مربوط");
                    cmbPaymentRelease.Items.Add("الكل");

                    DataTable dt = DatabaseHelper.ExecuteQuery("SELECT GroupName FROM PortGroups WHERE IsActive = 1 ORDER BY GroupName");

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            string groupName = row["GroupName"].ToString();
                            cmbAgriculturePermit.Items.Add(groupName);
                            cmbPaymentRelease.Items.Add(groupName);
                        }
                    }

                    cmbAgriculturePermit.SelectedIndex = 0;
                    cmbPaymentRelease.SelectedIndex = 0;
                }
                finally
                {
                    _suppressLinkSave = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل مجموعات المنافذ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== ✅ دالة تطبيع قيم مجموعات المنافذ ====================
        private string NormalizePortGroupValue(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
                return "غير مربوط";

            string v = row[columnName].ToString().Trim();

            if (string.IsNullOrEmpty(v) || v == "0" || v.Equals("False", StringComparison.OrdinalIgnoreCase))
                return "غير مربوط";
            if (v == "1" || v.Equals("True", StringComparison.OrdinalIgnoreCase))
                return "الكل";

            return v;
        }

        // ==================== ✅ إعداد الترقيم التلقائي ====================
        private void SetupAutoNumbering()
        {
            if (dgvUnits?.Columns["colNum"] != null) dgvUnits.Columns["colNum"].ReadOnly = true;
            if (dgvWeightUnits?.Columns["colWeightNum"] != null) dgvWeightUnits.Columns["colWeightNum"].ReadOnly = true;

            dgvUnits.CurrentCellDirtyStateChanged += (s, e) => { if (dgvUnits.IsCurrentCellDirty) dgvUnits.CommitEdit(DataGridViewDataErrorContexts.Commit); };
            dgvUnits.CellValueChanged += (s, e) => UpdateRowNumbers(dgvUnits, "colNum");
            dgvUnits.CellEndEdit += (s, e) => UpdateRowNumbers(dgvUnits, "colNum");
            dgvUnits.UserAddedRow += (s, e) => UpdateRowNumbers(dgvUnits, "colNum");
            dgvUnits.UserDeletedRow += (s, e) => UpdateRowNumbers(dgvUnits, "colNum");

            dgvWeightUnits.CurrentCellDirtyStateChanged += (s, e) => { if (dgvWeightUnits.IsCurrentCellDirty) dgvWeightUnits.CommitEdit(DataGridViewDataErrorContexts.Commit); };
            dgvWeightUnits.CellValueChanged += (s, e) => UpdateRowNumbers(dgvWeightUnits, "colWeightNum");
            dgvWeightUnits.CellEndEdit += (s, e) => UpdateRowNumbers(dgvWeightUnits, "colWeightNum");
            dgvWeightUnits.UserAddedRow += (s, e) => UpdateRowNumbers(dgvWeightUnits, "colWeightNum");
            dgvWeightUnits.UserDeletedRow += (s, e) => UpdateRowNumbers(dgvWeightUnits, "colWeightNum");
        }

        private bool _isRenumbering = false;
        private void UpdateRowNumbers(DataGridView grid, string columnName)
        {
            if (_isRenumbering || grid == null || grid.Columns[columnName] == null) return;
            _isRenumbering = true;
            try
            {
                int num = 1;
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if (row.IsNewRow) continue;
                    int current = 0;
                    int.TryParse(row.Cells[columnName].Value?.ToString(), out current);
                    if (current != num) row.Cells[columnName].Value = num;
                    num++;
                }
            }
            finally { _isRenumbering = false; }
        }

        private void AddEditButtonToToolbar()
        {
            ToolStrip ts = this.Controls.Find("toolStrip1", true).FirstOrDefault() as ToolStrip;
            if (ts != null)
            {
                int insertIndex = -1;
                for (int i = 0; i < ts.Items.Count; i++)
                {
                    if (ts.Items[i].Text.Contains("جديد"))
                    {
                        insertIndex = i + 1;
                        break;
                    }
                }
                if (insertIndex > 0)
                {
                    ToolStripButton btnEdit = new ToolStripButton("✏️ تعديل");
                    btnEdit.Name = "btnEditStrip";
                    btnEdit.DisplayStyle = ToolStripItemDisplayStyle.Text;
                    btnEdit.ForeColor = Color.FromArgb(155, 89, 182);
                    btnEdit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    btnEdit.ToolTipText = "تعديل الصنف الحالي";
                    btnEdit.Click += btnEdit_Click;
                    ts.Items.Insert(insertIndex, btnEdit);
                }
            }
        }

        // ==================== ✅ تحميل البيانات من قاعدة البيانات ====================
        private void LoadDataFromDatabase()
        {
            items.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Items ORDER BY ItemCode");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var item = new ItemData
                        {
                            ItemCode = row["ItemCode"].ToString(),
                            GroupNo = row["GroupNo"].ToString(),
                            GroupName = row["GroupName"].ToString(),
                            ItemName = row["ItemName"].ToString(),
                            ItemType = row["ItemType"].ToString(),
                            SubGroupUnder = row["SubGroupUnder"].ToString(),
                            Branch = row["Branch"].ToString(),
                            RefNo = row["RefNo"].ToString(),
                            Barcode = row["Barcode"].ToString(),
                            ForeignName = row["ForeignName"].ToString(),
                            ItemSpecs = row["ItemSpecs"].ToString(),
                            InitialCost = Convert.ToDecimal(row["InitialCost"]),
                            DefaultUnit = row["DefaultUnit"].ToString(),
                            ItemActivity = row["ItemActivity"].ToString(),
                            OpenCost = Convert.ToDecimal(row["OpenCost"]),
                            AvailableQty = Convert.ToDecimal(row["AvailableQty"]),
                            ItemImage = row["ItemImage"] != DBNull.Value ? (byte[])row["ItemImage"] : null,
                            Units = string.IsNullOrEmpty(row["UnitsJson"]?.ToString()) ? new List<ItemUnit>() : JsonSerializer.Deserialize<List<ItemUnit>>(row["UnitsJson"].ToString()),
                            WeightUnits = string.IsNullOrEmpty(row["WeightUnitsJson"]?.ToString()) ? new List<ItemWeightUnit>() : JsonSerializer.Deserialize<List<ItemWeightUnit>>(row["WeightUnitsJson"].ToString()),
                            Costs = string.IsNullOrEmpty(row["CostsJson"]?.ToString()) ? new List<ItemCost>() : JsonSerializer.Deserialize<List<ItemCost>>(row["CostsJson"].ToString()),
                            CountryOfOrigin = row["CountryOfOrigin"] != DBNull.Value ? row["CountryOfOrigin"].ToString() : "",
                            // ✅ القراءة من العمودين الجديدين (النصيين)
                            AgriculturePermitPortGroup = row.Table.Columns.Contains(COL_AGRI)
                                ? NormalizePortGroupValue(row, COL_AGRI)
                                : NormalizePortGroupValue(row, "AgriculturePermit"),
                            PaymentReleasePortGroup = row.Table.Columns.Contains(COL_PAY)
                                ? NormalizePortGroupValue(row, COL_PAY)
                                : NormalizePortGroupValue(row, "PaymentRelease")
                        };
                        items.Add(item);
                    }
                    Console.WriteLine($"✅ تم تحميل {items.Count} صنف من قاعدة البيانات");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحميل الأصناف:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadGroupsFromDatabase()
        {
            try
            {
                groups.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT GroupNo, GroupName, ForeignName, Description, IsActive FROM Groups WHERE IsActive = 1 ORDER BY GroupNo");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        groups.Add(new GroupData
                        {
                            GroupNo = row["GroupNo"].ToString(),
                            GroupName = row["GroupName"].ToString(),
                            ForeignName = row["ForeignName"]?.ToString() ?? "",
                            Description = row["Description"]?.ToString() ?? "",
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }
                }
                RefreshGroupComboBox();
                Console.WriteLine($"✅ تم تحميل {groups.Count} مجموعة من قاعدة البيانات");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ تحميل المجموعات:\n{ex.Message}", "خطأ");
            }
        }

        // ==================== ✅ حفظ البيانات إلى قاعدة البيانات ====================
        private void SaveDataToDatabase(ItemData item)
        {
            try
            {
                var check = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Items WHERE ItemCode = @ItemCode",
                    DatabaseHelper.CreateParameter("@ItemCode", item.ItemCode)
                );
                bool exists = Convert.ToInt32(check) > 0;

                string unitsJson = item.Units.Count > 0 ? JsonSerializer.Serialize(item.Units) : null;
                string weightJson = item.WeightUnits.Count > 0 ? JsonSerializer.Serialize(item.WeightUnits) : null;
                string costsJson = item.Costs.Count > 0 ? JsonSerializer.Serialize(item.Costs) : null;

                var imgParam = DatabaseHelper.CreateParameter("@ItemImage", item.ItemImage ?? (object)DBNull.Value);

                if (exists)
                {
                    // ✅ UPDATE مع إضافة العمودين الجديدين
                    string query = @"UPDATE Items SET
                        GroupNo = @GroupNo, GroupName = @GroupName, ItemName = @ItemName, ItemType = @ItemType,
                        SubGroupUnder = @SubGroupUnder, Branch = @Branch, RefNo = @RefNo, Barcode = @Barcode,
                        ForeignName = @ForeignName, ItemSpecs = @ItemSpecs, InitialCost = @InitialCost,
                        DefaultUnit = @DefaultUnit, ItemActivity = @ItemActivity, OpenCost = @OpenCost,
                        AvailableQty = @AvailableQty, UnitsJson = @UnitsJson, WeightUnitsJson = @WeightUnitsJson,
                        CostsJson = @CostsJson, CountryOfOrigin = @CountryOfOrigin,
                        AgriculturePermit = @AgriculturePermit, PaymentRelease = @PaymentRelease,
                        AgriLinkGroup = @AgriLink, PayLinkGroup = @PayLink,
                        ItemImage = @ItemImage, ModifiedDate = datetime('now')
                        WHERE ItemCode = @ItemCode";

                    DatabaseHelper.ExecuteNonQuery(query,
                        DatabaseHelper.CreateParameter("@GroupNo", item.GroupNo),
                        DatabaseHelper.CreateParameter("@GroupName", item.GroupName),
                        DatabaseHelper.CreateParameter("@ItemName", item.ItemName),
                        DatabaseHelper.CreateParameter("@ItemType", item.ItemType),
                        DatabaseHelper.CreateParameter("@SubGroupUnder", item.SubGroupUnder),
                        DatabaseHelper.CreateParameter("@Branch", item.Branch),
                        DatabaseHelper.CreateParameter("@RefNo", item.RefNo),
                        DatabaseHelper.CreateParameter("@Barcode", item.Barcode),
                        DatabaseHelper.CreateParameter("@ForeignName", item.ForeignName),
                        DatabaseHelper.CreateParameter("@ItemSpecs", item.ItemSpecs),
                        DatabaseHelper.CreateParameter("@InitialCost", item.InitialCost),
                        DatabaseHelper.CreateParameter("@DefaultUnit", item.DefaultUnit),
                        DatabaseHelper.CreateParameter("@ItemActivity", item.ItemActivity),
                        DatabaseHelper.CreateParameter("@OpenCost", item.OpenCost),
                        DatabaseHelper.CreateParameter("@AvailableQty", item.AvailableQty),
                        DatabaseHelper.CreateParameter("@UnitsJson", (object)unitsJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@WeightUnitsJson", (object)weightJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@CostsJson", (object)costsJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@CountryOfOrigin", item.CountryOfOrigin ?? ""),
                        DatabaseHelper.CreateParameter("@AgriculturePermit", item.AgriculturePermitPortGroup ?? "غير مربوط"),
                        DatabaseHelper.CreateParameter("@PaymentRelease", item.PaymentReleasePortGroup ?? "غير مربوط"),
                        DatabaseHelper.CreateParameter("@AgriLink", item.AgriculturePermitPortGroup ?? "غير مربوط"),
                        DatabaseHelper.CreateParameter("@PayLink", item.PaymentReleasePortGroup ?? "غير مربوط"),
                        imgParam,
                        DatabaseHelper.CreateParameter("@ItemCode", item.ItemCode)
                    );
                }
                else
                {
                    // ✅ INSERT مع إضافة العمودين الجديدين
                    string query = @"INSERT INTO Items (
                        ItemCode, GroupNo, GroupName, ItemName, ItemType, SubGroupUnder, Branch, RefNo, Barcode,
                        ForeignName, ItemSpecs, InitialCost, DefaultUnit, ItemActivity, OpenCost, AvailableQty,
                        UnitsJson, WeightUnitsJson, CostsJson, CountryOfOrigin, 
                        AgriculturePermit, PaymentRelease, AgriLinkGroup, PayLinkGroup,
                        ItemImage, ModifiedDate) 
                        VALUES (
                        @ItemCode, @GroupNo, @GroupName, @ItemName, @ItemType, @SubGroupUnder, @Branch, @RefNo, @Barcode,
                        @ForeignName, @ItemSpecs, @InitialCost, @DefaultUnit, @ItemActivity, @OpenCost, @AvailableQty,
                        @UnitsJson, @WeightUnitsJson, @CostsJson, @CountryOfOrigin, 
                        @AgriculturePermit, @PaymentRelease, @AgriLink, @PayLink,
                        @ItemImage, datetime('now'))";

                    DatabaseHelper.ExecuteNonQuery(query,
                        DatabaseHelper.CreateParameter("@ItemCode", item.ItemCode),
                        DatabaseHelper.CreateParameter("@GroupNo", item.GroupNo),
                        DatabaseHelper.CreateParameter("@GroupName", item.GroupName),
                        DatabaseHelper.CreateParameter("@ItemName", item.ItemName),
                        DatabaseHelper.CreateParameter("@ItemType", item.ItemType),
                        DatabaseHelper.CreateParameter("@SubGroupUnder", item.SubGroupUnder),
                        DatabaseHelper.CreateParameter("@Branch", item.Branch),
                        DatabaseHelper.CreateParameter("@RefNo", item.RefNo),
                        DatabaseHelper.CreateParameter("@Barcode", item.Barcode),
                        DatabaseHelper.CreateParameter("@ForeignName", item.ForeignName),
                        DatabaseHelper.CreateParameter("@ItemSpecs", item.ItemSpecs),
                        DatabaseHelper.CreateParameter("@InitialCost", item.InitialCost),
                        DatabaseHelper.CreateParameter("@DefaultUnit", item.DefaultUnit),
                        DatabaseHelper.CreateParameter("@ItemActivity", item.ItemActivity),
                        DatabaseHelper.CreateParameter("@OpenCost", item.OpenCost),
                        DatabaseHelper.CreateParameter("@AvailableQty", item.AvailableQty),
                        DatabaseHelper.CreateParameter("@UnitsJson", (object)unitsJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@WeightUnitsJson", (object)weightJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@CostsJson", (object)costsJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@CountryOfOrigin", item.CountryOfOrigin ?? ""),
                        DatabaseHelper.CreateParameter("@AgriculturePermit", item.AgriculturePermitPortGroup ?? "غير مربوط"),
                        DatabaseHelper.CreateParameter("@PaymentRelease", item.PaymentReleasePortGroup ?? "غير مربوط"),
                        DatabaseHelper.CreateParameter("@AgriLink", item.AgriculturePermitPortGroup ?? "غير مربوط"),
                        DatabaseHelper.CreateParameter("@PayLink", item.PaymentReleasePortGroup ?? "غير مربوط"),
                        imgParam
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ الصنف:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void SetControlsState(bool enabled)
        {
            isEditingMode = enabled;
            txtGroupNo.ReadOnly = !enabled;
            txtGroupName.ReadOnly = !enabled;
            txtItemCode.ReadOnly = !enabled;
            txtItemName.ReadOnly = !enabled;
            txtSubGroupUnder.ReadOnly = !enabled;
            txtBranch.ReadOnly = !enabled;
            txtRefNo.ReadOnly = !enabled;
            txtBarcode.ReadOnly = !enabled;
            txtForeignName.ReadOnly = !enabled;
            txtItemSpecs.ReadOnly = !enabled;
            txtInitialCost.ReadOnly = !enabled;
            txtOpenCost.ReadOnly = !enabled;
            txtAvailableQty.ReadOnly = !enabled;

            txtCountryOfOrigin.ReadOnly = !enabled;

            if (cmbAgriculturePermit != null) cmbAgriculturePermit.Enabled = enabled;
            if (cmbPaymentRelease != null) cmbPaymentRelease.Enabled = enabled;

            cmbItemType.Enabled = enabled;
            cmbDefaultUnit.Enabled = enabled;
            cmbItemActivity.Enabled = enabled;
            cmbGroupSelect.Enabled = enabled;
            btnOpenGroups.Enabled = enabled;
            btnImgAdd.Enabled = enabled;
            btnImgDel.Enabled = enabled;

            if (dgvUnits != null) dgvUnits.ReadOnly = !enabled;
            if (dgvCost != null) dgvCost.ReadOnly = !enabled;
            if (dgvWeightUnits != null) dgvWeightUnits.ReadOnly = !enabled;

            Color bgColor = enabled ? Color.FromArgb(255, 255, 224) : Color.White;
            txtGroupNo.BackColor = bgColor;
            txtGroupName.BackColor = bgColor;
            txtItemCode.BackColor = bgColor;
            txtItemName.BackColor = bgColor;
            txtSubGroupUnder.BackColor = bgColor;
            txtBranch.BackColor = bgColor;
            txtRefNo.BackColor = bgColor;
            txtBarcode.BackColor = bgColor;
            txtForeignName.BackColor = bgColor;
            txtItemSpecs.BackColor = bgColor;
            txtInitialCost.BackColor = bgColor;
            txtOpenCost.BackColor = bgColor;
            txtAvailableQty.BackColor = bgColor;
            txtCountryOfOrigin.BackColor = bgColor;

            UpdateNavigation();
        }

        private void TrackChanges()
        {
            if (isEditingMode)
            {
                hasChanges = true;
            }
        }

        // ==================== ✅ حدث تغيير القوائم ====================
        private void CmbPermitLink_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressLinkSave) return;
            TrackChanges();
            PersistLinkFieldsImmediately();
        }

        // ==================== ✅ حفظ فوري للربط في قاعدة البيانات ====================
        private void PersistLinkFieldsImmediately()
        {
            try
            {
                string itemCode = txtItemCode.Text.Trim();
                if (string.IsNullOrEmpty(itemCode)) return;

                string agri = cmbAgriculturePermit?.SelectedItem?.ToString() ?? "غير مربوط";
                string pay = cmbPaymentRelease?.SelectedItem?.ToString() ?? "غير مربوط";

                // ✅ الكتابة في العمودين النصيين الجديدين
                int affected = DatabaseHelper.ExecuteNonQuery(
                    $@"UPDATE Items SET {COL_AGRI} = @Agri, {COL_PAY} = @Pay, 
                      AgriculturePermit = @Agri, PaymentRelease = @Pay,
                      ModifiedDate = datetime('now') WHERE ItemCode = @ItemCode",
                    DatabaseHelper.CreateParameter("@Agri", agri),
                    DatabaseHelper.CreateParameter("@Pay", pay),
                    DatabaseHelper.CreateParameter("@ItemCode", itemCode));

                if (affected > 0)
                {
                    var mem = items.FirstOrDefault(i => i.ItemCode == itemCode);
                    if (mem != null)
                    {
                        mem.AgriculturePermitPortGroup = agri;
                        mem.PaymentReleasePortGroup = pay;
                    }
                    Console.WriteLine($"✅ حفظ الربط للصنف {itemCode}: زراعية={agri} | مدفوعات={pay}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ في حفظ الربط: {ex.Message}");
            }
        }

        // ==================== ✅ تعيين القيم برمجياً ====================
        private void SetLinkCombos(string agri, string pay)
        {
            _suppressLinkSave = true;
            try
            {
                if (cmbAgriculturePermit != null)
                {
                    string targetAgri = agri ?? "غير مربوط";
                    int index = -1;
                    for (int i = 0; i < cmbAgriculturePermit.Items.Count; i++)
                    {
                        if (cmbAgriculturePermit.Items[i].ToString() == targetAgri)
                        {
                            index = i;
                            break;
                        }
                    }
                    cmbAgriculturePermit.SelectedIndex = index >= 0 ? index : 0;
                }

                if (cmbPaymentRelease != null)
                {
                    string targetPay = pay ?? "غير مربوط";
                    int index = -1;
                    for (int i = 0; i < cmbPaymentRelease.Items.Count; i++)
                    {
                        if (cmbPaymentRelease.Items[i].ToString() == targetPay)
                        {
                            index = i;
                            break;
                        }
                    }
                    cmbPaymentRelease.SelectedIndex = index >= 0 ? index : 0;
                }
            }
            finally
            {
                _suppressLinkSave = false;
            }
        }

        private void SaveOriginalItem()
        {
            if (currentIndex >= 0 && currentIndex < items.Count)
            {
                string json = JsonSerializer.Serialize(items[currentIndex]);
                originalItem = JsonSerializer.Deserialize<ItemData>(json);
            }
            else
            {
                originalItem = null;
            }
            hasChanges = false;
        }

        private bool CheckUnsavedChanges()
        {
            if (isEditingMode)
            {
                DialogResult result = MessageBox.Show(
                    "⚠️ لديك تغييرات غير محفوظة!\n" +
                    "• اضغط 'نعم' لحفظ التغييرات\n" +
                    "• اضغط 'لا' للتجاهل والمتابعة\n" +
                    "• اضغط 'إلغاء' للبقاء والتعديل",
                    "تأكيد الخروج", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    if (!ValidateRequiredFields()) return false;
                    try
                    {
                        var existing = items.FirstOrDefault(i => i.ItemCode == txtItemCode.Text);
                        ItemData itemToSave;
                        if (existing != null)
                        {
                            UpdateItemFromForm(existing);
                            itemToSave = existing;
                            MessageBox.Show(LanguageManager.Get("msg_update_success"), LanguageManager.Get("success_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            var newItem = new ItemData();
                            UpdateItemFromForm(newItem);
                            items.Add(newItem);
                            currentIndex = items.Count - 1;
                            itemToSave = newItem;
                            MessageBox.Show(LanguageManager.Get("msg_add_success"), LanguageManager.Get("success_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        SaveDataToDatabase(itemToSave);
                        PersistLinkFieldsImmediately(); // ✅ إضافة
                        hasChanges = false;
                        return true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ خطأ في الحفظ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
            }
            return true;
        }

        public class WeightUnitOption
        {
            public string UnitName { get; set; }
            public decimal Weight { get; set; }
        }

        private void LoadWeightUnitOptions()
        {
            try
            {
                weightUnitOptions.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT UnitName, Weight FROM WeightUnits ORDER BY UnitName");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        weightUnitOptions.Add(new WeightUnitOption
                        {
                            UnitName = row["UnitName"].ToString(),
                            Weight = Convert.ToDecimal(row["Weight"])
                        });
                    }
                }
                else
                {
                    weightUnitOptions = new List<WeightUnitOption>
                    {
                        new WeightUnitOption { UnitName = "كرتون", Weight = 15.00m },
                        new WeightUnitOption { UnitName = "قطعة", Weight = 0.50m },
                        new WeightUnitOption { UnitName = "كجم", Weight = 1.00m },
                        new WeightUnitOption { UnitName = "لتر", Weight = 1.00m },
                        new WeightUnitOption { UnitName = "علبة", Weight = 0.25m }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ تحميل خيارات وحدات الوزن: {ex.Message}");
            }
        }

        private void OpenUnitsSelectionFormForItemUnits(int rowIndex)
        {
            Form selectForm = new Form();
            selectForm.Text = "🔍 اختر وحدة القياس (انقر مرتين أو اضغط Enter)";
            selectForm.Size = new Size(600, 400);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            DataGridView dgvSelect = new DataGridView();
            dgvSelect.Dock = DockStyle.Fill;
            dgvSelect.ReadOnly = true;
            dgvSelect.AllowUserToAddRows = false;
            dgvSelect.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSelect.MultiSelect = false;
            dgvSelect.BackgroundColor = Color.White;
            dgvSelect.RowHeadersVisible = false;
            dgvSelect.EnableHeadersVisualStyles = false;
            dgvSelect.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvSelect.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSelect.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvSelect.ColumnHeadersHeight = 40;
            dgvSelect.RowTemplate.Height = 35;
            dgvSelect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvSelect.Columns.Add("colCode", "رمز الوحدة");
            dgvSelect.Columns.Add("colName", "اسم الوحدة");
            dgvSelect.Columns.Add("colStatus", "الحالة");

            foreach (var unit in measurementUnits.Where(u => u.IsActive))
            {
                dgvSelect.Rows.Add(unit.UnitCode, unit.UnitName, "✅ نشط");
            }

            dgvSelect.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string selectedCode = dgvSelect.Rows[ev.RowIndex].Cells["colCode"].Value?.ToString();
                    var selectedUnit = measurementUnits.FirstOrDefault(u => u.UnitCode == selectedCode);
                    if (selectedUnit != null)
                    {
                        int target = rowIndex;
                        if (target >= dgvUnits.Rows.Count || dgvUnits.Rows[target].IsNewRow)
                            target = dgvUnits.Rows.Add();

                        dgvUnits.Rows[target].Cells["colUnit"].Value = selectedUnit.UnitName;
                        dgvUnits.Rows[target].Cells["colPackage"].Value = "1";
                        dgvUnits.CurrentCell = dgvUnits.Rows[target].Cells["colPackage"];
                        dgvUnits.BeginEdit(true);
                        UpdateRowNumbers(dgvUnits, "colNum");
                        TrackChanges();
                        selectForm.Close();
                    }
                }
            };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 50;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnClose = new Button();
            btnClose.Text = "❌ إغلاق";
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.BackColor = Color.FromArgb(158, 158, 158);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Size = new Size(120, 35);
            btnClose.Location = new Point(240, 8);
            btnClose.Click += (s, ev) => selectForm.Close();

            pnlButtons.Controls.Add(btnClose);
            selectForm.Controls.Add(dgvSelect);
            selectForm.Controls.Add(pnlButtons);
            selectForm.AcceptButton = btnClose;
            selectForm.ShowDialog();
        }

        private void OpenWeightUnitSelectionForm(int rowIndex)
        {
            Form selectForm = new Form();
            selectForm.Text = "⚖️ اختر وحدة الوزن (انقر مرتين أو اضغط Enter)";
            selectForm.Size = new Size(600, 400);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            DataGridView dgvSelect = new DataGridView();
            dgvSelect.Dock = DockStyle.Fill;
            dgvSelect.ReadOnly = true;
            dgvSelect.AllowUserToAddRows = false;
            dgvSelect.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSelect.MultiSelect = false;
            dgvSelect.BackgroundColor = Color.White;
            dgvSelect.RowHeadersVisible = false;
            dgvSelect.EnableHeadersVisualStyles = false;
            dgvSelect.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvSelect.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSelect.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvSelect.ColumnHeadersHeight = 40;
            dgvSelect.RowTemplate.Height = 35;
            dgvSelect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvSelect.Columns.Add("colName", "اسم الوحدة");
            dgvSelect.Columns.Add("colWeight", "الوزن");

            foreach (var unit in weightUnitOptions)
            {
                dgvSelect.Rows.Add(unit.UnitName, unit.Weight.ToString("N2"));
            }

            dgvSelect.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string selectedName = dgvSelect.Rows[ev.RowIndex].Cells["colName"].Value?.ToString();
                    var selectedUnit = weightUnitOptions.FirstOrDefault(u => u.UnitName == selectedName);
                    if (selectedUnit != null)
                    {
                        int target = rowIndex;
                        if (target >= dgvWeightUnits.Rows.Count || dgvWeightUnits.Rows[target].IsNewRow)
                            target = dgvWeightUnits.Rows.Add();

                        dgvWeightUnits.Rows[target].Cells["colWeightUnit"].Value = selectedUnit.UnitName;
                        dgvWeightUnits.Rows[target].Cells["colWeightValue"].Value = selectedUnit.Weight;
                        UpdateRowNumbers(dgvWeightUnits, "colWeightNum");
                        TrackChanges();
                        selectForm.Close();
                    }
                }
            };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 50;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnClose = new Button();
            btnClose.Text = "❌ إغلاق";
            btnClose.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnClose.BackColor = Color.FromArgb(158, 158, 158);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Size = new Size(120, 35);
            btnClose.Location = new Point(240, 8);
            btnClose.Click += (s, ev) => selectForm.Close();

            pnlButtons.Controls.Add(btnClose);
            selectForm.Controls.Add(dgvSelect);
            selectForm.Controls.Add(pnlButtons);
            selectForm.AcceptButton = btnClose;
            selectForm.ShowDialog();
        }

        private void DgvWeightUnits_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dgvWeightUnits.Columns[e.ColumnIndex].Name;
                if (columnName == "colWeightUnit")
                {
                    OpenWeightUnitSelectionForm(e.RowIndex);
                }
            }
        }

        private void DgvUnits_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dgvUnits.Columns[e.ColumnIndex].Name;
                if (columnName == "colUnit")
                {
                    OpenUnitsSelectionFormForItemUnits(e.RowIndex);
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F9)
            {
                if (txtItemName != null && txtItemName.Focused)
                {
                    OpenItemSelectionForm();
                    return true;
                }
                if (dgvUnits != null && dgvUnits.Focused && dgvUnits.CurrentCell != null)
                {
                    string columnName = dgvUnits.Columns[dgvUnits.CurrentCell.ColumnIndex].Name;
                    if (columnName == "colUnit")
                    {
                        OpenUnitsSelectionFormForItemUnits(dgvUnits.CurrentCell.RowIndex);
                        return true;
                    }
                }
                if (dgvWeightUnits != null && dgvWeightUnits.Focused && dgvWeightUnits.CurrentCell != null)
                {
                    string columnName = dgvWeightUnits.Columns[dgvWeightUnits.CurrentCell.ColumnIndex].Name;
                    if (columnName == "colWeightUnit")
                    {
                        OpenWeightUnitSelectionForm(dgvWeightUnits.CurrentCell.RowIndex);
                        return true;
                    }
                }
            }
            else if (keyData == (Keys.Control | Keys.S))
            {
                if (isEditingMode)
                {
                    btnSave_Click(null, EventArgs.Empty);
                    return true;
                }
            }
            else if (keyData == Keys.Escape)
            {
                if (isEditingMode)
                {
                    CancelEditMode();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OpenItemSelectionForm()
        {
            if (items == null || items.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد أصناف مضافة مسبقاً\nالرجاء إضافة أصناف في شاشة تهيئة الأصناف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form itemForm = new Form();
            itemForm.Text = $"🔍 اختر الصنف ({items.Count} صنف)";
            itemForm.Size = new Size(800, 500);
            itemForm.StartPosition = FormStartPosition.CenterParent;
            itemForm.RightToLeft = LanguageManager.RightToLeft;
            itemForm.RightToLeftLayout = LanguageManager.IsRTL;
            itemForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            itemForm.MaximizeBox = false;
            itemForm.MinimizeBox = false;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 50;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);

            Label lblSearch = new Label();
            lblSearch.Text = " بحث:";
            lblSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSearch.Location = new Point(650, 15);
            lblSearch.Size = new Size(60, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            TextBox txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(400, 15);
            txtSearch.Size = new Size(240, 25);

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);

            DataGridView dgvItems = new DataGridView();
            dgvItems.Dock = DockStyle.Fill;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvItems.MultiSelect = false;
            dgvItems.ReadOnly = true;
            dgvItems.AllowUserToAddRows = false;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.RowHeadersVisible = false;
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvItems.ColumnHeadersHeight = 40;
            dgvItems.RowTemplate.Height = 35;

            dgvItems.Columns.Add("colItemCode", "رمز الصنف");
            dgvItems.Columns.Add("colItemName", "اسم الصنف");
            dgvItems.Columns.Add("colGroupName", "المجموعة");
            dgvItems.Columns.Add("colDefaultUnit", "الوحدة");
            dgvItems.Columns.Add("colCountryOfOrigin", "بلد المنشأ");
            dgvItems.Columns.Add("colActivity", "النشاط");

            foreach (var item in items)
            {
                string unitName = item.DefaultUnit ?? "";
                if (string.IsNullOrEmpty(unitName))
                {
                    var unit = measurementUnits.FirstOrDefault(u => u.UnitCode == item.DefaultUnit);
                    unitName = unit?.UnitName ?? "";
                }
                dgvItems.Rows.Add(item.ItemCode, item.ItemName, item.GroupName, unitName, item.CountryOfOrigin ?? "", item.ItemActivity);
            }

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnOK = new Button();
            btnOK.Text = "✅ اختيار";
            btnOK.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnOK.BackColor = Color.FromArgb(76, 175, 80);
            btnOK.ForeColor = Color.White;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Size = new Size(150, 45);
            btnOK.Location = new Point(550, 8);
            btnOK.DialogResult = DialogResult.OK;

            Button btnCancel = new Button();
            btnCancel.Text = " إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(80, 8);
            btnCancel.DialogResult = DialogResult.Cancel;

            pnlButtons.Controls.Add(btnOK);
            pnlButtons.Controls.Add(btnCancel);

            itemForm.Controls.Add(dgvItems);
            itemForm.Controls.Add(pnlSearch);
            itemForm.Controls.Add(pnlButtons);
            itemForm.AcceptButton = btnOK;
            itemForm.CancelButton = btnCancel;

            txtSearch.TextChanged += (s, ev) =>
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    string itemCode = row.Cells["colItemCode"].Value?.ToString()?.ToLower() ?? "";
                    string itemName = row.Cells["colItemName"].Value?.ToString()?.ToLower() ?? "";
                    if (string.IsNullOrEmpty(searchText) || itemCode.Contains(searchText) || itemName.Contains(searchText))
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            };

            dgvItems.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) btnOK.PerformClick(); };

            if (itemForm.ShowDialog() == DialogResult.OK && dgvItems.SelectedRows.Count > 0)
            {
                string selectedItemCode = dgvItems.SelectedRows[0].Cells["colItemCode"].Value?.ToString();
                var selectedItem = items.FirstOrDefault(i => i.ItemCode == selectedItemCode);
                if (selectedItem != null)
                {
                    txtGroupNo.Text = selectedItem.GroupNo;
                    txtGroupName.Text = selectedItem.GroupName;
                    txtItemCode.Text = selectedItem.ItemCode;
                    txtItemName.Text = selectedItem.ItemName;
                    cmbItemType.SelectedItem = selectedItem.ItemType;
                    txtSubGroupUnder.Text = selectedItem.SubGroupUnder;
                    txtBranch.Text = selectedItem.Branch;
                    txtRefNo.Text = selectedItem.RefNo;
                    txtBarcode.Text = selectedItem.Barcode;
                    txtForeignName.Text = selectedItem.ForeignName;
                    txtItemSpecs.Text = selectedItem.ItemSpecs;
                    txtInitialCost.Text = selectedItem.InitialCost.ToString("0.00");
                    cmbItemActivity.SelectedItem = selectedItem.ItemActivity;
                    txtOpenCost.Text = selectedItem.OpenCost.ToString("0.00");
                    txtAvailableQty.Text = selectedItem.AvailableQty.ToString("0.00");

                    txtCountryOfOrigin.Text = selectedItem.CountryOfOrigin ?? "";

                    SetLinkCombos(selectedItem.AgriculturePermitPortGroup, selectedItem.PaymentReleasePortGroup);

                    if (!string.IsNullOrEmpty(selectedItem.DefaultUnit))
                    {
                        var unit = measurementUnits.FirstOrDefault(u => u.UnitCode == selectedItem.DefaultUnit);
                        if (unit != null) cmbDefaultUnit.SelectedItem = unit;
                    }

                    dgvUnits.Rows.Clear();
                    if (selectedItem.Units != null && selectedItem.Units.Count > 0)
                    {
                        int rowIndex = 1;
                        foreach (var unit in selectedItem.Units)
                        {
                            dgvUnits.Rows.Add(rowIndex++, unit.UnitName, unit.Package);
                        }
                    }

                    dgvWeightUnits.Rows.Clear();
                    if (selectedItem.WeightUnits != null && selectedItem.WeightUnits.Count > 0)
                    {
                        int wIndex = 1;
                        foreach (var weightUnit in selectedItem.WeightUnits)
                        {
                            dgvWeightUnits.Rows.Add(wIndex++, weightUnit.UnitName, weightUnit.Weight.ToString("0.00"));
                        }
                    }

                    if (selectedItem.ItemImage != null && selectedItem.ItemImage.Length > 0)
                    {
                        using (var ms = new MemoryStream(selectedItem.ItemImage))
                        {
                            picItem.Image = Image.FromStream(ms);
                            picItem.SizeMode = PictureBoxSizeMode.StretchImage;
                        }
                        currentImagePath = null;
                    }
                    else
                    {
                        picItem.Image = null;
                        currentImagePath = null;
                    }

                    MessageBox.Show($"✅ تم اختيار الصنف: {selectedItem.ItemName}\n اضغط 'تعديل' لفتح الحقول للتعديل", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (items == null || items.Count == 0)
            {
                MessageBox.Show("️ لا توجد أصناف مضافة لعرضها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form viewForm = new Form();
            viewForm.Text = $" عرض جميع الأصناف ({items.Count} صنف)";
            viewForm.Size = new Size(1000, 600);
            viewForm.StartPosition = FormStartPosition.CenterParent;
            viewForm.RightToLeft = LanguageManager.RightToLeft;
            viewForm.RightToLeftLayout = LanguageManager.IsRTL;
            viewForm.WindowState = FormWindowState.Maximized;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 60;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);

            Label lblSearch = new Label();
            lblSearch.Text = "عرض القائمه";
            lblSearch.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblSearch.Location = new Point(850, 18);
            lblSearch.Size = new Size(70, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            TextBox txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.Location = new Point(500, 18);
            txtSearch.Size = new Size(340, 28);

            Label lblCount = new Label();
            lblCount.Text = $" إجمالي الأصناف: {items.Count}";
            lblCount.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblCount.ForeColor = Color.FromArgb(33, 150, 243);
            lblCount.Location = new Point(20, 18);
            lblCount.Size = new Size(400, 25);

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblCount });

            DataGridView dgvView = new DataGridView();
            dgvView.Dock = DockStyle.Fill;
            dgvView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvView.MultiSelect = false;
            dgvView.ReadOnly = true;
            dgvView.AllowUserToAddRows = false;
            dgvView.BackgroundColor = Color.White;
            dgvView.RowHeadersVisible = false;
            dgvView.EnableHeadersVisualStyles = false;
            dgvView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvView.ColumnHeadersHeight = 45;
            dgvView.RowTemplate.Height = 38;
            dgvView.DefaultCellStyle.Font = new Font("Segoe UI", 10);

            dgvView.Columns.Add("colNum", "م");
            dgvView.Columns.Add("colItemCode", "رمز الصنف");
            dgvView.Columns.Add("colItemName", "اسم الصنف");
            dgvView.Columns.Add("colGroupName", "المجموعة");
            dgvView.Columns.Add("colItemType", "النوع");
            dgvView.Columns.Add("colDefaultUnit", "الوحدة");
            dgvView.Columns.Add("colInitialCost", "التكلفة");
            dgvView.Columns.Add("colCountryOfOrigin", "بلد المنشأ");
            dgvView.Columns.Add("colActivity", "النشاط");

            int idx = 1;
            foreach (var item in items)
            {
                string unitName = item.DefaultUnit ?? "";
                var unit = measurementUnits.FirstOrDefault(u => u.UnitCode == item.DefaultUnit);
                if (unit != null) unitName = unit.UnitName;
                dgvView.Rows.Add(idx++, item.ItemCode, item.ItemName, item.GroupName, item.ItemType ?? "", unitName, item.InitialCost.ToString("N2"), item.CountryOfOrigin ?? "", item.ItemActivity ?? "");
            }

            dgvView.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.RowIndex % 2 == 0)
                    dgvView.Rows[ev.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(248, 249, 250);
            };

            dgvView.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string itemCode = dgvView.Rows[ev.RowIndex].Cells["colItemCode"].Value?.ToString();
                    var selectedItem = items.FirstOrDefault(i => i.ItemCode == itemCode);
                    if (selectedItem != null)
                    {
                        currentIndex = items.IndexOf(selectedItem);
                        LoadItemToForm(selectedItem);
                        viewForm.Close();
                        MessageBox.Show($"✅ تم اختيار الصنف: {selectedItem.ItemName}\n💡 اضغط 'تعديل' لفتح الحقول للتعديل", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            txtSearch.TextChanged += (s, ev) =>
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                int visibleCount = 0;
                foreach (DataGridViewRow row in dgvView.Rows)
                {
                    string itemCode = row.Cells["colItemCode"].Value?.ToString()?.ToLower() ?? "";
                    string itemName = row.Cells["colItemName"].Value?.ToString()?.ToLower() ?? "";
                    string groupName = row.Cells["colGroupName"].Value?.ToString()?.ToLower() ?? "";

                    if (string.IsNullOrEmpty(searchText) || itemCode.Contains(searchText) || itemName.Contains(searchText) || groupName.Contains(searchText))
                    {
                        row.Visible = true;
                        visibleCount++;
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
                lblCount.Text = $"📊 الأصناف المعروضة: {visibleCount} من {items.Count}";
            };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnClose = new Button();
            btnClose.Text = "❌ إغلاق";
            btnClose.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnClose.BackColor = Color.FromArgb(158, 158, 158);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Size = new Size(150, 45);
            btnClose.Location = new Point(420, 8);
            btnClose.Click += (s, ev) => viewForm.Close();

            pnlButtons.Controls.Add(btnClose);

            viewForm.Controls.Add(dgvView);
            viewForm.Controls.Add(pnlSearch);
            viewForm.Controls.Add(pnlButtons);
            viewForm.ShowDialog();
        }

        private void RefreshAllTranslations()
        {
            this.Text = LanguageManager.Get("item_form_title");
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;

            if (dgvWeightUnits != null && dgvWeightUnits.Columns.Count > 0)
            {
                if (dgvWeightUnits.Columns["colWeightUnit"] != null)
                    dgvWeightUnits.Columns["colWeightUnit"].HeaderText = LanguageManager.Get("col_weight_unit");
                if (dgvWeightUnits.Columns["colWeightValue"] != null)
                    dgvWeightUnits.Columns["colWeightValue"].HeaderText = LanguageManager.Get("col_weight_value");
            }
        }

        private void OpenUnitsForm()
        {
            using (var unitsForm = new UnitsForm())
            {
                unitsForm.ShowDialog();
                LoadMeasurementUnits();
            }
        }

        private void OpenWeightUnitsForm()
        {
            using (var weightUnitsForm = new WeightUnitsForm())
            {
                weightUnitsForm.ShowDialog();
                LoadWeightUnitOptions();
            }
        }

        public class ItemData
        {
            public string GroupNo { get; set; }
            public string GroupName { get; set; }
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string ItemType { get; set; }
            public string SubGroupUnder { get; set; }
            public string Branch { get; set; }
            public string RefNo { get; set; }
            public string Barcode { get; set; }
            public string ForeignName { get; set; }
            public string ItemSpecs { get; set; }
            public string ItemSpecsForeign { get; set; }
            public string Assistant { get; set; }
            public string DetailGroup { get; set; }
            public string Class { get; set; }
            public decimal InitialCost { get; set; }
            public string DefaultUnit { get; set; }
            public string PurchaseInvoice { get; set; }
            public string ItemActivity { get; set; }
            public int ReturnPeriod { get; set; }
            public string Rank { get; set; }
            public string Length { get; set; }
            public string Size { get; set; }
            public string Width { get; set; }
            public string Color { get; set; }
            public string Height { get; set; }
            public string Season { get; set; }
            public string Volume { get; set; }
            public string Material { get; set; }
            public string Area { get; set; }
            public string Brand { get; set; }
            public string Manufacturer { get; set; }
            public string MfgNo { get; set; }
            public string CountryMain { get; set; }
            public string Storage { get; set; }
            public decimal OpenCost { get; set; }
            public string StopDate { get; set; }
            public decimal OpeningStock { get; set; }
            public string StopReason { get; set; }
            public decimal AvailableQty { get; set; }
            public byte[] ItemImage { get; set; }
            public List<ItemUnit> Units { get; set; } = new List<ItemUnit>();
            public List<ItemCost> Costs { get; set; } = new List<ItemCost>();
            public List<ItemWeightUnit> WeightUnits { get; set; } = new List<ItemWeightUnit>();

            public string CountryOfOrigin { get; set; }
            public string AgriculturePermitPortGroup { get; set; } = "غير مربوط";
            public string PaymentReleasePortGroup { get; set; } = "غير مربوط";
        }

        public class ItemUnit
        {
            public string UnitName { get; set; }
            public string Package { get; set; }
            public bool IsMain { get; set; }
            public bool IsSales { get; set; }
            public bool IsPurchase { get; set; }
            public bool IsInventory { get; set; }
            public bool NotForSale { get; set; }
            public bool IsStopped { get; set; }
        }

        public class ItemWeightUnit
        {
            public string UnitName { get; set; }
            public decimal Weight { get; set; }
        }

        public class ItemCost
        {
            public string WarehouseCode { get; set; }
            public string WarehouseName { get; set; }
            public decimal InitialCost { get; set; }
            public decimal AvgCost { get; set; }
            public decimal AvailableQty { get; set; }
            public bool IsStopped { get; set; }
        }

        public class GroupData
        {
            public int Id { get; set; }
            public string GroupNo { get; set; }
            public string GroupName { get; set; }
            public string ForeignName { get; set; }
            public string Description { get; set; }
            public bool IsActive { get; set; }
        }

        private void LoadMeasurementUnits()
        {
            try
            {
                measurementUnits.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT UnitCode, UnitName, IsActive FROM MeasurementUnits WHERE IsActive = 1 ORDER BY UnitCode");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        measurementUnits.Add(new UnitsForm.UnitData
                        {
                            UnitCode = row["UnitCode"].ToString(),
                            UnitName = row["UnitName"].ToString(),
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }
                }
                else
                {
                    measurementUnits = new List<UnitsForm.UnitData>
                    {
                        new UnitsForm.UnitData { UnitCode = "1", UnitName = "قطعة", IsActive = true },
                        new UnitsForm.UnitData { UnitCode = "2", UnitName = "كجم", IsActive = true },
                        new UnitsForm.UnitData { UnitCode = "3", UnitName = "لتر", IsActive = true },
                        new UnitsForm.UnitData { UnitCode = "4", UnitName = "علبة", IsActive = true },
                        new UnitsForm.UnitData { UnitCode = "5", UnitName = "كرتون", IsActive = true }
                    };
                }

                cmbDefaultUnit.Items.Clear();
                cmbDefaultUnit.Items.Add("");
                foreach (var unit in measurementUnits.Where(u => u.IsActive))
                {
                    cmbDefaultUnit.Items.Add(unit);
                }
                cmbDefaultUnit.DisplayMember = "UnitName";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ تحميل الوحدات:\n{ex.Message}", "خطأ");
            }
        }

        private void RefreshGroupComboBox()
        {
            cmbGroupSelect.Items.Clear();
            cmbGroupSelect.Items.Add("");
            foreach (var group in groups.Where(g => g.IsActive)) cmbGroupSelect.Items.Add(group);
            cmbGroupSelect.DisplayMember = "GroupName";
            cmbGroupSelect.SelectedIndex = -1;
        }

        private void InitializeGrids()
        {
            if (dgvUnits != null && dgvUnits.Columns.Count == 0)
            {
                dgvUnits.Columns.Add("colNum", LanguageManager.Get("col_unit_num"));
                dgvUnits.Columns.Add("colUnit", LanguageManager.Get("col_unit_name"));
                dgvUnits.Columns.Add("colPackage", LanguageManager.Get("col_unit_package"));
                dgvUnits.Columns["colNum"].Width = 40;
                dgvUnits.Columns["colUnit"].Width = 150;
                dgvUnits.Columns["colPackage"].Width = 100;
                dgvUnits.AllowUserToAddRows = false;
                dgvUnits.AllowUserToDeleteRows = true;
                dgvUnits.CellDoubleClick += DgvUnits_CellDoubleClick;
            }

            if (dgvCost != null && dgvCost.Columns.Count == 0)
            {
                dgvCost.Columns.Add("colWarehouse", LanguageManager.Get("col_cost_warehouse"));
                dgvCost.Columns.Add("colWarehouseName", LanguageManager.Get("col_cost_warehouse_name"));
                dgvCost.Columns.Add("colInitialCost", LanguageManager.Get("col_cost_initial"));
                dgvCost.Columns.Add("colAvgCost", LanguageManager.Get("col_cost_avg"));
                dgvCost.Columns.Add("colAvailableQty", LanguageManager.Get("col_cost_available"));
                dgvCost.Columns.Add("colStatus", LanguageManager.Get("col_cost_status"));
                dgvCost.Columns["colWarehouse"].Width = 150;
                dgvCost.Columns["colWarehouseName"].Width = 250;
                dgvCost.AllowUserToAddRows = true;
                dgvCost.AllowUserToDeleteRows = true;
            }

            if (dgvWeightUnits != null)
            {
                if (dgvWeightUnits.Columns.Count == 0)
                {
                    DataGridViewTextBoxColumn colWeightNum = new DataGridViewTextBoxColumn();
                    colWeightNum.Name = "colWeightNum";
                    colWeightNum.HeaderText = "م";
                    colWeightNum.Width = 40;
                    colWeightNum.ReadOnly = true;
                    colWeightNum.DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

                    DataGridViewTextBoxColumn colUnit = new DataGridViewTextBoxColumn();
                    colUnit.Name = "colWeightUnit";
                    colUnit.HeaderText = LanguageManager.Get("col_weight_unit");
                    colUnit.Width = 400;

                    DataGridViewTextBoxColumn colWeight = new DataGridViewTextBoxColumn();
                    colWeight.Name = "colWeightValue";
                    colWeight.HeaderText = LanguageManager.Get("col_weight_value");
                    colWeight.Width = 400;
                    colWeight.DefaultCellStyle.Format = "N2";
                    colWeight.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                    dgvWeightUnits.Columns.AddRange(colWeightNum, colUnit, colWeight);
                }
                dgvWeightUnits.AllowUserToAddRows = true;
                dgvWeightUnits.AllowUserToDeleteRows = true;
                dgvWeightUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvWeightUnits.BackgroundColor = Color.White;
                dgvWeightUnits.BorderStyle = BorderStyle.Fixed3D;
                dgvWeightUnits.EnableHeadersVisualStyles = false;
                dgvWeightUnits.RowHeadersVisible = false;
                dgvWeightUnits.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvWeightUnits.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(220, 245, 250);
                dgvWeightUnits.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
                dgvWeightUnits.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                dgvWeightUnits.ColumnHeadersHeight = 35;
                dgvWeightUnits.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
                dgvWeightUnits.DefaultCellStyle.BackColor = Color.White;
                dgvWeightUnits.DefaultCellStyle.SelectionBackColor = Color.FromArgb(200, 230, 255);
                dgvWeightUnits.DefaultCellStyle.SelectionForeColor = Color.Black;
                dgvWeightUnits.CellDoubleClick -= DgvWeightUnits_CellDoubleClick;
                dgvWeightUnits.CellDoubleClick += DgvWeightUnits_CellDoubleClick;
            }
        }

        private void DgvWeightUnits_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == 1)
            {
                string value = e.FormattedValue?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(value))
                {
                    if (!decimal.TryParse(value, out _))
                    {
                        MessageBox.Show("⚠️ الرجاء إدخال رقم صحيح للوزن", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        e.Cancel = true;
                    }
                }
            }
        }

        private void MarkRequiredFields()
        {
            Color reqColor = Color.FromArgb(255, 255, 190);
            txtGroupNo.BackColor = reqColor; txtGroupNo.Font = new Font(txtGroupNo.Font, FontStyle.Bold);
            txtGroupName.BackColor = reqColor; txtGroupName.Font = new Font(txtGroupName.Font, FontStyle.Bold);
            txtItemCode.BackColor = reqColor; txtItemCode.Font = new Font(txtItemCode.Font, FontStyle.Bold);
            txtItemName.BackColor = reqColor; txtItemName.Font = new Font(txtItemName.Font, FontStyle.Bold);

            lblGroupNo.Text = LanguageManager.Get("lbl_group_no");
            lblGroupName.Text = LanguageManager.Get("lbl_group_name");
            lblItemCode.Text = LanguageManager.Get("lbl_item_code");
            lblItemName.Text = LanguageManager.Get("lbl_item_name");
        }

        // ==================== ✅ تهيئة عناصر المجموعات ====================
        private void InitializeGroupControls()
        {
            cmbGroupSelect = new ComboBox
            {
                Location = new Point(850, 62),
                Size = new Size(150, 23),
                DropDownStyle = ComboBoxStyle.DropDownList,
                RightToLeft = RightToLeft.Yes,
                Font = new Font("Segoe UI", 9F)
            };
            cmbGroupSelect.SelectedIndexChanged += CmbGroupSelect_SelectedIndexChanged;
            this.Controls.Add(cmbGroupSelect);

            btnOpenGroups = new Button
            {
                Location = new Point(820, 62),
                Size = new Size(28, 23),
                Text = "...",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnOpenGroups.Click += (s, e) => OpenGroupsForm();
            this.Controls.Add(btnOpenGroups);

            if (cmbAgriculturePermit != null)
            {
                cmbAgriculturePermit.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbAgriculturePermit.RightToLeft = RightToLeft.Yes;
                cmbAgriculturePermit.SelectedIndexChanged += CmbPermitLink_SelectedIndexChanged;
            }

            if (cmbPaymentRelease != null)
            {
                cmbPaymentRelease.DropDownStyle = ComboBoxStyle.DropDownList;
                cmbPaymentRelease.RightToLeft = RightToLeft.Yes;
                cmbPaymentRelease.SelectedIndexChanged += CmbPermitLink_SelectedIndexChanged;
            }

            txtGroupNo.ReadOnly = false;
            txtGroupName.ReadOnly = false;
            txtGroupNo.BackColor = Color.FromArgb(255, 255, 190);
            txtGroupName.BackColor = Color.FromArgb(255, 255, 190);

            txtGroupNo.TextChanged += TxtGroupNo_TextChanged;
            txtGroupNo.KeyDown += TxtGroupNo_KeyDown;
            txtSubGroupUnder.KeyDown += TxtSubGroupUnder_KeyDown;
        }

        private void TxtSubGroupUnder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                OpenSubGroupSelectionForm();
            }
        }

        private void OpenSubGroupSelectionForm()
        {
            string groupNo = txtGroupNo.Text.Trim();
            if (string.IsNullOrEmpty(groupNo))
            {
                MessageBox.Show("⚠️ الرجاء اختيار المجموعة الرئيسية أولاً\n💡 اكتب رقم المجموعة أو اضغط F9 عليها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGroupNo.Focus();
                return;
            }

            DataTable dt = null;
            try
            {
                dt = DatabaseHelper.ExecuteQuery(
                    "SELECT SubGroupNo, SubGroupName FROM SubGroups WHERE GroupNo = @GroupNo ORDER BY SubGroupNo",
                    DatabaseHelper.CreateParameter("@GroupNo", groupNo));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحميل المجموعات الفرعية:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show($"⚠️ لا توجد مجموعات فرعية مضافة للمجموعة الرئيسية: {groupNo}\n💡 الرجاء إضافتها من شاشة المجموعات الفرعية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form selectForm = new Form();
            selectForm.Text = $"🔍 اختر المجموعة الفرعية ({dt.Rows.Count} مجموعة)";
            selectForm.Size = new Size(700, 500);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 50;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);

            Label lblSearch = new Label();
            lblSearch.Text = "🔍 بحث:";
            lblSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSearch.Location = new Point(550, 15);
            lblSearch.Size = new Size(60, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            TextBox txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(300, 15);
            txtSearch.Size = new Size(240, 25);

            Label lblCount = new Label();
            lblCount.Text = $" المجموعات الفرعية: {dt.Rows.Count}";
            lblCount.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCount.ForeColor = Color.FromArgb(33, 150, 243);
            lblCount.Location = new Point(20, 15);
            lblCount.Size = new Size(250, 25);

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(lblCount);

            DataGridView dgvSubGroups = new DataGridView();
            dgvSubGroups.Dock = DockStyle.Fill;
            dgvSubGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSubGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSubGroups.MultiSelect = false;
            dgvSubGroups.ReadOnly = true;
            dgvSubGroups.AllowUserToAddRows = false;
            dgvSubGroups.BackgroundColor = Color.White;
            dgvSubGroups.RowHeadersVisible = false;
            dgvSubGroups.EnableHeadersVisualStyles = false;
            dgvSubGroups.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvSubGroups.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSubGroups.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvSubGroups.ColumnHeadersHeight = 40;
            dgvSubGroups.RowTemplate.Height = 35;

            dgvSubGroups.Columns.Add("colSubGroupNo", "رقم المجموعة الفرعية");
            dgvSubGroups.Columns.Add("colSubGroupName", "اسم المجموعة الفرعية");

            foreach (DataRow row in dt.Rows)
            {
                dgvSubGroups.Rows.Add(row["SubGroupNo"].ToString(), row["SubGroupName"].ToString());
            }

            dgvSubGroups.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string selectedSubGroupNo = dgvSubGroups.Rows[ev.RowIndex].Cells["colSubGroupNo"].Value?.ToString();
                    txtSubGroupUnder.Text = selectedSubGroupNo;
                    selectForm.Close();
                }
            };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnSelect = new Button();
            btnSelect.Text = "✅ اختيار";
            btnSelect.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnSelect.BackColor = Color.FromArgb(76, 175, 80);
            btnSelect.ForeColor = Color.White;
            btnSelect.FlatStyle = FlatStyle.Flat;
            btnSelect.Size = new Size(150, 45);
            btnSelect.Location = new Point(450, 8);
            btnSelect.Click += (s, ev) =>
            {
                if (dgvSubGroups.SelectedRows.Count > 0)
                {
                    string selectedSubGroupNo = dgvSubGroups.SelectedRows[0].Cells["colSubGroupNo"].Value?.ToString();
                    txtSubGroupUnder.Text = selectedSubGroupNo;
                    selectForm.Close();
                }
            };

            Button btnCancel = new Button();
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(280, 8);
            btnCancel.Click += (s, ev) => selectForm.Close();

            pnlButtons.Controls.Add(btnSelect);
            pnlButtons.Controls.Add(btnCancel);

            selectForm.Controls.Add(dgvSubGroups);
            selectForm.Controls.Add(pnlSearch);
            selectForm.Controls.Add(pnlButtons);
            selectForm.AcceptButton = btnSelect;
            selectForm.CancelButton = btnCancel;

            txtSearch.TextChanged += (s, ev) =>
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                int visibleCount = 0;
                foreach (DataGridViewRow row in dgvSubGroups.Rows)
                {
                    string subNo = row.Cells["colSubGroupNo"].Value?.ToString()?.ToLower() ?? "";
                    string subName = row.Cells["colSubGroupName"].Value?.ToString()?.ToLower() ?? "";

                    if (string.IsNullOrEmpty(searchText) || subNo.Contains(searchText) || subName.Contains(searchText))
                    {
                        row.Visible = true;
                        visibleCount++;
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
                lblCount.Text = $"📊 المعروض: {visibleCount} من {dt.Rows.Count}";
            };

            selectForm.ShowDialog();
        }

        private void GenerateItemCode()
        {
            string groupNo = txtGroupNo.Text.Trim();
            if (string.IsNullOrEmpty(groupNo)) { txtItemCode.Clear(); return; }

            var groupItems = items.Where(i => i.GroupNo == groupNo).ToList();
            string newItemCode;
            if (groupItems.Count == 0) newItemCode = groupNo + "1";
            else
            {
                var lastItem = groupItems.OrderByDescending(i => i.ItemCode).FirstOrDefault();
                if (lastItem != null && lastItem.ItemCode.StartsWith(groupNo))
                {
                    string seq = lastItem.ItemCode.Substring(groupNo.Length);
                    newItemCode = int.TryParse(seq, out int sequence) ? groupNo + (sequence + 1) : groupNo + "1";
                }
                else newItemCode = groupNo + "1";
            }
            txtItemCode.Text = newItemCode;
        }

        private void TxtGroupNo_TextChanged(object sender, EventArgs e)
        {
            string groupNo = txtGroupNo.Text.Trim();
            if (string.IsNullOrEmpty(groupNo)) { txtGroupName.Clear(); txtItemCode.Clear(); return; }

            var group = groups.FirstOrDefault(g => g.GroupNo == groupNo);
            if (group != null)
            {
                txtGroupName.Text = group.GroupName;
                txtGroupName.BackColor = Color.LightGreen;
                GenerateItemCode();
            }
            else
            {
                txtGroupName.Text = "";
                txtGroupName.BackColor = Color.FromArgb(255, 255, 190);
                txtItemCode.Clear();
            }
            TrackChanges();
        }

        private void TxtGroupNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                OpenGroupSelectionForm();
            }
        }

        private void OpenGroupSelectionForm()
        {
            if (groups == null || groups.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد مجموعات مضافة\nالرجاء إضافة مجموعات من شاشة تهيئة المجموعات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form selectForm = new Form();
            selectForm.Text = $"🔍 اختر المجموعة ({groups.Count} مجموعة)";
            selectForm.Size = new Size(700, 500);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 50;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);

            Label lblSearch = new Label();
            lblSearch.Text = "🔍 بحث:";
            lblSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSearch.Location = new Point(550, 15);
            lblSearch.Size = new Size(60, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            TextBox txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(300, 15);
            txtSearch.Size = new Size(240, 25);

            Label lblCount = new Label();
            lblCount.Text = $" المجموعات: {groups.Count}";
            lblCount.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCount.ForeColor = Color.FromArgb(33, 150, 243);
            lblCount.Location = new Point(20, 15);
            lblCount.Size = new Size(200, 25);

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(lblCount);

            DataGridView dgvGroups = new DataGridView();
            dgvGroups.Dock = DockStyle.Fill;
            dgvGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGroups.MultiSelect = false;
            dgvGroups.ReadOnly = true;
            dgvGroups.AllowUserToAddRows = false;
            dgvGroups.BackgroundColor = Color.White;
            dgvGroups.RowHeadersVisible = false;
            dgvGroups.EnableHeadersVisualStyles = false;
            dgvGroups.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvGroups.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGroups.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvGroups.ColumnHeadersHeight = 40;
            dgvGroups.RowTemplate.Height = 35;

            dgvGroups.Columns.Add("colGroupNo", "رقم المجموعة");
            dgvGroups.Columns.Add("colGroupName", "اسم المجموعة");
            dgvGroups.Columns.Add("colStatus", "الحالة");

            foreach (var group in groups.Where(g => g.IsActive).OrderBy(g => g.GroupNo))
            {
                dgvGroups.Rows.Add(group.GroupNo, group.GroupName, "✅ نشط");
            }

            dgvGroups.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.ColumnIndex == 2)
                {
                    ev.CellStyle.ForeColor = Color.Green;
                    ev.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            };

            dgvGroups.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string selectedGroupNo = dgvGroups.Rows[ev.RowIndex].Cells["colGroupNo"].Value?.ToString();
                    var selectedGroup = groups.FirstOrDefault(g => g.GroupNo == selectedGroupNo);
                    if (selectedGroup != null)
                    {
                        cmbGroupSelect.SelectedItem = selectedGroup;
                        selectForm.Close();
                    }
                }
            };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnSelect = new Button();
            btnSelect.Text = "✅ اختيار";
            btnSelect.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnSelect.BackColor = Color.FromArgb(76, 175, 80);
            btnSelect.ForeColor = Color.White;
            btnSelect.FlatStyle = FlatStyle.Flat;
            btnSelect.Size = new Size(150, 45);
            btnSelect.Location = new Point(450, 8);
            btnSelect.Click += (s, ev) =>
            {
                if (dgvGroups.SelectedRows.Count > 0)
                {
                    string selectedGroupNo = dgvGroups.SelectedRows[0].Cells["colGroupNo"].Value?.ToString();
                    var selectedGroup = groups.FirstOrDefault(g => g.GroupNo == selectedGroupNo);
                    if (selectedGroup != null)
                    {
                        cmbGroupSelect.SelectedItem = selectedGroup;
                        selectForm.Close();
                    }
                }
            };

            Button btnCancel = new Button();
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(280, 8);
            btnCancel.Click += (s, ev) => selectForm.Close();

            pnlButtons.Controls.Add(btnSelect);
            pnlButtons.Controls.Add(btnCancel);

            selectForm.Controls.Add(dgvGroups);
            selectForm.Controls.Add(pnlSearch);
            selectForm.Controls.Add(pnlButtons);
            selectForm.AcceptButton = btnSelect;
            selectForm.CancelButton = btnCancel;

            txtSearch.TextChanged += (s, ev) =>
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                int visibleCount = 0;
                foreach (DataGridViewRow row in dgvGroups.Rows)
                {
                    string groupNo = row.Cells["colGroupNo"].Value?.ToString()?.ToLower() ?? "";
                    string groupName = row.Cells["colGroupName"].Value?.ToString()?.ToLower() ?? "";

                    if (string.IsNullOrEmpty(searchText) || groupNo.Contains(searchText) || groupName.Contains(searchText))
                    {
                        row.Visible = true;
                        visibleCount++;
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
                lblCount.Text = $"📊 المعروض: {visibleCount} من {groups.Count}";
            };

            selectForm.ShowDialog();
        }

        private void CmbGroupSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGroupSelect.SelectedItem is GroupData selectedGroup)
            {
                txtGroupNo.Text = selectedGroup.GroupNo;
                txtGroupName.Text = selectedGroup.GroupName;
                GenerateItemCode();
                TrackChanges();
            }
        }

        private void OpenGroupsForm()
        {
            using (var groupsForm = new GroupsForm())
            {
                groupsForm.FormClosed += (s, e) => { LoadGroupsFromDatabase(); };
                groupsForm.ShowDialog();
            }
        }

        public void RefreshGroupsList(List<GroupData> newGroups = null)
        {
            if (newGroups != null) groups = newGroups;
            RefreshGroupComboBox();
        }

        private bool ValidateRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(txtGroupNo.Text)) { MessageBox.Show(LanguageManager.Get("msg_group_required"), LanguageManager.Get("msg_warning")); txtGroupNo.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtGroupName.Text)) { MessageBox.Show(LanguageManager.Get("msg_group_name_required"), LanguageManager.Get("msg_warning")); txtGroupName.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtItemCode.Text)) { MessageBox.Show(LanguageManager.Get("msg_item_code_required"), LanguageManager.Get("msg_warning")); txtItemCode.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtItemName.Text)) { MessageBox.Show(LanguageManager.Get("msg_item_name_required"), LanguageManager.Get("msg_warning")); txtItemName.Focus(); return false; }
            return true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (isEditingMode)
            {
                MessageBox.Show("️ لا يمكن الحذف في وضع التعديل\nالرجاء إلغاء التعديل أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (currentIndex < 0 || currentIndex >= items.Count)
            {
                MessageBox.Show(LanguageManager.Get("msg_no_item"), LanguageManager.Get("msg_warning"));
                return;
            }

            var item = items[currentIndex];
            if (MessageBox.Show(string.Format(LanguageManager.Get("msg_delete_confirm"), item.ItemCode, item.ItemName), LanguageManager.Get("msg_confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "DELETE FROM Items WHERE ItemCode = @ItemCode",
                        DatabaseHelper.CreateParameter("@ItemCode", item.ItemCode)
                    );

                    items.RemoveAt(currentIndex);
                    if (items.Count > 0)
                    {
                        currentIndex = Math.Min(currentIndex, items.Count - 1);
                        LoadItemToForm(items[currentIndex]);
                    }
                    else
                    {
                        currentIndex = -1;
                        ClearFields();
                    }
                    UpdateNavigation();
                    MessageBox.Show(LanguageManager.Get("msg_delete_success"), LanguageManager.Get("success_title"));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($" خطأ في الحذف:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!isEditingMode)
            {
                MessageBox.Show("يجب الضغط على 'جديد' أو 'تعديل' أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateRequiredFields()) return;

            try
            {
                var itemToSave = new ItemData();
                UpdateItemFromForm(itemToSave);
                SaveDataToDatabase(itemToSave);

                var existingIndex = items.FindIndex(i => i.ItemCode == itemToSave.ItemCode);
                if (existingIndex >= 0)
                {
                    items[existingIndex] = itemToSave;
                }
                else
                {
                    items.Add(itemToSave);
                    currentIndex = items.Count - 1;
                }

                // ✅ بعد وجود الصنف في قاعدة البيانات، نعيد كتابة قيمتي الربط
                PersistLinkFieldsImmediately();

                // ✅ تحقق مرئي من قاعدة البيانات - نقرأ من العمودين الجديدين
                string savedAgri = "?", savedPay = "?";
                try
                {
                    DataTable verify = DatabaseHelper.ExecuteQuery(
                        $"SELECT {COL_AGRI}, {COL_PAY} FROM Items WHERE ItemCode = @C",
                        DatabaseHelper.CreateParameter("@C", itemToSave.ItemCode));
                    if (verify != null && verify.Rows.Count > 0)
                    {
                        savedAgri = verify.Rows[0][COL_AGRI]?.ToString() ?? "فارغ";
                        savedPay = verify.Rows[0][COL_PAY]?.ToString() ?? "فارغ";
                    }
                }
                catch { }

                hasChanges = false;
                SetControlsState(false);
                UpdateNavigation();

                MessageBox.Show(
                    LanguageManager.Get("msg_save_success") +
                    $"\n🌾 تصاريح زراعية (في قاعدة البيانات): {savedAgri}" +
                    $"\n💳 إفراج مدفوعات (في قاعدة البيانات): {savedPay}",
                    LanguageManager.Get("success_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($" {LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"));
            }
        }

        // ==================== ✅ تحديث الصنف من النموذج ====================
        private void UpdateItemFromForm(ItemData item)
        {
            item.GroupNo = txtGroupNo.Text.Trim();
            item.GroupName = txtGroupName.Text.Trim();
            item.ItemCode = txtItemCode.Text.Trim();
            item.ItemName = txtItemName.Text.Trim();
            item.ItemType = cmbItemType.SelectedItem?.ToString();
            item.SubGroupUnder = txtSubGroupUnder.Text.Trim();
            item.Branch = txtBranch.Text.Trim();
            item.RefNo = txtRefNo.Text.Trim();
            item.Barcode = txtBarcode.Text.Trim();
            item.ForeignName = txtForeignName.Text.Trim();
            item.ItemSpecs = txtItemSpecs.Text.Trim();
            item.InitialCost = string.IsNullOrWhiteSpace(txtInitialCost.Text) ? 0 : decimal.Parse(txtInitialCost.Text);

            var selectedUnit = cmbDefaultUnit.SelectedItem as UnitsForm.UnitData;
            item.DefaultUnit = selectedUnit?.UnitCode;

            item.ItemActivity = cmbItemActivity.SelectedItem?.ToString();
            item.OpenCost = string.IsNullOrWhiteSpace(txtOpenCost.Text) ? 0 : decimal.Parse(txtOpenCost.Text);
            item.AvailableQty = string.IsNullOrWhiteSpace(txtAvailableQty.Text) ? 0 : decimal.Parse(txtAvailableQty.Text);

            item.CountryOfOrigin = txtCountryOfOrigin.Text.Trim();

            item.AgriculturePermitPortGroup = cmbAgriculturePermit?.SelectedItem?.ToString() ?? "غير مربوط";
            item.PaymentReleasePortGroup = cmbPaymentRelease?.SelectedItem?.ToString() ?? "غير مربوط";

            item.Units.Clear();
            if (dgvUnits != null)
            {
                dgvUnits.EndEdit();
                foreach (DataGridViewRow row in dgvUnits.Rows)
                {
                    if (row.IsNewRow)
                    {
                        string testVal = row.Cells["colUnit"].Value?.ToString()?.Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(testVal)) continue;
                    }
                    string unitName = row.Cells["colUnit"].Value?.ToString()?.Trim() ?? "";
                    string package = row.Cells["colPackage"].Value?.ToString()?.Trim() ?? "1";
                    if (!string.IsNullOrWhiteSpace(unitName))
                    {
                        item.Units.Add(new ItemUnit { UnitName = unitName, Package = package });
                    }
                }
            }

            item.WeightUnits.Clear();
            if (dgvWeightUnits != null)
            {
                dgvWeightUnits.EndEdit();
                foreach (DataGridViewRow row in dgvWeightUnits.Rows)
                {
                    if (row.IsNewRow)
                    {
                        string testVal = row.Cells["colWeightUnit"].Value?.ToString()?.Trim() ?? "";
                        if (string.IsNullOrWhiteSpace(testVal)) continue;
                    }
                    string unitName = "";
                    decimal weight = 0;
                    try
                    {
                        if (row.Cells["colWeightUnit"] != null)
                            unitName = row.Cells["colWeightUnit"].Value?.ToString()?.Trim() ?? "";
                        if (row.Cells["colWeightValue"] != null)
                            decimal.TryParse(row.Cells["colWeightValue"].Value?.ToString(), out weight);
                    }
                    catch
                    {
                        if (row.Cells.Count > 1)
                        {
                            unitName = row.Cells[0].Value?.ToString()?.Trim() ?? "";
                            decimal.TryParse(row.Cells[1].Value?.ToString(), out weight);
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(unitName))
                    {
                        item.WeightUnits.Add(new ItemWeightUnit { UnitName = unitName, Weight = weight });
                    }
                }
            }

            if (currentImagePath != null && File.Exists(currentImagePath))
            {
                item.ItemImage = File.ReadAllBytes(currentImagePath);
            }
            else if (currentIndex >= 0 && currentIndex < items.Count && items[currentIndex].ItemCode == item.ItemCode)
            {
                item.ItemImage = items[currentIndex].ItemImage;
            }
        }

        private void ClearFields()
        {
            cmbGroupSelect.SelectedIndex = -1;
            txtGroupNo.Clear(); txtGroupName.Clear(); txtItemCode.Clear(); txtItemName.Clear();
            cmbItemType.SelectedIndex = -1;
            txtSubGroupUnder.Clear(); txtBarcode.Clear(); txtForeignName.Clear();
            txtInitialCost.Text = "0"; cmbDefaultUnit.SelectedIndex = -1; cmbItemActivity.SelectedIndex = -1;
            txtAvailableQty.Text = "0";

            txtCountryOfOrigin.Clear();

            SetLinkCombos("غير مربوط", "غير مربوط");

            if (picItem != null) picItem.Image = null;
            currentImagePath = null;

            if (dgvUnits != null) dgvUnits.Rows.Clear();
            if (dgvCost != null) dgvCost.Rows.Clear();
            if (dgvWeightUnits != null) dgvWeightUnits.Rows.Clear();

            currentIndex = -1;
            UpdateNavigation();
            if (txtItemCode != null) txtItemCode.Focus();
        }

        // ==================== ✅ تحميل الصنف إلى النموذج ====================
        private void LoadItemToForm(ItemData item)
        {
            var group = groups.FirstOrDefault(g => g.GroupNo == item.GroupNo);
            if (group != null) cmbGroupSelect.SelectedItem = group;

            txtGroupNo.Text = item.GroupNo;
            txtGroupName.Text = item.GroupName;
            txtItemCode.Text = item.ItemCode;
            txtItemName.Text = item.ItemName;
            cmbItemType.SelectedItem = item.ItemType;
            txtSubGroupUnder.Text = item.SubGroupUnder;
            txtBarcode.Text = item.Barcode;
            txtForeignName.Text = item.ForeignName;
            txtInitialCost.Text = item.InitialCost.ToString("0.00");

            var unit = measurementUnits.FirstOrDefault(u => u.UnitCode == item.DefaultUnit);
            cmbDefaultUnit.SelectedItem = unit;

            cmbItemActivity.SelectedItem = item.ItemActivity;
            txtAvailableQty.Text = item.AvailableQty.ToString("0.00");

            txtCountryOfOrigin.Text = item.CountryOfOrigin ?? "";

            SetLinkCombos(item.AgriculturePermitPortGroup, item.PaymentReleasePortGroup);

            if (dgvUnits != null)
            {
                dgvUnits.Rows.Clear();
                int rowIndex = 1;
                foreach (var unitItem in item.Units)
                {
                    dgvUnits.Rows.Add(rowIndex++, unitItem.UnitName, unitItem.Package);
                }
            }

            if (dgvWeightUnits != null)
            {
                dgvWeightUnits.Rows.Clear();
                if (item.WeightUnits != null)
                {
                    int wIndex = 1;
                    foreach (var weightUnit in item.WeightUnits)
                    {
                        dgvWeightUnits.Rows.Add(wIndex++, weightUnit.UnitName, weightUnit.Weight.ToString("0.00"));
                    }
                }
            }

            if (item.ItemImage != null && item.ItemImage.Length > 0 && picItem != null)
            {
                using (var ms = new MemoryStream(item.ItemImage))
                {
                    picItem.Image = Image.FromStream(ms);
                    picItem.SizeMode = PictureBoxSizeMode.StretchImage;
                }
            }
            else if (picItem != null)
            {
                picItem.Image = null;
            }

            UpdateRowNumbers(dgvUnits, "colNum");
            UpdateRowNumbers(dgvWeightUnits, "colWeightNum");
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (!CheckUnsavedChanges()) return;
            try
            {
                MainForm mainForm = this.MdiParent as MainForm ?? Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm != null && !mainForm.IsDisposed)
                {
                    foreach (Form child in mainForm.MdiChildren.ToList())
                        if (!child.IsDisposed && child != this) child.Close();
                    this.Close();
                    mainForm.ShowWelcome();
                    mainForm.WindowState = FormWindowState.Maximized;
                    mainForm.Activate();
                    mainForm.BringToFront();
                }
                else this.Close();
            }
            catch (Exception ex) { MessageBox.Show($"❌ {LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error); this.Close(); }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (isEditingMode && hasChanges)
            {
                DialogResult result = MessageBox.Show("⚠️ لديك تغييرات غير محفوظة!\nهل تريد تجاهلها والبدء بصنف جديد؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) return;
            }
            ClearFields();
            SetControlsState(true);
            originalItem = null;
            currentIndex = -1;
            MessageBox.Show("✅ تم فتح نموذج إضافة صنف جديد\n💡 أدخل البيانات ثم اضغط 'حفظ'", "إضافة صنف جديد", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= items.Count)
            {
                MessageBox.Show("⚠️ الرجاء اختيار صنف للتعديل أولاً\n💡 استخدم أزرار التنقل (الأول، السابق، التالي، الأخير) أو F9 للاختيار", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveOriginalItem();
            SetControlsState(true);
            MessageBox.Show($"✅ تم فتح وضع تعديل الصنف: {items[currentIndex].ItemName}\nعدّل البيانات ثم اضغط 'حفظ'", "تعديل صنف", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void CancelEditMode()
        {
            if (!isEditingMode) return;
            DialogResult result = MessageBox.Show("⚠️ هل تريد إلغاء التعديلات الحالية؟\nسيتم استعادة البيانات الأصلية", "تأكيد الإلغاء", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (originalItem != null && currentIndex >= 0)
                {
                    LoadItemToForm(originalItem);
                    PersistLinkFieldsImmediately();
                }
                else if (currentIndex < 0)
                {
                    ClearFields();
                }
                SetControlsState(false);
                hasChanges = false;
                originalItem = null;
            }
        }

        private void UpdateNavigation()
        {
            string mode = isEditingMode ? " [وضع التعديل]" : "";
            this.Text = string.Format(LanguageManager.Get("items_form_nav"), currentIndex + 1, items.Count) + mode;
        }

        private bool ConfirmNavigation()
        {
            if (isEditingMode && hasChanges)
            {
                DialogResult result = MessageBox.Show(
                    "⚠️ لديك تغييرات غير محفوظة!\n" +
                    "هل تريد حفظ التغييرات قبل الانتقال؟\n" +
                    "• نعم: حفظ والانتقال\n" +
                    "• لا: تجاهل التغييرات والانتقال\n" +
                    "• إلغاء: البقاء في الصنف الحالي",
                    "تأكيد التنقل", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    if (!ValidateRequiredFields()) return false;
                    ItemData itemToSave;
                    var existing = items.FirstOrDefault(i => i.ItemCode == txtItemCode.Text);
                    if (existing != null)
                    {
                        UpdateItemFromForm(existing);
                        itemToSave = existing;
                    }
                    else
                    {
                        var newItem = new ItemData();
                        UpdateItemFromForm(newItem);
                        items.Add(newItem);
                        currentIndex = items.Count - 1;
                        itemToSave = newItem;
                    }
                    SaveDataToDatabase(itemToSave);
                    PersistLinkFieldsImmediately(); // ✅ إضافة
                    hasChanges = false;
                    return true;
                }
                else if (result == DialogResult.No)
                {
                    hasChanges = false;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }

        private void btnFirst_Click(object sender, EventArgs e)
        {
            if (!ConfirmNavigation()) return;
            if (items.Count > 0)
            {
                currentIndex = 0;
                LoadItemToForm(items[0]);
                SetControlsState(false);
                UpdateNavigation();
            }
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (!ConfirmNavigation()) return;
            if (currentIndex > 0)
            {
                currentIndex--;
                LoadItemToForm(items[currentIndex]);
                SetControlsState(false);
                UpdateNavigation();
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (!ConfirmNavigation()) return;
            if (currentIndex < items.Count - 1)
            {
                currentIndex++;
                LoadItemToForm(items[currentIndex]);
                SetControlsState(false);
                UpdateNavigation();
            }
        }

        private void btnLast_Click(object sender, EventArgs e)
        {
            if (!ConfirmNavigation()) return;
            if (items.Count > 0)
            {
                currentIndex = items.Count - 1;
                LoadItemToForm(items[currentIndex]);
                SetControlsState(false);
                UpdateNavigation();
            }
        }

        private void btnFilter_Click(object sender, EventArgs e) { /* التصفية */ }
        private void btnPrint_Click(object sender, EventArgs e) { MessageBox.Show(LanguageManager.Get("msg_printing")); }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (items.Count == 0)
            {
                MessageBox.Show("️ لا توجد بيانات للتصدير", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV ملف|*.csv";
            sfd.FileName = $"Items_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("م,رمز الصنف,اسم الصنف,المجموعة,النوع,الوحدة,التكلفة,النشاط");
                        int idx = 1;
                        foreach (var item in items)
                        {
                            string unitName = item.DefaultUnit ?? "";
                            var unit = measurementUnits.FirstOrDefault(u => u.UnitCode == item.DefaultUnit);
                            if (unit != null) unitName = unit.UnitName;
                            sw.WriteLine($"{idx++},{item.ItemCode},{item.ItemName},{item.GroupName},{item.ItemType},{unitName},{item.InitialCost},{item.ItemActivity}");
                        }
                    }
                    MessageBox.Show($"✅ تم تصدير {items.Count} صنف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في التصدير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnExit_Click(object sender, EventArgs e) => this.Close();
        private void btnGroups_Click(object sender, EventArgs e) => OpenUnitsForm();

        private void btnImgAdd_Click(object sender, EventArgs e)
        {
            using (var ofd = new OpenFileDialog { Filter = "صور|*.jpg;*.png;*.gif", Title = LanguageManager.Get("msg_add_image") })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    currentImagePath = ofd.FileName;
                    picItem.Image = Image.FromFile(currentImagePath);
                    picItem.SizeMode = PictureBoxSizeMode.StretchImage;
                    TrackChanges();
                }
            }
        }

        private void btnImgDel_Click(object sender, EventArgs e)
        {
            if (picItem != null) picItem.Image = null;
            currentImagePath = null;
            TrackChanges();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!CheckUnsavedChanges())
            {
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }

        private void LoadSampleData()
        {
            items.AddRange(new[] {
                new ItemData { GroupNo = "100", GroupName = "مواد غذائية", ItemCode = "1001", ItemName = "أرز بسمتي", DefaultUnit = "1", ItemActivity = "نشط" },
                new ItemData { GroupNo = "100", GroupName = "مواد غذائية", ItemCode = "1002", ItemName = "سكر أبيض", DefaultUnit = "2", ItemActivity = "نشط" }
            });
        }
    }
}