using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace YemenCustomsSystem
{
    public class UnitsForm : Form
    {
        private List<UnitData> units = new List<UnitData>();
        private DataGridView dgvUnits;
        private TextBox txtUnitCode;
        private TextBox txtUnitName;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private int currentIndex = -1;

        // ✅ متغير لإدارة حالة التعديل
        private bool isEditingMode = false;

        public class UnitData
        {
            public string UnitCode { get; set; }
            public string UnitName { get; set; }
            public bool IsActive { get; set; }
        }

        public UnitData SelectedUnit { get; private set; }

        public UnitsForm()
        {
            SetupForm();
            LoadUnitsFromDatabase(); // ✅ تحميل البيانات من قاعدة البيانات
            SetFieldsReadOnly(true); // ✅ قفل الحقول افتراضياً
        }

        private void SetupForm()
        {
            this.Text = "📏 تهيئة وحدات القياس";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;

            // ✅ شريط الأدوات
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Dock = DockStyle.Top;
            toolStrip.BackColor = Color.FromArgb(52, 73, 94);

            ToolStripButton btnHome = new ToolStripButton();
            btnHome.Text = "🏠 الرئيسية";
            btnHome.ForeColor = Color.White;
            btnHome.Click += (s, e) => this.Close();

            ToolStripButton btnRefresh = new ToolStripButton();
            btnRefresh.Text = "🔄 تحديث";
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Click += (s, e) => LoadUnitsFromDatabase();

            toolStrip.Items.Add(btnHome);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnRefresh);

            // ✅ لوحة الإدخال
            Panel pnlInput = new Panel();
            pnlInput.Dock = DockStyle.Top;
            pnlInput.Height = 100;
            pnlInput.BackColor = Color.FromArgb(240, 248, 255);
            pnlInput.Padding = new Padding(15);

            Label lblCode = new Label();
            lblCode.Text = "رمز الوحدة:";
            lblCode.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCode.Location = new Point(550, 20);
            lblCode.Size = new Size(100, 25);
            lblCode.TextAlign = ContentAlignment.MiddleRight;

            txtUnitCode = new TextBox();
            txtUnitCode.Font = new Font("Segoe UI", 10);
            txtUnitCode.Location = new Point(420, 20);
            txtUnitCode.Size = new Size(120, 25);

            Label lblName = new Label();
            lblName.Text = "اسم الوحدة:";
            lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblName.Location = new Point(300, 20);
            lblName.Size = new Size(100, 25);
            lblName.TextAlign = ContentAlignment.MiddleRight;

            txtUnitName = new TextBox();
            txtUnitName.Font = new Font("Segoe UI", 10);
            txtUnitName.Location = new Point(150, 20);
            txtUnitName.Size = new Size(140, 25);

            chkIsActive = new CheckBox();
            chkIsActive.Text = "نشط";
            chkIsActive.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            chkIsActive.Location = new Point(50, 20);
            chkIsActive.Size = new Size(80, 25);
            chkIsActive.Checked = true;

            btnSave = new Button();
            btnSave.Text = "💾 حفظ";
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Location = new Point(530, 55);
            btnSave.Size = new Size(100, 35);
            btnSave.Click += BtnSave_Click;

            btnNew = new Button();
            btnNew.Text = "🆕 جديد";
            btnNew.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnNew.BackColor = Color.FromArgb(52, 152, 219);
            btnNew.ForeColor = Color.White;
            btnNew.FlatStyle = FlatStyle.Flat;
            btnNew.Location = new Point(420, 55);
            btnNew.Size = new Size(100, 35);
            btnNew.Click += BtnNew_Click;

            btnEdit = new Button();
            btnEdit.Text = "✏️ تعديل";
            btnEdit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEdit.BackColor = Color.FromArgb(155, 89, 182);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Location = new Point(310, 55);
            btnEdit.Size = new Size(100, 35);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "🗑️ حذف";
            btnDelete.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(200, 55);
            btnDelete.Size = new Size(100, 35);
            btnDelete.Click += BtnDelete_Click;

            pnlInput.Controls.AddRange(new Control[] { lblCode, txtUnitCode, lblName, txtUnitName, chkIsActive, btnSave, btnNew, btnEdit, btnDelete });

            // ✅ الجدول
            dgvUnits = new DataGridView();
            dgvUnits.Dock = DockStyle.Fill;
            dgvUnits.ReadOnly = true;
            dgvUnits.AllowUserToAddRows = false;
            dgvUnits.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnits.MultiSelect = false;
            dgvUnits.BackgroundColor = Color.White;
            dgvUnits.RowHeadersVisible = false;
            dgvUnits.EnableHeadersVisualStyles = false;
            dgvUnits.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvUnits.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUnits.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvUnits.ColumnHeadersHeight = 40;
            dgvUnits.RowTemplate.Height = 35;
            dgvUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvUnits.Columns.Add("colCode", "رمز الوحدة");
            dgvUnits.Columns.Add("colName", "اسم الوحدة");
            dgvUnits.Columns.Add("colStatus", "الحالة");

            dgvUnits.CellClick += DgvUnits_CellClick;
            dgvUnits.CellDoubleClick += DgvUnits_CellDoubleClick;

            this.Controls.Add(dgvUnits);
            this.Controls.Add(pnlInput);
            this.Controls.Add(toolStrip);
        }

        private void SetFieldsReadOnly(bool readOnly)
        {
            isEditingMode = !readOnly;

            txtUnitCode.ReadOnly = readOnly;
            txtUnitName.ReadOnly = readOnly;
            chkIsActive.Enabled = !readOnly;

            Color bgColor = readOnly ? Color.White : Color.FromArgb(255, 255, 224);
            txtUnitCode.BackColor = bgColor;
            txtUnitName.BackColor = bgColor;

            btnSave.Enabled = !readOnly;
            btnEdit.Enabled = readOnly;
            btnNew.Enabled = true;
            btnDelete.Enabled = readOnly && currentIndex >= 0;
        }

        // ==================== ✅ تحميل البيانات من قاعدة البيانات ====================
        private void LoadUnitsFromDatabase()
        {
            units.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT UnitCode, UnitName, IsActive FROM MeasurementUnits ORDER BY UnitCode");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        units.Add(new UnitData
                        {
                            UnitCode = row["UnitCode"].ToString(),
                            UnitName = row["UnitName"].ToString(),
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }
                }
                else
                {
                    // إذا كانت القاعدة فارغة، نضيف بيانات افتراضية
                    LoadDefaultUnitsToDatabase();
                    LoadUnitsFromDatabase(); // إعادة التحميل بعد الإضافة
                }
                RefreshGrid();
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ في تحميل الوحدات:\n{ex.Message}", "خطأ"); }
        }

        private void LoadDefaultUnitsToDatabase()
        {
            var defaults = new List<UnitData>
            {
                new UnitData { UnitCode = "1", UnitName = "قطعة", IsActive = true },
                new UnitData { UnitCode = "2", UnitName = "كجم", IsActive = true },
                new UnitData { UnitCode = "3", UnitName = "لتر", IsActive = true },
                new UnitData { UnitCode = "4", UnitName = "علبة", IsActive = true },
                new UnitData { UnitCode = "5", UnitName = "كرتون", IsActive = true }
            };

            foreach (var unit in defaults)
            {
                SaveUnitToDatabase(unit, false);
                units.Add(unit);
            }
        }

        private void SaveUnitToDatabase(UnitData unit, bool isUpdate)
        {
            try
            {
                if (isUpdate)
                {
                    DatabaseHelper.ExecuteNonQuery(@"UPDATE MeasurementUnits SET 
                        UnitName = @Name, IsActive = @IsActive 
                        WHERE UnitCode = @Code",
                        DatabaseHelper.CreateParameter("@Name", unit.UnitName),
                        DatabaseHelper.CreateParameter("@IsActive", unit.IsActive),
                        DatabaseHelper.CreateParameter("@Code", unit.UnitCode)
                    );
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO MeasurementUnits (UnitCode, UnitName, IsActive) 
                        VALUES (@Code, @Name, @IsActive)",
                        DatabaseHelper.CreateParameter("@Code", unit.UnitCode),
                        DatabaseHelper.CreateParameter("@Name", unit.UnitName),
                        DatabaseHelper.CreateParameter("@IsActive", unit.IsActive)
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ الوحدة:\n{ex.Message}", "خطأ");
                throw;
            }
        }

        private void RefreshGrid()
        {
            dgvUnits.Rows.Clear();
            foreach (var unit in units)
            {
                dgvUnits.Rows.Add(unit.UnitCode, unit.UnitName, unit.IsActive ? "✅ نشط" : "❌ معطل");
            }
        }

        private void DgvUnits_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                currentIndex = e.RowIndex;
                var unit = units[currentIndex];
                txtUnitCode.Text = unit.UnitCode;
                txtUnitName.Text = unit.UnitName;
                chkIsActive.Checked = unit.IsActive;

                btnDelete.Enabled = !isEditingMode && currentIndex >= 0;
            }
        }

        private void DgvUnits_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectedUnit = units[e.RowIndex];
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            txtUnitCode.Clear();
            txtUnitName.Clear();
            chkIsActive.Checked = true;
            currentIndex = -1;
            dgvUnits.ClearSelection();
            SetFieldsReadOnly(false);
            txtUnitCode.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار وحدة للتعديل أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetFieldsReadOnly(false);
            txtUnitName.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUnitCode.Text) || string.IsNullOrWhiteSpace(txtUnitName.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال رمز واسم الوحدة", "تنبيه");
                return;
            }

            var unitToSave = new UnitData
            {
                UnitCode = txtUnitCode.Text.Trim(),
                UnitName = txtUnitName.Text.Trim(),
                IsActive = chkIsActive.Checked
            };

            bool isUpdate = units.Any(u => u.UnitCode == unitToSave.UnitCode);

            try
            {
                SaveUnitToDatabase(unitToSave, isUpdate);

                if (isUpdate)
                {
                    var existing = units.First(u => u.UnitCode == unitToSave.UnitCode);
                    existing.UnitName = unitToSave.UnitName;
                    existing.IsActive = unitToSave.IsActive;
                    MessageBox.Show("✅ تم تحديث الوحدة بنجاح", "نجاح");
                }
                else
                {
                    units.Add(unitToSave);
                    MessageBox.Show("✅ تمت إضافة الوحدة بنجاح", "نجاح");
                }

                RefreshGrid();
                SetFieldsReadOnly(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ: {ex.Message}", "خطأ");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار وحدة للحذف", "تنبيه");
                return;
            }

            var unitToDelete = units[currentIndex];
            if (MessageBox.Show($"🗑️ هل أنت متأكد من حذف الوحدة '{unitToDelete.UnitName}'؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM MeasurementUnits WHERE UnitCode = @Code",
                        DatabaseHelper.CreateParameter("@Code", unitToDelete.UnitCode));

                    units.RemoveAt(currentIndex);
                    RefreshGrid();

                    txtUnitCode.Clear();
                    txtUnitName.Clear();
                    chkIsActive.Checked = true;
                    currentIndex = -1;
                    dgvUnits.ClearSelection();

                    MessageBox.Show("✅ تم الحذف بنجاح", "نجاح");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في الحذف:\n{ex.Message}", "خطأ");
                }
            }
        }
    }
}