using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace YemenCustomsSystem
{
    public class CustomersBranchesForm : Form
    {
        private List<CustomerBranch> records = new List<CustomerBranch>();

        private DataGridView dgvData;
        private TextBox txtCustomerNo;
        private TextBox txtCustomerName;
        private ComboBox cmbType;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private TextBox txtEmail;
        private TextBox txtContactPerson;
        private TextBox txtNotes;
        private CheckBox chkIsActive;
        private Button btnSave;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnSearch;
        private Button btnExport;
        private Button btnClose;
        private TextBox txtSearch;
        private Label lblTotal;

        private int currentIndex = -1;
        private bool isEditingMode = false;
        private bool hasChanges = false;
        private CustomerBranch originalRecord = null;
        private int editingRecordId = 0; // ✅ متغير لتتبع الـ ID أثناء التعديل

        public class CustomerBranch
        {
            public int Id { get; set; }
            public string CustomerNo { get; set; }
            public string CustomerName { get; set; }
            public string Type { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string Email { get; set; }
            public string ContactPerson { get; set; }
            public string Notes { get; set; }
            public bool IsActive { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
        }

        public CustomersBranchesForm()
        {
            Console.WriteLine("🔧 جاري إنشاء CustomersBranchesForm...");
            InitializeControls();
            Console.WriteLine("✅ تم إنشاء العناصر");
            LoadDataFromDatabase();
            Console.WriteLine($"✅ تم تحميل البيانات - العدد: {records.Count}");
            ClearFields();
            SetControlsState(false);
            Console.WriteLine("✅ تم إنشاء النموذج بنجاح");
        }

        private void InitializeControls()
        {
            this.Text = " بيانات العملاء والفروع - نظام الجمارك اليمنية";
            this.Size = new Size(1400, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;

            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 60;
            pnlHeader.BackColor = Color.FromArgb(33, 150, 243);

            Label lblTitle = new Label();
            lblTitle.Text = "👥 بيانات العملاء والفروع";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
            lblTitle.Size = new Size(500, 35);
            pnlHeader.Controls.Add(lblTitle);

            Panel pnlData = new Panel();
            pnlData.Dock = DockStyle.Top;
            pnlData.Height = 220;
            pnlData.BackColor = Color.FromArgb(240, 248, 255);
            pnlData.Padding = new Padding(10);

            Label lblCustomerNo = CreateLabel("رقم العميل/الفرع:", 580, 20);
            txtCustomerNo = CreateTextBox(400, 20);
            txtCustomerNo.BackColor = Color.FromArgb(255, 255, 190);
            txtCustomerNo.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            Label lblCustomerName = CreateLabel("الاسم:", 580, 50);
            txtCustomerName = CreateTextBox(400, 50);
            txtCustomerName.BackColor = Color.FromArgb(255, 255, 190);
            txtCustomerName.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            Label lblType = CreateLabel("النوع:", 580, 80);
            cmbType = new ComboBox();
            cmbType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbType.Font = new Font("Segoe UI", 9);
            cmbType.Location = new Point(400, 80);
            cmbType.Size = new Size(170, 23);
            cmbType.Items.AddRange(new object[] { "عميل", "فرع", "شركة", "مؤسسة", "جهة حكومية" });

            Label lblPhone = CreateLabel("الهاتف:", 580, 110);
            txtPhone = CreateTextBox(400, 110);

            Label lblAddress = CreateLabel("العنوان:", 580, 140);
            txtAddress = CreateTextBox(400, 140);
            txtAddress.Size = new Size(350, 23);

            Label lblEmail = CreateLabel("البريد الإلكتروني:", 580, 170);
            txtEmail = CreateTextBox(400, 170);
            txtEmail.Size = new Size(350, 23);

            Label lblContactPerson = CreateLabel("شخص الاتصال:", 200, 20);
            txtContactPerson = CreateTextBox(20, 20);

            Label lblNotes = CreateLabel("ملاحظات:", 200, 50);
            txtNotes = CreateTextBox(20, 50);
            txtNotes.Multiline = true;
            txtNotes.Size = new Size(350, 110);

            chkIsActive = new CheckBox();
            chkIsActive.Text = "نشط";
            chkIsActive.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            chkIsActive.Location = new Point(20, 170);
            chkIsActive.Size = new Size(100, 25);
            chkIsActive.Checked = true;

            btnSave = CreateButton(" حفظ", 20, 195, Color.FromArgb(76, 175, 80));
            btnSave.Size = new Size(80, 30);
            btnSave.Click += BtnSave_Click;

            btnNew = CreateButton("🆕 جديد", 105, 195, Color.FromArgb(33, 150, 243));
            btnNew.Size = new Size(80, 30);
            btnNew.Click += BtnNew_Click;

            btnEdit = CreateButton("✏️ تعديل", 190, 195, Color.FromArgb(155, 89, 182));
            btnEdit.Size = new Size(80, 30);
            btnEdit.Click += BtnEdit_Click;

            btnDelete = CreateButton("🗑️ حذف", 275, 195, Color.FromArgb(244, 67, 54));
            btnDelete.Size = new Size(80, 30);
            btnDelete.Click += BtnDelete_Click;

            pnlData.Controls.AddRange(new Control[] {
                lblCustomerNo, txtCustomerNo, lblCustomerName, txtCustomerName,
                lblType, cmbType, lblPhone, txtPhone, lblAddress, txtAddress,
                lblEmail, txtEmail, lblContactPerson, txtContactPerson,
                lblNotes, txtNotes, chkIsActive,
                btnSave, btnNew, btnEdit, btnDelete
            });

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 40;
            pnlSearch.BackColor = Color.FromArgb(245, 245, 245);

            Label lblSearch = new Label();
            lblSearch.Text = "🔍 بحث:";
            lblSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSearch.Location = new Point(700, 10);
            lblSearch.Size = new Size(60, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(400, 10);
            txtSearch.Size = new Size(290, 25);
            txtSearch.TextChanged += TxtSearch_TextChanged;

            btnSearch = CreateButton("بحث", 300, 8, Color.FromArgb(33, 150, 243));
            btnSearch.Size = new Size(90, 28);
            btnSearch.Click += BtnSearch_Click;

            btnExport = CreateButton("📤 تصدير", 200, 8, Color.FromArgb(255, 152, 0));
            btnExport.Size = new Size(90, 28);
            btnExport.Click += BtnExport_Click;

            lblTotal = new Label();
            lblTotal.Text = " الإجمالي: 0";
            lblTotal.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblTotal.ForeColor = Color.FromArgb(76, 175, 80);
            lblTotal.Location = new Point(20, 10);
            lblTotal.Size = new Size(170, 25);
            lblTotal.TextAlign = ContentAlignment.MiddleRight;

            pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch, btnSearch, btnExport, lblTotal });

            dgvData = new DataGridView();
            dgvData.Dock = DockStyle.Fill;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.Fixed3D;
            dgvData.RowHeadersVisible = false;
            dgvData.AllowUserToAddRows = false;
            dgvData.AllowUserToDeleteRows = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.MultiSelect = false;
            dgvData.ReadOnly = true;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.ColumnHeadersHeight = 40;
            dgvData.RowTemplate.Height = 35;
            dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvData.Columns.Add("colId", "م");
            dgvData.Columns.Add("colCustomerNo", "رقم العميل/الفرع");
            dgvData.Columns.Add("colCustomerName", "الاسم");
            dgvData.Columns.Add("colType", "النوع");
            dgvData.Columns.Add("colPhone", "الهاتف");
            dgvData.Columns.Add("colAddress", "العنوان");
            dgvData.Columns.Add("colEmail", "البريد");
            dgvData.Columns.Add("colContactPerson", "شخص الاتصال");
            dgvData.Columns.Add("colStatus", "الحالة");

            dgvData.CellDoubleClick += DgvData_CellDoubleClick;

            dgvData.SelectionChanged += DgvData_SelectionChanged;

            Panel pnlBottom = new Panel();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 50;
            pnlBottom.BackColor = Color.FromArgb(240, 248, 255);

            btnClose = CreateButton("❌ إغلاق", 20, 10, Color.FromArgb(158, 158, 158));
            btnClose.Size = new Size(120, 35);
            btnClose.Click += (s, ev) => this.Close();

            pnlBottom.Controls.Add(btnClose);

            this.Controls.Add(dgvData);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlData);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlBottom);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lbl.ForeColor = Color.FromArgb(51, 122, 183);
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(170, 23);
            lbl.TextAlign = ContentAlignment.MiddleRight;
            return lbl;
        }

        private TextBox CreateTextBox(int x, int y)
        {
            TextBox txt = new TextBox();
            txt.Font = new Font("Segoe UI", 9);
            txt.Location = new Point(x, y);
            txt.Size = new Size(170, 23);
            txt.TextAlign = HorizontalAlignment.Right;
            return txt;
        }

        private Button CreateButton(string text, int x, int y, Color color)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btn.BackColor = color;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Location = new Point(x, y);
            btn.Size = new Size(100, 30);
            return btn;
        }

        private void SetControlsState(bool enabled)
        {
            isEditingMode = enabled;

            txtCustomerNo.ReadOnly = !enabled;
            txtCustomerName.ReadOnly = !enabled;
            cmbType.Enabled = enabled;
            txtPhone.ReadOnly = !enabled;
            txtAddress.ReadOnly = !enabled;
            txtEmail.ReadOnly = !enabled;
            txtContactPerson.ReadOnly = !enabled;
            txtNotes.ReadOnly = !enabled;
            chkIsActive.Enabled = enabled;

            Color bgColor = enabled ? Color.FromArgb(255, 255, 224) : Color.White;
            txtCustomerNo.BackColor = enabled ? bgColor : Color.FromArgb(255, 255, 190);
            txtCustomerName.BackColor = enabled ? bgColor : Color.FromArgb(255, 255, 190);
            txtPhone.BackColor = bgColor;
            txtAddress.BackColor = bgColor;
            txtEmail.BackColor = bgColor;
            txtContactPerson.BackColor = bgColor;
            txtNotes.BackColor = bgColor;

            btnNew.Enabled = true;
            btnEdit.Enabled = !enabled && currentIndex >= 0;
            btnDelete.Enabled = !enabled && currentIndex >= 0;
            btnSave.Enabled = enabled;
        }

        private void TrackChanges()
        {
            if (isEditingMode) hasChanges = true;
        }

        private void SaveOriginalRecord()
        {
            if (currentIndex >= 0 && currentIndex < records.Count)
            {
                originalRecord = new CustomerBranch
                {
                    Id = records[currentIndex].Id,
                    CustomerNo = records[currentIndex].CustomerNo,
                    CustomerName = records[currentIndex].CustomerName,
                    Type = records[currentIndex].Type,
                    Phone = records[currentIndex].Phone,
                    Address = records[currentIndex].Address,
                    Email = records[currentIndex].Email,
                    ContactPerson = records[currentIndex].ContactPerson,
                    Notes = records[currentIndex].Notes,
                    IsActive = records[currentIndex].IsActive,
                    CreatedDate = records[currentIndex].CreatedDate,
                    ModifiedDate = records[currentIndex].ModifiedDate
                };
            }
            else
            {
                originalRecord = null;
            }
            hasChanges = false;
        }

        private bool ValidateRequiredFields()
        {
            if (string.IsNullOrWhiteSpace(txtCustomerNo.Text))
            {
                MessageBox.Show("⚠️ رقم العميل/الفرع مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerNo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtCustomerName.Text))
            {
                MessageBox.Show("⚠️ اسم العميل/الفرع مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtCustomerName.Focus();
                return false;
            }
            if (cmbType.SelectedIndex < 0)
            {
                MessageBox.Show("️ الرجاء اختيار النوع", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbType.Focus();
                return false;
            }
            return true;
        }

        private void LoadDataFromDatabase()
        {
            Console.WriteLine("🔄 جاري تحميل البيانات من قاعدة البيانات...");
            records.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM CustomersBranches ORDER BY CustomerNo");
                if (dt != null)
                {
                    Console.WriteLine($"✅ تم استرجاع {dt.Rows.Count} صف من قاعدة البيانات");
                    foreach (DataRow row in dt.Rows)
                    {
                        records.Add(new CustomerBranch
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            CustomerNo = row["CustomerNo"].ToString(),
                            CustomerName = row["CustomerName"].ToString(),
                            Type = row["Type"].ToString(),
                            Phone = row["Phone"]?.ToString(),
                            Address = row["Address"]?.ToString(),
                            Email = row["Email"]?.ToString(),
                            ContactPerson = row["ContactPerson"]?.ToString(),
                            Notes = row["Notes"]?.ToString(),
                            IsActive = Convert.ToBoolean(row["IsActive"]),
                            CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now,
                            ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]) : DateTime.Now
                        });
                    }
                    Console.WriteLine($"✅ تم تحميل {records.Count} سجل إلى الذاكرة");
                    RefreshGrid();
                    Console.WriteLine($"✅ تم تحديث الجدول");
                }
                else
                {
                    Console.WriteLine("❌ لم يتم استرجاع أي بيانات");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ خطأ في LoadDataFromDatabase: {ex.Message}");
                MessageBox.Show($"❌ خطأ في تحميل البيانات:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveDataToDatabase(CustomerBranch record)
        {
            try
            {
                if (record.Id > 0)
                {
                    // ✅ تحديث - تم استبدال GETDATE() بـ datetime('now')
                    int rowsAffected = DatabaseHelper.ExecuteNonQuery(@"UPDATE CustomersBranches SET 
                CustomerName = @CustomerName, 
                Type = @Type, 
                Phone = @Phone, 
                Address = @Address, 
                Email = @Email, 
                ContactPerson = @ContactPerson, 
                Notes = @Notes, 
                IsActive = @IsActive, 
                ModifiedDate = datetime('now')
                WHERE Id = @Id",
                        DatabaseHelper.CreateParameter("@CustomerName", record.CustomerName),
                        DatabaseHelper.CreateParameter("@Type", record.Type),
                        DatabaseHelper.CreateParameter("@Phone", record.Phone),
                        DatabaseHelper.CreateParameter("@Address", record.Address),
                        DatabaseHelper.CreateParameter("@Email", record.Email),
                        DatabaseHelper.CreateParameter("@ContactPerson", record.ContactPerson),
                        DatabaseHelper.CreateParameter("@Notes", record.Notes),
                        DatabaseHelper.CreateParameter("@IsActive", record.IsActive),
                        DatabaseHelper.CreateParameter("@Id", record.Id)
                    );

                    if (rowsAffected == 0)
                        throw new Exception("لم يتم تحديث أي سجل");
                }
                else
                {
                    // ✅ إضافة - تم استبدال GETDATE() بـ datetime('now') و SCOPE_IDENTITY() بـ last_insert_rowid()
                    var newId = DatabaseHelper.ExecuteScalar(@"INSERT INTO CustomersBranches 
                (CustomerNo, CustomerName, Type, Phone, Address, Email, ContactPerson, Notes, IsActive, CreatedDate, ModifiedDate) 
                VALUES 
                (@CustomerNo, @CustomerName, @Type, @Phone, @Address, @Email, @ContactPerson, @Notes, @IsActive, datetime('now'), datetime('now'));
                SELECT last_insert_rowid();",
                        DatabaseHelper.CreateParameter("@CustomerNo", record.CustomerNo),
                        DatabaseHelper.CreateParameter("@CustomerName", record.CustomerName),
                        DatabaseHelper.CreateParameter("@Type", record.Type),
                        DatabaseHelper.CreateParameter("@Phone", record.Phone),
                        DatabaseHelper.CreateParameter("@Address", record.Address),
                        DatabaseHelper.CreateParameter("@Email", record.Email),
                        DatabaseHelper.CreateParameter("@ContactPerson", record.ContactPerson),
                        DatabaseHelper.CreateParameter("@Notes", record.Notes),
                        DatabaseHelper.CreateParameter("@IsActive", record.IsActive)
                    );

                    record.Id = Convert.ToInt32(newId);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"خطأ في حفظ البيانات: {ex.Message}");
            }
        }
        private void DeleteDataFromDatabase(int id)
        {
            DatabaseHelper.ExecuteNonQuery("DELETE FROM CustomersBranches WHERE Id = @Id",
                DatabaseHelper.CreateParameter("@Id", id));
        }

        private void RefreshGrid()
        {
            Console.WriteLine($"🔄 جاري تحديث الجدول - عدد السجلات: {records.Count}");
            dgvData.Rows.Clear();
            int idx = 1;
            foreach (var r in records.OrderBy(x => x.CustomerNo))
            {
                dgvData.Rows.Add(
                    idx++,
                    r.CustomerNo,
                    r.CustomerName,
                    r.Type,
                    r.Phone ?? "",
                    r.Address ?? "",
                    r.Email ?? "",
                    r.ContactPerson ?? "",
                    r.IsActive ? "✅ نشط" : "❌ غير نشط"
                );
            }
            lblTotal.Text = $"📊 الإجمالي: {records.Count}";
            Console.WriteLine($"✅ تم عرض {dgvData.Rows.Count} صف في الجدول");
        }

        private void ClearFields()
        {
            txtCustomerNo.Clear();
            txtCustomerName.Clear();
            cmbType.SelectedIndex = -1;
            txtPhone.Clear();
            txtAddress.Clear();
            txtEmail.Clear();
            txtContactPerson.Clear();
            txtNotes.Clear();
            chkIsActive.Checked = true;
            currentIndex = -1;
            txtCustomerNo.Focus();
        }

        private void LoadRecordToForm(CustomerBranch record)
        {
            txtCustomerNo.Text = record.CustomerNo;
            txtCustomerName.Text = record.CustomerName;
            cmbType.SelectedItem = record.Type;
            txtPhone.Text = record.Phone ?? "";
            txtAddress.Text = record.Address ?? "";
            txtEmail.Text = record.Email ?? "";
            txtContactPerson.Text = record.ContactPerson ?? "";
            txtNotes.Text = record.Notes ?? "";
            chkIsActive.Checked = record.IsActive;
        }

        private CustomerBranch GetRecordFromForm()
        {
            return new CustomerBranch
            {
                CustomerNo = txtCustomerNo.Text.Trim(),
                CustomerName = txtCustomerName.Text.Trim(),
                Type = cmbType.SelectedItem?.ToString() ?? "",
                Phone = txtPhone.Text.Trim(),
                Address = txtAddress.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                ContactPerson = txtContactPerson.Text.Trim(),
                Notes = txtNotes.Text.Trim(),
                IsActive = chkIsActive.Checked,
                ModifiedDate = DateTime.Now
            };
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            ClearFields();
            SetControlsState(true);
            originalRecord = null;
            currentIndex = -1;
            hasChanges = false;
            editingRecordId = 0; // ✅ إعادة تعيين الـ ID للإضافة الجديدة

            int nextNo = records.Count > 0 ? records.Max(r => r.Id) + 1 : 1;
            txtCustomerNo.Text = $"C{nextNo:D4}";

            MessageBox.Show("✅ تم فتح نموذج إضافة عميل/فرع جديد", "إضافة جديد", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // ✅ التحقق من وجود بيانات في الحقول
            if (string.IsNullOrWhiteSpace(txtCustomerNo.Text))
            {
                MessageBox.Show("⚠️ الرجاء اختيار سجل أولاً\n\n💡 انقر نقراً مزدوجاً على الصف في الجدول",
                    "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ✅ إذا كان currentIndex = -1، ابحث عن السجل
            if (currentIndex < 0 || currentIndex >= records.Count)
            {
                string customerNo = txtCustomerNo.Text.Trim();
                var record = records.FirstOrDefault(r => r.CustomerNo == customerNo);
                if (record != null)
                {
                    currentIndex = records.IndexOf(record);
                }
            }

            if (currentIndex < 0 || currentIndex >= records.Count)
            {
                MessageBox.Show("⚠️ لم يتم العثور على السجل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // ✅ حفظ الـ ID الأصلي للتعديل
            editingRecordId = records[currentIndex].Id;
            SaveOriginalRecord();
            SetControlsState(true);

            Console.WriteLine($"✅ تم فتح وضع التعديل - ID: {editingRecordId}");

            MessageBox.Show($"✅ تم فتح وضع التعديل\n\n💡 عدّل البيانات ثم اضغط 'حفظ'",
                "تعديل", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!isEditingMode)
            {
                MessageBox.Show("⚠️ الرجاء الضغط على 'جديد' أو 'تعديل' أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateRequiredFields()) return;

            try
            {
                var record = GetRecordFromForm();

                // ✅ استخدام الـ ID المحفوظ للتعديل
                record.Id = editingRecordId;

                Console.WriteLine($"🔍 جاري الحفظ...");
                Console.WriteLine($"   - CustomerNo: {record.CustomerNo}");
                Console.WriteLine($"   - Id: {record.Id}");
                Console.WriteLine($"   - editingRecordId: {editingRecordId}");

                // ✅ التحقق من التكرار - استبعد السجل الحالي
                var existing = records.FirstOrDefault(r =>
                    r.CustomerNo == record.CustomerNo &&
                    r.Id != record.Id);

                if (existing != null)
                {
                    MessageBox.Show($"⚠️ رقم العميل/الفرع '{record.CustomerNo}' موجود مسبقاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // الحفظ في قاعدة البيانات
                SaveDataToDatabase(record);

                // تحديث القائمة المحلية
                var localExistingIndex = records.FindIndex(r => r.Id == record.Id);
                if (localExistingIndex >= 0)
                {
                    // ✅ تحديث سجل موجود
                    records[localExistingIndex] = record;
                    MessageBox.Show("✅ تم التحديث بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // ✅ إضافة سجل جديد
                    records.Add(record);
                    currentIndex = records.Count - 1;
                    MessageBox.Show("✅ تمت الإضافة بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // ✅ إعادة تعيين الـ ID
                editingRecordId = 0;

                RefreshGrid();
                SetControlsState(false);
                hasChanges = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= records.Count)
            {
                MessageBox.Show("️ الرجاء اختيار سجل للحذف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var record = records[currentIndex];
            DialogResult result = MessageBox.Show(
                $"🗑️ هل أنت متأكد من حذف:\n{record.CustomerNo} - {record.CustomerName}؟",
                "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DeleteDataFromDatabase(record.Id);
                    records.RemoveAt(currentIndex);
                    RefreshGrid();
                    ClearFields();
                    SetControlsState(false);
                    MessageBox.Show("✅ تم الحذف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                string customerNo = dgvData.Rows[e.RowIndex].Cells["colCustomerNo"].Value?.ToString();
                var record = records.FirstOrDefault(r => r.CustomerNo == customerNo);
                if (record != null)
                {
                    currentIndex = records.IndexOf(record);
                    LoadRecordToForm(record);

                    Console.WriteLine($"✅ تم النقر المزدوج - currentIndex: {currentIndex}, Id: {record.Id}");
                    Console.WriteLine($"   - CustomerNo: {record.CustomerNo}");
                }
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            string searchText = txtSearch.Text.Trim().ToLower();
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                string customerNo = row.Cells["colCustomerNo"].Value?.ToString()?.ToLower() ?? "";
                string customerName = row.Cells["colCustomerName"].Value?.ToString()?.ToLower() ?? "";
                string phone = row.Cells["colPhone"].Value?.ToString()?.ToLower() ?? "";

                if (string.IsNullOrEmpty(searchText) ||
                    customerNo.Contains(searchText) ||
                    customerName.Contains(searchText) ||
                    phone.Contains(searchText))
                {
                    row.Visible = true;
                }
                else
                {
                    row.Visible = false;
                }
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("⚠️ الرجاء إدخال نص للبحث", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            TxtSearch_TextChanged(sender, e);
        }


        private void DgvData_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvData.CurrentRow != null && !dgvData.CurrentRow.IsNewRow)
            {
                string customerNo = dgvData.CurrentRow.Cells["colCustomerNo"].Value?.ToString();
                var record = records.FirstOrDefault(r => r.CustomerNo == customerNo);
                if (record != null)
                {
                    currentIndex = records.IndexOf(record);

                    // ✅ تحديث حالة الأزرار فوراً
                    btnEdit.Enabled = !isEditingMode;
                    btnDelete.Enabled = !isEditingMode;

                    Console.WriteLine($"✅ تم تحديد السجل: {customerNo} - currentIndex: {currentIndex}");
                }
            }
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (records.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد بيانات للتصدير", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "CSV ملف|*.csv";
            sfd.FileName = $"Customers_Branches_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter sw = new StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine("م,رقم العميل/الفرع,الاسم,النوع,الهاتف,العنوان,البريد,شخص الاتصال,ملاحظات,الحالة");
                        int idx = 1;
                        foreach (var r in records)
                        {
                            sw.WriteLine($"{idx++},{r.CustomerNo},{r.CustomerName},{r.Type},{r.Phone},{r.Address},{r.Email},{r.ContactPerson},{r.Notes},{(r.IsActive ? "نشط" : "غير نشط")}");
                        }
                    }
                    MessageBox.Show($"✅ تم تصدير {records.Count} سجل بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($" خطأ في التصدير: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (isEditingMode && hasChanges)
            {
                DialogResult result = MessageBox.Show(
                    "⚠️ لديك تغييرات غير محفوظة!\n\n" +
                    "• اضغط 'نعم' لحفظ التغييرات\n" +
                    "• اضغط 'لا' للتجاهل والخروج\n" +
                    "• اضغط 'إلغاء' للبقاء والتعديل",
                    "تأكيد الخروج",
                    MessageBoxButtons.YesNoCancel,
                    MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    BtnSave_Click(this, EventArgs.Empty);
                    if (hasChanges) e.Cancel = true;
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
            base.OnFormClosing(e);
        }
    }
}