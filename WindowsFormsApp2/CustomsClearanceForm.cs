using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Data.SQLite;

namespace YemenCustomsSystem
{
    public class DriverInfo
    {
        public string DriverId { get; set; }
        public string DriverName { get; set; }
        public string VehicleNo { get; set; }
    }

    public class AdditionalAmountItem
    {
        public string ItemName { get; set; }
        public decimal Amount { get; set; }
    }

    public partial class CustomsClearanceForm : Form
    {
        private List<ClearanceRecord> records = new List<ClearanceRecord>();
        private List<PortsForm.PortData> allPorts = new List<PortsForm.PortData>();
        private List<ItemsDataForm.ItemsRecord> allItemsData = new List<ItemsDataForm.ItemsRecord>();
        private List<StockTransferForm.StockMovementRecord> allMovements = new List<StockTransferForm.StockMovementRecord>();
        private string currentPortClassification = "";
        private string currentStatementNo = "";
        private bool isEditingMode = false;
        private bool hasChanges = false;
        private bool isEditingExisting = false;

        // تعريف عناصر الواجهة
        private ToolStrip toolStrip1;
        private ToolStripSeparator toolStripSeparator1, toolStripSeparator2, toolStripSeparator3, toolStripSeparator4;
        private ToolStripButton btnSaveStrip, btnNewStrip, btnEditStrip, btnDeleteStrip, btnSearchStrip, btnExportStrip, btnExitStrip, btnHome;
        private ToolStripButton btnLoadStatementStrip, btnMovementsStrip, btnPrintReportStrip, btnDocumentReviewStrip;
        private TabControl tabControlMain;
        private TabPage tabPageMain;
        private TabPage tabPageOther;
        private TabPage tabPageAdditional;
        private ComboBox cmbPortName;
        private ComboBox cmbTariffType;
        private DateTimePicker dtpStatementDate;
        private TextBox txtStatementNo;
        private TextBox txtUnloadFrom;
        private TextBox txtTariffRequestNo;
        private DataGridView dgvData;
        private NumericUpDown numVehicleCount;
        private DataGridView dgvDrivers;
        private DataGridView dgvAdditionalAmounts;
        private Label lblTotalFeesTitle, lblTotalFeesValue;
        private Label lblOtherFeesTitle;
        private TextBox txtOtherFees;
        private Label lblTotalAmountTitle, lblTotalAmountValue;

        public class ClearanceRecord
        {
            public int Id { get; set; }
            public string PortName { get; set; }
            public DateTime StatementDate { get; set; }
            public string StatementNo { get; set; }
            public string TariffType { get; set; }
            public string UnloadFrom { get; set; }
            public string TariffRequestNo { get; set; }
            public string ClientName { get; set; }
            public string CarNo { get; set; }
            public string ProductName { get; set; }
            public string Unit { get; set; }
            public string WeightUnit { get; set; }
            public decimal Quantity { get; set; }
            public decimal Weight { get; set; }
            public decimal WeightTotal { get; set; }
            public decimal CustomsFeeTotal { get; set; }
            public decimal STTotal { get; set; }
            public decimal VATTotal { get; set; }
            public decimal PTTotal { get; set; }
            public decimal VFTotal { get; set; }
            public decimal TEFTotal { get; set; }
            public decimal MFFTotal { get; set; }
            public decimal Other { get; set; }
            public decimal Total { get; set; }
            public decimal AgriculturePermit { get; set; }
            public decimal PaymentRelease { get; set; }
            public string CountryOfOrigin { get; set; } // تم الإبقاء عليه في الكلاس للحفاظ على بيانات قاعدة البيانات
            public decimal TransferredQty { get; set; }
            public decimal RemainingQty { get; set; }
            public string DriversJson { get; set; }
            public string AdditionalAmountsJson { get; set; }
        }

        public CustomsClearanceForm()
        {
            CreateControlsManually();
            InitializeGrid();
            EnsureDocumentReviewsTable();
            EnsureClearanceColumns();
            LoadPortsFromDatabase();
            LoadItemsDataFromDatabase();
            LoadMovementsFromDatabase();
            LoadDataFromDatabase();
            ClearFields();
            SetControlsState(false);
            ApplyTranslations();
            ApplyPortPermissions();
            LanguageManager.LanguageChanged += (s, e) => RefreshAllTranslations();
        }

