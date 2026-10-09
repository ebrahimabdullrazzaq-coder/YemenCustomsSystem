using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Data.SQLite;

namespace YemenCustomsSystem
{
    public class PortGroupsForm : Form
    {
        // تعريف عناصر التحكم
        private TextBox txtGroupNo;
        private TextBox txtGroupName;
        private DataGridView dgvPortGroups;
        private Button btnSave;
        private Button btnDelete;
        private Button btnClear;

        private int? currentEditingId = null; // لتتبع ما إذا كنا نعدل سجلاً موجوداً

        public PortGroupsForm()
        {
            this.Text = "إدارة مجموعات المنافذ";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.FromArgb(245, 247, 250);

            EnsureTableExists();
            SetupUI();
            LoadData();
        }

        // 1. التأكد من وجود الجدول في قاعدة البيانات
        // 1. التأكد من وجود الجدول والأعمدة الصحيحة في قاعدة البيانات
        private void EnsureTableExists()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS PortGroups (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            GroupNo TEXT UNIQUE NOT NULL,
            GroupName TEXT NOT NULL,
            IsActive INTEGER DEFAULT 1,
            Description TEXT,
            CreatedDate TEXT DEFAULT (datetime('now'))
        )");

                var cnt = DatabaseHelper.ExecuteScalar("SELECT COUNT(*) FROM PortGroups");
                if (cnt == null || Convert.ToInt32(cnt) == 0)
                {
                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('1', 'مجموعة المنافذ الشمالية', 'منافذ المنطقة الشمالية', 1)");
                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('2', 'مجموعة المنافذ الجنوبية', 'منافذ المنطقة الجنوبية', 1)");
                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('3', 'مجموعة المنافذ الشرقية', 'منافذ المنطقة الشرقية', 1)");
                    DatabaseHelper.ExecuteNonQuery("INSERT INTO PortGroups (GroupNo, GroupName, Description, IsActive) VALUES ('4', 'مجموعة المنافذ الغربية', 'منافذ المنطقة الغربية', 1)");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في إنشاء الجدول: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 2. تصميم واجهة المستخدم
        private void SetupUI()
        {
            // لوحة الإدخال العلوية
            Panel pnlInput = new Panel();
            pnlInput.Dock = DockStyle.Top;
            pnlInput.Height = 100;
            pnlInput.BackColor = Color.White;
            pnlInput.Padding = new Padding(15);

            Label lblGroupNo = new Label { Text = "رقم مجموعة المنافذ:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(550, 15), Size = new Size(150, 25) };
            txtGroupNo = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(350, 12), Size = new Size(180, 25) };

            Label lblGroupName = new Label { Text = "اسم مجموعة المنافذ:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(550, 50), Size = new Size(150, 25) };
            txtGroupName = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(350, 47), Size = new Size(180, 25) };

            // الأزرار
            btnSave = new Button { Text = "💾 حفظ", BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(150, 15), Size = new Size(100, 35) };
            btnSave.Click += BtnSave_Click;

            btnDelete = new Button { Text = "🗑️ حذف", BackColor = Color.FromArgb(244, 67, 54), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(150, 55), Size = new Size(100, 35) };
            btnDelete.Click += BtnDelete_Click;

            btnClear = new Button { Text = "🔄 جديد / مسح", BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(10, 15), Size = new Size(120, 35) };
            btnClear.Click += (s, ev) => ClearFields();

            pnlInput.Controls.AddRange(new Control[] { lblGroupNo, txtGroupNo, lblGroupName, txtGroupName, btnSave, btnDelete, btnClear });

            // جدول العرض السفلي
            dgvPortGroups = new DataGridView();
            dgvPortGroups.Dock = DockStyle.Fill;
            dgvPortGroups.BackgroundColor = Color.White;
            dgvPortGroups.BorderStyle = BorderStyle.Fixed3D;
            dgvPortGroups.RowHeadersVisible = false;
            dgvPortGroups.AllowUserToAddRows = false;
            dgvPortGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPortGroups.EnableHeadersVisualStyles = false;
            dgvPortGroups.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvPortGroups.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPortGroups.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPortGroups.ColumnHeadersHeight = 40;
            dgvPortGroups.RowTemplate.Height = 35;
            dgvPortGroups.SelectionChanged += DgvPortGroups_SelectionChanged;

            // إضافة الأعمدة للجدول
            dgvPortGroups.Columns.Add("colId", "م");
            dgvPortGroups.Columns["colId"].Visible = false; // إخفاء المعرف الداخلي
            dgvPortGroups.Columns.Add("colGroupNo", "رقم مجموعة المنافذ");
            dgvPortGroups.Columns.Add("colGroupName", "اسم مجموعة المنافذ");

            // ضبط عرض الأعمدة
            dgvPortGroups.Columns["colGroupNo"].Width = 200;
            dgvPortGroups.Columns["colGroupName"].Width = 400;

            this.Controls.Add(dgvPortGroups);
            this.Controls.Add(pnlInput);
        }

        // 3. تحميل البيانات من قاعدة البيانات
        private void LoadData()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT Id, GroupNo, GroupName FROM PortGroups ORDER BY Id DESC");
                dgvPortGroups.Rows.Clear();

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        dgvPortGroups.Rows.Add(
                            row["Id"].ToString(),
                            row["GroupNo"].ToString(),
                            row["GroupName"].ToString()
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 4. حدث اختيار صف من الجدول (للتعديل)
        private void DgvPortGroups_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPortGroups.CurrentRow != null && dgvPortGroups.CurrentRow.Index >= 0)
            {
                currentEditingId = Convert.ToInt32(dgvPortGroups.CurrentRow.Cells["colId"].Value);
                txtGroupNo.Text = dgvPortGroups.CurrentRow.Cells["colGroupNo"].Value.ToString();
                txtGroupName.Text = dgvPortGroups.CurrentRow.Cells["colGroupName"].Value.ToString();
            }
        }

