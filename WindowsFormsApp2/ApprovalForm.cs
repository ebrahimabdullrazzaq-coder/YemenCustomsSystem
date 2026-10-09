using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public class ApprovalForm : Form
    {
        private ComboBox cmbStatus;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;
        private string statementNo;
        private string portName;
        private int levelNo;

        public ApprovalForm(string statementNo, string portName, int levelNo)
        {
            this.statementNo = statementNo;
            this.portName = portName;
            this.levelNo = levelNo;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = $"اعتماد البيان {statementNo} - المستوى {levelNo}";
            this.Size = new Size(600, 400);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 80,
                BackColor = Color.FromArgb(240, 248, 255),
                Padding = new Padding(10)
            };

            Label lblTitle = new Label
            {
                Text = "✅ اعتماد الطلب",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(10, 10),
                Size = new Size(560, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblInfo = new Label
            {
                Text = $"البيان: {statementNo} | المستوى: {levelNo}",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                Location = new Point(10, 45),
                Size = new Size(560, 25),
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblInfo);

            Label lblStatus = new Label
            {
                Text = "حالة الاعتماد:",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                Location = new Point(50, 110),
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            cmbStatus = new ComboBox
            {
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                Location = new Point(220, 110),
                Size = new Size(300, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.Add("approved - ✅ معتمد");
            cmbStatus.Items.Add("rejected - ❌ مرفوض");
            cmbStatus.Items.Add("returned - ↩️ مُرجع");
            cmbStatus.SelectedIndex = 0;

            Label lblNotes = new Label
            {
                Text = "الملاحظات:",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                Location = new Point(50, 160),
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtNotes = new TextBox
            {
                Font = new Font("Tahoma", 10),
                Location = new Point(220, 160),
                Size = new Size(300, 100),
                Multiline = true,
                AcceptsReturn = true
            };

            btnSave = new Button
            {
                Text = "💾 حفظ الاعتماد",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 45),
                Location = new Point(280, 280)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "❌ إلغاء",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 45),
                Location = new Point(100, 280),
                DialogResult = DialogResult.Cancel
            };

            this.Controls.Add(pnlHeader);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cmbStatus);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnSave;
            this.CancelButton = btnCancel;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string status = cmbStatus.SelectedItem.ToString().Split('-')[0].Trim();
                string notes = txtNotes.Text.Trim();
                var currentUser = UserManager.GetCurrentUser();

                // جلب معلومات المستوى
                var levels = ApprovalLevelsForm.GetApprovalLevels("TariffRequest");
                var level = levels.FirstOrDefault(l => l.LevelNo == levelNo);
                int levelId = level?.Id ?? levelNo;
                string levelName = level?.LevelName ?? $"المستوى {levelNo}";

                // حفظ في سجل الاعتمادات
                DatabaseHelper.ExecuteNonQuery(@"
                    INSERT OR REPLACE INTO ApprovalLog 
                    (StatementNo, PortName, LevelId, LevelNo, LevelName, UserId, UserName, 
                     ApprovalStatus, Notes, ApprovalDate)
                    VALUES (@S, @P, @L, @LN, @LName, @U, @UN, @Status, @Notes, datetime('now'))",
                    DatabaseHelper.CreateParameter("@S", statementNo),
                    DatabaseHelper.CreateParameter("@P", portName),
                    DatabaseHelper.CreateParameter("@L", levelId),
                    DatabaseHelper.CreateParameter("@LN", levelNo),
                    DatabaseHelper.CreateParameter("@LName", levelName),
                    DatabaseHelper.CreateParameter("@U", currentUser.Id),
                    DatabaseHelper.CreateParameter("@UN", currentUser.FullName),
                    DatabaseHelper.CreateParameter("@Status", status),
                    DatabaseHelper.CreateParameter("@Notes", notes));

                // إذا تم الرفض أو الإرجاع، تحديث حالة الطلب
                if (status == "rejected" || status == "returned")
                {
                    DatabaseHelper.ExecuteNonQuery(@"
                        UPDATE TariffRequests SET Status = 'rejected' 
                        WHERE StatementNo = @S AND PortName = @P",
                        DatabaseHelper.CreateParameter("@S", statementNo),
                        DatabaseHelper.CreateParameter("@P", portName));
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ الاعتماد: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}