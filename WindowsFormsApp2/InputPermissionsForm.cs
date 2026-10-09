using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static YemenCustomsSystem.UserManager;

namespace YemenCustomsSystem
{
    public partial class InputPermissionsForm : Form
    {
        private ComboBox cmbUserNumber;
        private Label lblUserInfo;
        private Button btnSave;
        private DataGridView dgvUserPorts;

        private TabControl tabBottom;
        private DataGridView dgvPortUsers;
        private DataGridView dgvGranted;
        private DataGridView dgvDenied;

        private List<string> allPorts;
        private List<UserManager.User> allUsers;
        private List<PortPermission> currentUserPermissions = new List<PortPermission>();
        private int currentUserId = 0;
        private bool loading = false;

        private static readonly string[] PermKeys = { "View", "Add", "Edit", "Delete", "Print" };
        private static readonly string[] PermHeaders = { "عرض", "إضافة", "تعديل", "حذف", "طباعة" };

        public InputPermissionsForm()
        {
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            this.Text = "صلاحيات المدخلات (المنافذ) - نظام الجمارك اليمنية";
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;

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
            flow.Controls.Add(btnSave);
            pnlTop.Controls.Add(flow);

            // ===== الجدول العلوي: منافذ المستخدم المختار =====
            dgvUserPorts = new DataGridView();
            dgvUserPorts.Dock = DockStyle.Fill;
            StyleGrid(dgvUserPorts);
            dgvUserPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "colNo", HeaderText = "م", Width = 40, ReadOnly = true });
            dgvUserPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPortName", HeaderText = "المنفذ", Width = 300, ReadOnly = true });
            AddPermColumns(dgvUserPorts);
            dgvUserPorts.CellValueChanged += DgvUserPorts_CellValueChanged;
            dgvUserPorts.SelectionChanged += (s, e) => { if (!loading) RefreshPortUsersGrid(); };

            // ===== التبويبات السفلية =====
            tabBottom = new TabControl();
            tabBottom.Dock = DockStyle.Fill;
            tabBottom.Font = new Font("Tahoma", 10, FontStyle.Bold);
            tabBottom.RightToLeft = RightToLeft.Yes;

            TabPage tabPortUsers = new TabPage("مستخدمين المنفذ");
            dgvPortUsers = new DataGridView();
            dgvPortUsers.Dock = DockStyle.Fill;
            StyleGrid(dgvPortUsers);
            dgvPortUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserNo", HeaderText = "رقم المستخدم", Width = 90, ReadOnly = true });
            dgvPortUsers.Columns.Add(new DataGridViewTextBoxColumn { Name = "colUserName", HeaderText = "اسم المستخدم", Width = 250, ReadOnly = true });
            AddPermColumns(dgvPortUsers);
            dgvPortUsers.CellValueChanged += DgvPortUsers_CellValueChanged;
            tabPortUsers.Controls.Add(dgvPortUsers);

            TabPage tabGranted = new TabPage("منافذ ممنوحة للمستخدم");
            dgvGranted = new DataGridView();
            dgvGranted.Dock = DockStyle.Fill;
            StyleGrid(dgvGranted);
            dgvGranted.Columns.Add("colNoG", "م");
            dgvGranted.Columns.Add("colPortG", "المنفذ");
            dgvGranted.Columns["colNoG"].Width = 50;
            dgvGranted.Columns["colPortG"].Width = 400;
            tabGranted.Controls.Add(dgvGranted);

            TabPage tabDenied = new TabPage("منافذ غير ممنوحة للمستخدم");
            dgvDenied = new DataGridView();
            dgvDenied.Dock = DockStyle.Fill;
            StyleGrid(dgvDenied);
            dgvDenied.Columns.Add("colNoD", "م");
            dgvDenied.Columns.Add("colPortD", "المنفذ");
            dgvDenied.Columns["colNoD"].Width = 50;
            dgvDenied.Columns["colPortD"].Width = 400;
            tabDenied.Controls.Add(dgvDenied);

