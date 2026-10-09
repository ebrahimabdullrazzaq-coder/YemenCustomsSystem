using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public class UsersManagementForm : Form
    {
        // ==================== المتغيرات العامة ====================
        private TabControl tabMain;
        private DataGridView dgvUsers;
        private DataGridView dgvUserPermissions;

        // تبويب إضافة/تعديل مستخدم
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private ComboBox cmbRole;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnClear;
        private Label lblStatus;
        private int currentUserId = -1;

        // تبويب الصلاحيات
        private ComboBox cmbSelectUser;
        private Button btnApplyPermissions;

        public UsersManagementForm()
        {
            if (!UserManager.IsAdmin())
            {
                MessageBox.Show("🚫 هذه الشاشة متاحة للمدير فقط", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.Close();
                return;
            }

            SetupForm();
            LoadUsers();
            LoadPermissionsList();
            LoadUsersComboBox();
        }

        private void SetupForm()
        {
            this.Text = "👥 إدارة المستخدمين والصلاحيات";
            this.Name = "UsersManagementForm";
            this.ClientSize = new Size(1400, 800);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(236, 240, 241);

            // شريط الأدوات العلوي
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Dock = DockStyle.Top;
            toolStrip.BackColor = Color.FromArgb(44, 62, 80);
            toolStrip.RightToLeft = RightToLeft.Yes;
            toolStrip.Padding = new Padding(5);

            ToolStripButton btnHome = new ToolStripButton("🏠 الرئيسية");
            btnHome.ForeColor = Color.White;
            btnHome.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnHome.Click += (s, e) => this.Close();

            ToolStripButton btnRefresh = new ToolStripButton("🔄 تحديث الكل");
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnRefresh.Click += (s, e) => { LoadUsers(); LoadPermissionsList(); LoadUsersComboBox(); };

            toolStrip.Items.Add(btnHome);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnRefresh);

            // اللوحة الرئيسية
            Panel pnlMain = new Panel();
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Padding = new Padding(15);
            pnlMain.BackColor = Color.FromArgb(236, 240, 241);

            // إنشاء TabControl
            tabMain = new TabControl();
            tabMain.Dock = DockStyle.Fill;
            tabMain.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            tabMain.Padding = new Point(15, 8);

            // التبويب 1: قائمة المستخدمين
            TabPage tabUsers = new TabPage("📋 قائمة المستخدمين");
            tabUsers.BackColor = Color.White;
            tabUsers.Padding = new Padding(10);
            SetupUsersTab(tabUsers);

            // التبويب 2: إضافة/تعديل مستخدم
            TabPage tabAddEdit = new TabPage("➕ إضافة / تعديل مستخدم");
            tabAddEdit.BackColor = Color.White;
            tabAddEdit.Padding = new Padding(10);
            SetupAddEditTab(tabAddEdit);

            // التبويب 3: إدارة الصلاحيات
            TabPage tabPermissions = new TabPage("🔐 إدارة الصلاحيات");
            tabPermissions.BackColor = Color.White;
            tabPermissions.Padding = new Padding(10);
            SetupPermissionsTab(tabPermissions);

            // إضافة التبويبات
            tabMain.TabPages.Add(tabUsers);
            tabMain.TabPages.Add(tabAddEdit);
            tabMain.TabPages.Add(tabPermissions);

            // تغيير التبويب عند النقر على مستخدم
            tabMain.SelectedIndexChanged += TabMain_SelectedIndexChanged;

            pnlMain.Controls.Add(tabMain);

            this.Controls.Add(pnlMain);
            this.Controls.Add(toolStrip);
        }

        // ==================== التبويب 1: قائمة المستخدمين ====================
        private void SetupUsersTab(TabPage tab)
        {
            Label lblTitle = new Label();
            lblTitle.Text = "📋 قائمة جميع المستخدمين المسجلين في النظام";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 152, 219);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.BackColor = Color.FromArgb(248, 249, 250);

            Panel pnlButtons = new Panel();
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.Height = 60;
            pnlButtons.BackColor = Color.FromArgb(248, 249, 250);

            Button btnAddNew = CreateButton("➕ إضافة مستخدم جديد", 10, 10, 200, 40, Color.FromArgb(46, 204, 113), Color.White);
            btnAddNew.Click += (s, e) => { tabMain.SelectedIndex = 1; ClearForm(); };

            Button btnEdit = CreateButton("✏️ تعديل المستخدم المحدد", 220, 10, 200, 40, Color.FromArgb(52, 152, 219), Color.White);
            btnEdit.Click += BtnEditSelected_Click;

            Button btnDelete = CreateButton("🗑️ حذف المستخدم المحدد", 430, 10, 200, 40, Color.FromArgb(231, 76, 60), Color.White);
            btnDelete.Click += BtnDeleteSelected_Click;

            Button btnPermissions = CreateButton("🔐 إدارة صلاحيات المستخدم", 640, 10, 200, 40, Color.FromArgb(155, 89, 182), Color.White);
            btnPermissions.Click += (s, e) => { tabMain.SelectedIndex = 2; LoadSelectedUserPermissions(); };

            pnlButtons.Controls.Add(btnAddNew);
            pnlButtons.Controls.Add(btnEdit);
            pnlButtons.Controls.Add(btnDelete);
            pnlButtons.Controls.Add(btnPermissions);

            dgvUsers = CreateUsersGrid();
            dgvUsers.Dock = DockStyle.Fill;

            tab.Controls.Add(dgvUsers);
            tab.Controls.Add(pnlButtons);
            tab.Controls.Add(lblTitle);
        }

        // ==================== التبويب 2: إضافة/تعديل مستخدم ====================
        private void SetupAddEditTab(TabPage tab)
        {
            Label lblTitle = new Label();
            lblTitle.Text = "➕ إضافة مستخدم جديد أو تعديل بيانات مستخدم موجود";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(52, 152, 219);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.BackColor = Color.FromArgb(248, 249, 250);

            GroupBox grpUser = new GroupBox();
            grpUser.Text = "👤 بيانات المستخدم";
            grpUser.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grpUser.ForeColor = Color.FromArgb(52, 152, 219);
            grpUser.Dock = DockStyle.Top;
            grpUser.Height = 380;
            grpUser.Padding = new Padding(20, 25, 20, 20);
            grpUser.BackColor = Color.White;

            int startY = 35;
            int controlWidth = 300;
            int labelX = 400;
            int controlX = 90;
            int rowSpacing = 50;

            Label lblUsername = CreateLabel("اسم المستخدم:", labelX, startY);
            txtUsername = CreateTextBox(controlX, startY, controlWidth, false);
            grpUser.Controls.Add(lblUsername);
            grpUser.Controls.Add(txtUsername);

            Label lblPassword = CreateLabel("كلمة المرور:", labelX, startY + rowSpacing);
            txtPassword = CreateTextBox(controlX, startY + rowSpacing, controlWidth, true);
            grpUser.Controls.Add(lblPassword);
            grpUser.Controls.Add(txtPassword);

            Label lblFullName = CreateLabel("الاسم الكامل:", labelX, startY + rowSpacing * 2);
            txtFullName = CreateTextBox(controlX, startY + rowSpacing * 2, controlWidth, false);
            grpUser.Controls.Add(lblFullName);
            grpUser.Controls.Add(txtFullName);

            Label lblRole = CreateLabel("الدور:", labelX, startY + rowSpacing * 3);
            cmbRole = CreateComboBox(controlX, startY + rowSpacing * 3, controlWidth);
            cmbRole.Items.Add("Admin");      // 👑 مدير
            cmbRole.Items.Add("Officer");    // 👤 مخلص
            cmbRole.Items.Add("Reviewer");   // 🔍 مراجع
            cmbRole.SelectedIndex = 1;
            grpUser.Controls.Add(lblRole);
            grpUser.Controls.Add(cmbRole);

            Label lblStatus2 = CreateLabel("الحالة:", labelX, startY + rowSpacing * 4);
            chkIsActive = new CheckBox();
            chkIsActive.Text = "نشط";
            chkIsActive.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            chkIsActive.Location = new Point(controlX, startY + rowSpacing * 4 + 5);
            chkIsActive.Size = new Size(100, 25);
            chkIsActive.Checked = true;
            chkIsActive.RightToLeft = RightToLeft.Yes;
            grpUser.Controls.Add(lblStatus2);
            grpUser.Controls.Add(chkIsActive);

            int btnY = startY + rowSpacing * 5 + 10;

            btnSave = CreateButton("💾 حفظ", 350, btnY, 180, 45, Color.FromArgb(46, 204, 113), Color.White);
            btnSave.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSave.Click += BtnSave_Click;

            btnClear = CreateButton("🔄 مسح الحقول", 150, btnY, 180, 45, Color.FromArgb(149, 165, 166), Color.White);
            btnClear.Click += (s, e) => ClearForm();

            grpUser.Controls.Add(btnSave);
            grpUser.Controls.Add(btnClear);

            Panel pnlStatus = new Panel();
            pnlStatus.Dock = DockStyle.Bottom;
            pnlStatus.Height = 50;
            pnlStatus.BackColor = Color.FromArgb(248, 249, 250);

            lblStatus = new Label();
            lblStatus.Text = "🆕 جاهز لإضافة مستخدم جديد - أدخل البيانات ثم اضغط حفظ";
            lblStatus.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            pnlStatus.Controls.Add(lblStatus);

            tab.Controls.Add(grpUser);
            tab.Controls.Add(pnlStatus);
            tab.Controls.Add(lblTitle);
        }

        // ==================== التبويب 3: إدارة الصلاحيات ====================
        private void SetupPermissionsTab(TabPage tab)
        {
            Label lblTitle = new Label();
            lblTitle.Text = "🔐 إدارة صلاحيات المستخدمين";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(231, 76, 60);
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.BackColor = Color.FromArgb(248, 249, 250);

            Panel pnlSelectUser = new Panel();
            pnlSelectUser.Dock = DockStyle.Top;
            pnlSelectUser.Height = 80;
            pnlSelectUser.BackColor = Color.White;
            pnlSelectUser.Padding = new Padding(20, 15, 20, 15);

            Label lblSelectUser = new Label();
            lblSelectUser.Text = "اختر المستخدم:";
            lblSelectUser.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblSelectUser.Location = new Point(600, 20);
            lblSelectUser.Size = new Size(120, 30);
            lblSelectUser.TextAlign = ContentAlignment.MiddleRight;

            cmbSelectUser = new ComboBox();
            cmbSelectUser.Font = new Font("Segoe UI", 11);
            cmbSelectUser.Location = new Point(200, 20);
            cmbSelectUser.Size = new Size(380, 30);
            cmbSelectUser.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSelectUser.SelectedIndexChanged += CmbSelectUser_SelectedIndexChanged;

            btnApplyPermissions = CreateButton("💾 تطبيق الصلاحيات", 10, 18, 180, 35, Color.FromArgb(46, 204, 113), Color.White);
            btnApplyPermissions.Click += BtnApplyPermissions_Click;

            pnlSelectUser.Controls.Add(lblSelectUser);
            pnlSelectUser.Controls.Add(cmbSelectUser);
            pnlSelectUser.Controls.Add(btnApplyPermissions);

            GroupBox grpPermissions = new GroupBox();
            grpPermissions.Text = "🔐 الصلاحيات المتاحة";
            grpPermissions.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            grpPermissions.ForeColor = Color.FromArgb(231, 76, 60);
            grpPermissions.Dock = DockStyle.Fill;
            grpPermissions.Padding = new Padding(15, 25, 15, 15);
            grpPermissions.BackColor = Color.White;

            dgvUserPermissions = CreatePermissionsGrid();
            grpPermissions.Controls.Add(dgvUserPermissions);

            tab.Controls.Add(grpPermissions);
            tab.Controls.Add(pnlSelectUser);
            tab.Controls.Add(lblTitle);
        }

        // ==================== دوال مساعدة ====================
        private Label CreateLabel(string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(130, 28);
            lbl.TextAlign = ContentAlignment.MiddleRight;
            lbl.BackColor = Color.Transparent;
            return lbl;
        }

        private TextBox CreateTextBox(int x, int y, int width, bool isPassword)
        {
            TextBox txt = new TextBox();
            txt.Font = new Font("Segoe UI", 10);
            txt.Location = new Point(x, y);
            txt.Size = new Size(width, 30);
            txt.UseSystemPasswordChar = isPassword;
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.FromArgb(248, 249, 250);
            return txt;
        }

        private ComboBox CreateComboBox(int x, int y, int width)
        {
            ComboBox cmb = new ComboBox();
            cmb.Font = new Font("Segoe UI", 10);
            cmb.Location = new Point(x, y);
            cmb.Size = new Size(width, 30);
            cmb.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb.BackColor = Color.FromArgb(248, 249, 250);
            return cmb;
        }

        private Button CreateButton(string text, int x, int y, int width, int height, Color backColor, Color foreColor)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Location = new Point(x, y);
            btn.Size = new Size(width, height);
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private DataGridView CreateUsersGrid()
        {
            DataGridView dgv = new DataGridView();
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.RightToLeft = RightToLeft.Yes;
            dgv.BackgroundColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(52, 152, 219);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 45;
            dgv.RowTemplate.Height = 40; // تم تصحيح الاستخدام
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.FromArgb(236, 240, 241);

            dgv.Columns.Add("colId", "م");
            dgv.Columns.Add("colUsername", "اسم المستخدم");
            dgv.Columns.Add("colFullName", "الاسم الكامل");
            dgv.Columns.Add("colRole", "الدور");
            dgv.Columns.Add("colStatus", "الحالة");
            dgv.Columns.Add("colLastLogin", "آخر دخول");

            dgv.CellClick += DgvUsers_CellClick;
            dgv.CellFormatting += DgvUsers_CellFormatting;

            return dgv;
        }

        private DataGridView CreatePermissionsGrid()
        {
            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.RightToLeft = RightToLeft.Yes;
            dgv.BackgroundColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(231, 76, 60);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersHeight = 45;
            dgv.RowTemplate.Height = 40; // تم تصحيح الاستخدام
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10.5f);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.BorderStyle = BorderStyle.None;
            dgv.GridColor = Color.FromArgb(236, 240, 241);

            DataGridViewTextBoxColumn colName = new DataGridViewTextBoxColumn();
            colName.Name = "colPermName";
            colName.HeaderText = "اسم الصلاحية";
            colName.ReadOnly = true;
            colName.FillWeight = 70;
            colName.DefaultCellStyle.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            dgv.Columns.Add(colName);

            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn();
            colCheck.Name = "colGranted";
            colCheck.HeaderText = "ممنوحة ✅";
            colCheck.FillWeight = 30;
            colCheck.FlatStyle = FlatStyle.Standard;
            colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns.Add(colCheck);

            return dgv;
        }

        // ==================== أحداث التبويبات ====================
        private void TabMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabMain.SelectedIndex == 1) // تبويب إضافة/تعديل
            {
                if (currentUserId >= 0)
                {
                    btnSave.Text = "💾 حفظ التعديلات";
                    lblStatus.Text = "📝 تعديل المستخدم الحالي";
                    lblStatus.ForeColor = Color.FromArgb(52, 152, 219);
                }
                else
                {
                    btnSave.Text = "💾 حفظ المستخدم الجديد";
                    lblStatus.Text = "🆕 جاهز لإضافة مستخدم جديد";
                    lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
                }
            }
            else if (tabMain.SelectedIndex == 2) // تبويب الصلاحيات
            {
                LoadUsersComboBox();
            }
        }

        private void DgvUsers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string username = dgvUsers.Rows[e.RowIndex].Cells["colUsername"].Value?.ToString();
            var user = UserManager.GetUserByUsername(username);
            if (user == null) return;

            currentUserId = user.Id;

            txtUsername.Text = user.Username;
            txtUsername.ReadOnly = true;
            txtPassword.Text = "";
            txtFullName.Text = user.FullName;
            cmbRole.SelectedItem = user.Role;
            chkIsActive.Checked = user.IsActive;

            lblStatus.Text = $"📝 تم تحديد المستخدم: {user.FullName} - يمكنك التعديل أو الانتقال لتبويب الصلاحيات";
            lblStatus.ForeColor = Color.FromArgb(52, 152, 219);
        }

        private void DgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            DataGridViewRow row = dgvUsers.Rows[e.RowIndex];

            if (row.Cells["colRole"].Value?.ToString() == "👑 مدير")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
                row.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
            else
            {
                row.DefaultCellStyle.BackColor = e.RowIndex % 2 == 0 ? Color.FromArgb(248, 249, 250) : Color.White;
            }

            if (e.ColumnIndex == dgvUsers.Columns["colStatus"].Index)
            {
                string status = e.Value?.ToString() ?? "";
                if (status.Contains("نشط"))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(39, 174, 96);
                    e.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(192, 57, 43);
                }
            }
        }

        private void CmbSelectUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSelectedUserPermissions();
        }

        // ==================== دوال العمليات ====================
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;
            string fullName = txtFullName.Text.Trim();
            string role = cmbRole.SelectedItem?.ToString() ?? "Officer";
            bool isActive = chkIsActive.Checked;

            if (currentUserId < 0)
            {
                if (string.IsNullOrEmpty(password))
                {
                    MessageBox.Show("⚠️ الرجاء إدخال كلمة المرور للمستخدم الجديد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }
                if (password.Length < 6)
                {
                    MessageBox.Show("⚠️ كلمة المرور يجب أن تكون 6 أحرف على الأقل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    txtPassword.Focus();
                    return;
                }

                var currentUser = UserManager.GetCurrentUser();
                var newUser = UserManager.AddUser(username, password, fullName, role, UserManager.GetAllPermissions(false), currentUser?.Username ?? "System");

                if (newUser == null)
                {
                    MessageBox.Show($"❌ اسم المستخدم '{username}' موجود مسبقاً", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                LoadUsers();
                LoadUsersComboBox();
                ClearForm();
                MessageBox.Show($"✅ تمت إضافة المستخدم '{fullName}' بنجاح\n\n💡 انتقل إلى تبويب 'إدارة الصلاحيات' لتحديد صلاحياته", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var user = UserManager.GetUserById(currentUserId);
                if (UserManager.UpdateUser(currentUserId, fullName, role, user.Permissions, isActive))
                {
                    LoadUsers();
                    MessageBox.Show("✅ تم تحديث بيانات المستخدم بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("❌ فشل تحديث بيانات المستخدم", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnEditSelected_Click(object sender, EventArgs e)
        {
            if (currentUserId < 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار مستخدم من القائمة أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            tabMain.SelectedIndex = 1;
        }

        private void BtnDeleteSelected_Click(object sender, EventArgs e)
        {
            if (currentUserId < 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار مستخدم للحذف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = UserManager.GetUserById(currentUserId);
            if (user == null) return;

            if (user.Role == "Admin")
            {
                MessageBox.Show("🚫 لا يمكن حذف حساب المدير", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show($"🗑️ هل أنت متأكد من حذف '{user.FullName}'؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (UserManager.DeleteUser(currentUserId))
                {
                    LoadUsers();
                    LoadUsersComboBox();
                    ClearForm();
                    MessageBox.Show("✅ تم حذف المستخدم بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnApplyPermissions_Click(object sender, EventArgs e)
        {
            if (cmbSelectUser.SelectedIndex < 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار مستخدم أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedText = cmbSelectUser.SelectedItem.ToString();
            string username = selectedText.Split('-')[0].Trim();
            var user = UserManager.GetUserByUsername(username);
            if (user == null) return;

            Dictionary<string, bool> permissions = new Dictionary<string, bool>();
            for (int i = 0; i < dgvUserPermissions.Rows.Count; i++)
            {
                string permKey = dgvUserPermissions.Rows[i].Tag?.ToString();
                bool granted = false;
                if (dgvUserPermissions.Rows[i].Cells["colGranted"].Value != null)
                {
                    bool.TryParse(dgvUserPermissions.Rows[i].Cells["colGranted"].Value.ToString(), out granted);
                }
                if (permKey != null)
                    permissions[permKey] = granted;
            }

            if (UserManager.UpdateUser(user.Id, user.FullName, user.Role, permissions, user.IsActive))
            {
                MessageBox.Show($"✅ تم تحديث صلاحيات المستخدم '{user.FullName}' بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("❌ فشل تحديث الصلاحيات", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== دوال التحميل ====================
        private void LoadUsers()
        {
            dgvUsers.Rows.Clear();
            var allUsers = UserManager.GetAllUsers();
            int idx = 1;

            foreach (var user in allUsers)
            {
                string roleDisplay;
                if (user.Role == "Admin")
                    roleDisplay = "👑 مدير";
                else if (user.Role == "Reviewer")
                    roleDisplay = "🔍 مراجع";
                else
                    roleDisplay = "👤 مخلص";
                string statusDisplay = user.IsActive ? "✅ نشط" : "❌ معطل";
                string lastLogin = user.LastLogin.HasValue ? user.LastLogin.Value.ToString("yyyy-MM-dd HH:mm") : "لم يدخل بعد";

                dgvUsers.Rows.Add(idx++, user.Username, user.FullName, roleDisplay, statusDisplay, lastLogin);
            }
        }

        private void LoadPermissionsList()
        {
            dgvUserPermissions.Rows.Clear();
            var permissions = UserManager.GetPermissionsDisplayNames();

            foreach (var perm in permissions)
            {
                int rowIndex = dgvUserPermissions.Rows.Add(perm.Value, false);
                dgvUserPermissions.Rows[rowIndex].Tag = perm.Key;

                if (rowIndex % 2 == 0)
                    dgvUserPermissions.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(253, 245, 245);
                else
                    dgvUserPermissions.Rows[rowIndex].DefaultCellStyle.BackColor = Color.White;
            }
        }

        private void LoadUsersComboBox()
        {
            cmbSelectUser.Items.Clear();
            var allUsers = UserManager.GetAllUsers();

            foreach (var user in allUsers)
            {
                string role;
                if (user.Role == "Admin")
                    role = "👑 مدير";
                else if (user.Role == "Reviewer")
                    role = "🔍 مراجع";
                else
                    role = "👤 مخلص";
                cmbSelectUser.Items.Add($"{user.Username} - {user.FullName} ({role})");
            }
        }

        private void LoadSelectedUserPermissions()
        {
            if (cmbSelectUser.SelectedIndex < 0) return;

            string selectedText = cmbSelectUser.SelectedItem.ToString();
            string username = selectedText.Split('-')[0].Trim();
            var user = UserManager.GetUserByUsername(username);
            if (user == null) return;

            for (int i = 0; i < dgvUserPermissions.Rows.Count; i++)
            {
                string permKey = dgvUserPermissions.Rows[i].Tag?.ToString();
                if (permKey != null && user.Permissions.ContainsKey(permKey))
                {
                    dgvUserPermissions.Rows[i].Cells["colGranted"].Value = user.Permissions[permKey];
                }
                else
                {
                    dgvUserPermissions.Rows[i].Cells["colGranted"].Value = false;
                }
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال اسم المستخدم", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال الاسم الكامل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return false;
            }
            if (cmbRole.SelectedIndex < 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار الدور", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbRole.Focus();
                return false;
            }
            return true;
        }

        private void ClearForm()
        {
            currentUserId = -1;
            txtUsername.Clear();
            txtUsername.ReadOnly = false;
            txtPassword.Clear();
            txtPassword.ReadOnly = false;
            txtFullName.Clear();
            cmbRole.SelectedIndex = 1;
            chkIsActive.Checked = true;

            btnSave.Text = "💾 حفظ المستخدم الجديد";
            lblStatus.Text = "🆕 جاهز لإضافة مستخدم جديد";
            lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
        }
    }
}