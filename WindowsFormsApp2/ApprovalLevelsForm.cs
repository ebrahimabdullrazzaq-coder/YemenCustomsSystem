using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class ApprovalLevelsForm : Form
    {
        // ==================== ✅ نماذج البيانات ====================
        public class ApprovalLevel
        {
            public int Id { get; set; }
            public string ScreenTag { get; set; }
            public int LevelNo { get; set; }
            public string LevelName { get; set; }
            public int OrderNo { get; set; }
            public string EnteredBy { get; set; }
            public string EnteredDate { get; set; }
            public string EnteredDevice { get; set; }
            public string ModifiedBy { get; set; }
            public string ModifiedDate { get; set; }
            public string ModifiedDevice { get; set; }
            public int PrintCount { get; set; }
            public int ModifyCount { get; set; }
        }

        public class LevelUser
        {
            public string UserNo { get; set; }
            public string UserName { get; set; }
        }

        // ==================== ✅ الشاشات المتاحة للاعتماد ====================
        private readonly List<(string Tag, string Name)> _screens = new List<(string, string)>
        {
            ("TariffRequest", "طلب ترسيم جمركي")
        };

        // ==================== ✅ المتغيرات ====================
        private List<ApprovalLevel> _levels = new List<ApprovalLevel>();
        private List<string> _screensWithLevels = new List<string>();
        private int _navIndex = -1;
        private bool _isEditing = false;
        private bool _locked = false;

        // ✅ متغير بسيط لتتبع المستوى المحدد حالياً (للاستخدام الداخلي فقط)
        private int _selectedLevelId = 0;

        // ==================== ✅ عناصر الواجهة ====================
        private Panel pnlTopInfo, pnlMain, pnlFooter;
        private Label lblUserInfo, lblScreenPath, lblPeriod, lblDate;
        private ToolStrip toolStrip1;
        private ToolStripButton btnNew, btnEdit, btnDelete, btnSave, btnFirst, btnPrev, btnNext, btnLast, btnSearch, btnPrint, btnLock;
        private Label lblScreen;
        private ComboBox cmbScreen;
        private DataGridView dgvLevels, dgvUsers;
        private TextBox txtEnteredBy, txtModifiedBy, txtEnteredDate, txtModifiedDate, txtEnteredDevice, txtModifiedDevice, txtPrintCount, txtModifyCount;

        public ApprovalLevelsForm()
        {
            EnsureTables();
            BuildUI();
            LoadScreensWithLevels();
            SetEditing(false);
            UpdateFooter(null);

            dgvLevels.Rows.Clear();
            dgvUsers.Rows.Clear();
        }

        private void EnsureTables()
        {
            try
            {
                var chk = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='ApprovalLevels'");
                bool exists = chk != null && Convert.ToInt32(chk) > 0;

                if (exists)
                {
                    DataTable schema = DatabaseHelper.ExecuteQuery("PRAGMA table_info(ApprovalLevels)");
                    var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    if (schema != null)
                        foreach (DataRow r in schema.Rows)
                            cols.Add(r["name"].ToString());

                    if (!cols.Contains("ScreenTag"))
                    {
                        DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE ApprovalLevels_New (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            ScreenTag TEXT NOT NULL, LevelNo INTEGER NOT NULL, LevelName TEXT NOT NULL, OrderNo INTEGER DEFAULT 1,
                            EnteredBy TEXT, EnteredDate TEXT DEFAULT (datetime('now','localtime')), EnteredDevice TEXT,
                            ModifiedBy TEXT, ModifiedDate TEXT, ModifiedDevice TEXT, PrintCount INTEGER DEFAULT 0, ModifyCount INTEGER DEFAULT 0)");

                        try
                        {
                            DatabaseHelper.ExecuteNonQuery(@"INSERT INTO ApprovalLevels_New (ScreenTag, LevelNo, LevelName, OrderNo, EnteredBy, EnteredDate, PrintCount, ModifyCount)
                                SELECT CASE WHEN DocumentType IS NULL OR DocumentType = '' THEN 'TariffRequest' ELSE DocumentType END,
                                LevelNo, LevelName, LevelNo, CreatedBy, CreatedDate, 0, 0 FROM ApprovalLevels");
                        }
                        catch { }

                        DatabaseHelper.ExecuteNonQuery("DROP TABLE ApprovalLevels");
                        DatabaseHelper.ExecuteNonQuery("ALTER TABLE ApprovalLevels_New RENAME TO ApprovalLevels");
                        DatabaseHelper.ExecuteNonQuery("DELETE FROM ApprovalLevels WHERE ScreenTag <> 'TariffRequest'");
                    }
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE ApprovalLevels (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT, ScreenTag TEXT NOT NULL, LevelNo INTEGER NOT NULL, LevelName TEXT NOT NULL, OrderNo INTEGER DEFAULT 1,
                        EnteredBy TEXT, EnteredDate TEXT DEFAULT (datetime('now','localtime')), EnteredDevice TEXT,
                        ModifiedBy TEXT, ModifiedDate TEXT, ModifiedDevice TEXT, PrintCount INTEGER DEFAULT 0, ModifyCount INTEGER DEFAULT 0)");
                }

                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS ApprovalLevelUsers (Id INTEGER PRIMARY KEY AUTOINCREMENT, LevelId INTEGER NOT NULL, UserNo TEXT, UserName TEXT)");

                DataTable uSchema = DatabaseHelper.ExecuteQuery("PRAGMA table_info(ApprovalLevelUsers)");
                bool hasLevelId = false;
                if (uSchema != null)
                    foreach (DataRow r in uSchema.Rows)
                        if (r["name"].ToString().Equals("LevelId", StringComparison.OrdinalIgnoreCase)) hasLevelId = true;

                if (!hasLevelId)
                {
                    DatabaseHelper.ExecuteNonQuery("DROP TABLE ApprovalLevelUsers");
                    DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE ApprovalLevelUsers (Id INTEGER PRIMARY KEY AUTOINCREMENT, LevelId INTEGER NOT NULL, UserNo TEXT, UserName TEXT)");
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
        }

        private void BuildUI()
        {
            this.Text = "(مستويات الإعتماد) - نظام الجمارك اليمنية";
            this.Size = new Size(1300, 850);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(184, 216, 240);

            pnlTopInfo = new Panel { Dock = DockStyle.Top, Height = 30, BackColor = Color.FromArgb(238, 244, 250) };
            var u = UserManager.GetCurrentUser();
            lblUserInfo = new Label { Text = $"المستخدم : {u?.Id ?? 0} - {u?.FullName ?? ""}", Font = new Font("Tahoma", 9, FontStyle.Bold), Location = new Point(1050, 5), Size = new Size(240, 22), TextAlign = ContentAlignment.MiddleRight };
            lblScreenPath = new Label { Text = "تهيئة النظام - التهيئة - مستويات الإعتماد", Font = new Font("Tahoma", 9, FontStyle.Bold), Location = new Point(450, 5), Size = new Size(400, 22), TextAlign = ContentAlignment.MiddleCenter };
            lblPeriod = new Label { Text = DateTime.Now.ToString("yyyy / M"), Font = new Font("Tahoma", 9, FontStyle.Bold), Location = new Point(300, 5), Size = new Size(80, 22), TextAlign = ContentAlignment.MiddleCenter, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            lblDate = new Label { Text = DateTime.Now.ToString("dd/MM/yyyy"), Font = new Font("Tahoma", 9, FontStyle.Bold), Location = new Point(180, 5), Size = new Size(100, 22), TextAlign = ContentAlignment.MiddleCenter, BorderStyle = BorderStyle.FixedSingle, BackColor = Color.White };
            pnlTopInfo.Controls.AddRange(new Control[] { lblUserInfo, lblScreenPath, lblPeriod, lblDate });

            toolStrip1 = new ToolStrip { Dock = DockStyle.Top, BackColor = Color.FromArgb(238, 244, 250), GripStyle = ToolStripGripStyle.Hidden, RightToLeft = RightToLeft.Yes, Font = new Font("Tahoma", 9, FontStyle.Bold) };
            btnNew = new ToolStripButton("➕") { ToolTipText = "جديد", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnEdit = new ToolStripButton("✏️") { ToolTipText = "تعديل", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnDelete = new ToolStripButton("❌") { ToolTipText = "حذف", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnSave = new ToolStripButton("💾") { ToolTipText = "حفظ", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnFirst = new ToolStripButton("⏮") { ToolTipText = "الأول", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnPrev = new ToolStripButton("◀") { ToolTipText = "السابق", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnNext = new ToolStripButton("▶") { ToolTipText = "التالي", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnLast = new ToolStripButton("⏭") { ToolTipText = "الأخير", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnSearch = new ToolStripButton("🔍") { ToolTipText = "بحث", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnPrint = new ToolStripButton("🖨️") { ToolTipText = "طباعة", DisplayStyle = ToolStripItemDisplayStyle.Text };
            btnLock = new ToolStripButton("🔓") { ToolTipText = "قفل/فتح الشاشة", DisplayStyle = ToolStripItemDisplayStyle.Text };

            btnNew.Click += BtnNew_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnSave.Click += BtnSave_Click;
            btnFirst.Click += (s, e) => Navigate(0);
            btnPrev.Click += (s, e) => Navigate(-1);
            btnNext.Click += (s, e) => Navigate(1);
            btnLast.Click += (s, e) => Navigate(99);
            btnSearch.Click += BtnSearch_Click;
            btnPrint.Click += BtnPrint_Click;
            btnLock.Click += BtnLock_Click;

            ToolStripButton btnLoad = new ToolStripButton("📥 تحميل المستويات") { ToolTipText = "تحميل المستويات للشاشة المحددة", DisplayStyle = ToolStripItemDisplayStyle.Text, Font = new Font("Tahoma", 9, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243) };
            btnLoad.Click += BtnLoad_Click;

            toolStrip1.Items.AddRange(new ToolStripItem[] { btnNew, btnEdit, btnDelete, btnSave, new ToolStripSeparator(), btnLoad, new ToolStripSeparator(), btnFirst, btnPrev, btnNext, btnLast, new ToolStripSeparator(), btnSearch, btnPrint, btnLock });

            Panel pnlScreen = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(184, 216, 240) };
            Panel box = new Panel { Location = new Point(300, 10), Size = new Size(700, 40), BackColor = Color.FromArgb(184, 216, 240), BorderStyle = BorderStyle.FixedSingle };
            lblScreen = new Label { Text = "الشاشة", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(620, 8), Size = new Size(70, 24), TextAlign = ContentAlignment.MiddleRight };
            cmbScreen = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(90, 6), Size = new Size(520, 26), BackColor = Color.White };
            foreach (var s in _screens) cmbScreen.Items.Add(s.Name);
            cmbScreen.SelectedIndexChanged += CmbScreen_SelectedIndexChanged;
            box.Controls.AddRange(new Control[] { lblScreen, cmbScreen });
            pnlScreen.Controls.Add(box);

            dgvLevels = new DataGridView { Dock = DockStyle.Top, Height = 200, BackgroundColor = Color.FromArgb(184, 216, 240), BorderStyle = BorderStyle.FixedSingle, RowHeadersVisible = false, AllowUserToAddRows = false, AllowUserToDeleteRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, EnableHeadersVisualStyles = false, Font = new Font("Tahoma", 10), ColumnHeadersHeight = 32, RowTemplate = { Height = 30 } };
            dgvLevels.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(184, 216, 240);
            dgvLevels.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvLevels.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
            dgvLevels.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLevelId", Visible = false });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLevelNo", HeaderText = "رقم المستوى", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(224, 247, 250), Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLevelName", HeaderText = "اسم المستوى", Width = 380, DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(255, 255, 224) } });
            dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "colOrder", HeaderText = "الترتيب", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(255, 224, 224), Alignment = DataGridViewContentAlignment.MiddleCenter } });
            var colScreen = new DataGridViewComboBoxColumn { Name = "colScreen", HeaderText = "الشاشة", Width = 260, DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing };
            foreach (var s in _screens) colScreen.Items.Add(s.Name);
            colScreen.ReadOnly = true;
            dgvLevels.Columns.Add(colScreen);

            dgvLevels.SelectionChanged += DgvLevels_SelectionChanged;
            dgvLevels.CellValueChanged += DgvLevels_CellValueChanged;

            dgvUsers = new DataGridView { Dock = DockStyle.Fill, BackgroundColor = Color.FromArgb(184, 216, 240), BorderStyle = BorderStyle.FixedSingle, RowHeadersVisible = false, AllowUserToAddRows = true, AllowUserToDeleteRows = true, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, EnableHeadersVisualStyles = false, Font = new Font("Tahoma", 10), ColumnHeadersHeight = 32, RowTemplate = { Height = 30 } };
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(184, 216, 240);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
            dgvUsers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserNo", HeaderText = "رقم المستخدم", Width = 150, DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(224, 247, 250), Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserName", HeaderText = "اسم المستخدم", Width = 450, DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White } });
            dgvUsers.CellValueChanged += DgvUsers_CellValueChanged;
            dgvUsers.KeyDown += DgvUsers_KeyDown;

            Panel pnlGrids = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(184, 216, 240), Padding = new Padding(150, 10, 150, 10) };
            pnlGrids.Controls.Add(dgvUsers);
            pnlGrids.Controls.Add(dgvLevels);

            pnlFooter = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(122, 178, 222) };
            int y1 = 5, y2 = 32;
            AddFooterField(pnlFooter, "مدخل السجل", out txtEnteredBy, 1080, y1, 200);
            AddFooterField(pnlFooter, "معدل السجل", out txtModifiedBy, 1080, y2, 200);
            AddFooterField(pnlFooter, "تاريخ الإدخال", out txtEnteredDate, 780, y1, 170);
            AddFooterField(pnlFooter, "تاريخ آخر تعديل", out txtModifiedDate, 780, y2, 170);
            AddFooterField(pnlFooter, "الجهاز المدخل", out txtEnteredDevice, 560, y1, 150);
            AddFooterField(pnlFooter, "الجهاز المعدل", out txtModifiedDevice, 560, y2, 150);
            AddFooterField(pnlFooter, "مرات الطباعة", out txtPrintCount, 380, y1, 80);
            AddFooterField(pnlFooter, "مرات التعديل", out txtModifyCount, 380, y2, 80);

            this.Controls.Add(pnlGrids);
            this.Controls.Add(pnlScreen);
            this.Controls.Add(pnlFooter);
            this.Controls.Add(toolStrip1);
            this.Controls.Add(pnlTopInfo);
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            string tag = CurrentScreenTag();
            if (string.IsNullOrEmpty(tag)) { MessageBox.Show("⚠️ الرجاء اختيار شاشة أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            LoadLevelsForScreen();
            MessageBox.Show($"✅ تم تحميل {_levels.Count} مستوى للشاشة {cmbScreen.SelectedItem}", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddFooterField(Panel parent, string caption, out TextBox txt, int x, int y, int w)
        {
            Label lbl = new Label { Text = caption, Font = new Font("Tahoma", 9, FontStyle.Bold), ForeColor = Color.Black, Location = new Point(x + w + 5, y), Size = new Size(110, 22), TextAlign = ContentAlignment.MiddleRight };
            txt = new TextBox { ReadOnly = true, Font = new Font("Tahoma", 9, FontStyle.Bold), BackColor = Color.FromArgb(224, 247, 250), BorderStyle = BorderStyle.FixedSingle, Location = new Point(x, y), Size = new Size(w, 22), TextAlign = HorizontalAlignment.Center };
            parent.Controls.Add(lbl);
            parent.Controls.Add(txt);
        }

        private void LoadScreensWithLevels()
        {
            _screensWithLevels.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT DISTINCT ScreenTag FROM ApprovalLevels ORDER BY ScreenTag");
                if (dt != null) foreach (DataRow r in dt.Rows) _screensWithLevels.Add(r["ScreenTag"].ToString());
            }
            catch { }
            if (cmbScreen.SelectedIndex < 0 && cmbScreen.Items.Count > 0) cmbScreen.SelectedIndex = 0;
        }

        private string CurrentScreenTag()
        {
            string name = cmbScreen.SelectedItem?.ToString() ?? "";
            return _screens.FirstOrDefault(s => s.Name == name).Tag ?? "";
        }

        private void LoadLevelsForScreen()
        {
            string tag = CurrentScreenTag();
            _levels.Clear();
            dgvLevels.Rows.Clear();
            dgvUsers.Rows.Clear();
            _selectedLevelId = 0;
            if (string.IsNullOrEmpty(tag)) return;

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM ApprovalLevels WHERE ScreenTag = @T ORDER BY OrderNo ASC",
                    DatabaseHelper.CreateParameter("@T", tag));

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                    {
                        var lv = new ApprovalLevel
                        {
                            Id = Convert.ToInt32(r["Id"]),
                            ScreenTag = r["ScreenTag"].ToString(),
                            LevelNo = Convert.ToInt32(r["LevelNo"]),
                            LevelName = r["LevelName"].ToString(),
                            OrderNo = r["OrderNo"] != DBNull.Value ? Convert.ToInt32(r["OrderNo"]) : Convert.ToInt32(r["LevelNo"]),
                            EnteredBy = r["EnteredBy"]?.ToString() ?? "",
                            EnteredDate = r["EnteredDate"]?.ToString() ?? "",
                            EnteredDevice = r["EnteredDevice"]?.ToString() ?? "",
                            ModifiedBy = r["ModifiedBy"]?.ToString() ?? "",
                            ModifiedDate = r["ModifiedDate"]?.ToString() ?? "",
                            ModifiedDevice = r["ModifiedDevice"]?.ToString() ?? "",
                            PrintCount = r["PrintCount"] != DBNull.Value ? Convert.ToInt32(r["PrintCount"]) : 0,
                            ModifyCount = r["ModifyCount"] != DBNull.Value ? Convert.ToInt32(r["ModifyCount"]) : 0
                        };
                        _levels.Add(lv);
                        dgvLevels.Rows.Add(lv.Id, lv.OrderNo, lv.LevelName, lv.OrderNo, GetScreenName(lv.ScreenTag));
                    }

                    if (dgvLevels.Rows.Count > 0)
                    {
                        dgvLevels.ClearSelection();
                        dgvLevels.Rows[0].Selected = true;
                        _selectedLevelId = _levels[0].Id;
                        LoadUsersForLevel(_selectedLevelId);
                    }
                }
                else
                {
                    MessageBox.Show("📭 لا توجد مستويات محفوظة لهذه الشاشة\n\n💡 اضغط 'جديد' لإضافة مستوى جديد", "معلومات", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetScreenName(string tag) => _screens.FirstOrDefault(s => s.Tag == tag).Name ?? tag;

        // ==================== ✅ تحميل المستخدمين - نسخة مبسطة ====================
        private void LoadUsersForLevel(int levelId)
        {
            dgvUsers.Rows.Clear();

            if (levelId <= 0)
            {
                // مستوى جديد لم يُحفظ بعد
                return;
            }

            var list = new List<LevelUser>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT UserNo, UserName FROM ApprovalLevelUsers WHERE LevelId = @L ORDER BY Id",
                    DatabaseHelper.CreateParameter("@L", levelId));
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow r in dt.Rows)
                        list.Add(new LevelUser { UserNo = r["UserNo"].ToString(), UserName = r["UserName"].ToString() });
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ في تحميل المستخدمين للمستوى {levelId}: {ex.Message}"); }

            if (list.Count > 0)
            {
                foreach (var lu in list) dgvUsers.Rows.Add(lu.UserNo, lu.UserName);
            }
        }

        // ==================== ✅ جمع المستخدمين من الجدول ====================
        private List<LevelUser> GetUsersFromGrid()
        {
            var list = new List<LevelUser>();
            foreach (DataGridViewRow r in dgvUsers.Rows)
            {
                if (r.IsNewRow) continue;
                string no = r.Cells["colUserNo"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(no)) continue;

                list.Add(new LevelUser
                {
                    UserNo = no,
                    UserName = r.Cells["colUserName"].Value?.ToString() ?? ""
                });
            }
            return list;
        }

        private void CmbScreen_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLevelsForScreen();
        }

        private void DgvLevels_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLevels.CurrentRow == null)
            {
                UpdateFooter(null);
                return;
            }

            object idValue = dgvLevels.CurrentRow.Cells["colLevelId"].Value;
            int id = 0;

            if (idValue != null && int.TryParse(idValue.ToString(), out id) && id > 0)
            {
                _selectedLevelId = id;
                var level = _levels.FirstOrDefault(l => l.Id == id);
                if (level != null)
                {
                    LoadUsersForLevel(id);
                    UpdateFooter(level);
                }
            }
            else
            {
                // مستوى جديد (Id = 0)
                _selectedLevelId = 0;
                dgvUsers.Rows.Clear();
                UpdateFooter(null);
            }
        }

        private void DgvLevels_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isEditing || e.RowIndex < 0) return;
            string colName = dgvLevels.Columns[e.ColumnIndex].Name;

            if (colName == "colLevelNo")
            {
                object levelNoValue = dgvLevels.Rows[e.RowIndex].Cells["colLevelNo"].Value;
                if (levelNoValue != null && !string.IsNullOrWhiteSpace(levelNoValue.ToString()))
                    dgvLevels.Rows[e.RowIndex].Cells["colOrder"].Value = levelNoValue;
            }
            if (colName == "colOrder")
            {
                object orderValue = dgvLevels.Rows[e.RowIndex].Cells["colOrder"].Value;
                if (orderValue != null && !string.IsNullOrWhiteSpace(orderValue.ToString()))
                    dgvLevels.Rows[e.RowIndex].Cells["colLevelNo"].Value = orderValue;
            }
        }

        private void DgvUsers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_isEditing || e.RowIndex < 0) return;
            if (dgvUsers.Columns[e.ColumnIndex].Name == "colUserNo")
            {
                string no = dgvUsers.Rows[e.RowIndex].Cells["colUserNo"].Value?.ToString() ?? "";
                dgvUsers.Rows[e.RowIndex].Cells["colUserName"].Value = LookupUserName(no);
            }
        }

        private void DgvUsers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9 && _isEditing) { OpenUserSelectionForm(); e.Handled = true; }
        }

        private string LookupUserName(string no)
        {
            if (string.IsNullOrWhiteSpace(no)) return "";
            try
            {
                var r = DatabaseHelper.ExecuteScalar("SELECT FullName FROM Users WHERE Id = @Id OR Username = @Id OR UserNumber = @Id LIMIT 1", DatabaseHelper.CreateParameter("@Id", no));
                return r?.ToString() ?? "";
            }
            catch { return ""; }
        }

        // ==================== ✅ نافذة اختيار المستخدم ====================
        private void OpenUserSelectionForm()
        {
            using (Form f = new Form
            {
                Text = "🔍 اختر المستخدم",
                Size = new Size(800, 600),
                StartPosition = FormStartPosition.CenterParent,
                RightToLeft = RightToLeft.Yes,
                RightToLeftLayout = true,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            })
            {
                Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(10) };
                Label lblSearch = new Label { Text = "🔍 بحث:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(650, 20), Size = new Size(80, 25), TextAlign = ContentAlignment.MiddleRight };
                TextBox txtSearch = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(350, 18), Size = new Size(290, 27) };
                Label lblCount = new Label { Text = "📊 الإجمالي: 0", Font = new Font("Tahoma", 10, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(20, 20), Size = new Size(200, 25) };
                pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, lblCount });

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
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
                dgv.ColumnHeadersHeight = 40;
                dgv.RowTemplate.Height = 35;

                dgv.Columns.Add("colId", "رقم المستخدم");
                dgv.Columns.Add("colUsername", "اسم الدخول");
                dgv.Columns.Add("colFullName", "الاسم الكامل");
                dgv.Columns.Add("colRole", "الدور");

                int totalCount = 0;
                try
                {
                    DataTable dt = DatabaseHelper.ExecuteQuery("SELECT Id, UserNumber, Username, FullName, Role FROM Users WHERE IsActive = 1 ORDER BY Id");
                    if (dt != null)
                    {
                        foreach (DataRow r in dt.Rows)
                        {
                            string roleDisplay = UserManager.GetRoleArabicName(r["Role"]?.ToString() ?? "");
                            int rowIdx = dgv.Rows.Add(
                                r["UserNumber"]?.ToString() ?? r["Id"].ToString(),
                                r["Username"]?.ToString() ?? "",
                                r["FullName"]?.ToString() ?? "",
                                roleDisplay);
                            totalCount++;
                        }
                    }
                }
                catch (Exception ex) { MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }

                lblCount.Text = $"📊 الإجمالي: {totalCount}";

                txtSearch.TextChanged += (s, ev) =>
                {
                    string search = txtSearch.Text.Trim().ToLower();
                    int visibleCount = 0;
                    foreach (DataGridViewRow row in dgv.Rows)
                    {
                        if (row.IsNewRow) continue;
                        string userNo = row.Cells["colId"].Value?.ToString()?.ToLower() ?? "";
                        string fullName = row.Cells["colFullName"].Value?.ToString()?.ToLower() ?? "";

                        if (string.IsNullOrEmpty(search) || userNo.Contains(search) || fullName.Contains(search))
                        { row.Visible = true; visibleCount++; }
                        else { row.Visible = false; }
                    }
                    lblCount.Text = $"📊 المعروض: {visibleCount} من {totalCount}";
                };

                Panel pnlButtons = new Panel { Height = 70, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(240, 248, 255) };
                Button btnSelect = new Button { Text = "✅ اختيار", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 45), Location = new Point(550, 12), DialogResult = DialogResult.OK };
                Button btnCancel = new Button { Text = "❌ إلغاء", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 45), Location = new Point(380, 12), DialogResult = DialogResult.Cancel };
                pnlButtons.Controls.Add(btnSelect);
                pnlButtons.Controls.Add(btnCancel);

                // ✅ الحل الجذري: استخدام متغير محلي لتحديد الصف المستهدف
                int targetRowIndex = -1;

                // إذا كان dgvUsers فارغاً أو يحتوي فقط على صف جديد، أضف صف جديد
                bool needNewRow = true;
                foreach (DataGridViewRow r in dgvUsers.Rows)
                {
                    if (!r.IsNewRow)
                    {
                        string no = r.Cells["colUserNo"].Value?.ToString() ?? "";
                        if (string.IsNullOrWhiteSpace(no))
                        {
                            targetRowIndex = r.Index;
                            needNewRow = false;
                            break;
                        }
                    }
                }

                if (needNewRow)
                {
                    targetRowIndex = dgvUsers.Rows.Add("", "");
                }

                dgv.CellDoubleClick += (s, ev) =>
                {
                    if (ev.RowIndex >= 0 && targetRowIndex >= 0)
                    {
                        dgvUsers.Rows[targetRowIndex].Cells["colUserNo"].Value = dgv.Rows[ev.RowIndex].Cells["colId"].Value?.ToString();
                        dgvUsers.Rows[targetRowIndex].Cells["colUserName"].Value = dgv.Rows[ev.RowIndex].Cells["colFullName"].Value?.ToString();
                        f.DialogResult = DialogResult.OK;
                        f.Close();
                    }
                };

                dgv.KeyDown += (s, ev) =>
                {
                    if (ev.KeyCode == Keys.Enter && dgv.CurrentRow != null && targetRowIndex >= 0)
                    {
                        ev.SuppressKeyPress = true;
                        dgvUsers.Rows[targetRowIndex].Cells["colUserNo"].Value = dgv.CurrentRow.Cells["colId"].Value?.ToString();
                        dgvUsers.Rows[targetRowIndex].Cells["colUserName"].Value = dgv.CurrentRow.Cells["colFullName"].Value?.ToString();
                        f.DialogResult = DialogResult.OK;
                        f.Close();
                    }
                };

                f.Controls.Add(dgv);
                f.Controls.Add(pnlSearch);
                f.Controls.Add(pnlButtons);
                f.AcceptButton = btnSelect;
                f.CancelButton = btnCancel;
                f.ShowDialog();
            }
        }

        // ==================== ✅ جديد / تعديل / حذف / حفظ ====================
        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (_locked) { MsgLocked(); return; }

            dgvLevels.Rows.Clear();
            dgvUsers.Rows.Clear();
            _selectedLevelId = 0;

            int nextOrder = _levels.Count > 0 ? _levels.Max(l => l.OrderNo) + 1 : 1;
            dgvLevels.Rows.Add(0, nextOrder, "", nextOrder, "طلب ترسيم جمركي");

            dgvLevels.ClearSelection();
            dgvLevels.Rows[0].Selected = true;

            SetEditing(true);
        }

        public static List<ApprovalLevel> GetApprovalLevels(string screenTag)
        {
            var list = new List<ApprovalLevel>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM ApprovalLevels WHERE ScreenTag = @T ORDER BY OrderNo, LevelNo", DatabaseHelper.CreateParameter("@T", screenTag));
                if (dt != null) foreach (DataRow r in dt.Rows) list.Add(new ApprovalLevel { Id = Convert.ToInt32(r["Id"]), ScreenTag = r["ScreenTag"].ToString(), LevelNo = Convert.ToInt32(r["LevelNo"]), LevelName = r["LevelName"].ToString(), OrderNo = r["OrderNo"] != DBNull.Value ? Convert.ToInt32(r["OrderNo"]) : 1 });
            }
            catch { }
            return list;
        }

        public static List<LevelUser> GetLevelUsers(int levelId)
        {
            var list = new List<LevelUser>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT UserNo, UserName FROM ApprovalLevelUsers WHERE LevelId = @L ORDER BY Id", DatabaseHelper.CreateParameter("@L", levelId));
                if (dt != null) foreach (DataRow r in dt.Rows) list.Add(new LevelUser { UserNo = r["UserNo"].ToString(), UserName = r["UserName"].ToString() });
            }
            catch { }
            return list;
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (_locked) { MsgLocked(); return; }
            if (dgvLevels.Rows.Count == 0) { MessageBox.Show("⚠️ لا توجد مستويات لهذه الشاشة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SetEditing(true);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_locked) { MsgLocked(); return; }
            if (dgvLevels.CurrentRow == null) return;
            int id = 0;
            int.TryParse(dgvLevels.CurrentRow.Cells["colLevelId"].Value?.ToString(), out id);

            var res = MessageBox.Show("🗑️ هل أنت متأكد من حذف المستوى المحدد ومستخدميه؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (res != DialogResult.Yes) return;

            if (id > 0)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM ApprovalLevelUsers WHERE LevelId = @L", DatabaseHelper.CreateParameter("@L", id));
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM ApprovalLevels WHERE Id = @Id", DatabaseHelper.CreateParameter("@Id", id));
                }
                catch (Exception ex) { MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            }
            dgvLevels.Rows.Remove(dgvLevels.CurrentRow);
            dgvUsers.Rows.Clear();
        }

        // ==================== ✅ الحفظ - نسخة مبسطة وموثوقة ====================
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!_isEditing)
            {
                MessageBox.Show("⚠️ اضغط جديد أو تعديل أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string currentUser = $"{UserManager.GetCurrentUser()?.Id ?? 0} - {UserManager.GetCurrentUser()?.FullName ?? ""}";
            string device = Environment.MachineName;
            string now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            // ✅ جمع المستخدمين من الجدول مرة واحدة فقط
            List<LevelUser> currentUsers = GetUsersFromGrid();

            try
            {
                // قاموس لربط الفهرس بالمعرف الجديد
                Dictionary<int, int> rowIndexToNewId = new Dictionary<int, int>();

                // أولاً: حفظ جميع المستويات
                foreach (DataGridViewRow row in dgvLevels.Rows)
                {
                    if (row.IsNewRow) continue;

                    string sNo = row.Cells["colLevelNo"].Value?.ToString() ?? "";
                    string sName = row.Cells["colLevelName"].Value?.ToString() ?? "";

                    if (string.IsNullOrWhiteSpace(sNo) && string.IsNullOrWhiteSpace(sName)) continue;

                    int levelNo = 0; int.TryParse(sNo, out levelNo);
                    int order = 0; int.TryParse(row.Cells["colOrder"].Value?.ToString(), out order);

                    if (order == 0) order = levelNo;
                    if (levelNo == 0) levelNo = order;
                    else levelNo = order;

                    string screenName = row.Cells["colScreen"].Value?.ToString() ?? cmbScreen.SelectedItem?.ToString();
                    string tag = _screens.FirstOrDefault(s => s.Name == screenName).Tag;
                    if (string.IsNullOrEmpty(tag)) tag = CurrentScreenTag();

                    if (string.IsNullOrWhiteSpace(sName))
                    {
                        MessageBox.Show("⚠️ اسم المستوى مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    int id = 0; int.TryParse(row.Cells["colLevelId"].Value?.ToString(), out id);

                    int newId;
                    if (id > 0)
                    {
                        DatabaseHelper.ExecuteNonQuery(@"UPDATE ApprovalLevels SET 
                            ScreenTag=@T, LevelNo=@N, LevelName=@Nm, OrderNo=@O,
                            ModifiedBy=@By, ModifiedDate=@Dt, ModifiedDevice=@Dev, 
                            ModifyCount=ModifyCount+1 WHERE Id=@Id",
                            DatabaseHelper.CreateParameter("@T", tag),
                            DatabaseHelper.CreateParameter("@N", levelNo),
                            DatabaseHelper.CreateParameter("@Nm", sName),
                            DatabaseHelper.CreateParameter("@O", order),
                            DatabaseHelper.CreateParameter("@By", currentUser),
                            DatabaseHelper.CreateParameter("@Dt", now),
                            DatabaseHelper.CreateParameter("@Dev", device),
                            DatabaseHelper.CreateParameter("@Id", id));
                        newId = id;
                    }
                    else
                    {
                        newId = Convert.ToInt32(DatabaseHelper.ExecuteScalar(@"INSERT INTO ApprovalLevels 
                            (ScreenTag, LevelNo, LevelName, OrderNo, EnteredBy, EnteredDate, EnteredDevice, 
                             PrintCount, ModifyCount)
                            VALUES (@T,@N,@Nm,@O,@By,@Dt,@Dev,0,0); 
                            SELECT last_insert_rowid();",
                            DatabaseHelper.CreateParameter("@T", tag),
                            DatabaseHelper.CreateParameter("@N", levelNo),
                            DatabaseHelper.CreateParameter("@Nm", sName),
                            DatabaseHelper.CreateParameter("@O", order),
                            DatabaseHelper.CreateParameter("@By", currentUser),
                            DatabaseHelper.CreateParameter("@Dt", now),
                            DatabaseHelper.CreateParameter("@Dev", device)));
                    }

                    // تحديث معرف الصف
                    row.Cells["colLevelId"].Value = newId;
                    rowIndexToNewId[row.Index] = newId;
                }

                // ثانياً: حفظ المستخدمين
                // ✅ في هذا النظام المبسط، نفترض أن جميع المستخدمين ينتمون إلى المستوى المحدد
                int targetLevelId = 0;

                if (dgvLevels.CurrentRow != null)
                {
                    int.TryParse(dgvLevels.CurrentRow.Cells["colLevelId"].Value?.ToString(), out targetLevelId);
                }

                if (targetLevelId == 0 && rowIndexToNewId.Count > 0)
                {
                    // استخدام أول معرف جديد
                    targetLevelId = rowIndexToNewId.Values.First();
                }

                if (targetLevelId > 0)
                {
                    // حذف المستخدمين القدامى
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM ApprovalLevelUsers WHERE LevelId = @L",
                        DatabaseHelper.CreateParameter("@L", targetLevelId));

                    // إضافة المستخدمين الجدد
                    foreach (var lu in currentUsers)
                    {
                        if (!string.IsNullOrWhiteSpace(lu.UserNo))
                        {
                            DatabaseHelper.ExecuteNonQuery(
                                "INSERT INTO ApprovalLevelUsers (LevelId, UserNo, UserName) VALUES (@L,@No,@Nm)",
                                DatabaseHelper.CreateParameter("@L", targetLevelId),
                                DatabaseHelper.CreateParameter("@No", lu.UserNo),
                                DatabaseHelper.CreateParameter("@Nm", lu.UserName));
                        }
                    }
                }

                MessageBox.Show("✅ تم الحفظ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

                SetEditing(false);
                LoadScreensWithLevels();
                LoadLevelsForScreen();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الحفظ: {ex.Message}\n\n{ex.StackTrace}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Navigate(int direction)
        {
            if (_screensWithLevels.Count == 0) { MessageBox.Show("⚠️ لا توجد مستويات محفوظة بعد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string cur = CurrentScreenTag();
            _navIndex = _screensWithLevels.IndexOf(cur);
            if (direction == 0) _navIndex = 0;
            else if (direction == 99) _navIndex = _screensWithLevels.Count - 1;
            else _navIndex = Math.Max(0, Math.Min(_screensWithLevels.Count - 1, _navIndex + direction));
            string tag = _screensWithLevels[_navIndex];
            cmbScreen.SelectedItem = GetScreenName(tag);
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            using (Form inputForm = new Form())
            {
                inputForm.Text = "بحث عن مستوى";
                inputForm.Size = new Size(400, 180);
                inputForm.StartPosition = FormStartPosition.CenterParent;
                inputForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                inputForm.MaximizeBox = false;
                inputForm.MinimizeBox = false;
                inputForm.RightToLeft = RightToLeft.Yes;
                inputForm.RightToLeftLayout = true;

                Label lblPrompt = new Label { Text = "أدخل اسم المستوى أو رقمه للبحث:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(20, 20), Size = new Size(340, 30), TextAlign = ContentAlignment.MiddleRight };
                TextBox txtInput = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(20, 60), Size = new Size(340, 27), TextAlign = HorizontalAlignment.Right };
                Button btnOK = new Button { Text = "✅ بحث", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(210, 100), Size = new Size(150, 40), DialogResult = DialogResult.OK };
                Button btnCancel = new Button { Text = "❌ إلغاء", Font = new Font("Tahoma", 10, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(30, 100), Size = new Size(150, 40), DialogResult = DialogResult.Cancel };

                inputForm.Controls.Add(lblPrompt);
                inputForm.Controls.Add(txtInput);
                inputForm.Controls.Add(btnOK);
                inputForm.Controls.Add(btnCancel);
                inputForm.AcceptButton = btnOK;
                inputForm.CancelButton = btnCancel;

                if (inputForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtInput.Text))
                {
                    string term = txtInput.Text.Trim();
                    foreach (DataGridViewRow row in dgvLevels.Rows)
                    {
                        string no = row.Cells["colLevelNo"].Value?.ToString() ?? "";
                        string nm = row.Cells["colLevelName"].Value?.ToString() ?? "";
                        if (no == term || nm.Contains(term)) { row.Selected = true; dgvLevels.FirstDisplayedScrollingRowIndex = row.Index; return; }
                    }
                    MessageBox.Show("❌ لم يتم العثور على نتائج", "بحث", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            using (PrintDialog pd = new PrintDialog())
            {
                PrintDocument doc = new PrintDocument();
                doc.PrintPage += (s, ev) =>
                {
                    Graphics g = ev.Graphics;
                    Font fTitle = new Font("Tahoma", 14, FontStyle.Bold);
                    Font fHead = new Font("Tahoma", 10, FontStyle.Bold);
                    Font fBody = new Font("Tahoma", 10);
                    float yPos = 60;

                    g.DrawString("مستويات الإعتماد - شاشة: " + (cmbScreen.SelectedItem?.ToString() ?? ""), fTitle, Brushes.Black, 60, yPos);
                    yPos += 40;
                    g.DrawString("رقم المستوى\tاسم المستوى\tالترتيب\tالشاشة", fHead, Brushes.Black, 60, yPos);
                    yPos += 25;
                    foreach (DataGridViewRow r in dgvLevels.Rows)
                    {
                        if (r.IsNewRow) continue;
                        g.DrawString($"{r.Cells["colLevelNo"].Value}\t{r.Cells["colLevelName"].Value}\t{r.Cells["colOrder"].Value}\t{r.Cells["colScreen"].Value}", fBody, Brushes.Black, 60, yPos);
                        yPos += 22;
                    }
                };
                pd.Document = doc;
                if (pd.ShowDialog() == DialogResult.OK)
                {
                    doc.Print();
                    foreach (var lv in _levels) DatabaseHelper.ExecuteNonQuery("UPDATE ApprovalLevels SET PrintCount = PrintCount + 1 WHERE Id = @Id", DatabaseHelper.CreateParameter("@Id", lv.Id));
                    LoadLevelsForScreen();
                }
            }
        }

        private void BtnLock_Click(object sender, EventArgs e)
        {
            _locked = !_locked;
            btnLock.Text = _locked ? "🔒" : "🔓";
            if (_locked) SetEditing(false);
            MessageBox.Show(_locked ? "🔒 تم قفل الشاشة - لا يمكن التعديل" : "🔓 تم فتح الشاشة للتعديل", "حالة القفل", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MsgLocked() => MessageBox.Show("🔒 الشاشة مقفلة - اضغط زر القفل لفكها", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        private void UpdateFooter(ApprovalLevel lv)
        {
            txtEnteredBy.Text = lv?.EnteredBy ?? "";
            txtModifiedBy.Text = lv?.ModifiedBy ?? "";
            txtEnteredDate.Text = lv?.EnteredDate ?? "";
            txtModifiedDate.Text = lv?.ModifiedDate ?? "";
            txtEnteredDevice.Text = lv?.EnteredDevice ?? "";
            txtModifiedDevice.Text = lv?.ModifiedDevice ?? "";
            txtPrintCount.Text = lv?.PrintCount.ToString() ?? "";
            txtModifyCount.Text = lv?.ModifyCount.ToString() ?? "";
        }

        private void SetEditing(bool on)
        {
            _isEditing = on;
            dgvLevels.ReadOnly = !on;
            dgvUsers.ReadOnly = !on;
            dgvLevels.AllowUserToAddRows = on;
            dgvUsers.AllowUserToAddRows = on;
            btnSave.Enabled = on;
            btnNew.Enabled = !on && !_locked;
            btnEdit.Enabled = !on && !_locked;
            btnDelete.Enabled = on && !_locked;
        }
    }
}