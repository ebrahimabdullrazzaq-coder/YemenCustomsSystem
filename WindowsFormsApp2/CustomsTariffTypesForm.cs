using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class CustomsTariffTypesForm : Form
    {
        private List<TariffTypeData> tariffTypes = new List<TariffTypeData>();
        private int currentIndex = -1;
        private bool isEditingMode = false;
        private bool hasChanges = false;

        private ToolStrip toolStrip1;
        private ToolStripButton btnHome, btnNew, btnSave, btnEdit, btnDelete, btnSearch, btnView, btnExit;
        private DataGridView dgvData;
        private TextBox txtTypeCode, txtTypeName, txtTypeNameAr, txtDescription;
        private ComboBox cmbIsActive;

        public CustomsTariffTypesForm()
        {
            InitializeComponent();
            LoadDataFromDatabase();
            ClearFields();
            SetControlsState(false);
            ApplyTranslations();
            LanguageManager.LanguageChanged += (s, e) => ApplyTranslations();
        }

        private void InitializeComponent()
        {
            this.Text = "أنواع الترسيم الجمركي - نظام الجمارك اليمنية";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // شريط الأدوات
            toolStrip1 = new ToolStrip();
            toolStrip1.Dock = DockStyle.Top;
            toolStrip1.BackColor = Color.FromArgb(52, 73, 94);
            toolStrip1.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            btnHome = new ToolStripButton("🏠 الرئيسية");
            btnHome.ForeColor = Color.White;
            btnHome.Click += BtnHome_Click;

            btnNew = new ToolStripButton("🆕 جديد");
            btnNew.ForeColor = Color.White;
            btnNew.Click += BtnNew_Click;

            btnSave = new ToolStripButton("💾 حفظ");
            btnSave.ForeColor = Color.White;
            btnSave.Click += BtnSave_Click;

            btnEdit = new ToolStripButton("✏️ تعديل");
            btnEdit.ForeColor = Color.White;
            btnEdit.Click += BtnEdit_Click;

            btnDelete = new ToolStripButton("🗑️ حذف");
            btnDelete.ForeColor = Color.White;
            btnDelete.Click += BtnDelete_Click;

            btnSearch = new ToolStripButton("🔍 بحث");
            btnSearch.ForeColor = Color.White;
            btnSearch.Click += BtnSearch_Click;

            btnView = new ToolStripButton("️ عرض");
            btnView.ForeColor = Color.White;
            btnView.Click += BtnView_Click;

            btnExit = new ToolStripButton(" خروج");
            btnExit.ForeColor = Color.White;
            btnExit.Click += (s, e) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] {
                btnHome, btnNew, btnSave, btnEdit, btnDelete,
                new ToolStripSeparator(),
                btnSearch, btnView,
                new ToolStripSeparator(),
                btnExit
            });

            // مجموعة البيانات
            GroupBox grpData = new GroupBox();
            grpData.Text = "بيانات نوع الترسيم";
            grpData.Dock = DockStyle.Top;
            grpData.Height = 140;
            grpData.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpData.ForeColor = Color.FromArgb(60, 60, 60);
            grpData.BackColor = Color.FromArgb(250, 250, 250);

            Label lblTypeCode = new Label();
            lblTypeCode.Text = "رمز النوع:";
            lblTypeCode.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTypeCode.ForeColor = Color.FromArgb(51, 122, 183);
            lblTypeCode.Location = new Point(950, 30);
            lblTypeCode.Size = new Size(80, 25);
            lblTypeCode.TextAlign = ContentAlignment.MiddleRight;

            txtTypeCode = new TextBox();
            txtTypeCode.Font = new Font("Segoe UI", 9F);
            txtTypeCode.Location = new Point(780, 30);
            txtTypeCode.Size = new Size(160, 25);
            txtTypeCode.BackColor = Color.FromArgb(255, 255, 190);

            Label lblTypeNameAr = new Label();
            lblTypeNameAr.Text = "الاسم بالعربي:";
            lblTypeNameAr.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTypeNameAr.ForeColor = Color.FromArgb(51, 122, 183);
            lblTypeNameAr.Location = new Point(650, 30);
            lblTypeNameAr.Size = new Size(100, 25);
            lblTypeNameAr.TextAlign = ContentAlignment.MiddleRight;

            txtTypeNameAr = new TextBox();
            txtTypeNameAr.Font = new Font("Segoe UI", 9F);
            txtTypeNameAr.Location = new Point(450, 30);
            txtTypeNameAr.Size = new Size(190, 25);
            txtTypeNameAr.BackColor = Color.FromArgb(255, 255, 190);

            Label lblTypeName = new Label();
            lblTypeName.Text = "الاسم بالإنجليزي:";
            lblTypeName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblTypeName.ForeColor = Color.FromArgb(51, 122, 183);
            lblTypeName.Location = new Point(950, 65);
            lblTypeName.Size = new Size(110, 25);
            lblTypeName.TextAlign = ContentAlignment.MiddleRight;

            txtTypeName = new TextBox();
            txtTypeName.Font = new Font("Segoe UI", 9F);
            txtTypeName.Location = new Point(780, 65);
            txtTypeName.Size = new Size(160, 25);

            Label lblDescription = new Label();
            lblDescription.Text = "الوصف:";
            lblDescription.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblDescription.ForeColor = Color.FromArgb(51, 122, 183);
            lblDescription.Location = new Point(650, 65);
            lblDescription.Size = new Size(60, 25);
            lblDescription.TextAlign = ContentAlignment.MiddleRight;

            txtDescription = new TextBox();
            txtDescription.Font = new Font("Segoe UI", 9F);
            txtDescription.Location = new Point(450, 65);
            txtDescription.Size = new Size(190, 25);

            Label lblIsActive = new Label();
            lblIsActive.Text = "الحالة:";
            lblIsActive.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblIsActive.ForeColor = Color.FromArgb(51, 122, 183);
            lblIsActive.Location = new Point(950, 100);
            lblIsActive.Size = new Size(60, 25);
            lblIsActive.TextAlign = ContentAlignment.MiddleRight;

            cmbIsActive = new ComboBox();
            cmbIsActive.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIsActive.Font = new Font("Segoe UI", 9F);
            cmbIsActive.Items.AddRange(new object[] { "نشط", "غير نشط" });
            cmbIsActive.Location = new Point(780, 100);
            cmbIsActive.Size = new Size(160, 25);
            cmbIsActive.SelectedIndex = 0;

            grpData.Controls.AddRange(new Control[] {
                lblTypeCode, txtTypeCode,
                lblTypeNameAr, txtTypeNameAr,
                lblTypeName, txtTypeName,
                lblDescription, txtDescription,
                lblIsActive, cmbIsActive
            });

            // الجدول
            dgvData = new DataGridView();
            dgvData.Dock = DockStyle.Fill;
            dgvData.AllowUserToAddRows = false;
            dgvData.AllowUserToDeleteRows = false;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.Fixed3D;
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.MultiSelect = false;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.ColumnHeadersHeight = 40;
            dgvData.RowTemplate.Height = 35;
            dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            dgvData.Columns.Add("colNum", "م");
            dgvData.Columns.Add("colTypeCode", "رمز النوع");
            dgvData.Columns.Add("colTypeNameAr", "الاسم بالعربي");
            dgvData.Columns.Add("colTypeName", "الاسم بالإنجليزي");
            dgvData.Columns.Add("colDescription", "الوصف");
            dgvData.Columns.Add("colStatus", "الحالة");

            dgvData.Columns["colNum"].Width = 50;
            dgvData.Columns["colStatus"].DefaultCellStyle.ForeColor = Color.Green;
            dgvData.Columns["colStatus"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            dgvData.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) LoadSelectedRecord(); };

            this.Controls.Add(dgvData);
            this.Controls.Add(grpData);
            this.Controls.Add(toolStrip1);
        }

        public class TariffTypeData
        {
            public int Id { get; set; }
            public string TypeCode { get; set; }
            public string TypeNameAr { get; set; }
            public string TypeName { get; set; }
            public string Description { get; set; }
            public bool IsActive { get; set; }
        }

        private void LoadDataFromDatabase()
        {
            tariffTypes.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM CustomsTariffTypes ORDER BY TypeCode");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        tariffTypes.Add(new TariffTypeData
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            TypeCode = row["TypeCode"].ToString(),
                            TypeNameAr = row["TypeNameAr"].ToString(),
                            TypeName = row["TypeName"].ToString(),
                            Description = row["Description"]?.ToString() ?? "",
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }
                }
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحميل البيانات:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGrid()
        {
            dgvData.Rows.Clear();
            int idx = 1;
            foreach (var item in tariffTypes)
            {
                dgvData.Rows.Add(
                    idx++,
                    item.TypeCode,
                    item.TypeNameAr,
                    item.TypeName,
                    item.Description,
                    item.IsActive ? "نشط" : "غير نشط"
                );
            }
        }

        private void SetControlsState(bool enabled)
        {
            isEditingMode = enabled;
            txtTypeCode.ReadOnly = !enabled;
            txtTypeNameAr.ReadOnly = !enabled;
            txtTypeName.ReadOnly = !enabled;
            txtDescription.ReadOnly = !enabled;
            cmbIsActive.Enabled = enabled;

            Color bgColor = enabled ? Color.FromArgb(255, 255, 224) : Color.White;
            txtTypeCode.BackColor = bgColor;
            txtTypeNameAr.BackColor = bgColor;
        }

        private void ClearFields()
        {
            txtTypeCode.Clear();
            txtTypeNameAr.Clear();
            txtTypeName.Clear();
            txtDescription.Clear();
            cmbIsActive.SelectedIndex = 0;
            currentIndex = -1;
        }

        private void LoadSelectedRecord()
        {
            if (dgvData.SelectedRows.Count > 0)
            {
                int rowIndex = dgvData.SelectedRows[0].Index;
                if (rowIndex >= 0 && rowIndex < tariffTypes.Count)
                {
                    var selected = tariffTypes[rowIndex];
                    currentIndex = rowIndex;
                    txtTypeCode.Text = selected.TypeCode;
                    txtTypeNameAr.Text = selected.TypeNameAr;
                    txtTypeName.Text = selected.TypeName;
                    txtDescription.Text = selected.Description;
                    cmbIsActive.SelectedIndex = selected.IsActive ? 0 : 1;
                }
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtTypeCode.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال رمز النوع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTypeCode.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtTypeNameAr.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال الاسم بالعربي", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTypeNameAr.Focus();
                return false;
            }
            return true;
        }

        private void SaveData()
        {
            if (!ValidateFields()) return;

            try
            {
                var existing = tariffTypes.FirstOrDefault(t => t.TypeCode == txtTypeCode.Text && currentIndex != tariffTypes.IndexOf(t));
                if (existing != null)
                {
                    MessageBox.Show($"⚠️ رمز النوع '{txtTypeCode.Text}' موجود مسبقاً!", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var newData = new TariffTypeData
                {
                    TypeCode = txtTypeCode.Text.Trim(),
                    TypeNameAr = txtTypeNameAr.Text.Trim(),
                    TypeName = txtTypeName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    IsActive = cmbIsActive.SelectedIndex == 0
                };

                if (currentIndex >= 0 && currentIndex < tariffTypes.Count)
                {
                    // تعديل
                    newData.Id = tariffTypes[currentIndex].Id;
                    DatabaseHelper.ExecuteNonQuery(
                        "UPDATE CustomsTariffTypes SET TypeNameAr=@TypeNameAr, TypeName=@TypeName, Description=@Description, IsActive=@IsActive WHERE Id=@Id",
                        DatabaseHelper.CreateParameter("@TypeNameAr", newData.TypeNameAr),
                        DatabaseHelper.CreateParameter("@TypeName", newData.TypeName),
                        DatabaseHelper.CreateParameter("@Description", newData.Description),
                        DatabaseHelper.CreateParameter("@IsActive", newData.IsActive),
                        DatabaseHelper.CreateParameter("@Id", newData.Id)
                    );
                    tariffTypes[currentIndex] = newData;
                    MessageBox.Show("✅ تم التعديل بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // إضافة
                    DatabaseHelper.ExecuteNonQuery(
                        "INSERT INTO CustomsTariffTypes (TypeCode, TypeNameAr, TypeName, Description, IsActive) VALUES (@TypeCode, @TypeNameAr, @TypeName, @Description, @IsActive)",
                        DatabaseHelper.CreateParameter("@TypeCode", newData.TypeCode),
                        DatabaseHelper.CreateParameter("@TypeNameAr", newData.TypeNameAr),
                        DatabaseHelper.CreateParameter("@TypeName", newData.TypeName),
                        DatabaseHelper.CreateParameter("@Description", newData.Description),
                        DatabaseHelper.CreateParameter("@IsActive", newData.IsActive)
                    );

                    var lastId = DatabaseHelper.ExecuteScalar("SELECT last_insert_rowid()");
                    newData.Id = Convert.ToInt32(lastId);
                    tariffTypes.Add(newData);
                    currentIndex = tariffTypes.Count - 1;
                    MessageBox.Show("✅ تمت الإضافة بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                RefreshGrid();
                SetControlsState(false);
                hasChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الحفظ:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteRecord()
        {
            if (currentIndex < 0 || currentIndex >= tariffTypes.Count)
            {
                MessageBox.Show("⚠️ الرجاء اختيار سجل للحذف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selected = tariffTypes[currentIndex];
            if (MessageBox.Show($"🗑️ هل أنت متأكد من حذف '{selected.TypeNameAr}'؟", "تأكيد الحذف",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery(
                        "DELETE FROM CustomsTariffTypes WHERE Id=@Id",
                        DatabaseHelper.CreateParameter("@Id", selected.Id)
                    );
                    tariffTypes.RemoveAt(currentIndex);
                    RefreshGrid();
                    ClearFields();
                    MessageBox.Show("✅ تم الحذف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في الحذف:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ApplyTranslations()
        {
            this.Text = "أنواع الترسيم الجمركي - نظام الجمارك اليمنية";
            if (btnHome != null) btnHome.Text = LanguageManager.Get("btn_home");
            if (btnNew != null) btnNew.Text = LanguageManager.Get("btn_new");
            if (btnSave != null) btnSave.Text = LanguageManager.Get("btn_save");
            if (btnEdit != null) btnEdit.Text = LanguageManager.Get("btn_edit");
            if (btnDelete != null) btnDelete.Text = LanguageManager.Get("btn_delete");
            if (btnSearch != null) btnSearch.Text = LanguageManager.Get("btn_search");
            if (btnView != null) btnView.Text = LanguageManager.Get("btn_view");
            if (btnExit != null) btnExit.Text = LanguageManager.Get("btn_exit");
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            ClearFields();
            SetControlsState(true);
            txtTypeCode.Focus();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!isEditingMode)
            {
                MessageBox.Show("⚠️ الرجاء الضغط على 'جديد' أو 'تعديل' أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SaveData();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= tariffTypes.Count)
            {
                MessageBox.Show("⚠️ الرجاء اختيار سجل للتعديل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetControlsState(true);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string search = Microsoft.VisualBasic.Interaction.InputBox("أدخل رمز أو اسم النوع للبحث:", "بحث", "");
            if (!string.IsNullOrWhiteSpace(search))
            {
                var found = tariffTypes.FirstOrDefault(t =>
                    t.TypeCode.Contains(search) ||
                    t.TypeNameAr.Contains(search) ||
                    t.TypeName.Contains(search));

                if (found != null)
                {
                    currentIndex = tariffTypes.IndexOf(found);
                    LoadSelectedRecord();
                    dgvData.CurrentCell = dgvData.Rows[currentIndex].Cells[0];
                    dgvData.Rows[currentIndex].Selected = true;
                    MessageBox.Show($"✅ تم العثور على: {found.TypeNameAr}", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("❌ لم يتم العثور على النوع", "بحث", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void BtnView_Click(object sender, EventArgs e)
        {
            if (tariffTypes.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد بيانات للعرض", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form viewForm = new Form();
            viewForm.Text = $"📋 قائمة أنواع الترسيم ({tariffTypes.Count} نوع)";
            viewForm.Size = new Size(900, 600);
            viewForm.StartPosition = FormStartPosition.CenterParent;
            viewForm.RightToLeft = RightToLeft.Yes;
            viewForm.RightToLeftLayout = true;

            DataGridView dgvView = new DataGridView();
            dgvView.Dock = DockStyle.Fill;
            dgvView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvView.ReadOnly = true;
            dgvView.AllowUserToAddRows = false;
            dgvView.BackgroundColor = Color.White;
            dgvView.RowHeadersVisible = false;
            dgvView.EnableHeadersVisualStyles = false;
            dgvView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvView.ColumnHeadersHeight = 40;
            dgvView.RowTemplate.Height = 35;

            dgvView.Columns.Add("colNum", "م");
            dgvView.Columns.Add("colTypeCode", "رمز النوع");
            dgvView.Columns.Add("colTypeNameAr", "الاسم بالعربي");
            dgvView.Columns.Add("colTypeName", "الاسم بالإنجليزي");
            dgvView.Columns.Add("colDescription", "الوصف");
            dgvView.Columns.Add("colStatus", "الحالة");

            int idx = 1;
            foreach (var item in tariffTypes)
            {
                dgvView.Rows.Add(idx++, item.TypeCode, item.TypeNameAr, item.TypeName, item.Description, item.IsActive ? "نشط" : "غير نشط");
            }

            Button btnClose = new Button();
            btnClose.Text = "❌ إغلاق";
            btnClose.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnClose.BackColor = Color.FromArgb(158, 158, 158);
            btnClose.ForeColor = Color.White;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Size = new Size(120, 40);
            btnClose.Location = new Point(380, 10);
            btnClose.Click += (s, ev) => viewForm.Close();

            Panel pnlBottom = new Panel();
            pnlBottom.Height = 60;
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.BackColor = Color.FromArgb(240, 248, 255);
            pnlBottom.Controls.Add(btnClose);

            viewForm.Controls.Add(dgvView);
            viewForm.Controls.Add(pnlBottom);
            viewForm.ShowDialog();
        }

        // ==================== ✅ إنشاء الجدول إن لم يوجد ====================
        public static void EnsureTableExists()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS CustomsTariffTypes (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    TypeCode TEXT NOT NULL UNIQUE,
                    TypeNameAr TEXT NOT NULL,
                    TypeName TEXT,
                    Description TEXT,
                    IsActive INTEGER DEFAULT 1
                )");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ إنشاء جدول أنواع الترسيم: {ex.Message}");
            }
        }
    }
}