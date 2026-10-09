using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    // ==================== ✅ كلاس بيانات المجموعة الفرعية ====================
    public class SubGroupData
    {
        public int Id { get; set; }
        public string GroupNo { get; set; }
        public string GroupName { get; set; }
        public string SubGroupNo { get; set; }
        public string SubGroupName { get; set; }
    }

    // ==================== ✅ شاشة المجموعات الفرعية ====================
    public partial class SubGroupsForm : Form
    {
        private ToolStrip toolStrip1;
        private ToolStripButton btnHome, btnNew, btnSave, btnEdit, btnDelete, btnSearch, btnExit;
        private ToolStripButton btnFirst, btnPrevious, btnNext, btnLast;
        private ToolStripSeparator sep1, sep2;

        private GroupBox grpHeader;
        private Label lblGroupNo, lblGroupName, lblSubGroupNo, lblSubGroupName, lblItemsCount;
        private TextBox txtGroupNo, txtGroupName, txtSubGroupNo, txtSubGroupName;

        // ✅ الجدول الآن يعرض الأصناف تحت المجموعة الفرعية
        private DataGridView dgvSubItems;

        private List<SubGroupData> subGroups = new List<SubGroupData>();
        private List<GroupData> groups = new List<GroupData>();
        private bool isEditing = false;
        private int editingId = 0;
        private int navIndex = -1;

        public SubGroupsForm()
        {
            EnsureTableExists();
            BuildUI();
            LoadGroups();
            LoadData();
            SetControlsState(false);

            // ✅ الصفحة تظهر فارغة في البداية
            ClearHeaderFields();
            dgvSubItems.Rows.Clear();
            this.Text = "المجموعات الفرعية";
        }

        // ==================== ✅ إنشاء الجدول إن لم يوجد ====================
        private void EnsureTableExists()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS SubGroups (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    GroupNo TEXT, GroupName TEXT, SubGroupNo TEXT, SubGroupName TEXT,
                    CreatedDate TEXT, ModifiedDate TEXT)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ إنشاء جدول المجموعات الفرعية: {ex.Message}");
            }
        }

        // ==================== ✅ بناء الواجهة ====================
        private void BuildUI()
        {
            this.Text = "المجموعات الفرعية - نظام الجمارك اليمنية";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);

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

            btnEdit = new ToolStripButton("✏️ تعديل"); btnEdit.ForeColor = Color.White;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new ToolStripButton("🗑️ حذف"); btnDelete.ForeColor = Color.White;
            btnDelete.Click += BtnDelete_Click;

            sep1 = new ToolStripSeparator();

            btnFirst = new ToolStripButton("⏮ الأول"); btnFirst.ForeColor = Color.White;
            btnFirst.Click += (s, e) =>
            {
                if (subGroups.Count == 0) { MessageBox.Show("⚠️ لا توجد مجموعات فرعية مضافة بعد\n💡 اضغط 'جديد' ثم 'حفظ' لإضافة أول مجموعة فرعية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                NavigateTo(0);
            };

            btnPrevious = new ToolStripButton("◀ السابق"); btnPrevious.ForeColor = Color.White;
            btnPrevious.Click += (s, e) =>
            {
                if (subGroups.Count == 0) { MessageBox.Show("⚠️ لا توجد مجموعات فرعية مضافة بعد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                if (navIndex > 0) NavigateTo(navIndex - 1);
                else MessageBox.Show("⚠️ أنت في أول سجل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnNext = new ToolStripButton("التالي ▶"); btnNext.ForeColor = Color.White;
            btnNext.Click += (s, e) =>
            {
                if (subGroups.Count == 0) { MessageBox.Show("⚠️ لا توجد مجموعات فرعية مضافة بعد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                if (navIndex < subGroups.Count - 1) NavigateTo(navIndex + 1);
                else MessageBox.Show("⚠️ أنت في آخر سجل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnLast = new ToolStripButton("الأخير ⏭"); btnLast.ForeColor = Color.White;
            btnLast.Click += (s, e) =>
            {
                if (subGroups.Count == 0) { MessageBox.Show("⚠️ لا توجد مجموعات فرعية مضافة بعد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
                NavigateTo(subGroups.Count - 1);
            };

            sep2 = new ToolStripSeparator();

            btnSearch = new ToolStripButton("عرض "); btnSearch.ForeColor = Color.White;
            btnSearch.Click += BtnSearch_Click;

            btnExit = new ToolStripButton("🚪 خروج"); btnExit.ForeColor = Color.White;
            btnExit.Click += (s, e) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] {
                btnHome, btnNew, btnSave, btnEdit, btnDelete, sep1,
                btnFirst, btnPrevious, btnNext, btnLast, sep2,
                btnSearch, btnExit
            });

            // ===== مجموعة البيانات =====
            grpHeader = new GroupBox();
            grpHeader.Text = "📁 بيانات المجموعة الفرعية";
            grpHeader.Dock = DockStyle.Top;
            grpHeader.Height = 130;
            grpHeader.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpHeader.ForeColor = Color.FromArgb(60, 60, 60);
            grpHeader.BackColor = Color.FromArgb(250, 250, 250);

            lblGroupNo = new Label();
            lblGroupNo.Text = "رقم المجموعه الرئيسية:";
            lblGroupNo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGroupNo.ForeColor = Color.FromArgb(51, 122, 183);
            lblGroupNo.Location = new Point(1140, 30);
            lblGroupNo.Size = new Size(120, 20);

            txtGroupNo = new TextBox();
            txtGroupNo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtGroupNo.BackColor = Color.FromArgb(255, 255, 190);
            txtGroupNo.Location = new Point(960, 27);
            txtGroupNo.Size = new Size(180, 23);
            txtGroupNo.TextAlign = HorizontalAlignment.Right;
            txtGroupNo.TextChanged += TxtGroupNo_TextChanged;
            txtGroupNo.KeyDown += TxtGroupNo_KeyDown;

            lblGroupName = new Label();
            lblGroupName.Text = "اسم المجموعه الرئيسية:";
            lblGroupName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblGroupName.ForeColor = Color.FromArgb(51, 122, 183);
            lblGroupName.Location = new Point(750, 30);
            lblGroupName.Size = new Size(140, 20);

            txtGroupName = new TextBox();
            txtGroupName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtGroupName.ReadOnly = true;
            txtGroupName.Location = new Point(560, 27);
            txtGroupName.Size = new Size(180, 23);
            txtGroupName.TextAlign = HorizontalAlignment.Right;

            lblSubGroupNo = new Label();
            lblSubGroupNo.Text = "رقم المجموعة الفرعية:";
            lblSubGroupNo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSubGroupNo.ForeColor = Color.FromArgb(51, 122, 183);
            lblSubGroupNo.Location = new Point(1120, 70);
            lblSubGroupNo.Size = new Size(140, 20);

            txtSubGroupNo = new TextBox();
            txtSubGroupNo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtSubGroupNo.BackColor = Color.FromArgb(255, 255, 190);
            txtSubGroupNo.Location = new Point(960, 67);
            txtSubGroupNo.Size = new Size(150, 27);
            txtSubGroupNo.TextAlign = HorizontalAlignment.Right;
            txtSubGroupNo.KeyDown += TxtSubGroupNo_KeyDown;

            lblSubGroupName = new Label();
            lblSubGroupName.Text = "اسم المجموعة الفرعية:";
            lblSubGroupName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblSubGroupName.ForeColor = Color.FromArgb(51, 122, 183);
            lblSubGroupName.Location = new Point(750, 70);
            lblSubGroupName.Size = new Size(140, 20);

            txtSubGroupName = new TextBox();
            txtSubGroupName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            txtSubGroupName.BackColor = Color.FromArgb(255, 255, 190);
            txtSubGroupName.Location = new Point(560, 67);
            txtSubGroupName.Size = new Size(180, 23);
            txtSubGroupName.TextAlign = HorizontalAlignment.Right;

            // ✅ عدّاد الأصناف
            lblItemsCount = new Label();
            lblItemsCount.Text = "📦 الأصناف تحت هذه المجموعة: 0";
            lblItemsCount.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblItemsCount.ForeColor = Color.FromArgb(33, 150, 243);
            lblItemsCount.Location = new Point(20, 40);
            lblItemsCount.Size = new Size(300, 30);
            lblItemsCount.TextAlign = ContentAlignment.MiddleRight;

            grpHeader.Controls.Add(lblGroupNo);
            grpHeader.Controls.Add(txtGroupNo);
            grpHeader.Controls.Add(lblGroupName);
            grpHeader.Controls.Add(txtGroupName);
            grpHeader.Controls.Add(lblSubGroupNo);
            grpHeader.Controls.Add(txtSubGroupNo);
            grpHeader.Controls.Add(lblSubGroupName);
            grpHeader.Controls.Add(txtSubGroupName);
            grpHeader.Controls.Add(lblItemsCount);

            // ===== ✅ الجدول: يعرض الأصناف تحت المجموعة الفرعية =====
            dgvSubItems = new DataGridView();
            dgvSubItems.Dock = DockStyle.Fill;
            dgvSubItems.AllowUserToAddRows = false;
            dgvSubItems.AllowUserToDeleteRows = false;
            dgvSubItems.ReadOnly = true;
            dgvSubItems.RowHeadersVisible = false;
            dgvSubItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSubItems.MultiSelect = false;
            dgvSubItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSubItems.BackgroundColor = Color.White;
            dgvSubItems.BorderStyle = BorderStyle.Fixed3D;
            dgvSubItems.RightToLeft = RightToLeft.Yes;
            dgvSubItems.EnableHeadersVisualStyles = false;
            dgvSubItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvSubItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSubItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvSubItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSubItems.ColumnHeadersHeight = 40;
            dgvSubItems.RowTemplate.Height = 32;
            dgvSubItems.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            dgvSubItems.Columns.Add("colNum", "م");
            dgvSubItems.Columns.Add("colItemCode", "رقم الصنف");
            dgvSubItems.Columns.Add("colItemName", "اسم الصنف");
            dgvSubItems.Columns["colNum"].Width = 50;

            this.Controls.Add(dgvSubItems);
            this.Controls.Add(grpHeader);
            this.Controls.Add(toolStrip1);
        }

        // ==================== ✅ تحميل المجموعات الرئيسية ====================
        private void LoadGroups()
        {
            groups.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT GroupNo, GroupName FROM Groups WHERE IsActive = 1 ORDER BY GroupNo");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        groups.Add(new GroupData
                        {
                            GroupNo = row["GroupNo"].ToString(),
                            GroupName = row["GroupName"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ تحميل المجموعات: {ex.Message}"); }
        }

        // ==================== ✅ تحميل المجموعات الفرعية (للذاكرة فقط) ====================
        private void LoadData()
        {
            subGroups.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT rowid AS RowId, * FROM SubGroups ORDER BY GroupNo, SubGroupNo");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        try
                        {
                            int id = dt.Columns.Contains("Id") ? Convert.ToInt32(row["Id"]) : Convert.ToInt32(row["RowId"]);
                            subGroups.Add(new SubGroupData
                            {
                                Id = id,
                                GroupNo = row["GroupNo"]?.ToString() ?? "",
                                GroupName = row["GroupName"]?.ToString() ?? "",
                                SubGroupNo = row["SubGroupNo"]?.ToString() ?? "",
                                SubGroupName = row["SubGroupName"]?.ToString() ?? ""
                            });
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ تحميل المجموعات الفرعية: {ex.Message}"); }
        }

        // ==================== ✅ تحميل الأصناف تحت المجموعة الفرعية المختارة ====================
        private void LoadItemsForSubGroup(string groupNo, string subGroupNo)
        {
            dgvSubItems.Rows.Clear();
            if (string.IsNullOrEmpty(groupNo) || string.IsNullOrEmpty(subGroupNo))
            {
                lblItemsCount.Text = "📦 الأصناف تحت هذه المجموعة: 0";
                return;
            }

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT ItemCode, ItemName FROM Items WHERE GroupNo = @GroupNo AND SubGroupUnder = @SubGroupNo ORDER BY ItemCode",
                    DatabaseHelper.CreateParameter("@GroupNo", groupNo),
                    DatabaseHelper.CreateParameter("@SubGroupNo", subGroupNo));

                if (dt != null)
                {
                    int n = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgvSubItems.Rows.Add(n++, row["ItemCode"].ToString(), row["ItemName"].ToString());
                    }
                    lblItemsCount.Text = $"📦 الأصناف تحت هذه المجموعة: {dt.Rows.Count}";
                }
                else
                {
                    lblItemsCount.Text = "📦 الأصناف تحت هذه المجموعة: 0";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ تحميل الأصناف: {ex.Message}");
                lblItemsCount.Text = "📦 الأصناف تحت هذه المجموعة: 0";
            }
        }

        // ==================== ✅ التنقل ====================
        private void NavigateTo(int index)
        {
            if (index < 0 || index >= subGroups.Count) return;
            navIndex = index;
            var item = subGroups[index];

            editingId = item.Id;
            txtGroupNo.Text = item.GroupNo;
            txtGroupName.Text = item.GroupName;
            txtGroupName.BackColor = Color.LightGreen;
            txtSubGroupNo.Text = item.SubGroupNo;
            txtSubGroupName.Text = item.SubGroupName;

            isEditing = false;
            SetControlsState(false);

            // ✅ عرض الأصناف تحت هذه المجموعة الفرعية
            LoadItemsForSubGroup(item.GroupNo, item.SubGroupNo);
            UpdateNavStatus();
        }

        private void UpdateNavStatus()
        {
            this.Text = $"المجموعات الفرعية - {navIndex + 1}/{subGroups.Count}";
        }

        // ==================== ✅ أحداث الحقول ====================
        private void TxtGroupNo_TextChanged(object sender, EventArgs e)
        {
            string groupNo = txtGroupNo.Text.Trim();
            if (string.IsNullOrEmpty(groupNo))
            {
                txtGroupName.Clear();
                txtGroupName.BackColor = Color.FromArgb(255, 255, 190);
                return;
            }

            var group = groups.FirstOrDefault(g => g.GroupNo == groupNo);
            if (group != null)
            {
                txtGroupName.Text = group.GroupName;
                txtGroupName.BackColor = Color.LightGreen;
                if (isEditing && editingId == 0)
                    txtSubGroupNo.Text = GenerateSubGroupNo(groupNo);
            }
            else
            {
                txtGroupName.Text = "";
                txtGroupName.BackColor = Color.FromArgb(255, 255, 190);
            }
        }

        private void TxtGroupNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                OpenGroupSelectionForm();
            }
        }

        private void TxtSubGroupNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                e.SuppressKeyPress = true;
                OpenSubGroupSelectionForm();
            }
        }

        private string GenerateSubGroupNo(string groupNo)
        {
            int max = 0;
            foreach (var s in subGroups.Where(x => x.GroupNo == groupNo))
            {
                int n;
                if (int.TryParse(s.SubGroupNo, out n) && n > max) max = n;
            }
            return (max + 1).ToString();
        }

        // ==================== ✅ نافذة اختيار المجموعة الرئيسية ====================
        private void OpenGroupSelectionForm()
        {
            if (groups.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد مجموعات مضافة\nالرجاء إضافة مجموعات من شاشة المجموعة الرئيسية للأصناف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form selectForm = new Form();
            selectForm.Text = $"🔍 اختر المجموعة ({groups.Count} مجموعة)";
            selectForm.Size = new Size(600, 450);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            DataGridView dgvGroups = new DataGridView();
            dgvGroups.Dock = DockStyle.Fill;
            dgvGroups.ReadOnly = true;
            dgvGroups.AllowUserToAddRows = false;
            dgvGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGroups.MultiSelect = false;
            dgvGroups.RowHeadersVisible = false;
            dgvGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGroups.BackgroundColor = Color.White;
            dgvGroups.EnableHeadersVisualStyles = false;
            dgvGroups.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvGroups.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGroups.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvGroups.ColumnHeadersHeight = 40;
            dgvGroups.RowTemplate.Height = 35;
            dgvGroups.Columns.Add("colGroupNo", "رقم المجموعه الرئيسية");
            dgvGroups.Columns.Add("colGroupName", "اسم المجموعه الرئيسية");

            foreach (var g in groups)
                dgvGroups.Rows.Add(g.GroupNo, g.GroupName);

            dgvGroups.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    txtGroupNo.Text = dgvGroups.Rows[ev.RowIndex].Cells["colGroupNo"].Value?.ToString();
                    selectForm.Close();
                }
            };

            Button btnClose = new Button();
            btnClose.Text = "❌ إغلاق";
            btnClose.Dock = DockStyle.Bottom;
            btnClose.Height = 45;
            btnClose.BackColor = Color.FromArgb(158, 158, 158);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += (s, ev) => selectForm.Close();

            selectForm.Controls.Add(dgvGroups);
            selectForm.Controls.Add(btnClose);
            selectForm.ShowDialog();
        }

        // ==================== ✅ نافذة اختيار مجموعة فرعية (F9) ====================
        private void OpenSubGroupSelectionForm()
        {
            if (subGroups.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد مجموعات فرعية مضافة بعد\n💡 اضغط 'جديد' ثم 'حفظ' لإضافة أول مجموعة فرعية", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Form selectForm = new Form();
            selectForm.Text = $"🔍 اختر المجموعة الفرعية ({subGroups.Count} مجموعة)";
            selectForm.Size = new Size(800, 500);
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
            lblSearch.Location = new Point(650, 15);
            lblSearch.Size = new Size(60, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            TextBox txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(350, 15);
            txtSearch.Size = new Size(280, 25);

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);

            DataGridView dgvSelect = new DataGridView();
            dgvSelect.Dock = DockStyle.Fill;
            dgvSelect.ReadOnly = true;
            dgvSelect.AllowUserToAddRows = false;
            dgvSelect.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSelect.MultiSelect = false;
            dgvSelect.RowHeadersVisible = false;
            dgvSelect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSelect.BackgroundColor = Color.White;
            dgvSelect.EnableHeadersVisualStyles = false;
            dgvSelect.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvSelect.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSelect.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvSelect.ColumnHeadersHeight = 40;
            dgvSelect.RowTemplate.Height = 35;

            dgvSelect.Columns.Add("colSubNo", "رقم الفرعية");
            dgvSelect.Columns.Add("colSubName", "اسم المجموعة الفرعية");
            dgvSelect.Columns.Add("colGroupNo", " رقم المجموعة الرئيسية");
            dgvSelect.Columns.Add("colGroupName", "المجموعة الرئيسية");

            foreach (var sg in subGroups)
                dgvSelect.Rows.Add(sg.SubGroupNo, sg.SubGroupName, sg.GroupNo, sg.GroupName);

            txtSearch.TextChanged += (s, ev) =>
            {
                string term = txtSearch.Text.Trim().ToLower();
                foreach (DataGridViewRow rRow in dgvSelect.Rows)
                {
                    string subNo = rRow.Cells["colSubNo"].Value?.ToString()?.ToLower() ?? "";
                    string subName = rRow.Cells["colSubName"].Value?.ToString()?.ToLower() ?? "";
                    string gNo = rRow.Cells["colGroupNo"].Value?.ToString()?.ToLower() ?? "";
                    string gName = rRow.Cells["colGroupName"].Value?.ToString()?.ToLower() ?? "";
                    rRow.Visible = string.IsNullOrEmpty(term) || subNo.Contains(term) || subName.Contains(term) || gNo.Contains(term) || gName.Contains(term);
                }
            };

            Action ApplySelection = () =>
            {
                if (dgvSelect.SelectedRows.Count == 0) return;
                var rRow = dgvSelect.SelectedRows[0];
                string gNo = rRow.Cells["colGroupNo"].Value?.ToString();
                string sNo = rRow.Cells["colSubNo"].Value?.ToString();

                var item = subGroups.FirstOrDefault(x => x.GroupNo == gNo && x.SubGroupNo == sNo);
                if (item != null)
                {
                    txtGroupNo.Text = item.GroupNo;
                    txtGroupName.Text = item.GroupName;
                    txtGroupName.BackColor = Color.LightGreen;
                    txtSubGroupNo.Text = item.SubGroupNo;
                    txtSubGroupName.Text = item.SubGroupName;

                    editingId = item.Id;
                    navIndex = subGroups.IndexOf(item);

                    LoadItemsForSubGroup(item.GroupNo, item.SubGroupNo);
                    UpdateNavStatus();
                }
                selectForm.Close();
            };

            dgvSelect.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) ApplySelection(); };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 55;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnOK = new Button();
            btnOK.Text = "✅ اختيار";
            btnOK.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnOK.BackColor = Color.FromArgb(76, 175, 80);
            btnOK.ForeColor = Color.White;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Size = new Size(150, 40);
            btnOK.Location = new Point(560, 8);
            btnOK.Click += (s, ev) => ApplySelection();

            Button btnCancel = new Button();
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 40);
            btnCancel.Location = new Point(390, 8);
            btnCancel.Click += (s, ev) => selectForm.Close();

            pnlButtons.Controls.Add(btnOK);
            pnlButtons.Controls.Add(btnCancel);

            selectForm.Controls.Add(dgvSelect);
            selectForm.Controls.Add(pnlSearch);
            selectForm.Controls.Add(pnlButtons);
            selectForm.AcceptButton = btnOK;
            selectForm.CancelButton = btnCancel;
            selectForm.ShowDialog();
        }

        // ==================== ✅ الأزرار ====================
        private void BtnNew_Click(object sender, EventArgs e)
        {
            editingId = 0;
            navIndex = -1;
            ClearHeaderFields();
            dgvSubItems.Rows.Clear();
            lblItemsCount.Text = "📦 الأصناف تحت هذه المجموعة: 0";
            isEditing = true;
            SetControlsState(true);
            txtGroupNo.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (navIndex < 0 || editingId == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار سجل للتعديل أولاً\n💡 استخدم أزرار التنقل أو F9", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            isEditing = true;
            SetControlsState(true);
            txtSubGroupName.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!isEditing)
            {
                MessageBox.Show("⚠️ اضغط 'جديد' أو 'تعديل' أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string groupNo = txtGroupNo.Text.Trim();
            string groupName = txtGroupName.Text.Trim();
            string subNo = txtSubGroupNo.Text.Trim();
            string subName = txtSubGroupName.Text.Trim();

            if (string.IsNullOrEmpty(groupNo)) { MessageBox.Show("⚠️ رقم المجموعة مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtGroupNo.Focus(); return; }
            if (string.IsNullOrEmpty(groupName)) { MessageBox.Show("⚠️ اسم المجموعة غير موجود - تحقق من رقم المجموعة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtGroupNo.Focus(); return; }
            if (string.IsNullOrEmpty(subName)) { MessageBox.Show("⚠️ اسم المجموعة الفرعية مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtSubGroupName.Focus(); return; }
            if (string.IsNullOrEmpty(subNo)) subNo = GenerateSubGroupNo(groupNo);

            var dup = subGroups.FirstOrDefault(s => s.GroupNo == groupNo && s.SubGroupNo == subNo && s.Id != editingId);
            if (dup != null)
            {
                MessageBox.Show($"⚠️ المجموعة الفرعية رقم {subNo} موجودة مسبقاً تحت المجموعة {groupNo}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSubGroupNo.Focus();
                return;
            }

            try
            {
                if (editingId > 0)
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "UPDATE SubGroups SET GroupNo=@G, GroupName=@GN, SubGroupNo=@S, SubGroupName=@SN, ModifiedDate=datetime('now') WHERE Id=@Id",
                        DatabaseHelper.CreateParameter("@G", groupNo),
                        DatabaseHelper.CreateParameter("@GN", groupName),
                        DatabaseHelper.CreateParameter("@S", subNo),
                        DatabaseHelper.CreateParameter("@SN", subName),
                        DatabaseHelper.CreateParameter("@Id", editingId));
                }
                else
                {
                    var newId = DatabaseHelper.ExecuteScalar(
                        "INSERT INTO SubGroups (GroupNo, GroupName, SubGroupNo, SubGroupName, CreatedDate, ModifiedDate) VALUES (@G, @GN, @S, @SN, datetime('now'), datetime('now')); SELECT last_insert_rowid();",
                        DatabaseHelper.CreateParameter("@G", groupNo),
                        DatabaseHelper.CreateParameter("@GN", groupName),
                        DatabaseHelper.CreateParameter("@S", subNo),
                        DatabaseHelper.CreateParameter("@SN", subName));
                    editingId = Convert.ToInt32(newId);
                }

                LoadData();
                isEditing = false;
                SetControlsState(false);

                int idx = subGroups.FindIndex(x => x.Id == editingId);
                if (idx >= 0) NavigateTo(idx);

                MessageBox.Show("✅ تم الحفظ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الحفظ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (navIndex < 0 || editingId == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار مجموعة فرعية للحذف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var item = subGroups.FirstOrDefault(x => x.Id == editingId);
            if (item == null) return;

            if (MessageBox.Show($"🗑️ هل تريد حذف المجموعة الفرعية:\n{item.SubGroupNo} - {item.SubGroupName}؟",
                "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM SubGroups WHERE Id=@Id",
                        DatabaseHelper.CreateParameter("@Id", item.Id));
                    LoadData();
                    if (subGroups.Count > 0)
                    {
                        if (navIndex >= subGroups.Count) navIndex = subGroups.Count - 1;
                        NavigateTo(navIndex);
                    }
                    else
                    {
                        navIndex = -1;
                        editingId = 0;
                        ClearHeaderFields();
                        dgvSubItems.Rows.Clear();
                        lblItemsCount.Text = "📦 الأصناف تحت هذه المجموعة: 0";
                        this.Text = "المجموعات الفرعية";
                    }
                    MessageBox.Show("✅ تم الحذف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (subGroups.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد مجموعات فرعية مضافة بعد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            OpenSubGroupSelectionForm();
        }

        private void ClearHeaderFields()
        {
            txtGroupNo.Clear();
            txtGroupName.Clear();
            txtGroupName.BackColor = Color.FromArgb(255, 255, 190);
            txtSubGroupNo.Clear();
            txtSubGroupName.Clear();
        }

        private void SetControlsState(bool enabled)
        {
            txtGroupNo.ReadOnly = !enabled;
            txtSubGroupNo.ReadOnly = !enabled;
            txtSubGroupName.ReadOnly = !enabled;
            txtGroupName.ReadOnly = true;

            Color bg = enabled ? Color.FromArgb(255, 255, 224) : Color.White;
            txtGroupNo.BackColor = enabled ? bg : Color.White;
            txtSubGroupNo.BackColor = enabled ? bg : Color.White;
            txtSubGroupName.BackColor = enabled ? bg : Color.White;
        }
    }
}