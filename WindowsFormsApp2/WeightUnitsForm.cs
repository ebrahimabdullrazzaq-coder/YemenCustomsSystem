using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace YemenCustomsSystem
{
    public class WeightUnitsForm : Form
    {
        private List<WeightUnitData> weightUnits = new List<WeightUnitData>();
        private DataGridView dgvWeightUnits;
        private TextBox txtUnitName;
        private TextBox txtWeight;
        private Button btnSave;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private int currentIndex = -1;

        // ✅ متغير لإدارة حالة التعديل
        private bool isEditingMode = false;

        public class WeightUnitData
        {
            public string UnitName { get; set; }
            public decimal Weight { get; set; }
        }

        public WeightUnitData SelectedWeightUnit { get; private set; }

        public WeightUnitsForm()
        {
            SetupForm();
            LoadWeightUnitsFromDatabase(); // ✅ تحميل البيانات من قاعدة البيانات
            SetFieldsReadOnly(true);       // ✅ قفل الحقول افتراضياً
        }

        private void SetupForm()
        {
            this.Text = "⚖️ تهيئة وحدات الأوزان";
            this.Size = new Size(700, 600);
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
            btnRefresh.Click += (s, e) => LoadWeightUnitsFromDatabase();

            toolStrip.Items.Add(btnHome);
            toolStrip.Items.Add(new ToolStripSeparator());
            toolStrip.Items.Add(btnRefresh);

            // ✅ لوحة الإدخال
            Panel pnlInput = new Panel();
            pnlInput.Dock = DockStyle.Top;
            pnlInput.Height = 100;
            pnlInput.BackColor = Color.FromArgb(240, 248, 255);
            pnlInput.Padding = new Padding(15);

            Label lblName = new Label();
            lblName.Text = "اسم الوحدة:";
            lblName.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblName.Location = new Point(450, 20);
            lblName.Size = new Size(100, 25);
            lblName.TextAlign = ContentAlignment.MiddleRight;

            txtUnitName = new TextBox();
            txtUnitName.Font = new Font("Segoe UI", 10);
            txtUnitName.Location = new Point(280, 20);
            txtUnitName.Size = new Size(160, 25);

            Label lblWeight = new Label();
            lblWeight.Text = "الوزن:";
            lblWeight.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblWeight.Location = new Point(200, 20);
            lblWeight.Size = new Size(70, 25);
            lblWeight.TextAlign = ContentAlignment.MiddleRight;

            txtWeight = new TextBox();
            txtWeight.Font = new Font("Segoe UI", 10);
            txtWeight.Location = new Point(100, 20);
            txtWeight.Size = new Size(90, 25);

            btnSave = new Button();
            btnSave.Text = "💾 حفظ";
            btnSave.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnSave.BackColor = Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Location = new Point(480, 55);
            btnSave.Size = new Size(100, 35);
            btnSave.Click += BtnSave_Click;

            btnNew = new Button();
            btnNew.Text = "🆕 جديد";
            btnNew.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnNew.BackColor = Color.FromArgb(52, 152, 219);
            btnNew.ForeColor = Color.White;
            btnNew.FlatStyle = FlatStyle.Flat;
            btnNew.Location = new Point(370, 55);
            btnNew.Size = new Size(100, 35);
            btnNew.Click += BtnNew_Click;

            btnEdit = new Button();
            btnEdit.Text = "✏️ تعديل";
            btnEdit.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnEdit.BackColor = Color.FromArgb(155, 89, 182);
            btnEdit.ForeColor = Color.White;
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Location = new Point(260, 55);
            btnEdit.Size = new Size(100, 35);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new Button();
            btnDelete.Text = "🗑️ حذف";
            btnDelete.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnDelete.BackColor = Color.FromArgb(231, 76, 60);
            btnDelete.ForeColor = Color.White;
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(150, 55);
            btnDelete.Size = new Size(100, 35);
            btnDelete.Click += BtnDelete_Click;

            pnlInput.Controls.AddRange(new Control[] { lblName, txtUnitName, lblWeight, txtWeight, btnSave, btnNew, btnEdit, btnDelete });

            // ✅ الجدول
            dgvWeightUnits = new DataGridView();
            dgvWeightUnits.Dock = DockStyle.Fill;
            dgvWeightUnits.ReadOnly = true;
            dgvWeightUnits.AllowUserToAddRows = false;
            dgvWeightUnits.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvWeightUnits.MultiSelect = false;
            dgvWeightUnits.BackgroundColor = Color.White;
            dgvWeightUnits.RowHeadersVisible = false;
            dgvWeightUnits.EnableHeadersVisualStyles = false;
            dgvWeightUnits.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvWeightUnits.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvWeightUnits.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvWeightUnits.ColumnHeadersHeight = 40;
            dgvWeightUnits.RowTemplate.Height = 35;
            dgvWeightUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvWeightUnits.Columns.Add("colName", "اسم الوحدة");
            dgvWeightUnits.Columns.Add("colWeight", "الوزن");

            dgvWeightUnits.CellClick += DgvWeightUnits_CellClick;
            dgvWeightUnits.CellDoubleClick += DgvWeightUnits_CellDoubleClick;

            this.Controls.Add(dgvWeightUnits);
            this.Controls.Add(pnlInput);
            this.Controls.Add(toolStrip);
        }

        private void SetFieldsReadOnly(bool readOnly)
        {
            isEditingMode = !readOnly;

            txtUnitName.ReadOnly = readOnly;
            txtWeight.ReadOnly = readOnly;

            Color bgColor = readOnly ? Color.White : Color.FromArgb(255, 255, 224);
            txtUnitName.BackColor = bgColor;
            txtWeight.BackColor = bgColor;

            btnSave.Enabled = !readOnly;
            btnEdit.Enabled = readOnly;
            btnNew.Enabled = true;
            btnDelete.Enabled = readOnly && currentIndex >= 0;
        }

        // ==================== ✅ تحميل البيانات من قاعدة البيانات ====================
        private void LoadWeightUnitsFromDatabase()
        {
            weightUnits.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT UnitName, Weight FROM WeightUnits ORDER BY UnitName");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        weightUnits.Add(new WeightUnitData
                        {
                            UnitName = row["UnitName"].ToString(),
                            Weight = Convert.ToDecimal(row["Weight"])
                        });
                    }
                }
                else
                {
                    // إذا كانت القاعدة فارغة، نضيف بيانات افتراضية
                    LoadDefaultWeightUnitsToDatabase();
                    LoadWeightUnitsFromDatabase(); // إعادة التحميل بعد الإضافة
                }
                RefreshGrid();
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ في تحميل وحدات الأوزان:\n{ex.Message}", "خطأ"); }
        }

        private void LoadDefaultWeightUnitsToDatabase()
        {
            var defaults = new List<WeightUnitData>
            {
                new WeightUnitData { UnitName = "كرتون", Weight = 15.00m },
                new WeightUnitData { UnitName = "قطعة", Weight = 0.50m },
                new WeightUnitData { UnitName = "كجم", Weight = 1.00m },
                new WeightUnitData { UnitName = "لتر", Weight = 1.00m },
                new WeightUnitData { UnitName = "علبة", Weight = 0.25m }
            };

            foreach (var unit in defaults)
            {
                SaveWeightUnitToDatabase(unit, false);
                weightUnits.Add(unit);
            }
        }

        private void SaveWeightUnitToDatabase(WeightUnitData unit, bool isUpdate)
        {
            try
            {
                if (isUpdate)
                {
                    DatabaseHelper.ExecuteNonQuery(@"UPDATE WeightUnits SET 
                        Weight = @Weight 
                        WHERE UnitName = @UnitName",
                        DatabaseHelper.CreateParameter("@Weight", unit.Weight),
                        DatabaseHelper.CreateParameter("@UnitName", unit.UnitName)
                    );
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO WeightUnits (UnitName, Weight) 
                        VALUES (@UnitName, @Weight)",
                        DatabaseHelper.CreateParameter("@UnitName", unit.UnitName),
                        DatabaseHelper.CreateParameter("@Weight", unit.Weight)
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ وحدة الوزن:\n{ex.Message}", "خطأ");
                throw;
            }
        }

        private void RefreshGrid()
        {
            dgvWeightUnits.Rows.Clear();
            foreach (var unit in weightUnits)
            {
                dgvWeightUnits.Rows.Add(unit.UnitName, unit.Weight.ToString("N2"));
            }
        }

        private void DgvWeightUnits_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                currentIndex = e.RowIndex;
                var unit = weightUnits[currentIndex];
                txtUnitName.Text = unit.UnitName;
                txtWeight.Text = unit.Weight.ToString("N2");

                btnDelete.Enabled = !isEditingMode && currentIndex >= 0;
            }
        }

        private void DgvWeightUnits_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                SelectedWeightUnit = weightUnits[e.RowIndex];
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            txtUnitName.Clear();
            txtWeight.Clear();
            currentIndex = -1;
            dgvWeightUnits.ClearSelection();
            SetFieldsReadOnly(false);
            txtUnitName.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار وحدة للتعديل أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetFieldsReadOnly(false);
            txtWeight.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUnitName.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال اسم الوحدة", "تنبيه");
                return;
            }

            if (!decimal.TryParse(txtWeight.Text, out decimal weight))
            {
                MessageBox.Show("⚠️ الرجاء إدخال وزن صحيح", "تنبيه");
                return;
            }

            var unitToSave = new WeightUnitData
            {
                UnitName = txtUnitName.Text.Trim(),
                Weight = weight
            };

            bool isUpdate = weightUnits.Any(u => u.UnitName == unitToSave.UnitName);

            try
            {
                SaveWeightUnitToDatabase(unitToSave, isUpdate);

                if (isUpdate)
                {
                    var existing = weightUnits.First(u => u.UnitName == unitToSave.UnitName);
                    existing.Weight = unitToSave.Weight;
                    MessageBox.Show("✅ تم تحديث وحدة الوزن بنجاح", "نجاح");
                }
                else
                {
                    weightUnits.Add(unitToSave);
                    MessageBox.Show("✅ تمت إضافة وحدة الوزن بنجاح", "نجاح");
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

            var unitToDelete = weightUnits[currentIndex];
            if (MessageBox.Show($"🗑️ هل أنت متأكد من حذف الوحدة '{unitToDelete.UnitName}'؟", "تأكيد",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM WeightUnits WHERE UnitName = @UnitName",
                        DatabaseHelper.CreateParameter("@UnitName", unitToDelete.UnitName));

                    weightUnits.RemoveAt(currentIndex);
                    RefreshGrid();

                    txtUnitName.Clear();
                    txtWeight.Clear();
                    currentIndex = -1;
                    dgvWeightUnits.ClearSelection();

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