            tabBottom.TabPages.Add(tabPortUsers);
            tabBottom.TabPages.Add(tabGranted);
            tabBottom.TabPages.Add(tabDenied);

            SplitContainer split = new SplitContainer();
            split.Dock = DockStyle.Fill;
            split.Orientation = Orientation.Horizontal;
            split.Panel1.Controls.Add(dgvUserPorts);
            split.Panel2.Controls.Add(tabBottom);

            this.Controls.Add(split);
            this.Controls.Add(pnlTop);
            this.Load += (s, e) => { try { split.SplitterDistance = this.ClientSize.Height / 2; } catch { } };
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

        private void AddPermColumns(DataGridView dgv)
        {
            for (int i = 0; i < PermKeys.Length; i++)
            {
                DataGridViewCheckBoxColumn chk = new DataGridViewCheckBoxColumn();
                chk.Name = "colP_" + PermKeys[i];
                chk.HeaderText = PermHeaders[i];
                chk.Width = 90;
                chk.TrueValue = true;
                chk.FalseValue = false;
                chk.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns.Add(chk);
            }
        }

        private void LoadData()
        {
            allPorts = new List<string>();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName FROM Ports ORDER BY PortName");
                foreach (DataRow r in dt.Rows) allPorts.Add(r["PortName"].ToString());
            }
            catch { }

