using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Data.SQLite;

namespace YemenCustomsSystem
{
    public partial class TariffRequestForm : Form
    {
        private List<TariffRequestRecord> requests = new List<TariffRequestRecord>();
        private List<PortsForm.PortData> allPorts = new List<PortsForm.PortData>();
        private List<ItemsDataForm.ItemsRecord> allItemsData = new List<ItemsDataForm.ItemsRecord>();

        private string currentPortClassification = "";
        private string currentPortGroupName = "";
        private bool _enterNavigation = false;
        private bool isEditingMode = false;
        private bool hasChanges = false;

        // ==================== ✅ متغيرات نظام الاعتماد ====================
        private ToolStripButton btnViewAllRequestsStrip, btnApproveStrip;
        private TextBox txtApprovalStatus;
        private int currentApprovalLevel = 0;
        private bool canApprove = false;

        private ToolStrip toolStrip1;
        private ToolStripButton btnSaveStrip, btnNewStrip, btnEditStrip, btnDeleteStrip, btnExitStrip, btnHome, btnShowStrip;
        private ToolStripSeparator toolStripSeparator1, toolStripSeparator2;

        private TabControl tabControlMain;
        private TabPage tabPageMain;
        private TabPage tabPageOther;
        private TabPage tabPageAdditional;

        private ComboBox cmbPortName, cmbTariffType;
        private DateTimePicker dtpStatementDate;
        private TextBox txtStatementNo;
        private DataGridView dgvData;

        private NumericUpDown numVehicleCount;
        private DataGridView dgvDrivers;
        private DataGridView dgvAdditionalAmounts;

        private Label lblTotalFeesTitle, lblTotalFeesValue;
        private Label lblOtherFeesTitle;
        private TextBox txtOtherFees;
        private Label lblTotalAmountTitle, lblTotalAmountValue;

        public class DriverInfo
        {
            public string DriverId { get; set; }
            public string DriverName { get; set; }
            public string VehicleNo { get; set; }
        }

        public class TariffRequestRecord
        {
            public int Id { get; set; }
            public string StatementNo { get; set; }
            public string PortName { get; set; }
            public string TariffType { get; set; }
            public string ClientName { get; set; }
            public string CarNo { get; set; }
            public DateTime StatementDate { get; set; }
            public string DriversJson { get; set; }
            public string AdditionalAmountsJson { get; set; }
            public string ItemsJson { get; set; }
            public string Status { get; set; }
            public string CreatedBy { get; set; }
            public string CreatedDate { get; set; }
            public string ReviewedBy { get; set; }
            public string ReviewNotes { get; set; }
        }

        public TariffRequestForm()
        {
            EnsureTariffRequestsTable();
            EnsureApprovalLogTable();
            EnsureAdditionalFeesTable(); // ✅ ضمان وجود جدول المبالغ الإضافية
            CreateControlsManually();
            InitializeGrid();
            LoadPortsFromDatabase();
            LoadItemsDataFromDatabase();
            LoadRequestsFromDatabase();

            ClearFields();
            SetControlsState(true);
            txtStatementNo.Focus();
        }

