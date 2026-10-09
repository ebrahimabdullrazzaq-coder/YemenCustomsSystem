using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace YemenCustomsSystem
{
    public partial class StockTransferForm : Form
    {
        private List<StockTransferRecord> transfers = new List<StockTransferRecord>();
        private List<WarehousesForm.WarehouseRecord> allWarehouses = new List<WarehousesForm.WarehouseRecord>();
        private List<CustomsClearanceForm.ClearanceRecord> allClearances = new List<CustomsClearanceForm.ClearanceRecord>();
        private List<StockMovementRecord> stockMovements = new List<StockMovementRecord>();

        public WarehousesForm warehousesForm;

        // تعريف الحقول العلوية
        private TextBox txtStatementNo;
        private DateTimePicker dtpTransferDate;
        private TextBox txtSourcePort;
        private ComboBox cmbTargetWarehouse;
        private TextBox txtNotes;
        private Button btnLoadItems, btnExecuteTransfer, btnViewMovements, btnFixData;
        private DataGridView dgvData;
        private Label lblTransferStatus;

        // شريط الأدوات
        private ToolStrip toolStrip1;
        private ToolStripButton btnSave, btnNew, btnDelete, btnSearch, btnExport, btnExit;
        private ToolStripButton btnHome;
        private ToolStripSeparator toolStripSeparator1, toolStripSeparator2, toolStripSeparator3;
        private ToolStripSeparator toolStripSeparatorHome;

        public StockTransferForm()
        {
            CreateControlsManually();
            InitializeGrid();
            LoadWarehousesFromDatabase();   // ✅ تحميل من قاعدة البيانات
            LoadClearancesFromDatabase();   // ✅ تحميل من قاعدة البيانات
            LoadMovementsFromDatabase();    // ✅ تحميل من قاعدة البيانات
            LoadTransfersFromDatabase();    // ✅ تحميل من قاعدة البيانات
            ClearFields();
        }

        // ==================== نماذج البيانات ====================
        public class StockTransferRecord
        {
            public int Id { get; set; }
            public string TransferNumber { get; set; }
            public DateTime TransferDate { get; set; }
            public string StatementNo { get; set; }
            public string SourcePort { get; set; }
            public string TargetWarehouse { get; set; }
            public string Notes { get; set; }
            public string Status { get; set; }
            public DateTime CreatedDate { get; set; }
            public List<TransferItem> Items { get; set; }
        }

        public class TransferItem
        {
            public int Id { get; set; }
            public string ProductName { get; set; }
            public decimal OriginalQty { get; set; }
            public decimal TransferQty { get; set; }
            public decimal RemainingQty { get; set; }
            public string Unit { get; set; }
            public decimal TotalValue { get; set; }
        }

        public class StockMovementRecord
        {
            public int Id { get; set; }
            public string StatementNo { get; set; }
            public string ProductName { get; set; }
            public decimal TransferQty { get; set; }
            public string FromLocation { get; set; }
            public string ToLocation { get; set; }
            public DateTime TransferDate { get; set; }
            public string TransferNumber { get; set; }
            public string Notes { get; set; }
        }

        // ==================== ✅ تحميل البيانات من قاعدة البيانات ====================
        private void LoadWarehousesFromDatabase()
        {
            try
            {
                cmbTargetWarehouse.Items.Clear();
                cmbTargetWarehouse.Items.Add("-- اختر مخزن --");

                // ✅ قراءة جميع المخازن بدون شرط صارم على كلمة "نشط"
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT WarehouseName, Status FROM Warehouses ORDER BY WarehouseName");

                if (dt != null && dt.Rows.Count > 0)
                {
                    int activeCount = 0;
                    foreach (DataRow row in dt.Rows)
                    {
                        string status = row["Status"]?.ToString()?.Trim() ?? "";

                        // ✅ شرط مرن للتحقق من أن المخزن نشط
                        if (status.Contains("نشط") || status == "1" || status.ToLower() == "active" || string.IsNullOrEmpty(status))
                        {
                            cmbTargetWarehouse.Items.Add(row["WarehouseName"].ToString());
                            activeCount++;
                        }
                    }

                    if (activeCount > 0)
                    {
                        cmbTargetWarehouse.SelectedIndex = 0;
                        Console.WriteLine($"✅ تم تحميل {activeCount} مخزن نشط");
                    }
                    else
                    {
                        cmbTargetWarehouse.Items.Add("-- لا توجد مخازن نشطة --");
                        cmbTargetWarehouse.SelectedIndex = 0;
                    }
                }
                else
                {
                    cmbTargetWarehouse.Items.Add("-- لا توجد مخازن في القاعدة --");
                    cmbTargetWarehouse.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحميل المخازن:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbTargetWarehouse.Items.Add("-- خطأ في التحميل --");
                cmbTargetWarehouse.SelectedIndex = 0;
            }
        }
        private void LoadClearancesFromDatabase()
        {
            try
            {
                allClearances.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM CustomsClearance ORDER BY StatementDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allClearances.Add(new CustomsClearanceForm.ClearanceRecord
                        {
                            StatementNo = row["StatementNo"]?.ToString(),
                            PortName = row["PortName"]?.ToString(),
                            StatementDate = row["StatementDate"] != DBNull.Value ? Convert.ToDateTime(row["StatementDate"]) : DateTime.Now,
                            ClientName = row["ClientName"]?.ToString(),
                            ProductName = row["ProductName"]?.ToString(),
                            Quantity = row["Quantity"] != DBNull.Value ? Convert.ToDecimal(row["Quantity"]) : 0,
                            Total = row["Total"] != DBNull.Value ? Convert.ToDecimal(row["Total"]) : 0,
                            Unit = row["Unit"]?.ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل بيانات التخليص: {ex.Message}"); }
        }

        private void LoadTransfersFromDatabase()
        {
            try
            {
                transfers.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM StockTransfers ORDER BY CreatedDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        transfers.Add(new StockTransferRecord
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            TransferNumber = row["TransferNumber"]?.ToString(),
                            TransferDate = row["TransferDate"] != DBNull.Value ? Convert.ToDateTime(row["TransferDate"]) : DateTime.Now,
                            StatementNo = row["StatementNo"]?.ToString(),
                            SourcePort = row["SourcePort"]?.ToString(),
                            TargetWarehouse = row["TargetWarehouse"]?.ToString(),
                            Notes = row["Notes"]?.ToString(),
                            Status = row["Status"]?.ToString(),
                            CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now,
                            Items = new List<TransferItem>() // يمكن تحميلها لاحقاً عند الحاجة
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل التحويلات: {ex.Message}"); }
        }

        private void LoadMovementsFromDatabase()
        {
            try
            {
                stockMovements.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM StockMovements ORDER BY TransferDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        stockMovements.Add(new StockMovementRecord
                        {
                            Id = Convert.ToInt32(row["Id"]),
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

        // ==================== ✅ فتح نافذة اختيار رقم البيان (F9) ====================
        private void OpenStatementSelectionForm()
        {
            LoadClearancesFromDatabase(); // تحديث البيانات قبل الفتح

            if (allClearances == null || allClearances.Count == 0)
            {
                MessageBox.Show("📭 لا توجد بيانات تخليص محفوظة\n\nالرجاء حفظ بيانات في شاشة التخليص الجمركي أولاً",
                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var groupedStatements = allClearances
                .GroupBy(c => c.StatementNo)
                .Select(g => new
                {
                    StatementNo = g.Key,
                    PortName = g.First().PortName,
                    StatementDate = g.First().StatementDate,
                    ClientName = g.First().ClientName,
                    ItemsCount = g.Count(),
                    TotalQty = g.Sum(r => r.Quantity),
                    TotalValue = g.Sum(r => r.Total)
                })
                .OrderByDescending(g => g.StatementDate)
                .ToList();

            if (groupedStatements.Count == 0)
            {
                MessageBox.Show("📭 لا توجد بيانات تخليص صالحة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form selectionForm = new Form();
            selectionForm.Text = $"📋 اختر رقم البيان ({groupedStatements.Count} بيان)";
            selectionForm.Size = new Size(1100, 550);
            selectionForm.StartPosition = FormStartPosition.CenterParent;
            selectionForm.RightToLeft = RightToLeft.Yes;
            selectionForm.RightToLeftLayout = true;
            selectionForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectionForm.MaximizeBox = false;
            selectionForm.MinimizeBox = false;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 50;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);

            Label lblSearch = new Label();
            lblSearch.Text = "عرض";
            lblSearch.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblSearch.Location = new Point(950, 15);
            lblSearch.Size = new Size(80, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            TextBox txtSearch = new TextBox();
            txtSearch.Font = new Font("Tahoma", 10);
            txtSearch.Location = new Point(700, 15);
            txtSearch.Size = new Size(240, 25);
            txtSearch.TextAlign = HorizontalAlignment.Right;

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);

            DataGridView dgvStatements = new DataGridView();
            dgvStatements.Dock = DockStyle.Fill;
            dgvStatements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvStatements.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvStatements.MultiSelect = false;
            dgvStatements.ReadOnly = true;
            dgvStatements.RightToLeft = RightToLeft.Yes;
            dgvStatements.AllowUserToAddRows = false;
            dgvStatements.BackgroundColor = Color.White;
            dgvStatements.EnableHeadersVisualStyles = false;
            dgvStatements.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvStatements.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvStatements.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvStatements.ColumnHeadersHeight = 45;
            dgvStatements.RowTemplate.Height = 35;
            dgvStatements.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            dgvStatements.Columns.Add("colStatementNo", "رقم البيان");
            dgvStatements.Columns.Add("colPortName", "المنفذ");
            dgvStatements.Columns.Add("colStatementDate", "التاريخ");
            dgvStatements.Columns.Add("colClientName", "العميل");
            dgvStatements.Columns.Add("colItemsCount", "عدد الأصناف");
            dgvStatements.Columns.Add("colTotalQty", "إجمالي الكمية");
            dgvStatements.Columns.Add("colTotalValue", "إجمالي الرسوم");

            dgvStatements.Columns["colTotalQty"].DefaultCellStyle.Format = "N2";
            dgvStatements.Columns["colTotalValue"].DefaultCellStyle.Format = "N2";
            dgvStatements.Columns["colTotalQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvStatements.Columns["colTotalValue"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            foreach (var stmt in groupedStatements)
            {
                dgvStatements.Rows.Add(
                    stmt.StatementNo,
                    stmt.PortName,
                    stmt.StatementDate.ToString("yyyy-MM-dd"),
                    stmt.ClientName,
                    stmt.ItemsCount,
                    stmt.TotalQty,
                    stmt.TotalValue
                );
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
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(100, 10);
            btnCancel.DialogResult = DialogResult.Cancel;

            pnlButtons.Controls.Add(btnOK);
            pnlButtons.Controls.Add(btnCancel);

            selectionForm.Controls.Add(dgvStatements);
            selectionForm.Controls.Add(pnlSearch);
            selectionForm.Controls.Add(pnlButtons);
            selectionForm.AcceptButton = btnOK;
            selectionForm.CancelButton = btnCancel;

            txtSearch.TextChanged += (s, ev) =>
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                foreach (DataGridViewRow r in dgvStatements.Rows)
                {
                    string stmtNo = r.Cells["colStatementNo"].Value?.ToString()?.ToLower() ?? "";
                    string portName = r.Cells["colPortName"].Value?.ToString()?.ToLower() ?? "";
                    string clientName = r.Cells["colClientName"].Value?.ToString()?.ToLower() ?? "";

                    r.Visible = string.IsNullOrEmpty(searchText) || stmtNo.Contains(searchText) || portName.Contains(searchText) || clientName.Contains(searchText);
                }
            };

            dgvStatements.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) btnOK.PerformClick(); };

            if (selectionForm.ShowDialog() == DialogResult.OK && dgvStatements.SelectedRows.Count > 0)
            {
                string selectedStatementNo = dgvStatements.SelectedRows[0].Cells["colStatementNo"].Value?.ToString();
                if (!string.IsNullOrEmpty(selectedStatementNo))
                {
                    txtStatementNo.Text = selectedStatementNo;
                    LoadItemsByStatementNo();
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F9 && txtStatementNo != null && txtStatementNo.Focused)
            {
                OpenStatementSelectionForm();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ==================== تهيئة الجدول ====================
        private void InitializeGrid()
        {
            if (dgvData == null) return;
            dgvData.Columns.Clear();

            dgvData.Columns.Add("colId", "م");
            dgvData.Columns.Add("colProductName", "اسم السلعة");
            dgvData.Columns.Add("colOriginalQty", "الكمية الواردة");
            dgvData.Columns.Add("colAlreadyTransferred", "الكمية المنقولة سابقاً");
            dgvData.Columns.Add("colAvailableQty", "الكمية المتاحة");
            dgvData.Columns.Add("colTransferQty", "الكمية المراد نقلها");
            dgvData.Columns.Add("colRemainingAfter", "المتبقية بعد النقل");
            dgvData.Columns.Add("colUnit", "الوحدة");

            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                col.ReadOnly = col.Name != "colTransferQty";
                if (col.Name.Contains("Qty"))
                {
                    col.DefaultCellStyle.Format = "N2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            dgvData.Columns["colAvailableQty"].DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
            dgvData.Columns["colAvailableQty"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dgvData.Columns["colRemainingAfter"].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
            dgvData.Columns["colRemainingAfter"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dgvData.AllowUserToAddRows = true;
            dgvData.AllowUserToDeleteRows = true;
            dgvData.RightToLeft = RightToLeft.Yes;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvData.ScrollBars = ScrollBars.Both;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.Fixed3D;
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.ColumnHeadersHeight = 45;
            dgvData.RowsDefaultCellStyle.BackColor = Color.White;
            dgvData.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvData.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvData.RowTemplate.Height = 35;

            dgvData.CellValueChanged += DgvData_CellValueChanged;
            dgvData.EditingControlShowing += DgvData_EditingControlShowing;
        }

        private void DgvData_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dgvData.CurrentCell != null && dgvData.CurrentCell.ColumnIndex == dgvData.Columns["colTransferQty"].Index)
            {
                if (e.Control is TextBox txt)
                {
                    txt.KeyPress -= Txt_KeyPress;
                    txt.KeyPress += Txt_KeyPress;
                }
            }
        }

        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }
        }

        private void DgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.ColumnIndex == dgvData.Columns["colTransferQty"].Index)
            {
                ValidateAndCalculateRow(e.RowIndex);
            }
        }

        private void ValidateAndCalculateRow(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.IsNewRow) return;

            decimal originalQty = ParseDecimal(row.Cells["colOriginalQty"].Value?.ToString());
            decimal alreadyTransferred = ParseDecimal(row.Cells["colAlreadyTransferred"].Value?.ToString());
            decimal availableQty = ParseDecimal(row.Cells["colAvailableQty"].Value?.ToString());
            decimal transferQty = ParseDecimal(row.Cells["colTransferQty"].Value?.ToString());

            if (transferQty > availableQty)
            {
                MessageBox.Show($"⚠️ الكمية المنقولة ({transferQty:N2}) لا يمكن أن تتجاوز الكمية المتاحة ({availableQty:N2})\nالسلعة: {row.Cells["colProductName"].Value}",
                    "تنبيه - كمية غير مسموحة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                row.Cells["colTransferQty"].Value = 0;
                transferQty = 0;
            }

            if (transferQty < 0)
            {
                row.Cells["colTransferQty"].Value = 0;
                transferQty = 0;
            }

            decimal remainingAfter = availableQty - transferQty;
            if (remainingAfter < 0) remainingAfter = 0;
            row.Cells["colRemainingAfter"].Value = remainingAfter;

            row.DefaultCellStyle.BackColor = transferQty > availableQty ? Color.FromArgb(255, 200, 200) : (transferQty > 0 ? Color.FromArgb(200, 255, 200) : Color.White);

            UpdateSummary();
        }

        private void UpdateSummary()
        {
            decimal totalTransferQty = 0;
            bool hasInvalidQty = false;

            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.IsNewRow) continue;
                decimal availableQty = ParseDecimal(row.Cells["colAvailableQty"].Value?.ToString());
                decimal transferQty = ParseDecimal(row.Cells["colTransferQty"].Value?.ToString());
                totalTransferQty += transferQty;
                if (transferQty > availableQty) hasInvalidQty = true;
            }

            lblTransferStatus.Text = $"📊 إجمالي الكمية المنقولة: {totalTransferQty:N2}";
            if (hasInvalidQty)
            {
                lblTransferStatus.Text += " | ⚠️ توجد كميات غير مسموحة";
                lblTransferStatus.ForeColor = Color.Red;
                btnExecuteTransfer.Enabled = false;
                btnSave.Enabled = false;
            }
            else
            {
                lblTransferStatus.ForeColor = Color.FromArgb(33, 150, 243);
                btnExecuteTransfer.Enabled = true;
                btnSave.Enabled = true;
            }
        }

        private void LoadItemsByStatementNo()
        {
            string statementNo = txtStatementNo.Text.Trim();
            if (string.IsNullOrWhiteSpace(statementNo))
            {
                MessageBox.Show("⚠️ الرجاء إدخال رقم البيان", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var clearanceRecords = allClearances.Where(c => c.StatementNo == statementNo).ToList();
            if (clearanceRecords.Count == 0)
            {
                MessageBox.Show($"❌ لا توجد بيانات لرقم البيان: {statementNo}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var firstRecord = clearanceRecords.First();
            txtSourcePort.Text = firstRecord.PortName;

            dgvData.Rows.Clear();
            int idx = 1;

            foreach (var item in clearanceRecords)
            {
                var previousMovements = stockMovements
                    .Where(m => m.StatementNo == statementNo && m.ProductName == item.ProductName && m.FromLocation == firstRecord.PortName)
                    .ToList();

                decimal alreadyTransferred = previousMovements.Sum(m => m.TransferQty);
                decimal availableQty = item.Quantity - alreadyTransferred;
                if (availableQty < 0) availableQty = 0;

                dgvData.Rows.Add(
                    idx++,
                    item.ProductName,
                    item.Quantity,
                    alreadyTransferred,
                    availableQty,
                    0,
                    availableQty,
                    item.Unit ?? "كرتون"
                );
            }

            lblTransferStatus.Text = $"✅ تم تحميل {clearanceRecords.Count} صنف من رقم البيان: {statementNo}";
            lblTransferStatus.ForeColor = Color.Green;
            btnExecuteTransfer.Enabled = true;
        }

        private void ViewProductMovements()
        {
            if (dgvData.CurrentRow == null || dgvData.CurrentRow.IsNewRow)
            {
                MessageBox.Show("⚠️ الرجاء تحديد صنف أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string statementNo = txtStatementNo.Text.Trim();
            string productName = dgvData.CurrentRow.Cells["colProductName"].Value?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(statementNo) || string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("⚠️ لم يتم تحديد رقم البيان أو اسم الصنف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var movements = stockMovements
                .Where(m => m.StatementNo == statementNo && m.ProductName == productName)
                .OrderByDescending(m => m.TransferDate)
                .ToList();

            if (movements.Count == 0)
            {
                MessageBox.Show($"📭 لا توجد حركات سابقة للصنف: {productName}", "سجل الحركات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form movementsForm = new Form();
            movementsForm.Text = $"📜 سجل حركات الصنف: {productName}";
            movementsForm.Size = new Size(900, 400);
            movementsForm.StartPosition = FormStartPosition.CenterParent;
            movementsForm.RightToLeft = RightToLeft.Yes;

            DataGridView dgvMovements = new DataGridView();
            dgvMovements.Dock = DockStyle.Fill;
            dgvMovements.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvMovements.RightToLeft = RightToLeft.Yes;
            dgvMovements.ReadOnly = true;
            dgvMovements.AllowUserToAddRows = false;
            dgvMovements.BackgroundColor = Color.White;
            dgvMovements.RowHeadersVisible = false;

            dgvMovements.Columns.Add("colDate", "التاريخ");
            dgvMovements.Columns.Add("colQty", "الكمية المنقولة");
            dgvMovements.Columns.Add("colFrom", "من");
            dgvMovements.Columns.Add("colTo", "إلى");
            dgvMovements.Columns.Add("colTransferNo", "رقم التحويل");

            decimal totalTransferred = 0;
            foreach (var m in movements)
            {
                totalTransferred += m.TransferQty;
                dgvMovements.Rows.Add(
                    m.TransferDate.ToString("yyyy-MM-dd"),
                    m.TransferQty.ToString("N2"),
                    m.FromLocation,
                    m.ToLocation,
                    m.TransferNumber
                );
            }

            movementsForm.Controls.Add(dgvMovements);

            Label lblTotal = new Label();
            lblTotal.Text = $"📊 إجمالي الكميات المنقولة: {totalTransferred:N2}";
            lblTotal.Dock = DockStyle.Bottom;
            lblTotal.Height = 40;
            lblTotal.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblTotal.TextAlign = ContentAlignment.MiddleRight;
            lblTotal.Padding = new Padding(0, 0, 10, 0);
            lblTotal.BackColor = Color.FromArgb(240, 248, 255);
            movementsForm.Controls.Add(lblTotal);

            movementsForm.ShowDialog();
        }

        private void FixCorruptedData()
        {
            bool hasErrors = false;
            string errorReport = "";
            int fixedCount = 0;

            foreach (var clearance in allClearances)
            {
                var movements = stockMovements
                    .Where(m => m.StatementNo == clearance.StatementNo && m.ProductName == clearance.ProductName)
                    .ToList();

                decimal totalTransferred = movements.Sum(m => m.TransferQty);

                if (totalTransferred > clearance.Quantity)
                {
                    hasErrors = true;
                    errorReport += $"\n• {clearance.ProductName} (بيان {clearance.StatementNo}): منقول {totalTransferred} > وارد {clearance.Quantity} (الفرق: {totalTransferred - clearance.Quantity})";
                    fixedCount++;
                }
            }

            if (hasErrors)
            {
                MessageBox.Show($"⚠️ تم اكتشاف أخطاء في بيانات المخزون:{errorReport}\n\n" +
                    $"عدد الأصناف التي بها مشاكل: {fixedCount}\n\n" +
                    "يرجى مراجعة سجل الحركات في قاعدة البيانات وتصحيحها.",
                    "تقرير فحص البيانات", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                MessageBox.Show("✅ جميع بيانات المخزون صحيحة! لا توجد أخطاء.", "فحص البيانات", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateWarehousesForm(string warehouseName, string statementNo, List<TransferItem> transferItems)
        {
            if (warehousesForm != null)
            {
                foreach (var item in transferItems)
                {
                    warehousesForm.AddReceivedFromPort(
                        warehouseName,
                        statementNo,
                        item.ProductName,
                        item.TransferQty,
                        item.Unit ?? "كرتون"
                    );
                }
            }
        }

        // ==================== ✅ تنفيذ التحويل وحفظه في قاعدة البيانات ====================
        private void ExecuteTransfer()
        {
            string statementNo = txtStatementNo.Text.Trim();
            string targetWarehouse = cmbTargetWarehouse.SelectedItem?.ToString() ?? "";
            string sourcePort = txtSourcePort.Text.Trim();

            if (string.IsNullOrWhiteSpace(statementNo))
            {
                MessageBox.Show("⚠️ رقم البيان مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (targetWarehouse == "-- اختر مخزن --" || string.IsNullOrWhiteSpace(targetWarehouse))
            {
                MessageBox.Show("⚠️ الرجاء اختيار المخزن المستهدف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(sourcePort))
            {
                MessageBox.Show("⚠️ لم يتم تحديد منشأ البضاعة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<TransferItem> transferItems = new List<TransferItem>();
            bool hasTransfer = false;
            bool hasInvalidQty = false;
            string invalidProducts = "";

            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.IsNewRow) continue;

                decimal originalQty = ParseDecimal(row.Cells["colOriginalQty"].Value?.ToString());
                decimal alreadyTransferred = ParseDecimal(row.Cells["colAlreadyTransferred"].Value?.ToString());
                decimal transferQty = ParseDecimal(row.Cells["colTransferQty"].Value?.ToString());
                string productName = row.Cells["colProductName"].Value?.ToString() ?? "";

                if (transferQty > 0)
                {
                    if (alreadyTransferred + transferQty > originalQty)
                    {
                        hasInvalidQty = true;
                        invalidProducts += $"\n• {productName}: سيصبح المجموع {alreadyTransferred + transferQty} > {originalQty}";
                    }

                    hasTransfer = true;
                    transferItems.Add(new TransferItem
                    {
                        Id = transferItems.Count + 1,
                        ProductName = productName,
                        OriginalQty = originalQty,
                        TransferQty = transferQty,
                        RemainingQty = originalQty - (alreadyTransferred + transferQty),
                        Unit = row.Cells["colUnit"].Value?.ToString() ?? "كرتون",
                        TotalValue = 0
                    });
                }
            }

            if (hasInvalidQty)
            {
                MessageBox.Show($"⚠️ لا يمكن تنفيذ التحويل بسبب الكميات التالية:{invalidProducts}\n\nيرجى تصحيح الكميات ثم إعادة المحاولة.",
                    "تحويل غير مسموح", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!hasTransfer)
            {
                MessageBox.Show("⚠️ لم يتم إدخال أي كميات للنقل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal totalQty = transferItems.Sum(i => i.TransferQty);

            DialogResult confirm = MessageBox.Show($"✅ تأكيد التحويل\n\n" +
                $"رقم البيان: {statementNo}\n" +
                $"من: {sourcePort}\n" +
                $"إلى: {targetWarehouse}\n" +
                $"عدد الأصناف: {transferItems.Count}\n" +
                $"إجمالي الكمية: {totalQty:N2}\n\n" +
                $"هل أنت متأكد من تنفيذ التحويل؟",
                "تأكيد التحويل", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                // 1. توليد رقم تحويل جديد
                var maxIdObj = DatabaseHelper.ExecuteScalar("SELECT IFNULL(MAX(Id), 0) FROM StockTransfers");
                int maxId = Convert.ToInt32(maxIdObj);
                string transferNumber = $"TR-{DateTime.Now:yyyyMMdd}-{maxId + 1}";

                // 2. حفظ رأس التحويل في قاعدة البيانات
                var newTransferId = DatabaseHelper.ExecuteScalar(@"INSERT INTO StockTransfers 
                    (TransferNumber, TransferDate, StatementNo, SourcePort, TargetWarehouse, Notes, Status, CreatedDate)
                    VALUES (@TransferNumber, @TransferDate, @StatementNo, @SourcePort, @TargetWarehouse, @Notes, @Status, datetime('now'));
                   SELECT last_insert_rowid();",
                    DatabaseHelper.CreateParameter("@TransferNumber", transferNumber),
                    DatabaseHelper.CreateParameter("@TransferDate", dtpTransferDate.Value),
                    DatabaseHelper.CreateParameter("@StatementNo", statementNo),
                    DatabaseHelper.CreateParameter("@SourcePort", sourcePort),
                    DatabaseHelper.CreateParameter("@TargetWarehouse", targetWarehouse),
                    DatabaseHelper.CreateParameter("@Notes", txtNotes.Text.Trim()),
                    DatabaseHelper.CreateParameter("@Status", "مكتمل")
                );

                int transferId = Convert.ToInt32(newTransferId);

                // 3. حفظ تفاصيل الحركات في قاعدة البيانات
                foreach (var item in transferItems)
                {
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO StockMovements 
                        (StatementNo, ProductName, TransferQty, FromLocation, ToLocation, TransferDate, TransferNumber, Notes)
                        VALUES (@StatementNo, @ProductName, @TransferQty, @FromLocation, @ToLocation, @TransferDate, @TransferNumber, @Notes)",
                        DatabaseHelper.CreateParameter("@StatementNo", statementNo),
                        DatabaseHelper.CreateParameter("@ProductName", item.ProductName),
                        DatabaseHelper.CreateParameter("@TransferQty", item.TransferQty),
                        DatabaseHelper.CreateParameter("@FromLocation", sourcePort),
                        DatabaseHelper.CreateParameter("@ToLocation", targetWarehouse),
                        DatabaseHelper.CreateParameter("@TransferDate", dtpTransferDate.Value),
                        DatabaseHelper.CreateParameter("@TransferNumber", transferNumber),
                        DatabaseHelper.CreateParameter("@Notes", txtNotes.Text.Trim())
                    );
                }

                // 4. تحديث القوائم المحلية
                transfers.Add(new StockTransferRecord
                {
                    Id = transferId,
                    TransferNumber = transferNumber,
                    TransferDate = dtpTransferDate.Value,
                    StatementNo = statementNo,
                    SourcePort = sourcePort,
                    TargetWarehouse = targetWarehouse,
                    Notes = txtNotes.Text.Trim(),
                    Status = "مكتمل",
                    CreatedDate = DateTime.Now,
                    Items = transferItems
                });

                // إعادة تحميل الحركات لضمان التزامن
                LoadMovementsFromDatabase();

                UpdateWarehousesForm(targetWarehouse, statementNo, transferItems);

                MessageBox.Show($"✅ تم تنفيذ التحويل بنجاح\n" +
                    $"رقم التحويل: {transferNumber}\n" +
                    $"عدد الأصناف: {transferItems.Count}\n" +
                    $"إجمالي الكمية: {totalQty:N2}",
                    "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تنفيذ التحويل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== إنشاء الواجهة ====================
        private void CreateControlsManually()
        {
            this.Text = "📦 شاشة التحويل المخزني - نظام الجمارك اليمنية";
            this.Size = new Size(1400, 750);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            toolStrip1 = new ToolStrip();
            toolStrip1.RightToLeft = RightToLeft.Yes;
            toolStrip1.BackColor = Color.FromArgb(52, 73, 94);
            toolStrip1.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            btnHome = new ToolStripButton("🏠 الرئيسية");
            btnHome.ForeColor = Color.White;
            btnHome.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnHome.Click += btnHome_Click;

            toolStripSeparatorHome = new ToolStripSeparator();

            btnSave = new ToolStripButton("💾 حفظ التحويل");
            btnSave.ForeColor = Color.White;
            btnSave.Click += (s, e) => ExecuteTransfer();

            btnNew = new ToolStripButton("🆕 جديد");
            btnNew.ForeColor = Color.White;
            btnNew.Click += (s, e) => ClearFields();

            toolStripSeparator1 = new ToolStripSeparator();

            btnDelete = new ToolStripButton("🗑️ حذف");
            btnDelete.ForeColor = Color.White;
            btnDelete.Click += (s, e) => btnDelete_Click(s, e);

            toolStripSeparator2 = new ToolStripSeparator();

            btnSearch = new ToolStripButton("عرض");
            btnSearch.ForeColor = Color.White;
            btnSearch.Click += (s, e) => btnSearch_Click(s, e);

            toolStripSeparator3 = new ToolStripSeparator();

            btnExport = new ToolStripButton("📤 تصدير");
            btnExport.ForeColor = Color.White;
            btnExport.Click += (s, e) => btnExport_Click(s, e);

            btnExit = new ToolStripButton("🚪 خروج");
            btnExit.ForeColor = Color.White;
            btnExit.Click += (s, e) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] {
                btnHome, toolStripSeparatorHome,
                btnSave, btnNew, toolStripSeparator1,
                btnDelete, toolStripSeparator2,
                btnSearch, toolStripSeparator3,
                btnExport, btnExit
            });

            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 180;
            pnlHeader.BackColor = Color.FromArgb(240, 248, 255);
            pnlHeader.Padding = new Padding(10);

            Label lblStatementNo = new Label();
            lblStatementNo.Text = "رقم البيان :";
            lblStatementNo.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblStatementNo.Location = new Point(1150, 15);
            lblStatementNo.Size = new Size(120, 25);
            lblStatementNo.TextAlign = ContentAlignment.MiddleRight;

            txtStatementNo = new TextBox();
            txtStatementNo.Font = new Font("Tahoma", 10);
            txtStatementNo.Location = new Point(960, 15);
            txtStatementNo.Size = new Size(180, 27);
            txtStatementNo.TextAlign = HorizontalAlignment.Right;
            txtStatementNo.BackColor = Color.FromArgb(255, 255, 190);

            Label lblTransferDate = new Label();
            lblTransferDate.Text = "تاريخ التحويل:";
            lblTransferDate.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblTransferDate.Location = new Point(810, 15);
            lblTransferDate.Size = new Size(100, 25);
            lblTransferDate.TextAlign = ContentAlignment.MiddleRight;

            dtpTransferDate = new DateTimePicker();
            dtpTransferDate.Format = DateTimePickerFormat.Short;
            dtpTransferDate.Font = new Font("Tahoma", 10);
            dtpTransferDate.Location = new Point(620, 15);
            dtpTransferDate.Size = new Size(180, 27);
            dtpTransferDate.Value = DateTime.Now;

            btnLoadItems = new Button();
            btnLoadItems.Text = "📥 تحميل الأصناف";
            btnLoadItems.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnLoadItems.BackColor = Color.FromArgb(33, 150, 243);
            btnLoadItems.ForeColor = Color.White;
            btnLoadItems.FlatStyle = FlatStyle.Flat;
            btnLoadItems.Location = new Point(450, 15);
            btnLoadItems.Size = new Size(140, 30);
            btnLoadItems.Click += (s, e) => LoadItemsByStatementNo();

            Label lblSourcePort = new Label();
            lblSourcePort.Text = "منشأ البضاعة (المنفذ):";
            lblSourcePort.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblSourcePort.Location = new Point(1150, 55);
            lblSourcePort.Size = new Size(160, 25);
            lblSourcePort.TextAlign = ContentAlignment.MiddleRight;

            txtSourcePort = new TextBox();
            txtSourcePort.Font = new Font("Tahoma", 10);
            txtSourcePort.Location = new Point(960, 55);
            txtSourcePort.Size = new Size(180, 27);
            txtSourcePort.ReadOnly = true;
            txtSourcePort.BackColor = Color.FromArgb(255, 255, 190);
            txtSourcePort.TextAlign = HorizontalAlignment.Right;

            Label lblTargetWarehouse = new Label();
            lblTargetWarehouse.Text = "إلى مخزن:";
            lblTargetWarehouse.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblTargetWarehouse.Location = new Point(810, 55);
            lblTargetWarehouse.Size = new Size(90, 25);
            lblTargetWarehouse.TextAlign = ContentAlignment.MiddleRight;

            cmbTargetWarehouse = new ComboBox();
            cmbTargetWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTargetWarehouse.Font = new Font("Tahoma", 10);
            cmbTargetWarehouse.Location = new Point(620, 55);
            cmbTargetWarehouse.Size = new Size(180, 27);
            cmbTargetWarehouse.Items.Add("-- اختر مخزن --");
            cmbTargetWarehouse.SelectedIndex = 0;

            btnExecuteTransfer = new Button();
            btnExecuteTransfer.Text = "✅ تنفيذ التحويل";
            btnExecuteTransfer.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnExecuteTransfer.BackColor = Color.FromArgb(76, 175, 80);
            btnExecuteTransfer.ForeColor = Color.White;
            btnExecuteTransfer.FlatStyle = FlatStyle.Flat;
            btnExecuteTransfer.Location = new Point(450, 55);
            btnExecuteTransfer.Size = new Size(140, 30);
            btnExecuteTransfer.Click += (s, e) => ExecuteTransfer();

            btnViewMovements = new Button();
            btnViewMovements.Text = "📜 سجل حركات الصنف";
            btnViewMovements.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnViewMovements.BackColor = Color.FromArgb(255, 152, 0);
            btnViewMovements.ForeColor = Color.White;
            btnViewMovements.FlatStyle = FlatStyle.Flat;
            btnViewMovements.Location = new Point(280, 55);
            btnViewMovements.Size = new Size(150, 30);
            btnViewMovements.Click += (s, e) => ViewProductMovements();

            btnFixData = new Button();
            btnFixData.Text = "🔧 فحص البيانات";
            btnFixData.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnFixData.BackColor = Color.FromArgb(255, 87, 34);
            btnFixData.ForeColor = Color.White;
            btnFixData.FlatStyle = FlatStyle.Flat;
            btnFixData.Location = new Point(110, 55);
            btnFixData.Size = new Size(150, 30);
            btnFixData.Click += (s, e) => FixCorruptedData();

            Label lblNotes = new Label();
            lblNotes.Text = "ملاحظات:";
            lblNotes.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblNotes.Location = new Point(1150, 95);
            lblNotes.Size = new Size(90, 25);
            lblNotes.TextAlign = ContentAlignment.MiddleRight;

            txtNotes = new TextBox();
            txtNotes.Font = new Font("Tahoma", 10);
            txtNotes.Location = new Point(960, 95);
            txtNotes.Size = new Size(300, 27);
            txtNotes.TextAlign = HorizontalAlignment.Right;

            lblTransferStatus = new Label();
            lblTransferStatus.Text = "📊 جاهز للتحويل";
            lblTransferStatus.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblTransferStatus.ForeColor = Color.FromArgb(33, 150, 243);
            lblTransferStatus.Location = new Point(450, 100);
            lblTransferStatus.Size = new Size(600, 25);
            lblTransferStatus.TextAlign = ContentAlignment.MiddleRight;

            pnlHeader.Controls.Add(lblStatementNo);
            pnlHeader.Controls.Add(txtStatementNo);
            pnlHeader.Controls.Add(lblTransferDate);
            pnlHeader.Controls.Add(dtpTransferDate);
            pnlHeader.Controls.Add(btnLoadItems);
            pnlHeader.Controls.Add(lblSourcePort);
            pnlHeader.Controls.Add(txtSourcePort);
            pnlHeader.Controls.Add(lblTargetWarehouse);
            pnlHeader.Controls.Add(cmbTargetWarehouse);
            pnlHeader.Controls.Add(btnExecuteTransfer);
            pnlHeader.Controls.Add(btnViewMovements);
            pnlHeader.Controls.Add(btnFixData);
            pnlHeader.Controls.Add(lblNotes);
            pnlHeader.Controls.Add(txtNotes);
            pnlHeader.Controls.Add(lblTransferStatus);

            dgvData = new DataGridView();
            dgvData.Dock = DockStyle.Fill;

            this.Controls.Add(dgvData);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(toolStrip1);
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm mainForm = this.MdiParent as MainForm ?? Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm != null && !mainForm.IsDisposed)
                {
                    var children = mainForm.MdiChildren.ToList();
                    foreach (Form child in children)
                    {
                        if (!child.IsDisposed && child != this) child.Close();
                    }
                    this.Close();
                    mainForm.ShowWelcome();
                    mainForm.WindowState = FormWindowState.Maximized;
                    mainForm.Activate();
                    mainForm.BringToFront();
                }
                else this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void ClearFields()
        {
            txtStatementNo.Clear();
            dtpTransferDate.Value = DateTime.Now;
            txtSourcePort.Clear();
            cmbTargetWarehouse.SelectedIndex = 0;
            txtNotes.Clear();
            dgvData.Rows.Clear();
            lblTransferStatus.Text = "📊 جاهز للتحويل";
            lblTransferStatus.ForeColor = Color.FromArgb(33, 150, 243);
            btnExecuteTransfer.Enabled = true;
            btnSave.Enabled = true;
            txtStatementNo.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ الرجاء تحديد السجل المراد حذفه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("🗑️ هل أنت متأكد من حذف السجل المحدد؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgvData.SelectedRows)
                {
                    if (!row.IsNewRow) dgvData.Rows.Remove(row);
                }
                UpdateSummary();
                MessageBox.Show("✅ تم حذف السجل بنجاح", "حذف", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (transfers.Count == 0)
            {
                MessageBox.Show("لا توجد تحويلات محفوظة للبحث عنها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ إنشاء نافذة عرض متكاملة مع بحث فوري
            using (Form viewForm = new Form())
            {
                viewForm.Text = "📋 عرض جميع التحويلات للبحث";
                viewForm.Size = new Size(1000, 550);
                viewForm.StartPosition = FormStartPosition.CenterParent;
                viewForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                viewForm.MaximizeBox = false;
                viewForm.MinimizeBox = false;
                viewForm.RightToLeft = RightToLeft.Yes;
                viewForm.RightToLeftLayout = true;

                // 1. لوحة علوية لحقل البحث
                Panel pnlTop = new Panel();
                pnlTop.Height = 60;
                pnlTop.Dock = DockStyle.Top;
                pnlTop.BackColor = Color.FromArgb(240, 248, 255);

                Label lblSearch = new Label();
                lblSearch.Text = "🔍 بحث برقم التحويل أو البيان:";
                lblSearch.Font = new Font("Tahoma", 10, FontStyle.Bold);
                lblSearch.Location = new Point(700, 15);
                lblSearch.Size = new Size(200, 25);
                lblSearch.TextAlign = ContentAlignment.MiddleRight;

                TextBox txtSearch = new TextBox();
                txtSearch.Font = new Font("Tahoma", 11);
                txtSearch.Location = new Point(400, 15);
                txtSearch.Size = new Size(290, 28);
                txtSearch.TextAlign = HorizontalAlignment.Right;
                txtSearch.BorderStyle = BorderStyle.FixedSingle;

                pnlTop.Controls.Add(lblSearch);
                pnlTop.Controls.Add(txtSearch);

                // 2. إنشاء جدول البيانات للعرض
                DataGridView dgvView = new DataGridView();
                dgvView.Dock = DockStyle.Fill;
                dgvView.AllowUserToAddRows = false;
                dgvView.AllowUserToDeleteRows = false;
                dgvView.ReadOnly = true;
                dgvView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvView.MultiSelect = false;
                dgvView.BackgroundColor = Color.White;
                dgvView.RowHeadersVisible = false;
                dgvView.EnableHeadersVisualStyles = false;

                // تنسيق الرأس
                dgvView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgvView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvView.ColumnHeadersHeight = 40;
                dgvView.RowTemplate.Height = 35;
                dgvView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

                // 3. إضافة الأعمدة الأساسية للعرض
                dgvView.Columns.Add("colTransferNumber", "رقم التحويل");
                dgvView.Columns.Add("colStatementNo", "رقم البيان");
                dgvView.Columns.Add("colTransferDate", "تاريخ التحويل");
                dgvView.Columns.Add("colSourcePort", "من (المنفذ)");
                dgvView.Columns.Add("colTargetWarehouse", "إلى (المخزن)");
                dgvView.Columns.Add("colStatus", "الحالة");

                // ضبط عرض الأعمدة
                dgvView.Columns["colTransferNumber"].Width = 150;
                dgvView.Columns["colStatementNo"].Width = 120;
                dgvView.Columns["colTransferDate"].Width = 120;
                dgvView.Columns["colSourcePort"].Width = 150;
                dgvView.Columns["colTargetWarehouse"].Width = 150;
                dgvView.Columns["colStatus"].Width = 100;

                // 4. تعبئة البيانات من القائمة
                foreach (var t in transfers)
                {
                    dgvView.Rows.Add(
                        t.TransferNumber,
                        t.StatementNo,
                        t.TransferDate.ToString("yyyy-MM-dd"),
                        t.SourcePort,
                        t.TargetWarehouse,
                        t.Status
                    );
                }

                // 5. ✅ منطق البحث (تصفية الصفوف فورياً أثناء الكتابة)
                txtSearch.TextChanged += (s, ev) =>
                {
                    string searchTerm = txtSearch.Text.Trim().ToLower();
                    foreach (DataGridViewRow row in dgvView.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string tNum = row.Cells["colTransferNumber"].Value?.ToString()?.ToLower() ?? "";
                        string sNum = row.Cells["colStatementNo"].Value?.ToString()?.ToLower() ?? "";

                        // إظهار الصف إذا تطابق مع رقم التحويل أو رقم البيان، أو إذا كان حقل البحث فارغاً
                        if (string.IsNullOrEmpty(searchTerm) || tNum.Contains(searchTerm) || sNum.Contains(searchTerm))
                        {
                            row.Visible = true;
                        }
                        else
                        {
                            row.Visible = false;
                        }
                    }
                };

                // 6. لوحة سفلية لزر الإغلاق
                Panel pnlBottom = new Panel();
                pnlBottom.Height = 60;
                pnlBottom.Dock = DockStyle.Bottom;
                pnlBottom.BackColor = Color.FromArgb(240, 248, 255);

                Button btnClose = new Button();
                btnClose.Text = "❌ إغلاق";
                btnClose.Font = new Font("Tahoma", 10, FontStyle.Bold);
                btnClose.BackColor = Color.FromArgb(158, 158, 158);
                btnClose.ForeColor = Color.White;
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.Size = new Size(120, 40);
                btnClose.Location = new Point((pnlBottom.Width - btnClose.Width) / 2, 10); // توسيط الزر
                btnClose.Anchor = AnchorStyles.Bottom;
                btnClose.Click += (s, ev) => viewForm.Close();

                pnlBottom.Controls.Add(btnClose);

                // 7. إضافة العناصر إلى النافذة بالترتيب الصحيح
                viewForm.Controls.Add(dgvView);   // يملأ المساحة المتبقية
                viewForm.Controls.Add(pnlTop);    // في الأعلى
                viewForm.Controls.Add(pnlBottom); // في الأسفل

                // 8. ✅ ميزة إضافية: النقر المزدوج على صف لتحميله في الشاشة الرئيسية
                dgvView.CellDoubleClick += (s, ev) =>
                {
                    if (ev.RowIndex >= 0)
                    {
                        string selectedTNum = dgvView.Rows[ev.RowIndex].Cells["colTransferNumber"].Value?.ToString();

                        // البحث عن التحويل في القائمة المحلية وتحميله
                        var transfer = transfers.FirstOrDefault(t => t.TransferNumber == selectedTNum);
                        if (transfer != null)
                        {
                            LoadTransferToForm(transfer);
                            viewForm.Close(); // إغلاق نافذة العرض بعد الاختيار
                        }
                    }
                };

                // عرض النافذة
                viewForm.ShowDialog();
            }
        }
        private void LoadTransferToForm(StockTransferRecord transfer)
        {
            txtStatementNo.Text = transfer.StatementNo;
            dtpTransferDate.Value = transfer.TransferDate;
            txtSourcePort.Text = transfer.SourcePort;
            cmbTargetWarehouse.SelectedItem = transfer.TargetWarehouse;
            txtNotes.Text = transfer.Notes;

            dgvData.Rows.Clear();
            int idx = 1;
            foreach (var item in transfer.Items)
            {
                decimal alreadyTransferred = item.OriginalQty - item.RemainingQty - item.TransferQty;
                if (alreadyTransferred < 0) alreadyTransferred = 0;

                decimal availableAtTime = item.RemainingQty + item.TransferQty;
                if (availableAtTime < 0) availableAtTime = 0;

                dgvData.Rows.Add(
                    idx++,
                    item.ProductName,
                    item.OriginalQty,
                    alreadyTransferred,
                    availableAtTime,
                    item.TransferQty,
                    item.RemainingQty,
                    item.Unit
                );
            }
            UpdateSummary();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (transfers.Count == 0 && dgvData.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "CSV ملف|*.csv",
                FileName = $"تقرير_التحويلات_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        for (int i = 0; i < dgvData.Columns.Count; i++)
                        {
                            sw.Write(dgvData.Columns[i].HeaderText);
                            if (i < dgvData.Columns.Count - 1) sw.Write(",");
                        }
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
                    MessageBox.Show($"✅ تم تصدير البيانات بنجاح", "تصدير", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            text = text.Replace(',', '.');
            return decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal val) ? val : 0;
        }
    }
}