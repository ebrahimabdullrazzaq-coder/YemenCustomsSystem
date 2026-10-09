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
    public partial class WarehouseToWarehouseTransferForm : Form
    {
        private List<WarehouseTransferRecord> transfers = new List<WarehouseTransferRecord>();
        private List<WarehousesForm.WarehouseRecord> allWarehouses = new List<WarehousesForm.WarehouseRecord>();
        private List<StockTransferForm.StockMovementRecord> allMovements = new List<StockTransferForm.StockMovementRecord>();
        private List<CustomsClearanceForm.ClearanceRecord> allClearances = new List<CustomsClearanceForm.ClearanceRecord>();

        // تعريف الحقول العلوية
        private ComboBox cmbSourceWarehouse;
        private ComboBox cmbTargetWarehouse;
        private DateTimePicker dtpTransferDate;
        private TextBox txtTransferNumber;
        private TextBox txtNotes;
        private Button btnLoadItems, btnExecuteTransfer, btnShowMovements;
        private DataGridView dgvData;
        private Label lblTransferStatus;

        // شريط الأدوات
        private ToolStrip toolStrip1;
        private ToolStripButton btnSave, btnNew, btnDelete, btnSearch, btnExport, btnExit, btnHome;
        private ToolStripSeparator toolStripSeparator1, toolStripSeparator2, toolStripSeparator3;

        public WarehouseToWarehouseTransferForm()
        {
            CreateControlsManually();
            InitializeGrid();
            LoadWarehousesFromDatabase();
            LoadClearancesFromDatabase();
            LoadMovementsFromDatabase();
            LoadTransfersFromDatabase();
            ClearFields();
        }

        // ==================== نماذج البيانات ====================
        public class WarehouseTransferRecord
        {
            public int Id { get; set; }
            public string TransferNumber { get; set; }
            public DateTime TransferDate { get; set; }
            public string SourceWarehouse { get; set; }
            public string TargetWarehouse { get; set; }
            public string Notes { get; set; }
            public string Status { get; set; }
            public DateTime CreatedDate { get; set; }
            public List<WarehouseTransferItem> Items { get; set; } = new List<WarehouseTransferItem>();
        }

        public class WarehouseTransferItem
        {
            public int Id { get; set; }
            public string StatementNo { get; set; }
            public string ProductName { get; set; }
            public decimal OriginalQtyInSource { get; set; }
            public decimal TransferQty { get; set; }
            public decimal RemainingQtyInSource { get; set; }
            public string Unit { get; set; }
            public string Notes { get; set; }
            public string StockStatus { get; set; }
        }

        // ==================== ✅ تحميل البيانات من قاعدة البيانات ====================
        private void LoadWarehousesFromDatabase()
        {
            try
            {
                allWarehouses.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT WarehouseName, Status FROM Warehouses ORDER BY WarehouseName");

                cmbSourceWarehouse.Items.Clear();
                cmbSourceWarehouse.Items.Add("-- اختر مخزن المصدر --");
                cmbTargetWarehouse.Items.Clear();
                cmbTargetWarehouse.Items.Add("-- اختر مخزن الهدف --");

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string status = row["Status"]?.ToString()?.Trim() ?? "";
                        if (status.Contains("نشط") || status == "1" || status.ToLower() == "active" || string.IsNullOrEmpty(status))
                        {
                            string whName = row["WarehouseName"].ToString();
                            allWarehouses.Add(new WarehousesForm.WarehouseRecord { WarehouseName = whName, Status = "نشط" });
                            cmbSourceWarehouse.Items.Add(whName);
                            cmbTargetWarehouse.Items.Add(whName);
                        }
                    }
                }

                if (cmbSourceWarehouse.Items.Count > 0) cmbSourceWarehouse.SelectedIndex = 0;
                if (cmbTargetWarehouse.Items.Count > 0) cmbTargetWarehouse.SelectedIndex = 0;
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل المخازن: {ex.Message}"); }
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

        private void LoadClearancesFromDatabase()
        {
            try
            {
                allClearances.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT StatementNo, ProductName, PortName, Quantity, Unit, Weight FROM CustomsClearance");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allClearances.Add(new CustomsClearanceForm.ClearanceRecord
                        {
                            StatementNo = row["StatementNo"]?.ToString(),
                            ProductName = row["ProductName"]?.ToString(),
                            PortName = row["PortName"]?.ToString(),
                            Quantity = row["Quantity"] != DBNull.Value ? Convert.ToDecimal(row["Quantity"]) : 0,
                            Unit = row["Unit"]?.ToString(),
                            Weight = row["Weight"] != DBNull.Value ? Convert.ToDecimal(row["Weight"]) : 0
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
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM WarehouseTransfers ORDER BY CreatedDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        transfers.Add(new WarehouseTransferRecord
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            TransferNumber = row["TransferNumber"]?.ToString(),
                            TransferDate = row["TransferDate"] != DBNull.Value ? Convert.ToDateTime(row["TransferDate"]) : DateTime.Now,
                            SourceWarehouse = row["SourceWarehouse"]?.ToString(),
                            TargetWarehouse = row["TargetWarehouse"]?.ToString(),
                            Notes = row["Notes"]?.ToString(),
                            Status = row["Status"]?.ToString(),
                            CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now,
                            Items = new List<WarehouseTransferItem>()
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل التحويلات: {ex.Message}"); }
        }

        // ==================== جلب الأصناف من المخزن المصدر ====================
        private void LoadItemsFromSourceWarehouse()
        {
            string sourceWarehouse = cmbSourceWarehouse.SelectedItem?.ToString() ?? "";

            if (sourceWarehouse == "-- اختر مخزن المصدر --" || string.IsNullOrWhiteSpace(sourceWarehouse))
            {
                MessageBox.Show("⚠️ الرجاء اختيار مخزن المصدر أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // إعادة تحميل الحركات لضمان أحدث البيانات
            LoadMovementsFromDatabase();

            var allIncomingMovements = allMovements
                .Where(m => m.ToLocation == sourceWarehouse)
                .GroupBy(m => new { m.StatementNo, m.ProductName })
                .Select(g => new
                {
                    StatementNo = g.Key.StatementNo,
                    ProductName = g.Key.ProductName,
                    TotalReceived = g.Sum(m => m.TransferQty)
                })
                .ToList();

            var allOutgoingMovements = allMovements
                .Where(m => m.FromLocation == sourceWarehouse)
                .GroupBy(m => new { m.StatementNo, m.ProductName })
                .Select(g => new
                {
                    StatementNo = g.Key.StatementNo,
                    ProductName = g.Key.ProductName,
                    TotalSent = g.Sum(m => m.TransferQty)
                })
                .ToList();

            var totalQtyPerProduct = allIncomingMovements
                .GroupBy(m => m.ProductName)
                .Select(g => new
                {
                    ProductName = g.Key,
                    TotalInWarehouse = g.Sum(m => m.TotalReceived) -
                                      allOutgoingMovements
                                          .Where(o => o.ProductName == g.Key)
                                          .Sum(o => o.TotalSent)
                })
                .ToDictionary(x => x.ProductName, x => x.TotalInWarehouse);

            if (allIncomingMovements.Count == 0)
            {
                MessageBox.Show($"📭 لا توجد أصناف في المخزن: {sourceWarehouse}", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            dgvData.Rows.Clear();
            int idx = 1;

            foreach (var incoming in allIncomingMovements)
            {
                var outgoing = allOutgoingMovements.FirstOrDefault(o =>
                    o.StatementNo == incoming.StatementNo &&
                    o.ProductName == incoming.ProductName);

                decimal totalReceived = incoming.TotalReceived;
                decimal totalSent = outgoing?.TotalSent ?? 0;
                decimal availableQty = Math.Max(0, totalReceived - totalSent);

                decimal totalQtyInWarehouse = totalQtyPerProduct.ContainsKey(incoming.ProductName)
                    ? totalQtyPerProduct[incoming.ProductName]
                    : availableQty;

                var clearanceInfo = allClearances.FirstOrDefault(c =>
                    c.StatementNo == incoming.StatementNo && c.ProductName == incoming.ProductName);

                string stockStatus = totalSent == 0 ? "📍 في المخزن" :
                                    totalSent >= totalReceived ? "✅ تم النقل كلياً" : "🚚 تم النقل جزئياً";

                if (availableQty > 0)
                {
                    dgvData.Rows.Add(
                        idx++,
                        incoming.StatementNo,
                        incoming.ProductName,
                        totalQtyInWarehouse,
                        totalReceived,
                        totalSent,
                        availableQty,
                        0, // TransferQty
                        availableQty, // RemainingAfterTransfer
                        stockStatus,
                        clearanceInfo?.Weight ?? 0,
                        clearanceInfo?.Unit ?? "كرتون"
                    );
                }
            }

            if (dgvData.Rows.Count == 0)
            {
                MessageBox.Show($"📭 جميع الأصناف في المخزن {sourceWarehouse} تم نقلها بالكامل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            lblTransferStatus.Text = $"✅ تم تحميل {dgvData.Rows.Count} صنف من المخزن: {sourceWarehouse}";
            lblTransferStatus.ForeColor = Color.Green;
            btnExecuteTransfer.Enabled = true;
            btnSave.Enabled = true;
        }

        // ==================== ✅ تنفيذ التحويل وحفظه في قاعدة البيانات ====================
        private void ExecuteTransfer()
        {
            string sourceWarehouse = cmbSourceWarehouse.SelectedItem?.ToString() ?? "";
            string targetWarehouse = cmbTargetWarehouse.SelectedItem?.ToString() ?? "";

            if (sourceWarehouse == "-- اختر مخزن المصدر --" || string.IsNullOrWhiteSpace(sourceWarehouse))
            {
                MessageBox.Show("⚠️ الرجاء اختيار مخزن المصدر", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (targetWarehouse == "-- اختر مخزن الهدف --" || string.IsNullOrWhiteSpace(targetWarehouse))
            {
                MessageBox.Show("⚠️ الرجاء اختيار مخزن الهدف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (sourceWarehouse == targetWarehouse)
            {
                MessageBox.Show("⚠️ لا يمكن التحويل إلى نفس المخزن", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<WarehouseTransferItem> transferItems = new List<WarehouseTransferItem>();
            bool hasTransfer = false, hasInvalidQty = false;
            string invalidProducts = "";

            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.IsNewRow) continue;

                decimal availableQty = ParseDecimal(row.Cells["colAvailableQty"].Value?.ToString());
                decimal transferQty = ParseDecimal(row.Cells["colTransferQty"].Value?.ToString());
                string productName = row.Cells["colProductName"].Value?.ToString() ?? "";
                string statementNo = row.Cells["colStatementNo"].Value?.ToString() ?? "";

                if (transferQty > 0)
                {
                    hasTransfer = true;

                    if (transferQty > availableQty)
                    {
                        hasInvalidQty = true;
                        invalidProducts += $"\n• {productName}: المطلوب {transferQty} > المتاح {availableQty}";
                    }

                    decimal originalQty = ParseDecimal(row.Cells["colReceivedQty"].Value?.ToString());
                    decimal alreadySent = ParseDecimal(row.Cells["colSentQty"].Value?.ToString());
                    decimal totalAfter = alreadySent + transferQty;

                    string stockStatus = totalAfter >= originalQty ? "✅ تم النقل كلياً" :
                                        totalAfter > 0 ? "🚚 تم النقل جزئياً" : "📍 في المخزن";

                    transferItems.Add(new WarehouseTransferItem
                    {
                        Id = transferItems.Count + 1,
                        StatementNo = statementNo,
                        ProductName = productName,
                        OriginalQtyInSource = originalQty,
                        TransferQty = transferQty,
                        RemainingQtyInSource = Math.Max(0, availableQty - transferQty),
                        Unit = row.Cells["colUnit"].Value?.ToString(),
                        Notes = txtNotes.Text.Trim(),
                        StockStatus = stockStatus
                    });
                }
            }

            if (hasInvalidQty)
            {
                MessageBox.Show($"⚠️ لا يمكن تنفيذ التحويل بسبب الكميات التالية:{invalidProducts}",
                    "تحويل غير مسموح", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!hasTransfer)
            {
                MessageBox.Show("⚠️ لم يتم إدخال أي كميات للنقل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"✅ تأكيد التحويل بين المخازن\n\nمن: {sourceWarehouse}\nإلى: {targetWarehouse}\n" +
                $"عدد الأصناف: {transferItems.Count}\nإجمالي الكمية: {transferItems.Sum(i => i.TransferQty):N2}\n\n" +
                $"هل أنت متأكد من تنفيذ التحويل؟",
                "تأكيد التحويل", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                // 1. توليد رقم تحويل جديد
                var maxIdObj = DatabaseHelper.ExecuteScalar("SELECT COALESCE(MAX(Id), 0) FROM WarehouseTransfers");
                int maxId = Convert.ToInt32(maxIdObj);
                string transferNumber = $"WH-{DateTime.Now:yyyyMMdd}-{maxId + 1:D4}";

                // 2. حفظ رأس التحويل في قاعدة البيانات
                var newTransferId = DatabaseHelper.ExecuteScalar(@"INSERT INTO WarehouseTransfers 
                    (TransferNumber, TransferDate, SourceWarehouse, TargetWarehouse, Notes, Status, CreatedDate)
                    VALUES (@TransferNumber, @TransferDate, @SourceWarehouse, @TargetWarehouse, @Notes, @Status, datetime('now'));
                    SELECT last_insert_rowid();",
                    DatabaseHelper.CreateParameter("@TransferNumber", transferNumber),
                    DatabaseHelper.CreateParameter("@TransferDate", dtpTransferDate.Value),
                    DatabaseHelper.CreateParameter("@SourceWarehouse", sourceWarehouse),
                    DatabaseHelper.CreateParameter("@TargetWarehouse", targetWarehouse),
                    DatabaseHelper.CreateParameter("@Notes", txtNotes.Text.Trim()),
                    DatabaseHelper.CreateParameter("@Status", "مكتمل")
                );

                int transferId = Convert.ToInt32(newTransferId);

                // 3. حفظ تفاصيل الحركات في جدول StockMovements
                foreach (var item in transferItems)
                {
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO StockMovements 
                        (StatementNo, ProductName, TransferQty, FromLocation, ToLocation, TransferDate, TransferNumber, Notes)
                        VALUES (@StatementNo, @ProductName, @TransferQty, @FromLocation, @ToLocation, @TransferDate, @TransferNumber, @Notes)",
                        DatabaseHelper.CreateParameter("@StatementNo", item.StatementNo),
                        DatabaseHelper.CreateParameter("@ProductName", item.ProductName),
                        DatabaseHelper.CreateParameter("@TransferQty", item.TransferQty),
                        DatabaseHelper.CreateParameter("@FromLocation", sourceWarehouse),
                        DatabaseHelper.CreateParameter("@ToLocation", targetWarehouse),
                        DatabaseHelper.CreateParameter("@TransferDate", dtpTransferDate.Value),
                        DatabaseHelper.CreateParameter("@TransferNumber", transferNumber),
                        DatabaseHelper.CreateParameter("@Notes", $"تحويل بين المخازن: {sourceWarehouse} → {targetWarehouse}. {item.Notes}")
                    );
                }

                // 4. تحديث القوائم المحلية
                transfers.Add(new WarehouseTransferRecord
                {
                    Id = transferId,
                    TransferNumber = transferNumber,
                    TransferDate = dtpTransferDate.Value,
                    SourceWarehouse = sourceWarehouse,
                    TargetWarehouse = targetWarehouse,
                    Notes = txtNotes.Text.Trim(),
                    Status = "مكتمل",
                    CreatedDate = DateTime.Now,
                    Items = transferItems
                });

                // إعادة تحميل الحركات لضمان التزامن
                LoadMovementsFromDatabase();

                MessageBox.Show($"✅ تم تنفيذ التحويل بنجاح\nرقم التحويل: {transferNumber}\n" +
                    $"عدد الأصناف: {transferItems.Count}\nإجمالي الكمية: {transferItems.Sum(i => i.TransferQty):N2}",
                    "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                RefreshAfterTransfer(sourceWarehouse);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تنفيذ التحويل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshAfterTransfer(string sourceWarehouse)
        {
            System.Threading.Thread.Sleep(100);
            LoadMovementsFromDatabase();
            LoadItemsFromSourceWarehouse();
            txtNotes.Clear();
            lblTransferStatus.Text = "📊 تم تحديث البيانات بنجاح";
            lblTransferStatus.ForeColor = Color.Green;
        }

        // ==================== عرض سجل حركات الصنف ====================
        private void ShowProductMovements()
        {
            if (dgvData.CurrentRow == null || dgvData.CurrentRow.IsNewRow)
            {
                MessageBox.Show("⚠️ الرجاء تحديد صنف أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string productName = dgvData.CurrentRow.Cells["colProductName"].Value?.ToString() ?? "";
            string statementNo = dgvData.CurrentRow.Cells["colStatementNo"].Value?.ToString() ?? "";

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("⚠️ لم يتم تحديد الصنف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var movements = allMovements
                .Where(m => m.ProductName == productName && (string.IsNullOrEmpty(statementNo) || m.StatementNo == statementNo))
                .OrderByDescending(m => m.TransferDate)
                .ToList();

            if (movements.Count == 0)
            {
                MessageBox.Show($"📭 لا توجد حركات سابقة للصنف: {productName}", "سجل الحركات",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form movementsForm = new Form
            {
                Text = $"📜 سجل حركات الصنف: {productName}",
                Size = new Size(1000, 500),
                StartPosition = FormStartPosition.CenterParent,
                RightToLeft = RightToLeft.Yes,
                WindowState = FormWindowState.Maximized
            };

            DataGridView dgvMovements = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                RightToLeft = RightToLeft.Yes,
                ReadOnly = true,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                ColumnHeadersHeight = 45
            };

            dgvMovements.Columns.Add("colDate", "التاريخ");
            dgvMovements.Columns.Add("colStatementNo", "رقم البيان");
            dgvMovements.Columns.Add("colQty", "الكمية");
            dgvMovements.Columns.Add("colFrom", "من");
            dgvMovements.Columns.Add("colTo", "إلى");
            dgvMovements.Columns.Add("colTransferNo", "رقم التحويل");
            dgvMovements.Columns.Add("colNotes", "ملاحظات");

            foreach (var m in movements)
            {
                dgvMovements.Rows.Add(
                    m.TransferDate.ToString("yyyy-MM-dd HH:mm"),
                    m.StatementNo,
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

        // ==================== تحديث حالة البضاعة والكمية المتبقية ====================
        private void UpdateStockStatusAndRemaining(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.IsNewRow) return;

            decimal originalQty = ParseDecimal(row.Cells["colReceivedQty"].Value?.ToString());
            decimal alreadySent = ParseDecimal(row.Cells["colSentQty"].Value?.ToString());
            decimal transferQty = ParseDecimal(row.Cells["colTransferQty"].Value?.ToString());
            decimal availableQty = ParseDecimal(row.Cells["colAvailableQty"].Value?.ToString());

            decimal remainingAfter = Math.Max(0, availableQty - transferQty);
            row.Cells["colRemainingAfterTransfer"].Value = remainingAfter;

            decimal totalAfter = alreadySent + transferQty;
            string stockStatus = transferQty == 0 ? (alreadySent == 0 ? "📍 في المخزن" : alreadySent >= originalQty ? "✅ تم النقل كلياً" : "🚚 تم النقل جزئياً") :
                                totalAfter >= originalQty ? "✅ تم النقل كلياً" : totalAfter > 0 ? "🚚 تم النقل جزئياً" : "📍 في المخزن";

            row.Cells["colStockStatus"].Value = stockStatus;

            if (stockStatus == "✅ تم النقل كلياً") { row.Cells["colStockStatus"].Style.ForeColor = Color.Green; row.Cells["colStockStatus"].Style.BackColor = Color.FromArgb(232, 245, 233); }
            else if (stockStatus == "🚚 تم النقل جزئياً") { row.Cells["colStockStatus"].Style.ForeColor = Color.Orange; row.Cells["colStockStatus"].Style.BackColor = Color.FromArgb(255, 243, 205); }
            else { row.Cells["colStockStatus"].Style.ForeColor = Color.Blue; }

            if (transferQty > availableQty) row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
            else if (transferQty > 0) row.DefaultCellStyle.BackColor = Color.FromArgb(200, 255, 200);
            else row.DefaultCellStyle.BackColor = Color.White;
        }

        // ==================== تهيئة الجدول ====================
        private void InitializeGrid()
        {
            if (dgvData == null) return;

            dgvData.Columns.Clear();
            dgvData.Columns.Add("colId", "م");
            dgvData.Columns.Add("colStatementNo", "رقم البيان");
            dgvData.Columns.Add("colProductName", "اسم السلعة");
            dgvData.Columns.Add("colTotalQtyInWarehouse", "إجمالي الكمية في المخزن");
            dgvData.Columns.Add("colReceivedQty", "الكمية الواردة للمخزن");
            dgvData.Columns.Add("colSentQty", "الكمية المنقولة منه سابقاً");
            dgvData.Columns.Add("colAvailableQty", "الكمية المتاحة");
            dgvData.Columns.Add("colTransferQty", "الكمية المراد نقلها");
            dgvData.Columns.Add("colRemainingAfterTransfer", "الكمية المتبقية بعد النقل");
            dgvData.Columns.Add("colStockStatus", "حالة البضاعة");
            dgvData.Columns.Add("colWeight", "الوزن");
            dgvData.Columns.Add("colUnit", "الوحدة");

            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                col.ReadOnly = col.Name != "colTransferQty";
                if (col.Name.Contains("Qty") || col.Name.Contains("Weight"))
                {
                    col.DefaultCellStyle.Format = "N2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            dgvData.Columns["colTotalQtyInWarehouse"].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
            dgvData.Columns["colTotalQtyInWarehouse"].DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvData.Columns["colTotalQtyInWarehouse"].DefaultCellStyle.ForeColor = Color.FromArgb(211, 47, 47);

            dgvData.Columns["colAvailableQty"].DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
            dgvData.Columns["colAvailableQty"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvData.Columns["colRemainingAfterTransfer"].DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 205);
            dgvData.Columns["colRemainingAfterTransfer"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvData.Columns["colStockStatus"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvData.Columns["colStockStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

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
            dgvData.ColumnHeadersHeight = 50;
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
            if (dgvData.CurrentCell?.ColumnIndex == dgvData.Columns["colTransferQty"]?.Index)
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
                e.Handled = true;
        }

        private void DgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvData.Columns[e.ColumnIndex].Name == "colTransferQty")
                ValidateAndCalculateRow(e.RowIndex);
        }

        private void ValidateAndCalculateRow(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.IsNewRow) return;

            decimal availableQty = ParseDecimal(row.Cells["colAvailableQty"].Value?.ToString());
            decimal transferQty = ParseDecimal(row.Cells["colTransferQty"].Value?.ToString());

            if (transferQty > availableQty)
            {
                MessageBox.Show($"⚠️ الكمية المنقولة ({transferQty:N2}) لا يمكن أن تتجاوز الكمية المتاحة ({availableQty:N2})\nالسلعة: {row.Cells["colProductName"].Value}",
                    "تنبيه - كمية غير مسموحة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                row.Cells["colTransferQty"].Value = 0;
                transferQty = 0;
            }
            if (transferQty < 0) { row.Cells["colTransferQty"].Value = 0; transferQty = 0; }

            UpdateStockStatusAndRemaining(rowIndex);
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

        // ==================== إنشاء الواجهة ====================
        private void CreateControlsManually()
        {
            this.Text = "🏚️ شاشة التحويل بين المخازن - نظام الجمارك اليمنية";
            this.Size = new Size(1500, 800);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            toolStrip1 = new ToolStrip
            {
                RightToLeft = RightToLeft.Yes,
                BackColor = Color.FromArgb(52, 73, 94),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            btnHome = new ToolStripButton("🏠 الرئيسية") { ForeColor = Color.White };
            btnHome.Click += (s, e) => btnHome_Click(s, e);

            btnSave = new ToolStripButton("💾 حفظ التحويل") { ForeColor = Color.White };
            btnSave.Click += (s, e) => ExecuteTransfer();

            btnNew = new ToolStripButton("🆕 جديد") { ForeColor = Color.White };
            btnNew.Click += (s, e) => ClearFields();

            toolStripSeparator1 = new ToolStripSeparator();

            btnDelete = new ToolStripButton("🗑️ حذف") { ForeColor = Color.White };
            btnDelete.Click += (s, e) => btnDelete_Click(s, e);

            toolStripSeparator2 = new ToolStripSeparator();
            btnSearch = new ToolStripButton("عرض") { ForeColor = Color.White };
            btnSearch.Click += (s, e) => btnSearch_Click(s, e);

            toolStripSeparator3 = new ToolStripSeparator();
            btnExport = new ToolStripButton("📤 تصدير") { ForeColor = Color.White };
            btnExport.Click += (s, e) => btnExport_Click(s, e);

            btnExit = new ToolStripButton("🚪 خروج") { ForeColor = Color.White };
            btnExit.Click += (s, e) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] {
                btnHome, btnSave, btnNew, toolStripSeparator1,
                btnDelete, toolStripSeparator2,
                btnSearch, toolStripSeparator3,
                btnExport, btnExit
            });

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 150,
                BackColor = Color.FromArgb(240, 248, 255),
                Padding = new Padding(10)
            };

            Label lblTransferNumber = new Label { Text = "رقم التحويل:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 15), Size = new Size(100, 25), TextAlign = ContentAlignment.MiddleRight };
            txtTransferNumber = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(960, 15), Size = new Size(180, 27), ReadOnly = true, BackColor = Color.FromArgb(255, 255, 190), Text = "يتولد تلقائياً" };

            Label lblTransferDate = new Label { Text = "تاريخ التحويل:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(810, 15), Size = new Size(100, 25), TextAlign = ContentAlignment.MiddleRight };
            dtpTransferDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Tahoma", 10), Location = new Point(620, 15), Size = new Size(180, 27), Value = DateTime.Now };

            Label lblSourceWarehouse = new Label { Text = "من مخزن:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 55), Size = new Size(90, 25), TextAlign = ContentAlignment.MiddleRight };
            cmbSourceWarehouse = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 10), Location = new Point(960, 55), Size = new Size(180, 27) };

            Label lblTargetWarehouse = new Label { Text = "إلى مخزن:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(810, 55), Size = new Size(90, 25), TextAlign = ContentAlignment.MiddleRight };
            cmbTargetWarehouse = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 10), Location = new Point(620, 55), Size = new Size(180, 27) };

            btnLoadItems = new Button { Text = "📥 تحميل أصناف المخزن", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(450, 55), Size = new Size(150, 30) };
            btnLoadItems.Click += (s, e) => LoadItemsFromSourceWarehouse();

            btnShowMovements = new Button { Text = "📜 سجل حركات الصنف", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(156, 39, 176), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(280, 55), Size = new Size(150, 30) };
            btnShowMovements.Click += (s, e) => ShowProductMovements();

            btnExecuteTransfer = new Button { Text = "✅ تنفيذ التحويل", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(110, 55), Size = new Size(150, 30) };
            btnExecuteTransfer.Click += (s, e) => ExecuteTransfer();

            Label lblNotes = new Label { Text = "ملاحظات:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 95), Size = new Size(90, 25), TextAlign = ContentAlignment.MiddleRight };
            txtNotes = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(960, 95), Size = new Size(300, 27), TextAlign = HorizontalAlignment.Right };

            lblTransferStatus = new Label { Text = "📊 جاهز للتحويل", Font = new Font("Tahoma", 10, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(450, 100), Size = new Size(600, 25), TextAlign = ContentAlignment.MiddleRight };

            pnlHeader.Controls.AddRange(new Control[] {
                lblTransferNumber, txtTransferNumber, lblTransferDate, dtpTransferDate,
                lblSourceWarehouse, cmbSourceWarehouse, lblTargetWarehouse, cmbTargetWarehouse,
                btnLoadItems, btnShowMovements, btnExecuteTransfer,
                lblNotes, txtNotes, lblTransferStatus
            });

            dgvData = new DataGridView { Dock = DockStyle.Fill };

            this.Controls.Add(dgvData);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(toolStrip1);
        }

        // ==================== أزرار العمليات ====================
        private void ClearFields()
        {
            if (cmbSourceWarehouse?.Items.Count > 0) cmbSourceWarehouse.SelectedIndex = 0;
            if (cmbTargetWarehouse?.Items.Count > 0) cmbTargetWarehouse.SelectedIndex = 0;
            dtpTransferDate.Value = DateTime.Now;
            txtNotes?.Clear();
            dgvData?.Rows.Clear();
            lblTransferStatus.Text = "📊 جاهز للتحويل";
            lblTransferStatus.ForeColor = Color.FromArgb(33, 150, 243);
            btnExecuteTransfer.Enabled = true;
            btnSave.Enabled = true;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ الرجاء تحديد السجل المراد حذفه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("🗑️ هل أنت متأكد من حذف السجل المحدد؟", "تأكيد الحذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (DataGridViewRow row in dgvData.SelectedRows.Cast<DataGridViewRow>().ToList())
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
                lblSearch.Text = "🔍 بحث برقم التحويل:";
                lblSearch.Font = new Font("Tahoma", 10, FontStyle.Bold);
                lblSearch.Location = new Point(750, 15);
                lblSearch.Size = new Size(150, 25);
                lblSearch.TextAlign = ContentAlignment.MiddleRight;

                TextBox txtSearch = new TextBox();
                txtSearch.Font = new Font("Tahoma", 11);
                txtSearch.Location = new Point(450, 15);
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
                dgvView.Columns.Add("colTransferDate", "تاريخ التحويل");
                dgvView.Columns.Add("colSourceWarehouse", "من (المخزن)");
                dgvView.Columns.Add("colTargetWarehouse", "إلى (المخزن)");
                dgvView.Columns.Add("colStatus", "الحالة");

                // ضبط عرض الأعمدة
                dgvView.Columns["colTransferNumber"].Width = 150;
                dgvView.Columns["colTransferDate"].Width = 120;
                dgvView.Columns["colSourceWarehouse"].Width = 200;
                dgvView.Columns["colTargetWarehouse"].Width = 200;
                dgvView.Columns["colStatus"].Width = 100;

                // 4. تعبئة البيانات من القائمة
                foreach (var t in transfers)
                {
                    dgvView.Rows.Add(
                        t.TransferNumber,
                        t.TransferDate.ToString("yyyy-MM-dd"),
                        t.SourceWarehouse,
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

                        // إظهار الصف إذا تطابق مع رقم التحويل، أو إذا كان حقل البحث فارغاً
                        if (string.IsNullOrEmpty(searchTerm) || tNum.Contains(searchTerm))
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
                         
                            viewForm.Close(); // إغلاق نافذة العرض بعد الاختيار
                        }
                    }
                };

                // عرض النافذة
                viewForm.ShowDialog();
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (transfers.Count == 0 && dgvData.Rows.Count == 0)
            {
                MessageBox.Show("لا توجد بيانات للتصدير", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV ملف|*.csv",
                FileName = $"تقرير_التحويل_بين_المخازن_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
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

        private void btnHome_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.MdiParent is MainForm mainForm && !mainForm.IsDisposed)
                {
                    foreach (Form child in mainForm.MdiChildren.ToList())
                    {
                        if (!child.IsDisposed && child != this) child.Close();
                    }
                    this.Close();
                    mainForm.ShowWelcome();
                    mainForm.Activate();
                    mainForm.BringToFront();
                }
                else { this.Close(); }
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ: {ex.Message}", "خطأ"); this.Close(); }
        }

        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            text = text.Replace(',', '.');
            return decimal.TryParse(text, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal val) ? val : 0;
        }

        private void WarehouseToWarehouseTransferForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // لم نعد بحاجة لحفظ JSON هنا لأننا نحفظ مباشرة في قاعدة البيانات
        }
    }
}