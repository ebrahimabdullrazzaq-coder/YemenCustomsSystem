using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class DocumentReviewForm : Form
    {
        private string _statementNo;
        private string _portName;

        private ComboBox cmbReviewStatus;
        private TextBox txtNotes;
        private TextBox txtUserName;
        private TextBox txtDate;
        private DataGridView dgvReviewHistory;
        private Button btnSave;
        private Button btnCancel;

        public DocumentReviewForm(string statementNo, string portName)
        {
            _statementNo = statementNo;
            _portName = portName;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "مراجعة وثائق النظام";
            this.Size = new Size(900, 650);
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // عنوان البيان
            Label lblTitle = new Label();
            lblTitle.Text = $"📄 مراجعة وثائق البيان رقم: {_statementNo} - منفذ: {_portName}";
            lblTitle.Font = new Font("Tahoma", 11, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(33, 150, 243);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(850, 30);
            lblTitle.BackColor = Color.FromArgb(227, 242, 253);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.BorderStyle = BorderStyle.FixedSingle;

            // حالة المراجعة
            Label lblStatus = new Label();
            lblStatus.Text = "حالة المراجعة:";
            lblStatus.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblStatus.Location = new Point(750, 60);
            lblStatus.Size = new Size(120, 25);
            lblStatus.TextAlign = ContentAlignment.MiddleRight;

            cmbReviewStatus = new ComboBox();
            cmbReviewStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReviewStatus.Font = new Font("Tahoma", 10, FontStyle.Bold);
            cmbReviewStatus.Items.AddRange(new object[] { "لم يراجع", "مطابق", "غير مطابق" });
            cmbReviewStatus.Location = new Point(600, 60);
            cmbReviewStatus.Size = new Size(140, 27);
            cmbReviewStatus.BackColor = Color.FromArgb(255, 255, 190);

            // ملاحظات
            Label lblNotes = new Label();
            lblNotes.Text = "ملاحظات:";
            lblNotes.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblNotes.Location = new Point(750, 100);
            lblNotes.Size = new Size(120, 25);
            lblNotes.TextAlign = ContentAlignment.MiddleRight;

            txtNotes = new TextBox();
            txtNotes.Font = new Font("Tahoma", 10);
            txtNotes.Multiline = true;
            txtNotes.ScrollBars = ScrollBars.Vertical;
            txtNotes.Location = new Point(20, 100);
            txtNotes.Size = new Size(720, 80);
            txtNotes.BackColor = Color.White;

            // اسم المستخدم
            Label lblUser = new Label();
            lblUser.Text = "اسم المستخدم:";
            lblUser.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblUser.Location = new Point(750, 195);
            lblUser.Size = new Size(120, 25);
            lblUser.TextAlign = ContentAlignment.MiddleRight;

            txtUserName = new TextBox();
            txtUserName.Font = new Font("Tahoma", 10);
            txtUserName.ReadOnly = true;
            txtUserName.BackColor = Color.FromArgb(240, 240, 240);
            txtUserName.Location = new Point(600, 195);
            txtUserName.Size = new Size(140, 27);

            // التاريخ
            Label lblDate = new Label();
            lblDate.Text = "التاريخ:";
            lblDate.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblDate.Location = new Point(750, 235);
            lblDate.Size = new Size(120, 25);
            lblDate.TextAlign = ContentAlignment.MiddleRight;

            txtDate = new TextBox();
            txtDate.Font = new Font("Tahoma", 10);
            txtDate.ReadOnly = true;
            txtDate.BackColor = Color.FromArgb(240, 240, 240);
            txtDate.Location = new Point(600, 235);
            txtDate.Size = new Size(140, 27);

            // جدول سجل المراجعات
            Label lblHistory = new Label();
            lblHistory.Text = "📋 سجل المراجعات:";
            lblHistory.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblHistory.ForeColor = Color.FromArgb(51, 122, 183);
            lblHistory.Location = new Point(20, 280);
            lblHistory.Size = new Size(850, 25);

            dgvReviewHistory = new DataGridView();
            dgvReviewHistory.Font = new Font("Tahoma", 9);
            dgvReviewHistory.Location = new Point(20, 310);
            dgvReviewHistory.Size = new Size(850, 220);
            dgvReviewHistory.AllowUserToAddRows = false;
            dgvReviewHistory.ReadOnly = true;
            dgvReviewHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReviewHistory.BackgroundColor = Color.White;
            dgvReviewHistory.EnableHeadersVisualStyles = false;
            dgvReviewHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvReviewHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReviewHistory.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 10, FontStyle.Bold);
            dgvReviewHistory.ColumnHeadersHeight = 35;
            dgvReviewHistory.RowTemplate.Height = 30;

            dgvReviewHistory.Columns.Add("colStatus", "حالة المراجعة");
            dgvReviewHistory.Columns.Add("colNotes", "ملاحظات");
            dgvReviewHistory.Columns.Add("colUser", "اسم المستخدم");
            dgvReviewHistory.Columns.Add("colDate", "التاريخ");

            dgvReviewHistory.Columns["colStatus"].Width = 120;
            dgvReviewHistory.Columns["colNotes"].Width = 350;
            dgvReviewHistory.Columns["colUser"].Width = 150;
            dgvReviewHistory.Columns["colDate"].Width = 180;

            // تلوين حسب الحالة
            dgvReviewHistory.CellFormatting += DgvReviewHistory_CellFormatting;

            // أزرار الحفظ والإلغاء
            btnSave = new Button();
            btnSave.Text = " حفظ";
            btnSave.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(76, 175, 80);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Location = new Point(300, 550);
            btnSave.Size = new Size(120, 40);
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button();
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(450, 550);
            btnCancel.Size = new Size(120, 40);
            btnCancel.Click += (s, ev) => this.Close();

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblStatus);
            this.Controls.Add(cmbReviewStatus);
            this.Controls.Add(lblNotes);
            this.Controls.Add(txtNotes);
            this.Controls.Add(lblUser);
            this.Controls.Add(txtUserName);
            this.Controls.Add(lblDate);
            this.Controls.Add(txtDate);
            this.Controls.Add(lblHistory);
            this.Controls.Add(dgvReviewHistory);
            this.Controls.Add(btnSave);
            this.Controls.Add(btnCancel);
        }

        private void DgvReviewHistory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 0 && e.Value != null)
            {
                string status = e.Value.ToString();
                if (status == "مطابق")
                {
                    e.CellStyle.BackColor = Color.FromArgb(220, 255, 220);
                    e.CellStyle.ForeColor = Color.DarkGreen;
                    e.CellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);
                }
                else if (status == "غير مطابق")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 220, 220);
                    e.CellStyle.ForeColor = Color.DarkRed;
                    e.CellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);
                }
                else if (status == "لم يراجع")
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 255, 220);
                    e.CellStyle.ForeColor = Color.DarkOrange;
                    e.CellStyle.Font = new Font("Tahoma", 9, FontStyle.Bold);
                }
            }
        }

        private void LoadData()
        {
            // تحميل آخر حالة مراجعة
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM DocumentReviews WHERE StatementNo = @StatementNo AND PortName = @PortName ORDER BY ReviewDate DESC LIMIT 1",
                    DatabaseHelper.CreateParameter("@StatementNo", _statementNo),
                    DatabaseHelper.CreateParameter("@PortName", _portName));

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow lastRow = dt.Rows[0];
                    cmbReviewStatus.SelectedItem = lastRow["ReviewStatus"].ToString();
                    txtNotes.Text = lastRow["Notes"]?.ToString() ?? "";
                    txtUserName.Text = lastRow["UserName"]?.ToString() ?? "";
                    txtDate.Text = lastRow["ReviewDate"].ToString();
                }
                else
                {
                    cmbReviewStatus.SelectedIndex = 0;
                    txtUserName.Text = UserManager.GetCurrentUser()?.FullName ?? "";
                    txtDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // تحميل سجل المراجعات
            LoadReviewHistory();
        }

        private void LoadReviewHistory()
        {
            dgvReviewHistory.Rows.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM DocumentReviews WHERE StatementNo = @StatementNo AND PortName = @PortName ORDER BY ReviewDate DESC",
                    DatabaseHelper.CreateParameter("@StatementNo", _statementNo),
                    DatabaseHelper.CreateParameter("@PortName", _portName));

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dgvReviewHistory.Rows.Add(
                            row["ReviewStatus"].ToString(),
                            row["Notes"]?.ToString() ?? "",
                            row["UserName"]?.ToString() ?? "",
                            row["ReviewDate"].ToString()
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تحميل السجل: {ex.Message}");
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (cmbReviewStatus.SelectedItem == null)
            {
                MessageBox.Show("⚠️ الرجاء اختيار حالة المراجعة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string status = cmbReviewStatus.SelectedItem.ToString();
                string notes = txtNotes.Text.Trim();
                string userName = UserManager.GetCurrentUser()?.FullName ?? "";
                string reviewDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                DatabaseHelper.ExecuteNonQuery(
                    @"INSERT INTO DocumentReviews 
                    (StatementNo, PortName, ReviewStatus, Notes, UserName, ReviewDate) 
                    VALUES (@StatementNo, @PortName, @Status, @Notes, @UserName, @ReviewDate)",
                    DatabaseHelper.CreateParameter("@StatementNo", _statementNo),
                    DatabaseHelper.CreateParameter("@PortName", _portName),
                    DatabaseHelper.CreateParameter("@Status", status),
                    DatabaseHelper.CreateParameter("@Notes", notes),
                    DatabaseHelper.CreateParameter("@UserName", userName),
                    DatabaseHelper.CreateParameter("@ReviewDate", reviewDate)
                );

                MessageBox.Show("✅ تم حفظ المراجعة بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadReviewHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الحفظ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}