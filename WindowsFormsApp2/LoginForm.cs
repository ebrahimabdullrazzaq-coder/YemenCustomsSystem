using System;
using System.Drawing;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public class LoginForm : Form
    {
        private TextBox txtUserNumber;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblUserNumber;
        private Label lblPassword;
        private Label lblStatus;
        private int loginAttempts = 0;
        private const int MAX_ATTEMPTS = 3;

        public LoginForm()
        {
            SetupForm();
            InitializeSystem();
        }

        private void InitializeSystem()
        {
            try
            {
                UserManager.Initialize();
                ShowStatus("✅ النظام جاهز - أدخل رقم المستخدم وكلمة المرور", Color.FromArgb(39, 174, 96));
            }
            catch (Exception ex)
            {
                ShowStatus($"❌ خطأ في الاتصال بقاعدة البيانات", Color.Red);
                MessageBox.Show(
                    $"⚠️ فشل الاتصال بقاعدة البيانات!\n\n{ex.Message}",
                    "خطأ في النظام",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SetupForm()
        {
            this.SuspendLayout();

            this.Text = "تسجيل الدخول - نظام الجمارك اليمنية";
            this.Name = "LoginForm";
            this.ClientSize = new Size(500, 450);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;
            this.KeyPreview = true;

            lblTitle = new Label
            {
                Text = "️ نظام الجمارك اليمنية المتكامل",
                Font = new Font("Tahoma", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(50, 20),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblUserNumber = new Label
            {
                Text = "رقم المستخدم:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(320, 170),
                Size = new Size(130, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtUserNumber = new TextBox
            {
                Font = new Font("Tahoma", 11),
                Location = new Point(100, 170),
                Size = new Size(210, 28),
                TextAlign = HorizontalAlignment.Center
            };

            lblPassword = new Label
            {
                Text = "كلمة المرور:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(320, 220),
                Size = new Size(130, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtPassword = new TextBox
            {
                Font = new Font("Tahoma", 11),
                Location = new Point(100, 220),
                Size = new Size(210, 28),
                TextAlign = HorizontalAlignment.Center,
                UseSystemPasswordChar = true
            };

            btnLogin = new Button
            {
                Text = "🔐 تسجيل الدخول",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(33, 150, 243),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(250, 280),
                Size = new Size(180, 40)
            };
            btnLogin.Click += BtnLogin_Click;

            btnCancel = new Button
            {
                Text = "❌ إلغاء",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(60, 280),
                Size = new Size(180, 40)
            };
            btnCancel.Click += BtnCancel_Click;

            lblStatus = new Label
            {
                Text = "💡 بيانات الدخول الافتراضية: رقم المستخدم 1 / admin",
                Font = new Font("Tahoma", 9, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(50, 350),
                Size = new Size(400, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblUserNumber);
            this.Controls.Add(txtUserNumber);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnCancel);
            this.Controls.Add(lblStatus);

            this.AcceptButton = btnLogin;
            this.CancelButton = btnCancel;

            this.ResumeLayout(false);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string userNumber = txtUserNumber.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(userNumber))
            {
                ShowStatus("️ الرجاء إدخال رقم المستخدم", Color.Red);
                txtUserNumber.Focus();
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowStatus("️ الرجاء إدخال كلمة المرور", Color.Red);
                txtPassword.Focus();
                return;
            }

            try
            {
                var user = UserManager.LoginByUserNumber(userNumber, password);

                if (user == null)
                {
                    loginAttempts++;
                    int remaining = MAX_ATTEMPTS - loginAttempts;

                    if (remaining <= 0)
                    {
                        MessageBox.Show("🚫 تم تجاوز عدد محاولات الدخول.\nسيتم إغلاق البرنامج.",
                            "تنبيه أمني", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                        return;
                    }

                    ShowStatus($"❌ بيانات الدخول غير صحيحة. المحاولات المتبقية: {remaining}", Color.Red);
                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                if (!user.IsActive)
                {
                    ShowStatus("🚫 هذا الحساب معطل. تواصل مع المدير.", Color.Red);
                    return;
                }

                ShowStatus($"✅ مرحباً {user.FullName} ({user.Role})", Color.FromArgb(39, 174, 96));
                this.DialogResult = DialogResult.OK;
                this.Tag = user;
                this.Close();
            }
            catch (Exception ex)
            {
                ShowStatus($"❌ خطأ في تسجيل الدخول", Color.Red);
                MessageBox.Show(
                    $"حدث خطأ أثناء تسجيل الدخول:\n\n{ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("هل تريد إغلاق البرنامج؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void ShowStatus(string message, Color color)
        {
            lblStatus.Text = message;
            lblStatus.ForeColor = color;
            lblStatus.Font = new Font("Tahoma", 10, FontStyle.Bold);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter)
            {
                BtnLogin_Click(this, EventArgs.Empty);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}