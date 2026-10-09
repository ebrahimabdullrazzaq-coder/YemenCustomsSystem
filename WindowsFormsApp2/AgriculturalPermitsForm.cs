using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    // ==================== ✅ النموذج الأساسي المشترك ====================
    public class PermitBaseForm : Form
    {
        protected string _formTitle;
        protected string _permitTable;
        protected string _itemsTable;
        private string _permitNoLabelText;
        private string _qtyLabelText;

        private ToolStrip toolStrip1;
        private ToolStripButton btnHome, btnNew, btnSave, btnDelete, btnView, btnExit;

        private GroupBox grpHeader;
        private Label lblPermitNo, lblPermitDate, lblPortName, lblExpiryDate;
        private TextBox txtPermitNo, txtPortName;
        private DateTimePicker dtpPermitDate, dtpExpiryDate;
        private DataGridView dgvItems;

        private bool isEditing = false;

        public PermitBaseForm(string formTitle, string permitTable, string itemsTable, string permitNoLabelText, string qtyLabelText)
        {
            _formTitle = formTitle;
            _permitTable = permitTable;
            _itemsTable = itemsTable;
            _permitNoLabelText = permitNoLabelText;
            _qtyLabelText = qtyLabelText;

            EnsureTables();
            BuildUI();
            SetEditing(false);
        }

        // ==================== ✅ إنشاء الجداول ====================
        private void EnsureTables()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery($@"CREATE TABLE IF NOT EXISTS {_permitTable} (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PermitNo TEXT, PermitDate TEXT, PortName TEXT, ExpiryDate TEXT,
            CreatedDate TEXT, ModifiedDate TEXT)");

                DatabaseHelper.ExecuteNonQuery($@"CREATE TABLE IF NOT EXISTS {_itemsTable} (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            PermitNo TEXT, SubGroupNo TEXT, Quantity REAL, AvailableQuantity REAL)");

                try { DatabaseHelper.ExecuteNonQuery($"ALTER TABLE {_itemsTable} ADD COLUMN SubGroupName TEXT"); } catch { }
                try { DatabaseHelper.ExecuteNonQuery($"ALTER TABLE {_itemsTable} ADD COLUMN AvailableQuantity REAL"); } catch { }

                // ✅ دائماً: إصلاح أي قيم NULL في الكمية المتبقية
                try { DatabaseHelper.ExecuteNonQuery($"UPDATE {_itemsTable} SET AvailableQuantity = Quantity WHERE AvailableQuantity IS NULL"); } catch { }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        // ==================== ✅ بناء الواجهة ====================
        private void BuildUI()
        {
            this.Text = _formTitle + " - نظام الجمارك اليمنية";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.KeyPreview = true;

            toolStrip1 = new ToolStrip();
            toolStrip1.RightToLeft = RightToLeft.Yes;
            toolStrip1.BackColor = Color.FromArgb(52, 73, 94);
            toolStrip1.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            toolStrip1.Dock = DockStyle.Top;

            btnHome = new ToolStripButton("🏠 الرئيسية"); btnHome.ForeColor = Color.White;
            btnHome.Click += (s, e) => this.Close();

            btnNew = new ToolStripButton("🆕 جديد"); btnNew.ForeColor = Color.White;
            btnNew.Click += BtnNew_Click;

            btnSave = new ToolStripButton("💾 حفظ"); btnSave.ForeColor = Color.White;
            btnSave.Click += BtnSave_Click;

            btnDelete = new ToolStripButton("🗑️ حذف سطر"); btnDelete.ForeColor = Color.White;
            btnDelete.Click += BtnDelete_Click;

            btnView = new ToolStripButton("📋 عرض"); btnView.ForeColor = Color.White;
            btnView.Click += BtnView_Click;

            btnExit = new ToolStripButton(" خروج"); btnExit.ForeColor = Color.White;
            btnExit.Click += (s, e) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] { btnHome, btnNew, btnSave, btnDelete, btnView, btnExit });

            grpHeader = new GroupBox();
            grpHeader.Text = "📋 بيانات الطلب";
            grpHeader.Dock = DockStyle.Top;
            grpHeader.Height = 110;
            grpHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpHeader.ForeColor = Color.FromArgb(60, 60, 60);
            grpHeader.BackColor = Color.FromArgb(250, 250, 250);

            lblPermitNo = new Label { Text = _permitNoLabelText + ":", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(51, 122, 183), Location = new Point(1150, 30), Size = new Size(100, 20) };
            txtPermitNo = new TextBox { Font = new Font("Segoe UI", 9F, FontStyle.Bold), Location = new Point(1000, 27), Size = new Size(140, 23), BackColor = Color.FromArgb(255, 255, 190) };

            lblPermitDate = new Label { Text = "تاريخ " + _permitNoLabelText + ":", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(51, 122, 183), Location = new Point(830, 30), Size = new Size(120, 20) };
            dtpPermitDate = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Font = new Font("Segoe UI", 9F), Location = new Point(660, 27), Size = new Size(160, 23) };

            lblPortName = new Label { Text = "اسم المنفذ :", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(51, 122, 183), Location = new Point(1150, 65), Size = new Size(100, 20) };
            txtPortName = new TextBox { Font = new Font("Segoe UI", 9F), Location = new Point(1000, 62), Size = new Size(140, 23), BackColor = Color.FromArgb(255, 255, 190) };

            lblExpiryDate = new Label { Text = "تاريخ الانتهاء:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), ForeColor = Color.FromArgb(211, 47, 47), Location = new Point(830, 65), Size = new Size(100, 20) };
            dtpExpiryDate = new DateTimePicker { Format = DateTimePickerFormat.Custom, CustomFormat = "yyyy-MM-dd", Font = new Font("Segoe UI", 9F), Location = new Point(660, 62), Size = new Size(160, 23) };

            grpHeader.Controls.AddRange(new Control[] {
                lblPermitNo, txtPermitNo, lblPermitDate, dtpPermitDate,
                lblPortName, txtPortName, lblExpiryDate, dtpExpiryDate
            });

            dgvItems = new DataGridView();
            dgvItems.Location = new Point(20, 130);
            dgvItems.Size = new Size(1250, 450);
            dgvItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvItems.AllowUserToAddRows = true;
            dgvItems.AllowUserToDeleteRows = true;
            dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dgvItems.BackgroundColor = Color.White;
            dgvItems.BorderStyle = BorderStyle.Fixed3D;
            dgvItems.RightToLeft = RightToLeft.Yes;
            dgvItems.RowHeadersVisible = false;
            dgvItems.EnableHeadersVisualStyles = false;
            dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvItems.ColumnHeadersHeight = 40;
            dgvItems.RowTemplate.Height = 32;
            dgvItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            dgvItems.CellValidating += DgvItems_CellValidating;
            dgvItems.CellValueChanged += DgvItems_CellValueChanged;

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSubGroupNo",
                HeaderText = "رقم المجموعة الفرعية للاصناف ",
                Width = 200,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colSubGroupName",
                HeaderText = "اسم المجموعة الفرعية للاصناف ",
                Width = 300,
                ReadOnly = true
            });

            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colQuantity",
                HeaderText = _qtyLabelText,
                Width = 150,
                ReadOnly = false,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight, Format = "N2" }
            });

            // ✅ جديد: الكمية المستخدمة (مخفية)
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUsed",
                HeaderText = "الكمية المستخدمة",
                Width = 150,
                ReadOnly = true,
                Visible = false
            });

            // ✅ جديد: الكمية المتبقية (للقراءة فقط - تتحدث تلقائياً)
            dgvItems.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colAvailable",
                HeaderText = "✅ الكمية المتبقية",
                Width = 170,
                ReadOnly = true,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Format = "N2",
                    ForeColor = Color.Green,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                }
            });

            this.Controls.Add(dgvItems);
            this.Controls.Add(grpHeader);
            this.Controls.Add(toolStrip1);
        }

        // ==================== ✅ أحداث الجدول ====================
        private void DgvItems_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dgvItems.Columns[e.ColumnIndex].Name == "colQuantity")
            {
                if (string.IsNullOrEmpty(e.FormattedValue?.ToString())) return;

                if (!decimal.TryParse(e.FormattedValue.ToString(), out decimal result) || result < 0)
                {
                    MessageBox.Show("⚠️ الرجاء إدخال رقم صحيح للكمية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void DgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvItems.Columns[e.ColumnIndex].Name == "colQuantity")
            {
                UpdateAvailableCell(e.RowIndex);
            }
        }

        // ✅ إعادة حساب الكمية المتبقية = الكمية المصرحة - المستخدمة
        private void UpdateAvailableCell(int rowIndex)
        {
            var row = dgvItems.Rows[rowIndex];
            decimal qty = ParseDec(row.Cells["colQuantity"].Value);
            decimal used = ParseDec(row.Cells["colUsed"].Value);
            decimal avail = qty - used;

            row.Cells["colAvailable"].Value = avail;
            row.Cells["colAvailable"].Style.ForeColor = avail > 0 ? Color.Green : Color.Red;
        }

        private decimal ParseDec(object v)
        {
            decimal d = 0;
            decimal.TryParse(v?.ToString(), out d);
            return d;
        }

        // ==================== ✅ اختصارات F9 ====================
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F9)
            {
                if (txtPortName != null && txtPortName.Focused) { OpenPortSelectionForm(); return true; }

                if (dgvItems != null && dgvItems.Focused && dgvItems.CurrentCell != null)
                {
                    string col = dgvItems.Columns[dgvItems.CurrentCell.ColumnIndex].Name;
                    int row = dgvItems.CurrentCell.RowIndex;
                    if (col == "colSubGroupNo") { OpenSubGroupSelectionForm(row); return true; }
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OpenPortSelectionForm()
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName FROM Ports ORDER BY PortName");
            if (dt == null || dt.Rows.Count == 0) { MessageBox.Show("⚠️ لا توجد منافذ مضافة", "تنبيه"); return; }

            using (Form f = new Form { Text = "🔍 اختر المنفذ", Size = new Size(500, 450), StartPosition = FormStartPosition.CenterParent, RightToLeft = RightToLeft.Yes, RightToLeftLayout = true, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                DataGridView dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersHeight = 40;
                dgv.RowTemplate.Height = 35;
                dgv.Columns.Add("colPort", "اسم المنفذ");
                foreach (DataRow r in dt.Rows) dgv.Rows.Add(r["PortName"].ToString());

                dgv.CellDoubleClick += (s, ev) => {
                    if (ev.RowIndex >= 0) { txtPortName.Text = dgv.Rows[ev.RowIndex].Cells[0].Value?.ToString(); f.Close(); }
                };
                f.Controls.Add(dgv);
                f.ShowDialog();
            }
        }

        private void OpenSubGroupSelectionForm(int rowIndex)
        {
            DataTable dt = DatabaseHelper.ExecuteQuery("SELECT GroupNo, SubGroupNo, SubGroupName FROM SubGroups ORDER BY GroupNo, SubGroupNo");
            if (dt == null || dt.Rows.Count == 0) { MessageBox.Show("⚠️ لا توجد مجموعات فرعية مضافة\n💡 أضفها من شاشة المجموعات الفرعية", "تنبيه"); return; }

            using (Form f = new Form { Text = "🔍 اختر المجموعة الفرعية", Size = new Size(700, 450), StartPosition = FormStartPosition.CenterParent, RightToLeft = RightToLeft.Yes, RightToLeftLayout = true, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                DataGridView dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersHeight = 40;
                dgv.RowTemplate.Height = 35;

                dgv.Columns.Add("colG", "رقم المجموعة");
                dgv.Columns.Add("colSub", "رقم الفرعية");
                dgv.Columns.Add("colSubName", "اسم الفرعية");

                foreach (DataRow r in dt.Rows)
                    dgv.Rows.Add(r["GroupNo"].ToString(), r["SubGroupNo"].ToString(), r["SubGroupName"].ToString());

                dgv.CellDoubleClick += (s, ev) =>
                {
                    if (ev.RowIndex >= 0)
                    {
                        dgvItems.Rows[rowIndex].Cells["colSubGroupNo"].Value = dgv.Rows[ev.RowIndex].Cells["colSub"].Value;
                        dgvItems.Rows[rowIndex].Cells["colSubGroupName"].Value = dgv.Rows[ev.RowIndex].Cells["colSubName"].Value;
                        f.Close();
                    }
                };
                f.Controls.Add(dgv);
                f.ShowDialog();
            }
        }

        // ==================== ✅ نافذة العرض مع الكميات ====================
        private void BtnView_Click(object sender, EventArgs e)
        {
            Form viewForm = new Form();
            viewForm.Text = $"📋 عرض {_formTitle}";
            viewForm.Size = new Size(1200, 700);
            viewForm.StartPosition = FormStartPosition.CenterParent;
            viewForm.RightToLeft = RightToLeft.Yes;
            viewForm.RightToLeftLayout = true;
            viewForm.WindowState = FormWindowState.Maximized;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 60;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);

            Label lblSearch = new Label { Text = "🔍 بحث:", Font = new Font("Tahoma", 11, FontStyle.Bold), Location = new Point(1050, 15), Size = new Size(80, 30), TextAlign = ContentAlignment.MiddleRight };
            TextBox txtSearch = new TextBox { Font = new Font("Tahoma", 11), Location = new Point(700, 15), Size = new Size(340, 30) };
            Label lblCount = new Label { Text = "📊 الإجمالي: 0", Font = new Font("Tahoma", 11, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(20, 15), Size = new Size(200, 30) };

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblCount });

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;
            dgv.RowTemplate.Height = 35;

            dgv.Columns.Add("colPermitNo", _permitNoLabelText);
            dgv.Columns.Add("colPermitDate", "التاريخ");
            dgv.Columns.Add("colPortName", "المنفذ");
            dgv.Columns.Add("colExpiryDate", "تاريخ الانتهاء");
            dgv.Columns.Add("colTotalQty", "الكمية المصرحة");       // ✅ جديد
            dgv.Columns.Add("colAvailQty", "الكمية المتبقية");      // ✅ جديد
            dgv.Columns.Add("colStatus", "الحالة");

            dgv.Columns["colPermitNo"].Width = 150;
            dgv.Columns["colPermitDate"].Width = 110;
            dgv.Columns["colPortName"].Width = 160;
            dgv.Columns["colExpiryDate"].Width = 110;
            dgv.Columns["colTotalQty"].Width = 120;
            dgv.Columns["colAvailQty"].Width = 120;
            dgv.Columns["colStatus"].Width = 110;

            dgv.Columns["colTotalQty"].DefaultCellStyle.Format = "N2";
            dgv.Columns["colAvailQty"].DefaultCellStyle.Format = "N2";
            dgv.Columns["colTotalQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgv.Columns["colAvailQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            // ✅ فقط التصاريح التي تحتوي على كمية متبقية > 0 في بنودها
            DataTable dt = DatabaseHelper.ExecuteQuery($@"
    SELECT DISTINCT p.* 
    FROM {_permitTable} p
    INNER JOIN {_itemsTable} i ON p.PermitNo = i.PermitNo
    WHERE i.AvailableQuantity > 0
    ORDER BY p.PermitDate DESC");
            int totalCount = 0;

            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    string permitNo = r["PermitNo"]?.ToString();
                    string permitDate = r["PermitDate"]?.ToString();
                    string portName = r["PortName"]?.ToString();
                    string expiryDate = r["ExpiryDate"]?.ToString();

                    decimal totalQty = 0, availQty = 0;
                    try
                    {
                        var q = DatabaseHelper.ExecuteQuery(
                            $"SELECT SUM(Quantity) AS T, SUM(AvailableQuantity) AS A FROM {_itemsTable} WHERE PermitNo = @P",
                            DatabaseHelper.CreateParameter("@P", permitNo));
                        if (q != null && q.Rows.Count > 0)
                        {
                            totalQty = q.Rows[0]["T"] == DBNull.Value ? 0 : Convert.ToDecimal(q.Rows[0]["T"]);
                            availQty = q.Rows[0]["A"] == DBNull.Value ? 0 : Convert.ToDecimal(q.Rows[0]["A"]);
                        }
                    }
                    catch { }

                    string status = "";
                    Color statusColor = Color.Black;
                    if (DateTime.TryParse(expiryDate, out DateTime expDate))
                    {
                        if (expDate < DateTime.Now) { status = "❌ منتهي"; statusColor = Color.Red; }
                        else if ((expDate - DateTime.Now).TotalDays < 30) { status = "⚠️ ينتهي قريباً"; statusColor = Color.Orange; }
                        else { status = "✅ ساري"; statusColor = Color.Green; }
                    }

                    int rowIndex = dgv.Rows.Add(permitNo, permitDate, portName, expiryDate, totalQty, availQty, status);
                    dgv.Rows[rowIndex].Cells["colStatus"].Style.ForeColor = statusColor;
                    dgv.Rows[rowIndex].Cells["colAvailQty"].Style.ForeColor = availQty > 0 ? Color.Green : Color.Red;
                    dgv.Rows[rowIndex].Cells["colAvailQty"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    totalCount++;
                }
            }
         

            if (dt != null)
            {
                foreach (DataRow r in dt.Rows)
                {
                    string permitNo = r["PermitNo"]?.ToString();
                    string permitDate = r["PermitDate"]?.ToString();
                    string portName = r["PortName"]?.ToString();
                    string expiryDate = r["ExpiryDate"]?.ToString();

                    // ✅ جلب مجموع الكميات والمتبقي
                    decimal totalQty = 0, availQty = 0;
                    try
                    {
                        var q = DatabaseHelper.ExecuteQuery(
                            $"SELECT SUM(Quantity) AS T, SUM(AvailableQuantity) AS A FROM {_itemsTable} WHERE PermitNo = @P",
                            DatabaseHelper.CreateParameter("@P", permitNo));
                        if (q != null && q.Rows.Count > 0)
                        {
                            totalQty = q.Rows[0]["T"] == DBNull.Value ? 0 : Convert.ToDecimal(q.Rows[0]["T"]);
                            availQty = q.Rows[0]["A"] == DBNull.Value ? 0 : Convert.ToDecimal(q.Rows[0]["A"]);
                        }
                    }
                    catch { }

                    string status = "";
                    Color statusColor = Color.Black;
                    if (DateTime.TryParse(expiryDate, out DateTime expDate))
                    {
                        if (expDate < DateTime.Now) { status = "❌ منتهي"; statusColor = Color.Red; }
                        else if ((expDate - DateTime.Now).TotalDays < 30) { status = "⚠️ ينتهي قريباً"; statusColor = Color.Orange; }
                        else { status = "✅ ساري"; statusColor = Color.Green; }
                    }

                    int rowIndex = dgv.Rows.Add(permitNo, permitDate, portName, expiryDate, totalQty, availQty, status);
                    dgv.Rows[rowIndex].Cells["colStatus"].Style.ForeColor = statusColor;
                    dgv.Rows[rowIndex].Cells["colAvailQty"].Style.ForeColor = availQty > 0 ? Color.Green : Color.Red;
                    dgv.Rows[rowIndex].Cells["colAvailQty"].Style.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    totalCount++;
                }
            }

            lblCount.Text = $"📊 الإجمالي: {totalCount}";

            txtSearch.TextChanged += (s, ev) =>
            {
                string search = txtSearch.Text.Trim().ToLower();
                int visibleCount = 0;
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    string pNo = row.Cells["colPermitNo"].Value?.ToString()?.ToLower() ?? "";
                    string pName = row.Cells["colPortName"].Value?.ToString()?.ToLower() ?? "";
                    if (string.IsNullOrEmpty(search) || pNo.Contains(search) || pName.Contains(search)) { row.Visible = true; visibleCount++; }
                    else row.Visible = false;
                }
                lblCount.Text = $"📊 المعروض: {visibleCount} من {totalCount}";
            };

            dgv.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    LoadPermitFromDatabase(dgv.Rows[ev.RowIndex].Cells["colPermitNo"].Value?.ToString());
                    viewForm.Close();
                }
            };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 70;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnLoad = new Button { Text = "📥 تحميل المحدد", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(200, 45), Location = new Point(500, 12) };
            btnLoad.Click += (s, ev) =>
            {
                if (dgv.SelectedRows.Count > 0)
                {
                    LoadPermitFromDatabase(dgv.SelectedRows[0].Cells["colPermitNo"].Value?.ToString());
                    viewForm.Close();
                }
                else MessageBox.Show("⚠️ الرجاء اختيار تصريح أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            };

            Button btnClose = new Button { Text = "❌ إغلاق", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 45), Location = new Point(50, 12) };
            btnClose.Click += (s, ev) => viewForm.Close();

            pnlButtons.Controls.AddRange(new Control[] { btnLoad, btnClose });
            viewForm.Controls.Add(dgv);
            viewForm.Controls.Add(pnlSearch);
            viewForm.Controls.Add(pnlButtons);
            viewForm.AcceptButton = btnLoad;
            viewForm.CancelButton = btnClose;
            viewForm.ShowDialog();
        }

        // ==================== ✅ تحميل التصريح مع الكميات المستخدمة والمتبقية ====================
        private void LoadPermitFromDatabase(string permitNo)
        {
            DataTable h = DatabaseHelper.ExecuteQuery($"SELECT * FROM {_permitTable} WHERE PermitNo = @P", DatabaseHelper.CreateParameter("@P", permitNo));
            if (h == null || h.Rows.Count == 0) { MessageBox.Show("❌ لم يتم العثور على السجل", "تنبيه"); return; }

            DataRow r0 = h.Rows[0];
            txtPermitNo.Text = r0["PermitNo"].ToString();
            txtPortName.Text = r0["PortName"].ToString();
            if (DateTime.TryParse(r0["PermitDate"].ToString(), out DateTime pd)) dtpPermitDate.Value = pd;
            if (DateTime.TryParse(r0["ExpiryDate"].ToString(), out DateTime ed)) dtpExpiryDate.Value = ed;

            dgvItems.Rows.Clear();
            DataTable it = DatabaseHelper.ExecuteQuery($"SELECT SubGroupNo, SubGroupName, Quantity, AvailableQuantity FROM {_itemsTable} WHERE PermitNo = @P", DatabaseHelper.CreateParameter("@P", permitNo));
            if (it != null)
            {
                foreach (DataRow r in it.Rows)
                {
                    decimal qty = r["Quantity"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Quantity"]);
                    decimal avail = r["AvailableQuantity"] == DBNull.Value ? qty : Convert.ToDecimal(r["AvailableQuantity"]);
                    decimal used = qty - avail;

                    int idx = dgvItems.Rows.Add(r["SubGroupNo"].ToString(), r["SubGroupName"].ToString(), qty, used, avail);
                    dgvItems.Rows[idx].Cells["colAvailable"].Style.ForeColor = avail > 0 ? Color.Green : Color.Red;
                }
            }

            SetEditing(true);
            MessageBox.Show($"✅ تم تحميل السجل رقم: {permitNo}\n💡 العمود الأخضر يعرض الكمية المتبقية بعد الاستخدام", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SetEditing(bool on)
        {
            isEditing = on;
            txtPermitNo.ReadOnly = !on;
            txtPortName.ReadOnly = !on;
            dtpPermitDate.Enabled = on;
            dtpExpiryDate.Enabled = on;
            dgvItems.ReadOnly = !on;

            // ✅ عمود الكمية قابل للتحرير حتى في وضع العرض للقراءة فقط لبقية الأعمدة
            if (dgvItems.Columns["colQuantity"] != null) dgvItems.Columns["colQuantity"].ReadOnly = !on;

            Color bg = on ? Color.FromArgb(255, 255, 224) : Color.White;
            txtPermitNo.BackColor = bg;
            txtPortName.BackColor = bg;
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            txtPermitNo.Clear(); txtPortName.Clear();
            dtpPermitDate.Value = DateTime.Now; dtpExpiryDate.Value = DateTime.Now.AddYears(1);
            dgvItems.Rows.Clear();
            SetEditing(true);
            txtPermitNo.Focus();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0) { MessageBox.Show("⚠️ حدد سطراً للحذف", "تنبيه"); return; }
            foreach (DataGridViewRow r in dgvItems.SelectedRows)
                if (!r.IsNewRow) dgvItems.Rows.Remove(r);
        }

        // ==================== ✅ الحفظ مع الحفاظ على الكميات المستخدمة ====================
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!isEditing) { MessageBox.Show("⚠️ اضغط 'جديد' أو 'تحميل' أولاً", "تنبيه"); return; }
            string permitNo = txtPermitNo.Text.Trim();
            if (string.IsNullOrEmpty(permitNo)) { MessageBox.Show("⚠️ الرقم مطلوب", "تنبيه"); txtPermitNo.Focus(); return; }

            string portName = txtPortName.Text.Trim();
            string pDate = dtpPermitDate.Value.ToString("yyyy-MM-dd");
            string eDate = dtpExpiryDate.Value.ToString("yyyy-MM-dd");

            try
            {
                DatabaseHelper.ExecuteNonQuery($"DELETE FROM {_itemsTable} WHERE PermitNo = @P", DatabaseHelper.CreateParameter("@P", permitNo));

                var chk = DatabaseHelper.ExecuteScalar($"SELECT COUNT(*) FROM {_permitTable} WHERE PermitNo = @P", DatabaseHelper.CreateParameter("@P", permitNo));
                if (Convert.ToInt32(chk) > 0)
                {
                    DatabaseHelper.ExecuteNonQuery($"UPDATE {_permitTable} SET PermitDate=@D, PortName=@Port, ExpiryDate=@E, ModifiedDate=datetime('now') WHERE PermitNo=@P",
                        DatabaseHelper.CreateParameter("@D", pDate), DatabaseHelper.CreateParameter("@Port", portName),
                        DatabaseHelper.CreateParameter("@E", eDate), DatabaseHelper.CreateParameter("@P", permitNo));
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery($"INSERT INTO {_permitTable} (PermitNo, PermitDate, PortName, ExpiryDate, CreatedDate, ModifiedDate) VALUES (@P,@D,@Port,@E,datetime('now'),datetime('now'))",
                        DatabaseHelper.CreateParameter("@P", permitNo), DatabaseHelper.CreateParameter("@D", pDate),
                        DatabaseHelper.CreateParameter("@Port", portName), DatabaseHelper.CreateParameter("@E", eDate));
                }

                foreach (DataGridViewRow row in dgvItems.Rows)
                {
                    if (row.IsNewRow) continue;
                    string subNo = row.Cells["colSubGroupNo"].Value?.ToString() ?? "";
                    string subName = row.Cells["colSubGroupName"].Value?.ToString() ?? "";
                    decimal qty = ParseDec(row.Cells["colQuantity"].Value);
                    decimal used = ParseDec(row.Cells["colUsed"].Value);

                    if (string.IsNullOrEmpty(subNo)) continue;

                    // ✅ المتبقية = الكمية الجديدة - المستخدمة سابقاً (لا تُصفّر عند إعادة الحفظ)
                    decimal avail = Math.Max(qty - used, 0);

                    DatabaseHelper.ExecuteNonQuery($"INSERT INTO {_itemsTable} (PermitNo, SubGroupNo, SubGroupName, Quantity, AvailableQuantity) VALUES (@P,@S,@SN,@Q,@AQ)",
                        DatabaseHelper.CreateParameter("@P", permitNo),
                        DatabaseHelper.CreateParameter("@S", subNo),
                        DatabaseHelper.CreateParameter("@SN", subName),
                        DatabaseHelper.CreateParameter("@Q", qty),
                        DatabaseHelper.CreateParameter("@AQ", avail));
                }

                MessageBox.Show("✅ تم الحفظ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetEditing(false);
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ في الحفظ:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ==================== ✅ خصم الكمية ====================
        public static bool DeductPermitQuantity(string permitTable, string itemsTable, string permitNo, string subGroupNo, decimal usedQuantity)
        {
            try
            {
                var result = DatabaseHelper.ExecuteScalar(
                    $"SELECT AvailableQuantity FROM {itemsTable} WHERE PermitNo = @PermitNo AND SubGroupNo = @SubGroupNo",
                    DatabaseHelper.CreateParameter("@PermitNo", permitNo),
                    DatabaseHelper.CreateParameter("@SubGroupNo", subGroupNo));

                if (result == null || result == DBNull.Value) return false;

                decimal availableQty = Convert.ToDecimal(result);
                if (availableQty < usedQuantity) return false;

                decimal newAvailableQty = availableQty - usedQuantity;

                DatabaseHelper.ExecuteNonQuery(
                    $"UPDATE {itemsTable} SET AvailableQuantity = @NewQty WHERE PermitNo = @PermitNo AND SubGroupNo = @SubGroupNo",
                    DatabaseHelper.CreateParameter("@NewQty", newAvailableQty),
                    DatabaseHelper.CreateParameter("@PermitNo", permitNo),
                    DatabaseHelper.CreateParameter("@SubGroupNo", subGroupNo));

                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في خصم الكمية: {ex.Message}");
                return false;
            }
        }

        // ==================== ✅ استرجاع الكمية ====================
        public static bool RestorePermitQuantity(string itemsTable, string permitNo, string subGroupNo, decimal restoredQuantity)
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(
                    $"UPDATE {itemsTable} SET AvailableQuantity = AvailableQuantity + @RestoredQty WHERE PermitNo = @PermitNo AND SubGroupNo = @SubGroupNo",
                    DatabaseHelper.CreateParameter("@RestoredQty", restoredQuantity),
                    DatabaseHelper.CreateParameter("@PermitNo", permitNo),
                    DatabaseHelper.CreateParameter("@SubGroupNo", subGroupNo));
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في استرجاع الكمية: {ex.Message}");
                return false;
            }
        }
    }

    // ==================== ✅ شاشة التصاريح الزراعية ====================
    public class AgriculturalPermitsForm : PermitBaseForm
    {
        public AgriculturalPermitsForm() : base(
            "🌾 التصاريح الزراعية",
            "AgriculturalPermits",
            "AgriculturalPermitItems",
            "رقم التصريح",
            "كمية التصاريح"
        )
        { }
    }

    // ==================== ✅ شاشة إفراج المدفوعات ====================
    public class PaymentsReleaseForm : PermitBaseForm
    {
        public PaymentsReleaseForm() : base(
            "💳 إفراج المدفوعات",
            "PaymentsReleases",
            "PaymentReleaseItems",
            "رقم الافراج",
            "كمية الافراج"
        )
        { }
    }
}