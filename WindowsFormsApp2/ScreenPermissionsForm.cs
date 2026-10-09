using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class ScreenPermissionsForm : Form
    {
        // ===== الأعلى =====
        private ComboBox cmbUserNumber;
        private Label lblUserInfo;
        private Button btnAddUser;
        private Button btnSave;
        private DataGridView dgvUserScreens;

        // ===== الأسفل (التبويبات) =====
        private TabControl tabBottom;
        private DataGridView dgvScreenUsers;
        private DataGridView dgvGranted;
        private DataGridView dgvDenied;

        // ===== البيانات =====
        private List<ScreenPermission> allScreens;
        private List<UserManager.User> allUsers;
        private List<ScreenPermission> currentUserPermissions = new List<ScreenPermission>();
        private int currentUserId = 0;
        private bool loading = false;

        private static readonly string[] PermKeys = { "Include", "Print", "ViewReport", "AddFrom", "Add", "Edit", "Delete", "View", "ViewList", "SendTo", "Review", "Post", "CancelPost" };
        private static readonly string[] PermHeaders = { "تضمين", "طباعة", "عرض تقرير", "إضافة من", "إضافة", "تعديل", "حذف", "عرض", "عرض القائمة", "ارسال الى", "مراجعة", "ترحيل", "الغاء الترحيل" };

        public ScreenPermissionsForm()
        {
            BuildUI();
            LoadData();
        }

        // ==================== بناء الواجهة ====================
        private void BuildUI()
        {
            this.Text = "إدارة صلاحيات الشاشات - نظام الجمارك اليمنية";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;

            // ===== الشريط العلوي (تخطيط تلقائي يضمن ظهور كل العناصر) =====
            Panel pnlTop = new Panel();
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Height = 75;
            pnlTop.BackColor = Color.FromArgb(240, 248, 255);

            FlowLayoutPanel flow = new FlowLayoutPanel();
            flow.Dock = DockStyle.Fill;
            flow.RightToLeft = RightToLeft.Yes;
            flow.FlowDirection = FlowDirection.RightToLeft;
            flow.Padding = new Padding(15);

            Label lblUserNo = new Label();
            lblUserNo.Text = "اختر المستخدم:";
            lblUserNo.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblUserNo.ForeColor = Color.FromArgb(33, 150, 243);
            lblUserNo.Size = new Size(115, 40);
            lblUserNo.TextAlign = ContentAlignment.MiddleCenter;

            cmbUserNumber = new ComboBox();
            cmbUserNumber.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUserNumber.Font = new Font("Tahoma", 11, FontStyle.Bold);
            cmbUserNumber.Size = new Size(300, 40);
            cmbUserNumber.SelectedIndexChanged += CmbUserNumber_SelectedIndexChanged;

            lblUserInfo = new Label();
            lblUserInfo.Text = "";
            lblUserInfo.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblUserInfo.ForeColor = Color.FromArgb(44, 62, 80);
            lblUserInfo.Size = new Size(250, 40);
            lblUserInfo.TextAlign = ContentAlignment.MiddleCenter;

            btnAddUser = new Button();
            btnAddUser.Text = "➕ إضافة مستخدم جديد";
            btnAddUser.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnAddUser.BackColor = Color.FromArgb(156, 39, 176);
            btnAddUser.ForeColor = Color.White;
            btnAddUser.FlatStyle = FlatStyle.Flat;
            btnAddUser.Size = new Size(180, 42);
            btnAddUser.Click += BtnAddUser_Click;

            btnSave = new Button();
            btnSave.Text = "💾 حفظ الصلاحيات";
            btnSave.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(76, 175, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Size = new Size(160, 42);
            btnSave.Click += BtnSave_Click;

            flow.Controls.Add(lblUserNo);
            flow.Controls.Add(cmbUserNumber);
            flow.Controls.Add(lblUserInfo);
            flow.Controls.Add(btnAddUser);
            flow.Controls.Add(btnSave);

            pnlTop.Controls.Add(flow);

            // ===== الجدول العلوي: صلاحيات المستخدم المختار =====
            dgvUserScreens = new DataGridView();
            dgvUserScreens.Dock = DockStyle.Fill;
            StyleGrid(dgvUserScreens);

            DataGridViewTextBoxColumn colNo = new DataGridViewTextBoxColumn();
            colNo.Name = "colNo"; colNo.HeaderText = "م"; colNo.Width = 40; colNo.ReadOnly = true;
            dgvUserScreens.Columns.Add(colNo);

            DataGridViewTextBoxColumn colScreenName = new DataGridViewTextBoxColumn();
            colScreenName.Name = "colScreenName"; colScreenName.HeaderText = "اسم الشاشة"; colScreenName.Width = 260; colScreenName.ReadOnly = true;
            dgvUserScreens.Columns.Add(colScreenName);

            AddPermissionColumns(dgvUserScreens);
            dgvUserScreens.CellValueChanged += DgvUserScreens_CellValueChanged;
            dgvUserScreens.SelectionChanged += DgvUserScreens_SelectionChanged;

            // ===== التبويب 1: مستخدمين الشاشة =====
            TabPage tabScreenUsers = new TabPage("مستخدمين الشاشة");
            dgvScreenUsers = new DataGridView();
            dgvScreenUsers.Dock = DockStyle.Fill;
            StyleGrid(dgvScreenUsers);

            DataGridViewTextBoxColumn colUserNo = new DataGridViewTextBoxColumn();
            colUserNo.Name = "colUserNo"; colUserNo.HeaderText = "رقم المستخدم"; colUserNo.Width = 90; colUserNo.ReadOnly = true;
            dgvScreenUsers.Columns.Add(colUserNo);

            DataGridViewTextBoxColumn colUserName = new DataGridViewTextBoxColumn();
            colUserName.Name = "colUserName"; colUserName.HeaderText = "اسم المستخدم"; colUserName.Width = 220; colUserName.ReadOnly = true;
            dgvScreenUsers.Columns.Add(colUserName);

            AddPermissionColumns(dgvScreenUsers);
            dgvScreenUsers.CellValueChanged += DgvScreenUsers_CellValueChanged;
            tabScreenUsers.Controls.Add(dgvScreenUsers);

            // ===== التبويب 2: شاشات ممنوحة للمستخدم =====
            TabPage tabGranted = new TabPage("شاشات ممنوحة للمستخدم");
            dgvGranted = new DataGridView();
            dgvGranted.Dock = DockStyle.Fill;
            StyleGrid(dgvGranted);
            dgvGranted.Columns.Add("colNoG", "م");
            dgvGranted.Columns.Add("colScreenNameG", "اسم الشاشة");
            dgvGranted.Columns["colNoG"].Width = 50;
            dgvGranted.Columns["colScreenNameG"].Width = 400;
            tabGranted.Controls.Add(dgvGranted);

            // ===== التبويب 3: شاشات غير ممنوحة للمستخدم =====
            TabPage tabDenied = new TabPage("شاشات غير ممنوحة للمستخدم");
            dgvDenied = new DataGridView();
            dgvDenied.Dock = DockStyle.Fill;
            StyleGrid(dgvDenied);
            dgvDenied.Columns.Add("colNoD", "م");
            dgvDenied.Columns.Add("colScreenNameD", "اسم الشاشة");
            dgvDenied.Columns["colNoD"].Width = 50;
            dgvDenied.Columns["colScreenNameD"].Width = 400;
            tabDenied.Controls.Add(dgvDenied);

            tabBottom = new TabControl();
            tabBottom.Dock = DockStyle.Fill;
            tabBottom.Font = new Font("Tahoma", 10, FontStyle.Bold);
            tabBottom.RightToLeft = RightToLeft.Yes;
            tabBottom.TabPages.Add(tabScreenUsers);
            tabBottom.TabPages.Add(tabGranted);
            tabBottom.TabPages.Add(tabDenied);

            // ===== التقسيم =====
            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.Panel1.Controls.Add(dgvUserScreens);
            split.Panel2.Controls.Add(tabBottom);

            this.Controls.Add(split);
            this.Controls.Add(pnlTop);

            this.Load += (s, e) =>
            {
                try { split.SplitterDistance = this.ClientSize.Height / 2; } catch { }
            };
        }

        private void StyleGrid(DataGridView dgv)
        {
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.RowHeadersVisible = false;
            dgv.BackgroundColor = Color.White;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 45;
            dgv.RowTemplate.Height = 32;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void AddPermissionColumns(DataGridView dgv)
        {
            for (int i = 0; i < PermKeys.Length; i++)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                chk.Name = "colP_" + PermKeys[i];
                chk.HeaderText = PermHeaders[i];
                chk.Width = 70;
                chk.TrueValue = true;
                chk.FalseValue = false;
                chk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns.Add(chk);
            }
        }

        // ==================== تحميل البيانات ====================
        private void LoadData()
        {
            allScreens = UserManager.GetAllSystemScreens();
            RefreshUsersList();
        }

        private void RefreshUsersList()
        {
            loading = true;
            allUsers = UserManager.GetAllUsers();
            cmbUserNumber.Items.Clear();

            foreach (var u in allUsers)
            {
                string roleText = UserManager.GetRoleArabicName(u.Role);
                cmbUserNumber.Items.Add(u.UserNumber + " - " + u.FullName + " (" + roleText + ")");
            }

            if (cmbUserNumber.Items.Count > 0)
            {
                int idx = 0;
                var cur = UserManager.GetCurrentUser();
                if (cur != null)
                {
                    for (int i = 0; i < allUsers.Count; i++)
                    {
                        if (allUsers[i].Id == cur.Id) { idx = i; break; }
                    }
                }
                cmbUserNumber.SelectedIndex = idx;
            }
            loading = false;
        }

        private void CmbUserNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbUserNumber.SelectedIndex < 0 || cmbUserNumber.SelectedIndex >= allUsers.Count) return;

            var user = allUsers[cmbUserNumber.SelectedIndex];
            currentUserId = user.Id;

            string roleText = UserManager.GetRoleArabicName(user.Role);
            lblUserInfo.Text = "الدور: " + roleText;

            UserManager.LoadScreenPermissionsFromDatabase(user.Id);
            currentUserPermissions = UserManager.GetScreenPermissions(user.Id);
            if (currentUserPermissions == null) currentUserPermissions = new List<ScreenPermission>();

            RefreshTopGrid();
            RefreshGrantedDenied();
            RefreshScreenUsersGrid();
        }

        // ==================== تحديث الجدول العلوي ====================
        private void RefreshTopGrid()
        {
            loading = true;
            dgvUserScreens.Rows.Clear();
            int no = 1;
            foreach (var s in allScreens)
            {
                var p = currentUserPermissions.FirstOrDefault(x => x.ScreenTag == s.ScreenTag);
                if (p == null)
                {
                    p = new ScreenPermission();
                    p.ScreenName = s.ScreenName;
                    p.ScreenTag = s.ScreenTag;
                    currentUserPermissions.Add(p);
                }

                int rowIndex = dgvUserScreens.Rows.Add();
                DataGridViewRow row = dgvUserScreens.Rows[rowIndex];
                row.Cells["colNo"].Value = no++;
                row.Cells["colScreenName"].Value = s.ScreenName;
                for (int i = 0; i < PermKeys.Length; i++)
                    row.Cells["colP_" + PermKeys[i]].Value = GetPermValue(p, PermKeys[i]);
            }
            loading = false;
            if (dgvUserScreens.Rows.Count > 0) dgvUserScreens.Rows[0].Selected = true;
        }

        // ==================== تحديث تبويبات الممنوعة/الممنوحة ====================
        private void RefreshGrantedDenied()
        {
            dgvGranted.Rows.Clear();
            dgvDenied.Rows.Clear();
            int g = 1, d = 1;
            foreach (var s in allScreens)
            {
                var p = currentUserPermissions.FirstOrDefault(x => x.ScreenTag == s.ScreenTag);
                if (p != null && p.IsGranted())
                    dgvGranted.Rows.Add(g++, s.ScreenName);
                else
                    dgvDenied.Rows.Add(d++, s.ScreenName);
            }
        }

        // ==================== تحديث تبويب مستخدمين الشاشة ====================
        private void RefreshScreenUsersGrid()
        {
            string screenName = GetSelectedScreenName();
            loading = true;
            dgvScreenUsers.Rows.Clear();

            if (!string.IsNullOrEmpty(screenName))
            {
                var screen = allScreens.FirstOrDefault(s => s.ScreenName == screenName);
                if (screen != null)
                {
                    foreach (var u in allUsers)
                    {
                        UserManager.LoadScreenPermissionsFromDatabase(u.Id);
                        var perms = UserManager.GetScreenPermissions(u.Id);
                        var p = perms.FirstOrDefault(x => x.ScreenTag == screen.ScreenTag);

                        int rowIndex = dgvScreenUsers.Rows.Add();
                        DataGridViewRow row = dgvScreenUsers.Rows[rowIndex];
                        row.Cells["colUserNo"].Value = u.UserNumber;
                        row.Cells["colUserName"].Value = u.FullName;
                        for (int i = 0; i < PermKeys.Length; i++)
                            row.Cells["colP_" + PermKeys[i]].Value = (p != null) && GetPermValue(p, PermKeys[i]);
                    }
                }
            }
            loading = false;
        }

        private string GetSelectedScreenName()
        {
            if (dgvUserScreens.CurrentRow != null && dgvUserScreens.CurrentRow.Cells["colScreenName"].Value != null)
                return dgvUserScreens.CurrentRow.Cells["colScreenName"].Value.ToString();
            if (dgvUserScreens.Rows.Count > 0 && dgvUserScreens.Rows[0].Cells["colScreenName"].Value != null)
                return dgvUserScreens.Rows[0].Cells["colScreenName"].Value.ToString();
            return null;
        }

        // ==================== الأحداث ====================
        private void DgvUserScreens_SelectionChanged(object sender, EventArgs e)
        {
            if (loading) return;
            RefreshScreenUsersGrid();
        }

        private void DgvUserScreens_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (loading || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string colName = dgvUserScreens.Columns[e.ColumnIndex].Name;
            if (!colName.StartsWith("colP_")) return;

            string key = colName.Substring(5);
            object nameVal = dgvUserScreens.Rows[e.RowIndex].Cells["colScreenName"].Value;
            if (nameVal == null) return;
            string screenName = nameVal.ToString();

            var p = currentUserPermissions.FirstOrDefault(x => x.ScreenName == screenName);
            if (p == null) return;

            object val = dgvUserScreens.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            bool v = val != null && Convert.ToBoolean(val);
            SetPermValue(p, key, v);

            UserManager.SaveScreenPermissions(currentUserId, currentUserPermissions);
            RefreshGrantedDenied();
        }

        private void DgvScreenUsers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (loading || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string colName = dgvScreenUsers.Columns[e.ColumnIndex].Name;
            if (!colName.StartsWith("colP_")) return;

            string key = colName.Substring(5);
            object noVal = dgvScreenUsers.Rows[e.RowIndex].Cells["colUserNo"].Value;
            if (noVal == null) return;
            string userNo = noVal.ToString();

            var user = allUsers.FirstOrDefault(u => u.UserNumber == userNo);
            if (user == null) return;

            string screenName = GetSelectedScreenName();
            var screen = allScreens.FirstOrDefault(s => s.ScreenName == screenName);
            if (screen == null) return;

            UserManager.LoadScreenPermissionsFromDatabase(user.Id);
            var perms = UserManager.GetScreenPermissions(user.Id);
            var p = perms.FirstOrDefault(x => x.ScreenTag == screen.ScreenTag);
            if (p == null)
            {
                p = new ScreenPermission();
                p.ScreenName = screen.ScreenName;
                p.ScreenTag = screen.ScreenTag;
                perms.Add(p);
            }

            object val = dgvScreenUsers.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            bool v = val != null && Convert.ToBoolean(val);
            SetPermValue(p, key, v);
            UserManager.SaveScreenPermissions(user.Id, perms);

            if (user.Id == currentUserId)
            {
                currentUserPermissions = perms;
                RefreshTopGrid();
                RefreshGrantedDenied();
            }
        }

        // ==================== الأزرار ====================
        private void BtnAddUser_Click(object sender, EventArgs e)
        {
            using (AddUserForm f = new AddUserForm())
            {
                if (f.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshUsersList();
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (currentUserId == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار مستخدم أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            UserManager.SaveScreenPermissions(currentUserId, currentUserPermissions);
            MessageBox.Show("✅ تم حفظ الصلاحيات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== قراءة/كتابة قيمة الصلاحية ====================
        private bool GetPermValue(ScreenPermission p, string key)
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

        private void SetPermValue(ScreenPermission p, string key, bool v)
        {
            switch (key)
            {
                case "Include": p.Include = v; break;
                case "Print": p.Print = v; break;
                case "ViewReport": p.ViewReport = v; break;
                case "AddFrom": p.AddFrom = v; break;
                case "Add": p.Add = v; break;
                case "Edit": p.Edit = v; break;
                case "Delete": p.Delete = v; break;
                case "View": p.View = v; break;
                case "ViewList": p.ViewList = v; break;
                case "SendTo": p.SendTo = v; break;
                case "Review": p.Review = v; break;
                case "Post": p.Post = v; break;
                case "CancelPost": p.CancelPost = v; break;
            }
        }
    }
}