        private void EnsureClearanceColumns()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery("ALTER TABLE CustomsClearance ADD COLUMN TariffType TEXT");
                DatabaseHelper.ExecuteNonQuery("ALTER TABLE CustomsClearance ADD COLUMN UnloadFrom TEXT");
                DatabaseHelper.ExecuteNonQuery("ALTER TABLE CustomsClearance ADD COLUMN DriversJson TEXT");
                DatabaseHelper.ExecuteNonQuery("ALTER TABLE CustomsClearance ADD COLUMN AdditionalAmountsJson TEXT");
                DatabaseHelper.ExecuteNonQuery("ALTER TABLE CustomsClearance ADD COLUMN CountryOfOrigin TEXT");
            }
            catch { }
        }

        private void EnsureDocumentReviewsTable()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS DocumentReviews (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StatementNo TEXT NOT NULL,
                    PortName TEXT NOT NULL,
                    ReviewStatus TEXT NOT NULL,
                    Notes TEXT,
                    UserName TEXT,
                    ReviewDate TEXT NOT NULL
                )");
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ إنشاء جدول المراجعات: {ex.Message}"); }
        }

        private void ApplyPortPermissions()
        {
            if (UserManager.IsAdmin()) return;
            if (!UserManager.GetAllowedPorts("Add").Any()) { btnNewStrip.Enabled = false; btnSaveStrip.Enabled = false; }
            if (!UserManager.GetAllowedPorts("Edit").Any()) { btnEditStrip.Enabled = false; }
            if (!UserManager.GetAllowedPorts("Delete").Any()) { btnDeleteStrip.Enabled = false; }
            if (!UserManager.GetAllowedPorts("Print").Any()) { btnPrintReportStrip.Enabled = false; }
        }

        private void ApplyTranslations()
        {
            this.Text = LanguageManager.Get("clearance_form_title");
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;
            if (lblTotalFeesTitle != null) lblTotalFeesTitle.Text = "💰 إجمالي رسوم البيان:";
            if (lblOtherFeesTitle != null) lblOtherFeesTitle.Text = "📋 رسوم أخرى:";
            if (lblTotalAmountTitle != null) lblTotalAmountTitle.Text = "💵 إجمالي مبلغ البيان:";
            if (btnHome != null) btnHome.Text = LanguageManager.Get("btn_home");
            if (btnSaveStrip != null) btnSaveStrip.Text = LanguageManager.Get("btn_save");
            if (btnNewStrip != null) btnNewStrip.Text = LanguageManager.Get("btn_new");
            if (btnEditStrip != null) btnEditStrip.Text = LanguageManager.Get("btn_edit");
            if (btnDeleteStrip != null) btnDeleteStrip.Text = LanguageManager.Get("btn_delete");
            if (btnSearchStrip != null) btnSearchStrip.Text = LanguageManager.Get("btn_search");
            if (btnExportStrip != null) btnExportStrip.Text = LanguageManager.Get("btn_export");
            if (btnExitStrip != null) btnExitStrip.Text = LanguageManager.Get("btn_exit");
            if (btnLoadStatementStrip != null) btnLoadStatementStrip.Text = "📥 تحميل البيان";
            if (btnMovementsStrip != null) btnMovementsStrip.Text = "📜 السجل";
            if (btnPrintReportStrip != null) btnPrintReportStrip.Text = "🖨️ اصدار ";
            if (btnDocumentReviewStrip != null) btnDocumentReviewStrip.Text = "📋 مراجعة الوثائق";
            UpdateGridHeaders();
        }

        private void RefreshAllTranslations()
        {
            ApplyTranslations();
            RefreshGrid();
            UpdateTotalAmount();
        }

        private void UpdateGridHeaders()
        {
            if (dgvData == null || dgvData.Columns.Count == 0) return;
            dgvData.Columns["colId"].HeaderText = LanguageManager.Get("col_id");
            dgvData.Columns["colProductName"].HeaderText = LanguageManager.Get("col_product_name");
            dgvData.Columns["colUnit"].HeaderText = "وحدة الصنف";
            dgvData.Columns["colWeightUnit"].HeaderText = "وحدة الوزن";
            dgvData.Columns["colQuantity"].HeaderText = LanguageManager.Get("col_quantity");
            dgvData.Columns["colTotalWeight"].HeaderText = "إجمالي الوزن";
            // ✅ تم حذف colCountryOfOrigin من هنا
            dgvData.Columns["colAgriculturePermit"].HeaderText = "تصريح الزراعة";
            dgvData.Columns["colPaymentRelease"].HeaderText = "إفراج المدفوعات";
            dgvData.Columns["colCustomsFeeTotal"].HeaderText = "الرسوم الجمركية الإجمالية";
            dgvData.Columns["colSTTotal"].HeaderText = "ض/المبيعات الإجمالي";
            dgvData.Columns["colVATTotal"].HeaderText = "ض/القيمة المضافة الإجمالي";
            dgvData.Columns["colPTTotal"].HeaderText = "ض/الأرباح الإجمالي";
            dgvData.Columns["colVFTotal"].HeaderText = "التحسين الإجمالي";
            dgvData.Columns["colTEFTotal"].HeaderText = "المعلم الإجمالي";
            dgvData.Columns["colMFFTotal"].HeaderText = "الشهيد الإجمالي";
            dgvData.Columns["colOther"].HeaderText = "أخرى";
            dgvData.Columns["colTotal"].HeaderText = "الإجمالي";
        }

        private void LoadPortsFromDatabase()
        {
            try
            {
                allPorts.Clear();
                cmbPortName.Items.Clear();
                cmbPortName.Items.Add("-- اختر منفذ --");
                var allowedPorts = UserManager.GetAllowedPorts("View");
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName, Classification FROM Ports ORDER BY PortName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string portName = row["PortName"].ToString();
                        if (!UserManager.IsAdmin() && !allowedPorts.Contains(portName)) continue;
                        allPorts.Add(new PortsForm.PortData { Name = portName, Classification = row["Classification"].ToString() });
                        cmbPortName.Items.Add(portName);
                    }
                }
                if (cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = 0;
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل المنافذ: {ex.Message}"); }
        }

        private void LoadItemsDataFromDatabase()
        {
            try
            {
                allItemsData.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM ItemsPricingData ORDER BY ProductName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allItemsData.Add(new ItemsDataForm.ItemsRecord
                        {
                            ProductName = row["ProductName"]?.ToString(),
                            PortGroup = row["PortClassification"]?.ToString(), // ✅ تم التعديل: PortGroup بدلاً من PortClassification
                            Unit = row["Unit"]?.ToString(),
                            WeightUnit = row["WeightUnit"]?.ToString(),
                            Weight = row["Weight"] != DBNull.Value ? Convert.ToDecimal(row["Weight"]) : 0,
                            TotalCustoms = row["TotalCustoms"] != DBNull.Value ? Convert.ToDecimal(row["TotalCustoms"]) : 0,
                            TotalST = row["TotalST"] != DBNull.Value ? Convert.ToDecimal(row["TotalST"]) : 0,
                            TotalVAT = row["TotalVAT"] != DBNull.Value ? Convert.ToDecimal(row["TotalVAT"]) : 0,
                            TotalPT = row["TotalPT"] != DBNull.Value ? Convert.ToDecimal(row["TotalPT"]) : 0,
                            TotalVF = row["TotalVF"] != DBNull.Value ? Convert.ToDecimal(row["TotalVF"]) : 0,
                            TotalTEF = row["TotalTEF"] != DBNull.Value ? Convert.ToDecimal(row["TotalTEF"]) : 0,
                            TotalMFF = row["TotalMFF"] != DBNull.Value ? Convert.ToDecimal(row["TotalMFF"]) : 0
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل الأصناف: {ex.Message}"); }
        }
        private void LoadMovementsFromDatabase()
        {
            try
            {
                allMovements.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM StockMovements ORDER BY TransferDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allMovements.Add(new StockTransferForm.StockMovementRecord
                        {
                            Id = row["Id"] != DBNull.Value ? Convert.ToInt32(row["Id"]) : 0,
                            StatementNo = row["StatementNo"]?.ToString(),
                            ProductName = row["ProductName"]?.ToString(),
                            TransferQty = row["TransferQty"] != DBNull.Value ? Convert.ToDecimal(row["TransferQty"]) : 0,
                            FromLocation = row["FromLocation"]?.ToString(),
                            ToLocation = row["ToLocation"]?.ToString(),
                            TransferDate = row["TransferDate"] != DBNull.Value ? Convert.ToDateTime(row["TransferDate"]) : DateTime.Now,
                            TransferNumber = row["TransferNumber"]?.ToString(),
                            Notes = row["Notes"]?.ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل الحركات: {ex.Message}"); }
        }

        private void InitializeGrid()
        {
            if (dgvData == null) return;
            dgvData.Columns.Clear();

            // ✅ الترتيب الجديد للأعمدة (تم حذف بلد المنشأ ونقل التصاريح بعد إجمالي الوزن)
            dgvData.Columns.Add("colId", LanguageManager.Get("col_id"));
            dgvData.Columns.Add("colProductName", LanguageManager.Get("col_product_name"));
            dgvData.Columns.Add("colUnit", "وحدة الصنف");
            dgvData.Columns.Add("colWeightUnit", "وحدة الوزن");
            dgvData.Columns.Add("colQuantity", LanguageManager.Get("col_quantity"));
            dgvData.Columns.Add("colTotalWeight", "إجمالي الوزن");
            dgvData.Columns.Add("colAgriculturePermit", "تصريح الزراعة");
            dgvData.Columns.Add("colPaymentRelease", "إفراج المدفوعات");
            dgvData.Columns.Add("colCustomsFeeTotal", "الرسوم الجمركية الإجمالية");
            dgvData.Columns.Add("colSTTotal", "ض/المبيعات الإجمالي");
            dgvData.Columns.Add("colVATTotal", "ض/القيمة المضافة الإجمالي");
            dgvData.Columns.Add("colPTTotal", "ض/الأرباح الإجمالي");
            dgvData.Columns.Add("colVFTotal", "التحسين الإجمالي");
            dgvData.Columns.Add("colTEFTotal", "المعلم الإجمالي");
            dgvData.Columns.Add("colMFFTotal", "الشهيد الإجمالي");
            dgvData.Columns.Add("colOther", "أخرى");
            dgvData.Columns.Add("colTotal", "الإجمالي");

            // الأعمدة المخفية (قيمة الوحدة + وزن الوحدة)
            dgvData.Columns.Add("colCustomsFeeUnit", "قيمة الوحدة - رسوم");
            dgvData.Columns.Add("colSTUnit", "قيمة الوحدة - ST");
            dgvData.Columns.Add("colVATUnit", "قيمة الوحدة - VAT");
            dgvData.Columns.Add("colPTUnit", "قيمة الوحدة - PT");
            dgvData.Columns.Add("colVFUnit", "قيمة الوحدة - VF");
            dgvData.Columns.Add("colTEFUnit", "قيمة الوحدة - TEF");
            dgvData.Columns.Add("colMFFUnit", "قيمة الوحدة - MFF");
            dgvData.Columns.Add("colUnitWeight", "وزن الوحدة");

            // إخفاء أعمدة قيمة الوحدة ووزن الوحدة
            for (int i = dgvData.Columns.Count - 8; i < dgvData.Columns.Count; i++)
                dgvData.Columns[i].Visible = false;

            SetDefaultLockedColumns();

            // تنسيق الأعمدة
            if (dgvData.Columns["colUnit"] != null)
            {
                dgvData.Columns["colUnit"].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
                dgvData.Columns["colUnit"].DefaultCellStyle.ForeColor = Color.DarkOrange;
                dgvData.Columns["colUnit"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            if (dgvData.Columns["colWeightUnit"] != null)
            {
                dgvData.Columns["colWeightUnit"].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                dgvData.Columns["colWeightUnit"].DefaultCellStyle.ForeColor = Color.DarkGreen;
                dgvData.Columns["colWeightUnit"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            if (dgvData.Columns["colTotalWeight"] != null)
            {
                dgvData.Columns["colTotalWeight"].DefaultCellStyle.BackColor = Color.FromArgb(220, 240, 255);
                dgvData.Columns["colTotalWeight"].DefaultCellStyle.ForeColor = Color.DarkBlue;
                dgvData.Columns["colTotalWeight"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                if (col.Name.Contains("Quantity") || col.Name.Contains("Fee") || col.Name.Contains("Total") ||
                    col.Name.Contains("Permit") || col.Name.Contains("Release") || col.Name == "colOther" || col.Name == "colTotalWeight")
                {
                    col.DefaultCellStyle.Format = "N2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            if (dgvData.Columns["colTotal"] != null)
            {
                dgvData.Columns["colTotal"].DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                dgvData.Columns["colTotal"].DefaultCellStyle.ForeColor = Color.DarkRed;
                dgvData.Columns["colTotal"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            if (dgvData.Columns["colOther"] != null)
            {
                dgvData.Columns["colOther"].DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 238);
                dgvData.Columns["colOther"].DefaultCellStyle.ForeColor = Color.DarkOrange;
                dgvData.Columns["colOther"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            if (dgvData.Columns["colAgriculturePermit"] != null)
            {
                dgvData.Columns["colAgriculturePermit"].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                dgvData.Columns["colAgriculturePermit"].DefaultCellStyle.ForeColor = Color.DarkGreen;
                dgvData.Columns["colAgriculturePermit"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }
            if (dgvData.Columns["colPaymentRelease"] != null)
            {
                dgvData.Columns["colPaymentRelease"].DefaultCellStyle.BackColor = Color.FromArgb(220, 230, 255);
                dgvData.Columns["colPaymentRelease"].DefaultCellStyle.ForeColor = Color.DarkBlue;
                dgvData.Columns["colPaymentRelease"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            dgvData.AllowUserToAddRows = true;
            dgvData.AllowUserToDeleteRows = true;
            dgvData.RightToLeft = LanguageManager.RightToLeft;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvData.ScrollBars = ScrollBars.Both;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.Fixed3D;
            dgvData.GridColor = Color.FromArgb(220, 220, 220);
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.ColumnHeadersHeight = 50;
            dgvData.RowsDefaultCellStyle.BackColor = Color.White;
            dgvData.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvData.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvData.RowTemplate.Height = 35;
            dgvData.CellValueChanged += DgvData_CellValueChanged;
            dgvData.RowsAdded += DgvData_RowsAdded;
            dgvData.RowsRemoved += DgvData_RowsRemoved;
        }

        private void SetDefaultLockedColumns()
        {
            foreach (DataGridViewColumn col in dgvData.Columns)
                col.ReadOnly = true;
        }

        private void SetAllEditableColumns()
        {
            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                if (!col.Visible) continue;
                // ✅ تم إزالة colCountryOfOrigin من قائمة الأعمدة غير القابلة للتعديل
                if (col.Name == "colId" || col.Name == "colProductName" || col.Name == "colUnit" ||
                    col.Name == "colWeightUnit" || col.Name == "colTotal" || col.Name == "colTotalWeight" ||
                    col.Name == "colAgriculturePermit" || col.Name == "colPaymentRelease")
                {
                    col.ReadOnly = true;
                }
                else
                {
                    col.ReadOnly = false;
                    col.DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 224);
                }
            }
        }

        private void TrackChanges() { if (isEditingMode) hasChanges = true; }

        private bool CheckUnsavedChanges()
        {
            if (isEditingMode && hasChanges)
            {
                DialogResult result = MessageBox.Show(
                    "⚠️ لديك تغييرات غير محفوظة!\n" +
                    "• اضغط 'نعم' لحفظ التغييرات\n" +
                    "• اضغط 'لا' للتجاهل والخروج\n" +
                    "• اضغط 'إلغاء' للبقاء والتعديل",
                    "تأكيد الخروج", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    btnSave_Click(this, EventArgs.Empty);
                    return !hasChanges;
                }
                else if (result == DialogResult.Cancel)
                    return false;
                hasChanges = false;
            }
            return true;
        }

        private void DgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dgvData.Rows[e.RowIndex];
                string columnName = dgvData.Columns[e.ColumnIndex].Name;

                if (columnName == "colQuantity")
                {
                    CalculateAllTotals(e.RowIndex);
                    CalculateTotalWeight(e.RowIndex);
                }
                else if (columnName == "colCustomsFeeTotal" || columnName == "colSTTotal" ||
                         columnName == "colVATTotal" || columnName == "colPTTotal" ||
                         columnName == "colVFTotal" || columnName == "colTEFTotal" ||
                         columnName == "colMFFTotal" || columnName == "colOther" ||
                         columnName == "colAgriculturePermit" || columnName == "colPaymentRelease")
                {
                    CalculateTotal(e.RowIndex);
                }

                UpdateTotalFees();
                UpdateTotalAmount();
                TrackChanges();
            }
        }

        private void DgvData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < e.RowCount; i++)
            {
                CalculateAllTotals(e.RowIndex + i);
                CalculateTotalWeight(e.RowIndex + i);
            }
            UpdateTotalFees();
            UpdateTotalAmount();
        }

        private void DgvData_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            UpdateTotalFees();
            UpdateTotalAmount();
            TrackChanges();
        }

        private void CalculateAllTotals(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.IsNewRow) return;

            string productName = row.Cells["colProductName"].Value?.ToString();
            decimal quantity = ParseDecimal(row.Cells["colQuantity"].Value?.ToString());
            var itemData = allItemsData.FirstOrDefault(i => i.ProductName == productName && i.PortGroup == currentPortClassification); // ✅ PortGroup

            if (itemData != null && quantity > 0)
            {
                row.Cells["colCustomsFeeTotal"].Value = itemData.TotalCustoms * quantity;
                row.Cells["colSTTotal"].Value = itemData.TotalST * quantity;
                row.Cells["colVATTotal"].Value = itemData.TotalVAT * quantity;
                row.Cells["colPTTotal"].Value = itemData.TotalPT * quantity;
                row.Cells["colVFTotal"].Value = itemData.TotalVF * quantity;
                row.Cells["colTEFTotal"].Value = itemData.TotalTEF * quantity;
                row.Cells["colMFFTotal"].Value = itemData.TotalMFF * quantity;
                row.Cells["colCustomsFeeUnit"].Value = itemData.TotalCustoms;
                row.Cells["colSTUnit"].Value = itemData.TotalST;
                row.Cells["colVATUnit"].Value = itemData.TotalVAT;
                row.Cells["colPTUnit"].Value = itemData.TotalPT;
                row.Cells["colVFUnit"].Value = itemData.TotalVF;
                row.Cells["colTEFUnit"].Value = itemData.TotalTEF;
                row.Cells["colMFFUnit"].Value = itemData.TotalMFF;
                row.Cells["colUnitWeight"].Value = itemData.Weight;
            }
            else if (quantity > 0)
            {
                decimal customsFeeUnit = ParseDecimal(row.Cells["colCustomsFeeUnit"].Value?.ToString());
                decimal stUnit = ParseDecimal(row.Cells["colSTUnit"].Value?.ToString());
                decimal vatUnit = ParseDecimal(row.Cells["colVATUnit"].Value?.ToString());
                decimal ptUnit = ParseDecimal(row.Cells["colPTUnit"].Value?.ToString());
                decimal vfUnit = ParseDecimal(row.Cells["colVFUnit"].Value?.ToString());
                decimal tefUnit = ParseDecimal(row.Cells["colTEFUnit"].Value?.ToString());
                decimal mffUnit = ParseDecimal(row.Cells["colMFFUnit"].Value?.ToString());

                row.Cells["colCustomsFeeTotal"].Value = customsFeeUnit * quantity;
                row.Cells["colSTTotal"].Value = stUnit * quantity;
                row.Cells["colVATTotal"].Value = vatUnit * quantity;
                row.Cells["colPTTotal"].Value = ptUnit * quantity;
                row.Cells["colVFTotal"].Value = vfUnit * quantity;
                row.Cells["colTEFTotal"].Value = tefUnit * quantity;
                row.Cells["colMFFTotal"].Value = mffUnit * quantity;
            }
            CalculateTotal(rowIndex);
        }

        private void CalculateTotalWeight(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.IsNewRow) return;

            decimal quantity = ParseDecimal(row.Cells["colQuantity"].Value?.ToString());
            decimal unitWeight = ParseDecimal(row.Cells["colUnitWeight"].Value?.ToString());
            row.Cells["colTotalWeight"].Value = quantity * unitWeight;
        }

        private void CalculateTotal(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            decimal total = ParseDecimal(row.Cells["colCustomsFeeTotal"].Value?.ToString())
                          + ParseDecimal(row.Cells["colSTTotal"].Value?.ToString())
                          + ParseDecimal(row.Cells["colVATTotal"].Value?.ToString())
                          + ParseDecimal(row.Cells["colPTTotal"].Value?.ToString())
                          + ParseDecimal(row.Cells["colVFTotal"].Value?.ToString())
                          + ParseDecimal(row.Cells["colTEFTotal"].Value?.ToString())
                          + ParseDecimal(row.Cells["colMFFTotal"].Value?.ToString())
                          + ParseDecimal(row.Cells["colOther"].Value?.ToString());
            row.Cells["colTotal"].Value = total;
        }

        private void UpdateTotalFees()
        {
            if (dgvData == null || dgvData.Rows.Count == 0)
            {
                SetTotalFeesValue(0);
                return;
            }
            decimal total = 0;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.IsNewRow) continue;
                total += ParseDecimal(row.Cells["colTotal"].Value?.ToString());
            }
            SetTotalFeesValue(total);
        }

        private void SetTotalFeesValue(decimal value)
        {
            if (lblTotalFeesValue == null) return;
            lblTotalFeesValue.Text = value.ToString("N2");
            lblTotalFeesValue.ForeColor = value > 0 ? Color.FromArgb(76, 175, 80) : Color.Gray;
        }

        private void CalculateAdditionalAmounts()
        {
            decimal totalAdditional = 0;
            if (dgvAdditionalAmounts != null)
            {
                foreach (DataGridViewRow row in dgvAdditionalAmounts.Rows)
                {
                    if (row.IsNewRow) continue;
                    decimal amount = ParseDecimal(row.Cells["colAmount"].Value?.ToString());
                    totalAdditional += amount;
                }
            }
            txtOtherFees.Text = totalAdditional.ToString("N2");
            UpdateTotalAmount();
        }

        private void DgvAdditionalAmounts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvAdditionalAmounts.Columns[e.ColumnIndex].Name == "colAmount")
            {
                CalculateAdditionalAmounts();
            }
        }

        private void UpdateTotalAmount()
        {
            if (lblTotalAmountValue == null || lblTotalFeesValue == null) return;
            try
            {
                decimal totalFees = ParseDecimal(lblTotalFeesValue.Text);
                decimal otherFees = ParseDecimal(txtOtherFees?.Text ?? "0");
                decimal totalAmount = totalFees + otherFees;
                lblTotalAmountValue.Text = totalAmount.ToString("N2");
                lblTotalAmountValue.ForeColor = totalAmount > 0 ? Color.White : Color.LightGray;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ في UpdateTotalAmount: {ex.Message}");
            }
        }

        private void SetControlsState(bool enabled)
        {
            isEditingMode = enabled;
            cmbPortName.Enabled = enabled;
            cmbTariffType.Enabled = enabled;
            dtpStatementDate.Enabled = enabled;
            txtStatementNo.Enabled = enabled;
            txtUnloadFrom.Enabled = enabled;
            txtTariffRequestNo.Enabled = enabled;
            numVehicleCount.Enabled = enabled;

            if (dgvDrivers != null) dgvDrivers.ReadOnly = !enabled;
            if (dgvAdditionalAmounts != null) dgvAdditionalAmounts.ReadOnly = !enabled;
            if (dgvData != null)
            {
                dgvData.ReadOnly = !enabled;
                if (!enabled) SetDefaultLockedColumns();
            }
        }

        private void LoadDataFromDatabase()
        {
            records.Clear();
            try
            {
                var allowedPorts = UserManager.GetAllowedPorts("View");
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM CustomsClearance ORDER BY StatementDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string portName = row["PortName"]?.ToString();
                        if (!UserManager.IsAdmin() && !allowedPorts.Contains(portName)) continue;

                        records.Add(new ClearanceRecord
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            PortName = portName,
                            StatementDate = row["StatementDate"] != DBNull.Value ? Convert.ToDateTime(row["StatementDate"]) : DateTime.Now,
                            StatementNo = row["StatementNo"]?.ToString(),
                            TariffType = row["TariffType"]?.ToString(),
                            UnloadFrom = row["UnloadFrom"]?.ToString(),
                            TariffRequestNo = row["TariffRequestNo"]?.ToString(),
                            ClientName = row["ClientName"]?.ToString(),
                            CarNo = row["CarNo"]?.ToString(),
                            ProductName = row["ProductName"]?.ToString(),
                            Unit = row["Unit"]?.ToString(),
                            WeightUnit = row["WeightUnit"]?.ToString(),
                            Quantity = row["Quantity"] != DBNull.Value ? Convert.ToDecimal(row["Quantity"]) : 0,
                            Weight = row["Weight"] != DBNull.Value ? Convert.ToDecimal(row["Weight"]) : 0,
                            WeightTotal = row["WeightTotal"] != DBNull.Value ? Convert.ToDecimal(row["WeightTotal"]) : 0,
                            CustomsFeeTotal = row["CustomsFeeTotal"] != DBNull.Value ? Convert.ToDecimal(row["CustomsFeeTotal"]) : 0,
                            STTotal = row["STTotal"] != DBNull.Value ? Convert.ToDecimal(row["STTotal"]) : 0,
                            VATTotal = row["VATTotal"] != DBNull.Value ? Convert.ToDecimal(row["VATTotal"]) : 0,
                            PTTotal = row["PTTotal"] != DBNull.Value ? Convert.ToDecimal(row["PTTotal"]) : 0,
                            VFTotal = row["VFTotal"] != DBNull.Value ? Convert.ToDecimal(row["VFTotal"]) : 0,
                            TEFTotal = row["TEFTotal"] != DBNull.Value ? Convert.ToDecimal(row["TEFTotal"]) : 0,
                            MFFTotal = row["MFFTotal"] != DBNull.Value ? Convert.ToDecimal(row["MFFTotal"]) : 0,
                            Other = row["Other"] != DBNull.Value ? Convert.ToDecimal(row["Other"]) : 0,
                            Total = row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0,
                            AgriculturePermit = row["AgriculturePermit"] != DBNull.Value ? Convert.ToDecimal(row["AgriculturePermit"]) : 0,
                            PaymentRelease = row["PaymentRelease"] != DBNull.Value ? Convert.ToDecimal(row["PaymentRelease"]) : 0,
                            CountryOfOrigin = row["CountryOfOrigin"]?.ToString(),
                            TransferredQty = row["TransferredQty"] != DBNull.Value ? Convert.ToDecimal(row["TransferredQty"]) : 0,
                            RemainingQty = row["RemainingQty"] != DBNull.Value ? Convert.ToDecimal(row["RemainingQty"]) : 0,
                            DriversJson = row["DriversJson"]?.ToString(),
                            AdditionalAmountsJson = row["AdditionalAmountsJson"]?.ToString()
                        });
                    }
                    RefreshGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحميل البيانات:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveDataToDatabase(List<ClearanceRecord> gridRecords)
        {
            try
            {
                foreach (var record in gridRecords)
                {
                    if (record.Id > 0)
                    {
                        DatabaseHelper.ExecuteNonQuery(@"UPDATE CustomsClearance SET
                            PortName = @PortName, StatementDate = @StatementDate, StatementNo = @StatementNo,
                            TariffType = @TariffType, UnloadFrom = @UnloadFrom, TariffRequestNo = @TariffRequestNo,
                            ClientName = @ClientName, CarNo = @CarNo, ProductName = @ProductName, Unit = @Unit, WeightUnit = @WeightUnit,
                            Quantity = @Quantity, Weight = @Weight, WeightTotal = @WeightTotal, CustomsFeeTotal = @CustomsFeeTotal,
                            STTotal = @STTotal, VATTotal = @VATTotal, PTTotal = @PTTotal, VFTotal = @VFTotal, TEFTotal = @TEFTotal, MFFTotal = @MFFTotal,
                            Other = @Other, Total = @Total, AgriculturePermit = @AgriculturePermit, PaymentRelease = @PaymentRelease,
                            CountryOfOrigin = @CountryOfOrigin,
                            TransferredQty = @TransferredQty, RemainingQty = @RemainingQty,
                            DriversJson = @DriversJson, AdditionalAmountsJson = @AdditionalAmountsJson,
                            ModifiedDate = datetime('now')
                            WHERE Id = @Id",
                            DatabaseHelper.CreateParameter("@PortName", record.PortName),
                            DatabaseHelper.CreateParameter("@StatementDate", record.StatementDate),
                            DatabaseHelper.CreateParameter("@StatementNo", record.StatementNo),
                            DatabaseHelper.CreateParameter("@TariffType", record.TariffType ?? ""),
                            DatabaseHelper.CreateParameter("@UnloadFrom", record.UnloadFrom ?? ""),
                            DatabaseHelper.CreateParameter("@TariffRequestNo", record.TariffRequestNo ?? ""),
                            DatabaseHelper.CreateParameter("@ClientName", record.ClientName),
                            DatabaseHelper.CreateParameter("@CarNo", record.CarNo),
                            DatabaseHelper.CreateParameter("@ProductName", record.ProductName),
                            DatabaseHelper.CreateParameter("@Unit", record.Unit),
                            DatabaseHelper.CreateParameter("@WeightUnit", record.WeightUnit),
                            DatabaseHelper.CreateParameter("@Quantity", record.Quantity),
                            DatabaseHelper.CreateParameter("@Weight", record.Weight),
                            DatabaseHelper.CreateParameter("@WeightTotal", record.WeightTotal),
                            DatabaseHelper.CreateParameter("@CustomsFeeTotal", record.CustomsFeeTotal),
                            DatabaseHelper.CreateParameter("@STTotal", record.STTotal),
                            DatabaseHelper.CreateParameter("@VATTotal", record.VATTotal),
                            DatabaseHelper.CreateParameter("@PTTotal", record.PTTotal),
                            DatabaseHelper.CreateParameter("@VFTotal", record.VFTotal),
                            DatabaseHelper.CreateParameter("@TEFTotal", record.TEFTotal),
                            DatabaseHelper.CreateParameter("@MFFTotal", record.MFFTotal),
                            DatabaseHelper.CreateParameter("@Other", record.Other),
                            DatabaseHelper.CreateParameter("@Total", record.Total),
                            DatabaseHelper.CreateParameter("@AgriculturePermit", record.AgriculturePermit),
                            DatabaseHelper.CreateParameter("@PaymentRelease", record.PaymentRelease),
                            DatabaseHelper.CreateParameter("@CountryOfOrigin", record.CountryOfOrigin ?? ""),
                            DatabaseHelper.CreateParameter("@TransferredQty", record.TransferredQty),
                            DatabaseHelper.CreateParameter("@RemainingQty", record.RemainingQty),
                            DatabaseHelper.CreateParameter("@DriversJson", (object)record.DriversJson ?? DBNull.Value),
                            DatabaseHelper.CreateParameter("@AdditionalAmountsJson", (object)record.AdditionalAmountsJson ?? DBNull.Value),
                            DatabaseHelper.CreateParameter("@Id", record.Id)
                        );
                    }
                    else
                    {
                        var newId = DatabaseHelper.ExecuteScalar(@"INSERT INTO CustomsClearance
                            (PortName, StatementDate, StatementNo, TariffType, UnloadFrom, TariffRequestNo, ClientName, CarNo,
                            ProductName, Unit, WeightUnit, Quantity, Weight, WeightTotal, CustomsFeeTotal, STTotal, VATTotal, PTTotal, VFTotal, TEFTotal, MFFTotal,
                            Other, Total, AgriculturePermit, PaymentRelease, CountryOfOrigin, TransferredQty, RemainingQty,
                            DriversJson, AdditionalAmountsJson, CreatedDate, ModifiedDate)
                            VALUES (@PortName, @StatementDate, @StatementNo, @TariffType, @UnloadFrom, @TariffRequestNo, @ClientName, @CarNo,
                            @ProductName, @Unit, @WeightUnit, @Quantity, @Weight, @WeightTotal, @CustomsFeeTotal, @STTotal, @VATTotal, @PTTotal, @VFTotal, @TEFTotal, @MFFTotal,
                            @Other, @Total, @AgriculturePermit, @PaymentRelease, @CountryOfOrigin, @TransferredQty, @RemainingQty,
                            @DriversJson, @AdditionalAmountsJson, datetime('now'), datetime('now'));
                            SELECT last_insert_rowid();",
                            DatabaseHelper.CreateParameter("@PortName", record.PortName),
                            DatabaseHelper.CreateParameter("@StatementDate", record.StatementDate),
                            DatabaseHelper.CreateParameter("@StatementNo", record.StatementNo),
                            DatabaseHelper.CreateParameter("@TariffType", record.TariffType ?? ""),
                            DatabaseHelper.CreateParameter("@UnloadFrom", record.UnloadFrom ?? ""),
                            DatabaseHelper.CreateParameter("@TariffRequestNo", record.TariffRequestNo ?? ""),
                            DatabaseHelper.CreateParameter("@ClientName", record.ClientName),
                            DatabaseHelper.CreateParameter("@CarNo", record.CarNo),
                            DatabaseHelper.CreateParameter("@ProductName", record.ProductName),
                            DatabaseHelper.CreateParameter("@Unit", record.Unit),
                            DatabaseHelper.CreateParameter("@WeightUnit", record.WeightUnit),
                            DatabaseHelper.CreateParameter("@Quantity", record.Quantity),
                            DatabaseHelper.CreateParameter("@Weight", record.Weight),
                            DatabaseHelper.CreateParameter("@WeightTotal", record.WeightTotal),
                            DatabaseHelper.CreateParameter("@CustomsFeeTotal", record.CustomsFeeTotal),
                            DatabaseHelper.CreateParameter("@STTotal", record.STTotal),
                            DatabaseHelper.CreateParameter("@VATTotal", record.VATTotal),
                            DatabaseHelper.CreateParameter("@PTTotal", record.PTTotal),
                            DatabaseHelper.CreateParameter("@VFTotal", record.VFTotal),
                            DatabaseHelper.CreateParameter("@TEFTotal", record.TEFTotal),
                            DatabaseHelper.CreateParameter("@MFFTotal", record.MFFTotal),
                            DatabaseHelper.CreateParameter("@Other", record.Other),
                            DatabaseHelper.CreateParameter("@Total", record.Total),
                            DatabaseHelper.CreateParameter("@AgriculturePermit", record.AgriculturePermit),
                            DatabaseHelper.CreateParameter("@PaymentRelease", record.PaymentRelease),
                            DatabaseHelper.CreateParameter("@CountryOfOrigin", record.CountryOfOrigin ?? ""),
                            DatabaseHelper.CreateParameter("@TransferredQty", record.TransferredQty),
                            DatabaseHelper.CreateParameter("@RemainingQty", record.RemainingQty),
                            DatabaseHelper.CreateParameter("@DriversJson", (object)record.DriversJson ?? DBNull.Value),
                            DatabaseHelper.CreateParameter("@AdditionalAmountsJson", (object)record.AdditionalAmountsJson ?? DBNull.Value)
                        );
                        record.Id = Convert.ToInt32(newId);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في حفظ البيانات: {ex.Message}");
            }
        }

        private void DeleteDataFromDatabase(string statementNo, string portName)
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM CustomsClearance WHERE StatementNo = @StatementNo AND PortName = @PortName",
                DatabaseHelper.CreateParameter("@StatementNo", statementNo),
                DatabaseHelper.CreateParameter("@PortName", portName));
        }

        private void LoadStatement()
        {
            string statementNo = txtStatementNo.Text.Trim();
            if (string.IsNullOrWhiteSpace(statementNo))
            {
                MessageBox.Show(LanguageManager.Get("msg_enter_statement_no"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var existingRecords = records.Where(r => r.StatementNo == statementNo).ToList();
            if (existingRecords.Count == 0)
            {
                MessageBox.Show(string.Format(LanguageManager.Get("msg_no_data_for_statement"), statementNo), LanguageManager.Get("btn_search"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string portName = existingRecords.First().PortName;
            if (!UserManager.CanPort(portName, "Edit"))
            {
                MessageBox.Show($"🚫 ليس لديك صلاحية تعديل البيانات في منفذ '{portName}'", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            currentStatementNo = statementNo;
            LoadRecordToForm(existingRecords.First());
            SetControlsState(true);
            SetAllEditableColumns();
            isEditingExisting = true;
            hasChanges = false;
            MessageBox.Show(string.Format(LanguageManager.Get("msg_statement_loaded"), statementNo) + "\n✅ يمكنك الآن التعديل على البيانات", LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowMovements()
        {
            string statementNo = txtStatementNo.Text.Trim();
            if (string.IsNullOrWhiteSpace(statementNo))
            {
                MessageBox.Show(LanguageManager.Get("msg_enter_statement_first"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var movements = allMovements.Where(m => m.StatementNo == statementNo).ToList();
            if (movements.Count == 0)
            {
                MessageBox.Show(string.Format(LanguageManager.Get("msg_no_movements"), statementNo), LanguageManager.Get("msg_movements_log"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Form movementsForm = new Form();
            movementsForm.Text = string.Format(LanguageManager.Get("movements_form_title"), statementNo);
            movementsForm.Size = new Size(900, 500);
            movementsForm.StartPosition = FormStartPosition.CenterParent;
            movementsForm.RightToLeft = LanguageManager.RightToLeft;
            movementsForm.WindowState = FormWindowState.Maximized;

            DataGridView dgvMovements = new DataGridView();
            dgvMovements.Dock = DockStyle.Fill;
            dgvMovements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvMovements.RightToLeft = LanguageManager.RightToLeft;
            dgvMovements.ReadOnly = true;
            dgvMovements.AllowUserToAddRows = false;
            dgvMovements.Columns.Add("colDate", LanguageManager.Get("col_date"));
            dgvMovements.Columns.Add("colProduct", LanguageManager.Get("col_product"));
            dgvMovements.Columns.Add("colQty", LanguageManager.Get("col_qty"));
            dgvMovements.Columns.Add("colFrom", LanguageManager.Get("col_from"));
            dgvMovements.Columns.Add("colTo", LanguageManager.Get("col_to"));
            dgvMovements.Columns.Add("colTransferNo", LanguageManager.Get("col_transfer_no"));
            dgvMovements.Columns.Add("colNotes", LanguageManager.Get("col_notes"));

            foreach (var m in movements.OrderByDescending(m => m.TransferDate))
            {
                dgvMovements.Rows.Add(
                    m.TransferDate.ToString("yyyy-MM-dd HH:mm"),
                    m.ProductName,
                    m.TransferQty.ToString("N2"),
                    m.FromLocation,
                    m.ToLocation,
                    m.TransferNumber,
                    m.Notes
                );
            }
            movementsForm.Controls.Add(dgvMovements);
            movementsForm.ShowDialog();
        }

        private void OpenDocumentReviewForm()
        {
            string statementNo = txtStatementNo.Text.Trim();
            string portName = cmbPortName.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(statementNo) || portName == "-- اختر منفذ --")
            {
                MessageBox.Show("⚠️ الرجاء اختيار المنفذ وإدخال رقم البيان أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var reviewForm = new DocumentReviewForm(statementNo, portName))
            {
                reviewForm.ShowDialog(this);
            }
        }

        private void PrintMovementsReport()
        {
            string portName = cmbPortName.SelectedItem?.ToString() ?? "";
            if (!string.IsNullOrEmpty(portName) && portName != "-- اختر منفذ --" && !UserManager.CanPort(portName, "Print"))
            {
                MessageBox.Show($"🚫 ليس لديك صلاحية الطباعة في منفذ '{portName}'", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string statementNo = txtStatementNo.Text.Trim();
            if (string.IsNullOrWhiteSpace(statementNo))
            {
                MessageBox.Show(LanguageManager.Get("msg_enter_statement_first"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            var clearanceRecords = records.Where(r => r.StatementNo == statementNo && r.PortName == portName).ToList();
            if (clearanceRecords.Count == 0)
            {
                MessageBox.Show($"لا توجد بيانات للطباعة للبيان رقم: {statementNo}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            PrintDocument printDoc = new PrintDocument();
            printDoc.DocumentName = $"Sind_{statementNo}";
            printDoc.DefaultPageSettings.PaperSize = new PaperSize("A4", 2000, 1400);
            printDoc.DefaultPageSettings.Landscape = true;
            printDoc.DefaultPageSettings.Margins = new Margins(50, 50, 50, 50);

            string clientName = txtUnloadFrom.Text.Trim();
            string tariffRequestNo = txtTariffRequestNo.Text.Trim();
            string tariffType = cmbTariffType.Text.Trim();
            DateTime statementDate = dtpStatementDate.Value;

            Image logoImage = null;
            try
            {
                string logoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dadih_logo.png");
                if (File.Exists(logoPath)) logoImage = Image.FromFile(logoPath);
            }
            catch { }

            Font headerFont = new Font("Arial", 13, FontStyle.Bold);
            Font subHeaderFont = new Font("Arial", 8, FontStyle.Bold);
            Font normalFont = new Font("Arial", 7.5f, FontStyle.Regular);
            Font titleFont = new Font("Arial", 11, FontStyle.Bold);
            Font smallFont = new Font("Arial", 6.5f, FontStyle.Regular);

            printDoc.PrintPage += (sender, e) =>
            {
                float yPos = e.MarginBounds.Top;
                float leftMargin = e.MarginBounds.Left;
                float rightMargin = e.MarginBounds.Right;
                float bottomMargin = e.MarginBounds.Bottom;
                float centerX = (leftMargin + rightMargin) / 2;

                float logoWidth = 120;
                float logoHeight = 80;
                if (logoImage != null)
                {
                    e.Graphics.DrawImage(logoImage, rightMargin - logoWidth, yPos, logoWidth, logoHeight);
                }

                string companyNameAr = "داحيه للتجارة العامة";
                string companyNameEn = "DADIH GENERAL TRADING";
                SizeF sizeAr = e.Graphics.MeasureString(companyNameAr, titleFont);
                SizeF sizeEn = e.Graphics.MeasureString(companyNameEn, subHeaderFont);
                e.Graphics.DrawString(companyNameAr, titleFont, Brushes.DarkGreen, centerX - sizeAr.Width / 2, yPos + 10);
                e.Graphics.DrawString(companyNameEn, subHeaderFont, Brushes.DarkGreen, centerX - sizeEn.Width / 2, yPos + 40);

                string docTitle = $"سند تخليص جمركي\nبيان رقم: {statementNo}";
                SizeF titleSize = e.Graphics.MeasureString(docTitle, headerFont);
                e.Graphics.DrawString(docTitle, headerFont, Brushes.DarkBlue, leftMargin, yPos + 10);
                yPos += logoHeight + 20;
                e.Graphics.DrawLine(Pens.DarkBlue, leftMargin, yPos, rightMargin, yPos);
                yPos += 15;

                float infoColWidth = (rightMargin - leftMargin) / 4;
                e.Graphics.DrawString($"اسم المنفذ: {portName}", normalFont, Brushes.Black, rightMargin - infoColWidth, yPos);
                e.Graphics.DrawString($"تاريخ البيان: {statementDate:yyyy-MM-dd}", normalFont, Brushes.Black, rightMargin - (infoColWidth * 2), yPos);
                e.Graphics.DrawString($"نوع الترسيم: {tariffType}", normalFont, Brushes.Black, rightMargin - (infoColWidth * 3), yPos);
                e.Graphics.DrawString($"إنزال من: {clientName}", normalFont, Brushes.Black, rightMargin - (infoColWidth * 4), yPos);
                yPos += 25;
                e.Graphics.DrawString($"رقم طلب الترسيم: {tariffRequestNo}", normalFont, Brushes.Black, rightMargin - infoColWidth, yPos);
                yPos += 25;

                float totalWidth = rightMargin - leftMargin;
                float rowHeight = 30;
                float headerHeight = 45;
                float[] columnRatios = { 0.08f, 0.05f, 0.05f, 0.05f, 0.05f, 0.07f, 0.05f, 0.07f, 0.06f, 0.06f, 0.05f, 0.06f, 0.12f, 0.11f, 0.03f };
                float[] colWidths = new float[columnRatios.Length];
                for (int i = 0; i < colWidths.Length; i++) colWidths[i] = totalWidth * columnRatios[i];

                string[] headers = { "الإجمالي", "الشهيد", "المعلم", "التحسين", "ض/الأرباح", "ض/القيمة المضافة", "ض/المبيعات", "الرسوم الجمركية", "إلى", "من", "الكمية", "الوحدة", "اسم السلعة", "تاريخ التحويل", "م" };
                float tableStartX = leftMargin;
                float totalTableWidth = totalWidth;

                e.Graphics.FillRectangle(Brushes.LightBlue, tableStartX, yPos, totalTableWidth, headerHeight);
                e.Graphics.DrawRectangle(Pens.Black, tableStartX, yPos, totalTableWidth, headerHeight);
                float lineX = tableStartX;
                for (int i = 0; i < colWidths.Length - 1; i++)
                {
                    lineX += colWidths[i];
                    e.Graphics.DrawLine(Pens.DarkBlue, lineX, yPos, lineX, yPos + headerHeight);
                }
                float xPos = tableStartX;
                for (int i = 0; i < headers.Length; i++)
                {
                    RectangleF rect = new RectangleF(xPos, yPos, colWidths[i], headerHeight);
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString(headers[i], subHeaderFont, Brushes.DarkBlue, rect, sf);
                    xPos += colWidths[i];
                }
                yPos += headerHeight;

                int rowNum = 1;
                decimal totalCustoms = 0, totalST = 0, totalVAT = 0, totalPT = 0, totalVF = 0, totalTEF = 0, totalMFF = 0, totalAmount = 0, totalQty = 0;

                foreach (var record in clearanceRecords)
                {
                    if (rowNum % 2 == 0) e.Graphics.FillRectangle(Brushes.LightGray, tableStartX, yPos, totalTableWidth, rowHeight);
                    e.Graphics.DrawRectangle(Pens.Black, tableStartX, yPos, totalTableWidth, rowHeight);
                    lineX = tableStartX;
                    for (int i = 0; i < colWidths.Length - 1; i++)
                    {
                        lineX += colWidths[i];
                        e.Graphics.DrawLine(Pens.Gray, lineX, yPos, lineX, yPos + rowHeight);
                    }

                    decimal customsFee = record.CustomsFeeTotal;
                    decimal st = record.STTotal;
                    decimal vat = record.VATTotal;
                    decimal pt = record.PTTotal;
                    decimal vf = record.VFTotal;
                    decimal tef = record.TEFTotal;
                    decimal mff = record.MFFTotal;
                    decimal total = record.Total;

                    string[] rowData = {
                        total.ToString("N2"), mff.ToString("N2"), tef.ToString("N2"), vf.ToString("N2"),
                        pt.ToString("N2"), vat.ToString("N2"), st.ToString("N2"), customsFee.ToString("N2"),
                        "", "", record.Quantity.ToString("N0"), record.Unit ?? "", record.ProductName ?? "", statementDate.ToString("yyyy-MM-dd"), rowNum.ToString()
                    };

                    xPos = tableStartX;
                    for (int i = 0; i < rowData.Length; i++)
                    {
                        RectangleF rect = new RectangleF(xPos, yPos, colWidths[i], rowHeight);
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        if (i == 8 || i == 9 || i == 11 || i == 12 || i == 13) sf.Alignment = StringAlignment.Near;
                        e.Graphics.DrawString(rowData[i], normalFont, Brushes.Black, rect, sf);
                        xPos += colWidths[i];
                    }

                    totalCustoms += customsFee;
                    totalST += st;
                    totalVAT += vat;
                    totalPT += pt;
                    totalVF += vf;
                    totalTEF += tef;
                    totalMFF += mff;
                    totalAmount += total;
                    totalQty += record.Quantity;
                    yPos += rowHeight;
                    rowNum++;
                }

                e.Graphics.FillRectangle(Brushes.LightGreen, tableStartX, yPos, totalTableWidth, rowHeight);
                e.Graphics.DrawRectangle(Pens.DarkGreen, tableStartX, yPos, totalTableWidth, rowHeight);
                lineX = tableStartX;
                for (int i = 0; i < colWidths.Length - 1; i++)
                {
                    lineX += colWidths[i];
                    e.Graphics.DrawLine(Pens.DarkGreen, lineX, yPos, lineX, yPos + rowHeight);
                }

                string[] totalRowData = {
                    totalAmount.ToString("N2"), totalMFF.ToString("N2"), totalTEF.ToString("N2"), totalVF.ToString("N2"),
                    totalPT.ToString("N2"), totalVAT.ToString("N2"), totalST.ToString("N2"), totalCustoms.ToString("N2"),
                    "", "", totalQty.ToString("N0"), "", "الإجمالي الكلي", "", ""
                };

                xPos = tableStartX;
                Font boldFont = new Font("Arial", 8, FontStyle.Bold);
                for (int i = 0; i < totalRowData.Length; i++)
                {
                    RectangleF rect = new RectangleF(xPos, yPos, colWidths[i], rowHeight);
                    StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    Font cellFont = (i == 12) ? boldFont : normalFont;
                    Brush cellBrush = (i == 12) ? Brushes.DarkGreen : Brushes.Black;
                    e.Graphics.DrawString(totalRowData[i], cellFont, cellBrush, rect, sf);
                    xPos += colWidths[i];
                }
                yPos += rowHeight + 25;

                List<DriverInfo> drivers = new List<DriverInfo>();
                if (!string.IsNullOrEmpty(clearanceRecords.First().DriversJson))
                {
                    try { drivers = JsonSerializer.Deserialize<List<DriverInfo>>(clearanceRecords.First().DriversJson); } catch { }
                }
                if (drivers.Count > 0)
                {
                    e.Graphics.DrawString("📋 بيانات السائقين والمركبات:", titleFont, Brushes.DarkBlue, rightMargin - 250, yPos);
                    yPos += 30;
                    float[] drvColRatios = { 0.1f, 0.4f, 0.5f };
                    float[] drvColWidths = new float[3];
                    for (int i = 0; i < 3; i++) drvColWidths[i] = totalWidth * drvColRatios[i];
                    string[] drvHeaders = { "م", "اسم السائق", "رقم السيارة" };

                    e.Graphics.FillRectangle(Brushes.LightBlue, leftMargin, yPos, totalWidth, 30);
                    e.Graphics.DrawRectangle(Pens.Black, leftMargin, yPos, totalWidth, 30);
                    lineX = leftMargin;
                    for (int i = 0; i < drvColWidths.Length - 1; i++)
                    {
                        lineX += drvColWidths[i];
                        e.Graphics.DrawLine(Pens.DarkBlue, lineX, yPos, lineX, yPos + 30);
                    }
                    xPos = leftMargin;
                    for (int i = 0; i < drvHeaders.Length; i++)
                    {
                        RectangleF rect = new RectangleF(xPos, yPos, drvColWidths[i], 30);
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        e.Graphics.DrawString(drvHeaders[i], subHeaderFont, Brushes.DarkBlue, rect, sf);
                        xPos += drvColWidths[i];
                    }
                    yPos += 30;

                    int drvRowNum = 1;
                    foreach (var driver in drivers)
                    {
                        e.Graphics.DrawRectangle(Pens.Black, leftMargin, yPos, totalWidth, 25);
                        string[] drvRowData = { drvRowNum.ToString(), driver.DriverName ?? "", driver.VehicleNo ?? "" };
                        xPos = leftMargin;
                        for (int i = 0; i < drvRowData.Length; i++)
                        {
                            RectangleF rect = new RectangleF(xPos, yPos, drvColWidths[i], 25);
                            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            if (i == 1) sf.Alignment = StringAlignment.Near;
                            e.Graphics.DrawString(drvRowData[i], normalFont, Brushes.Black, rect, sf);
                            xPos += drvColWidths[i];
                        }
                        yPos += 25;
                        drvRowNum++;
                    }
                    yPos += 20;
                }

                List<AdditionalAmountItem> addAmounts = new List<AdditionalAmountItem>();
                if (!string.IsNullOrEmpty(clearanceRecords.First().AdditionalAmountsJson))
                {
                    try { addAmounts = JsonSerializer.Deserialize<List<AdditionalAmountItem>>(clearanceRecords.First().AdditionalAmountsJson); } catch { }
                }
                if (addAmounts.Count > 0)
                {
                    e.Graphics.DrawString("💰 المبالغ الإضافية:", titleFont, Brushes.DarkBlue, rightMargin - 200, yPos);
                    yPos += 30;
                    float[] addColRatios = { 0.1f, 0.6f, 0.3f };
                    float[] addColWidths = new float[3];
                    for (int i = 0; i < 3; i++) addColWidths[i] = totalWidth * addColRatios[i];
                    string[] addHeaders = { "م", "البند", "المبلغ" };

                    e.Graphics.FillRectangle(Brushes.LightBlue, leftMargin, yPos, totalWidth, 30);
                    e.Graphics.DrawRectangle(Pens.Black, leftMargin, yPos, totalWidth, 30);
                    lineX = leftMargin;
                    for (int i = 0; i < addColWidths.Length - 1; i++)
                    {
                        lineX += addColWidths[i];
                        e.Graphics.DrawLine(Pens.DarkBlue, lineX, yPos, lineX, yPos + 30);
                    }
                    xPos = leftMargin;
                    for (int i = 0; i < addHeaders.Length; i++)
                    {
                        RectangleF rect = new RectangleF(xPos, yPos, addColWidths[i], 30);
                        StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        e.Graphics.DrawString(addHeaders[i], subHeaderFont, Brushes.DarkBlue, rect, sf);
                        xPos += addColWidths[i];
                    }
                    yPos += 30;

                    decimal totalAddAmount = 0;
                    int addRowNum = 1;
                    foreach (var amt in addAmounts)
                    {
                        e.Graphics.DrawRectangle(Pens.Black, leftMargin, yPos, totalWidth, 25);
                        totalAddAmount += amt.Amount;
                        string[] addRowData = { addRowNum.ToString(), amt.ItemName ?? "", amt.Amount.ToString("N2") };
                        xPos = leftMargin;
                        for (int i = 0; i < addRowData.Length; i++)
                        {
                            RectangleF rect = new RectangleF(xPos, yPos, addColWidths[i], 25);
                            StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                            if (i == 1) sf.Alignment = StringAlignment.Near;
                            e.Graphics.DrawString(addRowData[i], normalFont, Brushes.Black, rect, sf);
                            xPos += addColWidths[i];
                        }
                        yPos += 25;
                        addRowNum++;
                    }
                    yPos += 20;
                }

                decimal grandTotal = totalAmount + addAmounts.Sum(a => a.Amount);
                float totalBoxHeight = 50;
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(76, 175, 80)), rightMargin - 400, yPos, 400, totalBoxHeight);
                e.Graphics.DrawRectangle(Pens.DarkGreen, rightMargin - 400, yPos, 400, totalBoxHeight);
                string grandTotalText = $"الإجمالي النهائي: {grandTotal.ToString("N2")}";
                SizeF gtSize = e.Graphics.MeasureString(grandTotalText, headerFont);
                e.Graphics.DrawString(grandTotalText, headerFont, Brushes.White, rightMargin - 200 - gtSize.Width / 2, yPos + 12);
                yPos += totalBoxHeight + 30;

                float sigWidth = 200;
                float sigHeight = 70;
                float sigY = yPos;
                e.Graphics.DrawRectangle(Pens.Black, rightMargin - sigWidth, sigY, sigWidth, sigHeight);
                e.Graphics.DrawString("توقيع المدير", subHeaderFont, Brushes.Black, rightMargin - sigWidth + sigWidth / 2 - 40, sigY + 10);
                e.Graphics.DrawRectangle(Pens.Black, centerX - sigWidth / 2, sigY, sigWidth, sigHeight);
                e.Graphics.DrawString("توقيع المحاسب", subHeaderFont, Brushes.Black, centerX - 40, sigY + 10);
                e.Graphics.DrawRectangle(Pens.Black, leftMargin, sigY, sigWidth, sigHeight);
                e.Graphics.DrawString("توقيع المستلم", subHeaderFont, Brushes.Black, leftMargin + sigWidth / 2 - 50, sigY + 10);

                string printDate = $"تاريخ الطباعة: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
                SizeF pdSize = e.Graphics.MeasureString(printDate, smallFont);
                e.Graphics.DrawString(printDate, smallFont, Brushes.Gray, rightMargin - pdSize.Width, bottomMargin - 20);
                e.HasMorePages = false;
            };

            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = printDoc;
            previewDialog.Width = 1500;
            previewDialog.Height = 850;
            previewDialog.StartPosition = FormStartPosition.CenterParent;
            previewDialog.WindowState = FormWindowState.Maximized;
            previewDialog.ShowDialog();
        }

        private void ShowAllStatementsList()
        {
            var allowedPorts = UserManager.GetAllowedPorts("View");
            var filteredRecords = UserManager.IsAdmin() ? records : records.Where(r => allowedPorts.Contains(r.PortName)).ToList();
            if (filteredRecords.Count == 0)
            {
                MessageBox.Show(LanguageManager.Get("msg_no_clearance_data"), LanguageManager.Get("msg_data_list"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Form listForm = new Form();
            listForm.Text = $"📋 قائمة بيانات التخليص الجمركي - اختر البيان";
            listForm.Size = new Size(1400, 700);
            listForm.StartPosition = FormStartPosition.CenterScreen;
            listForm.WindowState = FormWindowState.Maximized;
            listForm.RightToLeft = RightToLeft.Yes;
            listForm.RightToLeftLayout = true;
            listForm.BackColor = Color.White;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 70;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);
            pnlSearch.Padding = new Padding(10);

            Label lblSearchIcon = new Label { Text = "🔍", Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(10, 15), Size = new Size(40, 40), TextAlign = ContentAlignment.MiddleCenter };
            Label lblSearchText = new Label { Text = "بحث برقم البيان:", Font = new Font("Tahoma", 12, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(60, 20), Size = new Size(150, 30), TextAlign = ContentAlignment.MiddleRight };
            TextBox txtSearch = new TextBox { Font = new Font("Tahoma", 12), Location = new Point(220, 20), Size = new Size(350, 30), BorderStyle = BorderStyle.FixedSingle };
            Label lblTotalRecords = new Label();
            int totalStatements = filteredRecords.GroupBy(r => r.StatementNo).Count();
            lblTotalRecords.Text = $" إجمالي البيانات: {totalStatements} بيان";
            lblTotalRecords.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblTotalRecords.ForeColor = Color.FromArgb(76, 175, 80);
            lblTotalRecords.Location = new Point(600, 25);
            lblTotalRecords.Size = new Size(400, 25);
            lblTotalRecords.TextAlign = ContentAlignment.MiddleRight;
            pnlSearch.Controls.AddRange(new Control[] { lblSearchIcon, lblSearchText, txtSearch, lblTotalRecords });

            DataGridView dgvList = new DataGridView();
            dgvList.Dock = DockStyle.Fill;
            dgvList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvList.MultiSelect = false;
            dgvList.ReadOnly = true;
            dgvList.RightToLeft = RightToLeft.Yes;
            dgvList.AllowUserToAddRows = false;
            dgvList.BackgroundColor = Color.White;
            dgvList.BorderStyle = BorderStyle.Fixed3D;
            dgvList.RowHeadersVisible = false;
            dgvList.EnableHeadersVisualStyles = false;
            dgvList.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvList.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvList.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvList.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvList.ColumnHeadersHeight = 50;
            dgvList.RowTemplate.Height = 40;
            dgvList.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvList.RowsDefaultCellStyle.Font = new Font("Segoe UI", 10);

            dgvList.Columns.Add("colStatementNo", "رقم البيان:");
            dgvList.Columns.Add("colTariffType", "نوع الترسيم:");
            dgvList.Columns.Add("colPortName", "اسم المنفذ:");
            dgvList.Columns.Add("colDate", "تاريخ البيان:");
            dgvList.Columns.Add("colItemsCount", "عدد الأصناف");
            dgvList.Columns.Add("colTotalQty", "إجمالي الكمية");
            dgvList.Columns.Add("colTotalWeight", "إجمالي الوزن");
            dgvList.Columns.Add("colTotalValue", "الإجمالي النهائي");

            dgvList.Columns["colTotalQty"].DefaultCellStyle.Format = "N2";
            dgvList.Columns["colTotalQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvList.Columns["colTotalWeight"].DefaultCellStyle.Format = "N2";
            dgvList.Columns["colTotalWeight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvList.Columns["colTotalValue"].DefaultCellStyle.Format = "N2";
            dgvList.Columns["colTotalValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvList.Columns["colItemsCount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvList.Columns["colDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvList.Columns["colStatementNo"].DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
            dgvList.Columns["colTotalValue"].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
            dgvList.Columns["colTotalValue"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            void RefreshList(string searchText = "")
            {
                dgvList.Rows.Clear();
                var filtered = filteredRecords.AsEnumerable();
                if (!string.IsNullOrWhiteSpace(searchText))
                    filtered = filtered.Where(r => r.StatementNo != null && r.StatementNo.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0);

                var groupedStatements = filtered.GroupBy(r => r.StatementNo).Select(g => new
                {
                    StatementNo = g.Key,
                    TariffType = g.First().TariffType ?? "-",
                    PortName = g.First().PortName,
                    StatementDate = g.First().StatementDate,
                    ItemsCount = g.Count(),
                    TotalQty = g.Sum(r => r.Quantity),
                    TotalWeight = g.Sum(r => r.WeightTotal),
                    TotalValue = g.Sum(r => r.Total)
                }).OrderByDescending(g => g.StatementDate).ToList();

                foreach (var stmt in groupedStatements)
                {
                    dgvList.Rows.Add(stmt.StatementNo, stmt.TariffType, stmt.PortName, stmt.StatementDate.ToString("yyyy-MM-dd"), stmt.ItemsCount, stmt.TotalQty.ToString("N2"), stmt.TotalWeight.ToString("N2"), stmt.TotalValue.ToString("N2"));
                }
                lblTotalRecords.Text = $"📊 إجمالي البيانات: {dgvList.Rows.Count} بيان";
            }

            txtSearch.TextChanged += (s, e) => RefreshList(txtSearch.Text);
            RefreshList();

            Panel pnlBottom = new Panel { Height = 70, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(10) };
            Button btnSelect = new Button { Text = "✅ اختيار البيان المحدد", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(200, 45), Location = new Point(250, 12) };
            btnSelect.Click += (s, ev) =>
            {
                if (dgvList.SelectedRows.Count > 0)
                {
                    string selectedStatementNo = dgvList.SelectedRows[0].Cells["colStatementNo"].Value?.ToString();
                    if (!string.IsNullOrEmpty(selectedStatementNo))
                    {
                        var selectedRecord = filteredRecords.FirstOrDefault(r => r.StatementNo == selectedStatementNo);
                        if (selectedRecord != null)
                        {
                            LoadRecordToForm(selectedRecord);
                            listForm.DialogResult = DialogResult.OK;
                            listForm.Close();
                            MessageBox.Show($"✅ تم تحميل البيان رقم: {selectedStatementNo}", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("⚠️ الرجاء اختيار بيان أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            Button btnClose = new Button { Text = "❌ إغلاق", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 45), Location = new Point(50, 12) };
            btnClose.Click += (s, ev) => listForm.Close();

            pnlBottom.Controls.Add(btnSelect);
            pnlBottom.Controls.Add(btnClose);
            listForm.Controls.Add(dgvList);
            listForm.Controls.Add(pnlBottom);
            listForm.Controls.Add(pnlSearch);

            dgvList.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string selectedStatementNo = dgvList.Rows[ev.RowIndex].Cells["colStatementNo"].Value?.ToString();
                    if (!string.IsNullOrEmpty(selectedStatementNo))
                    {
                        var selectedRecord = filteredRecords.FirstOrDefault(r => r.StatementNo == selectedStatementNo);
                        if (selectedRecord != null)
                        {
                            LoadRecordToForm(selectedRecord);
                            listForm.DialogResult = DialogResult.OK;
                            listForm.Close();
                            MessageBox.Show($"✅ تم تحميل البيان رقم: {selectedStatementNo}", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            };
            listForm.ShowDialog();
        }

        private void CmbPortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPort = cmbPortName.SelectedItem?.ToString() ?? "";
            if (selectedPort != "-- اختر منفذ --" && !string.IsNullOrEmpty(selectedPort))
            {
                var port = allPorts.FirstOrDefault(p => p.Name == selectedPort);
                if (port != null)
                {
                    string classification = port.Classification ?? "";
                    if (classification.Contains("داخلي") || classification.Contains("شمال")) currentPortClassification = "داخلي";
                    else if (classification.Contains("خارجي") || classification.Contains("جنوب")) currentPortClassification = "خارجي";
                    else currentPortClassification = classification;
                }
            }
            else
            {
                currentPortClassification = "";
            }
        }

        private void CmbTariffType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                Form selectForm = new Form();
                selectForm.Text = "📋 اختر نوع الترسيم";
                selectForm.Size = new Size(600, 500);
                selectForm.StartPosition = FormStartPosition.CenterParent;
                selectForm.RightToLeft = RightToLeft.Yes;
                selectForm.RightToLeftLayout = true;
                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.ReadOnly = true;
                dgv.AllowUserToAddRows = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersHeight = 40;
                dgv.Columns.Add("colTypeName", "نوع الترسيم");
                try
                {
                    DataTable dt = DatabaseHelper.ExecuteQuery("SELECT TypeNameAr FROM CustomsTariffTypes WHERE IsActive = 1 ORDER BY TypeNameAr");
                    if (dt != null)
                    {
                        foreach (DataRow row in dt.Rows) dgv.Rows.Add(row["TypeNameAr"].ToString());
                    }
                }
                catch { }
                dgv.CellDoubleClick += (s, ev) =>
                {
                    if (ev.RowIndex >= 0) { cmbTariffType.Text = dgv.Rows[ev.RowIndex].Cells["colTypeName"].Value?.ToString(); selectForm.Close(); }
                };
                dgv.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter && dgv.CurrentRow != null)
                    {
                        ev.SuppressKeyPress = true;
                        cmbTariffType.Text = dgv.CurrentRow.Cells["colTypeName"].Value?.ToString();
                        selectForm.Close();
                    }
                };
                selectForm.Controls.Add(dgv);
                selectForm.ShowDialog();
            }
        }

        private void TxtStatementNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                if (string.IsNullOrWhiteSpace(txtUnloadFrom.Text) || txtUnloadFrom.Text != "طلب ترسيم")
                {
                    MessageBox.Show("⚠️ الرجاء تحديد مصدر الإنزال أولاً\n(اضغط F9 في حقل 'إنزال من' وسيتم تحديده تلقائياً)", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtUnloadFrom.Focus();
                    return;
                }
                ShowApprovedTariffRequests();
            }
        }

        private void OpenItemsSelectionForm()
        {
            try
            {
                if (string.IsNullOrEmpty(currentPortClassification))
                {
                    MessageBox.Show("⚠️ الرجاء اختيار المنفذ أولاً", LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var filteredItems = allItemsData.Where(item =>
                {
                    if (string.IsNullOrEmpty(item.PortGroup)) return false; // ✅ PortGroup
                    if (currentPortClassification == "داخلي")
                        return item.PortGroup.Contains("داخلي") || item.PortGroup.Contains("شمال"); // ✅ PortGroup
                    else if (currentPortClassification == "خارجي")
                        return item.PortGroup.Contains("خارجي") || item.PortGroup.Contains("جنوب"); // ✅ PortGroup
                    return false;
                }).Select(item => new { item.ProductName, item.Unit, item.WeightUnit, item.Weight, item.TotalCustoms, item.TotalST, item.TotalVAT, item.TotalPT, item.TotalVF, item.TotalTEF, item.TotalMFF, item.CountryOfOrigin }).Distinct().ToList();

                string regionName = currentPortClassification.Contains("داخلي") ? LanguageManager.Get("north_region") : LanguageManager.Get("south_region");
                if (filteredItems.Count == 0)
                {
                    MessageBox.Show($"لا توجد أصناف مسجلة لـ {regionName}", LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Form selectionForm = new Form();
                selectionForm.Text = $"📦 اختيار الصنف - {regionName}";
                selectionForm.Size = new Size(700, 500);
                selectionForm.StartPosition = FormStartPosition.CenterParent;
                selectionForm.RightToLeft = LanguageManager.RightToLeft;
                selectionForm.WindowState = FormWindowState.Maximized;

                DataGridView dgvItems = new DataGridView();
                dgvItems.Dock = DockStyle.Fill;
                dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvItems.MultiSelect = false;
                dgvItems.RightToLeft = LanguageManager.RightToLeft;
                dgvItems.ReadOnly = true;
                dgvItems.AllowUserToAddRows = false;
                dgvItems.Columns.Add("colProductName", LanguageManager.Get("col_product_name"));
                dgvItems.Columns.Add("colUnit", "وحدة الصنف");
                dgvItems.Columns.Add("colWeightUnit", "وحدة الوزن");
                foreach (var item in filteredItems) dgvItems.Rows.Add(item.ProductName, item.Unit ?? "", item.WeightUnit ?? "");

                dgvItems.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter && dgvItems.CurrentRow != null) { ev.SuppressKeyPress = true; selectionForm.DialogResult = DialogResult.OK; selectionForm.Close(); }
                };

                Panel pnlButtons = new Panel { Height = 60, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(240, 248, 255) };
                Button btnOK = new Button { Text = "✅ اختيار", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(120, 40), Location = new Point(200, 10), DialogResult = DialogResult.OK };
                Button btnCancel = new Button { Text = "❌ إلغاء", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(120, 40), Location = new Point(80, 10), DialogResult = DialogResult.Cancel };
                pnlButtons.Controls.Add(btnOK);
                pnlButtons.Controls.Add(btnCancel);
                selectionForm.Controls.Add(dgvItems);
                selectionForm.Controls.Add(pnlButtons);
                selectionForm.AcceptButton = btnOK;
                selectionForm.CancelButton = btnCancel;

                if (selectionForm.ShowDialog() == DialogResult.OK && dgvItems.SelectedRows.Count > 0)
                {
                    string selectedProductName = dgvItems.SelectedRows[0].Cells["colProductName"].Value?.ToString();
                    var selectedItem = filteredItems.FirstOrDefault(i => i.ProductName == selectedProductName);
                    if (selectedItem != null)
                    {
                        int rowIndex = dgvData.Rows.Add();
                        DataGridViewRow newRow = dgvData.Rows[rowIndex];
                        newRow.Cells["colId"].Value = rowIndex + 1;
                        newRow.Cells["colProductName"].Value = selectedItem.ProductName;
                        newRow.Cells["colUnit"].Value = selectedItem.Unit ?? "";
                        newRow.Cells["colWeightUnit"].Value = selectedItem.WeightUnit ?? "";
                        newRow.Cells["colQuantity"].Value = 0;
                        newRow.Cells["colUnitWeight"].Value = selectedItem.Weight;
                        newRow.Cells["colTotalWeight"].Value = 0;
                        // ✅ تم حذف تعيين colCountryOfOrigin من هنا
                        newRow.Cells["colCustomsFeeUnit"].Value = selectedItem.TotalCustoms;
                        newRow.Cells["colSTUnit"].Value = selectedItem.TotalST;
                        newRow.Cells["colVATUnit"].Value = selectedItem.TotalVAT;
                        newRow.Cells["colPTUnit"].Value = selectedItem.TotalPT;
                        newRow.Cells["colVFUnit"].Value = selectedItem.TotalVF;
                        newRow.Cells["colTEFUnit"].Value = selectedItem.TotalTEF;
                        newRow.Cells["colMFFUnit"].Value = selectedItem.TotalMFF;
                        newRow.Cells["colCustomsFeeTotal"].Value = 0;
                        newRow.Cells["colSTTotal"].Value = 0;
                        newRow.Cells["colVATTotal"].Value = 0;
                        newRow.Cells["colPTTotal"].Value = 0;
                        newRow.Cells["colVFTotal"].Value = 0;
                        newRow.Cells["colTEFTotal"].Value = 0;
                        newRow.Cells["colMFFTotal"].Value = 0;
                        newRow.Cells["colOther"].Value = 0;
                        newRow.Cells["colTotal"].Value = 0;
                        newRow.Cells["colAgriculturePermit"].Value = 0;
                        newRow.Cells["colPaymentRelease"].Value = 0;
                        MessageBox.Show($"✅ تم إضافة الصنف: {selectedItem.ProductName}\n💡 أدخل الكمية لحساب المبالغ والوزن تلقائياً", LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في OpenItemsSelectionForm: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F9 && dgvData != null && dgvData.Focused && dgvData.CurrentCell != null)
            {
                if (dgvData.CurrentCell.OwningColumn?.Name == "colProductName")
                {
                    OpenItemsSelectionForm();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void CreateControlsManually()
        {
            this.Text = LanguageManager.Get("clearance_form_title");
            this.Size = new Size(1500, 800);
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            toolStrip1 = new ToolStrip();
            toolStrip1.RightToLeft = RightToLeft.Yes;
            toolStrip1.BackColor = Color.FromArgb(52, 73, 94);
            toolStrip1.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnHome = new ToolStripButton(LanguageManager.Get("btn_home"));
            btnHome.ForeColor = Color.White;
            btnHome.Click += btnHome_Click;
            toolStripSeparator1 = new ToolStripSeparator();
            btnSaveStrip = new ToolStripButton(LanguageManager.Get("btn_save"));
            btnSaveStrip.ForeColor = Color.White;
            btnSaveStrip.Click += (s, ev) => btnSave_Click(s, ev);
            btnNewStrip = new ToolStripButton(LanguageManager.Get("btn_new"));
            btnNewStrip.ForeColor = Color.White;
            btnNewStrip.Click += (s, ev) => btnNew_Click(s, ev);
            toolStripSeparator2 = new ToolStripSeparator();
            btnEditStrip = new ToolStripButton(LanguageManager.Get("btn_edit"));
            btnEditStrip.ForeColor = Color.White;
            btnEditStrip.Click += (s, ev) => btnEdit_Click(s, ev);
            btnDeleteStrip = new ToolStripButton(LanguageManager.Get("btn_delete"));
            btnDeleteStrip.ForeColor = Color.White;
            btnDeleteStrip.Click += (s, ev) => btnDelete_Click(s, ev);
            toolStripSeparator3 = new ToolStripSeparator();
            btnLoadStatementStrip = new ToolStripButton("📥 تحميل البيان");
            btnLoadStatementStrip.ForeColor = Color.White;
            btnLoadStatementStrip.Click += (s, ev) => LoadStatement();
            btnMovementsStrip = new ToolStripButton("📜 السجل");
            btnMovementsStrip.ForeColor = Color.White;
            btnMovementsStrip.Click += (s, ev) => ShowMovements();
            btnPrintReportStrip = new ToolStripButton("🖨️ اصدار  ");
            btnPrintReportStrip.ForeColor = Color.White;
            btnPrintReportStrip.Click += (s, ev) => PrintMovementsReport();
            btnDocumentReviewStrip = new ToolStripButton("📋 مراجعة الوثائق");
            btnDocumentReviewStrip.ForeColor = Color.White;
            btnDocumentReviewStrip.Click += (s, ev) => OpenDocumentReviewForm();
            toolStripSeparator4 = new ToolStripSeparator();
            btnSearchStrip = new ToolStripButton(LanguageManager.Get("btn_search"));
            btnSearchStrip.ForeColor = Color.White;
            btnSearchStrip.Click += (s, ev) => ShowAllStatementsList();
            btnExportStrip = new ToolStripButton(LanguageManager.Get("btn_export"));
            btnExportStrip.ForeColor = Color.White;
            btnExportStrip.Click += (s, ev) => btnExport_Click(s, ev);
            btnExitStrip = new ToolStripButton(LanguageManager.Get("btn_exit"));
            btnExitStrip.ForeColor = Color.White;
            btnExitStrip.Click += (s, ev) => btnExit_Click(s, ev);

            toolStrip1.Items.AddRange(new ToolStripItem[] { btnHome, toolStripSeparator1, btnSaveStrip, btnNewStrip, toolStripSeparator2, btnEditStrip, btnDeleteStrip, toolStripSeparator3, btnLoadStatementStrip, btnMovementsStrip, btnPrintReportStrip, btnDocumentReviewStrip, toolStripSeparator4, btnSearchStrip, btnExportStrip, btnExitStrip });

            tabControlMain = new TabControl();
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.RightToLeft = RightToLeft.Yes;
            tabControlMain.RightToLeftLayout = true;
            tabControlMain.Alignment = TabAlignment.Top;
            tabPageMain = new TabPage("الرئيسية");
            tabPageOther = new TabPage("بيانات أخرى");
            tabPageAdditional = new TabPage("المبالغ الإضافية");
            tabControlMain.TabPages.Add(tabPageMain);
            tabControlMain.TabPages.Add(tabPageOther);
            tabControlMain.TabPages.Add(tabPageAdditional);
            this.Controls.Add(tabControlMain);
            this.Controls.Add(toolStrip1);

            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 160;
            pnlHeader.BackColor = Color.FromArgb(240, 248, 255);
            pnlHeader.Padding = new Padding(10);
            Label lblTariffType = new Label { Text = "نوع الترسيم (F9):", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 15), Size = new Size(110, 25), TextAlign = ContentAlignment.MiddleRight };
            cmbTariffType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Font = new Font("Tahoma", 10), Location = new Point(960, 15), Size = new Size(180, 27) };
            cmbTariffType.KeyDown += CmbTariffType_KeyDown;
            Label lblPortName = new Label { Text = LanguageManager.Get("lbl_port_name"), Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(810, 15), Size = new Size(90, 25), TextAlign = ContentAlignment.MiddleRight };
            cmbPortName = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 10), Location = new Point(658, 15), Size = new Size(140, 27) };
            cmbPortName.Items.Add("-- اختر منفذ --");
            cmbPortName.SelectedIndex = 0;
            cmbPortName.SelectedIndexChanged += CmbPortName_SelectedIndexChanged;
            Label lblStatementNo = new Label { Text = LanguageManager.Get("lbl_statement_no"), Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 55), Size = new Size(110, 25), TextAlign = ContentAlignment.MiddleRight };
            txtStatementNo = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(960, 55), Size = new Size(180, 27), TextAlign = HorizontalAlignment.Right, BackColor = Color.FromArgb(255, 255, 190) };
            txtStatementNo.KeyDown += TxtStatementNo_KeyDown;
            Label lblStatementDate = new Label { Text = LanguageManager.Get("lbl_statement_date"), Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(810, 55), Size = new Size(90, 25), TextAlign = ContentAlignment.MiddleRight };
            dtpStatementDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Tahoma", 10), Location = new Point(658, 55), Size = new Size(140, 27) };
            Label lblUnloadFrom = new Label { Text = "إنزال من:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 95), Size = new Size(110, 25), TextAlign = ContentAlignment.MiddleRight };
            txtUnloadFrom = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(960, 95), Size = new Size(180, 27), TextAlign = HorizontalAlignment.Right, ReadOnly = true };
            txtUnloadFrom.KeyDown += TxtUnloadFrom_KeyDown;
            Label lblTariffRequestNo = new Label { Text = "رقم طلب الترسيم:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(810, 95), Size = new Size(110, 25), TextAlign = ContentAlignment.MiddleRight };
            txtTariffRequestNo = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(658, 95), Size = new Size(140, 27), TextAlign = HorizontalAlignment.Right };
            pnlHeader.Controls.AddRange(new Control[] { lblTariffType, cmbTariffType, lblPortName, cmbPortName, lblStatementNo, txtStatementNo, lblStatementDate, dtpStatementDate, lblUnloadFrom, txtUnloadFrom, lblTariffRequestNo, txtTariffRequestNo });

            dgvData = new DataGridView { Dock = DockStyle.Fill };
            tabPageMain.Controls.Add(dgvData);
            tabPageMain.Controls.Add(pnlHeader);

            Panel pnlOther = new Panel();
            pnlOther.Dock = DockStyle.Fill;
            pnlOther.BackColor = Color.White;
            pnlOther.Padding = new Padding(15);
            Label lblVehicleCount = new Label();
            lblVehicleCount.Text = "عدد سيارات النقل:";
            lblVehicleCount.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblVehicleCount.ForeColor = Color.FromArgb(51, 122, 183);
            lblVehicleCount.Location = new Point(1100, 15);
            lblVehicleCount.Size = new Size(160, 25);
            lblVehicleCount.TextAlign = ContentAlignment.MiddleRight;
            numVehicleCount = new NumericUpDown();
            numVehicleCount.Font = new Font("Tahoma", 11, FontStyle.Bold);
            numVehicleCount.Location = new Point(1000, 15);
            numVehicleCount.Size = new Size(100, 27);
            numVehicleCount.Minimum = 0;
            numVehicleCount.Maximum = 50;
            numVehicleCount.Value = 0;
            numVehicleCount.TextAlign = HorizontalAlignment.Center;
            dgvDrivers = new DataGridView();
            dgvDrivers.Location = new Point(20, 55);
            dgvDrivers.Size = new Size(180, 450);
            dgvDrivers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDrivers.BackgroundColor = Color.White;
            dgvDrivers.BorderStyle = BorderStyle.Fixed3D;
            dgvDrivers.RowHeadersVisible = false;
            dgvDrivers.AllowUserToAddRows = false;
            dgvDrivers.AllowUserToDeleteRows = true;
            dgvDrivers.ReadOnly = false;
            dgvDrivers.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvDrivers.EnableHeadersVisualStyles = false;
            dgvDrivers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvDrivers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDrivers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDrivers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDrivers.ColumnHeadersHeight = 40;
            dgvDrivers.RowTemplate.Height = 35;
            dgvDrivers.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvDrivers.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (dgvDrivers.Columns.Count == 0)
            {
                dgvDrivers.Columns.Add("colRowNum", "م");
                dgvDrivers.Columns.Add("colDriverId", "رقم السائق");
                dgvDrivers.Columns.Add("colDriverName", "اسم السائق");
                dgvDrivers.Columns.Add("colVehicleNo", "رقم السيارة");
                dgvDrivers.Columns["colRowNum"].Width = 50;
                dgvDrivers.Columns["colRowNum"].ReadOnly = true;
                dgvDrivers.Columns["colRowNum"].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
                dgvDrivers.Columns["colDriverId"].Width = 150;
                dgvDrivers.Columns["colDriverName"].Width = 300;
                dgvDrivers.Columns["colVehicleNo"].Width = 200;
            }
            numVehicleCount.ValueChanged += (s, ev) =>
            {
                int count = (int)numVehicleCount.Value;
                dgvDrivers.Rows.Clear();
                for (int i = 1; i <= count; i++) { dgvDrivers.Rows.Add(i, "", "", ""); }
            };
            pnlOther.Controls.Add(lblVehicleCount);
            pnlOther.Controls.Add(numVehicleCount);
            pnlOther.Controls.Add(dgvDrivers);
            tabPageOther.Controls.Add(pnlOther);

            dgvAdditionalAmounts = new DataGridView();
            dgvAdditionalAmounts.Dock = DockStyle.Fill;
            dgvAdditionalAmounts.AutoGenerateColumns = false;
            dgvAdditionalAmounts.AllowUserToAddRows = true;
            dgvAdditionalAmounts.AllowUserToDeleteRows = true;
            dgvAdditionalAmounts.BackgroundColor = Color.White;
            dgvAdditionalAmounts.BorderStyle = BorderStyle.None;
            dgvAdditionalAmounts.RowHeadersVisible = false;
            dgvAdditionalAmounts.RightToLeft = RightToLeft.Yes;
            dgvAdditionalAmounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            if (dgvAdditionalAmounts.Columns.Count == 0)
            {
                dgvAdditionalAmounts.Columns.Add("colItemName", "البند");
                dgvAdditionalAmounts.Columns.Add("colAmount", "المبلغ");
                dgvAdditionalAmounts.Columns["colItemName"].Width = 600;
                dgvAdditionalAmounts.Columns["colAmount"].Width = 400;
                dgvAdditionalAmounts.Columns["colAmount"].DefaultCellStyle.Format = "N2";
                dgvAdditionalAmounts.CellValueChanged += DgvAdditionalAmounts_CellValueChanged;
                dgvAdditionalAmounts.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvAdditionalAmounts.EnableHeadersVisualStyles = false;
                dgvAdditionalAmounts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgvAdditionalAmounts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvAdditionalAmounts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                dgvAdditionalAmounts.ColumnHeadersHeight = 40;
                dgvAdditionalAmounts.DefaultCellStyle.Font = new Font("Segoe UI", 10);
                dgvAdditionalAmounts.DefaultCellStyle.Padding = new Padding(5);
                dgvAdditionalAmounts.RowTemplate.Height = 35;
            }
            tabPageAdditional.Padding = new Padding(0);
            tabPageAdditional.Controls.Add(dgvAdditionalAmounts);

            Panel pnlBottom = new Panel();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 110;
            pnlBottom.BackColor = Color.FromArgb(240, 248, 255);
            pnlBottom.Padding = new Padding(10);
            lblTotalFeesTitle = new Label { Text = " إجمالي رسوم البيان:", Font = new Font("Tahoma", 11, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(1050, 10), Size = new Size(180, 25), TextAlign = ContentAlignment.MiddleRight };
            lblTotalFeesValue = new Label { Name = "lblTotalFeesValue", Text = "0.00", Font = new Font("Tahoma", 13, FontStyle.Bold), ForeColor = Color.FromArgb(76, 175, 80), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(820, 10), Size = new Size(220, 30) };
            lblOtherFeesTitle = new Label { Text = "📋 رسوم أخرى:", Font = new Font("Tahoma", 10, FontStyle.Bold), ForeColor = Color.FromArgb(255, 152, 0), Location = new Point(400, 50), Size = new Size(180, 25), TextAlign = ContentAlignment.MiddleRight };
            txtOtherFees = new TextBox { Text = "0", Font = new Font("Tahoma", 11, FontStyle.Bold), ForeColor = Color.FromArgb(255, 152, 0), BackColor = Color.FromArgb(255, 248, 225), BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center, Location = new Point(820, 50), Size = new Size(220, 30) };
            txtOtherFees.TextChanged += (s, ev) => { UpdateTotalAmount(); TrackChanges(); };
            lblTotalAmountTitle = new Label { Text = "💵 إجمالي مبلغ البيان:", Font = new Font("Tahoma", 12, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(33, 150, 243), Location = new Point(250, 15), Size = new Size(200, 80), TextAlign = ContentAlignment.MiddleCenter, BorderStyle = BorderStyle.FixedSingle };
            lblTotalAmountValue = new Label { Name = "lblTotalAmountValue", Text = "0.00", Font = new Font("Tahoma", 18, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(76, 175, 80), BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(10, 15), Size = new Size(300, 80) };
            pnlBottom.Controls.AddRange(new Control[] { lblTotalFeesTitle, lblTotalFeesValue, txtOtherFees, lblTotalAmountTitle, lblTotalAmountValue });
            pnlBottom.Controls.Add(new Label { Text = "📋 رسوم أخرى", Font = new Font("Tahoma", 8, FontStyle.Bold), ForeColor = Color.FromArgb(255, 152, 0), Location = new Point(700, 82), Size = new Size(100, 20) });
            this.Controls.Add(pnlBottom);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (!CheckUnsavedChanges()) return;
            try
            {
                MainForm mainForm = this.MdiParent as MainForm ?? Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm != null && !mainForm.IsDisposed)
                {
                    foreach (Form child in mainForm.MdiChildren.ToList()) if (!child.IsDisposed && child != this) child.Close();
                    this.Close();
                    mainForm.ShowWelcome();
                    mainForm.WindowState = FormWindowState.Maximized;
                    mainForm.Activate();
                    mainForm.BringToFront();
                }
                else { this.Close(); }
            }
            catch (Exception ex) { MessageBox.Show($"{LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error); this.Close(); }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            var currentUser = UserManager.GetCurrentUser();
            if (currentUser?.Role != "Admin")
            {
                MessageBox.Show("⚠️ لا يمكنك إضافة بيان من هذه الشاشة\nيمكنك إضافة بيان من شاشة 'طلب الترسيم' وسوف يتم مراجعتها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!CheckUnsavedChanges()) return;
            ClearFields();
            SetControlsState(true);
            SetAllEditableColumns();
            isEditingExisting = false;
            hasChanges = false;
            txtStatementNo.Focus();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatementNo.Text)) { MessageBox.Show("⚠️ الرجاء تحميل البيان أولاً", LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (dgvData.Rows.Count == 0) { MessageBox.Show("لا توجد بيانات للتعديل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SetControlsState(true);
            SetAllEditableColumns();
            isEditingExisting = true;
            hasChanges = false;
            MessageBox.Show("✅ يمكنك الآن تعديل جميع القيم في الجدول\n💡 بعد التعديل اضغط '💾 حفظ'", "وضع التعديل", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatementNo.Text)) { MessageBox.Show(LanguageManager.Get("msg_statement_no_required"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); txtStatementNo.Focus(); return; }
            string portName = cmbPortName.SelectedItem?.ToString() ?? "";
            string statementNo = txtStatementNo.Text.Trim();
            if (string.IsNullOrEmpty(portName) || portName == "-- اختر منفذ --") { MessageBox.Show("⚠️ الرجاء اختيار المنفذ أولاً", LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); cmbPortName.Focus(); return; }
            if (numVehicleCount.Value == 0) { MessageBox.Show("⚠️ الرجاء إدخال عدد سيارات النقل (يجب أن يكون أكبر من 0)", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); numVehicleCount.Focus(); return; }

            foreach (DataGridViewRow row in dgvDrivers.Rows)
            {
                if (row.IsNewRow) continue;
                string driverName = row.Cells["colDriverName"].Value?.ToString()?.Trim();
                string vehicleNo = row.Cells["colVehicleNo"].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(driverName)) { MessageBox.Show("⚠️ الرجاء إدخال اسم السائق في جميع الصفوف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); dgvDrivers.CurrentCell = row.Cells["colDriverName"]; dgvDrivers.BeginEdit(true); return; }
                if (string.IsNullOrEmpty(vehicleNo)) { MessageBox.Show("⚠️ الرجاء إدخال رقم السيارة في جميع الصفوف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); dgvDrivers.CurrentCell = row.Cells["colVehicleNo"]; dgvDrivers.BeginEdit(true); return; }
            }

            if (!isEditingExisting && !UserManager.CanPort(portName, "Add")) { MessageBox.Show($"🚫 ليس لديك صلاحية إضافة بيانات في منفذ '{portName}'", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (isEditingExisting && !UserManager.CanPort(portName, "Edit")) { MessageBox.Show($"🚫 ليس لديك صلاحية تعديل البيانات في منفذ '{portName}'", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!isEditingExisting)
            {
                var existingStatement = records.FirstOrDefault(r => r.StatementNo == statementNo && r.PortName == portName);
                if (existingStatement != null) { MessageBox.Show($"⚠️ رقم البيان '{statementNo}' موجود مسبقاً في المنفذ '{portName}'!", "تكرار رقم البيان", MessageBoxButtons.OK, MessageBoxIcon.Error); txtStatementNo.Focus(); return; }
            }

            try
            {
                List<ClearanceRecord> gridRecords = new List<ClearanceRecord>();
                var existingRecords = records.Where(r => r.StatementNo == statementNo && r.PortName == portName).ToList();
                int existingIndex = 0;

                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (row.IsNewRow || row.Cells["colProductName"].Value == null) continue;
                    int recordId = 0;
                    if (existingIndex < existingRecords.Count) { recordId = existingRecords[existingIndex].Id; existingIndex++; }

                    var record = new ClearanceRecord
                    {
                        Id = recordId,
                        PortName = portName,
                        StatementDate = dtpStatementDate.Value,
                        StatementNo = statementNo,
                        TariffType = cmbTariffType.Text.Trim(),
                        UnloadFrom = txtUnloadFrom.Text.Trim(),
                        TariffRequestNo = txtTariffRequestNo.Text.Trim(),
                        ProductName = row.Cells["colProductName"].Value?.ToString(),
                        Unit = row.Cells["colUnit"].Value?.ToString() ?? "",
                        WeightUnit = row.Cells["colWeightUnit"].Value?.ToString() ?? "",
                        Quantity = ParseDecimal(row.Cells["colQuantity"].Value?.ToString()),
                        Weight = ParseDecimal(row.Cells["colUnitWeight"].Value?.ToString()),
                        WeightTotal = ParseDecimal(row.Cells["colTotalWeight"].Value?.ToString()),
                        CustomsFeeTotal = ParseDecimal(row.Cells["colCustomsFeeTotal"].Value?.ToString()),
                        STTotal = ParseDecimal(row.Cells["colSTTotal"].Value?.ToString()),
                        VATTotal = ParseDecimal(row.Cells["colVATTotal"].Value?.ToString()),
                        PTTotal = ParseDecimal(row.Cells["colPTTotal"].Value?.ToString()),
                        VFTotal = ParseDecimal(row.Cells["colVFTotal"].Value?.ToString()),
                        TEFTotal = ParseDecimal(row.Cells["colTEFTotal"].Value?.ToString()),
                        MFFTotal = ParseDecimal(row.Cells["colMFFTotal"].Value?.ToString()),
                        Other = ParseDecimal(row.Cells["colOther"].Value?.ToString()),
                        Total = ParseDecimal(row.Cells["colTotal"].Value?.ToString()),
                        AgriculturePermit = ParseDecimal(row.Cells["colAgriculturePermit"].Value?.ToString()),
                        PaymentRelease = ParseDecimal(row.Cells["colPaymentRelease"].Value?.ToString()),
                        CountryOfOrigin = row.Cells["colCountryOfOrigin"].Value?.ToString() ?? "",
                        TransferredQty = 0,
                        RemainingQty = 0,
                        DriversJson = GetDriversJson(),
                        AdditionalAmountsJson = GetAdditionalAmountsJson()
                    };
                    gridRecords.Add(record);
                }

                records.RemoveAll(r => r.StatementNo == statementNo && r.PortName == portName);
                records.AddRange(gridRecords);
                SaveDataToDatabase(gridRecords);
                if (gridRecords.Count > 0) LoadRecordToForm(gridRecords.First());
                UpdateTotalFees();
                UpdateTotalAmount();
                SetControlsState(false);
                hasChanges = false;
                MessageBox.Show(string.Format(LanguageManager.Get("msg_records_saved"), gridRecords.Count), LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex) { MessageBox.Show($"{LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string GetDriversJson()
        {
            List<DriverInfo> drivers = new List<DriverInfo>();
            if (dgvDrivers != null)
            {
                foreach (DataGridViewRow row in dgvDrivers.Rows)
                {
                    if (row.IsNewRow) continue;
                    drivers.Add(new DriverInfo { DriverId = row.Cells["colDriverId"].Value?.ToString() ?? "", DriverName = row.Cells["colDriverName"].Value?.ToString() ?? "", VehicleNo = row.Cells["colVehicleNo"].Value?.ToString() ?? "" });
                }
            }
            return drivers.Count > 0 ? JsonSerializer.Serialize(drivers) : null;
        }

        private string GetAdditionalAmountsJson()
        {
            List<AdditionalAmountItem> amounts = new List<AdditionalAmountItem>();
            if (dgvAdditionalAmounts != null)
            {
                foreach (DataGridViewRow row in dgvAdditionalAmounts.Rows)
                {
                    if (row.IsNewRow) continue;
                    amounts.Add(new AdditionalAmountItem { ItemName = row.Cells["colItemName"].Value?.ToString() ?? "", Amount = ParseDecimal(row.Cells["colAmount"].Value?.ToString()) });
                }
            }
            return amounts.Count > 0 ? JsonSerializer.Serialize(amounts) : null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatementNo.Text)) { MessageBox.Show(LanguageManager.Get("msg_no_statement_to_delete"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string portName = cmbPortName.SelectedItem?.ToString() ?? "";
            if (!string.IsNullOrEmpty(portName) && portName != "-- اختر منفذ --" && !UserManager.CanPort(portName, "Delete")) { MessageBox.Show($"🚫 ليس لديك صلاحية حذف البيانات في منفذ '{portName}'", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            DialogResult result = MessageBox.Show(string.Format(LanguageManager.Get("msg_confirm_delete_statement"), txtStatementNo.Text), LanguageManager.Get("msg_delete_title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Yes)
            {
                try
                {
                    DeleteDataFromDatabase(txtStatementNo.Text, portName);
                    records.RemoveAll(r => r.StatementNo == txtStatementNo.Text && r.PortName == portName);
                    RefreshGrid();
                    ClearFields();
                    MessageBox.Show(LanguageManager.Get("msg_statement_deleted"), LanguageManager.Get("msg_delete_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"❌ خطأ في الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (Form searchForm = new Form())
            {
                searchForm.Text = " بحث عن بيان";
                searchForm.Size = new Size(450, 180);
                searchForm.StartPosition = FormStartPosition.CenterParent;
                searchForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                searchForm.MaximizeBox = false;
                searchForm.MinimizeBox = false;
                searchForm.RightToLeft = RightToLeft.Yes;
                searchForm.RightToLeftLayout = true;
                Label lblPrompt = new Label { Text = "أدخل رقم البيان للبحث:", Font = new Font("Tahoma", 11, FontStyle.Bold), Location = new Point(20, 20), Size = new Size(380, 25), TextAlign = ContentAlignment.MiddleRight };
                TextBox txtSearchInput = new TextBox { Font = new Font("Tahoma", 12), Location = new Point(20, 50), Size = new Size(380, 30), TextAlign = HorizontalAlignment.Right };
                Button btnOK = new Button { Text = "✅ بحث", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(220, 90), Size = new Size(100, 35), DialogResult = DialogResult.OK };
                Button btnCancel = new Button { Text = "❌ إلغاء", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(100, 90), Size = new Size(100, 35), DialogResult = DialogResult.Cancel };
                searchForm.Controls.AddRange(new Control[] { lblPrompt, txtSearchInput, btnOK, btnCancel });
                searchForm.AcceptButton = btnOK;
                searchForm.CancelButton = btnCancel;

                if (searchForm.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txtSearchInput.Text))
                {
                    string searchNo = txtSearchInput.Text.Trim();
                    var found = records.FirstOrDefault(r => r.StatementNo == searchNo);
                    if (found != null)
                    {
                        LoadRecordToForm(found);
                        UpdateTotalFees();
                        UpdateTotalAmount();
                        isEditingExisting = true;
                        hasChanges = false;
                        MessageBox.Show(string.Format(LanguageManager.Get("msg_statement_found"), found.StatementNo), LanguageManager.Get("btn_search"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else { MessageBox.Show(LanguageManager.Get("msg_statement_not_found"), LanguageManager.Get("btn_search"), MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                }
            }
        }

        private void LoadRecordToForm(ClearanceRecord record)
        {
            cmbPortName.SelectedItem = record.PortName;
            dtpStatementDate.Value = record.StatementDate;
            txtStatementNo.Text = record.StatementNo;
            cmbTariffType.Text = record.TariffType ?? "";
            txtUnloadFrom.Text = record.UnloadFrom ?? "";
            txtTariffRequestNo.Text = record.TariffRequestNo ?? "";
            dgvData.Rows.Clear();
            int idx = 1;
            var allRecords = records.Where(r => r.StatementNo == record.StatementNo && r.PortName == record.PortName).ToList();

            foreach (var r in allRecords)
            {
                var itemData = allItemsData.FirstOrDefault(i => i.ProductName == r.ProductName && i.PortGroup == (r.PortName.Contains("داخلي") ? "داخلي" : "خارجي"));
                decimal customsFeeUnit = 0, stUnit = 0, vatUnit = 0, ptUnit = 0, vfUnit = 0, tefUnit = 0, mffUnit = 0;
                decimal unitWeight = 0;

                if (itemData != null)
                {
                    customsFeeUnit = itemData.TotalCustoms; stUnit = itemData.TotalST; vatUnit = itemData.TotalVAT; ptUnit = itemData.TotalPT; vfUnit = itemData.TotalVF; tefUnit = itemData.TotalTEF; mffUnit = itemData.TotalMFF; unitWeight = itemData.Weight;
                }
                else if (r.Quantity > 0)
                {
                    customsFeeUnit = r.CustomsFeeTotal / r.Quantity; stUnit = r.STTotal / r.Quantity; vatUnit = r.VATTotal / r.Quantity; ptUnit = r.PTTotal / r.Quantity; vfUnit = r.VFTotal / r.Quantity; tefUnit = r.TEFTotal / r.Quantity; mffUnit = r.MFFTotal / r.Quantity; unitWeight = r.Weight;
                }

                // ✅ تم تحديث ترتيب المعاملات ليتطابق مع الأعمدة الجديدة (بدون بلد المنشأ)
                int rowIndex = dgvData.Rows.Add(
                    idx++, r.ProductName, r.Unit ?? "", r.WeightUnit ?? "", r.Quantity, r.WeightTotal,
                    r.AgriculturePermit, r.PaymentRelease, r.CustomsFeeTotal, r.STTotal, r.VATTotal, r.PTTotal,
                    r.VFTotal, r.TEFTotal, r.MFFTotal, r.Other, r.Total
                );

                dgvData.Rows[rowIndex].Cells["colCustomsFeeUnit"].Value = customsFeeUnit;
                dgvData.Rows[rowIndex].Cells["colSTUnit"].Value = stUnit;
                dgvData.Rows[rowIndex].Cells["colVATUnit"].Value = vatUnit;
                dgvData.Rows[rowIndex].Cells["colPTUnit"].Value = ptUnit;
                dgvData.Rows[rowIndex].Cells["colVFUnit"].Value = vfUnit;
                dgvData.Rows[rowIndex].Cells["colTEFUnit"].Value = tefUnit;
                dgvData.Rows[rowIndex].Cells["colMFFUnit"].Value = mffUnit;
                dgvData.Rows[rowIndex].Cells["colUnitWeight"].Value = unitWeight;
            }

            dgvDrivers.Rows.Clear();
            if (!string.IsNullOrEmpty(record.DriversJson))
            {
                try
                {
                    var drivers = JsonSerializer.Deserialize<List<DriverInfo>>(record.DriversJson);
                    numVehicleCount.Value = drivers.Count;
                    for (int i = 0; i < drivers.Count; i++)
                    {
                        if (i < dgvDrivers.Rows.Count)
                        {
                            dgvDrivers.Rows[i].Cells["colDriverId"].Value = drivers[i].DriverId ?? "";
                            dgvDrivers.Rows[i].Cells["colDriverName"].Value = drivers[i].DriverName ?? "";
                            dgvDrivers.Rows[i].Cells["colVehicleNo"].Value = drivers[i].VehicleNo ?? "";
                        }
                    }
                }
                catch { }
            }

            dgvAdditionalAmounts.Rows.Clear();
            if (!string.IsNullOrEmpty(record.AdditionalAmountsJson))
            {
                try
                {
                    var amounts = JsonSerializer.Deserialize<List<AdditionalAmountItem>>(record.AdditionalAmountsJson);
                    foreach (var amt in amounts) { dgvAdditionalAmounts.Rows.Add(amt.ItemName, amt.Amount); }
                }
                catch { }
            }
            currentStatementNo = record.StatementNo;
            UpdateTotalFees();
            UpdateTotalAmount();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (records.Count == 0) { MessageBox.Show(LanguageManager.Get("msg_no_data_export"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SaveFileDialog sfd = new SaveFileDialog() { Filter = "CSV ملف|*.csv", FileName = $"Clearance_{DateTime.Now:yyyyMMdd_HHmmss}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        for (int i = 0; i < dgvData.Columns.Count; i++) { sw.Write(dgvData.Columns[i].HeaderText); if (i < dgvData.Columns.Count - 1) sw.Write(","); }
                        sw.WriteLine();
                        foreach (DataGridViewRow row in dgvData.Rows)
                        {
                            if (row.IsNewRow) continue;
                            for (int i = 0; i < dgvData.Columns.Count; i++)
                            {
                                string value = row.Cells[i].Value?.ToString() ?? "";
                                if (value.Contains(",")) value = $"\"{value}\"";
                                sw.Write(value);
                                if (i < dgvData.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }
                    MessageBox.Show(LanguageManager.Get("msg_export_success"), LanguageManager.Get("btn_export"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"{LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void btnExit_Click(object sender, EventArgs e) => this.Close();

        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            text = text.Replace(",", "").Replace(" ", "").Trim();
            if (decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal val)) return val;
            return 0;
        }

        private void ClearFields()
        {
            try
            {
                if (cmbPortName != null && cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = 0;
                if (cmbTariffType != null) cmbTariffType.Text = "";
                if (dtpStatementDate != null) dtpStatementDate.Value = DateTime.Now;
                if (txtStatementNo != null) txtStatementNo.Clear();
                if (txtUnloadFrom != null) txtUnloadFrom.Clear();
                if (txtTariffRequestNo != null) txtTariffRequestNo.Clear();
                if (numVehicleCount != null) numVehicleCount.Value = 0;
                if (dgvDrivers != null) dgvDrivers.Rows.Clear();
                if (dgvAdditionalAmounts != null) dgvAdditionalAmounts.Rows.Clear();
                if (dgvData != null) dgvData.Rows.Clear();
                if (txtOtherFees != null) txtOtherFees.Text = "0";
                currentPortClassification = "";
                currentStatementNo = "";
                SetTotalFeesValue(0);
                UpdateTotalAmount();
                hasChanges = false;
                isEditingMode = false;
                isEditingExisting = false;
            }
            catch (Exception ex) { Console.WriteLine($"خطأ في تفريغ الحقول: {ex.Message}"); }
        }

        private void RefreshGrid()
        {
            if (dgvData == null) return;
            dgvData.Rows.Clear();
            int idx = 1;
            foreach (var r in records)
            {
                decimal quantity = r.Quantity;
                decimal customsFeeUnit = quantity > 0 ? r.CustomsFeeTotal / quantity : 0;
                decimal stUnit = quantity > 0 ? r.STTotal / quantity : 0;
                decimal vatUnit = quantity > 0 ? r.VATTotal / quantity : 0;
                decimal ptUnit = quantity > 0 ? r.PTTotal / quantity : 0;
                decimal vfUnit = quantity > 0 ? r.VFTotal / quantity : 0;
                decimal tefUnit = quantity > 0 ? r.TEFTotal / quantity : 0;
                decimal mffUnit = quantity > 0 ? r.MFFTotal / quantity : 0;

                // ✅ تم تحديث ترتيب المعاملات ليتطابق مع الأعمدة الجديدة (بدون بلد المنشأ)
                int rowIndex = dgvData.Rows.Add(
                    idx++, r.ProductName, r.Unit ?? "", r.WeightUnit ?? "", r.Quantity, r.WeightTotal,
                    r.AgriculturePermit, r.PaymentRelease, r.CustomsFeeTotal, r.STTotal, r.VATTotal, r.PTTotal,
                    r.VFTotal, r.TEFTotal, r.MFFTotal, r.Other, r.Total
                );

                dgvData.Rows[rowIndex].Cells["colCustomsFeeUnit"].Value = customsFeeUnit;
                dgvData.Rows[rowIndex].Cells["colSTUnit"].Value = stUnit;
                dgvData.Rows[rowIndex].Cells["colVATUnit"].Value = vatUnit;
                dgvData.Rows[rowIndex].Cells["colPTUnit"].Value = ptUnit;
                dgvData.Rows[rowIndex].Cells["colVFUnit"].Value = vfUnit;
                dgvData.Rows[rowIndex].Cells["colTEFUnit"].Value = tefUnit;
                dgvData.Rows[rowIndex].Cells["colMFFUnit"].Value = mffUnit;
                dgvData.Rows[rowIndex].Cells["colUnitWeight"].Value = r.Weight;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!CheckUnsavedChanges()) { e.Cancel = true; return; }
            base.OnFormClosing(e);
        }

        private void ShowApprovedTariffRequests()
        {
            Form selectForm = new Form();
            selectForm.Text = "📋 اختر بيان معتمد من شاشة طلب الترسيم";
            selectForm.Size = new Size(950, 500);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.Columns.Add("colStatementNo", "رقم البيان");
            dgv.Columns.Add("colPortName", "المنفذ");
            dgv.Columns.Add("colTariffType", "نوع الترسيم");
            dgv.Columns.Add("colCreatedBy", "المخلص");
            dgv.Columns.Add("colStatementDate", "تاريخ البيان");
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(@"SELECT StatementNo, PortName, TariffType, CreatedBy, StatementDate FROM TariffRequests WHERE Status = 'approved' ORDER BY ReviewedDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dgv.Rows.Add(row["StatementNo"].ToString(), row["PortName"].ToString(), row["TariffType"]?.ToString() ?? "-", row["CreatedBy"]?.ToString() ?? "-", row["StatementDate"] != DBNull.Value ? Convert.ToDateTime(row["StatementDate"]).ToString("yyyy-MM-dd") : "");
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"خطأ في جلب البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            dgv.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string statementNo = dgv.Rows[ev.RowIndex].Cells["colStatementNo"].Value?.ToString();
                    txtStatementNo.Text = statementNo;
                    cmbPortName.SelectedItem = dgv.Rows[ev.RowIndex].Cells["colPortName"].Value?.ToString();
                    cmbTariffType.Text = dgv.Rows[ev.RowIndex].Cells["colTariffType"].Value?.ToString();
                    LoadItemsForStatement(statementNo);
                    selectForm.Close();
                }
            };
            dgv.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter && dgv.CurrentRow != null)
                {
                    ev.SuppressKeyPress = true;
                    string statementNo = dgv.CurrentRow.Cells["colStatementNo"].Value?.ToString();
                    txtStatementNo.Text = statementNo;
                    cmbPortName.SelectedItem = dgv.CurrentRow.Cells["colPortName"].Value?.ToString();
                    cmbTariffType.Text = dgv.CurrentRow.Cells["colTariffType"].Value?.ToString();
                    LoadItemsForStatement(statementNo);
                    selectForm.Close();
                }
            };
            selectForm.Controls.Add(dgv);
            selectForm.ShowDialog();
        }

        private void LoadItemsForStatement(string statementNo)
        {
            dgvData.Rows.Clear();
            dgvDrivers.Rows.Clear();
            dgvAdditionalAmounts.Rows.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM TariffRequests WHERE StatementNo = @StatementNo", DatabaseHelper.CreateParameter("@StatementNo", statementNo));
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    cmbPortName.SelectedItem = row["PortName"]?.ToString();
                    cmbTariffType.Text = row["TariffType"]?.ToString() ?? "";
                    dtpStatementDate.Value = row["StatementDate"] != DBNull.Value ? Convert.ToDateTime(row["StatementDate"]) : DateTime.Now;
                    txtUnloadFrom.Text = "طلب ترسيم";
                    txtTariffRequestNo.Text = statementNo;

                    string itemsJson = row["ItemsJson"]?.ToString();
                    if (!string.IsNullOrEmpty(itemsJson))
                    {
                        using (JsonDocument doc = JsonDocument.Parse(itemsJson))
                        {
                            var items = doc.RootElement.EnumerateArray().ToList();
                            int idx = 1;
                            foreach (var item in items)
                            {
                                int rowIndex = dgvData.Rows.Add();
                                var dgvRow = dgvData.Rows[rowIndex];
                                dgvRow.Cells["colId"].Value = idx++;
                                dgvRow.Cells["colProductName"].Value = GetJsonValue(item, "ProductName");
                                dgvRow.Cells["colUnit"].Value = GetJsonValue(item, "Unit");
                                dgvRow.Cells["colWeightUnit"].Value = GetJsonValue(item, "WeightUnit");
                                dgvRow.Cells["colQuantity"].Value = GetJsonDecimal(item, "Quantity");
                                // ✅ تم حذف تعيين colCountryOfOrigin من هنا
                                dgvRow.Cells["colCustomsFeeTotal"].Value = GetJsonDecimal(item, "CustomsFeeTotal");
                                dgvRow.Cells["colSTTotal"].Value = GetJsonDecimal(item, "STTotal");
                                dgvRow.Cells["colVATTotal"].Value = GetJsonDecimal(item, "VATTotal");
                                dgvRow.Cells["colPTTotal"].Value = GetJsonDecimal(item, "PTTotal");
                                dgvRow.Cells["colVFTotal"].Value = GetJsonDecimal(item, "VFTotal");
                                dgvRow.Cells["colTEFTotal"].Value = GetJsonDecimal(item, "TEFTotal");
                                dgvRow.Cells["colMFFTotal"].Value = GetJsonDecimal(item, "MFFTotal");
                                dgvRow.Cells["colOther"].Value = GetJsonDecimal(item, "Other");
                                dgvRow.Cells["colTotal"].Value = GetJsonDecimal(item, "Total");

                                string prodName = GetJsonValue(item, "ProductName");
                                decimal qty = GetJsonDecimal(item, "Quantity");
                                var itemData = allItemsData.FirstOrDefault(i => i.ProductName == prodName);
                                decimal unitWeight = itemData?.Weight ?? 0;
                                dgvRow.Cells["colUnitWeight"].Value = unitWeight;
                                dgvRow.Cells["colTotalWeight"].Value = qty * unitWeight;
                            }
                        }
                    }

                    string driversJson = row["DriversJson"]?.ToString();
                    if (!string.IsNullOrEmpty(driversJson))
                    {
                        try
                        {
                            var drivers = JsonSerializer.Deserialize<List<DriverInfo>>(driversJson);
                            numVehicleCount.Value = drivers.Count;
                            for (int i = 0; i < drivers.Count; i++)
                            {
                                if (i < dgvDrivers.Rows.Count)
                                {
                                    dgvDrivers.Rows[i].Cells["colDriverId"].Value = drivers[i].DriverId ?? "";
                                    dgvDrivers.Rows[i].Cells["colDriverName"].Value = drivers[i].DriverName ?? "";
                                    dgvDrivers.Rows[i].Cells["colVehicleNo"].Value = drivers[i].VehicleNo ?? "";
                                }
                            }
                        }
                        catch { }
                    }

                    string additionalJson = row["AdditionalAmountsJson"]?.ToString();
                    if (!string.IsNullOrEmpty(additionalJson))
                    {
                        try
                        {
                            var amounts = JsonSerializer.Deserialize<List<AdditionalAmountItem>>(additionalJson);
                            foreach (var amt in amounts) { dgvAdditionalAmounts.Rows.Add(amt.ItemName, amt.Amount); }
                            CalculateAdditionalAmounts();
                        }
                        catch { }
                    }
                    UpdateTotalFees();
                    UpdateTotalAmount();
                    MessageBox.Show($"✅ تم تحميل البيان رقم: {statementNo}\nتم تحميل الأصناف والسائقين والمبالغ الإضافية", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex) { MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private string GetJsonValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null) { return prop.GetString(); }
            return "";
        }

        private decimal GetJsonDecimal(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                if (prop.ValueKind == JsonValueKind.Number) { return prop.GetDecimal(); }
                else if (decimal.TryParse(prop.GetString(), out var result)) { return result; }
            }
            return 0;
        }

        // ==================== ✅ إنشاء جدول سجل الاعتمادات ====================
        private void EnsureApprovalLogTable()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS ApprovalLog (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            StatementNo TEXT NOT NULL,
            PortName TEXT NOT NULL,
            LevelId INTEGER NOT NULL,
            LevelNo INTEGER NOT NULL,
            LevelName TEXT NOT NULL,
            UserId INTEGER NOT NULL,
            UserName TEXT NOT NULL,
            ApprovalStatus TEXT NOT NULL,
            Notes TEXT,
            ApprovalDate TEXT DEFAULT (datetime('now','localtime')),
            UNIQUE(StatementNo, PortName, LevelId)
        )");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ إنشاء جدول سجل الاعتمادات: {ex.Message}");
            }
        }

        private void TxtUnloadFrom_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9) { e.SuppressKeyPress = true; ShowUnloadFromSources(); }
        }

        private void ShowUnloadFromSources()
        {
            Form selectForm = new Form();
            selectForm.Text = "اختر مصدر الإنزال";
            selectForm.Size = new Size(500, 400);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 40;
            dgv.Columns.Add("colScreenName", "اسم الشاشة");
            dgv.Columns.Add("colDescription", "الوصف");
            dgv.Rows.Add("طلب ترسيم", "البيانات المعتمدة من شاشة طلب الترسيم");
            if (UserManager.IsAdmin()) { dgv.Rows.Add("تخليص مباشر", "إضافة تخليص جمركي مباشر (للأدمن فقط)"); }

            dgv.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string selectedScreen = dgv.Rows[ev.RowIndex].Cells["colScreenName"].Value?.ToString();
                    txtUnloadFrom.Text = selectedScreen;
                    if (selectedScreen == "طلب ترسيم") ShowApprovedTariffRequests();
                    else if (selectedScreen == "تخليص مباشر" && UserManager.IsAdmin()) MessageBox.Show("ℹ️ تم اختيار التخليص المباشر\nيمكنك الآن إضافة بيان جديد مباشرة", "معلومة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    selectForm.Close();
                }
            };
            dgv.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.Enter && dgv.CurrentRow != null)
                {
                    ev.SuppressKeyPress = true;
                    string selectedScreen = dgv.CurrentRow.Cells["colScreenName"].Value?.ToString();
                    txtUnloadFrom.Text = selectedScreen;
                    if (selectedScreen == "طلب ترسيم") ShowApprovedTariffRequests();
                    else if (selectedScreen == "تخليص مباشر" && UserManager.IsAdmin()) MessageBox.Show("ℹ️ تم اختيار التخليص المباشر\nيمكنك الآن إضافة بيان جديد مباشرة", "معلومة", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    selectForm.Close();
                }
            };
            selectForm.Controls.Add(dgv);
            selectForm.ShowDialog();
        }
    }
}