        // 5. حفظ البيانات (إضافة أو تعديل)
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtGroupNo.Text) || string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال رقم واسم مجموعة المنافذ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                if (currentEditingId.HasValue)
                {
                    // تحديث
                    DatabaseHelper.ExecuteNonQuery(@"UPDATE PortGroups SET GroupNo = @GroupNo, GroupName = @GroupName WHERE Id = @Id",
                        DatabaseHelper.CreateParameter("@GroupNo", txtGroupNo.Text.Trim()),
                        DatabaseHelper.CreateParameter("@GroupName", txtGroupName.Text.Trim()),
                        DatabaseHelper.CreateParameter("@Id", currentEditingId.Value));

                    MessageBox.Show("✅ تم تحديث البيانات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // إضافة جديد
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO PortGroups (GroupNo, GroupName) VALUES (@GroupNo, @GroupName)",
                        DatabaseHelper.CreateParameter("@GroupNo", txtGroupNo.Text.Trim()),
                        DatabaseHelper.CreateParameter("@GroupName", txtGroupName.Text.Trim()));

                    MessageBox.Show("✅ تم إضافة مجموعة المنافذ بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadData();
                ClearFields();
            }
            catch (SQLiteException ex) when (ex.Message.Contains("UNIQUE constraint failed"))
            {
                MessageBox.Show("⚠️ رقم مجموعة المنافذ هذا موجود مسبقاً، الرجاء استخدام رقم مختلف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الحفظ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 6. حذف البيانات
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (!currentEditingId.HasValue)
            {
                MessageBox.Show("⚠️ الرجاء اختيار سجل من الجدول لحذفه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("هل أنت متأكد من حذف هذه المجموعة؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM PortGroups WHERE Id = @Id",
                        DatabaseHelper.CreateParameter("@Id", currentEditingId.Value));

                    MessageBox.Show("✅ تم الحذف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    ClearFields();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // 7. مسح الحقول للبدء بإدخال جديد
        private void ClearFields()
        {
            currentEditingId = null;
            txtGroupNo.Clear();
            txtGroupName.Clear();
            dgvPortGroups.ClearSelection();
            txtGroupNo.Focus();
        }
    }
}