namespace YemenCustomsSystem
{
    partial class PortsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            InitializeToolStrip();
            InitializeMainPanels();
            InitializePortData();
            InitializeForm();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #region ⚙️ تهيئة شريط الأدوات
        private void InitializeToolStrip()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnHome = new System.Windows.Forms.ToolStripButton();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnView = new System.Windows.Forms.ToolStripButton();
            this.btnSearch = new System.Windows.Forms.ToolStripButton();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.btnPrevious = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnExit = new System.Windows.Forms.ToolStripButton();

            this.toolStrip1.BackColor = System.Drawing.Color.White;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnHome, this.btnNew, this.btnSave, this.btnEdit, this.btnDelete,
                this.btnView, this.btnSearch, this.btnPrint, this.btnPrevious, this.btnNext, this.btnExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStrip1.TabIndex = 0;

            this.btnHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnHome.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(85, 57);
            this.btnHome.Text = "🏠 الرئيسية";
            this.btnHome.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);

            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(70, 57);
            this.btnNew.Text = "➕ جديد";
            this.btnNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(70, 57);
            this.btnSave.Text = "💾 حفظ";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(255, 152, 0);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(70, 57);
            this.btnEdit.Text = "✏️ تعديل";
            this.btnEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);

            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.FromArgb(244, 67, 54);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(70, 57);
            this.btnDelete.Text = "🗑️ حذف";
            this.btnDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.btnView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnView.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnView.ForeColor = System.Drawing.Color.FromArgb(96, 125, 139);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(70, 57);
            this.btnView.Text = "👁️ عرض";
            this.btnView.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnView.Click += new System.EventHandler(this.btnView_Click);

            this.btnSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnSearch.ForeColor = System.Drawing.Color.FromArgb(96, 125, 139);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(70, 57);
            this.btnSearch.Text = "🔍 بحث";
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnPrint.ForeColor = System.Drawing.Color.FromArgb(96, 125, 139);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(70, 57);
            this.btnPrint.Text = "🖨️ طباعة";
            this.btnPrint.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);

            this.btnPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnPrevious.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnPrevious.ForeColor = System.Drawing.Color.FromArgb(96, 125, 139);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(60, 57);
            this.btnPrevious.Text = "⬅️ سابق";
            this.btnPrevious.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);

            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnNext.ForeColor = System.Drawing.Color.FromArgb(96, 125, 139);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(60, 57);
            this.btnNext.Text = "تالي ➡️";
            this.btnNext.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);

            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(158, 158, 158);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 57);
            this.btnExit.Text = "❌ خروج";
            this.btnExit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
        }
        #endregion

        #region 📦 تهيئة اللوحات الرئيسية
        private void InitializeMainPanels()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.dgvPorts = new System.Windows.Forms.DataGridView();

            this.pnlMain.BackColor = System.Drawing.Color.FromArgb(245, 248, 250);
            this.pnlMain.Controls.Add(this.pnlGrid);
            this.pnlMain.Controls.Add(this.pnlTop);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 60);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(15);
            this.pnlMain.Size = new System.Drawing.Size(1400, 640);
            this.pnlMain.TabIndex = 1;

            this.pnlTop.BackColor = System.Drawing.Color.White;
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(15, 15);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1370, 230);
            this.pnlTop.TabIndex = 0;
            this.pnlTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.pnlGrid.BackColor = System.Drawing.Color.White;
            this.pnlGrid.Controls.Add(this.dgvPorts);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(15, 260);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(1370, 380);
            this.pnlGrid.TabIndex = 1;
            this.pnlGrid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            this.dgvPorts.AllowUserToAddRows = false;
            this.dgvPorts.AllowUserToDeleteRows = false;
            this.dgvPorts.BackgroundColor = System.Drawing.Color.White;
            this.dgvPorts.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPorts.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvPorts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPorts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPorts.EnableHeadersVisualStyles = false;
            this.dgvPorts.GridColor = System.Drawing.Color.FromArgb(224, 224, 224);
            this.dgvPorts.Location = new System.Drawing.Point(0, 0);
            this.dgvPorts.MultiSelect = false;
            this.dgvPorts.Name = "dgvPorts";
            this.dgvPorts.ReadOnly = true;
            this.dgvPorts.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvPorts.RowHeadersVisible = false;
            this.dgvPorts.RowTemplate.Height = 35;
            this.dgvPorts.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPorts.Size = new System.Drawing.Size(1368, 378);
            this.dgvPorts.TabIndex = 0;
        }
        #endregion

        #region 🏛️ تهيئة بيانات المنفذ
        private void InitializePortData()
        {
            this.grpPortData = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();

            this.lblPortNumber = new System.Windows.Forms.Label();
            this.txtPortNumber = new System.Windows.Forms.TextBox();
            this.lblPortName = new System.Windows.Forms.Label();
            this.txtPortName = new System.Windows.Forms.TextBox();
            this.lblPortAddress = new System.Windows.Forms.Label();
            this.txtPortAddress = new System.Windows.Forms.TextBox();
            this.lblPortType = new System.Windows.Forms.Label();
            this.comboPortType = new System.Windows.Forms.ComboBox();
            this.lblCurrency = new System.Windows.Forms.Label();
            this.comboCurrency = new System.Windows.Forms.ComboBox();

            this.grpClassification = new System.Windows.Forms.GroupBox();
            this.cmbPortGroup = new System.Windows.Forms.ComboBox(); // ✅ العنصر الجديد

            this.grpNature = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelNature = new System.Windows.Forms.FlowLayoutPanel();
            this.radioLand = new System.Windows.Forms.RadioButton();
            this.radioAir = new System.Windows.Forms.RadioButton();
            this.radioSea = new System.Windows.Forms.RadioButton();

            this.grpRate = new System.Windows.Forms.GroupBox();
            this.lblExchangeRateUSD = new System.Windows.Forms.Label();
            this.txtExchangeRateUSD = new System.Windows.Forms.TextBox();
            this.lblMinRate = new System.Windows.Forms.Label();
            this.txtMinRate = new System.Windows.Forms.TextBox();
            this.lblMaxRate = new System.Windows.Forms.Label();
            this.txtMaxRate = new System.Windows.Forms.TextBox();

            this.grpPayment = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanelPayment = new System.Windows.Forms.FlowLayoutPanel();
            this.radioCash = new System.Windows.Forms.RadioButton();
            this.radioCheck = new System.Windows.Forms.RadioButton();
            this.lblCashPercent = new System.Windows.Forms.Label();
            this.txtCashPercent = new System.Windows.Forms.TextBox();
            this.lblCheckPercent = new System.Windows.Forms.Label();
            this.txtCheckPercent = new System.Windows.Forms.TextBox();

            this.grpPortData.BackColor = System.Drawing.Color.White;
            this.grpPortData.Controls.Add(this.tableLayoutPanel1);
            this.grpPortData.Controls.Add(this.grpRate);
            this.grpPortData.Controls.Add(this.grpNature);
            this.grpPortData.Controls.Add(this.grpClassification);
            this.grpPortData.Controls.Add(this.grpPayment);
            this.grpPortData.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpPortData.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grpPortData.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.grpPortData.Location = new System.Drawing.Point(0, 0);
            this.grpPortData.Name = "grpPortData";
            this.grpPortData.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.grpPortData.Size = new System.Drawing.Size(1368, 228);
            this.grpPortData.TabIndex = 1;
            this.grpPortData.TabStop = false;
            this.grpPortData.Text = "🏛️ بيانات المنفذ";

            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(530, 25);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.Size = new System.Drawing.Size(480, 140);
            this.tableLayoutPanel1.TabIndex = 0;

            this.lblPortNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPortNumber.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPortNumber.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.lblPortNumber.Text = "رقم المنفذ:";
            this.lblPortNumber.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtPortNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPortNumber.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtPortNumber.ReadOnly = true;
            this.txtPortNumber.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
            this.txtPortNumber.Margin = new System.Windows.Forms.Padding(3, 5, 10, 5);

            this.lblPortName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPortName.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPortName.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.lblPortName.Text = "اسم المنفذ:";
            this.lblPortName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtPortName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPortName.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtPortName.Margin = new System.Windows.Forms.Padding(3, 5, 10, 5);

            this.lblPortAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPortAddress.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPortAddress.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.lblPortAddress.Text = "عنوان المنفذ:";
            this.lblPortAddress.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.txtPortAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPortAddress.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtPortAddress.Margin = new System.Windows.Forms.Padding(3, 5, 10, 5);

            this.lblPortType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPortType.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblPortType.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.lblPortType.Text = "نوع المنفذ:";
            this.lblPortType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.comboPortType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboPortType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPortType.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.comboPortType.Items.AddRange(new object[] { "رئيسي", "رقابي" });
            this.comboPortType.SelectedIndex = 0;
            this.comboPortType.Margin = new System.Windows.Forms.Padding(3, 5, 10, 5);

            this.lblCurrency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblCurrency.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCurrency.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.lblCurrency.Text = "العملة:";
            this.lblCurrency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            this.comboCurrency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboCurrency.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboCurrency.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.comboCurrency.Items.AddRange(new object[] { "ريال يمني", "ريال جنوبي" });
            this.comboCurrency.SelectedIndex = 0;
            this.comboCurrency.Margin = new System.Windows.Forms.Padding(3, 5, 10, 5);

            this.tableLayoutPanel1.Controls.Add(this.lblPortNumber, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtPortNumber, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPortName, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.txtPortName, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPortAddress, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtPortAddress, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblPortType, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.comboPortType, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblCurrency, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.comboCurrency, 1, 2);

            // ✅ تهيئة مجموعة التصنيف الجديدة (ComboBox بدلاً من RadioButtons)
            this.grpClassification.BackColor = System.Drawing.Color.White;
            this.grpClassification.Controls.Add(this.cmbPortGroup);
            this.grpClassification.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpClassification.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.grpClassification.Location = new System.Drawing.Point(18, 25);
            this.grpClassification.Name = "grpClassification";
            this.grpClassification.Size = new System.Drawing.Size(250, 80);
            this.grpClassification.TabIndex = 1;
            this.grpClassification.TabStop = false;
            this.grpClassification.Text = "🗂️ التصنيف (مجموعة المنافذ)";

            this.cmbPortGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbPortGroup.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPortGroup.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.cmbPortGroup.FormattingEnabled = true;
            this.cmbPortGroup.Location = new System.Drawing.Point(3, 22);
            this.cmbPortGroup.Name = "cmbPortGroup";
            this.cmbPortGroup.Size = new System.Drawing.Size(244, 25);
            this.cmbPortGroup.TabIndex = 0;

            this.grpNature.BackColor = System.Drawing.Color.White;
            this.grpNature.Controls.Add(this.flowLayoutPanelNature);
            this.grpNature.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpNature.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.grpNature.Location = new System.Drawing.Point(18, 110);
            this.grpNature.Name = "grpNature";
            this.grpNature.Size = new System.Drawing.Size(250, 80);
            this.grpNature.TabIndex = 2;
            this.grpNature.TabStop = false;
            this.grpNature.Text = "🌍 الطبيعة";

            this.flowLayoutPanelNature.AutoSize = true;
            this.flowLayoutPanelNature.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelNature.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanelNature.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanelNature.Name = "flowLayoutPanelNature";
            this.flowLayoutPanelNature.Padding = new System.Windows.Forms.Padding(10, 10, 10, 5);
            this.flowLayoutPanelNature.Size = new System.Drawing.Size(244, 55);
            this.flowLayoutPanelNature.TabIndex = 0;

            this.radioLand.AutoSize = true;
            this.radioLand.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioLand.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.radioLand.Text = "بري";

            this.radioAir.AutoSize = true;
            this.radioAir.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioAir.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.radioAir.Text = "جوي";

            this.radioSea.AutoSize = true;
            this.radioSea.Checked = true;
            this.radioSea.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioSea.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.radioSea.Text = "بحري";

            this.flowLayoutPanelNature.Controls.Add(this.radioLand);
            this.flowLayoutPanelNature.Controls.Add(this.radioAir);
            this.flowLayoutPanelNature.Controls.Add(this.radioSea);

            this.grpRate.BackColor = System.Drawing.Color.White;
            this.grpRate.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpRate.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.grpRate.Location = new System.Drawing.Point(280, 25);
            this.grpRate.Name = "grpRate";
            this.grpRate.Size = new System.Drawing.Size(240, 210);
            this.grpRate.TabIndex = 3;
            this.grpRate.TabStop = false;
            this.grpRate.Text = "💰 سعر الصرف الجمركي";

            this.lblExchangeRateUSD.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblExchangeRateUSD.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.lblExchangeRateUSD.Location = new System.Drawing.Point(15, 25);
            this.lblExchangeRateUSD.Size = new System.Drawing.Size(210, 20);
            this.lblExchangeRateUSD.Text = "💵 سعر الصرف مقابل الدولار:";
            this.lblExchangeRateUSD.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtExchangeRateUSD.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtExchangeRateUSD.Location = new System.Drawing.Point(15, 50);
            this.txtExchangeRateUSD.Size = new System.Drawing.Size(210, 25);
            this.txtExchangeRateUSD.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtExchangeRateUSD.BackColor = System.Drawing.Color.FromArgb(255, 248, 225);
            this.txtExchangeRateUSD.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);

            this.lblMinRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMinRate.ForeColor = System.Drawing.Color.FromArgb(22, 160, 133);
            this.lblMinRate.Location = new System.Drawing.Point(15, 85);
            this.lblMinRate.Size = new System.Drawing.Size(210, 20);
            this.lblMinRate.Text = "⬇️ الحد الأدنى:";
            this.lblMinRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtMinRate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtMinRate.Location = new System.Drawing.Point(15, 110);
            this.txtMinRate.Size = new System.Drawing.Size(210, 25);
            this.txtMinRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMinRate.BackColor = System.Drawing.Color.FromArgb(232, 245, 233);
            this.txtMinRate.ForeColor = System.Drawing.Color.FromArgb(22, 160, 133);

            this.lblMaxRate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMaxRate.ForeColor = System.Drawing.Color.FromArgb(211, 47, 47);
            this.lblMaxRate.Location = new System.Drawing.Point(15, 145);
            this.lblMaxRate.Size = new System.Drawing.Size(210, 20);
            this.lblMaxRate.Text = "⬆️ الحد الأعلى:";
            this.lblMaxRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtMaxRate.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.txtMaxRate.Location = new System.Drawing.Point(15, 170);
            this.txtMaxRate.Size = new System.Drawing.Size(210, 25);
            this.txtMaxRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtMaxRate.BackColor = System.Drawing.Color.FromArgb(255, 235, 238);
            this.txtMaxRate.ForeColor = System.Drawing.Color.FromArgb(211, 47, 47);

            this.grpRate.Controls.Add(this.lblExchangeRateUSD);
            this.grpRate.Controls.Add(this.txtExchangeRateUSD);
            this.grpRate.Controls.Add(this.lblMinRate);
            this.grpRate.Controls.Add(this.txtMinRate);
            this.grpRate.Controls.Add(this.lblMaxRate);
            this.grpRate.Controls.Add(this.txtMaxRate);

            this.grpPayment.BackColor = System.Drawing.Color.White;
            this.grpPayment.Controls.Add(this.flowLayoutPanelPayment);
            this.grpPayment.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpPayment.ForeColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.grpPayment.Location = new System.Drawing.Point(280, 110);
            this.grpPayment.Name = "grpPayment";
            this.grpPayment.Size = new System.Drawing.Size(240, 80);
            this.grpPayment.TabIndex = 4;
            this.grpPayment.TabStop = false;
            this.grpPayment.Text = "💳 طريقة الدفع";

            this.flowLayoutPanelPayment.AutoSize = true;
            this.flowLayoutPanelPayment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelPayment.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanelPayment.Location = new System.Drawing.Point(3, 22);
            this.flowLayoutPanelPayment.Name = "flowLayoutPanelPayment";
            this.flowLayoutPanelPayment.Padding = new System.Windows.Forms.Padding(10, 8, 10, 5);
            this.flowLayoutPanelPayment.Size = new System.Drawing.Size(234, 55);
            this.flowLayoutPanelPayment.TabIndex = 0;

            this.radioCash.AutoSize = true;
            this.radioCash.Checked = true;
            this.radioCash.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioCash.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.radioCash.Text = "نقد";

            this.radioCheck.AutoSize = true;
            this.radioCheck.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.radioCheck.ForeColor = System.Drawing.Color.FromArgb(66, 66, 66);
            this.radioCheck.Text = "شيك";

            this.lblCashPercent.AutoSize = true;
            this.lblCashPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCashPercent.Text = "%";

            this.txtCashPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCashPercent.Size = new System.Drawing.Size(50, 24);
            this.txtCashPercent.Text = "100";
            this.txtCashPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.lblCheckPercent.AutoSize = true;
            this.lblCheckPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCheckPercent.Text = "%";

            this.txtCheckPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCheckPercent.Size = new System.Drawing.Size(50, 24);
            this.txtCheckPercent.Text = "0";
            this.txtCheckPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.flowLayoutPanelPayment.Controls.Add(this.radioCheck);
            this.flowLayoutPanelPayment.Controls.Add(this.txtCheckPercent);
            this.flowLayoutPanelPayment.Controls.Add(this.lblCheckPercent);
            this.flowLayoutPanelPayment.Controls.Add(this.radioCash);
            this.flowLayoutPanelPayment.Controls.Add(this.txtCashPercent);
            this.flowLayoutPanelPayment.Controls.Add(this.lblCashPercent);

            this.pnlTop.Controls.Add(this.grpPortData);
        }
        #endregion

        #region 🖥️ تهيئة النموذج الرئيسي
        private void InitializeForm()
        {
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 700);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.toolStrip1);
            this.Name = "PortsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "🏛️ تهيئة المنافذ - نظام الجمارك اليمنية";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
        }
        #endregion

        // ==================== تعريفات العناصر ====================
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnHome, btnNew, btnSave, btnEdit, btnDelete, btnView, btnSearch, btnPrint, btnPrevious, btnNext, btnExit;

        private System.Windows.Forms.Panel pnlMain, pnlTop, pnlGrid;
        private System.Windows.Forms.GroupBox grpPortData;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblPortNumber, lblPortName, lblPortAddress, lblPortType, lblCurrency;
        private System.Windows.Forms.TextBox txtPortNumber, txtPortName, txtPortAddress;
        private System.Windows.Forms.ComboBox comboPortType, comboCurrency;

        private System.Windows.Forms.GroupBox grpClassification, grpNature, grpRate, grpPayment;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelNature, flowLayoutPanelPayment;
        private System.Windows.Forms.RadioButton radioLand, radioAir, radioSea, radioCash, radioCheck;
        private System.Windows.Forms.Label lblCheckPercent, lblCashPercent;
        private System.Windows.Forms.TextBox txtCheckPercent, txtCashPercent;

        // ✅ تعريفات الحقول الجديدة لسعر الصرف
        private System.Windows.Forms.Label lblExchangeRateUSD, lblMinRate, lblMaxRate;
        private System.Windows.Forms.TextBox txtExchangeRateUSD, txtMinRate, txtMaxRate;

        // ✅ تعريف ComboBox مجموعة المنافذ الجديد
        private System.Windows.Forms.ComboBox cmbPortGroup;

        private System.Windows.Forms.DataGridView dgvPorts;
    }
}