        // ==================== ✅ ضمان وجود جدول المبالغ الإضافية ====================
        private void EnsureAdditionalFeesTable()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS AdditionalFees (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    FeeNo TEXT,
                    FeeName TEXT,
                    EffectType TEXT,
                    Currency TEXT,
                    ValueType TEXT,
                    FeeScope TEXT,
                    PortId TEXT,
                    PortName TEXT,
                    PortGroupId TEXT,
                    PortGroupName TEXT,
                    ItemId TEXT,
                    ItemName TEXT,
                    ItemGroupId TEXT,
                    ItemGroupName TEXT,
                    MinLimit REAL DEFAULT 0,
                    CalcType TEXT,
                    Amount REAL DEFAULT 0,
                    CreatedDate TEXT DEFAULT (datetime('now','localtime'))
                )");
            }
            catch { }
        }

        private void EnsureApprovalLogTable()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS ApprovalLog (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    StatementNo TEXT NOT NULL,
                    PortName TEXT NOT NULL,
                    LevelId INTEGER NOT NULL,
                    LevelNo INTEGER NOT NULL,
                    LevelName TEXT NOT NULL,
                    UserId INTEGER NOT NULL,
                    UserName TEXT NOT NULL,
                    ApprovalStatus TEXT NOT NULL,
                    Notes TEXT,
                    ApprovalDate TEXT DEFAULT (datetime('now','localtime')),
                    UNIQUE(StatementNo, PortName, LevelId)
                )");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ إنشاء جدول سجل الاعتمادات: {ex.Message}");
            }
        }

        private void EnsureTariffRequestsTable()
        {
            try
            {
                DatabaseHelper.ExecuteNonQuery(@"CREATE TABLE IF NOT EXISTS TariffRequests (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            StatementNo TEXT NOT NULL,
            PortName TEXT NOT NULL,
            TariffType TEXT,
            ClientName TEXT,
            CarNo TEXT,
            StatementDate TEXT,
            DriversJson TEXT,
            AdditionalAmountsJson TEXT,
            ItemsJson TEXT,
            Status TEXT DEFAULT 'pending',
            CreatedBy TEXT,
            CreatedDate TEXT,
            ReviewedBy TEXT,
            ReviewedDate TEXT,
            ReviewNotes TEXT,
            ModifiedDate TEXT
        )");

                // ✅ إضافة العمود إذا كان الجدول موجوداً مسبقاً
                try
                {
                    DatabaseHelper.ExecuteNonQuery("ALTER TABLE TariffRequests ADD COLUMN ModifiedDate TEXT");
                }
                catch { } // تجاهل الخطأ إذا كان العمود موجوداً بالفعل
            }
            catch { }
        }

        private void CreateControlsManually()
        {
            this.Text = "📝 طلب ترسيم - نظام الجمارك اليمنية";
            this.Size = new Size(1500, 800);
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            toolStrip1 = new ToolStrip();
            toolStrip1.RightToLeft = RightToLeft.Yes;
            toolStrip1.BackColor = Color.FromArgb(52, 73, 94);
            toolStrip1.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            toolStrip1.AutoSize = true;
            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;

            btnHome = new ToolStripButton("🏠 الرئيسية");
            btnHome.ForeColor = Color.White;
            btnHome.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnHome.Click += BtnHome_Click;
            toolStripSeparator1 = new ToolStripSeparator();

            btnSaveStrip = new ToolStripButton("💾 حفظ");
            btnSaveStrip.ForeColor = Color.White;
            btnSaveStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSaveStrip.Click += BtnSave_Click;

            btnNewStrip = new ToolStripButton("🆕 جديد");
            btnNewStrip.ForeColor = Color.White;
            btnNewStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnNewStrip.Click += BtnNew_Click;

            toolStripSeparator2 = new ToolStripSeparator();

            btnEditStrip = new ToolStripButton("✏️ تعديل");
            btnEditStrip.ForeColor = Color.White;
            btnEditStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnEditStrip.Click += BtnEdit_Click;

            btnDeleteStrip = new ToolStripButton("🗑️ حذف");
            btnDeleteStrip.ForeColor = Color.White;
            btnDeleteStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnDeleteStrip.Click += BtnDelete_Click;

            btnShowStrip = new ToolStripButton("📋 عرض طلباتي");
            btnShowStrip.ForeColor = Color.White;
            btnShowStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnShowStrip.Click += BtnShow_Click;

            btnViewAllRequestsStrip = new ToolStripButton("📋 عرض جميع الطلبات");
            btnViewAllRequestsStrip.ForeColor = Color.White;
            btnViewAllRequestsStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnViewAllRequestsStrip.Visible = false;
            btnViewAllRequestsStrip.Click += BtnViewAllRequestsStrip_Click;

            ToolStripButton btnSearchAllRequests = new ToolStripButton("🔍 بحث عن جميع الطلبات");
            btnSearchAllRequests.ForeColor = Color.White;
            btnSearchAllRequests.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnSearchAllRequests.Visible = false;
            btnSearchAllRequests.Click += BtnSearchAllRequests_Click;

            toolStrip1.Items.Add(btnSearchAllRequests);
            CheckUserApprovalPermissions(btnSearchAllRequests);

            btnApproveStrip = new ToolStripButton("✅ اعتماد الطلب");
            btnApproveStrip.ForeColor = Color.White;
            btnApproveStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnApproveStrip.Visible = false;
            btnApproveStrip.Click += BtnApproveStrip_Click;

            btnExitStrip = new ToolStripButton("🚪 خروج");
            btnExitStrip.ForeColor = Color.White;
            btnExitStrip.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnExitStrip.Click += (s, ev) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] {
                btnHome, toolStripSeparator1, btnSaveStrip, btnNewStrip, toolStripSeparator2,
                btnEditStrip, btnDeleteStrip, btnShowStrip, btnViewAllRequestsStrip, btnApproveStrip, btnExitStrip
            });

            tabControlMain = new TabControl();
            tabControlMain.Dock = DockStyle.Fill;
            tabControlMain.RightToLeft = RightToLeft.Yes;
            tabControlMain.RightToLeftLayout = true;
            tabControlMain.Alignment = TabAlignment.Top;

            tabPageMain = new TabPage("الرئيسية");
            tabPageOther = new TabPage("بيانات أخرى");
            tabPageAdditional = new TabPage("المبالغ الإضافية");

            tabControlMain.TabPages.Add(tabPageMain);
            tabControlMain.TabPages.Add(tabPageOther);
            tabControlMain.TabPages.Add(tabPageAdditional);

            this.Controls.Add(tabControlMain);
            this.Controls.Add(toolStrip1);

            Panel pnlHeader = new Panel();
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Height = 160;
            pnlHeader.BackColor = Color.FromArgb(240, 248, 255);
            pnlHeader.Padding = new Padding(10);

            Label lblTariffType = new Label { Text = "نوع الترسيم :", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 15), Size = new Size(140, 25), TextAlign = ContentAlignment.MiddleRight };
            cmbTariffType = new ComboBox { DropDownStyle = ComboBoxStyle.DropDown, Font = new Font("Tahoma", 10), Location = new Point(960, 15), Size = new Size(180, 27) };
            cmbTariffType.KeyDown += CmbTariffType_KeyDown;

            Label lblPortName = new Label { Text = "اسم المنفذ:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(810, 15), Size = new Size(90, 25), TextAlign = ContentAlignment.MiddleRight };
            cmbPortName = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 10), Location = new Point(658, 15), Size = new Size(140, 27) };
            cmbPortName.Items.Add("-- اختر منفذ --");
            cmbPortName.SelectedIndex = 0;
            cmbPortName.SelectedIndexChanged += CmbPortName_SelectedIndexChanged;

            Label lblStatementNo = new Label { Text = "رقم البيان:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(1150, 55), Size = new Size(110, 25), TextAlign = ContentAlignment.MiddleRight };
            txtStatementNo = new TextBox { Font = new Font("Tahoma", 10), Location = new Point(960, 55), Size = new Size(180, 27), TextAlign = HorizontalAlignment.Right, BackColor = Color.FromArgb(255, 255, 190) };
            txtStatementNo.KeyDown += TxtStatementNo_KeyDown;

            Button btnAutoGenerate = new Button { Text = "🔄 توليد تلقائي", Font = new Font("Tahoma", 9, FontStyle.Bold), BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Location = new Point(810, 55), Size = new Size(140, 27) };
            btnAutoGenerate.Click += (s, ev) => txtStatementNo.Text = GenerateStatementNo();

            Label lblStatementDate = new Label { Text = "تاريخ البيان:", Font = new Font("Tahoma", 10, FontStyle.Bold), Location = new Point(658, 55), Size = new Size(90, 25), TextAlign = ContentAlignment.MiddleRight };
            dtpStatementDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Tahoma", 10), Location = new Point(480, 55), Size = new Size(140, 27) };

            Label lblApprovalStatus = new Label
            {
                Text = "حالة الاعتماد:",
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(211, 47, 47),
                Location = new Point(480, 15),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleRight
            };

            txtApprovalStatus = new TextBox
            {
                Font = new Font("Tahoma", 10, FontStyle.Bold),
                Location = new Point(300, 15),
                Size = new Size(170, 27),
                TextAlign = HorizontalAlignment.Center,
                ReadOnly = true,
                BackColor = Color.FromArgb(255, 243, 224),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnShowApprovalStatus = new Button
            {
                Text = "📊 حالة الاعتماد",
                Font = new Font("Tahoma", 9, FontStyle.Bold),
                BackColor = Color.FromArgb(156, 39, 176),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(100, 15),
                Size = new Size(150, 27)
            };
            btnShowApprovalStatus.Click += BtnShowApprovalStatus_Click;

            pnlHeader.Controls.AddRange(new Control[] {
                lblTariffType, cmbTariffType, lblPortName, cmbPortName,
                lblStatementNo, txtStatementNo, btnAutoGenerate, lblStatementDate, dtpStatementDate,
                lblApprovalStatus, txtApprovalStatus, btnShowApprovalStatus
            });

            dgvData = new DataGridView { Dock = DockStyle.Fill };
            tabPageMain.Controls.Add(dgvData);
            tabPageMain.Controls.Add(pnlHeader);

            Panel pnlOther = new Panel();
            pnlOther.Dock = DockStyle.Fill;
            pnlOther.BackColor = Color.White;
            pnlOther.Padding = new Padding(15);

            Label lblVehicleCount = new Label { Text = "عدد سيارات النقل:", Font = new Font("Tahoma", 11, FontStyle.Bold), ForeColor = Color.FromArgb(51, 122, 183), Location = new Point(1100, 15), Size = new Size(160, 25), TextAlign = ContentAlignment.MiddleRight };
            numVehicleCount = new NumericUpDown { Font = new Font("Tahoma", 11, FontStyle.Bold), Location = new Point(1000, 15), Size = new Size(100, 27), Minimum = 0, Maximum = 50, Value = 0 };

            // ✅ ربط حدث تغيير عدد السيارات لتحديث المبالغ الإضافية
            numVehicleCount.ValueChanged += (s, ev) =>
            {
                int count = (int)numVehicleCount.Value;
                dgvDrivers.Rows.Clear();
                for (int i = 1; i <= count; i++)
                {
                    dgvDrivers.Rows.Add(i, "", "", "");
                }
                // ✅ إعادة احتساب المبالغ الإضافية (لمستوى القاطرة)
                RefreshAutoAdditionalAmounts();
            };

            dgvDrivers = new DataGridView();
            dgvDrivers.Location = new Point(20, 55);
            dgvDrivers.Size = new Size(180, 450);
            dgvDrivers.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvDrivers.BackgroundColor = Color.White;
            dgvDrivers.BorderStyle = BorderStyle.Fixed3D;
            dgvDrivers.RowHeadersVisible = false;
            dgvDrivers.AllowUserToAddRows = false;
            dgvDrivers.AllowUserToDeleteRows = true;
            dgvDrivers.ReadOnly = false;
            dgvDrivers.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvDrivers.EnableHeadersVisualStyles = false;
            dgvDrivers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvDrivers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDrivers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDrivers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvDrivers.ColumnHeadersHeight = 40;
            dgvDrivers.RowTemplate.Height = 35;
            dgvDrivers.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvDrivers.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (dgvDrivers.Columns.Count == 0)
            {
                dgvDrivers.Columns.Add("colRowNum", "م");
                dgvDrivers.Columns.Add("colDriverId", "رقم السائق");
                dgvDrivers.Columns.Add("colDriverName", "اسم السائق");
                dgvDrivers.Columns.Add("colVehicleNo", "رقم السيارة");

                dgvDrivers.Columns["colRowNum"].Width = 50;
                dgvDrivers.Columns["colRowNum"].ReadOnly = true;
                dgvDrivers.Columns["colRowNum"].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);

                dgvDrivers.Columns["colDriverId"].Width = 150;
                dgvDrivers.Columns["colDriverName"].Width = 300;
                dgvDrivers.Columns["colVehicleNo"].Width = 200;
            }

            pnlOther.Controls.Add(lblVehicleCount);
            pnlOther.Controls.Add(numVehicleCount);
            pnlOther.Controls.Add(dgvDrivers);
            tabPageOther.Controls.Add(pnlOther);

            tabPageAdditional.Padding = new Padding(0);

            dgvAdditionalAmounts = new DataGridView();
            dgvAdditionalAmounts.Dock = DockStyle.Fill;
            dgvAdditionalAmounts.AutoGenerateColumns = false;
            dgvAdditionalAmounts.AllowUserToAddRows = true;
            dgvAdditionalAmounts.AllowUserToDeleteRows = true;
            dgvAdditionalAmounts.BackgroundColor = Color.White;
            dgvAdditionalAmounts.BorderStyle = BorderStyle.None;
            dgvAdditionalAmounts.RowHeadersVisible = false;
            dgvAdditionalAmounts.RightToLeft = RightToLeft.Yes;
            dgvAdditionalAmounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAdditionalAmounts.EnableHeadersVisualStyles = false;

            dgvAdditionalAmounts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvAdditionalAmounts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvAdditionalAmounts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvAdditionalAmounts.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAdditionalAmounts.ColumnHeadersHeight = 45;
            dgvAdditionalAmounts.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvAdditionalAmounts.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvAdditionalAmounts.RowTemplate.Height = 40;
            dgvAdditionalAmounts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

            if (dgvAdditionalAmounts.Columns.Count == 0)
            {
                dgvAdditionalAmounts.Columns.Add("colItemName", "البند");
                dgvAdditionalAmounts.Columns.Add("colAmount", "المبلغ");

                dgvAdditionalAmounts.Columns["colItemName"].FillWeight = 70;
                dgvAdditionalAmounts.Columns["colAmount"].FillWeight = 30;

                dgvAdditionalAmounts.Columns["colAmount"].DefaultCellStyle.Format = "N2";
                dgvAdditionalAmounts.Columns["colAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvAdditionalAmounts.Columns["colAmount"].DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvAdditionalAmounts.Columns["colAmount"].DefaultCellStyle.ForeColor = Color.FromArgb(255, 152, 0);
            }

            dgvAdditionalAmounts.CellValueChanged += DgvAdditionalAmounts_CellValueChanged;

            tabPageAdditional.Controls.Add(dgvAdditionalAmounts);

            Panel pnlBottom = new Panel();
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Height = 80;
            pnlBottom.BackColor = Color.FromArgb(240, 248, 255);
            pnlBottom.Padding = new Padding(10);

            lblTotalFeesTitle = new Label { Text = "إجمالي رسوم البيان:", Font = new Font("Tahoma", 11, FontStyle.Bold), ForeColor = Color.FromArgb(33, 150, 243), Location = new Point(1050, 10), Size = new Size(180, 25), TextAlign = ContentAlignment.MiddleRight };
            lblTotalFeesValue = new Label { Text = "0.00", Font = new Font("Tahoma", 13, FontStyle.Bold), ForeColor = Color.FromArgb(76, 175, 80), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(820, 10), Size = new Size(220, 30) };

            lblOtherFeesTitle = new Label { Text = "رسوم أخرى:", Font = new Font("Tahoma", 10, FontStyle.Bold), ForeColor = Color.FromArgb(255, 152, 0), Location = new Point(1050, 50), Size = new Size(180, 25), TextAlign = ContentAlignment.MiddleRight };
            txtOtherFees = new TextBox { Text = "0", Font = new Font("Tahoma", 11, FontStyle.Bold), ForeColor = Color.FromArgb(255, 152, 0), BackColor = Color.FromArgb(255, 248, 225), BorderStyle = BorderStyle.FixedSingle, TextAlign = HorizontalAlignment.Center, Location = new Point(820, 50), Size = new Size(220, 30) };

            lblTotalAmountTitle = new Label { Text = "💵 إجمالي مبلغ البيان:", Font = new Font("Tahoma", 12, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(33, 150, 243), Location = new Point(250, 5), Size = new Size(200, 70), TextAlign = ContentAlignment.MiddleCenter, BorderStyle = BorderStyle.FixedSingle };
            lblTotalAmountValue = new Label { Text = "0.00", Font = new Font("Tahoma", 18, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.FromArgb(76, 175, 80), BorderStyle = BorderStyle.FixedSingle, TextAlign = ContentAlignment.MiddleCenter, Location = new Point(10, 5), Size = new Size(300, 70) };

            pnlBottom.Controls.AddRange(new Control[] { lblTotalFeesTitle, lblTotalFeesValue, lblOtherFeesTitle, txtOtherFees, lblTotalAmountTitle, lblTotalAmountValue });
            this.Controls.Add(pnlBottom);
        }

        private void CheckUserApprovalPermissions(ToolStripButton searchButton)
        {
            try
            {
                var currentUser = UserManager.GetCurrentUser();
                if (currentUser == null) return;

                var levels = ApprovalLevelsForm.GetApprovalLevels("TariffRequest");

                foreach (var level in levels)
                {
                    var users = ApprovalLevelsForm.GetLevelUsers(level.Id);
                    if (users.Any(u => u.UserNo == currentUser.Id.ToString() || u.UserName == currentUser.FullName))
                    {
                        searchButton.Visible = true;
                        return;
                    }
                }
            }
            catch { }
        }

        private void BtnSearchAllRequests_Click(object sender, EventArgs e)
        {
            try
            {
                Form searchForm = new Form();
                searchForm.Text = "🔍 بحث عن جميع طلبات الترسيم";
                searchForm.Size = new Size(1400, 700);
                searchForm.StartPosition = FormStartPosition.CenterParent;
                searchForm.WindowState = FormWindowState.Maximized;
                searchForm.RightToLeft = RightToLeft.Yes;
                searchForm.RightToLeftLayout = true;

                Panel pnlSearch = new Panel
                {
                    Dock = DockStyle.Top,
                    Height = 70,
                    BackColor = Color.FromArgb(240, 248, 255),
                    Padding = new Padding(10)
                };

                Label lblSearchText = new Label
                {
                    Text = "🔍 ابحث برقم البيان أو اسم المخلص:",
                    Font = new Font("Tahoma", 11, FontStyle.Bold),
                    Location = new Point(10, 20),
                    Size = new Size(250, 25),
                    TextAlign = ContentAlignment.MiddleRight
                };

                TextBox txtSearch = new TextBox
                {
                    Font = new Font("Tahoma", 11),
                    Location = new Point(270, 20),
                    Size = new Size(300, 27),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Button btnSearch = new Button
                {
                    Text = "🔍 بحث",
                    Font = new Font("Tahoma", 10, FontStyle.Bold),
                    BackColor = Color.FromArgb(33, 150, 243),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Location = new Point(580, 18),
                    Size = new Size(100, 32)
                };

                pnlSearch.Controls.AddRange(new Control[] { lblSearchText, txtSearch, btnSearch });

                DataGridView dgv = new DataGridView
                {
                    Dock = DockStyle.Fill,
                    ReadOnly = true,
                    AllowUserToAddRows = false,
                    SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                    MultiSelect = false,
                    EnableHeadersVisualStyles = false,
                    ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
                    ColumnHeadersHeight = 45
                };

                dgv.Columns.Add("colStatementNo", "رقم البيان");
                dgv.Columns.Add("colPortName", "المنفذ");
                dgv.Columns.Add("colTariffType", "نوع الترسيم");
                dgv.Columns.Add("colCreatedBy", "المخلص");
                dgv.Columns.Add("colCreatedDate", "تاريخ الإنشاء");
                dgv.Columns.Add("colStatus", "حالة الاعتماد");
                dgv.Columns.Add("colApprovedLevels", "المستويات المعتمدة");

                dgv.Columns["colStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgv.Columns["colApprovedLevels"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                void LoadData(string searchText = "")
                {
                    dgv.Rows.Clear();
                    try
                    {
                        string query = @"
                    SELECT DISTINCT tr.StatementNo, tr.PortName, tr.TariffType, tr.CreatedBy, tr.CreatedDate, tr.Status,
                    (SELECT COUNT(*) FROM ApprovalLog al 
                     WHERE al.StatementNo = tr.StatementNo AND al.PortName = tr.PortName AND al.ApprovalStatus = 'approved') as ApprovedLevels
                    FROM TariffRequests tr";

                        if (!string.IsNullOrWhiteSpace(searchText))
                        {
                            query += " WHERE tr.StatementNo LIKE @Search OR tr.CreatedBy LIKE @Search";
                        }
                        query += " ORDER BY tr.CreatedDate DESC";
                        DataTable dt;
                        if (string.IsNullOrWhiteSpace(searchText))
                        {
                            dt = DatabaseHelper.ExecuteQuery(query);
                        }
                        else
                        {
                            dt = DatabaseHelper.ExecuteQuery(query, DatabaseHelper.CreateParameter("@Search", $"%{searchText}%"));
                        }
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            foreach (DataRow row in dt.Rows)
                            {
                                try
                                {
                                    string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "pending";
                                    string statementNo = row["StatementNo"] != DBNull.Value ? row["StatementNo"].ToString() : "";
                                    string portName = row["PortName"] != DBNull.Value ? row["PortName"].ToString() : "";
                                    string tariffType = row["TariffType"] != DBNull.Value ? row["TariffType"].ToString() : "-";
                                    string createdBy = row["CreatedBy"] != DBNull.Value ? row["CreatedBy"].ToString() : "-";
                                    DateTime createdDate = row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]) : DateTime.Now;

                                    int approvedLevels = 0;
                                    if (row["ApprovedLevels"] != DBNull.Value)
                                    {
                                        approvedLevels = Convert.ToInt32(row["ApprovedLevels"]);
                                    }

                                    int totalLevels = 0;
                                    try
                                    {
                                        var allLevels = ApprovalLevelsForm.GetApprovalLevels("TariffRequest");
                                        totalLevels = allLevels?.Count ?? 0;
                                    }
                                    catch
                                    {
                                        totalLevels = 0;
                                    }

                                    string statusText = "";
                                    Color statusColor = Color.Black;

                                    if (status == "approved") { statusText = "✅ معتمد كلياً"; statusColor = Color.FromArgb(76, 175, 80); }
                                    else if (approvedLevels > 0) { statusText = $"🔄 تحت المراجعة ({approvedLevels}/{totalLevels})"; statusColor = Color.FromArgb(255, 152, 0); }
                                    else { statusText = " غير معتمد"; statusColor = Color.FromArgb(244, 67, 54); }

                                    dgv.Rows.Add(
                                        statementNo,
                                        portName,
                                        tariffType,
                                        createdBy,
                                        createdDate.ToString("yyyy-MM-dd HH:mm"),
                                        statusText,
                                        $"{approvedLevels}/{totalLevels}"
                                    );

                                    int lastRow = dgv.Rows.Count - 1;
                                    dgv.Rows[lastRow].Cells["colStatus"].Style.ForeColor = statusColor;
                                    dgv.Rows[lastRow].Cells["colStatus"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"خطأ في معالجة الصف: {ex.Message}");
                                    continue;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}\n\n{ex.StackTrace}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                btnSearch.Click += (s, ev) => LoadData(txtSearch.Text);
                txtSearch.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter) LoadData(txtSearch.Text); };

                Panel pnlBottom = new Panel { Height = 70, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(10) };

                Button btnLoad = new Button
                {
                    Text = "📥 تحميل البيان المحدد",
                    Font = new Font("Tahoma", 11, FontStyle.Bold),
                    BackColor = Color.FromArgb(76, 175, 80),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(220, 45),
                    Location = new Point(250, 12)
                };

                btnLoad.Click += (s, ev) =>
                {
                    if (dgv.SelectedRows.Count > 0)
                    {
                        string stmtNo = dgv.SelectedRows[0].Cells["colStatementNo"].Value?.ToString();
                        if (!string.IsNullOrEmpty(stmtNo))
                        {
                            LoadRequestToForm(stmtNo);
                            searchForm.Close();
                        }
                    }
                    else { MessageBox.Show("⚠️ الرجاء اختيار بيان أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
                };

                Button btnClose = new Button
                {
                    Text = "❌ إغلاق",
                    Font = new Font("Tahoma", 11, FontStyle.Bold),
                    BackColor = Color.FromArgb(158, 158, 158),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(150, 45),
                    Location = new Point(50, 12),
                    DialogResult = DialogResult.Cancel
                };

                pnlBottom.Controls.Add(btnLoad);
                pnlBottom.Controls.Add(btnClose);
                searchForm.Controls.Add(dgv);
                searchForm.Controls.Add(pnlBottom);
                searchForm.Controls.Add(pnlSearch);

                LoadData();
                searchForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في فتح نافذة البحث: {ex.Message}\n\n{ex.StackTrace}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewAllRequestsStrip_Click(object sender, EventArgs e)
        {
            Form allRequestsForm = new Form();
            allRequestsForm.Text = "📋 قائمة جميع طلبات الترسيم";
            allRequestsForm.Size = new Size(1400, 700);
            allRequestsForm.StartPosition = FormStartPosition.CenterParent;
            allRequestsForm.WindowState = FormWindowState.Maximized;
            allRequestsForm.RightToLeft = RightToLeft.Yes;
            allRequestsForm.RightToLeftLayout = true;

            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                EnableHeadersVisualStyles = false,
                ColumnHeadersDefaultCellStyle = { BackColor = Color.FromArgb(33, 150, 243), ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter },
                ColumnHeadersHeight = 45

            };

            dgv.Columns.Add("colStatementNo", "رقم البيان");
            dgv.Columns.Add("colPortName", "المنفذ");
            dgv.Columns.Add("colTariffType", "نوع الترسيم");
            dgv.Columns.Add("colCreatedBy", "المخلص");
            dgv.Columns.Add("colCreatedDate", "تاريخ الإنشاء");
            dgv.Columns.Add("colStatus", "حالة الاعتماد");
            dgv.Columns.Add("colCurrentLevel", "المستوى الحالي");

            dgv.Columns["colStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["colCurrentLevel"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(@"
                    SELECT DISTINCT tr.StatementNo, tr.PortName, tr.TariffType, tr.CreatedBy, tr.CreatedDate, tr.Status,
                    (SELECT COUNT(*) FROM ApprovalLog al WHERE al.StatementNo = tr.StatementNo AND al.PortName = tr.PortName AND al.ApprovalStatus = 'approved') as ApprovedLevels
                    FROM TariffRequests tr ORDER BY tr.CreatedDate DESC");

                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string status = row["Status"].ToString();
                        int approvedLevels = Convert.ToInt32(row["ApprovedLevels"]);
                        var allLevels = ApprovalLevelsForm.GetApprovalLevels("TariffRequest");
                        int totalLevels = allLevels.Count;

                        string statusText = "";
                        Color statusColor = Color.Black;

                        if (status == "approved") { statusText = "✅ معتمد كلياً"; statusColor = Color.FromArgb(76, 175, 80); }
                        else if (approvedLevels > 0) { statusText = $"🔄 تحت المراجعة ({approvedLevels}/{totalLevels})"; statusColor = Color.FromArgb(255, 152, 0); }
                        else { statusText = "⏳ غير معتمد"; statusColor = Color.FromArgb(244, 67, 54); }

                        dgv.Rows.Add(row["StatementNo"].ToString(), row["PortName"].ToString(), row["TariffType"]?.ToString() ?? "-", row["CreatedBy"].ToString(), Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-dd HH:mm"), statusText, $"{approvedLevels}/{totalLevels}");

                        int lastRow = dgv.Rows.Count - 1;
                        dgv.Rows[lastRow].Cells["colStatus"].Style.ForeColor = statusColor;
                        dgv.Rows[lastRow].Cells["colStatus"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            Panel pnlBottom = new Panel { Height = 70, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(10) };

            Button btnLoad = new Button { Text = "📥 تحميل البيان المحدد", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(220, 45), Location = new Point(250, 12) };
            btnLoad.Click += (s, ev) =>
            {
                if (dgv.SelectedRows.Count > 0)
                {
                    string stmtNo = dgv.SelectedRows[0].Cells["colStatementNo"].Value?.ToString();
                    string portName = dgv.SelectedRows[0].Cells["colPortName"].Value?.ToString();
                    LoadRequestToForm(stmtNo);
                    CheckApprovalPermissions(portName);
                    allRequestsForm.Close();
                }
                else { MessageBox.Show("⚠️ الرجاء اختيار بيان أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            };

            Button btnClose = new Button { Text = "❌ إغلاق", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 45), Location = new Point(50, 12), DialogResult = DialogResult.Cancel };
            pnlBottom.Controls.Add(btnLoad);
            pnlBottom.Controls.Add(btnClose);
            allRequestsForm.Controls.Add(dgv);
            allRequestsForm.Controls.Add(pnlBottom);
            allRequestsForm.ShowDialog();
        }

        private void CheckApprovalPermissions(string portName)
        {
            canApprove = false;
            currentApprovalLevel = 0;
            btnApproveStrip.Visible = false;

            try
            {
                var currentUser = UserManager.GetCurrentUser();
                if (currentUser == null) return;

                var levels = ApprovalLevelsForm.GetApprovalLevels("TariffRequest");
                foreach (var level in levels.OrderBy(l => l.OrderNo))
                {
                    var users = ApprovalLevelsForm.GetLevelUsers(level.Id);
                    if (users.Any(u => u.UserNo == currentUser.Id.ToString() || u.UserName == currentUser.FullName))
                    {
                        var log = DatabaseHelper.ExecuteQuery(@"SELECT ApprovalStatus FROM ApprovalLog WHERE StatementNo = @S AND PortName = @P AND LevelId = @L",
                            DatabaseHelper.CreateParameter("@S", txtStatementNo.Text),
                            DatabaseHelper.CreateParameter("@P", portName),
                            DatabaseHelper.CreateParameter("@L", level.Id));

                        bool alreadyApproved = false;
                        if (log != null && log.Rows.Count > 0)
                        {
                            string status = log.Rows[0]["ApprovalStatus"].ToString();
                            if (status == "approved" || status == "rejected") alreadyApproved = true;
                        }

                        if (!alreadyApproved)
                        {
                            canApprove = true;
                            currentApprovalLevel = level.LevelNo;
                            btnApproveStrip.Visible = true;
                            btnApproveStrip.Text = $"✅ اعتماد (المستوى {level.LevelNo})";
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ في التحقق من الصلاحيات: {ex.Message}"); }
        }

        private void BtnApproveStrip_Click(object sender, EventArgs e)
        {
            if (!canApprove || currentApprovalLevel == 0)
            {
                MessageBox.Show("⚠️ ليس لديك صلاحية اعتماد هذا الطلب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var approvalForm = new ApprovalForm(txtStatementNo.Text, cmbPortName.SelectedItem?.ToString() ?? "", currentApprovalLevel))
            {
                if (approvalForm.ShowDialog() == DialogResult.OK)
                {
                    UpdateApprovalStatus();
                    MessageBox.Show("✅ تم تسجيل الاعتماد بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void BtnShowApprovalStatus_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatementNo.Text))
            {
                MessageBox.Show("⚠️ الرجاء تحميل بيان أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var statusForm = new ApprovalStatusForm(txtStatementNo.Text, cmbPortName.SelectedItem?.ToString() ?? ""))
            {
                statusForm.ShowDialog(this);
            }
        }

        private void UpdateApprovalStatus()
        {
            try
            {
                var levels = ApprovalLevelsForm.GetApprovalLevels("TariffRequest");
                int totalLevels = levels.Count;

                DataTable dt = DatabaseHelper.ExecuteQuery(@"SELECT COUNT(*) as ApprovedCount FROM ApprovalLog WHERE StatementNo = @S AND PortName = @P AND ApprovalStatus = 'approved'",
                    DatabaseHelper.CreateParameter("@S", txtStatementNo.Text),
                    DatabaseHelper.CreateParameter("@P", cmbPortName.SelectedItem?.ToString() ?? ""));

                int approvedCount = 0;
                if (dt != null && dt.Rows.Count > 0) approvedCount = Convert.ToInt32(dt.Rows[0]["ApprovedCount"]);

                string statusText = "";
                Color statusColor = Color.Orange;

                if (approvedCount == 0) { statusText = "⏳ غير معتمد"; statusColor = Color.FromArgb(244, 67, 54); }
                else if (approvedCount < totalLevels) { statusText = $"🔄 تحت المراجعة ({approvedCount}/{totalLevels})"; statusColor = Color.FromArgb(255, 152, 0); }
                else
                {
                    statusText = "✅ معتمد";
                    statusColor = Color.FromArgb(76, 175, 80);
                    FinalizeApproval();
                }

                txtApprovalStatus.Text = statusText;
                txtApprovalStatus.ForeColor = statusColor;
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ في تحديث الحالة: {ex.Message}"); }
        }

        private void FinalizeApproval()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM TariffRequests WHERE StatementNo = @S LIMIT 1",
                    DatabaseHelper.CreateParameter("@S", txtStatementNo.Text));

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("️ لم يتم العثور على البيان", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow row = dt.Rows[0];
                string itemsJson = row["ItemsJson"]?.ToString();
                string driversJson = row["DriversJson"]?.ToString();
                string additionalJson = row["AdditionalAmountsJson"]?.ToString();

                if (string.IsNullOrEmpty(itemsJson) || itemsJson == "null" || itemsJson == "[]")
                {
                    MessageBox.Show("⚠️ لا توجد أصناف في هذا البيان!\nالرجاء إضافة أصناف قبل الاعتماد.",
                        "بيانات ناقصة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!ApplyPermitDeductions(itemsJson, out string errorMsg))
                {
                    MessageBox.Show($"️ تم الاعتماد ولكن حدث خطأ في خصم الكميات:\n{errorMsg}",
                        "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(itemsJson);

                    foreach (var item in items)
                    {
                        string productName = GetStr(item, "ProductName");
                        decimal quantity = GetDec(item, "Quantity");
                        decimal totalWeight = GetDec(item, "TotalWeight");
                        decimal customsFee = GetDec(item, "CustomsFeeTotal");
                        decimal st = GetDec(item, "STTotal");
                        decimal vat = GetDec(item, "VATTotal");
                        decimal pt = GetDec(item, "PTTotal");
                        decimal vf = GetDec(item, "VFTotal");
                        decimal tef = GetDec(item, "TEFTotal");
                        decimal mff = GetDec(item, "MFFTotal");
                        decimal other = GetDec(item, "Other");
                        decimal total = GetDec(item, "Total");
                        string unit = GetStr(item, "Unit");
                        string weightUnit = GetStr(item, "WeightUnit");
                        string subGroupNo = GetStr(item, "SubGroupNo");

                        DatabaseHelper.ExecuteNonQuery(@"
                    INSERT INTO CustomsClearance (
                        StatementNo, PortName, StatementDate, TariffType, 
                        ProductName, Unit, WeightUnit, Quantity, WeightTotal,
                        CustomsFeeTotal, STTotal, VATTotal, PTTotal, VFTotal, TEFTotal, MFFTotal, Other, Total,
                        DriversJson, AdditionalAmountsJson, CreatedDate
                    ) VALUES (
                        @StatementNo, @PortName, @StatementDate, @TariffType,
                        @ProductName, @Unit, @WeightUnit, @Quantity, @WeightTotal,
                        @CustomsFeeTotal, @STTotal, @VATTotal, @PTTotal, @VFTotal, @TEFTotal, @MFFTotal, @Other, @Total,
                        @DriversJson, @AdditionalAmountsJson, datetime('now')
                    )",
                            DatabaseHelper.CreateParameter("@StatementNo", txtStatementNo.Text),
                            DatabaseHelper.CreateParameter("@PortName", row["PortName"]?.ToString() ?? ""),
                            DatabaseHelper.CreateParameter("@StatementDate", row["StatementDate"]?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd")),
                            DatabaseHelper.CreateParameter("@TariffType", row["TariffType"]?.ToString() ?? ""),
                            DatabaseHelper.CreateParameter("@ProductName", productName),
                            DatabaseHelper.CreateParameter("@Unit", unit),
                            DatabaseHelper.CreateParameter("@WeightUnit", weightUnit),
                            DatabaseHelper.CreateParameter("@Quantity", quantity),
                            DatabaseHelper.CreateParameter("@WeightTotal", totalWeight),
                            DatabaseHelper.CreateParameter("@CustomsFeeTotal", customsFee),
                            DatabaseHelper.CreateParameter("@STTotal", st),
                            DatabaseHelper.CreateParameter("@VATTotal", vat),
                            DatabaseHelper.CreateParameter("@PTTotal", pt),
                            DatabaseHelper.CreateParameter("@VFTotal", vf),
                            DatabaseHelper.CreateParameter("@TEFTotal", tef),
                            DatabaseHelper.CreateParameter("@MFFTotal", mff),
                            DatabaseHelper.CreateParameter("@Other", other),
                            DatabaseHelper.CreateParameter("@Total", total),
                            DatabaseHelper.CreateParameter("@DriversJson", (object)driversJson ?? DBNull.Value),
                            DatabaseHelper.CreateParameter("@AdditionalAmountsJson", (object)additionalJson ?? DBNull.Value)
                        );
                    }

                    MessageBox.Show("✅ تم الاعتماد النهائي وإنشاء سجل التخليص الجمركي",
                        "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في إدخال البيانات: {ex.Message}",
                        "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الاعتماد النهائي: {ex.Message}",
                    "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetItemsListJson()
        {
            try
            {
                var result = DatabaseHelper.ExecuteScalar(
                    "SELECT ItemsJson FROM TariffRequests WHERE StatementNo = @S LIMIT 1",
                    DatabaseHelper.CreateParameter("@S", txtStatementNo.Text));

                string json = result?.ToString();

                if (string.IsNullOrWhiteSpace(json) || json == "null")
                {
                    MessageBox.Show("⚠️ تحذير: لا توجد بيانات أصناف مسجلة لهذا البيان!\n\nالرجاء إضافة أصناف قبل الاعتماد.",
                        "بيانات ناقصة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return "[]";
                }

                return json;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في جلب بيانات الأصناف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "[]";
            }
        }

        private string GenerateStatementNo()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string prefix = "TR";
            int nextNum = 1;
            try
            {
                var result = DatabaseHelper.ExecuteScalar($"SELECT COUNT(*) FROM TariffRequests WHERE StatementNo LIKE '{prefix}-{datePart}-%'");
                if (result != null && result != DBNull.Value) nextNum = Convert.ToInt32(result) + 1;
            }
            catch { }
            return $"{prefix}-{datePart}-{nextNum:D4}";
        }

        private void CmbTariffType_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9 || e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                Form selectForm = new Form();
                selectForm.Text = "📋 اختر نوع الترسيم";
                selectForm.Size = new Size(600, 500);
                selectForm.StartPosition = FormStartPosition.CenterParent;
                selectForm.RightToLeft = RightToLeft.Yes;
                selectForm.RightToLeftLayout = true;

                DataGridView dgv = new DataGridView();
                dgv.Dock = DockStyle.Fill;
                dgv.ReadOnly = true;
                dgv.AllowUserToAddRows = false;
                dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersHeight = 40;
                dgv.Columns.Add("colTypeName", "نوع الترسيم");

                try
                {
                    DataTable dt = DatabaseHelper.ExecuteQuery("SELECT TypeNameAr FROM CustomsTariffTypes WHERE IsActive = 1 ORDER BY TypeNameAr");
                    if (dt != null) foreach (DataRow row in dt.Rows) dgv.Rows.Add(row["TypeNameAr"].ToString());
                }
                catch { }

                dgv.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter && dgv.CurrentRow != null) { ev.SuppressKeyPress = true; cmbTariffType.Text = dgv.CurrentRow.Cells["colTypeName"].Value?.ToString(); selectForm.Close(); } };
                dgv.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) { cmbTariffType.Text = dgv.Rows[ev.RowIndex].Cells["colTypeName"].Value?.ToString(); selectForm.Close(); } };

                selectForm.Controls.Add(dgv);
                selectForm.ShowDialog();
            }
        }

        private void TxtStatementNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9) { e.SuppressKeyPress = true; txtStatementNo.Text = GenerateStatementNo(); }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (dgvData != null && dgvData.Focused && dgvData.CurrentCell != null)
            {
                string colName = dgvData.CurrentCell.OwningColumn?.Name;
                if (colName == "colProductName" && (keyData == Keys.F9 || keyData == Keys.Enter)) { OpenItemsSelectionForm(); return true; }
                else if ((colName == "colAgriculturePermit" || colName == "colPaymentRelease") && keyData == Keys.F9) { OpenPermitSelectionForm(dgvData.CurrentCell.RowIndex, colName); return true; }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private string ResolvePortGroupName(string portName)
        {
            return GetPortGroupInfo(portName).GroupName;
        }

        private bool ItemMatchesCurrentPort(ItemsDataForm.ItemsRecord item)
        {
            string pg = item.PortGroup ?? "";
            if (string.IsNullOrEmpty(pg)) return false;
            if (!string.IsNullOrEmpty(currentPortGroupName) && pg == currentPortGroupName) return true;
            if (!string.IsNullOrEmpty(currentPortClassification) && pg == currentPortClassification) return true;
            if (currentPortClassification == "داخلي") return pg.Contains("داخلي") || pg.Contains("شمال");
            if (currentPortClassification == "خارجي") return pg.Contains("خارجي") || pg.Contains("جنوب");
            if (!string.IsNullOrEmpty(currentPortGroupName)) return pg.Contains(currentPortGroupName) || currentPortGroupName.Contains(pg);
            return false;
        }

        private (string agriGroup, string payGroup) GetItemPortGroupLinks(string productName)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(@"SELECT AgriLinkGroup, PayLinkGroup, AgriculturePermit, PaymentRelease FROM Items WHERE ItemName = @Name LIMIT 1", DatabaseHelper.CreateParameter("@Name", productName));
                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    string agri = dt.Columns.Contains("AgriLinkGroup") && row["AgriLinkGroup"] != DBNull.Value ? row["AgriLinkGroup"].ToString().Trim() : (dt.Columns.Contains("AgriculturePermit") && row["AgriculturePermit"] != DBNull.Value ? row["AgriculturePermit"].ToString().Trim() : "");
                    string pay = dt.Columns.Contains("PayLinkGroup") && row["PayLinkGroup"] != DBNull.Value ? row["PayLinkGroup"].ToString().Trim() : (dt.Columns.Contains("PaymentRelease") && row["PaymentRelease"] != DBNull.Value ? row["PaymentRelease"].ToString().Trim() : "");

                    if (agri == "0" || agri == "False" || agri == "1" || agri == "True") agri = "";
                    if (pay == "0" || pay == "False" || pay == "1" || pay == "True") pay = "";
                    return (agri, pay);
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ في جلب بيانات الربط: {ex.Message}"); }
            return ("", "");
        }

        private bool ValidatePermitNumber(string columnName, string permitNo, string portName)
        {
            try
            {
                string mainTable = columnName == "colAgriculturePermit" ? "AgriculturalPermits" : "PaymentsReleases";
                string itemsTable = columnName == "colAgriculturePermit" ? "AgriculturalPermitItems" : "PaymentReleaseItems";
                string query = $@"SELECT COUNT(*) FROM {mainTable} p WHERE p.PermitNo = @No AND (p.ExpiryDate IS NULL OR p.ExpiryDate = '' OR date(p.ExpiryDate) >= date('now')) AND EXISTS (SELECT 1 FROM {itemsTable} i WHERE i.PermitNo = p.PermitNo AND i.AvailableQuantity > 0)";
                var parms = new List<SQLiteParameter> { DatabaseHelper.CreateParameter("@No", permitNo) };
                if (!string.IsNullOrEmpty(portName)) { query += " AND p.PortName = @Port"; parms.Add(DatabaseHelper.CreateParameter("@Port", portName)); }

                var cnt = DatabaseHelper.ExecuteScalar(query, parms.ToArray());
                return cnt != null && cnt != DBNull.Value && Convert.ToInt32(cnt) > 0;
            }
            catch (Exception ex) { Console.WriteLine($"خطأ في التحقق من رقم التصريح: {ex.Message}"); return false; }
        }

        private void DgvData_KeyDown(object sender, KeyEventArgs ev)
        {
            if (ev.KeyCode != Keys.Enter) return;
            string colName = dgvData.CurrentCell?.OwningColumn?.Name ?? "";
            int rowIndex = dgvData.CurrentCell.RowIndex;

            if (colName == "colAgriculturePermit" || colName == "colPaymentRelease")
            {
                string productName = dgvData.Rows[rowIndex].Cells["colProductName"].Value?.ToString();
                if (!string.IsNullOrEmpty(productName))
                {
                    var (agriGroup, payGroup) = GetItemPortGroupLinks(productName);
                    string itemGroup = colName == "colAgriculturePermit" ? agriGroup : payGroup;
                    string permitTypeName = colName == "colAgriculturePermit" ? "التصريح الزراعي" : "إفراج المدفوعات";

                    if (!string.IsNullOrEmpty(itemGroup) && itemGroup != "غير مربوط")
                    {
                        string cellValue = dgvData.Rows[rowIndex].Cells[colName].Value?.ToString()?.Trim() ?? "";
                        string portName = cmbPortName.SelectedItem?.ToString() ?? "";
                        if (portName == "-- اختر منفذ --") portName = "";

                        if (string.IsNullOrEmpty(cellValue))
                        {
                            ev.SuppressKeyPress = true;
                            MessageBox.Show($"⚠️ رقم {permitTypeName} مطلوب:\n\nالصنف '{productName}' مربوط بمجموعة المنافذ '{itemGroup}' في شاشة تهيئة الأصناف.\n\n💡 أدخل الرقم يدوياً أو اضغط F9 للاختيار من القائمة.", $"{permitTypeName} مطلوب", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }

                        if (!ValidatePermitNumber(colName, cellValue, portName))
                        {
                            ev.SuppressKeyPress = true;
                            MessageBox.Show($"⚠️ تأكد من صحة رقم التصريح أو رقم الإفراج.\n\nالرقم '{cellValue}' غير صالح للاستخدام لأنه:\n• غير مسجل للمنفذ '{(string.IsNullOrEmpty(portName) ? "—" : portName)}'، أو\n• منتهي الصلاحية، أو\n• استُهلكت كميته بالكامل.\n\n💡 اضغط F9 لاختيار تصريح متاح من القائمة.", "رقم غير صحيح", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }
            }
            ev.SuppressKeyPress = true;
            _enterNavigation = true;
            SendKeys.Send("{TAB}");
        }

        private void DgvData_CellEnter(object sender, DataGridViewCellEventArgs ev)
        {
            if (!_enterNavigation) return;
            _enterNavigation = false;
            string colName = dgvData.Columns[ev.ColumnIndex].Name;
            if (colName != "colAgriculturePermit" && colName != "colPaymentRelease") return;

            string productName = dgvData.Rows[ev.RowIndex].Cells["colProductName"].Value?.ToString();
            if (string.IsNullOrEmpty(productName)) return;

            var (agriGroup, payGroup) = GetItemPortGroupLinks(productName);
            string itemGroup = colName == "colAgriculturePermit" ? agriGroup : payGroup;
            if (string.IsNullOrEmpty(itemGroup) || itemGroup == "غير مربوط") return;

            string permitTypeName = colName == "colAgriculturePermit" ? "التصريح الزراعي" : "إفراج المدفوعات";
            MessageBox.Show($"ℹ️ رقم {permitTypeName} مطلوب:\n\nالصنف '{productName}' مربوط بمجموعة المنافذ '{itemGroup}' في شاشة تهيئة الأصناف.\n\n💡 أدخل الرقم أو اضغط F9 للاختيار من القائمة.", $"{permitTypeName} مطلوب", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private bool ValidatePermitColumnsBeforeSave(out string errorMessage)
        {
            errorMessage = "";
            if (dgvData == null) return true;

            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.IsNewRow) continue;
                string productName = row.Cells["colProductName"].Value?.ToString();
                if (string.IsNullOrEmpty(productName)) continue;

                var (agriGroup, payGroup) = GetItemPortGroupLinks(productName);
                string[] cols = { "colAgriculturePermit", "colPaymentRelease" };
                string[] groups = { agriGroup, payGroup };
                string[] names = { "التصريح الزراعي", "إفراج المدفوعات" };

                for (int k = 0; k < 2; k++)
                {
                    if (string.IsNullOrEmpty(groups[k]) || groups[k] == "غير مربوط") continue;
                    string val = row.Cells[cols[k]].Value?.ToString()?.Trim() ?? "";

                    if (string.IsNullOrEmpty(val))
                    {
                        errorMessage = $"⚠️ لا يمكن الحفظ:\nالصنف '{productName}' مربوط في شاشة تهيئة الأصناف.\nيجب إدخال رقم {names[k]}.";
                        dgvData.CurrentCell = row.Cells[cols[k]];
                        return false;
                    }

                    string portName = cmbPortName.SelectedItem?.ToString() ?? "";
                    if (portName == "-- اختر منفذ --") portName = "";

                    if (!ValidatePermitNumber(cols[k], val, portName))
                    {
                        errorMessage = $"⚠️ لا يمكن الحفظ:\nرقم {names[k]} '{val}' للصنف '{productName}' غير صالح.\n(غير مسجل للمنفذ / منتهي الصلاحية / الكمية مستهلكة بالكامل).";
                        dgvData.CurrentCell = row.Cells[cols[k]];
                        return false;
                    }
                }
            }
            return true;
        }

        private void OpenItemsSelectionForm()
        {
            try
            {
                if (string.IsNullOrEmpty(currentPortClassification)) { MessageBox.Show("⚠️ الرجاء اختيار المنفذ أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); cmbPortName.Focus(); return; }

                var filteredItems = allItemsData.Where(ItemMatchesCurrentPort).Select(item => new { item.ProductName, item.Unit, item.WeightUnit, item.Weight, item.TotalCustoms, item.TotalST, item.TotalVAT, item.TotalPT, item.TotalVF, item.TotalTEF, item.TotalMFF }).Distinct().ToList();
                string regionName = !string.IsNullOrEmpty(currentPortGroupName) ? currentPortGroupName : (currentPortClassification == "داخلي" ? "المنطقة الداخلية/الشمالية" : "المنطقة الخارجية/الجنوبية");

                if (filteredItems.Count == 0) { MessageBox.Show($"لا توجد أصناف مسجلة لـ {regionName}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

                Form selectionForm = new Form();
                selectionForm.Text = $"📦 اختيار الصنف - {regionName}";
                selectionForm.Size = new Size(900, 600);
                selectionForm.StartPosition = FormStartPosition.CenterParent;
                selectionForm.RightToLeft = RightToLeft.Yes;
                selectionForm.RightToLeftLayout = true;
                selectionForm.WindowState = FormWindowState.Maximized;

                Panel pnlSearch = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(10) };
                Label lblSearch = new Label { Text = "🔍 بحث:", Font = new Font("Tahoma", 11, FontStyle.Bold), Location = new Point(10, 15), Size = new Size(80, 30), TextAlign = ContentAlignment.MiddleRight };
                TextBox txtSearch = new TextBox { Font = new Font("Tahoma", 11), Location = new Point(100, 15), Size = new Size(400, 30) };
                pnlSearch.Controls.AddRange(new Control[] { lblSearch, txtSearch });

                DataGridView dgvItems = new DataGridView();
                dgvItems.Dock = DockStyle.Fill;
                dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dgvItems.MultiSelect = false;
                dgvItems.RightToLeft = RightToLeft.Yes;
                dgvItems.ReadOnly = true;
                dgvItems.AllowUserToAddRows = false;
                dgvItems.EnableHeadersVisualStyles = false;
                dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvItems.ColumnHeadersHeight = 40;
                dgvItems.Columns.Add("colProductName", "اسم السلعة");
                dgvItems.Columns.Add("colUnit", "وحدة الصنف");
                dgvItems.Columns.Add("colWeightUnit", "وحدة الوزن");

                void RefreshItems(string search = "")
                {
                    dgvItems.Rows.Clear();
                    var filtered = filteredItems.AsEnumerable();
                    if (!string.IsNullOrWhiteSpace(search)) filtered = filtered.Where(i => i.ProductName != null && i.ProductName.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
                    foreach (var item in filtered) dgvItems.Rows.Add(item.ProductName, item.Unit ?? "", item.WeightUnit ?? "");
                }

                txtSearch.TextChanged += (s, ev) => RefreshItems(txtSearch.Text);
                RefreshItems();
                dgvItems.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter && dgvItems.CurrentRow != null) { ev.SuppressKeyPress = true; selectionForm.DialogResult = DialogResult.OK; selectionForm.Close(); } };

                Panel pnlButtons = new Panel { Height = 60, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(240, 248, 255) };
                Button btnOK = new Button { Text = "✅ اختيار", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 40), Location = new Point(200, 10), DialogResult = DialogResult.OK };
                Button btnCancel = new Button { Text = "❌ إلغاء", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 40), Location = new Point(30, 10), DialogResult = DialogResult.Cancel };
                pnlButtons.Controls.Add(btnOK);
                pnlButtons.Controls.Add(btnCancel);
                selectionForm.Controls.Add(dgvItems);
                selectionForm.Controls.Add(pnlButtons);
                selectionForm.Controls.Add(pnlSearch);
                selectionForm.AcceptButton = btnOK;
                selectionForm.CancelButton = btnCancel;

                if (selectionForm.ShowDialog() == DialogResult.OK && dgvItems.SelectedRows.Count > 0)
                {
                    string selectedProductName = dgvItems.SelectedRows[0].Cells["colProductName"].Value?.ToString();
                    var selectedItem = filteredItems.FirstOrDefault(i => i.ProductName == selectedProductName);
                    if (selectedItem != null)
                    {
                        int rowIndex = dgvData.Rows.Add();
                        DataGridViewRow newRow = dgvData.Rows[rowIndex];
                        newRow.Cells["colId"].Value = rowIndex + 1;
                        newRow.Cells["colProductName"].Value = selectedItem.ProductName;
                        newRow.Cells["colUnit"].Value = selectedItem.Unit ?? "";
                        newRow.Cells["colWeightUnit"].Value = selectedItem.WeightUnit ?? "";
                        newRow.Cells["colQuantity"].Value = 0;
                        newRow.Cells["colUnitWeight"].Value = selectedItem.Weight;
                        newRow.Cells["colTotalWeight"].Value = 0;
                        newRow.Cells["colCustomsFeeUnit"].Value = selectedItem.TotalCustoms;
                        newRow.Cells["colSTUnit"].Value = selectedItem.TotalST;
                        newRow.Cells["colVATUnit"].Value = selectedItem.TotalVAT;
                        newRow.Cells["colPTUnit"].Value = selectedItem.TotalPT;
                        newRow.Cells["colVFUnit"].Value = selectedItem.TotalVF;
                        newRow.Cells["colTEFUnit"].Value = selectedItem.TotalTEF;
                        newRow.Cells["colMFFUnit"].Value = selectedItem.TotalMFF;
                        newRow.Cells["colCustomsFeeTotal"].Value = 0;
                        newRow.Cells["colSTTotal"].Value = 0;
                        newRow.Cells["colVATTotal"].Value = 0;
                        newRow.Cells["colPTTotal"].Value = 0;
                        newRow.Cells["colVFTotal"].Value = 0;
                        newRow.Cells["colTEFTotal"].Value = 0;
                        newRow.Cells["colMFFTotal"].Value = 0;
                        newRow.Cells["colOther"].Value = 0;
                        newRow.Cells["colTotal"].Value = 0;
                        newRow.Cells["colAgriculturePermit"].Value = "";
                        newRow.Cells["colPaymentRelease"].Value = "";

                        // ✅ إعادة احتساب المبالغ الإضافية بعد إضافة صنف جديد
                        RefreshAutoAdditionalAmounts();

                        MessageBox.Show($"✅ تم إضافة الصنف: {selectedItem.ProductName}\nأدخل الكمية لحساب المبالغ والوزن تلقائياً", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnShow_Click(object sender, EventArgs e)
        {
            Form showForm = new Form();
            showForm.Text = "📋 قائمة طلبات الترسيم الخاصة بي";
            showForm.Size = new Size(1200, 700);
            showForm.StartPosition = FormStartPosition.CenterParent;
            showForm.RightToLeft = RightToLeft.Yes;
            showForm.RightToLeftLayout = true;
            showForm.WindowState = FormWindowState.Maximized;

            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 45;
            dgv.RowTemplate.Height = 35;

            dgv.Columns.Add("colId", "م");
            dgv.Columns.Add("colStatementNo", "رقم البيان");
            dgv.Columns.Add("colPortName", "المنفذ");
            dgv.Columns.Add("colTariffType", "نوع الترسيم");
            dgv.Columns.Add("colStatus", "الحالة");
            dgv.Columns.Add("colCreatedBy", "المخلص");
            dgv.Columns.Add("colCreatedDate", "تاريخ الإنشاء");
            dgv.Columns.Add("colReviewedBy", "المراجع");
            dgv.Columns.Add("colReviewedDate", "تاريخ المراجعة");

            dgv.Columns["colStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.Columns["colStatus"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            try
            {
                string currentUser = UserManager.GetCurrentUser()?.FullName ?? "";
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM TariffRequests WHERE CreatedBy = @User ORDER BY CreatedDate DESC", DatabaseHelper.CreateParameter("@User", currentUser));
                int idx = 1;
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        string status = row["Status"]?.ToString() ?? "pending";
                        string statusText = "";
                        Color statusColor = Color.Black;
                        if (status == "approved") { statusText = "✅ معتمد"; statusColor = Color.FromArgb(76, 175, 80); }
                        else if (status == "rejected") { statusText = "❌ مرفوض"; statusColor = Color.FromArgb(244, 67, 54); }
                        else { statusText = "⏳ قيد المراجعة"; statusColor = Color.FromArgb(255, 152, 0); }

                        int rowIdx = dgv.Rows.Add(idx++, row["StatementNo"]?.ToString(), row["PortName"]?.ToString(), row["TariffType"]?.ToString() ?? "-", statusText, row["CreatedBy"]?.ToString(), row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-dd HH:mm") : "", row["ReviewedBy"] != DBNull.Value ? row["ReviewedBy"].ToString() : "-", row["ReviewedDate"] != DBNull.Value ? Convert.ToDateTime(row["ReviewedDate"]).ToString("yyyy-MM-dd HH:mm") : "-");
                        dgv.Rows[rowIdx].Cells["colStatus"].Style.ForeColor = statusColor;
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }

            Panel pnlBottom = new Panel { Height = 70, Dock = DockStyle.Bottom, BackColor = Color.FromArgb(240, 248, 255), Padding = new Padding(10) };
            Button btnLoad = new Button { Text = "📥 تحميل البيان المحدد", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(220, 45), Location = new Point(250, 12) };
            btnLoad.Click += (s, ev) => { if (dgv.SelectedRows.Count > 0) { LoadRequestToForm(dgv.SelectedRows[0].Cells["colStatementNo"].Value?.ToString()); showForm.Close(); } else MessageBox.Show("⚠️ الرجاء اختيار طلب أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); };
            Button btnClose = new Button { Text = "❌ إغلاق", Font = new Font("Tahoma", 11, FontStyle.Bold), BackColor = Color.FromArgb(158, 158, 158), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Size = new Size(150, 45), Location = new Point(50, 12) };
            btnClose.Click += (s, ev) => showForm.Close();
            pnlBottom.Controls.Add(btnLoad);
            pnlBottom.Controls.Add(btnClose);
            showForm.Controls.Add(dgv);
            showForm.Controls.Add(pnlBottom);
            showForm.ShowDialog();
        }

        private void LoadRequestToForm(string statementNo)
        {
            var request = requests.FirstOrDefault(r => r.StatementNo == statementNo);

            if (request == null)
            {
                try
                {
                    DataTable dt = DatabaseHelper.ExecuteQuery(
                        "SELECT * FROM TariffRequests WHERE StatementNo = @S LIMIT 1",
                        DatabaseHelper.CreateParameter("@S", statementNo));

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        DataRow row = dt.Rows[0];
                        request = new TariffRequestRecord
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            StatementNo = row["StatementNo"]?.ToString(),
                            PortName = row["PortName"]?.ToString(),
                            TariffType = row["TariffType"]?.ToString(),
                            StatementDate = row["StatementDate"] != DBNull.Value ? Convert.ToDateTime(row["StatementDate"]) : DateTime.Now,
                            DriversJson = row["DriversJson"]?.ToString(),
                            AdditionalAmountsJson = row["AdditionalAmountsJson"]?.ToString(),
                            ItemsJson = row["ItemsJson"]?.ToString(),
                            Status = row["Status"]?.ToString() ?? "pending",
                            CreatedBy = row["CreatedBy"]?.ToString(),
                            CreatedDate = row["CreatedDate"]?.ToString()
                        };
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في جلب البيانات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            if (request == null) return;

            var existingRequest = requests.FirstOrDefault(r => r.StatementNo == statementNo);
            if (existingRequest != null)
            {
                requests.Remove(existingRequest);
            }
            requests.Add(request);

            cmbPortName.SelectedItem = request.PortName;
            cmbTariffType.Text = request.TariffType ?? "";
            txtStatementNo.Text = request.StatementNo;
            dtpStatementDate.Value = request.StatementDate;

            dgvData.Rows.Clear();
            if (!string.IsNullOrEmpty(request.ItemsJson))
            {
                try
                {
                    var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(request.ItemsJson);
                    int idx = 1;
                    foreach (var item in items)
                    {
                        int rowIndex = dgvData.Rows.Add();
                        var row = dgvData.Rows[rowIndex];

                        row.Cells["colId"].Value = idx++;
                        row.Cells["colProductName"].Value = GetItemValue(item, "ProductName");
                        row.Cells["colUnit"].Value = GetItemValue(item, "Unit");
                        row.Cells["colWeightUnit"].Value = GetItemValue(item, "WeightUnit");
                        row.Cells["colQuantity"].Value = GetItemDecimal(item, "Quantity");

                        string pName = GetItemValue(item, "ProductName");
                        var itemData = allItemsData.FirstOrDefault(i => i.ProductName == pName);
                        decimal quantity = GetItemDecimal(item, "Quantity");
                        decimal uWeight = itemData?.Weight ?? 0;
                        decimal totalWeight = item.ContainsKey("TotalWeight") ? GetItemDecimal(item, "TotalWeight") : (uWeight * quantity);

                        row.Cells["colUnitWeight"].Value = uWeight;
                        row.Cells["colTotalWeight"].Value = totalWeight;

                        row.Cells["colCustomsFeeUnit"].Value = itemData?.TotalCustoms ?? 0;
                        row.Cells["colSTUnit"].Value = itemData?.TotalST ?? 0;
                        row.Cells["colVATUnit"].Value = itemData?.TotalVAT ?? 0;
                        row.Cells["colPTUnit"].Value = itemData?.TotalPT ?? 0;
                        row.Cells["colVFUnit"].Value = itemData?.TotalVF ?? 0;
                        row.Cells["colTEFUnit"].Value = itemData?.TotalTEF ?? 0;
                        row.Cells["colMFFUnit"].Value = itemData?.TotalMFF ?? 0;

                        row.Cells["colCustomsFeeTotal"].Value = GetItemDecimal(item, "CustomsFeeTotal");
                        row.Cells["colSTTotal"].Value = GetItemDecimal(item, "STTotal");
                        row.Cells["colVATTotal"].Value = GetItemDecimal(item, "VATTotal");
                        row.Cells["colPTTotal"].Value = GetItemDecimal(item, "PTTotal");
                        row.Cells["colVFTotal"].Value = GetItemDecimal(item, "VFTotal");
                        row.Cells["colTEFTotal"].Value = GetItemDecimal(item, "TEFTotal");
                        row.Cells["colMFFTotal"].Value = GetItemDecimal(item, "MFFTotal");
                        row.Cells["colOther"].Value = GetItemDecimal(item, "Other");
                        row.Cells["colTotal"].Value = GetItemDecimal(item, "Total");

                        row.Cells["colAgriculturePermit"].Value = GetItemValue(item, "AgriculturePermit");
                        row.Cells["colPaymentRelease"].Value = GetItemValue(item, "PaymentRelease");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطأ في تحميل الأصناف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            dgvDrivers.Rows.Clear();
            if (!string.IsNullOrEmpty(request.DriversJson))
            {
                try
                {
                    var drivers = JsonSerializer.Deserialize<List<DriverInfo>>(request.DriversJson);
                    numVehicleCount.Value = drivers.Count;
                    for (int i = 0; i < drivers.Count; i++)
                    {
                        if (i < dgvDrivers.Rows.Count)
                        {
                            dgvDrivers.Rows[i].Cells["colDriverId"].Value = drivers[i].DriverId ?? "";
                            dgvDrivers.Rows[i].Cells["colDriverName"].Value = drivers[i].DriverName ?? "";
                            dgvDrivers.Rows[i].Cells["colVehicleNo"].Value = drivers[i].VehicleNo ?? "";
                        }
                    }
                }
                catch { }
            }

            dgvAdditionalAmounts.Rows.Clear();
            if (!string.IsNullOrEmpty(request.AdditionalAmountsJson))
            {
                try
                {
                    var amounts = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(request.AdditionalAmountsJson);
                    foreach (var amt in amounts)
                    {
                        string itemName = amt.ContainsKey("ItemName") ? amt["ItemName"]?.ToString() : "";
                        decimal amount = amt.ContainsKey("Amount") ? Convert.ToDecimal(amt["Amount"]) : 0;
                        dgvAdditionalAmounts.Rows.Add(itemName, amount);
                    }
                }
                catch { }
            }

            // ✅ إعادة احتساب المبالغ الإضافية التلقائية بعد تحميل البيان
            RefreshAutoAdditionalAmounts();

            UpdateTotalFees();
            UpdateTotalAmount();
            SetControlsState(request.Status != "approved");
            MessageBox.Show($"✅ تم تحميل البيان رقم: {statementNo}", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);

            UpdateApprovalStatus();
            CheckApprovalPermissions(request.PortName);
        }

        private string GetItemValue(Dictionary<string, object> item, string key)
        {
            if (item.ContainsKey(key) && item[key] != null)
                return item[key].ToString();
            return "";
        }

        private decimal GetItemDecimal(Dictionary<string, object> item, string key)
        {
            if (item.ContainsKey(key) && item[key] != null)
            {
                if (item[key] is decimal d) return d;
                if (item[key] is int i) return i;
                if (item[key] is double db) return (decimal)db;
                if (decimal.TryParse(item[key].ToString(), out decimal result)) return result;
            }
            return 0;
        }

        private void InitializeGrid()
        {
            if (dgvData == null) return;
            dgvData.Columns.Clear();

            dgvData.Columns.Add("colId", "م");
            dgvData.Columns.Add("colProductName", "اسم السلعة");
            dgvData.Columns.Add("colUnit", "وحدة الصنف");
            dgvData.Columns.Add("colWeightUnit", "وحدة الوزن");
            dgvData.Columns.Add("colQuantity", "الكمية");
            dgvData.Columns.Add("colTotalWeight", "إجمالي الوزن");
            dgvData.Columns.Add("colUnitWeight", "وزن الوحدة");
            dgvData.Columns["colUnitWeight"].Visible = false;

            dgvData.Columns.Add("colAgriculturePermit", "تصريح الزراعة");
            dgvData.Columns.Add("colPaymentRelease", "إفراج المدفوعات");
            dgvData.Columns.Add("colCustomsFeeTotal", "الرسوم الجمركية الإجمالية");
            dgvData.Columns.Add("colSTTotal", "ض/المبيعات الإجمالي");
            dgvData.Columns.Add("colVATTotal", "ض/القيمة المضافة الإجمالي");
            dgvData.Columns.Add("colPTTotal", "ض/الأرباح الإجمالي");
            dgvData.Columns.Add("colVFTotal", "التحسين الإجمالي");
            dgvData.Columns.Add("colTEFTotal", "المعلم الإجمالي");
            dgvData.Columns.Add("colMFFTotal", "الشهيد الإجمالي");
            dgvData.Columns.Add("colOther", "أخرى");
            dgvData.Columns.Add("colTotal", "الإجمالي");

            dgvData.Columns.Add("colCustomsFeeUnit", "قيمة الوحدة - رسوم");
            dgvData.Columns.Add("colSTUnit", "قيمة الوحدة - ST");
            dgvData.Columns.Add("colVATUnit", "قيمة الوحدة - VAT");
            dgvData.Columns.Add("colPTUnit", "قيمة الوحدة - PT");
            dgvData.Columns.Add("colVFUnit", "قيمة الوحدة - VF");
            dgvData.Columns.Add("colTEFUnit", "قيمة الوحدة - TEF");
            dgvData.Columns.Add("colMFFUnit", "قيمة الوحدة - MFF");

            for (int i = dgvData.Columns.Count - 7; i < dgvData.Columns.Count; i++) dgvData.Columns[i].Visible = false;

            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                if (col.Name.Contains("Quantity") || col.Name.Contains("Fee") || col.Name.Contains("Total") || col.Name == "colOther" || col.Name == "colTotalWeight" || col.Name == "colAgriculturePermit" || col.Name == "colPaymentRelease")
                {
                    col.DefaultCellStyle.Format = "N2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            if (dgvData.Columns["colTotalWeight"] != null)
            {
                dgvData.Columns["colTotalWeight"].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);
                dgvData.Columns["colTotalWeight"].DefaultCellStyle.ForeColor = Color.DarkGreen;
                dgvData.Columns["colTotalWeight"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            }

            dgvData.AllowUserToAddRows = true;
            dgvData.AllowUserToDeleteRows = true;
            dgvData.RightToLeft = RightToLeft.Yes;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.Fixed3D;
            dgvData.RowHeadersVisible = false;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.ColumnHeadersHeight = 50;
            dgvData.RowTemplate.Height = 35;

            dgvData.CellValueChanged += DgvData_CellValueChanged;
            dgvData.KeyDown += DgvData_KeyDown;
            dgvData.CellEnter += DgvData_CellEnter;

            if (dgvAdditionalAmounts != null && dgvAdditionalAmounts.Columns.Count == 0)
            {
                dgvAdditionalAmounts.Columns.Add("colItemName", "البند");
                dgvAdditionalAmounts.Columns.Add("colAmount", "المبلغ");
                dgvAdditionalAmounts.Columns["colItemName"].Width = 1000;
                dgvAdditionalAmounts.Columns["colAmount"].Width = 1000;
                dgvAdditionalAmounts.Columns["colAmount"].DefaultCellStyle.Format = "N2";
                dgvAdditionalAmounts.CellValueChanged += DgvAdditionalAmounts_CellValueChanged;
            }
        }

        private void CalculateAdditionalAmounts()
        {
            decimal totalAdditional = 0;
            if (dgvAdditionalAmounts != null)
            {
                foreach (DataGridViewRow row in dgvAdditionalAmounts.Rows)
                {
                    if (row.IsNewRow) continue;
                    decimal amount = ParseDecimal(row.Cells["colAmount"].Value?.ToString());
                    totalAdditional += amount;
                }
            }
            txtOtherFees.Text = totalAdditional.ToString("N2");
            UpdateTotalAmount();
        }

        private void DgvAdditionalAmounts_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvAdditionalAmounts.Columns[e.ColumnIndex].Name == "colAmount") CalculateAdditionalAmounts();
        }

        private void LoadPortsFromDatabase()
        {
            try
            {
                allPorts.Clear();
                cmbPortName.Items.Clear();
                cmbPortName.Items.Add("-- اختر منفذ --");
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName, Classification FROM Ports ORDER BY PortName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allPorts.Add(new PortsForm.PortData { Name = row["PortName"].ToString(), Classification = row["Classification"].ToString() });
                        cmbPortName.Items.Add(row["PortName"].ToString());
                    }
                }
                if (cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = 0;
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل المنافذ: {ex.Message}"); }
        }

        private void LoadItemsDataFromDatabase()
        {
            try
            {
                allItemsData.Clear();
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM ItemsPricingData ORDER BY ProductName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allItemsData.Add(new ItemsDataForm.ItemsRecord
                        {
                            ProductName = row["ProductName"]?.ToString(),
                            PortGroup = row["PortClassification"]?.ToString(),
                            Unit = row["Unit"]?.ToString(),
                            WeightUnit = row["WeightUnit"]?.ToString(),
                            Weight = row["Weight"] != DBNull.Value ? Convert.ToDecimal(row["Weight"]) : 0,
                            TotalCustoms = row["TotalCustoms"] != DBNull.Value ? Convert.ToDecimal(row["TotalCustoms"]) : 0,
                            TotalST = row["TotalST"] != DBNull.Value ? Convert.ToDecimal(row["TotalST"]) : 0,
                            TotalVAT = row["TotalVAT"] != DBNull.Value ? Convert.ToDecimal(row["TotalVAT"]) : 0,
                            TotalPT = row["TotalPT"] != DBNull.Value ? Convert.ToDecimal(row["TotalPT"]) : 0,
                            TotalVF = row["TotalVF"] != DBNull.Value ? Convert.ToDecimal(row["TotalVF"]) : 0,
                            TotalTEF = row["TotalTEF"] != DBNull.Value ? Convert.ToDecimal(row["TotalTEF"]) : 0,
                            TotalMFF = row["TotalMFF"] != DBNull.Value ? Convert.ToDecimal(row["TotalMFF"]) : 0
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل الأصناف: {ex.Message}"); }
        }

        private void LoadRequestsFromDatabase()
        {
            requests.Clear();
            try
            {
                string currentUser = UserManager.GetCurrentUser()?.FullName ?? "";
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM TariffRequests WHERE CreatedBy = @User ORDER BY CreatedDate DESC", DatabaseHelper.CreateParameter("@User", currentUser));
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        requests.Add(new TariffRequestRecord
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            StatementNo = row["StatementNo"]?.ToString(),
                            PortName = row["PortName"]?.ToString(),
                            TariffType = row["TariffType"]?.ToString(),
                            ClientName = row["ClientName"]?.ToString(),
                            CarNo = row["CarNo"]?.ToString(),
                            StatementDate = row["StatementDate"] != DBNull.Value ? Convert.ToDateTime(row["StatementDate"]) : DateTime.Now,
                            DriversJson = row["DriversJson"]?.ToString(),
                            AdditionalAmountsJson = row["AdditionalAmountsJson"]?.ToString(),
                            ItemsJson = row["ItemsJson"]?.ToString(),
                            Status = row["Status"]?.ToString() ?? "pending",
                            CreatedBy = row["CreatedBy"]?.ToString(),
                            CreatedDate = row["CreatedDate"]?.ToString(),
                            ReviewedBy = row["ReviewedBy"]?.ToString(),
                            ReviewNotes = row["ReviewNotes"]?.ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"خطأ في تحميل الطلبات: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ✅ نقطة الاستدعاء 1: عند تغيير المنفذ
        private void CmbPortName_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedPort = cmbPortName.SelectedItem?.ToString() ?? "";
            currentPortGroupName = "";
            if (selectedPort != "-- اختر منفذ --" && !string.IsNullOrEmpty(selectedPort))
            {
                var port = allPorts.FirstOrDefault(p => p.Name == selectedPort);
                if (port != null)
                {
                    string classification = port.Classification ?? "";
                    if (classification.Contains("داخلي") || classification.Contains("شمال")) currentPortClassification = "داخلي";
                    else if (classification.Contains("خارجي") || classification.Contains("جنوب")) currentPortClassification = "خارجي";
                    else currentPortClassification = classification;
                }
                currentPortGroupName = ResolvePortGroupName(selectedPort);
            }
            else currentPortClassification = "";

            // ✅ إعادة احتساب المبالغ الإضافية عند تغيير المنفذ
            RefreshAutoAdditionalAmounts();
        }

        // ✅ نقطة الاستدعاء 2: عند تغيير الكمية أو الصنف
        private void DgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dgvData.Rows[e.RowIndex];
                string columnName = dgvData.Columns[e.ColumnIndex].Name;

                if (columnName == "colQuantity")
                {
                    CalculateAllTotals(e.RowIndex);
                    CalculateTotalWeight(e.RowIndex);
                    // ✅ إعادة احتساب المبالغ الإضافية عند تغيير الكمية
                    RefreshAutoAdditionalAmounts();
                }
                else if (columnName == "colProductName")
                {
                    // ✅ إعادة احتساب عند تغيير الصنف
                    RefreshAutoAdditionalAmounts();
                }
                else if (columnName.Contains("Total") || columnName == "colOther" || columnName == "colAgriculturePermit" || columnName == "colPaymentRelease")
                {
                    CalculateTotal(e.RowIndex);
                }

                UpdateTotalFees();
                UpdateTotalAmount();
                hasChanges = true;
            }
        }

        private void CalculateAllTotals(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.IsNewRow) return;
            string productName = row.Cells["colProductName"].Value?.ToString();
            decimal quantity = ParseDecimal(row.Cells["colQuantity"].Value?.ToString());
            var itemData = allItemsData.FirstOrDefault(i => i.ProductName == productName && ItemMatchesCurrentPort(i));

            if (itemData != null && quantity > 0)
            {
                row.Cells["colCustomsFeeTotal"].Value = itemData.TotalCustoms * quantity;
                row.Cells["colSTTotal"].Value = itemData.TotalST * quantity;
                row.Cells["colVATTotal"].Value = itemData.TotalVAT * quantity;
                row.Cells["colPTTotal"].Value = itemData.TotalPT * quantity;
                row.Cells["colVFTotal"].Value = itemData.TotalVF * quantity;
                row.Cells["colTEFTotal"].Value = itemData.TotalTEF * quantity;
                row.Cells["colMFFTotal"].Value = itemData.TotalMFF * quantity;
                row.Cells["colCustomsFeeUnit"].Value = itemData.TotalCustoms;
                row.Cells["colSTUnit"].Value = itemData.TotalST;
                row.Cells["colVATUnit"].Value = itemData.TotalVAT;
                row.Cells["colPTUnit"].Value = itemData.TotalPT;
                row.Cells["colVFUnit"].Value = itemData.TotalVF;
                row.Cells["colTEFUnit"].Value = itemData.TotalTEF;
                row.Cells["colMFFUnit"].Value = itemData.TotalMFF;
            }
            CalculateTotal(rowIndex);
        }

        private void CalculateTotalWeight(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.IsNewRow) return;
            decimal quantity = ParseDecimal(row.Cells["colQuantity"].Value?.ToString());
            decimal unitWeight = ParseDecimal(row.Cells["colUnitWeight"].Value?.ToString());
            row.Cells["colTotalWeight"].Value = quantity * unitWeight;
        }

        private void CalculateTotal(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            decimal total = ParseDecimal(row.Cells["colCustomsFeeTotal"].Value?.ToString()) + ParseDecimal(row.Cells["colSTTotal"].Value?.ToString()) + ParseDecimal(row.Cells["colVATTotal"].Value?.ToString()) + ParseDecimal(row.Cells["colPTTotal"].Value?.ToString()) + ParseDecimal(row.Cells["colVFTotal"].Value?.ToString()) + ParseDecimal(row.Cells["colTEFTotal"].Value?.ToString()) + ParseDecimal(row.Cells["colMFFTotal"].Value?.ToString()) + ParseDecimal(row.Cells["colOther"].Value?.ToString()) + ParseDecimal(row.Cells["colAgriculturePermit"].Value?.ToString()) + ParseDecimal(row.Cells["colPaymentRelease"].Value?.ToString());
            row.Cells["colTotal"].Value = total;
        }

        private void UpdateTotalFees()
        {
            if (dgvData == null || dgvData.Rows.Count == 0) { lblTotalFeesValue.Text = "0.00"; return; }
            decimal total = 0;
            foreach (DataGridViewRow row in dgvData.Rows) { if (row.IsNewRow) continue; total += ParseDecimal(row.Cells["colTotal"].Value?.ToString()); }
            lblTotalFeesValue.Text = total.ToString("N2");
        }

        private void UpdateTotalAmount()
        {
            decimal totalFees = ParseDecimal(lblTotalFeesValue.Text);
            decimal otherFees = ParseDecimal(txtOtherFees?.Text ?? "0");
            decimal totalAmount = totalFees + otherFees;
            lblTotalAmountValue.Text = totalAmount.ToString("N2");
        }

        private void SetControlsState(bool enabled)
        {
            isEditingMode = enabled;
            cmbPortName.Enabled = enabled;
            cmbTariffType.Enabled = enabled;
            dtpStatementDate.Enabled = enabled;
            txtStatementNo.Enabled = enabled;
            txtOtherFees.Enabled = enabled;
            numVehicleCount.Enabled = enabled;
            if (dgvDrivers != null) dgvDrivers.ReadOnly = !enabled;
            if (dgvAdditionalAmounts != null) dgvAdditionalAmounts.ReadOnly = !enabled;
            if (dgvData != null) dgvData.ReadOnly = !enabled;
        }

        private void BtnNew_Click(object sender, EventArgs e)
        {
            if (hasChanges) { if (MessageBox.Show("⚠️ لديك تغييرات غير محفوظة. هل تريد تجاهلها والبدء بطلب جديد؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return; }
            ClearFields();
            SetControlsState(true);
            hasChanges = false;
            txtStatementNo.Text = GenerateStatementNo();
            txtStatementNo.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatementNo.Text)) { MessageBox.Show("⚠️ الرجاء اختيار طلب للتعديل", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SetControlsState(true);
            hasChanges = false;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatementNo.Text)) { MessageBox.Show("⚠️ رقم البيان مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtStatementNo.Focus(); return; }
            string portName = cmbPortName.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(portName) || portName == "-- اختر منفذ --") { MessageBox.Show("⚠️ الرجاء اختيار المنفذ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); cmbPortName.Focus(); return; }
            if (numVehicleCount.Value == 0) { MessageBox.Show("⚠️ الرجاء إدخال عدد سيارات النقل (يجب أن يكون أكبر من 0)", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); numVehicleCount.Focus(); return; }

            foreach (DataGridViewRow row in dgvDrivers.Rows)
            {
                if (row.IsNewRow) continue;
                string driverName = row.Cells["colDriverName"].Value?.ToString()?.Trim();
                string vehicleNo = row.Cells["colVehicleNo"].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(driverName)) { MessageBox.Show("⚠️ الرجاء إدخال اسم السائق في جميع الصفوف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); dgvDrivers.CurrentCell = row.Cells["colDriverName"]; dgvDrivers.BeginEdit(true); return; }
                if (string.IsNullOrEmpty(vehicleNo)) { MessageBox.Show("⚠️ الرجاء إدخال رقم السيارة في جميع الصفوف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); dgvDrivers.CurrentCell = row.Cells["colVehicleNo"]; dgvDrivers.BeginEdit(true); return; }
            }

            if (!ValidatePermitColumnsBeforeSave(out string permitError)) { MessageBox.Show(permitError, "بيانات ناقصة", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                string currentUser = UserManager.GetCurrentUser()?.FullName ?? "";
                string itemsJson = JsonSerializer.Serialize(GetItemsList());
                string driversJson = GetDriversJson();
                string additionalJson = GetAdditionalAmountsJson();

                var existing = requests.FirstOrDefault(r => r.StatementNo == txtStatementNo.Text.Trim() && r.PortName == portName);

                if (existing != null)
                {
                    DatabaseHelper.ExecuteNonQuery(@"UPDATE TariffRequests SET TariffType = @TariffType, ClientName = @ClientName, CarNo = @CarNo, StatementDate = @StatementDate, DriversJson = @DriversJson, AdditionalAmountsJson = @AdditionalAmountsJson, ItemsJson = @ItemsJson, Status = 'pending', ModifiedDate = datetime('now') WHERE Id = @Id",
                        DatabaseHelper.CreateParameter("@TariffType", cmbTariffType.Text.Trim()), DatabaseHelper.CreateParameter("@ClientName", ""), DatabaseHelper.CreateParameter("@CarNo", ""),
                        DatabaseHelper.CreateParameter("@StatementDate", dtpStatementDate.Value.ToString("yyyy-MM-dd")), DatabaseHelper.CreateParameter("@DriversJson", (object)driversJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@AdditionalAmountsJson", (object)additionalJson ?? DBNull.Value), DatabaseHelper.CreateParameter("@ItemsJson", itemsJson), DatabaseHelper.CreateParameter("@Id", existing.Id));
                    MessageBox.Show("✅ تم تحديث طلب الترسيم وإرساله للمراجعة", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO TariffRequests (StatementNo, PortName, TariffType, StatementDate, DriversJson, AdditionalAmountsJson, ItemsJson, Status, CreatedBy, CreatedDate) VALUES (@StatementNo, @PortName, @TariffType, @StatementDate, @DriversJson, @AdditionalAmountsJson, @ItemsJson, 'pending', @CreatedBy, datetime('now'))",
                        DatabaseHelper.CreateParameter("@StatementNo", txtStatementNo.Text.Trim()), DatabaseHelper.CreateParameter("@PortName", portName), DatabaseHelper.CreateParameter("@TariffType", cmbTariffType.Text.Trim()),
                        DatabaseHelper.CreateParameter("@StatementDate", dtpStatementDate.Value.ToString("yyyy-MM-dd")), DatabaseHelper.CreateParameter("@DriversJson", (object)driversJson ?? DBNull.Value),
                        DatabaseHelper.CreateParameter("@AdditionalAmountsJson", (object)additionalJson ?? DBNull.Value), DatabaseHelper.CreateParameter("@ItemsJson", itemsJson), DatabaseHelper.CreateParameter("@CreatedBy", currentUser));
                    MessageBox.Show("✅ تم إرسال طلب الترسيم للمراجعة\n⏳ في انتظار اعتماد الأدمن", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadRequestsFromDatabase();
                SetControlsState(false);
                hasChanges = false;
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ في الحفظ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStatementNo.Text)) { MessageBox.Show("⚠️ الرجاء اختيار طلب للحذف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            var existing = requests.FirstOrDefault(r => r.StatementNo == txtStatementNo.Text.Trim());
            if (existing == null) return;
            if (existing.Status == "approved") { MessageBox.Show("⚠️ لا يمكن حذف طلب معتمد", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            if (MessageBox.Show($"🗑️ هل أنت متأكد من حذف طلب الترسيم رقم {txtStatementNo.Text}؟", "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM TariffRequests WHERE Id = @Id", DatabaseHelper.CreateParameter("@Id", existing.Id));
                    LoadRequestsFromDatabase();
                    ClearFields();
                    MessageBox.Show("✅ تم الحذف", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"❌ خطأ في الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void BtnHome_Click(object sender, EventArgs e)
        {
            if (hasChanges && MessageBox.Show("⚠️ لديك تغييرات غير محفوظة. هل تريد الخروج؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No) return;
            this.Close();
        }

        private string GetItemSubGroupNo(string productName)
        {
            try { var result = DatabaseHelper.ExecuteScalar("SELECT SubGroupUnder FROM Items WHERE ItemName = @Name LIMIT 1", DatabaseHelper.CreateParameter("@Name", productName)); return result?.ToString() ?? ""; }
            catch { return ""; }
        }

        private List<Dictionary<string, object>> GetItemsList()
        {
            var items = new List<Dictionary<string, object>>();
            if (dgvData == null) return items;
            foreach (DataGridViewRow row in dgvData.Rows)
            {
                if (row.IsNewRow || row.Cells["colProductName"].Value == null) continue;
                string productName = row.Cells["colProductName"].Value?.ToString();
                items.Add(new Dictionary<string, object>
                {
                    { "ProductName", productName }, { "Unit", row.Cells["colUnit"].Value?.ToString() }, { "WeightUnit", row.Cells["colWeightUnit"].Value?.ToString() },
                    { "Quantity", ParseDecimal(row.Cells["colQuantity"].Value?.ToString()) }, { "TotalWeight", ParseDecimal(row.Cells["colTotalWeight"].Value?.ToString()) },
                    { "SubGroupNo", GetItemSubGroupNo(productName) }, { "AgriculturePermit", row.Cells["colAgriculturePermit"].Value?.ToString() ?? "" },
                    { "PaymentRelease", row.Cells["colPaymentRelease"].Value?.ToString() ?? "" }, { "CustomsFeeTotal", ParseDecimal(row.Cells["colCustomsFeeTotal"].Value?.ToString()) },
                    { "STTotal", ParseDecimal(row.Cells["colSTTotal"].Value?.ToString()) }, { "VATTotal", ParseDecimal(row.Cells["colVATTotal"].Value?.ToString()) },
                    { "PTTotal", ParseDecimal(row.Cells["colPTTotal"].Value?.ToString()) }, { "VFTotal", ParseDecimal(row.Cells["colVFTotal"].Value?.ToString()) },
                    { "TEFTotal", ParseDecimal(row.Cells["colTEFTotal"].Value?.ToString()) }, { "MFFTotal", ParseDecimal(row.Cells["colMFFTotal"].Value?.ToString()) },
                    { "Other", ParseDecimal(row.Cells["colOther"].Value?.ToString()) }, { "Total", ParseDecimal(row.Cells["colTotal"].Value?.ToString()) }
                });
            }
            return items;
        }

        private string GetDriversJson()
        {
            List<DriverInfo> drivers = new List<DriverInfo>();
            if (dgvDrivers != null)
            {
                foreach (DataGridViewRow row in dgvDrivers.Rows)
                {
                    if (row.IsNewRow) continue;
                    drivers.Add(new DriverInfo { DriverId = row.Cells["colDriverId"].Value?.ToString() ?? "", DriverName = row.Cells["colDriverName"].Value?.ToString() ?? "", VehicleNo = row.Cells["colVehicleNo"].Value?.ToString() ?? "" });
                }
            }
            return drivers.Count > 0 ? JsonSerializer.Serialize(drivers) : null;
        }

        private string GetAdditionalAmountsJson()
        {
            var amounts = new List<Dictionary<string, object>>();
            if (dgvAdditionalAmounts != null)
            {
                foreach (DataGridViewRow row in dgvAdditionalAmounts.Rows)
                {
                    if (row.IsNewRow) continue;
                    amounts.Add(new Dictionary<string, object> { { "ItemName", row.Cells["colItemName"].Value?.ToString() ?? "" }, { "Amount", ParseDecimal(row.Cells["colAmount"].Value?.ToString()) } });
                }
            }
            return amounts.Count > 0 ? JsonSerializer.Serialize(amounts) : null;
        }

        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            text = text.Replace(",", "").Replace(" ", "").Trim();
            if (decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal val)) return val;
            return 0;
        }

        private void ClearFields()
        {
            if (cmbPortName != null && cmbPortName.Items.Count > 0) cmbPortName.SelectedIndex = 0;
            if (cmbTariffType != null) cmbTariffType.Text = "";
            if (dtpStatementDate != null) dtpStatementDate.Value = DateTime.Now;
            if (txtStatementNo != null) txtStatementNo.Clear();
            if (numVehicleCount != null) numVehicleCount.Value = 0;
            if (dgvDrivers != null) dgvDrivers.Rows.Clear();
            if (dgvAdditionalAmounts != null) dgvAdditionalAmounts.Rows.Clear();
            if (dgvData != null) dgvData.Rows.Clear();
            if (txtOtherFees != null) txtOtherFees.Text = "0";
            currentPortClassification = "";
            currentPortGroupName = "";
            _enterNavigation = false;
            lblTotalFeesValue.Text = "0.00";
            lblTotalAmountValue.Text = "0.00";
            hasChanges = false;
            isEditingMode = false;
        }

        private void OpenPermitSelectionForm(int rowIndex, string columnName)
        {
            string productName = dgvData.Rows[rowIndex].Cells["colProductName"].Value?.ToString();
            if (string.IsNullOrEmpty(productName)) { MessageBox.Show("⚠️ الرجاء اختيار اسم السلعة أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            string portName = cmbPortName.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(portName) || portName == "-- اختر منفذ --") { MessageBox.Show("⚠️ الرجاء اختيار المنفذ أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string subGroupNo = "";
            try { var result = DatabaseHelper.ExecuteScalar("SELECT SubGroupUnder FROM Items WHERE ItemName = @Name LIMIT 1", DatabaseHelper.CreateParameter("@Name", productName)); if (result != null && result != DBNull.Value) subGroupNo = result.ToString(); }
            catch { }

            string itemsTable = columnName == "colAgriculturePermit" ? "AgriculturalPermitItems" : "PaymentReleaseItems";
            string mainTable = columnName == "colAgriculturePermit" ? "AgriculturalPermits" : "PaymentsReleases";
            string permitTypeName = columnName == "colAgriculturePermit" ? "التصاريح الزراعية" : "إفراج المدفوعات";

            string query;
            List<SQLiteParameter> parameters = new List<SQLiteParameter> { DatabaseHelper.CreateParameter("@PortName", portName) };
            if (!string.IsNullOrEmpty(subGroupNo))
            {
                query = $@"SELECT p.PermitNo, p.PermitDate, i.SubGroupName, i.Quantity, i.AvailableQuantity FROM {itemsTable} i INNER JOIN {mainTable} p ON i.PermitNo = p.PermitNo WHERE p.PortName = @PortName AND i.SubGroupNo = @SubGroupNo AND i.AvailableQuantity > 0 AND (p.ExpiryDate IS NULL OR p.ExpiryDate = '' OR date(p.ExpiryDate) >= date('now'))";
                parameters.Add(DatabaseHelper.CreateParameter("@SubGroupNo", subGroupNo));
            }
            else
            {
                query = $@"SELECT p.PermitNo, p.PermitDate, i.SubGroupName, i.Quantity, i.AvailableQuantity FROM {itemsTable} i INNER JOIN {mainTable} p ON i.PermitNo = p.PermitNo WHERE p.PortName = @PortName AND i.AvailableQuantity > 0 AND (p.ExpiryDate IS NULL OR p.ExpiryDate = '' OR date(p.ExpiryDate) >= date('now'))";
            }

            DataTable dt = DatabaseHelper.ExecuteQuery(query, parameters.ToArray());
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show($"⚠️ لا يوجد تصريح متاح:\n\nلا توجد {permitTypeName} صالحة للاستخدام للمنفذ '{portName}'.\n\n💡 الأسباب المحتملة:\n• انتهت صلاحية جميع التصاريح المسجلة\n• استُهلكت الكميات بالكامل (المتبقية = 0)\n• لا توجد تصاريح مسجلة لهذه المجموعة الفرعية\n\n📋 الرجاء إضافة تصريح جديد من شاشة {permitTypeName} أولاً.", "لا يوجد تصريح متاح", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form f = new Form { Text = $"🔍 اختر رقم {permitTypeName} (السارية والمتاحة فقط)", Size = new Size(900, 450), StartPosition = FormStartPosition.CenterParent, RightToLeft = RightToLeft.Yes, RightToLeftLayout = true, FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false })
            {
                DataGridView dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false, RowHeadersVisible = false, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
                dgv.EnableHeadersVisualStyles = false;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgv.ColumnHeadersHeight = 40;
                dgv.RowTemplate.Height = 35;
                dgv.Columns.Add("colPermitNo", "رقم التصريح/الإفراج");
                dgv.Columns.Add("colDate", "التاريخ");
                dgv.Columns.Add("colSubGroup", "المجموعة الفرعية");
                dgv.Columns.Add("colQty", "الكمية المصرح بها");
                dgv.Columns.Add("colAvailQty", "✅ الكمية المتبقية");

                foreach (DataRow r in dt.Rows)
                {
                    int idx = dgv.Rows.Add(r["PermitNo"].ToString(), r["PermitDate"].ToString(), r["SubGroupName"].ToString(), r["Quantity"].ToString(), r["AvailableQuantity"].ToString());
                    dgv.Rows[idx].Cells["colAvailQty"].Style.ForeColor = Color.Green;
                    dgv.Rows[idx].Cells["colAvailQty"].Style.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                }

                dgv.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) { dgvData.Rows[rowIndex].Cells[columnName].Value = dgv.Rows[ev.RowIndex].Cells["colPermitNo"].Value?.ToString(); f.Close(); } };
                dgv.KeyDown += (s, ev) => { if (ev.KeyCode == Keys.Enter && dgv.CurrentRow != null) { ev.SuppressKeyPress = true; dgvData.Rows[rowIndex].Cells[columnName].Value = dgv.CurrentRow.Cells["colPermitNo"].Value?.ToString(); f.Close(); } };
                f.Controls.Add(dgv);
                f.ShowDialog();
            }
        }

        public static bool ApplyPermitDeductions(string itemsJson, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(itemsJson);
                if (items == null || items.Count == 0) return true;

                foreach (var item in items)
                {
                    string agri = GetStr(item, "AgriculturePermit");
                    string pay = GetStr(item, "PaymentRelease");
                    string sub = GetStr(item, "SubGroupNo");
                    decimal weight = GetDec(item, "TotalWeight");
                    string name = GetStr(item, "ProductName");
                    if (weight <= 0 || string.IsNullOrEmpty(sub)) continue;

                    if (!string.IsNullOrEmpty(agri))
                    {
                        decimal avail = GetAvailableQty("AgriculturalPermitItems", agri, sub);
                        if (avail < 0) { errorMessage = $"التصريح الزراعي '{agri}' غير موجود في شاشة التصاريح."; return false; }
                        if (avail < weight) { errorMessage = $"الكمية المتبقية في التصريح الزراعي '{agri}' ({avail}) أقل من إجمالي الوزن للصنف '{name}' ({weight})."; return false; }
                    }
                    if (!string.IsNullOrEmpty(pay))
                    {
                        decimal avail = GetAvailableQty("PaymentReleaseItems", pay, sub);
                        if (avail < 0) { errorMessage = $"إفراج المدفوعات '{pay}' غير موجود في شاشة الإفراجات."; return false; }
                        if (avail < weight) { errorMessage = $"الكمية المتبقية في إفراج المدفوعات '{pay}' ({avail}) أقل من إجمالي الوزن للصنف '{name}' ({weight})."; return false; }
                    }
                }

                foreach (var item in items)
                {
                    string agri = GetStr(item, "AgriculturePermit");
                    string pay = GetStr(item, "PaymentRelease");
                    string sub = GetStr(item, "SubGroupNo");
                    decimal weight = GetDec(item, "TotalWeight");
                    if (weight <= 0 || string.IsNullOrEmpty(sub)) continue;

                    if (!string.IsNullOrEmpty(agri)) PermitBaseForm.DeductPermitQuantity("AgriculturalPermits", "AgriculturalPermitItems", agri, sub, weight);
                    if (!string.IsNullOrEmpty(pay)) PermitBaseForm.DeductPermitQuantity("PaymentsReleases", "PaymentReleaseItems", pay, sub, weight);
                }
                return true;
            }
            catch (Exception ex) { errorMessage = ex.Message; return false; }
        }

        public static void RestorePermitDeductions(string itemsJson)
        {
            try
            {
                var items = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(itemsJson);
                if (items == null) return;
                foreach (var item in items)
                {
                    string agri = GetStr(item, "AgriculturePermit");
                    string pay = GetStr(item, "PaymentRelease");
                    string sub = GetStr(item, "SubGroupNo");
                    decimal weight = GetDec(item, "TotalWeight");
                    if (weight <= 0 || string.IsNullOrEmpty(sub)) continue;
                    if (!string.IsNullOrEmpty(agri)) PermitBaseForm.RestorePermitQuantity("AgriculturalPermitItems", agri, sub, weight);
                    if (!string.IsNullOrEmpty(pay)) PermitBaseForm.RestorePermitQuantity("PaymentReleaseItems", pay, sub, weight);
                }
            }
            catch { }
        }

        private static string GetStr(Dictionary<string, object> d, string key) => d.ContainsKey(key) && d[key] != null ? d[key].ToString() : "";
        private static decimal GetDec(Dictionary<string, object> d, string key)
        {
            if (d.ContainsKey(key) && d[key] != null && decimal.TryParse(d[key].ToString(), out decimal v)) return v;
            return 0;
        }
        private static decimal GetAvailableQty(string itemsTable, string permitNo, string subGroupNo)
        {
            try
            {
                var r = DatabaseHelper.ExecuteScalar($"SELECT AvailableQuantity FROM {itemsTable} WHERE PermitNo = @P AND SubGroupNo = @S", DatabaseHelper.CreateParameter("@P", permitNo), DatabaseHelper.CreateParameter("@S", subGroupNo));
                if (r == null || r == DBNull.Value) return -1;
                return Convert.ToDecimal(r);
            }
            catch { return -1; }
        }

        // ================================================================
        // ✅ نظام المبالغ الإضافية التلقائية من AdditionalFees
        // ================================================================

        /// <summary>
        /// احتساب المبالغ الإضافية تلقائياً من جدول AdditionalFees
        /// </summary>
        private void RefreshAutoAdditionalAmounts()
        {
            if (dgvAdditionalAmounts == null) return;

            // 1) حذف الصفوف التلقائية القديمة
            for (int i = dgvAdditionalAmounts.Rows.Count - 1; i >= 0; i--)
            {
                var r = dgvAdditionalAmounts.Rows[i];
                if (!r.IsNewRow && r.Tag != null && r.Tag.ToString() == "AUTO_FEE")
                    dgvAdditionalAmounts.Rows.RemoveAt(i);
            }

            string portName = cmbPortName?.SelectedItem?.ToString() ?? "";
            if (string.IsNullOrEmpty(portName) || portName == "-- اختر منفذ --")
            {
                CalculateAdditionalAmounts();
                return;
            }

            DataTable feesTable;
            try { feesTable = DatabaseHelper.ExecuteQuery("SELECT * FROM AdditionalFees"); }
            catch { feesTable = null; }

            if (feesTable == null || feesTable.Rows.Count == 0)
            {
                CalculateAdditionalAmounts();
                return;
            }

            var pg = GetPortGroupInfo(portName);
            decimal vehicleCount = numVehicleCount != null ? numVehicleCount.Value : 0;

            var itemRows = new List<(string Name, decimal Qty, decimal TotalValue, string SubNo, string SubName)>();
            if (dgvData != null)
            {
                foreach (DataGridViewRow r in dgvData.Rows)
                {
                    if (r.IsNewRow) continue;
                    string pn = r.Cells["colProductName"].Value?.ToString();
                    if (string.IsNullOrEmpty(pn)) continue;
                    decimal q = ParseDecimal(r.Cells["colQuantity"].Value?.ToString());
                    decimal tot = ParseDecimal(r.Cells["colTotal"].Value?.ToString());
                    var sg = GetItemSubGroupInfo(pn);
                    itemRows.Add((pn, q, tot, sg.SubNo, sg.SubName));
                }
            }

            bool diagShown = false; // ✅ متغير لإظهار رسالة التشخيص مرة واحدة فقط

            foreach (DataRow fee in feesTable.Rows)
            {
                try
                {
                    string feeName = fee["FeeName"]?.ToString() ?? "";
                    string effect = fee["EffectType"]?.ToString() ?? "";
                    string scope = fee["FeeScope"]?.ToString() ?? "";
                    string valueType = fee["ValueType"]?.ToString() ?? "";
                    string calcType = fee["CalcType"]?.ToString() ?? "";
                    string feePortName = fee["PortName"]?.ToString() ?? "";
                    string feePortGroupId = (fee["PortGroupId"]?.ToString() ?? "").Trim();
                    string feePortGroupName = (fee["PortGroupName"]?.ToString() ?? "").Trim();
                    string feeItemId = fee["ItemGroupId"]?.ToString() ?? "";
                    string feeItemGroup = fee["ItemGroupName"]?.ToString() ?? "";
                    string feeItemName = fee["ItemName"]?.ToString() ?? "";
                    decimal minLimit = ParseDecimal(fee["MinLimit"]?.ToString());
                    decimal unitAmount = ParseDecimal(fee["Amount"]?.ToString());

                    // ========== مطابقة المنفذ / مجموعة المنافذ ==========
                    // ========== مطابقة المنفذ / مجموعة المنافذ (الاثنان معاً) ==========
                    string curPort = portName.Trim();
                    string gNo = (pg.GroupNo ?? "").Trim();
                    string gName = (pg.GroupName ?? "").Trim();
                    string feePortId = (fee["PortId"]?.ToString() ?? "").Trim();

                    // 1) مطابقة المنفذ (اسم مطابق، أو أحدهما يحتوي الآخر، أو رقم المنفذ)
                    bool byPort = false;
                    string fpn = feePortName.Trim();
                    if (!string.IsNullOrEmpty(fpn))
                    {
                        byPort = fpn == curPort
                              || curPort.Contains(fpn)
                              || fpn.Contains(curPort);
                    }
                    if (!byPort && !string.IsNullOrEmpty(feePortId))
                    {
                        var pid = DatabaseHelper.ExecuteScalar(
                            "SELECT PortId FROM Ports WHERE PortName = @P LIMIT 1",
                            DatabaseHelper.CreateParameter("@P", portName));
                        byPort = pid != null && pid.ToString().Trim() == feePortId;
                    }

                    // 2) مطابقة مجموعة المنافذ (بالاسم أو بالرقم)
                    bool byGroup =
                        (!string.IsNullOrEmpty(feePortGroupName) &&
                            (feePortGroupName == gName || feePortGroupName == gNo)) ||
                        (!string.IsNullOrEmpty(feePortGroupId) &&
                            (feePortGroupId == gNo || feePortGroupId == gName));

                    // الرسم يُطبَّق إذا طابق المنفذ أو مجموعة المنافذ
                    bool portMatch = byPort || byGroup;

                    if (!portMatch) continue;

                    // ========== تحديد الكمية المنطبقة ==========
                    decimal applicableQty = 0, baseValue = 0;
                    bool isVehicle = (effect == "على مستوى القاطرة");

                    if (effect == "على مستوى البيان") { applicableQty = 1; baseValue = ParseDecimal(lblTotalFeesValue?.Text); }
                    else if (isVehicle) { applicableQty = 1; baseValue = ParseDecimal(lblTotalFeesValue?.Text); }
                    else if (effect == "على مستوى الصنف")
                    {
                        foreach (var ir in itemRows)
                            if (!string.IsNullOrEmpty(feeItemName) && ir.Name == feeItemName) { applicableQty += ir.Qty; baseValue += ir.TotalValue; }
                    }
                    else if (effect == "على مستوى مجموعة الأصناف الفرعية")
                    {
                        foreach (var ir in itemRows)
                        {
                            bool subMatch =
                                (!string.IsNullOrEmpty(feeItemGroup) && (feeItemGroup == ir.SubName || feeItemGroup == ir.SubNo)) ||
                                (!string.IsNullOrEmpty(feeItemId) && (feeItemId == ir.SubNo || feeItemId == ir.SubName));
                            if (subMatch) { applicableQty += ir.Qty; baseValue += ir.TotalValue; }
                        }
                    }

                    if (!isVehicle && effect != "على مستوى البيان" && applicableQty <= 0) continue;
                    if (!isVehicle && minLimit > 0 && applicableQty < minLimit) continue;

                    // ========== احتساب المبلغ ==========
                    decimal finalAmount;
                    if (valueType == "نسبة مئوية")
                    {
                        finalAmount = Math.Round(baseValue * unitAmount / 100m, 2);
                    }
                    else if (isVehicle)
                    {
                        // الكمية: المبلغ كما هو | الكمية ومضاعفاتها: الحد الأدنى × المبلغ
                        if (calcType == "الكمية ومضاعفاتها")
                            finalAmount = Math.Round((minLimit > 0 ? minLimit : 1) * unitAmount, 2);
                        else
                            finalAmount = Math.Round(unitAmount, 2);
                    }
                    else
                    {
                        decimal multiplier = applicableQty;
                        if (calcType == "الكمية ومضاعفاتها" && minLimit > 0) multiplier = Math.Ceiling(applicableQty / minLimit);
                        finalAmount = Math.Round(unitAmount * multiplier, 2);
                    }
                    if (finalAmount <= 0) continue;

                    // ========== إضافة/تجميع الصف ==========
                    int existing = -1;
                    for (int i = 0; i < dgvAdditionalAmounts.Rows.Count; i++)
                    {
                        var rr = dgvAdditionalAmounts.Rows[i];
                        if (!rr.IsNewRow && rr.Tag != null && rr.Tag.ToString() == "AUTO_FEE" && rr.Cells["colItemName"].Value?.ToString() == feeName) { existing = i; break; }
                    }

                    if (existing >= 0)
                    {
                        decimal old = ParseDecimal(dgvAdditionalAmounts.Rows[existing].Cells["colAmount"].Value?.ToString());
                        dgvAdditionalAmounts.Rows[existing].Cells["colAmount"].Value = old + finalAmount;
                    }
                    else
                    {
                        int idx = dgvAdditionalAmounts.Rows.Add(feeName, finalAmount);
                        dgvAdditionalAmounts.Rows[idx].Tag = "AUTO_FEE";
                        dgvAdditionalAmounts.Rows[idx].ReadOnly = true;
                        dgvAdditionalAmounts.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
                        dgvAdditionalAmounts.Rows[idx].DefaultCellStyle.ForeColor = Color.DarkOrange;
                    }
                }
                catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"خطأ في معالجة الرسم: {ex.Message}"); }
            }
            CalculateAdditionalAmounts();
        }
        /// <summary>
        /// الحصول على اسم مجموعة المنافذ للمنفذ المحدد
        /// </summary>
        /// <summary>
        /// ✅ جلب (رقم + اسم) مجموعة المنافذ للمنفذ المحدد - اكتشاف تلقائي شامل
        /// </summary>
        /// <summary>
        /// ✅ جلب (رقم + اسم) مجموعة المنافذ للمنفذ المحدد - كل الاستراتيجيات الممكنة
        /// </summary>
        /// <summary>
        /// ✅ جلب (رقم + اسم) مجموعة المنافذ للمنفذ المحدد - كل الاستراتيجيات الممكنة
        /// </summary>
        private (string GroupNo, string GroupName) GetPortGroupInfo(string portName)
        {
            string gNo = "", gName = "";
            if (string.IsNullOrEmpty(portName)) return (gNo, gName);

            try
            {
                // المنفذ يخزّن اسم مجموعته في عمود Classification
                var val = DatabaseHelper.ExecuteScalar(
                    "SELECT Classification FROM Ports WHERE PortName = @P LIMIT 1",
                    DatabaseHelper.CreateParameter("@P", portName));
                string v = val?.ToString().Trim() ?? "";

                if (!string.IsNullOrEmpty(v))
                {
                    DataTable g = DatabaseHelper.ExecuteQuery(
                        "SELECT GroupNo, GroupName FROM PortGroups WHERE GroupName = @V OR GroupNo = @V LIMIT 1",
                        DatabaseHelper.CreateParameter("@V", v));
                    if (g != null && g.Rows.Count > 0)
                    {
                        gNo = g.Rows[0]["GroupNo"]?.ToString().Trim() ?? "";
                        gName = g.Rows[0]["GroupName"]?.ToString().Trim() ?? "";
                    }
                    else
                    {
                        gName = v; // المجموعة غير موجودة في الجدول، نستخدم النص كما هو
                    }
                }
            }
            catch { }
            return (gNo, gName);
        }

        // ✅ دالة مساعدة: جلب أسماء أعمدة أي جدول
        private HashSet<string> GetTableColumns(string tableName)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                DataTable schema = DatabaseHelper.ExecuteQuery($"PRAGMA table_info({tableName})");
                if (schema != null) foreach (DataRow r in schema.Rows) set.Add(r["name"].ToString());
            }
            catch { }
            return set;
        }

        // ✅ دالة مساعدة: أول عمود موجود من قائمة مرشحة
        private string FirstExistingColumn(HashSet<string> cols, string[] candidates)
        {
            foreach (var c in candidates) if (cols.Contains(c)) return c;
            return null;
        }

        // ✅ دالة مساعدة: أول عمود موجود من قائمة مرشحة
       
        /// <summary>
        /// الحصول على اسم المجموعة الفرعية للصنف
        /// </summary>
        private (string SubNo, string SubName) GetItemSubGroupInfo(string productName)
        {
            string sNo = "", sName = "";
            try
            {
                DataTable schema = DatabaseHelper.ExecuteQuery("PRAGMA table_info(Items)");
                var cols = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                if (schema != null) foreach (DataRow r in schema.Rows) cols.Add(r["name"].ToString());

                string noCol = cols.Contains("SubGroupUnder") ? "SubGroupUnder" : (cols.Contains("SubGroupNo") ? "SubGroupNo" : null);
                string nameCol = cols.Contains("SubGroupName") ? "SubGroupName" : null;

                if (noCol != null)
                {
                    var v = DatabaseHelper.ExecuteScalar($"SELECT {noCol} FROM Items WHERE ItemName = @N LIMIT 1",
                        DatabaseHelper.CreateParameter("@N", productName));
                    sNo = v?.ToString().Trim() ?? "";
                }
                if (nameCol != null)
                {
                    var v2 = DatabaseHelper.ExecuteScalar($"SELECT {nameCol} FROM Items WHERE ItemName = @N LIMIT 1",
                        DatabaseHelper.CreateParameter("@N", productName));
                    sName = v2?.ToString().Trim() ?? "";
                }

                // ✅ تحويل الرقم إلى اسم (من جداول المجموعات الفرعية أو التصاريح)
                if (!string.IsNullOrEmpty(sNo) && string.IsNullOrEmpty(sName))
                {
                    foreach (string t in new[] { "SubGroups", "ItemSubGroups", "SubGroupItems" })
                    {
                        try
                        {
                            var n = DatabaseHelper.ExecuteScalar($"SELECT SubGroupName FROM {t} WHERE SubGroupNo = @V OR SubGroupName = @V LIMIT 1",
                                DatabaseHelper.CreateParameter("@V", sNo));
                            if (n != null && n != DBNull.Value && !string.IsNullOrEmpty(n.ToString())) { sName = n.ToString(); break; }
                        }
                        catch { }
                    }
                    if (string.IsNullOrEmpty(sName))
                    {
                        try
                        {
                            var n2 = DatabaseHelper.ExecuteScalar("SELECT SubGroupName FROM AgriculturalPermitItems WHERE SubGroupNo = @V LIMIT 1",
                                DatabaseHelper.CreateParameter("@V", sNo));
                            if (n2 != null && n2 != DBNull.Value) sName = n2.ToString();
                        }
                        catch { }
                    }
                }

                // ✅ تحويل الاسم إلى رقم
                if (!string.IsNullOrEmpty(sName) && string.IsNullOrEmpty(sNo))
                {
                    foreach (string t in new[] { "SubGroups", "ItemSubGroups", "SubGroupItems" })
                    {
                        try
                        {
                            var n = DatabaseHelper.ExecuteScalar($"SELECT SubGroupNo FROM {t} WHERE SubGroupName = @V LIMIT 1",
                                DatabaseHelper.CreateParameter("@V", sName));
                            if (n != null && n != DBNull.Value && !string.IsNullOrEmpty(n.ToString())) { sNo = n.ToString(); break; }
                        }
                        catch { }
                    }
                    if (string.IsNullOrEmpty(sNo))
                    {
                        try
                        {
                            var n2 = DatabaseHelper.ExecuteScalar("SELECT SubGroupNo FROM AgriculturalPermitItems WHERE SubGroupName = @V LIMIT 1",
                                DatabaseHelper.CreateParameter("@V", sName));
                            if (n2 != null && n2 != DBNull.Value) sNo = n2.ToString();
                        }
                        catch { }
                    }
                }
            }
            catch { }
            return (sNo, sName);
        }
    }
}