            loading = true;
            allUsers = UserManager.GetAllUsers();
            cmbUserNumber.Items.Clear();
            foreach (var u in allUsers)
            {
                string roleText = u.Role == "Admin" ? "مدير" : "مخلص";
                cmbUserNumber.Items.Add(u.UserNumber + " - " + u.FullName + " (" + roleText + ")");
            }
            if (cmbUserNumber.Items.Count > 0)
            {
                int idx = 0;
                var cur = UserManager.GetCurrentUser();
                if (cur != null)
                {
                    for (int i = 0; i < allUsers.Count; i++)
                        if (allUsers[i].Id == cur.Id) { idx = i; break; }
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
            lblUserInfo.Text = "الدور: " + (user.Role == "Admin" ? "مدير" : "مخلص");

            UserManager.LoadPortPermissionsFromDatabase(user.Id);
            currentUserPermissions = UserManager.GetPortPermissions(user.Id);
            if (currentUserPermissions == null) currentUserPermissions = new List<PortPermission>();

            RefreshTopGrid();
            RefreshGrantedDenied();
            RefreshPortUsersGrid();
        }

        private void RefreshTopGrid()
        {
            loading = true;
            dgvUserPorts.Rows.Clear();
            int no = 1;
            foreach (var port in allPorts)
            {
                var p = currentUserPermissions.FirstOrDefault(x => x.PortName == port);
                if (p == null)
                {
                    p = new PortPermission { PortName = port };
                    currentUserPermissions.Add(p);
                }
                int rowIndex = dgvUserPorts.Rows.Add();
                var row = dgvUserPorts.Rows[rowIndex];
                row.Cells["colNo"].Value = no++;
                row.Cells["colPortName"].Value = port;
                for (int i = 0; i < PermKeys.Length; i++)
                    row.Cells["colP_" + PermKeys[i]].Value = UserManager.GetPortPermValue(p, PermKeys[i]);
            }
            loading = false;
            if (dgvUserPorts.Rows.Count > 0) dgvUserPorts.Rows[0].Selected = true;
        }

        private void RefreshGrantedDenied()
        {
            dgvGranted.Rows.Clear();
            dgvDenied.Rows.Clear();
            int g = 1, d = 1;
            foreach (var port in allPorts)
            {
                var p = currentUserPermissions.FirstOrDefault(x => x.PortName == port);
                bool any = p != null && (p.View || p.Add || p.Edit || p.Delete || p.Print);
                if (any) dgvGranted.Rows.Add(g++, port);
                else dgvDenied.Rows.Add(d++, port);
            }
        }

        private void RefreshPortUsersGrid()
        {
            string portName = GetSelectedPortName();
            loading = true;
            dgvPortUsers.Rows.Clear();

            if (!string.IsNullOrEmpty(portName))
            {
                foreach (var u in allUsers)
                {
                    UserManager.LoadPortPermissionsFromDatabase(u.Id);
                    var perms = UserManager.GetPortPermissions(u.Id);
                    var p = perms.FirstOrDefault(x => x.PortName == portName);

                    int rowIndex = dgvPortUsers.Rows.Add();
                    var row = dgvPortUsers.Rows[rowIndex];
                    row.Cells["colUserNo"].Value = u.UserNumber;
                    row.Cells["colUserName"].Value = u.FullName;
                    for (int i = 0; i < PermKeys.Length; i++)
                        row.Cells["colP_" + PermKeys[i]].Value = (p != null) && UserManager.GetPortPermValue(p, PermKeys[i]);
                }
            }
            loading = false;
        }

        private string GetSelectedPortName()
        {
            if (dgvUserPorts.CurrentRow != null && dgvUserPorts.CurrentRow.Cells["colPortName"].Value != null)
                return dgvUserPorts.CurrentRow.Cells["colPortName"].Value.ToString();
            if (dgvUserPorts.Rows.Count > 0 && dgvUserPorts.Rows[0].Cells["colPortName"].Value != null)
                return dgvUserPorts.Rows[0].Cells["colPortName"].Value.ToString();
            return null;
        }

        private void DgvUserPorts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (loading || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string colName = dgvUserPorts.Columns[e.ColumnIndex].Name;
            if (!colName.StartsWith("colP_")) return;

            string key = colName.Substring(5);
            object nameVal = dgvUserPorts.Rows[e.RowIndex].Cells["colPortName"].Value;
            if (nameVal == null) return;

            var p = currentUserPermissions.FirstOrDefault(x => x.PortName == nameVal.ToString());
            if (p == null) return;

            object val = dgvUserPorts.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            bool v = val != null && Convert.ToBoolean(val);
            SetPerm(p, key, v);

            UserManager.SavePortPermissions(currentUserId, currentUserPermissions);
            RefreshGrantedDenied();
        }

        private void DgvPortUsers_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (loading || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            string colName = dgvPortUsers.Columns[e.ColumnIndex].Name;
            if (!colName.StartsWith("colP_")) return;

            string key = colName.Substring(5);
            object noVal = dgvPortUsers.Rows[e.RowIndex].Cells["colUserNo"].Value;
            if (noVal == null) return;

            var user = allUsers.FirstOrDefault(u => u.UserNumber == noVal.ToString());
            if (user == null) return;

            string portName = GetSelectedPortName();
            if (string.IsNullOrEmpty(portName)) return;

            UserManager.LoadPortPermissionsFromDatabase(user.Id);
            var perms = UserManager.GetPortPermissions(user.Id);
            var p = perms.FirstOrDefault(x => x.PortName == portName);
            if (p == null)
            {
                p = new PortPermission { PortName = portName };
                perms.Add(p);
            }

            object val = dgvPortUsers.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            bool v = val != null && Convert.ToBoolean(val);
            SetPerm(p, key, v);
            UserManager.SavePortPermissions(user.Id, perms);

            if (user.Id == currentUserId)
            {
                currentUserPermissions = perms;
                RefreshTopGrid();
                RefreshGrantedDenied();
            }
        }

        private void SetPerm(PortPermission p, string key, bool v)
        {
            switch (key)
            {
                case "View": p.View = v; break;
                case "Add": p.Add = v; break;
                case "Edit": p.Edit = v; break;
                case "Delete": p.Delete = v; break;
                case "Print": p.Print = v; break;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (currentUserId == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار مستخدم أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            UserManager.SavePortPermissions(currentUserId, currentUserPermissions);
            MessageBox.Show("✅ تم حفظ صلاحيات المنافذ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}