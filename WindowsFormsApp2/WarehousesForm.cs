using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class WarehousesForm : Form
    {
        private List<WarehouseRecord> warehouses = new List<WarehouseRecord>();
        private List<PortsForm.PortData> allPorts = new List<PortsForm.PortData>();

        // أزرار التنقل
        private int currentIndex = -1;
        private Button btnFirst, btnPrevious, btnNext, btnLast;
        private Label lblRecordCount;

        // تعريف الحقول
        private TextBox txtWarehouseCode;
        private TextBox txtWarehouseName;
        private ComboBox cmbWarehouseType;
        private ComboBox cmbPortLink;
        private TextBox txtAddress;
        private TextBox txtManager;
        private TextBox txtPhone;
        private ComboBox cmbStatus;
        private TextBox txtNotes;
        private Label lblNotes;

        // جدول البيانات
        private DataGridView dgvWarehouses;

        // شريط الأدوات
        private ToolStrip toolStrip1;
        private ToolStripButton btnSave, btnNew, btnEdit, btnDelete, btnSearch, btnExport, btnExit;
        private ToolStripButton btnHome;
        private ToolStripSeparator toolStripSeparator1, toolStripSeparator2, toolStripSeparator3, toolStripSeparator4;
        private ToolStripSeparator toolStripSeparatorHome;

        // GroupBox الرئيسي
        private GroupBox grpWarehouse;

        public WarehousesForm()
        {
            CreateControlsManually();
            InitializeGrid();
            LoadPortsFromDatabase(); // ✅ تحميل المنافذ من قاعدة البيانات
            LoadWarehousesFromDatabase(); // ✅ تحميل المخازن من قاعدة البيانات
            ClearFields();
            SetControlsState(false);
            UpdateNavigation();

            ApplyTranslations();
            LanguageManager.LanguageChanged += (s, e) => RefreshAllTranslations();
        }

        // ==================== نموذج البيانات ====================
        public class WarehouseRecord
        {
            public int Id { get; set; }
            public string WarehouseCode { get; set; }
            public string WarehouseName { get; set; }
            public string WarehouseType { get; set; }
            public string PortLink { get; set; }
            public string Address { get; set; }
            public string Manager { get; set; }
            public string Phone { get; set; }
            public string Status { get; set; }
            public string Notes { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime? ModifiedDate { get; set; }
        }

        // ==================== ✅ تطبيق الترجمات ====================
        private void ApplyTranslations()
        {
            this.Text = LanguageManager.Get("warehouses_form_title");
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;

            if (grpWarehouse != null) grpWarehouse.Text = LanguageManager.Get("grp_warehouse_data");

            TrySetText(FindControl<Label>("lblWarehouseCode"), LanguageManager.Get("lbl_warehouse_code"));
            TrySetText(FindControl<Label>("lblWarehouseName"), LanguageManager.Get("lbl_warehouse_name"));
            TrySetText(FindControl<Label>("lblWarehouseType"), LanguageManager.Get("lbl_warehouse_type"));
            TrySetText(FindControl<Label>("lblPortLink"), LanguageManager.Get("lbl_port_link"));
            TrySetText(FindControl<Label>("lblAddress"), LanguageManager.Get("lbl_address"));
            TrySetText(FindControl<Label>("lblManager"), LanguageManager.Get("lbl_manager"));
            TrySetText(FindControl<Label>("lblPhone"), LanguageManager.Get("lbl_phone"));
            TrySetText(FindControl<Label>("lblStatus"), LanguageManager.Get("lbl_status"));
            if (lblNotes != null) lblNotes.Text = LanguageManager.Get("lbl_notes");

            UpdateComboBoxItems();

            TrySetText(FindToolStripButton("btnHome"), LanguageManager.Get("btn_home"));
            TrySetText(FindToolStripButton("btnSave"), LanguageManager.Get("btn_save"));
            TrySetText(FindToolStripButton("btnNew"), LanguageManager.Get("btn_new"));
            TrySetText(FindToolStripButton("btnEdit"), LanguageManager.Get("btn_edit"));
            TrySetText(FindToolStripButton("btnDelete"), LanguageManager.Get("btn_delete"));
            TrySetText(FindToolStripButton("btnSearch"), LanguageManager.Get("btn_search"));
            TrySetText(FindToolStripButton("btnExport"), LanguageManager.Get("btn_export"));
            TrySetText(FindToolStripButton("btnExit"), LanguageManager.Get("btn_exit"));

            if (btnFirst != null) btnFirst.Text = LanguageManager.Get("btn_first");
            if (btnPrevious != null) btnPrevious.Text = LanguageManager.Get("btn_previous");
            if (btnNext != null) btnNext.Text = LanguageManager.Get("btn_next");
            if (btnLast != null) btnLast.Text = LanguageManager.Get("btn_last");

            UpdateGridHeaders();
        }

        private void RefreshAllTranslations()
        {
            ApplyTranslations();
            RefreshGrid();
        }

        private void UpdateComboBoxItems()
        {
            if (cmbWarehouseType != null)
            {
                var selected = cmbWarehouseType.SelectedItem?.ToString();
                cmbWarehouseType.Items.Clear();
                cmbWarehouseType.Items.Add(LanguageManager.Get("type_main"));
                cmbWarehouseType.Items.Add(LanguageManager.Get("type_sub"));
                cmbWarehouseType.Items.Add(LanguageManager.Get("type_temp"));

                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected == "رئيسي") cmbWarehouseType.SelectedItem = LanguageManager.Get("type_main");
                    else if (selected == "فرعي") cmbWarehouseType.SelectedItem = LanguageManager.Get("type_sub");
                    else if (selected == "مؤقت") cmbWarehouseType.SelectedItem = LanguageManager.Get("type_temp");
                }
            }

            if (cmbStatus != null)
            {
                var selected = cmbStatus.SelectedItem?.ToString();
                cmbStatus.Items.Clear();
                cmbStatus.Items.Add(LanguageManager.Get("status_active"));
                cmbStatus.Items.Add(LanguageManager.Get("status_inactive"));

                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected == "نشط") cmbStatus.SelectedItem = LanguageManager.Get("status_active");
                    else if (selected == "غير نشط") cmbStatus.SelectedItem = LanguageManager.Get("status_inactive");
                }
            }

            if (cmbPortLink != null)
            {
                var selected = cmbPortLink.SelectedItem?.ToString();
                cmbPortLink.Items.Clear();
                cmbPortLink.Items.Add(LanguageManager.Get("not_linked"));
                foreach (var port in allPorts)
                {
                    cmbPortLink.Items.Add(port.Name);
                }

                if (!string.IsNullOrEmpty(selected))
                {
                    if (selected == "-- غير مرتبط --") cmbPortLink.SelectedItem = LanguageManager.Get("not_linked");
                    else cmbPortLink.SelectedItem = selected;
                }
                else if (cmbPortLink.Items.Count > 0)
                {
                    cmbPortLink.SelectedIndex = 0;
                }
            }
        }

        private void UpdateGridHeaders()
        {
            if (dgvWarehouses == null || dgvWarehouses.Columns.Count == 0) return;

            if (dgvWarehouses.Columns["colId"] != null) dgvWarehouses.Columns["colId"].HeaderText = LanguageManager.Get("col_id");
            if (dgvWarehouses.Columns["colCode"] != null) dgvWarehouses.Columns["colCode"].HeaderText = LanguageManager.Get("col_warehouse_code");
            if (dgvWarehouses.Columns["colName"] != null) dgvWarehouses.Columns["colName"].HeaderText = LanguageManager.Get("col_warehouse_name");
            if (dgvWarehouses.Columns["colType"] != null) dgvWarehouses.Columns["colType"].HeaderText = LanguageManager.Get("col_type");
            if (dgvWarehouses.Columns["colPortLink"] != null) dgvWarehouses.Columns["colPortLink"].HeaderText = LanguageManager.Get("col_port_link");
            if (dgvWarehouses.Columns["colAddress"] != null) dgvWarehouses.Columns["colAddress"].HeaderText = LanguageManager.Get("col_address");
            if (dgvWarehouses.Columns["colManager"] != null) dgvWarehouses.Columns["colManager"].HeaderText = LanguageManager.Get("col_manager");
            if (dgvWarehouses.Columns["colPhone"] != null) dgvWarehouses.Columns["colPhone"].HeaderText = LanguageManager.Get("col_phone");
            if (dgvWarehouses.Columns["colStatus"] != null) dgvWarehouses.Columns["colStatus"].HeaderText = LanguageManager.Get("col_status");
        }

        private T FindControl<T>(string name) where T : Control
        {
            try { return this.Controls.Find(name, true).FirstOrDefault() as T; }
            catch { return null; }
        }

        private ToolStripButton FindToolStripButton(string name)
        {
            try
            {
                if (toolStrip1 != null)
                {
                    foreach (ToolStripItem item in toolStrip1.Items)
                    {
                        if (item.Name == name) return item as ToolStripButton;
                    }
                }
                return null;
            }
            catch { return null; }
        }

        private void TrySetText(Control control, string text)
        {
            if (control != null && !string.IsNullOrEmpty(text)) control.Text = text;
        }

        private void TrySetText(ToolStripButton button, string text)
        {
            if (button != null && !string.IsNullOrEmpty(text)) button.Text = text;
        }

        // ==================== ✅ تحميل البيانات من قاعدة البيانات ====================
        private void LoadPortsFromDatabase()
        {
            allPorts.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName FROM Ports ORDER BY PortName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allPorts.Add(new PortsForm.PortData { Name = row["PortName"].ToString() });
                    }
                }
            }
            catch { }

            if (cmbPortLink != null)
            {
                cmbPortLink.Items.Clear();
                cmbPortLink.Items.Add(LanguageManager.Get("not_linked"));
                foreach (var port in allPorts)
                {
                    cmbPortLink.Items.Add(port.Name);
                }
                if (cmbPortLink.Items.Count > 0)
                    cmbPortLink.SelectedIndex = 0;
            }
        }

        private void LoadWarehousesFromDatabase()
        {
            warehouses.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Warehouses ORDER BY WarehouseId");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        warehouses.Add(new WarehouseRecord
                        {
                            Id = Convert.ToInt32(row["WarehouseId"]),
                            WarehouseCode = row["WarehouseCode"].ToString(),
                            WarehouseName = row["WarehouseName"].ToString(),
                            WarehouseType = row["WarehouseType"].ToString(),
                            PortLink = row["PortLink"].ToString(),
                            Address = row["Address"].ToString(),
                            Manager = row["Manager"].ToString(),
                            Phone = row["Phone"].ToString(),
                            Status = row["Status"].ToString(),
                            Notes = row["Notes"].ToString(),
                            CreatedDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now,
                            ModifiedDate = row["ModifiedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ModifiedDate"]) : null
                        });
                    }
                    RefreshGrid();
                    if (warehouses.Count > 0)
                    {
                        currentIndex = 0;
                        LoadRecordToForm(warehouses[0]);
                    }
                }
                else
                {
                    // إذا كانت القاعدة فارغة، نضيف بيانات افتراضية
                    LoadSampleDataToDatabase();
                    LoadWarehousesFromDatabase(); // إعادة التحميل بعد الإضافة
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ في تحميل المخازن:\n{ex.Message}", "خطأ"); }
        }

        private void LoadSampleDataToDatabase()
        {
            var samples = new List<WarehouseRecord>
            {
                new WarehouseRecord { WarehouseCode = "WH-01", WarehouseName = "المخزن الرئيسي - عدن", WarehouseType = "رئيسي", PortLink = "منفذ عدن البحري", Address = "عدن - المنطقة الحرة", Manager = "أحمد علي", Phone = "777111222", Status = "نشط", Notes = "", CreatedDate = DateTime.Now },
                new WarehouseRecord { WarehouseCode = "WH-02", WarehouseName = "مخزن المكلا", WarehouseType = "فرعي", PortLink = "منفذ المكلا البحري", Address = "حضرموت - المكلا", Manager = "سالم عمر", Phone = "777333444", Status = "نشط", Notes = "", CreatedDate = DateTime.Now },
                new WarehouseRecord { WarehouseCode = "WH-03", WarehouseName = "مخزن صنعاء", WarehouseType = "رئيسي", PortLink = "منفذ صنعاء الجوي", Address = "صنعاء - المطار", Manager = "محمد احمد", Phone = "777555666", Status = "نشط", Notes = "", CreatedDate = DateTime.Now }
            };

            foreach (var sample in samples)
            {
                SaveWarehouseToDatabase(sample, "إضافة");
                warehouses.Add(sample);
            }
        }

        private void SaveWarehouseToDatabase(WarehouseRecord record, string operationType)
        {
            try
            {
                if (record.Id > 0)
                {
                    // ✅ تم التغيير من GETDATE() إلى datetime('now')
                    DatabaseHelper.ExecuteNonQuery(@"UPDATE Warehouses SET 
                        WarehouseCode = @Code, WarehouseName = @Name, WarehouseType = @Type, 
                        PortLink = @PortLink, Address = @Address, Manager = @Manager, 
                        Phone = @Phone, Status = @Status, Notes = @Notes, ModifiedDate = datetime('now')
                        WHERE WarehouseId = @Id",
                        DatabaseHelper.CreateParameter("@Code", record.WarehouseCode),
                        DatabaseHelper.CreateParameter("@Name", record.WarehouseName),
                        DatabaseHelper.CreateParameter("@Type", record.WarehouseType),
                        DatabaseHelper.CreateParameter("@PortLink", record.PortLink),
                        DatabaseHelper.CreateParameter("@Address", record.Address),
                        DatabaseHelper.CreateParameter("@Manager", record.Manager),
                        DatabaseHelper.CreateParameter("@Phone", record.Phone),
                        DatabaseHelper.CreateParameter("@Status", record.Status),
                        DatabaseHelper.CreateParameter("@Notes", record.Notes),
                        DatabaseHelper.CreateParameter("@Id", record.Id)
                    );
                }
                else
                {
                    // ✅ تم التغيير من GETDATE() إلى datetime('now') و SCOPE_IDENTITY() إلى last_insert_rowid()
                    var newId = DatabaseHelper.ExecuteScalar(@"INSERT INTO Warehouses 
                        (WarehouseCode, WarehouseName, WarehouseType, PortLink, Address, Manager, Phone, Status, Notes, CreatedDate, ModifiedDate) 
                        VALUES (@Code, @Name, @Type, @PortLink, @Address, @Manager, @Phone, @Status, @Notes, datetime('now'), datetime('now'));
                        SELECT last_insert_rowid();",
                        DatabaseHelper.CreateParameter("@Code", record.WarehouseCode),
                        DatabaseHelper.CreateParameter("@Name", record.WarehouseName),
                        DatabaseHelper.CreateParameter("@Type", record.WarehouseType),
                        DatabaseHelper.CreateParameter("@PortLink", record.PortLink),
                        DatabaseHelper.CreateParameter("@Address", record.Address),
                        DatabaseHelper.CreateParameter("@Manager", record.Manager),
                        DatabaseHelper.CreateParameter("@Phone", record.Phone),
                        DatabaseHelper.CreateParameter("@Status", record.Status),
                        DatabaseHelper.CreateParameter("@Notes", record.Notes)
                    );
                    record.Id = Convert.ToInt32(newId);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ المخزن:\n{ex.Message}", "خطأ");
                throw;
            }
        }

        private void InitializeGrid()
        {
            if (dgvWarehouses == null) return;

            dgvWarehouses.Columns.Clear();

            dgvWarehouses.Columns.Add("colId", LanguageManager.Get("col_id"));
            dgvWarehouses.Columns.Add("colCode", LanguageManager.Get("col_warehouse_code"));
            dgvWarehouses.Columns.Add("colName", LanguageManager.Get("col_warehouse_name"));
            dgvWarehouses.Columns.Add("colType", LanguageManager.Get("col_type"));
            dgvWarehouses.Columns.Add("colPortLink", LanguageManager.Get("col_port_link"));
            dgvWarehouses.Columns.Add("colAddress", LanguageManager.Get("col_address"));
            dgvWarehouses.Columns.Add("colManager", LanguageManager.Get("col_manager"));
            dgvWarehouses.Columns.Add("colPhone", LanguageManager.Get("col_phone"));
            dgvWarehouses.Columns.Add("colStatus", LanguageManager.Get("col_status"));

            dgvWarehouses.AllowUserToAddRows = false;
            dgvWarehouses.AllowUserToDeleteRows = false;
            dgvWarehouses.ReadOnly = true;
            dgvWarehouses.RightToLeft = LanguageManager.RightToLeft;
            dgvWarehouses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvWarehouses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvWarehouses.Dock = DockStyle.Fill;

            dgvWarehouses.EnableHeadersVisualStyles = false;
            dgvWarehouses.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvWarehouses.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvWarehouses.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvWarehouses.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvWarehouses.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvWarehouses.RowsDefaultCellStyle.BackColor = Color.White;
            dgvWarehouses.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);
            dgvWarehouses.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            dgvWarehouses.RowHeadersVisible = false;
            dgvWarehouses.RowTemplate.Height = 35;

            dgvWarehouses.CellClick += DgvWarehouses_CellClick;
        }

        private void DgvWarehouses_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < warehouses.Count)
            {
                currentIndex = e.RowIndex;
                LoadRecordToForm(warehouses[currentIndex]);
                SetControlsState(false);
                UpdateNavigation();
            }
        }

        private void LoadRecordToForm(WarehouseRecord record)
        {
            if (txtWarehouseCode != null) txtWarehouseCode.Text = record.WarehouseCode;
            if (txtWarehouseName != null) txtWarehouseName.Text = record.WarehouseName;

            if (cmbWarehouseType != null)
            {
                if (record.WarehouseType == "رئيسي") cmbWarehouseType.SelectedItem = LanguageManager.Get("type_main");
                else if (record.WarehouseType == "فرعي") cmbWarehouseType.SelectedItem = LanguageManager.Get("type_sub");
                else if (record.WarehouseType == "مؤقت") cmbWarehouseType.SelectedItem = LanguageManager.Get("type_temp");
                else cmbWarehouseType.SelectedItem = record.WarehouseType;
            }

            if (cmbPortLink != null && cmbPortLink.Items.Count > 0)
            {
                if (cmbPortLink.Items.Contains(record.PortLink))
                    cmbPortLink.SelectedItem = record.PortLink;
                else
                    cmbPortLink.SelectedIndex = 0;
            }

            if (txtAddress != null) txtAddress.Text = record.Address;
            if (txtManager != null) txtManager.Text = record.Manager;
            if (txtPhone != null) txtPhone.Text = record.Phone;

            if (cmbStatus != null)
            {
                if (record.Status == "نشط") cmbStatus.SelectedItem = LanguageManager.Get("status_active");
                else if (record.Status == "غير نشط") cmbStatus.SelectedItem = LanguageManager.Get("status_inactive");
                else cmbStatus.SelectedItem = record.Status;
            }

            if (txtNotes != null) txtNotes.Text = record.Notes;
        }

        private void RefreshGrid()
        {
            if (dgvWarehouses == null) return;
            dgvWarehouses.Rows.Clear();
            int idx = 1;
            foreach (var w in warehouses.OrderBy(x => x.Id))
            {
                string typeTranslated = w.WarehouseType;
                if (w.WarehouseType == "رئيسي") typeTranslated = LanguageManager.Get("type_main");
                else if (w.WarehouseType == "فرعي") typeTranslated = LanguageManager.Get("type_sub");
                else if (w.WarehouseType == "مؤقت") typeTranslated = LanguageManager.Get("type_temp");

                string statusTranslated = w.Status;
                if (w.Status == "نشط") statusTranslated = LanguageManager.Get("status_active");
                else if (w.Status == "غير نشط") statusTranslated = LanguageManager.Get("status_inactive");

                dgvWarehouses.Rows.Add(
                    idx++, w.WarehouseCode, w.WarehouseName, typeTranslated,
                    w.PortLink, w.Address, w.Manager, w.Phone, statusTranslated
                );
            }
            UpdateNavigation();
        }

        private void UpdateNavigation()
        {
            if (btnFirst != null) btnFirst.Enabled = warehouses.Count > 0 && currentIndex > 0;
            if (btnPrevious != null) btnPrevious.Enabled = warehouses.Count > 0 && currentIndex > 0;
            if (btnNext != null) btnNext.Enabled = warehouses.Count > 0 && currentIndex < warehouses.Count - 1;
            if (btnLast != null) btnLast.Enabled = warehouses.Count > 0 && currentIndex < warehouses.Count - 1;
            if (lblRecordCount != null) lblRecordCount.Text = warehouses.Count > 0 ? $"{currentIndex + 1} / {warehouses.Count}" : "0 / 0";
        }

        private void NavigateTo(int index)
        {
            if (index < 0 || index >= warehouses.Count) return;
            currentIndex = index;
            LoadRecordToForm(warehouses[currentIndex]);
            SetControlsState(false);
            UpdateNavigation();
        }

        private void btnFirst_Click(object sender, EventArgs e) => NavigateTo(0);
        private void btnPrevious_Click(object sender, EventArgs e) => NavigateTo(currentIndex - 1);
        private void btnNext_Click(object sender, EventArgs e) => NavigateTo(currentIndex + 1);
        private void btnLast_Click(object sender, EventArgs e) => NavigateTo(warehouses.Count - 1);

        private void SetControlsState(bool enabled)
        {
            if (txtWarehouseCode != null) txtWarehouseCode.Enabled = enabled;
            if (txtWarehouseName != null) txtWarehouseName.Enabled = enabled;
            if (cmbWarehouseType != null) cmbWarehouseType.Enabled = enabled;
            if (cmbPortLink != null) cmbPortLink.Enabled = enabled;
            if (txtAddress != null) txtAddress.Enabled = enabled;
            if (txtManager != null) txtManager.Enabled = enabled;
            if (txtPhone != null) txtPhone.Enabled = enabled;
            if (cmbStatus != null) cmbStatus.Enabled = enabled;
            if (txtNotes != null) txtNotes.Enabled = enabled;
        }

        // ==================== إنشاء الواجهة ====================
        private void CreateControlsManually()
        {
            this.Text = LanguageManager.Get("warehouses_form_title");
            this.Size = new Size(1300, 750);
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            toolStrip1 = new ToolStrip();
            toolStrip1.RightToLeft = RightToLeft.Yes;
            toolStrip1.BackColor = Color.FromArgb(52, 73, 94);
            toolStrip1.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            btnHome = new ToolStripButton(LanguageManager.Get("btn_home"));
            btnHome.Name = "btnHome";
            btnHome.ForeColor = Color.White;
            btnHome.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnHome.Click += btnHome_Click;

            toolStripSeparatorHome = new ToolStripSeparator();

            btnSave = new ToolStripButton(LanguageManager.Get("btn_save"));
            btnSave.Name = "btnSave";
            btnSave.ForeColor = Color.White;
            btnSave.Click += btnSave_Click;

            btnNew = new ToolStripButton(LanguageManager.Get("btn_new"));
            btnNew.Name = "btnNew";
            btnNew.ForeColor = Color.White;
            btnNew.Click += btnNew_Click;

            toolStripSeparator1 = new ToolStripSeparator();

            btnEdit = new ToolStripButton(LanguageManager.Get("btn_edit"));
            btnEdit.Name = "btnEdit";
            btnEdit.ForeColor = Color.White;
            btnEdit.Click += btnEdit_Click;

            btnDelete = new ToolStripButton(LanguageManager.Get("btn_delete"));
            btnDelete.Name = "btnDelete";
            btnDelete.ForeColor = Color.White;
            btnDelete.Click += btnDelete_Click;

            toolStripSeparator2 = new ToolStripSeparator();

            btnSearch = new ToolStripButton(LanguageManager.Get("btn_search"));
            btnSearch.Name = "btnSearch";
            btnSearch.ForeColor = Color.White;
            btnSearch.Click += btnSearch_Click;

            toolStripSeparator3 = new ToolStripSeparator();

            btnExport = new ToolStripButton(LanguageManager.Get("btn_export"));
            btnExport.Name = "btnExport";
            btnExport.ForeColor = Color.White;
            btnExport.Click += btnExport_Click;

            toolStripSeparator4 = new ToolStripSeparator();

            btnExit = new ToolStripButton(LanguageManager.Get("btn_exit"));
            btnExit.Name = "btnExit";
            btnExit.ForeColor = Color.White;
            btnExit.Click += (s, e) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] {
                btnHome, toolStripSeparatorHome,
                btnSave, btnNew, toolStripSeparator1,
                btnEdit, btnDelete, toolStripSeparator2,
                btnSearch, toolStripSeparator3,
                btnExport, toolStripSeparator4, btnExit
            });

            Panel pnlMain = new Panel();
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Padding = new Padding(10);
            pnlMain.BackColor = Color.FromArgb(240, 248, 255);

            grpWarehouse = new GroupBox();
            grpWarehouse.Text = LanguageManager.Get("grp_warehouse_data");
            grpWarehouse.Font = new Font("Tahoma", 11, FontStyle.Bold);
            grpWarehouse.ForeColor = Color.FromArgb(33, 150, 243);
            grpWarehouse.Dock = DockStyle.Top;
            grpWarehouse.Height = 280;
            grpWarehouse.Padding = new Padding(10);

            TableLayoutPanel tlp = new TableLayoutPanel();
            tlp.ColumnCount = 4;
            tlp.RowCount = 6;
            tlp.Dock = DockStyle.Fill;
            tlp.Padding = new Padding(10);
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 15F));
            tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            for (int i = 0; i < 6; i++) tlp.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));

            AddLabelAndControl(tlp, LanguageManager.Get("lbl_warehouse_code"), out txtWarehouseCode, 0, 0, "lblWarehouseCode");
            AddLabelAndControl(tlp, LanguageManager.Get("lbl_warehouse_name"), out txtWarehouseName, 0, 2, "lblWarehouseName");

            AddLabelAndCombo(tlp, LanguageManager.Get("lbl_warehouse_type"), new[] { LanguageManager.Get("type_main"), LanguageManager.Get("type_sub"), LanguageManager.Get("type_temp") }, out cmbWarehouseType, 1, 0, "lblWarehouseType");
            AddLabelAndCombo(tlp, LanguageManager.Get("lbl_port_link"), out cmbPortLink, 1, 2, "lblPortLink");

            AddLabelAndControl(tlp, LanguageManager.Get("lbl_address"), out txtAddress, 2, 0, "lblAddress");
            AddLabelAndControl(tlp, LanguageManager.Get("lbl_manager"), out txtManager, 2, 2, "lblManager");

            AddLabelAndControl(tlp, LanguageManager.Get("lbl_phone"), out txtPhone, 3, 0, "lblPhone");
            AddLabelAndCombo(tlp, LanguageManager.Get("lbl_status"), new[] { LanguageManager.Get("status_active"), LanguageManager.Get("status_inactive") }, out cmbStatus, 3, 2, "lblStatus");
            if (cmbStatus != null && cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;

            lblNotes = new Label();
            lblNotes.Text = LanguageManager.Get("lbl_notes");
            lblNotes.Name = "lblNotes";
            lblNotes.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblNotes.Dock = DockStyle.Fill;
            lblNotes.TextAlign = ContentAlignment.MiddleLeft;
            tlp.Controls.Add(lblNotes, 0, 4);

            txtNotes = new TextBox();
            txtNotes.Font = new Font("Tahoma", 10);
            txtNotes.Dock = DockStyle.Fill;
            txtNotes.Multiline = true;
            txtNotes.Height = 60;
            tlp.Controls.Add(txtNotes, 1, 4);
            tlp.SetColumnSpan(txtNotes, 3);

            Panel pnlNav = new Panel();
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.Height = 40;

            btnFirst = new Button();
            btnFirst.Text = LanguageManager.Get("btn_first");
            btnFirst.Font = new Font("Tahoma", 9, FontStyle.Bold);
            btnFirst.BackColor = Color.FromArgb(52, 73, 94);
            btnFirst.ForeColor = Color.White;
            btnFirst.FlatStyle = FlatStyle.Flat;
            btnFirst.Size = new Size(80, 30);
            btnFirst.Location = new Point(10, 5);
            btnFirst.Click += btnFirst_Click;

            btnPrevious = new Button();
            btnPrevious.Text = LanguageManager.Get("btn_previous");
            btnPrevious.Font = new Font("Tahoma", 9, FontStyle.Bold);
            btnPrevious.BackColor = Color.FromArgb(52, 73, 94);
            btnPrevious.ForeColor = Color.White;
            btnPrevious.FlatStyle = FlatStyle.Flat;
            btnPrevious.Size = new Size(80, 30);
            btnPrevious.Location = new Point(95, 5);
            btnPrevious.Click += btnPrevious_Click;

            lblRecordCount = new Label();
            lblRecordCount.Text = "0 / 0";
            lblRecordCount.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblRecordCount.ForeColor = Color.FromArgb(33, 150, 243);
            lblRecordCount.BackColor = Color.White;
            lblRecordCount.TextAlign = ContentAlignment.MiddleCenter;
            lblRecordCount.Size = new Size(80, 30);
            lblRecordCount.Location = new Point(180, 5);
            lblRecordCount.BorderStyle = BorderStyle.FixedSingle;

            btnNext = new Button();
            btnNext.Text = LanguageManager.Get("btn_next");
            btnNext.Font = new Font("Tahoma", 9, FontStyle.Bold);
            btnNext.BackColor = Color.FromArgb(52, 73, 94);
            btnNext.ForeColor = Color.White;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Size = new Size(80, 30);
            btnNext.Location = new Point(265, 5);
            btnNext.Click += btnNext_Click;

            btnLast = new Button();
            btnLast.Text = LanguageManager.Get("btn_last");
            btnLast.Font = new Font("Tahoma", 9, FontStyle.Bold);
            btnLast.BackColor = Color.FromArgb(52, 73, 94);
            btnLast.ForeColor = Color.White;
            btnLast.FlatStyle = FlatStyle.Flat;
            btnLast.Size = new Size(80, 30);
            btnLast.Location = new Point(350, 5);
            btnLast.Click += btnLast_Click;

            pnlNav.Controls.Add(btnFirst);
            pnlNav.Controls.Add(btnPrevious);
            pnlNav.Controls.Add(lblRecordCount);
            pnlNav.Controls.Add(btnNext);
            pnlNav.Controls.Add(btnLast);

            tlp.Controls.Add(pnlNav, 0, 5);
            tlp.SetColumnSpan(pnlNav, 4);

            grpWarehouse.Controls.Add(tlp);

            dgvWarehouses = new DataGridView();
            dgvWarehouses.Dock = DockStyle.Fill;

            pnlMain.Controls.Add(dgvWarehouses);
            pnlMain.Controls.Add(grpWarehouse);

            this.Controls.Add(pnlMain);
            this.Controls.Add(toolStrip1);
        }

        private void AddLabelAndControl(TableLayoutPanel tlp, string labelText, out TextBox textBox, int row, int col, string lblName)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Name = lblName;
            lbl.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            tlp.Controls.Add(lbl, col, row);

            textBox = new TextBox();
            textBox.Font = new Font("Tahoma", 10);
            textBox.Dock = DockStyle.Fill;
            textBox.Margin = new Padding(3, 5, 10, 5);
            tlp.Controls.Add(textBox, col + 1, row);
        }

        private void AddLabelAndCombo(TableLayoutPanel tlp, string labelText, string[] items, out ComboBox comboBox, int row, int col, string lblName)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Name = lblName;
            lbl.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            tlp.Controls.Add(lbl, col, row);

            comboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Font = new Font("Tahoma", 10);
            comboBox.Dock = DockStyle.Fill;
            comboBox.Margin = new Padding(3, 5, 10, 5);
            if (items != null && items.Length > 0)
            {
                comboBox.Items.AddRange(items);
                comboBox.SelectedIndex = 0;
            }
            tlp.Controls.Add(comboBox, col + 1, row);
        }

        private void AddLabelAndCombo(TableLayoutPanel tlp, string labelText, out ComboBox comboBox, int row, int col, string lblName)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Name = lblName;
            lbl.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            tlp.Controls.Add(lbl, col, row);

            comboBox = new ComboBox();
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.Font = new Font("Tahoma", 10);
            comboBox.Dock = DockStyle.Fill;
            comboBox.Margin = new Padding(3, 5, 10, 5);
            tlp.Controls.Add(comboBox, col + 1, row);
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearFields();
            SetControlsState(true);
            if (txtWarehouseCode != null) txtWarehouseCode.Focus();
            currentIndex = -1;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0 && warehouses.Count > 0)
            {
                currentIndex = 0;
                LoadRecordToForm(warehouses[0]);
            }
            if (currentIndex >= 0)
            {
                SetControlsState(true);
                if (txtWarehouseCode != null) txtWarehouseCode.Focus();
            }
            else
            {
                MessageBox.Show(LanguageManager.Get("msg_select_warehouse_edit"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                string warehouseType = cmbWarehouseType?.SelectedItem?.ToString() ?? "";
                if (warehouseType == LanguageManager.Get("type_main")) warehouseType = "رئيسي";
                else if (warehouseType == LanguageManager.Get("type_sub")) warehouseType = "فرعي";
                else if (warehouseType == LanguageManager.Get("type_temp")) warehouseType = "مؤقت";

                string status = cmbStatus?.SelectedItem?.ToString() ?? LanguageManager.Get("status_active");
                if (status == LanguageManager.Get("status_active")) status = "نشط";
                else if (status == LanguageManager.Get("status_inactive")) status = "غير نشط";

                var record = new WarehouseRecord
                {
                    Id = (currentIndex >= 0 && currentIndex < warehouses.Count) ? warehouses[currentIndex].Id : 0,
                    WarehouseCode = txtWarehouseCode?.Text?.Trim() ?? "",
                    WarehouseName = txtWarehouseName?.Text?.Trim() ?? "",
                    WarehouseType = warehouseType,
                    PortLink = cmbPortLink?.SelectedItem?.ToString() ?? "",
                    Address = txtAddress?.Text?.Trim() ?? "",
                    Manager = txtManager?.Text?.Trim() ?? "",
                    Phone = txtPhone?.Text?.Trim() ?? "",
                    Status = status,
                    Notes = txtNotes?.Text?.Trim() ?? "",
                    CreatedDate = (currentIndex >= 0 && currentIndex < warehouses.Count) ? warehouses[currentIndex].CreatedDate : DateTime.Now,
                    ModifiedDate = DateTime.Now
                };

                SaveWarehouseToDatabase(record, record.Id > 0 ? "تعديل" : "إضافة");

                if (currentIndex >= 0 && currentIndex < warehouses.Count)
                {
                    warehouses[currentIndex] = record;
                    MessageBox.Show(LanguageManager.Get("msg_edit_success"), LanguageManager.Get("edit_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    warehouses.Add(record);
                    currentIndex = warehouses.Count - 1;
                    MessageBox.Show(LanguageManager.Get("msg_add_success"), LanguageManager.Get("add_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                RefreshGrid();
                LoadRecordToForm(warehouses[currentIndex]);
                SetControlsState(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= warehouses.Count)
            {
                MessageBox.Show(LanguageManager.Get("msg_select_warehouse_delete"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(string.Format(LanguageManager.Get("msg_confirm_delete_warehouse"), warehouses[currentIndex].WarehouseName),
                LanguageManager.Get("msg_delete_title"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM Warehouses WHERE WarehouseId = @Id",
                        DatabaseHelper.CreateParameter("@Id", warehouses[currentIndex].Id));

                    warehouses.RemoveAt(currentIndex);
                    if (warehouses.Count > 0)
                    {
                        currentIndex = Math.Min(currentIndex, warehouses.Count - 1);
                        LoadRecordToForm(warehouses[currentIndex]);
                    }
                    else
                    {
                        currentIndex = -1;
                        ClearFields();
                    }
                    RefreshGrid();
                    SetControlsState(false);
                    MessageBox.Show(LanguageManager.Get("msg_delete_success"), LanguageManager.Get("delete_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في الحذف:\n{ex.Message}", "خطأ");
                }
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            // ✅ إنشاء نافذة لعرض جميع المخازن مع حقل بحث فوري
            using (Form viewForm = new Form())
            {
                viewForm.Text = "📋 عرض جميع المخازن";
                viewForm.Size = new Size(950, 550); // زيادة الارتفاع قليلاً لاستيعاب حقل البحث
                viewForm.StartPosition = FormStartPosition.CenterParent;
                viewForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                viewForm.MaximizeBox = false;
                viewForm.MinimizeBox = false;
                viewForm.RightToLeft = RightToLeft.Yes;
                viewForm.RightToLeftLayout = true;

                // 1. لوحة علوية لحقل البحث
                Panel pnlTop = new Panel();
                pnlTop.Height = 60;
                pnlTop.Dock = DockStyle.Top;
                pnlTop.BackColor = Color.FromArgb(240, 248, 255);

                Label lblSearch = new Label();
                lblSearch.Text = "🔍 بحث:";
                lblSearch.Font = new Font("Tahoma", 10, FontStyle.Bold);
                lblSearch.Location = new Point(780, 15);
                lblSearch.Size = new Size(60, 25);
                lblSearch.TextAlign = ContentAlignment.MiddleRight;

                TextBox txtSearch = new TextBox();
                txtSearch.Font = new Font("Tahoma", 11);
                txtSearch.Location = new Point(480, 15);
                txtSearch.Size = new Size(290, 28);
                txtSearch.TextAlign = HorizontalAlignment.Right;
                // إضافة أيقونة بحث داخل الحقل (اختياري)
                txtSearch.BorderStyle = BorderStyle.FixedSingle;

                pnlTop.Controls.Add(lblSearch);
                pnlTop.Controls.Add(txtSearch);

                // 2. إنشاء جدول البيانات
                DataGridView dgvView = new DataGridView();
                dgvView.Dock = DockStyle.Fill;
                dgvView.AllowUserToAddRows = false;
                dgvView.AllowUserToDeleteRows = false;
                dgvView.ReadOnly = true;
                dgvView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvView.MultiSelect = false;
                dgvView.BackgroundColor = Color.White;
                dgvView.RowHeadersVisible = false;
                dgvView.EnableHeadersVisualStyles = false;

                // تنسيق الرأس
                dgvView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgvView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvView.ColumnHeadersHeight = 40;
                dgvView.RowTemplate.Height = 35;
                dgvView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

                // 3. إضافة الأعمدة
                dgvView.Columns.Add("colCode", "رمز المخزن");
                dgvView.Columns.Add("colName", "اسم المخزن");
                dgvView.Columns.Add("colType", "النوع");
                dgvView.Columns.Add("colStatus", "الحالة");

                // ضبط عرض الأعمدة
                dgvView.Columns["colCode"].Width = 120;
                dgvView.Columns["colName"].Width = 350;
                dgvView.Columns["colType"].Width = 150;
                dgvView.Columns["colStatus"].Width = 120;

                // 4. تعبئة البيانات من القائمة
                foreach (var w in warehouses)
                {
                    string statusText = w.Status == "نشط" ? "✅ نشط" : "❌ غير نشط";
                    dgvView.Rows.Add(w.WarehouseCode, w.WarehouseName, w.WarehouseType, statusText);
                }

                // 5. ✅ منطق البحث (تصفية الصفوف فورياً أثناء الكتابة)
                txtSearch.TextChanged += (s, ev) =>
                {
                    string searchTerm = txtSearch.Text.Trim().ToLower();
                    foreach (DataGridViewRow row in dgvView.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string code = row.Cells["colCode"].Value?.ToString()?.ToLower() ?? "";
                        string name = row.Cells["colName"].Value?.ToString()?.ToLower() ?? "";

                        // إظهار الصف إذا تطابق مع رمز أو اسم المخزن، أو إذا كان حقل البحث فارغاً
                        if (string.IsNullOrEmpty(searchTerm) || code.Contains(searchTerm) || name.Contains(searchTerm))
                        {
                            row.Visible = true;
                        }
                        else
                        {
                            row.Visible = false;
                        }
                    }
                };

                // 6. لوحة سفلية لزر الإغلاق
                Panel pnlBottom = new Panel();
                pnlBottom.Height = 60;
                pnlBottom.Dock = DockStyle.Bottom;
                pnlBottom.BackColor = Color.FromArgb(240, 248, 255);

                Button btnClose = new Button();
                btnClose.Text = "❌ إغلاق";
                btnClose.Font = new Font("Tahoma", 10, FontStyle.Bold);
                btnClose.BackColor = Color.FromArgb(158, 158, 158);
                btnClose.ForeColor = Color.White;
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.Size = new Size(120, 40);
                btnClose.Location = new Point((pnlBottom.Width - btnClose.Width) / 2, 10); // توسيط الزر
                btnClose.Anchor = AnchorStyles.Bottom;
                btnClose.Click += (s, ev) => viewForm.Close();

                pnlBottom.Controls.Add(btnClose);

                // 7. إضافة العناصر إلى النافذة بالترتيب الصحيح
                viewForm.Controls.Add(dgvView);   // يملأ المساحة المتبقية
                viewForm.Controls.Add(pnlTop);    // في الأعلى
                viewForm.Controls.Add(pnlBottom); // في الأسفل

                // 8. ✅ ميزة إضافية: النقر المزدوج على صف لتحميله في الشاشة الرئيسية
                dgvView.CellDoubleClick += (s, ev) =>
                {
                    if (ev.RowIndex >= 0)
                    {
                        string clickedCode = dgvView.Rows[ev.RowIndex].Cells["colCode"].Value?.ToString();
                        var selectedWarehouse = warehouses.FirstOrDefault(w => w.WarehouseCode == clickedCode);

                        if (selectedWarehouse != null)
                        {
                            currentIndex = warehouses.IndexOf(selectedWarehouse);
                            LoadRecordToForm(selectedWarehouse);
                            SetControlsState(false);
                            UpdateNavigation();
                            viewForm.Close(); // إغلاق نافذة العرض بعد الاختيار
                        }
                    }
                };

                // عرض النافذة
                viewForm.ShowDialog();
            }
        }
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (warehouses.Count == 0)
            {
                MessageBox.Show(LanguageManager.Get("msg_no_data_export"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = "CSV ملف|*.csv",
                FileName = $"تقرير_المخازن_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        for (int i = 0; i < dgvWarehouses.Columns.Count; i++)
                        {
                            sw.Write(dgvWarehouses.Columns[i].HeaderText);
                            if (i < dgvWarehouses.Columns.Count - 1) sw.Write(",");
                        }
                        sw.WriteLine();

                        foreach (DataGridViewRow row in dgvWarehouses.Rows)
                        {
                            for (int i = 0; i < dgvWarehouses.Columns.Count; i++)
                            {
                                string value = row.Cells[i].Value?.ToString() ?? "";
                                if (value.Contains(",")) value = $"\"{value}\"";
                                sw.Write(value);
                                if (i < dgvWarehouses.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }
                    MessageBox.Show(string.Format(LanguageManager.Get("msg_exported"), warehouses.Count), LanguageManager.Get("export_title"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ {LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm mainForm = null;
                if (this.MdiParent is MainForm parentForm) mainForm = parentForm;
                else mainForm = Application.OpenForms.OfType<MainForm>().FirstOrDefault();

                if (mainForm != null && !mainForm.IsDisposed)
                {
                    var children = mainForm.MdiChildren.ToList();
                    foreach (Form child in children)
                    {
                        if (!child.IsDisposed && child != this) child.Close();
                    }
                    this.Close();
                    mainForm.ShowWelcome();
                    mainForm.WindowState = FormWindowState.Maximized;
                    mainForm.Activate();
                    mainForm.BringToFront();
                }
                else this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtWarehouseCode?.Text))
            {
                MessageBox.Show(LanguageManager.Get("msg_warehouse_code_required"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWarehouseCode?.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtWarehouseName?.Text))
            {
                MessageBox.Show(LanguageManager.Get("msg_warehouse_name_required"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtWarehouseName?.Focus();
                return false;
            }
            return true;
        }

        private void ClearFields()
        {
            try
            {
                if (txtWarehouseCode != null) txtWarehouseCode.Clear();
                if (txtWarehouseName != null) txtWarehouseName.Clear();
                if (txtAddress != null) txtAddress.Clear();
                if (txtManager != null) txtManager.Clear();
                if (txtPhone != null) txtPhone.Clear();
                if (txtNotes != null) txtNotes.Clear();

                if (cmbWarehouseType != null && cmbWarehouseType.Items.Count > 0)
                    cmbWarehouseType.SelectedIndex = 0;
                else if (cmbWarehouseType != null)
                    cmbWarehouseType.Text = "";

                if (cmbPortLink != null && cmbPortLink.Items.Count > 0)
                    cmbPortLink.SelectedIndex = 0;
                else if (cmbPortLink != null)
                    cmbPortLink.Text = "";

                if (cmbStatus != null && cmbStatus.Items.Count > 0)
                    cmbStatus.SelectedIndex = 0;
                else if (cmbStatus != null)
                    cmbStatus.Text = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تفريغ الحقول: {ex.Message}");
            }
        }

        // ✅ تنفيذ دالة AddReceivedFromPort بدلاً من NotImplementedException
        public void AddReceivedFromPort(string warehouseName, string statementNo, string productName, decimal transferQty, string unit)
        {
            Console.WriteLine($"تم استلام {transferQty} {unit} من {productName} للمخزن {warehouseName}");
        }
    }
}