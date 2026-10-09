using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public class ApprovalStatusForm : Form
    {
        private DataGridView dgvApprovalLog;
        private string statementNo;
        private string portName;

        public ApprovalStatusForm(string statementNo, string portName)
        {
            this.statementNo = statementNo;
            this.portName = portName;
            InitializeComponent();
            LoadApprovalLog();
        }

        private void InitializeComponent()
        {
            this.Text = $"حالة الاعتماد - بيان رقم: {statementNo}";
            this.Size = new Size(1100, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(240, 248, 255),
                Padding = new Padding(10)
            };

            Label lblTitle = new Label
            {
                Text = "📊 حالة الاعتماد للبيان",
                Font = new Font("Tahoma", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 150, 243),
                Location = new Point(10, 10),
                Size = new Size(300, 30),
                TextAlign = ContentAlignment.MiddleRight
            };

            Label lblStatementNo = new Label
            {
                Text = $"رقم البيان: {statementNo}",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(76, 175, 80),
                Location = new Point(10, 40),
                Size = new Size(300, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblStatementNo);

            dgvApprovalLog = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                EnableHeadersVisualStyles = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D,
                ColumnHeadersDefaultCellStyle =
                {
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    Font = new Font("Tahoma", 10, FontStyle.Bold),
                    Alignment = DataGridViewContentAlignment.MiddleCenter
                },
                ColumnHeadersHeight = 45,
            
                AlternatingRowsDefaultCellStyle = { BackColor = Color.FromArgb(245, 245, 245) },
                DefaultCellStyle = { Font = new Font("Tahoma", 9.5f) }
            };

            dgvApprovalLog.Columns.Add("colLevelNo", "المستوى");
            dgvApprovalLog.Columns.Add("colLevelName", "اسم المستوى");
            dgvApprovalLog.Columns.Add("colUserName", "المستخدم");
            dgvApprovalLog.Columns.Add("colApprovalStatus", "حالة الاعتماد");
            dgvApprovalLog.Columns.Add("colApprovalDate", "تاريخ الاعتماد");
            dgvApprovalLog.Columns.Add("colNotes", "ملاحظات");

            dgvApprovalLog.Columns["colLevelNo"].Width = 80;
            dgvApprovalLog.Columns["colLevelName"].Width = 150;
            dgvApprovalLog.Columns["colUserName"].Width = 150;
            dgvApprovalLog.Columns["colApprovalStatus"].Width = 150;
            dgvApprovalLog.Columns["colApprovalDate"].Width = 150;
            dgvApprovalLog.Columns["colNotes"].Width = 200;

            dgvApprovalLog.Columns["colApprovalStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvApprovalLog.Columns["colApprovalStatus"].DefaultCellStyle.Font = new Font("Tahoma", 9.5f, FontStyle.Bold);

            Panel pnlBottom = new Panel
            {
                Height = 60,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(240, 248, 255),
                Padding = new Padding(10)
            };

            Button btnClose = new Button
            {
                Text = " إغلاق",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(10, 10),
                DialogResult = DialogResult.Cancel
            };

            pnlBottom.Controls.Add(btnClose);

            this.Controls.Add(dgvApprovalLog);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBottom);
            this.AcceptButton = btnClose;
            this.CancelButton = btnClose;
        }

        private void LoadApprovalLog()
        {
            dgvApprovalLog.Rows.Clear();

            try
            {
                // جلب جميع مستويات الاعتماد للشاشة
                var levels = ApprovalLevelsForm.GetApprovalLevels("TariffRequest");

                // جلب سجل الاعتمادات
                DataTable dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT LevelNo, LevelName, UserName, ApprovalStatus, ApprovalDate, Notes 
                    FROM ApprovalLog 
                    WHERE StatementNo = @S AND PortName = @P 
                    ORDER BY LevelNo",
                    DatabaseHelper.CreateParameter("@S", statementNo),
                    DatabaseHelper.CreateParameter("@P", portName));

                // عرض جميع المستويات حتى لو لم تُعتمد بعد
                foreach (var level in levels)
                {
                    var logRow = dt?.Rows.Cast<DataRow>()
                        .FirstOrDefault(r => Convert.ToInt32(r["LevelNo"]) == level.LevelNo);

                    string status = " غير معتمد";
                    Color statusColor = Color.Orange;
                    string userName = "-";
                    string approvalDate = "-";
                    string notes = "";

                    if (logRow != null)
                    {
                        status = logRow["ApprovalStatus"].ToString();
                        userName = logRow["UserName"].ToString();
                        approvalDate = logRow["ApprovalDate"].ToString();
                        notes = logRow["Notes"]?.ToString() ?? "";

                        switch (status)
                        {
                            case "approved":
                                status = "✅ معتمد";
                                statusColor = Color.FromArgb(76, 175, 80);
                                break;
                            case "rejected":
                                status = "❌ مرفوض";
                                statusColor = Color.FromArgb(244, 67, 54);
                                break;
                            case "returned":
                                status = "️ مُرجع";
                                statusColor = Color.FromArgb(255, 152, 0);
                                break;
                        }
                    }

                    int rowIndex = dgvApprovalLog.Rows.Add(
                        level.LevelNo,
                        level.LevelName,
                        userName,
                        status,
                        approvalDate != "-" ? Convert.ToDateTime(approvalDate).ToString("yyyy-MM-dd HH:mm") : "-",
                        notes
                    );

                    dgvApprovalLog.Rows[rowIndex].Cells["colApprovalStatus"].Style.ForeColor = statusColor;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل سجل الاعتمادات: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}