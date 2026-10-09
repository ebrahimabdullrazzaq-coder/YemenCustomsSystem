using System;
using System.Drawing;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public class AddUserForm : Form
    {
        private TextBox txtUserNumber;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private TextBox txtFullName;
        private ComboBox cmbRole;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnCancel;

        public AddUserForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "إضافة مستخدم جديد - نظام الجمارك اليمنية";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = " إضافة مستخدم جديد",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(200, 20),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblUserNumber = new Label
            {
                Text = "رقم المستخدم:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(350, 80),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtUserNumber = new TextBox
            {
                Font = new Font("Tahoma", 10),
                Location = new Point(100, 80),
                Size = new Size(240, 27),
                TextAlign = HorizontalAlignment.Right
            };

            Label lblUsername = new Label
            {
                Text = "اسم المستخدم:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(350, 120),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtUsername = new TextBox
            {
                Font = new Font("Tahoma", 10),
                Location = new Point(100, 120),
                Size = new Size(240, 27),
                TextAlign = HorizontalAlignment.Right
            };

            Label lblPassword = new Label
            {
                Text = "كلمة المرور:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(350, 160),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtPassword = new TextBox
            {
                Font = new Font("Tahoma", 10),
                Location = new Point(100, 160),
                Size = new Size(240, 27),
                TextAlign = HorizontalAlignment.Right,
                UseSystemPasswordChar = true
            };

            Label lblFullName = new Label
            {
                Text = "الاسم الكامل:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(350, 200),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtFullName = new TextBox
            {
                Font = new Font("Tahoma", 10),
                Location = new Point(100, 200),
                Size = new Size(240, 27),
                TextAlign = HorizontalAlignment.Right
            };

            Label lblRole = new Label
            {
                Text = "الوظيفة/الدور:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(350, 240),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            cmbRole = new ComboBox
            {
                Font = new Font("Tahoma", 10),
                Location = new Point(100, 240),
                Size = new Size(240, 27),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbRole.Items.Clear();
            foreach (var r in UserManager.GetRoleDisplays())
                cmbRole.Items.Add(r);
            cmbRole.SelectedIndex = 1; // الافتراضي: مخلص جمركي
          

            chkIsActive = new CheckBox
            {
                Text = "حساب نشط",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(100, 280),
                Size = new Size(150, 25),
                Checked = true
            };

            btnSave = new Button
            {
                Text = " حفظ المستخدم",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(280, 330),
                Size = new Size(160, 40)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "❌ إلغاء",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(100, 330),
                Size = new Size(160, 40)
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUserNumber);
            this.Controls.Add(txtUserNumber);
            this.Controls.Add(lblUsername);
            this.Controls.Add(txtUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(lblFullName);
            this.Controls.Add(txtFullName);
            this.Controls.Add(lblRole);
            this.Controls.Add(cmbRole);
            this.Controls.Add(chkIsActive);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);

            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUserNumber.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال رقم المستخدم", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserNumber.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("️ الرجاء إدخال اسم المستخدم", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("️ الرجاء إدخال كلمة المرور", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("️ الرجاء إدخال الاسم الكامل", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtFullName.Focus();
                return;
            }

            try
            {
                string role = UserManager.GetRoleValueFromDisplay(cmbRole.SelectedItem?.ToString());

                var newUser = UserManager.AddUser(
                    txtUserNumber.Text.Trim(),
                    txtUsername.Text.Trim(),
                    txtPassword.Text,
                    txtFullName.Text.Trim(),
                    role,
                    chkIsActive.Checked
                );

                if (newUser != null)
                {
                    MessageBox.Show($"✅ تم إضافة المستخدم بنجاح\nرقم المستخدم: {newUser.UserNumber}\nالاسم: {newUser.FullName}",
                        "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("❌ فشل إضافة المستخدم. تأكد من عدم تكرار رقم المستخدم أو اسم المستخدم",
                        "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"حدث خطأ أثناء إضافة المستخدم:\n\n{ex.Message}",
                    "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}