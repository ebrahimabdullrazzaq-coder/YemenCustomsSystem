using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class AdditionalAmountsForm : Form
    {
        // ==================== ✅ حقول الواجهة ====================
        private TextBox txtFeeNo;
        private TextBox txtFeeName;
        private ComboBox cmbEffectType;
        private ComboBox cmbCurrency;
        private ComboBox cmbValueType;
        private ComboBox cmbFeeScope;
        private DataGridView dgvData;

        public AdditionalAmountsForm()
        {
            EnsureAdditionalFeesTable();   // ✅ إنشاء جدول التخزين
            InitializeComponentManual();
            PopulateDropdowns();

            cmbValueType.SelectedIndex = 0;
            cmbFeeScope.SelectedIndex = 0;
            cmbEffectType.SelectedIndex = 0;

            UpdateGridColumns();

            // ✅ تحميل تلقائي عند مغادرة حقل رقم الرسوم
            txtFeeNo.Leave += (s, e) => TryLoadFee(txtFeeNo.Text.Trim());
        }

        // ==========================================
        // ✅ إنشاء جدول تعريف الرسوم الإضافية في قاعدة البيانات
        // ==========================================
        private void EnsureAdditionalFeesTable()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS AdditionalFees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FeeNo TEXT,
                    FeeName TEXT,
                    EffectType TEXT,
                    Currency TEXT,
                    ValueType TEXT,
                    FeeScope TEXT,
                    PortId TEXT,
                    PortName TEXT,
                    PortGroupId TEXT,
                    PortGroupName TEXT,
                    ItemId TEXT,
                    ItemName TEXT,
                    ItemGroupId TEXT,
                    ItemGroupName TEXT,
                    MinLimit REAL DEFAULT 0,
                    CalcType TEXT,
                    Amount REAL DEFAULT 0,
                    CreatedDate TEXT DEFAULT (datetime('now','localtime'))
                )");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ إنشاء جدول الرسوم الإضافية: {ex.Message}");
            }
        }

        private void InitializeComponentManual()
        {
            this.Text = "إدارة المبالغ الإضافية";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            // ===== الجدول السفلي =====
            dgvData = new DataGridView
            {
                Location = new Point(15, 185),
                Size = new Size(1050, 350),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                RowHeadersVisible = false,
                AllowUserToAddRows = true,
                AllowUserToDeleteRows = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold) },
                ColumnHeadersHeight = 40,
                RowTemplate = { Height = 35 }
            };
            dgvData.CellValueChanged += DgvData_CellValueChanged;
            this.Controls.Add(dgvData);

            // ===== لوحة الحقول العلوية =====
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 170, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(15) };

            CreateLabelAndTextBox(pnlTop, "رقم الرسوم:", 1170, 15, 980, 15, out Label lblNo, out txtFeeNo);
            CreateLabelAndTextBox(pnlTop, "اسم الرسوم:", 830, 15, 650, 15, out Label lblName, out txtFeeName);
            CreateLabelAndComboBox(pnlTop, "نوع التأثير:", 1170, 55, 980, 55, out Label lblEffect, out cmbEffectType);
            CreateLabelAndComboBox(pnlTop, "العملة:", 830, 55, 650, 55, out Label lblCurr, out cmbCurrency);
            CreateLabelAndComboBox(pnlTop, "نوع القيمة:", 1170, 95, 980, 95, out Label lblValType, out cmbValueType);
            CreateLabelAndComboBox(pnlTop, "نوع الرسوم:", 830, 95, 650, 95, out Label lblScope, out cmbFeeScope);

            // ✅ تلميح للمستخدم
            Label lblHint = new Label
            {
                Text = "💡 اضغط F9 داخل خلايا (الصنف / مجموعة الأصناف الفرعية / المنفذ / مجموعة المنافذ) للاختيار — عند الحفظ سيظهر تلقائياً في شاشة طلب الترسيم (تبويب المبالغ الإضافية) حسب تطابق المنفذ/المجموعة والصنف",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(60, 135),
                Size = new Size(1000, 25),
                TextAlign = ContentAlignment.MiddleRight
            };
            pnlTop.Controls.Add(lblHint);

            cmbFeeScope.SelectedIndexChanged += Cmb_Changed;
            cmbEffectType.SelectedIndexChanged += Cmb_Changed;

            this.Controls.Add(pnlTop);

            // ===== لوحة الأزرار السفلية =====
            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 70, BackColor = Color.FromArgb(240, 248, 255) };

            Button btnSave = CreateButton("💾 حفظ", Color.FromArgb(76, 175, 80), 600, 15, btnSave_Click);
            Button btnNew = CreateButton("🆕 جديد", Color.FromArgb(33, 150, 243), 430, 15, btnNew_Click);
            Button btnDelete = CreateButton("🗑️ حذف", Color.FromArgb(244, 67, 54), 260, 15, btnDelete_Click);
            Button btnExit = CreateButton("🚪 خروج", Color.FromArgb(158, 158, 158), 90, 15, (s, ev) => this.Close());

            pnlBottom.Controls.AddRange(new Control[] { btnSave, btnNew, btnDelete, btnExit });
            this.Controls.Add(pnlBottom);
        }

        // ==========================================
        // ✅ اعتراض F9 على مستوى النموذج
        // ==========================================
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.F9 && dgvData != null && dgvData.Focused && dgvData.CurrentCell != null)
            {
                string colName = dgvData.CurrentCell.OwningColumn?.Name ?? "";
                int rowIndex = dgvData.CurrentCell.RowIndex;

                switch (colName)
                {
                    case "colItemId":
                    case "colItemName":
                        OpenItemsLookup(rowIndex);
                        return true;

                    case "colItemGroupId":
                    case "colItemGroupName":
                        OpenItemSubGroupsLookup(rowIndex);   // ✅ مجموعة الأصناف الفرعية
                        return true;

                    case "colPortId":
                    case "colPortName":
                        OpenPortsLookup(rowIndex);
                        return true;

                    case "colPortGroupId":
                    case "colPortGroupName":
                        OpenPortGroupsLookup(rowIndex);
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ==========================================
        // ✅ دوال فتح نوافذ الاختيار
        // ==========================================
        private void OpenItemsLookup(int rowIndex)
        {
            var data = GetItemsData();
            if (data.Count == 0) { MessageBox.Show("⚠️ لا توجد أصناف مسجلة في شاشة تهيئة الأصناف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (ShowLookupForm("🔍 اختر الصنف (F9)", "رقم الصنف", "اسم الصنف", data, out string code, out string name))
            {
                SetCell(rowIndex, "colItemId", code);
                SetCell(rowIndex, "colItemName", name);
            }
        }

        // ✅ مجموعة الأصناف الفرعية
        private void OpenItemSubGroupsLookup(int rowIndex)
        {
            var data = GetItemSubGroupsData();
            if (data.Count == 0) { MessageBox.Show("⚠️ لا توجد مجموعات أصناف فرعية مسجلة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (ShowLookupForm("🔍 اختر مجموعة الأصناف الفرعية (F9)", "رقم المجموعة الفرعية", "اسم المجموعة الفرعية", data, out string code, out string name))
            {
                SetCell(rowIndex, "colItemGroupId", code);
                SetCell(rowIndex, "colItemGroupName", name);
            }
        }

        private void OpenPortsLookup(int rowIndex)
        {
            var data = GetPortsData();
            if (data.Count == 0) { MessageBox.Show("⚠️ لا توجد منافذ مسجلة في شاشة بيانات المنافذ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (ShowLookupForm("🔍 اختر المنفذ (F9)", "رقم المنفذ", "اسم المنفذ", data, out string code, out string name))
            {
                SetCell(rowIndex, "colPortId", code);
                SetCell(rowIndex, "colPortName", name);
            }
        }

        private void OpenPortGroupsLookup(int rowIndex)
        {
            var data = GetPortGroupsData();
            if (data.Count == 0) { MessageBox.Show("⚠️ لا توجد مجموعات منافذ مسجلة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (ShowLookupForm("🔍 اختر مجموعة المنافذ (F9)", "رقم المجموعة", "اسم المجموعة", data, out string code, out string name))
            {
                SetCell(rowIndex, "colPortGroupId", code);
                SetCell(rowIndex, "colPortGroupName", name);
            }
        }

        // ==========================================
        // ✅ نافذة البحث والاختيار الموحدة
        // ==========================================
        private bool ShowLookupForm(string title, string codeHeader, string nameHeader,
            List<KeyValuePair<string, string>> data, out string selectedCode, out string selectedName)
        {
            selectedCode = "";
            selectedName = "";

            using (Form f = new Form())
            {
                f.Text = title;
                f.Size = new Size(620, 520);
                f.StartPosition = FormStartPosition.CenterParent;
                f.RightToLeft = RightToLeft.Yes;
                f.RightToLeftLayout = true;
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.MaximizeBox = false;
                f.MinimizeBox = false;

                Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 55, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(10) };
                Label lbl = new Label { Text = "🔍 بحث:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(440, 15), Size = new Size(70, 25), TextAlign = ContentAlignment.MiddleRight };
                TextBox txt = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(150, 14), Size = new Size(280, 25) };
                Label lblCount = new Label { Text = "", Font = new Font("Tahoma", 9, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(20, 16), Size = new Size(120, 22), TextAlign = ContentAlignment.MiddleRight };
                pnlSearch.Controls.AddRange(new Control[] { lbl, txt, lblCount });

                DataGridView dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    RowHeadersVisible = false,
                    EnableHeadersVisualStyles = false,
                    BackgroundColor = Color.White
                };
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.ColumnHeadersHeight = 38;
                dgv.RowTemplate.Height = 32;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

                dgv.Columns.Add("colCode", codeHeader);
                dgv.Columns.Add("colName", nameHeader);
                dgv.Columns["colCode"].Width = 160;
                dgv.Columns["colName"].Width = 400;
                dgv.Columns["colCode"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                void Refresh(string search = "")
                {
                    dgv.Rows.Clear();
                    int visible = 0;
                    foreach (var kv in data)
                    {
                        if (string.IsNullOrWhiteSpace(search) ||
                            (kv.Key ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (kv.Value ?? "").IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dgv.Rows.Add(kv.Key, kv.Value);
                            visible++;
                        }
                    }
                    lblCount.Text = $"📊 {visible}";
                }

                txt.TextChanged += (s, e) => Refresh(txt.Text);
                Refresh();

                Panel pnlBtn = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(240, 248, 255) };
                Button ok = new Button { Text = "✅ اختيار", DialogResult = DialogResult.OK, BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 10, FontStyle.Bold), Size = new Size(140, 40), Location = new Point(400, 10) };
                Button cancel = new Button { Text = "❌ إلغاء", DialogResult = DialogResult.Cancel, BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 10, FontStyle.Bold), Size = new Size(140, 40), Location = new Point(240, 10) };
                pnlBtn.Controls.AddRange(new Control[] { ok, cancel });
                f.AcceptButton = ok;
                f.CancelButton = cancel;

                dgv.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) ok.PerformClick(); };
                dgv.KeyDown += (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter && dgv.CurrentRow != null) { e.SuppressKeyPress = true; ok.PerformClick(); }
                };

                f.Controls.AddRange(new Control[] { dgv, pnlSearch, pnlBtn });

                if (f.ShowDialog() == DialogResult.OK && dgv.CurrentRow != null)
                {
                    selectedCode = dgv.CurrentRow.Cells["colCode"].Value?.ToString() ?? "";
                    selectedName = dgv.CurrentRow.Cells["colName"].Value?.ToString() ?? "";
                    return true;
                }
            }
            return false;
        }

        // ==========================================
        // ✅ التعبئة التلقائية للاسم عند إدخال الرقم يدوياً
        // ==========================================
        private void DgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string col = dgvData.Columns[e.ColumnIndex].Name;
            string code = dgvData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(code)) return;

            switch (col)
            {
                case "colItemId":
                    { var n = FindNameByCode(GetItemsData(), code); if (n != null) SetCell(e.RowIndex, "colItemName", n); }
                    break;
                case "colItemGroupId":
                    { var n = FindNameByCode(GetItemSubGroupsData(), code); if (n != null) SetCell(e.RowIndex, "colItemGroupName", n); }
                    break;
                case "colPortId":
                    { var n = FindNameByCode(GetPortsData(), code); if (n != null) SetCell(e.RowIndex, "colPortName", n); }
                    break;
                case "colPortGroupId":
                    { var n = FindNameByCode(GetPortGroupsData(), code); if (n != null) SetCell(e.RowIndex, "colPortGroupName", n); }
                    break;
            }
        }

        private string FindNameByCode(List<KeyValuePair<string, string>> data, string code)
        {
            foreach (var kv in data) if (kv.Key == code) return kv.Value;
            return null;
        }

        private void SetCell(int rowIndex, string colName, object value)
        {
            if (rowIndex < 0 || rowIndex >= dgvData.Rows.Count) return;
            if (dgvData.Columns[colName] == null) return;
            dgvData.Rows[rowIndex].Cells[colName].Value = value;
        }

        // ==========================================
        // ✅ جلب البيانات من قاعدة البيانات
        // ==========================================
        private string FindColumn(DataTable dt, string[] candidates)
        {
            foreach (var c in candidates) if (dt.Columns.Contains(c)) return c;
            return null;
        }

        private List<KeyValuePair<string, string>> GetItemsData()
        {
            var list = new List<KeyValuePair<string, string>>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Items ORDER BY ItemName");
                if (dt != null)
                {
                    string codeCol = FindColumn(dt, new[] { "ItemCode", "ItemNo", "ItemNumber", "Code", "Id" });
                    string nameCol = FindColumn(dt, new[] { "ItemName", "Name" });
                    if (nameCol != null)
                        foreach (DataRow r in dt.Rows)
                            list.Add(new KeyValuePair<string, string>(codeCol != null ? r[codeCol]?.ToString() ?? "" : "", r[nameCol]?.ToString() ?? ""));
                }
            }
            catch { }
            return list;
        }

        // ✅ مجموعة الأصناف الفرعية (SubGroups)
        // ✅ مجموعة الأصناف الفرعية (SubGroups) - نسخة محصنة ضد أخطاء الأعمدة
        private List<KeyValuePair<string, string>> GetItemSubGroupsData()
        {
            var list = new List<KeyValuePair<string, string>>();

            // 1. محاولة من جداول المجموعات الفرعية المستقلة
            string[] possibleTables = { "SubGroups", "ItemSubGroups", "SubGroupItems", "ItemsSubGroups" };
            foreach (string table in possibleTables)
            {
                try
                {
                    // التحقق من وجود الجدول أولاً لتجنب الأخطاء
                    var exists = DatabaseHelper.ExecuteScalar($"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{table}'");
                    if (exists == null || Convert.ToInt32(exists) == 0) continue;

                    // جلب البيانات بدون ORDER BY لتجنب أخطاء الأعمدة غير الموجودة
                    DataTable dt = DatabaseHelper.ExecuteQuery($"SELECT * FROM {table}");
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        // اكتشاف أسماء الأعمدة تلقائياً
                        string codeCol = FindColumn(dt, new[] { "SubGroupNo", "SubGroupCode", "Code", "Id", "GroupNo" });
                        string nameCol = FindColumn(dt, new[] { "SubGroupName", "Name", "GroupName" });

                        if (nameCol != null)
                        {
                            foreach (DataRow r in dt.Rows)
                            {
                                string code = codeCol != null ? r[codeCol]?.ToString() ?? "" : "";
                                string name = r[nameCol]?.ToString() ?? "";
                                if (!string.IsNullOrWhiteSpace(name))
                                    list.Add(new KeyValuePair<string, string>(code, name));
                            }
                            if (list.Count > 0) return list;
                        }
                    }
                }
                catch { }
            }

            // 2. احتياطي: من جدول الأصناف (Items) - البحث عن أعمدة المجموعات الفرعية
            try
            {
                DataTable dtSchema = DatabaseHelper.ExecuteQuery("PRAGMA table_info(Items)");
                if (dtSchema != null)
                {
                    var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    foreach (DataRow r in dtSchema.Rows) cols.Add(r["name"].ToString());

                    // البحث عن الأعمدة المحتملة (SubGroupNo أو SubGroupUnder)
                    string codeCol = cols.Contains("SubGroupNo") ? "SubGroupNo" : (cols.Contains("SubGroupUnder") ? "SubGroupUnder" : null);
                    string nameCol = cols.Contains("SubGroupName") ? "SubGroupName" : null;

                    if (nameCol != null || codeCol != null)
                    {
                        string selectCols = "";
                        if (codeCol != null && nameCol != null) selectCols = $"{codeCol}, {nameCol}";
                        else if (nameCol != null) selectCols = nameCol;
                        else selectCols = codeCol;

                        string whereCol = nameCol ?? codeCol;
                        string query = $"SELECT DISTINCT {selectCols} FROM Items WHERE {whereCol} IS NOT NULL AND {whereCol} <> '' ORDER BY {whereCol}";

                        DataTable dt2 = DatabaseHelper.ExecuteQuery(query);
                        if (dt2 != null)
                        {
                            foreach (DataRow r in dt2.Rows)
                            {
                                string c = codeCol != null ? r[codeCol]?.ToString() ?? "" : "";
                                string n = nameCol != null ? r[nameCol]?.ToString() ?? "" : "";
                                list.Add(new KeyValuePair<string, string>(c, n));
                            }
                            if (list.Count > 0) return list;
                        }
                    }
                }
            }
            catch { }

            // 3. احتياطي: من جدول التصاريح الزراعية (AgriculturalPermitItems)
            try
            {
                var exists = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='AgriculturalPermitItems'");
                if (exists != null && Convert.ToInt32(exists) > 0)
                {
                    DataTable dt3 = DatabaseHelper.ExecuteQuery("SELECT DISTINCT SubGroupNo, SubGroupName FROM AgriculturalPermitItems WHERE SubGroupName IS NOT NULL AND SubGroupName <> '' ORDER BY SubGroupName");
                    if (dt3 != null)
                    {
                        foreach (DataRow r in dt3.Rows)
                            list.Add(new KeyValuePair<string, string>(r["SubGroupNo"]?.ToString() ?? "", r["SubGroupName"]?.ToString() ?? ""));
                    }
                }
            }
            catch { }

            return list;
        }
        private List<KeyValuePair<string, string>> GetPortsData()
        {
            var list = new List<KeyValuePair<string, string>>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Ports ORDER BY PortName");
                if (dt != null)
                {
                    string codeCol = FindColumn(dt, new[] { "PortNo", "PortNumber", "PortId", "Id" });
                    string nameCol = FindColumn(dt, new[] { "PortName", "Name" });
                    if (nameCol != null)
                        foreach (DataRow r in dt.Rows)
                            list.Add(new KeyValuePair<string, string>(codeCol != null ? r[codeCol]?.ToString() ?? "" : "", r[nameCol]?.ToString() ?? ""));
                }
            }
            catch { }
            return list;
        }

        private List<KeyValuePair<string, string>> GetPortGroupsData()
        {
            var list = new List<KeyValuePair<string, string>>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM PortGroups ORDER BY GroupName");
                if (dt != null)
                {
                    string codeCol = FindColumn(dt, new[] { "GroupNo", "GroupNumber", "Code", "Id" });
                    string nameCol = FindColumn(dt, new[] { "GroupName", "Name" });
                    if (nameCol != null)
                        foreach (DataRow r in dt.Rows)
                            list.Add(new KeyValuePair<string, string>(codeCol != null ? r[codeCol]?.ToString() ?? "" : "", r[nameCol]?.ToString() ?? ""));
                }
            }
            catch { }
            return list;
        }

        // ==========================================
        // دوال مساعدة لإنشاء العناصر
        // ==========================================
        private void CreateLabelAndTextBox(Panel parent, string labelText, int labelX, int labelY, int textBoxX, int textBoxY, out Label lbl, out TextBox txt)
        {
            lbl = new Label { Text = labelText, Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(labelX, labelY), Size = new Size(130, 25), TextAlign = ContentAlignment.MiddleRight };
            parent.Controls.Add(lbl);
            txt = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(textBoxX, textBoxY), Size = new Size(180, 25), TextAlign = HorizontalAlignment.Right };
            parent.Controls.Add(txt);
        }

        private void CreateLabelAndComboBox(Panel parent, string labelText, int labelX, int labelY, int comboBoxX, int comboBoxY, out Label lbl, out ComboBox cmb)
        {
            lbl = new Label { Text = labelText, Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(labelX, labelY), Size = new Size(130, 25), TextAlign = ContentAlignment.MiddleRight };
            parent.Controls.Add(lbl);
            cmb = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 10), Location = new Point(comboBoxX, comboBoxY), Size = new Size(180, 25) };
            parent.Controls.Add(cmb);
        }

        private Button CreateButton(string text, Color backColor, int x, int y, EventHandler clickHandler)
        {
            Button btn = new Button { Text = text, Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = backColor, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(x, y), Size = new Size(150, 45) };
            btn.Click += clickHandler;
            return btn;
        }

        // ==========================================
        // تعبئة القوائم المنسدلة
        // ==========================================
        private void PopulateDropdowns()
        {
            cmbEffectType.Items.Add("على مستوى الصنف");
            cmbEffectType.Items.Add("على مستوى مجموعة الأصناف الفرعية");   // ✅
            cmbEffectType.Items.Add("على مستوى البيان");
            cmbEffectType.Items.Add("على مستوى القاطرة");

            cmbCurrency.Items.Add("ريال يمني");
            cmbCurrency.Items.Add("ريال جنوبي");
            cmbCurrency.Items.Add("دولار أمريكي");

            cmbValueType.Items.Add("مبلغ ثابت");
            cmbValueType.Items.Add("نسبة مئوية");

            cmbFeeScope.Items.Add("حسب المنفذ");
            cmbFeeScope.Items.Add("حسب مجموعة المنافذ");
        }

        private void Cmb_Changed(object sender, EventArgs e) => UpdateGridColumns();

        private void UpdateGridColumns()
        {
            dgvData.Columns.Clear();
            dgvData.Rows.Clear();

            string selectedEffect = cmbEffectType.Text;
            string selectedScope = cmbFeeScope.Text;

            void AddTextColumn(string name, string header, int width = 150)
            {
                var col = new DataGridViewTextBoxColumn { Name = name, HeaderText = header, Width = width };
                dgvData.Columns.Add(col);
            }

            void AddNumericColumn(string name, string header, int width = 120)
            {
                var col = new DataGridViewTextBoxColumn { Name = name, HeaderText = header, Width = width };
                col.DefaultCellStyle.Format = "N2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvData.Columns.Add(col);
            }

            void AddCalculationTypeColumn()
            {
                var col = new DataGridViewComboBoxColumn { Name = "colCalcType", HeaderText = "نوع الاحتساب", Width = 180 };
                col.Items.Add("الكمية");
                col.Items.Add("الكمية ومضاعفاتها");
                dgvData.Columns.Add(col);
            }

            void AddAmountColumn()
            {
                var col = new DataGridViewTextBoxColumn { Name = "colAmount", HeaderText = "مبلغ الرسوم الإضافية (للكرتون/القطعة)", Width = 180 };
                col.DefaultCellStyle.Format = "N2";
                col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvData.Columns.Add(col);
            }

            if (selectedEffect == "على مستوى البيان" && selectedScope == "حسب المنفذ")
            {
                AddTextColumn("colPortId", "رقم المنفذ (F9)", 110);
                AddTextColumn("colPortName", "اسم المنفذ", 220);
                AddAmountColumn();
            }
            else if (selectedEffect == "على مستوى البيان" && selectedScope == "حسب مجموعة المنافذ")
            {
                AddTextColumn("colPortGroupId", "رقم مجموعة المنافذ (F9)", 160);
                AddTextColumn("colPortGroupName", "اسم مجموعة المنافذ", 250);
                AddAmountColumn();
            }
            else if (selectedEffect == "على مستوى مجموعة الأصناف الفرعية" && selectedScope == "حسب المنفذ")
            {
                AddTextColumn("colPortId", "رقم المنفذ (F9)", 100);
                AddTextColumn("colPortName", "اسم المنفذ", 150);
                AddTextColumn("colItemGroupId", "رقم المجموعة الفرعية (F9)", 140);
                AddTextColumn("colItemGroupName", "اسم المجموعة الفرعية", 160);
                AddNumericColumn("colMinLimit", "الحد الأدنى", 100);
                AddCalculationTypeColumn();
                AddAmountColumn();
            }
            else if (selectedEffect == "على مستوى الصنف" && selectedScope == "حسب مجموعة المنافذ")
            {
                AddTextColumn("colPortGroupId", "رقم مجموعة المنافذ (F9)", 130);
                AddTextColumn("colPortGroupName", "اسم مجموعة المنافذ", 160);
                AddTextColumn("colItemId", "رقم الصنف (F9)", 110);
                AddTextColumn("colItemName", "اسم الصنف", 160);
                AddNumericColumn("colMinLimit", "الحد الأدنى", 100);
                AddCalculationTypeColumn();
                AddAmountColumn();
            }
            else if (selectedEffect == "على مستوى مجموعة الأصناف الفرعية" && selectedScope == "حسب مجموعة المنافذ")
            {
                AddTextColumn("colPortGroupId", "رقم مجموعة المنافذ (F9)", 130);
                AddTextColumn("colPortGroupName", "اسم مجموعة المنافذ", 160);
                AddTextColumn("colItemGroupId", "رقم المجموعة الفرعية (F9)", 130);
                AddTextColumn("colItemGroupName", "اسم المجموعة الفرعية", 160);
                AddNumericColumn("colMinLimit", "الحد الأدنى", 100);
                AddCalculationTypeColumn();
                AddAmountColumn();
            }
            else if (selectedEffect == "على مستوى الصنف" && selectedScope == "حسب المنفذ")
            {
                AddTextColumn("colPortId", "رقم المنفذ (F9)", 100);
                AddTextColumn("colPortName", "اسم المنفذ", 150);
                AddTextColumn("colItemId", "رقم الصنف (F9)", 110);
                AddTextColumn("colItemName", "اسم الصنف", 160);
                AddNumericColumn("colMinLimit", "الحد الأدنى", 100);
                AddCalculationTypeColumn();
                AddAmountColumn();
            }
            else if (selectedEffect == "على مستوى القاطرة" && selectedScope == "حسب المنفذ")
            {
                AddTextColumn("colPortId", "رقم المنفذ (F9)", 110);
                AddTextColumn("colPortName", "اسم المنفذ", 200);
                AddNumericColumn("colMinLimit", "الحد الأدنى", 100);
                AddCalculationTypeColumn();
                AddAmountColumn();
            }
            else if (selectedEffect == "على مستوى القاطرة" && selectedScope == "حسب مجموعة المنافذ")
            {
                AddTextColumn("colPortGroupId", "رقم مجموعة المنافذ (F9)", 150);
                AddTextColumn("colPortGroupName", "اسم مجموعة المنافذ", 250);
                AddNumericColumn("colMinLimit", "الحد الأدنى", 100);
                AddCalculationTypeColumn();
                AddAmountColumn();
            }
            else
            {
                AddTextColumn("colReference", "المرجع (رقم القاطرة)", 250);
                AddNumericColumn("colMinLimit", "الحد الأدنى", 120);
                AddCalculationTypeColumn();
                AddAmountColumn();
            }
        }

        // ==========================================
        // ✅ الحفظ الفعلي في قاعدة البيانات
        // ==========================================
        private void btnSave_Click(object sender, EventArgs e)
        {
            string feeNo = txtFeeNo.Text.Trim();
            string feeName = txtFeeName.Text.Trim();

            if (string.IsNullOrEmpty(feeNo)) { MessageBox.Show("⚠️ رقم الرسوم مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtFeeNo.Focus(); return; }
            if (string.IsNullOrEmpty(feeName)) { MessageBox.Show("⚠️ اسم الرسوم مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtFeeName.Focus(); return; }

            var rows = new List<Dictionary<string, string>>();
            foreach (DataGridViewRow r in dgvData.Rows)
            {
                if (r.IsNewRow) continue;
                bool hasData = false;
                foreach (DataGridViewCell c in r.Cells)
                    if (c.Value != null && !string.IsNullOrWhiteSpace(c.Value.ToString())) { hasData = true; break; }
                if (!hasData) continue;

                var d = new Dictionary<string, string>();
                foreach (DataGridViewColumn c in dgvData.Columns)
                    d[c.Name] = r.Cells[c.Name].Value?.ToString() ?? "";
                rows.Add(d);
            }

            if (rows.Count == 0)
            {
                MessageBox.Show("⚠️ أضف سطراً واحداً على الأقل في الجدول\n(المنفذ/المجموعة + الحد الأدنى + المبلغ)", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // حذف التعريف القديم لنفس الرقم ثم إعادة إدخاله
                DatabaseHelper.ExecuteNonQuery("DELETE FROM AdditionalFees WHERE FeeNo = @F",
                    DatabaseHelper.CreateParameter("@F", feeNo));

                foreach (var d in rows)
                {
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO AdditionalFees
                        (FeeNo, FeeName, EffectType, Currency, ValueType, FeeScope,
                         PortId, PortName, PortGroupId, PortGroupName,
                         ItemId, ItemName, ItemGroupId, ItemGroupName,
                         MinLimit, CalcType, Amount)
                        VALUES
                        (@FeeNo,@FeeName,@EffectType,@Currency,@ValueType,@FeeScope,
                         @PortId,@PortName,@PortGroupId,@PortGroupName,
                         @ItemId,@ItemName,@ItemGroupId,@ItemGroupName,
                         @MinLimit,@CalcType,@Amount)",
                        DatabaseHelper.CreateParameter("@FeeNo", feeNo),
                        DatabaseHelper.CreateParameter("@FeeName", feeName),
                        DatabaseHelper.CreateParameter("@EffectType", cmbEffectType.Text),
                        DatabaseHelper.CreateParameter("@Currency", cmbCurrency.Text),
                        DatabaseHelper.CreateParameter("@ValueType", cmbValueType.Text),
                        DatabaseHelper.CreateParameter("@FeeScope", cmbFeeScope.Text),
                        DatabaseHelper.CreateParameter("@PortId", GetVal(d, "colPortId")),
                        DatabaseHelper.CreateParameter("@PortName", GetVal(d, "colPortName")),
                        DatabaseHelper.CreateParameter("@PortGroupId", GetVal(d, "colPortGroupId")),
                        DatabaseHelper.CreateParameter("@PortGroupName", GetVal(d, "colPortGroupName")),
                        DatabaseHelper.CreateParameter("@ItemId", GetVal(d, "colItemId")),
                        DatabaseHelper.CreateParameter("@ItemName", GetVal(d, "colItemName")),
                        DatabaseHelper.CreateParameter("@ItemGroupId", GetVal(d, "colItemGroupId")),
                        DatabaseHelper.CreateParameter("@ItemGroupName", GetVal(d, "colItemGroupName")),
                        DatabaseHelper.CreateParameter("@MinLimit", GetDec(d, "colMinLimit")),
                        DatabaseHelper.CreateParameter("@CalcType", GetVal(d, "colCalcType")),
                        DatabaseHelper.CreateParameter("@Amount", GetDec(d, "colAmount")));
                }

                MessageBox.Show($"✅ تم حفظ الرسوم \"{feeName}\" بنجاح ({rows.Count} سجل)\n\n" +
                    "💡 سيظهر تلقائياً في شاشة طلب الترسيم ← تبويب المبالغ الإضافية\n" +
                    "عند تطابق المنفذ/مجموعة المنافذ والصنف/المجموعة الفرعية\n" +
                    "وسيُحسب: مبلغ الوحدة × الكمية المدخلة",
                    "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الحفظ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // ✅ تحميل تعريف محفوظ عند مغادرة حقل الرقم
        // ==========================================
        private void TryLoadFee(string feeNo)
        {
            if (string.IsNullOrWhiteSpace(feeNo)) return;
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM AdditionalFees WHERE FeeNo = @F ORDER BY Id",
                    DatabaseHelper.CreateParameter("@F", feeNo));
                if (dt == null || dt.Rows.Count == 0) return;

                DataRow first = dt.Rows[0];
                cmbEffectType.Text = first["EffectType"]?.ToString() ?? "";
                cmbCurrency.Text = first["Currency"]?.ToString() ?? "";
                cmbValueType.Text = first["ValueType"]?.ToString() ?? "";
                cmbFeeScope.Text = first["FeeScope"]?.ToString() ?? "";
                txtFeeName.Text = first["FeeName"]?.ToString() ?? "";

                dgvData.Rows.Clear();
                foreach (DataRow r in dt.Rows)
                {
                    int idx = dgvData.Rows.Add();
                    var row = dgvData.Rows[idx];
                    SetIfExists(row, "colPortId", r["PortId"]);
                    SetIfExists(row, "colPortName", r["PortName"]);
                    SetIfExists(row, "colPortGroupId", r["PortGroupId"]);
                    SetIfExists(row, "colPortGroupName", r["PortGroupName"]);
                    SetIfExists(row, "colItemId", r["ItemId"]);
                    SetIfExists(row, "colItemName", r["ItemName"]);
                    SetIfExists(row, "colItemGroupId", r["ItemGroupId"]);
                    SetIfExists(row, "colItemGroupName", r["ItemGroupName"]);
                    SetIfExists(row, "colMinLimit", r["MinLimit"]);
                    SetIfExists(row, "colCalcType", r["CalcType"]);
                    SetIfExists(row, "colAmount", r["Amount"]);
                }
            }
            catch { }
        }

        private void SetIfExists(DataGridViewRow row, string col, object value)
        {
            if (dgvData.Columns[col] != null && value != null && value != DBNull.Value)
                row.Cells[col].Value = value.ToString();
        }

        private string GetVal(Dictionary<string, string> d, string key) => d.ContainsKey(key) ? d[key] : "";

        private decimal GetDec(Dictionary<string, string> d, string key)
        {
            string s = d.ContainsKey(key) ? d[key] : "";
            s = (s ?? "").Replace(",", "").Trim();
            return decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal v) ? v : 0;
        }

        // ==========================================
        // أحداث الأزرار
        // ==========================================
        private void btnNew_Click(object sender, EventArgs e)
        {
            txtFeeNo.Clear();
            txtFeeName.Clear();
            cmbEffectType.SelectedIndex = 0;
            cmbCurrency.SelectedIndex = 0;
            cmbValueType.SelectedIndex = 0;
            cmbFeeScope.SelectedIndex = 0;
            dgvData.Rows.Clear();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null && !dgvData.CurrentRow.IsNewRow)
                dgvData.Rows.Remove(dgvData.CurrentRow);
        }
    }
}