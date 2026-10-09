using System;
using System.Drawing;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    partial class ItemDataForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ==========================================
            // شريط الأدوات
            // ==========================================
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparatorGroups = new System.Windows.Forms.ToolStripSeparator();
            this.btnPrint = new System.Windows.Forms.ToolStripButton();
            this.btnExport = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFirst = new System.Windows.Forms.ToolStripButton();
            this.btnPrevious = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnLast = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSearch = new System.Windows.Forms.ToolStripButton();
            this.btnFilter = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnHome = new System.Windows.Forms.ToolStripButton();
            this.btnExit = new System.Windows.Forms.ToolStripButton();

            this.grpHeader = new System.Windows.Forms.GroupBox();

            // العمود الأيمن
            this.lblItemType = new System.Windows.Forms.Label();
            this.cmbItemType = new System.Windows.Forms.ComboBox();
            this.lblGroupNo = new System.Windows.Forms.Label();
            this.txtGroupNo = new System.Windows.Forms.TextBox();
            this.lblGroupName = new System.Windows.Forms.Label();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.lblSubGroupUnder = new System.Windows.Forms.Label();
            this.txtSubGroupUnder = new System.Windows.Forms.TextBox();
            this.lblItemCode = new System.Windows.Forms.Label();
            this.txtItemCode = new System.Windows.Forms.TextBox();
            this.lblItemName = new System.Windows.Forms.Label();
            this.txtItemName = new System.Windows.Forms.TextBox();

            // العمود الأوسط
            this.lblBranch = new System.Windows.Forms.Label();
            this.txtBranch = new System.Windows.Forms.TextBox();
            this.lblRefNo = new System.Windows.Forms.Label();
            this.txtRefNo = new System.Windows.Forms.TextBox();

            // ✅ الحقول الجديدة في الـ Header (ComboBoxes بدلاً من CheckBoxes)
            this.lblCountryOfOrigin = new System.Windows.Forms.Label();
            this.txtCountryOfOrigin = new System.Windows.Forms.TextBox();
            this.lblAgriculturePermit = new System.Windows.Forms.Label();
            this.cmbAgriculturePermit = new System.Windows.Forms.ComboBox();
            this.lblPaymentRelease = new System.Windows.Forms.Label();
            this.cmbPaymentRelease = new System.Windows.Forms.ComboBox();

            // العمود الأيسر
            this.lblBarcode = new System.Windows.Forms.Label();
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.lblForeignName = new System.Windows.Forms.Label();
            this.txtForeignName = new System.Windows.Forms.TextBox();

            // صورة الصنف
            this.picItem = new System.Windows.Forms.PictureBox();
            this.btnImgAdd = new System.Windows.Forms.Button();
            this.btnImgDel = new System.Windows.Forms.Button();
            this.lblItemImage = new System.Windows.Forms.Label();

            // التبويبات
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageMain = new System.Windows.Forms.TabPage();
            this.tabPageUnits = new System.Windows.Forms.TabPage();
            this.tabPageOther = new System.Windows.Forms.TabPage();
            this.tabPageCost = new System.Windows.Forms.TabPage();
            this.tabPageMovement = new System.Windows.Forms.TabPage();
            this.tabPageAnalysis = new System.Windows.Forms.TabPage();
            this.tabPagePrices = new System.Windows.Forms.TabPage();

            // تبويب البيانات الرئيسية
            this.lblItemSpecs = new System.Windows.Forms.Label();
            this.txtItemSpecs = new System.Windows.Forms.TextBox();
            this.lblItemSpecsForeign = new System.Windows.Forms.Label();
            this.txtItemSpecsForeign = new System.Windows.Forms.TextBox();
            this.lblAssistant = new System.Windows.Forms.Label();
            this.txtAssistant = new System.Windows.Forms.TextBox();
            this.lblDetailGroup = new System.Windows.Forms.Label();
            this.txtDetailGroup = new System.Windows.Forms.TextBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.txtClass = new System.Windows.Forms.TextBox();
            this.lblInitialCost = new System.Windows.Forms.Label();
            this.txtInitialCost = new System.Windows.Forms.TextBox();
            this.lblDefaultUnit = new System.Windows.Forms.Label();
            this.cmbDefaultUnit = new System.Windows.Forms.ComboBox();
            this.lblPurchaseInvoice = new System.Windows.Forms.Label();
            this.txtPurchaseInvoice = new System.Windows.Forms.TextBox();
            this.lblItemActivity = new System.Windows.Forms.Label();
            this.cmbItemActivity = new System.Windows.Forms.ComboBox();
            this.lblReturnPeriod = new System.Windows.Forms.Label();
            this.txtReturnPeriod = new System.Windows.Forms.TextBox();
            this.lblRank = new System.Windows.Forms.Label();
            this.cmbRank = new System.Windows.Forms.ComboBox();
            this.lblLength = new System.Windows.Forms.Label();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.lblSize = new System.Windows.Forms.Label();
            this.txtSize = new System.Windows.Forms.TextBox();
            this.lblWidth = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.lblColor = new System.Windows.Forms.Label();
            this.txtColor = new System.Windows.Forms.TextBox();
            this.lblHeight = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.lblSeason = new System.Windows.Forms.Label();
            this.cmbSeason = new System.Windows.Forms.ComboBox();
            this.lblVolume = new System.Windows.Forms.Label();
            this.txtVolume = new System.Windows.Forms.TextBox();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.txtMaterial = new System.Windows.Forms.TextBox();
            this.lblArea = new System.Windows.Forms.Label();
            this.txtArea = new System.Windows.Forms.TextBox();
            this.lblBrand = new System.Windows.Forms.Label();
            this.txtBrand = new System.Windows.Forms.TextBox();
            this.lblManufacturer = new System.Windows.Forms.Label();
            this.txtManufacturer = new System.Windows.Forms.TextBox();
            this.lblMfgNo = new System.Windows.Forms.Label();
            this.txtMfgNo = new System.Windows.Forms.TextBox();
            this.lblCountryMain = new System.Windows.Forms.Label();
            this.txtCountryMain = new System.Windows.Forms.TextBox();
            this.lblStorage = new System.Windows.Forms.Label();
            this.txtStorage = new System.Windows.Forms.TextBox();
            this.lblOpenCost = new System.Windows.Forms.Label();
            this.txtOpenCost = new System.Windows.Forms.TextBox();
            this.lblOpeningStock = new System.Windows.Forms.Label();
            this.txtOpeningStock = new System.Windows.Forms.TextBox();
            this.lblStopDate = new System.Windows.Forms.Label();
            this.txtStopDate = new System.Windows.Forms.TextBox();
            this.lblAvailableQty = new System.Windows.Forms.Label();
            this.txtAvailableQty = new System.Windows.Forms.TextBox();
            this.lblStopReason = new System.Windows.Forms.Label();
            this.txtStopReason = new System.Windows.Forms.TextBox();

            // تبويب الوحدات (تم دمج وحدات الوزن هنا)
            this.dgvUnits = new System.Windows.Forms.DataGridView();
            this.dgvWeightUnits = new System.Windows.Forms.DataGridView();
            this.colWeightUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWeightValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkMainUnit = new System.Windows.Forms.CheckBox();
            this.chkSalesUnit = new System.Windows.Forms.CheckBox();
            this.chkPurchaseUnit = new System.Windows.Forms.CheckBox();
            this.chkInventoryUnit = new System.Windows.Forms.CheckBox();
            this.chkNotForSale = new System.Windows.Forms.CheckBox();
            this.chkStop = new System.Windows.Forms.CheckBox();
            this.lblMainUnit = new System.Windows.Forms.Label();
            this.lblSalesUnit = new System.Windows.Forms.Label();
            this.lblPurchaseUnit = new System.Windows.Forms.Label();
            this.lblInventoryUnit = new System.Windows.Forms.Label();
            this.lblNotForSale = new System.Windows.Forms.Label();
            this.lblStop = new System.Windows.Forms.Label();

            // تبويب التكلفة
            this.dgvCost = new System.Windows.Forms.DataGridView();
            this.colWarehouse = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWarehouseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colInitialCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAvgCost = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAvailableQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewCheckBoxColumn();

            // تبويب بيانات أخرى
            this.chkAllowFreeQty = new System.Windows.Forms.CheckBox();
            this.chkHasWarranty = new System.Windows.Forms.CheckBox();
            this.lblWarrantyPeriod = new System.Windows.Forms.Label();
            this.txtWarrantyPeriod = new System.Windows.Forms.TextBox();
            this.lblWarrantyDay = new System.Windows.Forms.Label();
            this.chkHasCommission = new System.Windows.Forms.CheckBox();
            this.lblCommissionType = new System.Windows.Forms.Label();
            this.cmbCommissionType = new System.Windows.Forms.ComboBox();
            this.txtCommissionType = new System.Windows.Forms.TextBox();
            this.lblFreeQtyPercent = new System.Windows.Forms.Label();
            this.txtFreeQtyPercent = new System.Windows.Forms.TextBox();
            this.lblExpiryDuration = new System.Windows.Forms.Label();
            this.txtExpiryDuration = new System.Windows.Forms.TextBox();
            this.lblExpiryDay = new System.Windows.Forms.Label();
            this.lblMinCostPercent = new System.Windows.Forms.Label();
            this.txtMinCostPercent = new System.Windows.Forms.TextBox();
            this.lblMinCostSign = new System.Windows.Forms.Label();
            this.lblMaxCostPercent = new System.Windows.Forms.Label();
            this.txtMaxCostPercent = new System.Windows.Forms.TextBox();
            this.lblMaxCostSign = new System.Windows.Forms.Label();
            this.pnlSeparator = new System.Windows.Forms.Panel();
            this.chkIsWeighted = new System.Windows.Forms.CheckBox();
            this.lblScaleName = new System.Windows.Forms.Label();
            this.cmbScaleName = new System.Windows.Forms.ComboBox();
            this.txtScaleName = new System.Windows.Forms.TextBox();
            this.lblScaleBtn = new System.Windows.Forms.Button();
            this.lblKey = new System.Windows.Forms.Label();
            this.txtKey = new System.Windows.Forms.TextBox();
            this.lblDocType = new System.Windows.Forms.Label();
            this.txtDocType = new System.Windows.Forms.TextBox();
            this.lblDocRefNo = new System.Windows.Forms.Label();
            this.txtDocRefNo = new System.Windows.Forms.TextBox();
            this.lblDocSeq = new System.Windows.Forms.Label();
            this.txtDocSeq = new System.Windows.Forms.TextBox();

            this.toolStrip1.SuspendLayout();
            this.grpHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPageMain.SuspendLayout();
            this.tabPageUnits.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnits)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWeightUnits)).BeginInit();
            this.tabPageCost.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCost)).BeginInit();
            this.tabPageOther.SuspendLayout();
            this.SuspendLayout();

            // ==========================================
            // شريط الأدوات
            // ==========================================
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnNew, this.btnSave, this.btnDelete, this.toolStripSeparator1,
                this.btnPrint, this.btnExport, this.toolStripSeparator2,
                this.btnFirst, this.btnPrevious, this.btnNext, this.btnLast, this.toolStripSeparator3,
                this.btnSearch, this.btnFilter, this.toolStripSeparator4,
                this.btnHome, this.btnExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1366, 30);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));

            this.btnHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnHome.ForeColor = System.Drawing.Color.DarkBlue;
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(116, 27);
            this.btnHome.Text = LanguageManager.Get("btn_home");
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);

            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNew.ForeColor = System.Drawing.Color.Green;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(45, 27);
            this.btnNew.Text = LanguageManager.Get("btn_new");
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.ForeColor = System.Drawing.Color.Blue;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(40, 27);
            this.btnSave.Text = LanguageManager.Get("btn_save");
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDelete.ForeColor = System.Drawing.Color.Red;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(40, 27);
            this.btnDelete.Text = LanguageManager.Get("btn_delete");
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 30);
            this.toolStripSeparatorGroups.Name = "toolStripSeparatorGroups";
            this.toolStripSeparatorGroups.Size = new System.Drawing.Size(6, 30);

            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(50, 27);
            this.btnPrint.Text = LanguageManager.Get("btn_print");
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);

            this.btnExport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(50, 27);
            this.btnExport.Text = LanguageManager.Get("btn_export");
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);

            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 30);

            this.btnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(40, 27);
            this.btnFirst.Text = LanguageManager.Get("btn_first");
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);

            this.btnPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(50, 27);
            this.btnPrevious.Text = LanguageManager.Get("btn_previous");
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);

            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(40, 27);
            this.btnNext.Text = LanguageManager.Get("btn_next");
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);

            this.btnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(40, 27);
            this.btnLast.Text = LanguageManager.Get("btn_last");
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);

            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 30);

            this.btnSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(45, 27);
            this.btnSearch.Text = LanguageManager.Get("btn_search");
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.btnFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(50, 27);
            this.btnFilter.Text = LanguageManager.Get("btn_filter");
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);

            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 30);

            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(45, 27);
            this.btnExit.Text = LanguageManager.Get("btn_exit");
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // ==========================================
            // مجموعة البيانات (Header)
            // ==========================================
            this.grpHeader.Controls.Add(this.lblItemType);
            this.grpHeader.Controls.Add(this.cmbItemType);
            this.grpHeader.Controls.Add(this.lblGroupNo);
            this.grpHeader.Controls.Add(this.txtGroupNo);
            this.grpHeader.Controls.Add(this.lblGroupName);
            this.grpHeader.Controls.Add(this.txtGroupName);
            this.grpHeader.Controls.Add(this.lblSubGroupUnder);
            this.grpHeader.Controls.Add(this.txtSubGroupUnder);
            this.grpHeader.Controls.Add(this.lblItemCode);
            this.grpHeader.Controls.Add(this.txtItemCode);
            this.grpHeader.Controls.Add(this.lblItemName);
            this.grpHeader.Controls.Add(this.txtItemName);
            this.grpHeader.Controls.Add(this.lblBranch);
            this.grpHeader.Controls.Add(this.txtBranch);
            this.grpHeader.Controls.Add(this.lblRefNo);
            this.grpHeader.Controls.Add(this.txtRefNo);
            this.grpHeader.Controls.Add(this.lblBarcode);
            this.grpHeader.Controls.Add(this.txtBarcode);
            this.grpHeader.Controls.Add(this.lblForeignName);
            this.grpHeader.Controls.Add(this.txtForeignName);
            this.grpHeader.Controls.Add(this.picItem);
            this.grpHeader.Controls.Add(this.btnImgAdd);
            this.grpHeader.Controls.Add(this.btnImgDel);
            this.grpHeader.Controls.Add(this.lblItemImage);
            this.grpHeader.Controls.Add(this.lblCountryOfOrigin);
            this.grpHeader.Controls.Add(this.txtCountryOfOrigin);
            this.grpHeader.Controls.Add(this.lblAgriculturePermit);
            this.grpHeader.Controls.Add(this.cmbAgriculturePermit);
            this.grpHeader.Controls.Add(this.lblPaymentRelease);
            this.grpHeader.Controls.Add(this.cmbPaymentRelease);

            this.grpHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHeader.Location = new System.Drawing.Point(0, 30);
            this.grpHeader.Name = "grpHeader";
            this.grpHeader.Size = new System.Drawing.Size(1366, 220);
            this.grpHeader.TabIndex = 1;
            this.grpHeader.TabStop = false;
            this.grpHeader.Text = LanguageManager.Get("grp_header");
            this.grpHeader.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.grpHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.grpHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));

            this.lblItemImage.AutoSize = true;
            this.lblItemImage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItemImage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblItemImage.Location = new System.Drawing.Point(270, 15);
            this.lblItemImage.Name = "lblItemImage";
            this.lblItemImage.Size = new System.Drawing.Size(85, 15);
            this.lblItemImage.Text = LanguageManager.Get("lbl_item_image");

            this.picItem.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.picItem.Location = new System.Drawing.Point(270, 35);
            this.picItem.Name = "picItem";
            this.picItem.Size = new System.Drawing.Size(70, 70);
            this.picItem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picItem.TabIndex = 100;
            this.picItem.TabStop = false;
            this.picItem.BackColor = System.Drawing.Color.White;

            this.btnImgAdd.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImgAdd.Location = new System.Drawing.Point(270, 110);
            this.btnImgAdd.Name = "btnImgAdd";
            this.btnImgAdd.Size = new System.Drawing.Size(30, 25);
            this.btnImgAdd.TabIndex = 101;
            this.btnImgAdd.Text = "+";
            this.btnImgAdd.UseVisualStyleBackColor = true;
            this.btnImgAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(184)))), ((int)(((byte)(92)))));
            this.btnImgAdd.ForeColor = System.Drawing.Color.White;
            this.btnImgAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImgAdd.Click += new System.EventHandler(this.btnImgAdd_Click);

            this.btnImgDel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnImgDel.Location = new System.Drawing.Point(310, 110);
            this.btnImgDel.Name = "btnImgDel";
            this.btnImgDel.Size = new System.Drawing.Size(30, 25);
            this.btnImgDel.TabIndex = 102;
            this.btnImgDel.Text = "X";
            this.btnImgDel.UseVisualStyleBackColor = true;
            this.btnImgDel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(83)))), ((int)(((byte)(79)))));
            this.btnImgDel.ForeColor = System.Drawing.Color.White;
            this.btnImgDel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnImgDel.Click += new System.EventHandler(this.btnImgDel_Click);

            // ==========================================
            // العمود الأيمن
            // ==========================================
            this.lblItemType.AutoSize = true;
            this.lblItemType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItemType.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblItemType.Location = new System.Drawing.Point(1170, 35);
            this.lblItemType.Name = "lblItemType";
            this.lblItemType.Size = new System.Drawing.Size(75, 15);
            this.lblItemType.Text = LanguageManager.Get("lbl_item_type");

            this.cmbItemType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbItemType.FormattingEnabled = true;
            this.cmbItemType.Items.AddRange(new object[] {
                LanguageManager.Get("item_type_goods"),
                LanguageManager.Get("item_type_service"),
                LanguageManager.Get("item_type_asset")
            });
            this.cmbItemType.Location = new System.Drawing.Point(1010, 32);
            this.cmbItemType.Name = "cmbItemType";
            this.cmbItemType.Size = new System.Drawing.Size(160, 23);
            this.cmbItemType.TabIndex = 0;

            this.lblGroupNo.AutoSize = true;
            this.lblGroupNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGroupNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblGroupNo.Location = new System.Drawing.Point(1170, 65);
            this.lblGroupNo.Name = "lblGroupNo";
            this.lblGroupNo.Size = new System.Drawing.Size(95, 15);
            this.lblGroupNo.Text = LanguageManager.Get("lbl_group_no");

            this.txtGroupNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtGroupNo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(190)))));
            this.txtGroupNo.Location = new System.Drawing.Point(1010, 62);
            this.txtGroupNo.Name = "txtGroupNo";
            this.txtGroupNo.Size = new System.Drawing.Size(160, 23);
            this.txtGroupNo.TabIndex = 1;
            this.txtGroupNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblGroupName.AutoSize = true;
            this.lblGroupName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblGroupName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblGroupName.Location = new System.Drawing.Point(1170, 95);
            this.lblGroupName.Name = "lblGroupName";
            this.lblGroupName.Size = new System.Drawing.Size(95, 15);
            this.lblGroupName.Text = LanguageManager.Get("lbl_group_name");

            this.txtGroupName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtGroupName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(190)))));
            this.txtGroupName.Location = new System.Drawing.Point(1010, 92);
            this.txtGroupName.Name = "txtGroupName";
            this.txtGroupName.Size = new System.Drawing.Size(160, 23);
            this.txtGroupName.TabIndex = 2;
            this.txtGroupName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblSubGroupUnder.AutoSize = true;
            this.lblSubGroupUnder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSubGroupUnder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblSubGroupUnder.Location = new System.Drawing.Point(1175, 125);
            this.lblSubGroupUnder.Name = "lblSubGroupUnder";
            this.lblSubGroupUnder.Size = new System.Drawing.Size(110, 15);
            this.lblSubGroupUnder.Text = LanguageManager.Get("lbl_sub_group_under");

            this.txtSubGroupUnder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSubGroupUnder.Location = new System.Drawing.Point(1010, 122);
            this.txtSubGroupUnder.Name = "txtSubGroupUnder";
            this.txtSubGroupUnder.Size = new System.Drawing.Size(160, 23);
            this.txtSubGroupUnder.TabIndex = 3;
            this.txtSubGroupUnder.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblItemCode.AutoSize = true;
            this.lblItemCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItemCode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblItemCode.Location = new System.Drawing.Point(1170, 155);
            this.lblItemCode.Name = "lblItemCode";
            this.lblItemCode.Size = new System.Drawing.Size(85, 15);
            this.lblItemCode.Text = LanguageManager.Get("lbl_item_code");

            this.txtItemCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtItemCode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(190)))));
            this.txtItemCode.Location = new System.Drawing.Point(1010, 152);
            this.txtItemCode.Name = "txtItemCode";
            this.txtItemCode.Size = new System.Drawing.Size(160, 23);
            this.txtItemCode.TabIndex = 4;
            this.txtItemCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblItemName.AutoSize = true;
            this.lblItemName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItemName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblItemName.Location = new System.Drawing.Point(1170, 185);
            this.lblItemName.Name = "lblItemName";
            this.lblItemName.Size = new System.Drawing.Size(85, 15);
            this.lblItemName.Text = LanguageManager.Get("lbl_item_name");

            this.txtItemName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtItemName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(190)))));
            this.txtItemName.Location = new System.Drawing.Point(1010, 182);
            this.txtItemName.Name = "txtItemName";
            this.txtItemName.Size = new System.Drawing.Size(160, 23);
            this.txtItemName.TabIndex = 5;
            this.txtItemName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // العمود الأوسط
            this.lblBranch.AutoSize = true;
            this.lblBranch.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblBranch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblBranch.Location = new System.Drawing.Point(890, 100);
            this.lblBranch.Name = "lblBranch";
            this.lblBranch.Size = new System.Drawing.Size(75, 15);
            this.lblBranch.Text = LanguageManager.Get("lbl_branch");

            this.txtBranch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBranch.Location = new System.Drawing.Point(700, 100);
            this.txtBranch.Name = "txtBranch";
            this.txtBranch.Size = new System.Drawing.Size(185, 23);
            this.txtBranch.TabIndex = 6;
            this.txtBranch.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblRefNo.AutoSize = true;
            this.lblRefNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRefNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblRefNo.Location = new System.Drawing.Point(890, 125);
            this.lblRefNo.Name = "lblRefNo";
            this.lblRefNo.Size = new System.Drawing.Size(85, 15);
            this.lblRefNo.Text = LanguageManager.Get("lbl_ref_no");

            this.txtRefNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRefNo.Location = new System.Drawing.Point(700, 125);
            this.txtRefNo.Name = "txtRefNo";
            this.txtRefNo.Size = new System.Drawing.Size(185, 23);
            this.txtRefNo.TabIndex = 7;
            this.txtRefNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ✅ حقل بلد المنشأ
            this.lblCountryOfOrigin.AutoSize = true;
            this.lblCountryOfOrigin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCountryOfOrigin.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblCountryOfOrigin.Location = new System.Drawing.Point(890, 155);
            this.lblCountryOfOrigin.Name = "lblCountryOfOrigin";
            this.lblCountryOfOrigin.Size = new System.Drawing.Size(95, 15);
            this.lblCountryOfOrigin.Text = "بلد المنشأ:";

            this.txtCountryOfOrigin.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCountryOfOrigin.Location = new System.Drawing.Point(700, 155);
            this.txtCountryOfOrigin.Name = "txtCountryOfOrigin";
            this.txtCountryOfOrigin.Size = new System.Drawing.Size(185, 23);
            this.txtCountryOfOrigin.TabIndex = 10;
            this.txtCountryOfOrigin.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ✅ ComboBox مربوط بتصاريح زراعية
            this.lblAgriculturePermit.AutoSize = true;
            this.lblAgriculturePermit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAgriculturePermit.ForeColor = System.Drawing.Color.Green;
            this.lblAgriculturePermit.Location = new System.Drawing.Point(500, 155);
            this.lblAgriculturePermit.Name = "lblAgriculturePermit";
            this.lblAgriculturePermit.Size = new System.Drawing.Size(180, 15);
            this.lblAgriculturePermit.Text = "مربوط بتصاريح زراعية:";
            this.lblAgriculturePermit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cmbAgriculturePermit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAgriculturePermit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbAgriculturePermit.FormattingEnabled = true;
            this.cmbAgriculturePermit.Location = new System.Drawing.Point(300, 152);
            this.cmbAgriculturePermit.Name = "cmbAgriculturePermit";
            this.cmbAgriculturePermit.Size = new System.Drawing.Size(190, 23);
            this.cmbAgriculturePermit.TabIndex = 11;

            // ✅ ComboBox مربوط بإفراج مدفوعات
            this.lblPaymentRelease.AutoSize = true;
            this.lblPaymentRelease.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPaymentRelease.ForeColor = System.Drawing.Color.Blue;
            this.lblPaymentRelease.Location = new System.Drawing.Point(500, 185);
            this.lblPaymentRelease.Name = "lblPaymentRelease";
            this.lblPaymentRelease.Size = new System.Drawing.Size(180, 15);
            this.lblPaymentRelease.Text = "مربوط بإفراج مدفوعات:";
            this.lblPaymentRelease.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cmbPaymentRelease.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentRelease.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbPaymentRelease.FormattingEnabled = true;
            this.cmbPaymentRelease.Location = new System.Drawing.Point(300, 182);
            this.cmbPaymentRelease.Name = "cmbPaymentRelease";
            this.cmbPaymentRelease.Size = new System.Drawing.Size(190, 23);
            this.cmbPaymentRelease.TabIndex = 12;

            // العمود الأيسر
            this.lblBarcode.AutoSize = true;
            this.lblBarcode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblBarcode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblBarcode.Location = new System.Drawing.Point(890, 35);
            this.lblBarcode.Name = "lblBarcode";
            this.lblBarcode.Size = new System.Drawing.Size(95, 15);
            this.lblBarcode.Text = LanguageManager.Get("lbl_barcode");

            this.txtBarcode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBarcode.Location = new System.Drawing.Point(700, 35);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(185, 23);
            this.txtBarcode.TabIndex = 8;
            this.txtBarcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblForeignName.AutoSize = true;
            this.lblForeignName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblForeignName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblForeignName.Location = new System.Drawing.Point(890, 65);
            this.lblForeignName.Name = "lblForeignName";
            this.lblForeignName.Size = new System.Drawing.Size(95, 15);
            this.lblForeignName.Text = LanguageManager.Get("lbl_foreign_name");

            this.txtForeignName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtForeignName.Location = new System.Drawing.Point(700, 62);
            this.txtForeignName.Name = "txtForeignName";
            this.txtForeignName.Size = new System.Drawing.Size(185, 23);
            this.txtForeignName.TabIndex = 9;
            this.txtForeignName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ==========================================
            // التبويبات
            // ==========================================
            this.tabControl1.Controls.Add(this.tabPageMain);
            this.tabControl1.Controls.Add(this.tabPageUnits);
            this.tabControl1.Controls.Add(this.tabPageOther);
            this.tabControl1.Controls.Add(this.tabPageCost);
            this.tabControl1.Controls.Add(this.tabPageMovement);
            this.tabControl1.Controls.Add(this.tabPageAnalysis);
            this.tabControl1.Controls.Add(this.tabPagePrices);

            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.tabControl1.Location = new System.Drawing.Point(0, 250);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.tabControl1.RightToLeftLayout = true;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1366, 480);
            this.tabControl1.TabIndex = 2;

            // ==========================================
            // التبويب الرئيسي
            // ==========================================
            this.tabPageMain.Controls.Add(this.lblItemSpecs);
            this.tabPageMain.Controls.Add(this.txtItemSpecs);
            this.tabPageMain.Controls.Add(this.lblItemSpecsForeign);
            this.tabPageMain.Controls.Add(this.txtItemSpecsForeign);
            this.tabPageMain.Controls.Add(this.lblAssistant);
            this.tabPageMain.Controls.Add(this.txtAssistant);
            this.tabPageMain.Controls.Add(this.lblDetailGroup);
            this.tabPageMain.Controls.Add(this.txtDetailGroup);
            this.tabPageMain.Controls.Add(this.lblClass);
            this.tabPageMain.Controls.Add(this.txtClass);
            this.tabPageMain.Controls.Add(this.lblInitialCost);
            this.tabPageMain.Controls.Add(this.txtInitialCost);
            this.tabPageMain.Controls.Add(this.lblDefaultUnit);
            this.tabPageMain.Controls.Add(this.cmbDefaultUnit);
            this.tabPageMain.Controls.Add(this.lblPurchaseInvoice);
            this.tabPageMain.Controls.Add(this.txtPurchaseInvoice);
            this.tabPageMain.Controls.Add(this.lblItemActivity);
            this.tabPageMain.Controls.Add(this.cmbItemActivity);
            this.tabPageMain.Controls.Add(this.lblReturnPeriod);
            this.tabPageMain.Controls.Add(this.txtReturnPeriod);
            this.tabPageMain.Controls.Add(this.lblRank);
            this.tabPageMain.Controls.Add(this.cmbRank);
            this.tabPageMain.Controls.Add(this.lblLength);
            this.tabPageMain.Controls.Add(this.txtLength);
            this.tabPageMain.Controls.Add(this.lblSize);
            this.tabPageMain.Controls.Add(this.txtSize);
            this.tabPageMain.Controls.Add(this.lblWidth);
            this.tabPageMain.Controls.Add(this.txtWidth);
            this.tabPageMain.Controls.Add(this.lblColor);
            this.tabPageMain.Controls.Add(this.txtColor);
            this.tabPageMain.Controls.Add(this.lblHeight);
            this.tabPageMain.Controls.Add(this.txtHeight);
            this.tabPageMain.Controls.Add(this.lblSeason);
            this.tabPageMain.Controls.Add(this.cmbSeason);
            this.tabPageMain.Controls.Add(this.lblVolume);
            this.tabPageMain.Controls.Add(this.txtVolume);
            this.tabPageMain.Controls.Add(this.lblMaterial);
            this.tabPageMain.Controls.Add(this.txtMaterial);
            this.tabPageMain.Controls.Add(this.lblArea);
            this.tabPageMain.Controls.Add(this.txtArea);
            this.tabPageMain.Controls.Add(this.lblBrand);
            this.tabPageMain.Controls.Add(this.txtBrand);
            this.tabPageMain.Controls.Add(this.lblManufacturer);
            this.tabPageMain.Controls.Add(this.txtManufacturer);
            this.tabPageMain.Controls.Add(this.lblMfgNo);
            this.tabPageMain.Controls.Add(this.txtMfgNo);
            this.tabPageMain.Controls.Add(this.lblCountryMain);
            this.tabPageMain.Controls.Add(this.txtCountryMain);
            this.tabPageMain.Controls.Add(this.lblStorage);
            this.tabPageMain.Controls.Add(this.txtStorage);
            this.tabPageMain.Controls.Add(this.lblOpenCost);
            this.tabPageMain.Controls.Add(this.txtOpenCost);
            this.tabPageMain.Controls.Add(this.lblStopDate);
            this.tabPageMain.Controls.Add(this.txtStopDate);
            this.tabPageMain.Controls.Add(this.lblOpeningStock);
            this.tabPageMain.Controls.Add(this.txtOpeningStock);
            this.tabPageMain.Controls.Add(this.lblStopReason);
            this.tabPageMain.Controls.Add(this.txtStopReason);
            this.tabPageMain.Controls.Add(this.lblAvailableQty);
            this.tabPageMain.Controls.Add(this.txtAvailableQty);

            this.tabPageMain.Location = new System.Drawing.Point(4, 26);
            this.tabPageMain.Name = "tabPageMain";
            this.tabPageMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMain.Size = new System.Drawing.Size(1358, 450);
            this.tabPageMain.TabIndex = 0;
            this.tabPageMain.Text = LanguageManager.Get("tab_main");
            this.tabPageMain.UseVisualStyleBackColor = true;

            // ==========================================
            // تبويب وحدات الصنف (تم دمج وحدات الوزن هنا)
            // ==========================================
            this.tabPageUnits.Controls.Add(this.dgvUnits);
            this.tabPageUnits.Controls.Add(this.dgvWeightUnits);
            this.tabPageUnits.Controls.Add(this.lblMainUnit);
            this.tabPageUnits.Controls.Add(this.chkMainUnit);
            this.tabPageUnits.Controls.Add(this.lblSalesUnit);
            this.tabPageUnits.Controls.Add(this.chkSalesUnit);
            this.tabPageUnits.Controls.Add(this.lblPurchaseUnit);
            this.tabPageUnits.Controls.Add(this.chkPurchaseUnit);
            this.tabPageUnits.Controls.Add(this.lblInventoryUnit);
            this.tabPageUnits.Controls.Add(this.chkInventoryUnit);
            this.tabPageUnits.Controls.Add(this.lblNotForSale);
            this.tabPageUnits.Controls.Add(this.chkNotForSale);
            this.tabPageUnits.Controls.Add(this.lblStop);
            this.tabPageUnits.Controls.Add(this.chkStop);

            this.tabPageUnits.Location = new System.Drawing.Point(4, 26);
            this.tabPageUnits.Name = "tabPageUnits";
            this.tabPageUnits.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageUnits.Size = new System.Drawing.Size(1358, 450);
            this.tabPageUnits.TabIndex = 1;
            this.tabPageUnits.Text = LanguageManager.Get("tab_units");
            this.tabPageUnits.UseVisualStyleBackColor = true;

            // ✅ جدول وحدات القياس
            this.dgvUnits.AllowUserToAddRows = true;
            this.dgvUnits.AllowUserToDeleteRows = true;
            this.dgvUnits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvUnits.BackgroundColor = System.Drawing.Color.White;
            this.dgvUnits.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvUnits.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvUnits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvUnits.EnableHeadersVisualStyles = false;
            this.dgvUnits.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvUnits.Location = new System.Drawing.Point(800, 20);
            this.dgvUnits.Name = "dgvUnits";
            this.dgvUnits.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvUnits.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvUnits.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvUnits.Size = new System.Drawing.Size(400, 190);
            this.dgvUnits.TabIndex = 0;

            this.dgvUnits.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(240)))), ((int)(((byte)(250)))));
            this.dgvUnits.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgvUnits.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvUnits.ColumnHeadersHeight = 30;
            this.dgvUnits.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.dgvUnits.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(200)))));
            this.dgvUnits.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.dgvUnits.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            System.Windows.Forms.DataGridViewTextBoxColumn colNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colNum.Name = "colNum";
            colNum.HeaderText = LanguageManager.Get("col_unit_num");
            colNum.Width = 40;
            colNum.ReadOnly = true;
            colNum.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));

            System.Windows.Forms.DataGridViewTextBoxColumn colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colUnit.Name = "colUnit";
            colUnit.HeaderText = LanguageManager.Get("col_unit_name");
            colUnit.Width = 100;

            System.Windows.Forms.DataGridViewTextBoxColumn colPackage = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colPackage.Name = "colPackage";
            colPackage.HeaderText = LanguageManager.Get("col_unit_package");
            colPackage.Width = 80;

            this.dgvUnits.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colNum, colUnit, colPackage });

            // ✅ جدول وحدات الوزن
            this.dgvWeightUnits.AllowUserToAddRows = true;
            this.dgvWeightUnits.AllowUserToDeleteRows = true;
            this.dgvWeightUnits.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvWeightUnits.BackgroundColor = System.Drawing.Color.White;
            this.dgvWeightUnits.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvWeightUnits.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvWeightUnits.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvWeightUnits.EnableHeadersVisualStyles = false;
            this.dgvWeightUnits.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvWeightUnits.Location = new System.Drawing.Point(210, 20);
            this.dgvWeightUnits.Name = "dgvWeightUnits";
            this.dgvWeightUnits.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvWeightUnits.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvWeightUnits.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvWeightUnits.Size = new System.Drawing.Size(400, 190);
            this.dgvWeightUnits.TabIndex = 1;

            this.dgvWeightUnits.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.dgvWeightUnits.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgvWeightUnits.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvWeightUnits.ColumnHeadersHeight = 35;
            this.dgvWeightUnits.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.dgvWeightUnits.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.dgvWeightUnits.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.dgvWeightUnits.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            // ✅ إضافة عمود الترقيم لجدول وحدات الوزن
            System.Windows.Forms.DataGridViewTextBoxColumn colWeightNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            colWeightNum.Name = "colWeightNum";
            colWeightNum.HeaderText = "م";
            colWeightNum.Width = 40;
            colWeightNum.ReadOnly = true;
            colWeightNum.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));

            this.colWeightUnit.Name = "colWeightUnit";
            this.colWeightUnit.HeaderText = LanguageManager.Get("col_weight_unit");
            this.colWeightUnit.Width = 400;

            this.colWeightValue.Name = "colWeightValue";
            this.colWeightValue.HeaderText = LanguageManager.Get("col_weight_value");
            this.colWeightValue.Width = 400;
            this.colWeightValue.DefaultCellStyle.Format = "N2";
            this.colWeightValue.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;

            this.dgvWeightUnits.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { colWeightNum, this.colWeightUnit, this.colWeightValue });

            // ==========================================
            // تبويب التكلفة
            // ==========================================
            this.tabPageCost.Controls.Add(this.dgvCost);
            this.tabPageCost.Location = new System.Drawing.Point(4, 26);
            this.tabPageCost.Name = "tabPageCost";
            this.tabPageCost.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageCost.Size = new System.Drawing.Size(1358, 450);
            this.tabPageCost.TabIndex = 3;
            this.tabPageCost.Text = LanguageManager.Get("tab_cost");
            this.tabPageCost.UseVisualStyleBackColor = true;
            this.tabPageCost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));

            this.dgvCost.AllowUserToAddRows = true;
            this.dgvCost.AllowUserToDeleteRows = true;
            this.dgvCost.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvCost.BackgroundColor = System.Drawing.Color.White;
            this.dgvCost.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvCost.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgvCost.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCost.EnableHeadersVisualStyles = false;
            this.dgvCost.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dgvCost.Location = new System.Drawing.Point(50, 20);
            this.dgvCost.Name = "dgvCost";
            this.dgvCost.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvCost.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvCost.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCost.Size = new System.Drawing.Size(1200, 400);
            this.dgvCost.TabIndex = 0;

            this.dgvCost.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(245)))), ((int)(((byte)(250)))));
            this.dgvCost.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            this.dgvCost.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.dgvCost.ColumnHeadersHeight = 35;
            this.dgvCost.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));
            this.dgvCost.DefaultCellStyle.BackColor = System.Drawing.Color.White;
            this.dgvCost.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(230)))), ((int)(((byte)(255)))));
            this.dgvCost.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.Black;

            this.colWarehouse.Name = "colWarehouse";
            this.colWarehouse.HeaderText = LanguageManager.Get("col_cost_warehouse");
            this.colWarehouse.Width = 150;

            this.colWarehouseName.Name = "colWarehouseName";
            this.colWarehouseName.HeaderText = LanguageManager.Get("col_cost_warehouse_name");
            this.colWarehouseName.Width = 250;

            this.colInitialCost.Name = "colInitialCost";
            this.colInitialCost.HeaderText = LanguageManager.Get("col_cost_initial");
            this.colInitialCost.Width = 200;

            this.colAvgCost.Name = "colAvgCost";
            this.colAvgCost.HeaderText = LanguageManager.Get("col_cost_avg");
            this.colAvgCost.Width = 200;

            this.colAvailableQty.Name = "colAvailableQty";
            this.colAvailableQty.HeaderText = LanguageManager.Get("col_cost_available");
            this.colAvailableQty.Width = 200;

            this.colStatus.Name = "colStatus";
            this.colStatus.HeaderText = LanguageManager.Get("col_cost_status");
            this.colStatus.Width = 80;
            this.colStatus.FalseValue = "false";
            this.colStatus.TrueValue = "true";

            this.dgvCost.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
                this.colWarehouse, this.colWarehouseName, this.colInitialCost, this.colAvgCost, this.colAvailableQty, this.colStatus });

            // ==========================================
            // تبويب بيانات أخرى
            // ==========================================
            this.tabPageOther.Controls.Add(this.chkAllowFreeQty);
            this.tabPageOther.Controls.Add(this.chkHasWarranty);
            this.tabPageOther.Controls.Add(this.lblWarrantyPeriod);
            this.tabPageOther.Controls.Add(this.txtWarrantyPeriod);
            this.tabPageOther.Controls.Add(this.lblWarrantyDay);
            this.tabPageOther.Controls.Add(this.chkHasCommission);
            this.tabPageOther.Controls.Add(this.lblCommissionType);
            this.tabPageOther.Controls.Add(this.cmbCommissionType);
            this.tabPageOther.Controls.Add(this.txtCommissionType);
            this.tabPageOther.Controls.Add(this.lblFreeQtyPercent);
            this.tabPageOther.Controls.Add(this.txtFreeQtyPercent);
            this.tabPageOther.Controls.Add(this.lblExpiryDuration);
            this.tabPageOther.Controls.Add(this.txtExpiryDuration);
            this.tabPageOther.Controls.Add(this.lblExpiryDay);
            this.tabPageOther.Controls.Add(this.lblMinCostPercent);
            this.tabPageOther.Controls.Add(this.txtMinCostPercent);
            this.tabPageOther.Controls.Add(this.lblMinCostSign);
            this.tabPageOther.Controls.Add(this.lblMaxCostPercent);
            this.tabPageOther.Controls.Add(this.txtMaxCostPercent);
            this.tabPageOther.Controls.Add(this.lblMaxCostSign);
            this.tabPageOther.Controls.Add(this.pnlSeparator);
            this.tabPageOther.Controls.Add(this.chkIsWeighted);
            this.tabPageOther.Controls.Add(this.lblScaleName);
            this.tabPageOther.Controls.Add(this.cmbScaleName);
            this.tabPageOther.Controls.Add(this.txtScaleName);
            this.tabPageOther.Controls.Add(this.lblScaleBtn);
            this.tabPageOther.Controls.Add(this.lblKey);
            this.tabPageOther.Controls.Add(this.txtKey);
            this.tabPageOther.Controls.Add(this.lblDocType);
            this.tabPageOther.Controls.Add(this.txtDocType);
            this.tabPageOther.Controls.Add(this.lblDocRefNo);
            this.tabPageOther.Controls.Add(this.txtDocRefNo);
            this.tabPageOther.Controls.Add(this.lblDocSeq);
            this.tabPageOther.Controls.Add(this.txtDocSeq);

            this.tabPageOther.Location = new System.Drawing.Point(4, 26);
            this.tabPageOther.Name = "tabPageOther";
            this.tabPageOther.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOther.Size = new System.Drawing.Size(1358, 450);
            this.tabPageOther.TabIndex = 2;
            this.tabPageOther.Text = LanguageManager.Get("tab_other");
            this.tabPageOther.UseVisualStyleBackColor = true;
            this.tabPageOther.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(245)))), ((int)(((byte)(245)))));

            this.chkAllowFreeQty.AutoSize = true;
            this.chkAllowFreeQty.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAllowFreeQty.Location = new System.Drawing.Point(800, 30);
            this.chkAllowFreeQty.Name = "chkAllowFreeQty";
            this.chkAllowFreeQty.Size = new System.Drawing.Size(130, 19);
            this.chkAllowFreeQty.TabIndex = 0;
            this.chkAllowFreeQty.Text = LanguageManager.Get("chk_allow_free_qty");
            this.chkAllowFreeQty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkAllowFreeQty.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkAllowFreeQty.UseVisualStyleBackColor = true;

            this.chkHasWarranty.AutoSize = true;
            this.chkHasWarranty.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkHasWarranty.Location = new System.Drawing.Point(800, 60);
            this.chkHasWarranty.Name = "chkHasWarranty";
            this.chkHasWarranty.Size = new System.Drawing.Size(90, 19);
            this.chkHasWarranty.TabIndex = 1;
            this.chkHasWarranty.Text = LanguageManager.Get("chk_has_warranty");
            this.chkHasWarranty.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHasWarranty.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkHasWarranty.UseVisualStyleBackColor = true;

            this.lblWarrantyPeriod.AutoSize = true;
            this.lblWarrantyPeriod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblWarrantyPeriod.Location = new System.Drawing.Point(560, 58);
            this.lblWarrantyPeriod.Name = "lblWarrantyPeriod";
            this.lblWarrantyPeriod.Size = new System.Drawing.Size(80, 15);
            this.lblWarrantyPeriod.TabIndex = 2;
            this.lblWarrantyPeriod.Text = LanguageManager.Get("lbl_warranty_period");
            this.lblWarrantyPeriod.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtWarrantyPeriod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtWarrantyPeriod.Location = new System.Drawing.Point(420, 55);
            this.txtWarrantyPeriod.Name = "txtWarrantyPeriod";
            this.txtWarrantyPeriod.Size = new System.Drawing.Size(130, 23);
            this.txtWarrantyPeriod.TabIndex = 3;
            this.txtWarrantyPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblWarrantyDay.AutoSize = true;
            this.lblWarrantyDay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblWarrantyDay.ForeColor = System.Drawing.Color.Blue;
            this.lblWarrantyDay.Location = new System.Drawing.Point(390, 58);
            this.lblWarrantyDay.Name = "lblWarrantyDay";
            this.lblWarrantyDay.Size = new System.Drawing.Size(30, 15);
            this.lblWarrantyDay.TabIndex = 4;
            this.lblWarrantyDay.Text = LanguageManager.Get("lbl_warranty_day");
            this.lblWarrantyDay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.chkHasCommission.AutoSize = true;
            this.chkHasCommission.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkHasCommission.Location = new System.Drawing.Point(800, 90);
            this.chkHasCommission.Name = "chkHasCommission";
            this.chkHasCommission.Size = new System.Drawing.Size(85, 19);
            this.chkHasCommission.TabIndex = 5;
            this.chkHasCommission.Text = LanguageManager.Get("chk_has_commission");
            this.chkHasCommission.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkHasCommission.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkHasCommission.UseVisualStyleBackColor = true;

            this.lblCommissionType.AutoSize = true;
            this.lblCommissionType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCommissionType.Location = new System.Drawing.Point(560, 88);
            this.lblCommissionType.Name = "lblCommissionType";
            this.lblCommissionType.Size = new System.Drawing.Size(85, 15);
            this.lblCommissionType.TabIndex = 6;
            this.lblCommissionType.Text = LanguageManager.Get("lbl_commission_type");
            this.lblCommissionType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cmbCommissionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCommissionType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbCommissionType.FormattingEnabled = true;
            this.cmbCommissionType.Location = new System.Drawing.Point(420, 85);
            this.cmbCommissionType.Name = "cmbCommissionType";
            this.cmbCommissionType.Size = new System.Drawing.Size(130, 23);
            this.cmbCommissionType.TabIndex = 7;

            this.txtCommissionType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCommissionType.Location = new System.Drawing.Point(420, 110);
            this.txtCommissionType.Name = "txtCommissionType";
            this.txtCommissionType.Size = new System.Drawing.Size(130, 23);
            this.txtCommissionType.TabIndex = 8;
            this.txtCommissionType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblFreeQtyPercent.AutoSize = true;
            this.lblFreeQtyPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblFreeQtyPercent.Location = new System.Drawing.Point(260, 28);
            this.lblFreeQtyPercent.Name = "lblFreeQtyPercent";
            this.lblFreeQtyPercent.Size = new System.Drawing.Size(120, 15);
            this.lblFreeQtyPercent.TabIndex = 9;
            this.lblFreeQtyPercent.Text = LanguageManager.Get("lbl_free_qty_percent");
            this.lblFreeQtyPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtFreeQtyPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtFreeQtyPercent.Location = new System.Drawing.Point(120, 25);
            this.txtFreeQtyPercent.Name = "txtFreeQtyPercent";
            this.txtFreeQtyPercent.Size = new System.Drawing.Size(130, 23);
            this.txtFreeQtyPercent.TabIndex = 10;
            this.txtFreeQtyPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblExpiryDuration.AutoSize = true;
            this.lblExpiryDuration.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblExpiryDuration.Location = new System.Drawing.Point(260, 58);
            this.lblExpiryDuration.Name = "lblExpiryDuration";
            this.lblExpiryDuration.Size = new System.Drawing.Size(120, 15);
            this.lblExpiryDuration.TabIndex = 11;
            this.lblExpiryDuration.Text = LanguageManager.Get("lbl_expiry_duration");
            this.lblExpiryDuration.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtExpiryDuration.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtExpiryDuration.Location = new System.Drawing.Point(120, 55);
            this.txtExpiryDuration.Name = "txtExpiryDuration";
            this.txtExpiryDuration.Size = new System.Drawing.Size(130, 23);
            this.txtExpiryDuration.TabIndex = 12;
            this.txtExpiryDuration.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblExpiryDay.AutoSize = true;
            this.lblExpiryDay.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblExpiryDay.ForeColor = System.Drawing.Color.DarkRed;
            this.lblExpiryDay.Location = new System.Drawing.Point(90, 58);
            this.lblExpiryDay.Name = "lblExpiryDay";
            this.lblExpiryDay.Size = new System.Drawing.Size(30, 15);
            this.lblExpiryDay.TabIndex = 13;
            this.lblExpiryDay.Text = LanguageManager.Get("lbl_expiry_day");
            this.lblExpiryDay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.lblMinCostPercent.AutoSize = true;
            this.lblMinCostPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMinCostPercent.Location = new System.Drawing.Point(260, 88);
            this.lblMinCostPercent.Name = "lblMinCostPercent";
            this.lblMinCostPercent.Size = new System.Drawing.Size(150, 15);
            this.lblMinCostPercent.TabIndex = 14;
            this.lblMinCostPercent.Text = LanguageManager.Get("lbl_min_cost_percent");
            this.lblMinCostPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtMinCostPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMinCostPercent.Location = new System.Drawing.Point(120, 85);
            this.txtMinCostPercent.Name = "txtMinCostPercent";
            this.txtMinCostPercent.Size = new System.Drawing.Size(130, 23);
            this.txtMinCostPercent.TabIndex = 15;
            this.txtMinCostPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblMinCostSign.AutoSize = true;
            this.lblMinCostSign.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMinCostSign.ForeColor = System.Drawing.Color.Red;
            this.lblMinCostSign.Location = new System.Drawing.Point(90, 88);
            this.lblMinCostSign.Name = "lblMinCostSign";
            this.lblMinCostSign.Size = new System.Drawing.Size(20, 15);
            this.lblMinCostSign.TabIndex = 16;
            this.lblMinCostSign.Text = "%";
            this.lblMinCostSign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.lblMaxCostPercent.AutoSize = true;
            this.lblMaxCostPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblMaxCostPercent.Location = new System.Drawing.Point(260, 118);
            this.lblMaxCostPercent.Name = "lblMaxCostPercent";
            this.lblMaxCostPercent.Size = new System.Drawing.Size(150, 15);
            this.lblMaxCostPercent.TabIndex = 17;
            this.lblMaxCostPercent.Text = LanguageManager.Get("lbl_max_cost_percent");
            this.lblMaxCostPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtMaxCostPercent.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMaxCostPercent.Location = new System.Drawing.Point(120, 115);
            this.txtMaxCostPercent.Name = "txtMaxCostPercent";
            this.txtMaxCostPercent.Size = new System.Drawing.Size(130, 23);
            this.txtMaxCostPercent.TabIndex = 18;
            this.txtMaxCostPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblMaxCostSign.AutoSize = true;
            this.lblMaxCostSign.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMaxCostSign.ForeColor = System.Drawing.Color.Red;
            this.lblMaxCostSign.Location = new System.Drawing.Point(90, 118);
            this.lblMaxCostSign.Name = "lblMaxCostSign";
            this.lblMaxCostSign.Size = new System.Drawing.Size(20, 15);
            this.lblMaxCostSign.TabIndex = 19;
            this.lblMaxCostSign.Text = "%";
            this.lblMaxCostSign.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.pnlSeparator.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.pnlSeparator.Location = new System.Drawing.Point(20, 155);
            this.pnlSeparator.Name = "pnlSeparator";
            this.pnlSeparator.Size = new System.Drawing.Size(1320, 2);
            this.pnlSeparator.TabIndex = 20;

            this.chkIsWeighted.AutoSize = true;
            this.chkIsWeighted.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkIsWeighted.Location = new System.Drawing.Point(800, 175);
            this.chkIsWeighted.Name = "chkIsWeighted";
            this.chkIsWeighted.Size = new System.Drawing.Size(90, 19);
            this.chkIsWeighted.TabIndex = 21;
            this.chkIsWeighted.Text = LanguageManager.Get("chk_is_weighted");
            this.chkIsWeighted.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkIsWeighted.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.chkIsWeighted.UseVisualStyleBackColor = true;

            this.lblScaleName.AutoSize = true;
            this.lblScaleName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblScaleName.Location = new System.Drawing.Point(560, 205);
            this.lblScaleName.Name = "lblScaleName";
            this.lblScaleName.Size = new System.Drawing.Size(75, 15);
            this.lblScaleName.TabIndex = 22;
            this.lblScaleName.Text = LanguageManager.Get("lbl_scale_name");
            this.lblScaleName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cmbScaleName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbScaleName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbScaleName.FormattingEnabled = true;
            this.cmbScaleName.Location = new System.Drawing.Point(420, 202);
            this.cmbScaleName.Name = "cmbScaleName";
            this.cmbScaleName.Size = new System.Drawing.Size(130, 23);
            this.cmbScaleName.TabIndex = 23;

            this.txtScaleName.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtScaleName.Location = new System.Drawing.Point(260, 202);
            this.txtScaleName.Name = "txtScaleName";
            this.txtScaleName.Size = new System.Drawing.Size(150, 23);
            this.txtScaleName.TabIndex = 24;
            this.txtScaleName.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblScaleBtn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblScaleBtn.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblScaleBtn.Location = new System.Drawing.Point(230, 200);
            this.lblScaleBtn.Name = "lblScaleBtn";
            this.lblScaleBtn.Size = new System.Drawing.Size(25, 27);
            this.lblScaleBtn.TabIndex = 25;
            this.lblScaleBtn.Text = "↓";
            this.lblScaleBtn.UseVisualStyleBackColor = true;

            this.lblKey.AutoSize = true;
            this.lblKey.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblKey.Location = new System.Drawing.Point(800, 205);
            this.lblKey.Name = "lblKey";
            this.lblKey.Size = new System.Drawing.Size(55, 15);
            this.lblKey.TabIndex = 26;
            this.lblKey.Text = LanguageManager.Get("lbl_key");
            this.lblKey.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtKey.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtKey.Location = new System.Drawing.Point(620, 202);
            this.txtKey.Name = "txtKey";
            this.txtKey.Size = new System.Drawing.Size(170, 23);
            this.txtKey.TabIndex = 27;
            this.txtKey.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblDocType.AutoSize = true;
            this.lblDocType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDocType.Location = new System.Drawing.Point(260, 240);
            this.lblDocType.Name = "lblDocType";
            this.lblDocType.Size = new System.Drawing.Size(100, 15);
            this.lblDocType.TabIndex = 28;
            this.lblDocType.Text = LanguageManager.Get("lbl_doc_type");
            this.lblDocType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtDocType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDocType.Location = new System.Drawing.Point(120, 237);
            this.txtDocType.Name = "txtDocType";
            this.txtDocType.Size = new System.Drawing.Size(130, 23);
            this.txtDocType.TabIndex = 29;
            this.txtDocType.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblDocRefNo.AutoSize = true;
            this.lblDocRefNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDocRefNo.Location = new System.Drawing.Point(260, 270);
            this.lblDocRefNo.Name = "lblDocRefNo";
            this.lblDocRefNo.Size = new System.Drawing.Size(115, 15);
            this.lblDocRefNo.TabIndex = 30;
            this.lblDocRefNo.Text = LanguageManager.Get("lbl_doc_ref_no");
            this.lblDocRefNo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtDocRefNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDocRefNo.Location = new System.Drawing.Point(120, 267);
            this.txtDocRefNo.Name = "txtDocRefNo";
            this.txtDocRefNo.Size = new System.Drawing.Size(130, 23);
            this.txtDocRefNo.TabIndex = 31;
            this.txtDocRefNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblDocSeq.AutoSize = true;
            this.lblDocSeq.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDocSeq.Location = new System.Drawing.Point(260, 300);
            this.lblDocSeq.Name = "lblDocSeq";
            this.lblDocSeq.Size = new System.Drawing.Size(90, 15);
            this.lblDocSeq.TabIndex = 32;
            this.lblDocSeq.Text = LanguageManager.Get("lbl_doc_seq");
            this.lblDocSeq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtDocSeq.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDocSeq.Location = new System.Drawing.Point(120, 297);
            this.txtDocSeq.Name = "txtDocSeq";
            this.txtDocSeq.Size = new System.Drawing.Size(130, 23);
            this.txtDocSeq.TabIndex = 33;
            this.txtDocSeq.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ==========================================
            // حقول التبويب الرئيسي (مواصفات الصنف)
            // ==========================================
            this.lblItemSpecs.AutoSize = true;
            this.lblItemSpecs.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItemSpecs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblItemSpecs.Location = new System.Drawing.Point(1150, 5);
            this.lblItemSpecs.Name = "lblItemSpecs";
            this.lblItemSpecs.Size = new System.Drawing.Size(120, 15);
            this.lblItemSpecs.Text = LanguageManager.Get("lbl_item_specs");

            this.txtItemSpecs.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtItemSpecs.Location = new System.Drawing.Point(900, 5);
            this.txtItemSpecs.Name = "txtItemSpecs";
            this.txtItemSpecs.Size = new System.Drawing.Size(240, 23);
            this.txtItemSpecs.TabIndex = 0;
            this.txtItemSpecs.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblItemSpecsForeign.AutoSize = true;
            this.lblItemSpecsForeign.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItemSpecsForeign.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblItemSpecsForeign.Location = new System.Drawing.Point(1150, 23);
            this.lblItemSpecsForeign.Name = "lblItemSpecsForeign";
            this.lblItemSpecsForeign.Size = new System.Drawing.Size(150, 15);
            this.lblItemSpecsForeign.Text = LanguageManager.Get("lbl_item_specs_foreign");

            this.txtItemSpecsForeign.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtItemSpecsForeign.Location = new System.Drawing.Point(900, 25);
            this.txtItemSpecsForeign.Name = "txtItemSpecsForeign";
            this.txtItemSpecsForeign.Size = new System.Drawing.Size(240, 23);
            this.txtItemSpecsForeign.TabIndex = 1;
            this.txtItemSpecsForeign.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblAssistant.AutoSize = true;
            this.lblAssistant.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAssistant.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblAssistant.Location = new System.Drawing.Point(1150, 45);
            this.lblAssistant.Name = "lblAssistant";
            this.lblAssistant.Size = new System.Drawing.Size(100, 15);
            this.lblAssistant.Text = LanguageManager.Get("lbl_assistant");

            this.txtAssistant.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAssistant.Location = new System.Drawing.Point(900, 45);
            this.txtAssistant.Name = "txtAssistant";
            this.txtAssistant.Size = new System.Drawing.Size(240, 23);
            this.txtAssistant.TabIndex = 2;
            this.txtAssistant.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblDetailGroup.AutoSize = true;
            this.lblDetailGroup.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDetailGroup.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblDetailGroup.Location = new System.Drawing.Point(1150, 65);
            this.lblDetailGroup.Name = "lblDetailGroup";
            this.lblDetailGroup.Size = new System.Drawing.Size(100, 15);
            this.lblDetailGroup.Text = LanguageManager.Get("lbl_detail_group");

            this.txtDetailGroup.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtDetailGroup.Location = new System.Drawing.Point(900, 65);
            this.txtDetailGroup.Name = "txtDetailGroup";
            this.txtDetailGroup.Size = new System.Drawing.Size(240, 23);
            this.txtDetailGroup.TabIndex = 3;
            this.txtDetailGroup.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblClass.AutoSize = true;
            this.lblClass.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblClass.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblClass.Location = new System.Drawing.Point(1150, 84);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(70, 15);
            this.lblClass.Text = LanguageManager.Get("lbl_class");

            this.txtClass.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtClass.Location = new System.Drawing.Point(900, 84);
            this.txtClass.Name = "txtClass";
            this.txtClass.Size = new System.Drawing.Size(240, 23);
            this.txtClass.TabIndex = 4;
            this.txtClass.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblInitialCost.AutoSize = true;
            this.lblInitialCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblInitialCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblInitialCost.Location = new System.Drawing.Point(1150, 107);
            this.lblInitialCost.Name = "lblInitialCost";
            this.lblInitialCost.Size = new System.Drawing.Size(90, 15);
            this.lblInitialCost.Text = LanguageManager.Get("lbl_initial_cost");

            this.txtInitialCost.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtInitialCost.Location = new System.Drawing.Point(1040, 107);
            this.txtInitialCost.Name = "txtInitialCost";
            this.txtInitialCost.Size = new System.Drawing.Size(100, 23);
            this.txtInitialCost.Text = "0";
            this.txtInitialCost.TabIndex = 5;
            this.txtInitialCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblDefaultUnit.AutoSize = true;
            this.lblDefaultUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDefaultUnit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblDefaultUnit.Location = new System.Drawing.Point(910, 107);
            this.lblDefaultUnit.Name = "lblDefaultUnit";
            this.lblDefaultUnit.Size = new System.Drawing.Size(110, 15);
            this.lblDefaultUnit.Text = LanguageManager.Get("lbl_default_unit");

            this.cmbDefaultUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDefaultUnit.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbDefaultUnit.FormattingEnabled = true;
            this.cmbDefaultUnit.Items.AddRange(new object[] {
                LanguageManager.Get("default_unit_piece"),
                LanguageManager.Get("default_unit_kg"),
                LanguageManager.Get("default_unit_liter"),
                LanguageManager.Get("default_unit_carton"),
                LanguageManager.Get("default_unit_can")
            });
            this.cmbDefaultUnit.Location = new System.Drawing.Point(740, 107);
            this.cmbDefaultUnit.Name = "cmbDefaultUnit";
            this.cmbDefaultUnit.Size = new System.Drawing.Size(160, 23);
            this.cmbDefaultUnit.TabIndex = 6;

            this.lblPurchaseInvoice.AutoSize = true;
            this.lblPurchaseInvoice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPurchaseInvoice.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblPurchaseInvoice.Location = new System.Drawing.Point(1150, 127);
            this.lblPurchaseInvoice.Name = "lblPurchaseInvoice";
            this.lblPurchaseInvoice.Size = new System.Drawing.Size(120, 15);
            this.lblPurchaseInvoice.Text = LanguageManager.Get("lbl_purchase_invoice");

            this.txtPurchaseInvoice.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPurchaseInvoice.Location = new System.Drawing.Point(1040, 127);
            this.txtPurchaseInvoice.Name = "txtPurchaseInvoice";
            this.txtPurchaseInvoice.Size = new System.Drawing.Size(100, 23);
            this.txtPurchaseInvoice.TabIndex = 7;
            this.txtPurchaseInvoice.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblItemActivity.AutoSize = true;
            this.lblItemActivity.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblItemActivity.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblItemActivity.Location = new System.Drawing.Point(910, 130);
            this.lblItemActivity.Name = "lblItemActivity";
            this.lblItemActivity.Size = new System.Drawing.Size(90, 15);
            this.lblItemActivity.Text = LanguageManager.Get("lbl_item_activity");

            this.cmbItemActivity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbItemActivity.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbItemActivity.FormattingEnabled = true;
            this.cmbItemActivity.Items.AddRange(new object[] {
                LanguageManager.Get("item_activity_active"),
                LanguageManager.Get("item_activity_inactive"),
                LanguageManager.Get("item_activity_temp"),
                LanguageManager.Get("item_activity_stopped")
            });
            this.cmbItemActivity.Location = new System.Drawing.Point(740, 132);
            this.cmbItemActivity.Name = "cmbItemActivity";
            this.cmbItemActivity.Size = new System.Drawing.Size(160, 23);
            this.cmbItemActivity.TabIndex = 8;

            this.lblReturnPeriod.AutoSize = true;
            this.lblReturnPeriod.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblReturnPeriod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblReturnPeriod.Location = new System.Drawing.Point(1150, 149);
            this.lblReturnPeriod.Name = "lblReturnPeriod";
            this.lblReturnPeriod.Size = new System.Drawing.Size(100, 15);
            this.lblReturnPeriod.Text = LanguageManager.Get("lbl_return_period");

            this.txtReturnPeriod.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtReturnPeriod.Location = new System.Drawing.Point(1040, 149);
            this.txtReturnPeriod.Name = "txtReturnPeriod";
            this.txtReturnPeriod.Size = new System.Drawing.Size(100, 23);
            this.txtReturnPeriod.TabIndex = 9;
            this.txtReturnPeriod.Text = "0";
            this.txtReturnPeriod.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblRank.AutoSize = true;
            this.lblRank.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRank.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblRank.Location = new System.Drawing.Point(910, 156);
            this.lblRank.Name = "lblRank";
            this.lblRank.Size = new System.Drawing.Size(50, 15);
            this.lblRank.Text = LanguageManager.Get("lbl_rank");

            this.cmbRank.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRank.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbRank.FormattingEnabled = true;
            this.cmbRank.Items.AddRange(new object[] {
                LanguageManager.Get("rank_a"),
                LanguageManager.Get("rank_b"),
                LanguageManager.Get("rank_c"),
                LanguageManager.Get("rank_d"),
                LanguageManager.Get("rank_e")
            });
            this.cmbRank.Location = new System.Drawing.Point(740, 156);
            this.cmbRank.Name = "cmbRank";
            this.cmbRank.Size = new System.Drawing.Size(160, 23);
            this.cmbRank.TabIndex = 10;

            this.lblLength.AutoSize = true;
            this.lblLength.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblLength.Location = new System.Drawing.Point(1150, 169);
            this.lblLength.Name = "lblLength";
            this.lblLength.Size = new System.Drawing.Size(50, 15);
            this.lblLength.Text = LanguageManager.Get("lbl_length");

            this.txtLength.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtLength.Location = new System.Drawing.Point(1060, 171);
            this.txtLength.Name = "txtLength";
            this.txtLength.Size = new System.Drawing.Size(80, 23);
            this.txtLength.TabIndex = 11;
            this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblSize.AutoSize = true;
            this.lblSize.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblSize.Location = new System.Drawing.Point(910, 174);
            this.lblSize.Name = "lblSize";
            this.lblSize.Size = new System.Drawing.Size(60, 15);
            this.lblSize.Text = LanguageManager.Get("lbl_size");

            this.txtSize.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSize.Location = new System.Drawing.Point(740, 174);
            this.txtSize.Name = "txtSize";
            this.txtSize.Size = new System.Drawing.Size(160, 23);
            this.txtSize.TabIndex = 12;
            this.txtSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblWidth.AutoSize = true;
            this.lblWidth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblWidth.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblWidth.Location = new System.Drawing.Point(1150, 193);
            this.lblWidth.Name = "lblWidth";
            this.lblWidth.Size = new System.Drawing.Size(60, 15);
            this.lblWidth.Text = LanguageManager.Get("lbl_width");

            this.txtWidth.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtWidth.Location = new System.Drawing.Point(1060, 193);
            this.txtWidth.Name = "txtWidth";
            this.txtWidth.Size = new System.Drawing.Size(80, 23);
            this.txtWidth.TabIndex = 13;
            this.txtWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblColor.AutoSize = true;
            this.lblColor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblColor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblColor.Location = new System.Drawing.Point(910, 196);
            this.lblColor.Name = "lblColor";
            this.lblColor.Size = new System.Drawing.Size(50, 15);
            this.lblColor.Text = LanguageManager.Get("lbl_color");

            this.txtColor.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtColor.Location = new System.Drawing.Point(740, 196);
            this.txtColor.Name = "txtColor";
            this.txtColor.Size = new System.Drawing.Size(160, 23);
            this.txtColor.TabIndex = 14;
            this.txtColor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblHeight.AutoSize = true;
            this.lblHeight.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblHeight.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblHeight.Location = new System.Drawing.Point(1150, 217);
            this.lblHeight.Name = "lblHeight";
            this.lblHeight.Size = new System.Drawing.Size(60, 15);
            this.lblHeight.Text = LanguageManager.Get("lbl_height");

            this.txtHeight.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtHeight.Location = new System.Drawing.Point(1060, 217);
            this.txtHeight.Name = "txtHeight";
            this.txtHeight.Size = new System.Drawing.Size(80, 23);
            this.txtHeight.TabIndex = 15;
            this.txtHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblSeason.AutoSize = true;
            this.lblSeason.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSeason.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblSeason.Location = new System.Drawing.Point(910, 217);
            this.lblSeason.Name = "lblSeason";
            this.lblSeason.Size = new System.Drawing.Size(60, 15);
            this.lblSeason.Text = LanguageManager.Get("lbl_season");

            this.cmbSeason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSeason.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbSeason.FormattingEnabled = true;
            this.cmbSeason.Items.AddRange(new object[] {
                LanguageManager.Get("season_summer"),
                LanguageManager.Get("season_winter"),
                LanguageManager.Get("season_spring"),
                LanguageManager.Get("season_autumn"),
                LanguageManager.Get("season_all"),
                LanguageManager.Get("season_none")
            });
            this.cmbSeason.Location = new System.Drawing.Point(740, 217);
            this.cmbSeason.Name = "cmbSeason";
            this.cmbSeason.Size = new System.Drawing.Size(160, 23);
            this.cmbSeason.TabIndex = 16;

            this.lblVolume.AutoSize = true;
            this.lblVolume.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblVolume.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblVolume.Location = new System.Drawing.Point(1150, 241);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(60, 15);
            this.lblVolume.Text = LanguageManager.Get("lbl_volume");

            this.txtVolume.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtVolume.Location = new System.Drawing.Point(1060, 241);
            this.txtVolume.Name = "txtVolume";
            this.txtVolume.Size = new System.Drawing.Size(80, 23);
            this.txtVolume.TabIndex = 17;
            this.txtVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblMaterial.AutoSize = true;
            this.lblMaterial.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMaterial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblMaterial.Location = new System.Drawing.Point(910, 241);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.Size = new System.Drawing.Size(60, 15);
            this.lblMaterial.Text = LanguageManager.Get("lbl_material");

            this.txtMaterial.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMaterial.Location = new System.Drawing.Point(740, 241);
            this.txtMaterial.Name = "txtMaterial";
            this.txtMaterial.Size = new System.Drawing.Size(160, 23);
            this.txtMaterial.TabIndex = 18;
            this.txtMaterial.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblArea.AutoSize = true;
            this.lblArea.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblArea.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblArea.Location = new System.Drawing.Point(1150, 265);
            this.lblArea.Name = "lblArea";
            this.lblArea.Size = new System.Drawing.Size(60, 15);
            this.lblArea.Text = LanguageManager.Get("lbl_area");

            this.txtArea.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtArea.Location = new System.Drawing.Point(1060, 265);
            this.txtArea.Name = "txtArea";
            this.txtArea.Size = new System.Drawing.Size(80, 23);
            this.txtArea.TabIndex = 19;
            this.txtArea.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblBrand.AutoSize = true;
            this.lblBrand.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblBrand.Location = new System.Drawing.Point(910, 265);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(60, 15);
            this.lblBrand.Text = LanguageManager.Get("lbl_brand");

            this.txtBrand.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtBrand.Location = new System.Drawing.Point(740, 265);
            this.txtBrand.Name = "txtBrand";
            this.txtBrand.Size = new System.Drawing.Size(160, 23);
            this.txtBrand.TabIndex = 20;
            this.txtBrand.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblManufacturer.AutoSize = true;
            this.lblManufacturer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblManufacturer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblManufacturer.Location = new System.Drawing.Point(910, 289);
            this.lblManufacturer.Name = "lblManufacturer";
            this.lblManufacturer.Size = new System.Drawing.Size(110, 15);
            this.lblManufacturer.Text = LanguageManager.Get("lbl_manufacturer");

            this.txtManufacturer.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtManufacturer.Location = new System.Drawing.Point(740, 289);
            this.txtManufacturer.Name = "txtManufacturer";
            this.txtManufacturer.Size = new System.Drawing.Size(160, 23);
            this.txtManufacturer.TabIndex = 22;
            this.txtManufacturer.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblMfgNo.AutoSize = true;
            this.lblMfgNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblMfgNo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblMfgNo.Location = new System.Drawing.Point(570, 5);
            this.lblMfgNo.Name = "lblMfgNo";
            this.lblMfgNo.Size = new System.Drawing.Size(100, 15);
            this.lblMfgNo.Text = LanguageManager.Get("lbl_mfg_no");

            this.txtMfgNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtMfgNo.Location = new System.Drawing.Point(420, 5);
            this.txtMfgNo.Name = "txtMfgNo";
            this.txtMfgNo.Size = new System.Drawing.Size(140, 23);
            this.txtMfgNo.TabIndex = 23;
            this.txtMfgNo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblCountryMain.AutoSize = true;
            this.lblCountryMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCountryMain.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblCountryMain.Location = new System.Drawing.Point(300, 5);
            this.lblCountryMain.Name = "lblCountryMain";
            this.lblCountryMain.Size = new System.Drawing.Size(80, 15);
            this.lblCountryMain.Text = LanguageManager.Get("lbl_country_main");

            this.txtCountryMain.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtCountryMain.Location = new System.Drawing.Point(150, 5);
            this.txtCountryMain.Name = "txtCountryMain";
            this.txtCountryMain.Size = new System.Drawing.Size(140, 23);
            this.txtCountryMain.TabIndex = 24;
            this.txtCountryMain.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblStorage.AutoSize = true;
            this.lblStorage.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStorage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblStorage.Location = new System.Drawing.Point(570, 40);
            this.lblStorage.Name = "lblStorage";
            this.lblStorage.Size = new System.Drawing.Size(110, 15);
            this.lblStorage.Text = LanguageManager.Get("lbl_storage");

            this.txtStorage.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtStorage.Location = new System.Drawing.Point(420, 40);
            this.txtStorage.Name = "txtStorage";
            this.txtStorage.Size = new System.Drawing.Size(140, 23);
            this.txtStorage.TabIndex = 25;
            this.txtStorage.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblOpenCost.AutoSize = true;
            this.lblOpenCost.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOpenCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblOpenCost.Location = new System.Drawing.Point(300, 40);
            this.lblOpenCost.Name = "lblOpenCost";
            this.lblOpenCost.Size = new System.Drawing.Size(140, 15);
            this.lblOpenCost.Text = LanguageManager.Get("lbl_open_cost");

            this.txtOpenCost.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtOpenCost.Location = new System.Drawing.Point(150, 40);
            this.txtOpenCost.Name = "txtOpenCost";
            this.txtOpenCost.Size = new System.Drawing.Size(140, 23);
            this.txtOpenCost.Text = "0";
            this.txtOpenCost.TabIndex = 26;
            this.txtOpenCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblStopDate.AutoSize = true;
            this.lblStopDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStopDate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblStopDate.Location = new System.Drawing.Point(570, 64);
            this.lblStopDate.Name = "lblStopDate";
            this.lblStopDate.Size = new System.Drawing.Size(100, 15);
            this.lblStopDate.Text = LanguageManager.Get("lbl_stop_date");

            this.txtStopDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtStopDate.Location = new System.Drawing.Point(420, 64);
            this.txtStopDate.Name = "txtStopDate";
            this.txtStopDate.Size = new System.Drawing.Size(140, 23);
            this.txtStopDate.TabIndex = 27;
            this.txtStopDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblOpeningStock.AutoSize = true;
            this.lblOpeningStock.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblOpeningStock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblOpeningStock.Location = new System.Drawing.Point(300, 64);
            this.lblOpeningStock.Name = "lblOpeningStock";
            this.lblOpeningStock.Size = new System.Drawing.Size(120, 15);
            this.lblOpeningStock.Text = LanguageManager.Get("lbl_opening_stock");

            this.txtOpeningStock.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtOpeningStock.Location = new System.Drawing.Point(150, 64);
            this.txtOpeningStock.Name = "txtOpeningStock";
            this.txtOpeningStock.Size = new System.Drawing.Size(140, 23);
            this.txtOpeningStock.Text = "0";
            this.txtOpeningStock.TabIndex = 28;
            this.txtOpeningStock.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblStopReason.AutoSize = true;
            this.lblStopReason.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStopReason.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblStopReason.Location = new System.Drawing.Point(570, 88);
            this.lblStopReason.Name = "lblStopReason";
            this.lblStopReason.Size = new System.Drawing.Size(100, 15);
            this.lblStopReason.Text = LanguageManager.Get("lbl_stop_reason");

            this.txtStopReason.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtStopReason.Location = new System.Drawing.Point(420, 88);
            this.txtStopReason.Name = "txtStopReason";
            this.txtStopReason.Size = new System.Drawing.Size(140, 23);
            this.txtStopReason.TabIndex = 29;
            this.txtStopReason.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblAvailableQty.AutoSize = true;
            this.lblAvailableQty.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblAvailableQty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(122)))), ((int)(((byte)(183)))));
            this.lblAvailableQty.Location = new System.Drawing.Point(300, 88);
            this.lblAvailableQty.Name = "lblAvailableQty";
            this.lblAvailableQty.Size = new System.Drawing.Size(120, 15);
            this.lblAvailableQty.Text = LanguageManager.Get("lbl_available_qty");

            this.txtAvailableQty.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtAvailableQty.Location = new System.Drawing.Point(150, 88);
            this.txtAvailableQty.Name = "txtAvailableQty";
            this.txtAvailableQty.Size = new System.Drawing.Size(140, 23);
            this.txtAvailableQty.Text = "0";
            this.txtAvailableQty.TabIndex = 30;
            this.txtAvailableQty.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ==========================================
            // باقي التبويبات
            // ==========================================
            this.tabPageMovement.Location = new System.Drawing.Point(4, 26);
            this.tabPageMovement.Name = "tabPageMovement";
            this.tabPageMovement.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMovement.Size = new System.Drawing.Size(1358, 450);
            this.tabPageMovement.TabIndex = 4;
            this.tabPageMovement.Text = LanguageManager.Get("tab_movement");
            this.tabPageMovement.UseVisualStyleBackColor = true;

            this.tabPageAnalysis.Location = new System.Drawing.Point(4, 26);
            this.tabPageAnalysis.Name = "tabPageAnalysis";
            this.tabPageAnalysis.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAnalysis.Size = new System.Drawing.Size(1358, 450);
            this.tabPageAnalysis.TabIndex = 5;
            this.tabPageAnalysis.Text = LanguageManager.Get("tab_analysis");
            this.tabPageAnalysis.UseVisualStyleBackColor = true;

            this.tabPagePrices.Location = new System.Drawing.Point(4, 26);
            this.tabPagePrices.Name = "tabPagePrices";
            this.tabPagePrices.Padding = new System.Windows.Forms.Padding(3);
            this.tabPagePrices.Size = new System.Drawing.Size(1358, 450);
            this.tabPagePrices.TabIndex = 6;
            this.tabPagePrices.Text = LanguageManager.Get("tab_prices");
            this.tabPagePrices.UseVisualStyleBackColor = true;

            // ==========================================
            // النموذج
            // ==========================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(247)))), ((int)(((byte)(250)))));
            this.ClientSize = new System.Drawing.Size(1366, 700);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.grpHeader);
            this.Controls.Add(this.toolStrip1);
            this.Name = "ItemDataForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = LanguageManager.Get("item_form_title");
            this.AutoScroll = true;

            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpHeader.ResumeLayout(false);
            this.grpHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPageMain.ResumeLayout(false);
            this.tabPageMain.PerformLayout();
            this.tabPageUnits.ResumeLayout(false);
            this.tabPageUnits.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvUnits)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvWeightUnits)).EndInit();
            this.tabPageCost.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCost)).EndInit();
            this.tabPageOther.ResumeLayout(false);
            this.tabPageOther.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ==========================================
        // تعريفات العناصر
        // ==========================================
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnNew, btnSave, btnDelete, btnPrint, btnExport;
        private System.Windows.Forms.ToolStripButton btnFirst, btnPrevious, btnNext, btnLast;
        private System.Windows.Forms.ToolStripButton btnSearch, btnFilter, btnHome, btnExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1, toolStripSeparator2, toolStripSeparator3, toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparatorGroups;

        private System.Windows.Forms.GroupBox grpHeader;
        private System.Windows.Forms.Label lblItemType;
        private System.Windows.Forms.ComboBox cmbItemType;
        private System.Windows.Forms.Label lblGroupNo;
        private System.Windows.Forms.TextBox txtGroupNo;
        private System.Windows.Forms.Label lblGroupName;
        private System.Windows.Forms.TextBox txtGroupName;
        private System.Windows.Forms.Label lblSubGroupUnder;
        private System.Windows.Forms.TextBox txtSubGroupUnder;
        private System.Windows.Forms.Label lblItemCode;
        private System.Windows.Forms.TextBox txtItemCode;
        private System.Windows.Forms.Label lblItemName;
        private System.Windows.Forms.TextBox txtItemName;
        private System.Windows.Forms.Label lblBranch;
        private System.Windows.Forms.TextBox txtBranch;
        private System.Windows.Forms.Label lblRefNo;
        private System.Windows.Forms.TextBox txtRefNo;

        // ✅ تعريفات الحقول الجديدة في الـ Header (ComboBoxes بدلاً من CheckBoxes)
        private System.Windows.Forms.Label lblCountryOfOrigin;
        private System.Windows.Forms.TextBox txtCountryOfOrigin;
        private System.Windows.Forms.Label lblAgriculturePermit;
        private System.Windows.Forms.ComboBox cmbAgriculturePermit;
        private System.Windows.Forms.Label lblPaymentRelease;
        private System.Windows.Forms.ComboBox cmbPaymentRelease;

        private System.Windows.Forms.Label lblBarcode;
        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.Label lblForeignName;
        private System.Windows.Forms.TextBox txtForeignName;
        private System.Windows.Forms.PictureBox picItem;
        private System.Windows.Forms.Button btnImgAdd;
        private System.Windows.Forms.Button btnImgDel;
        private System.Windows.Forms.Label lblItemImage;

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPageMain;
        private System.Windows.Forms.TabPage tabPageUnits;
        private System.Windows.Forms.TabPage tabPageOther;
        private System.Windows.Forms.TabPage tabPageCost;
        private System.Windows.Forms.TabPage tabPageMovement;
        private System.Windows.Forms.TabPage tabPageAnalysis;
        private System.Windows.Forms.TabPage tabPagePrices;

        private System.Windows.Forms.Label lblItemSpecs;
        private System.Windows.Forms.TextBox txtItemSpecs;
        private System.Windows.Forms.Label lblItemSpecsForeign;
        private System.Windows.Forms.TextBox txtItemSpecsForeign;
        private System.Windows.Forms.Label lblAssistant;
        private System.Windows.Forms.TextBox txtAssistant;
        private System.Windows.Forms.Label lblDetailGroup;
        private System.Windows.Forms.TextBox txtDetailGroup;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.TextBox txtClass;
        private System.Windows.Forms.Label lblInitialCost;
        private System.Windows.Forms.TextBox txtInitialCost;
        private System.Windows.Forms.Label lblDefaultUnit;
        private System.Windows.Forms.ComboBox cmbDefaultUnit;
        private System.Windows.Forms.Label lblPurchaseInvoice;
        private System.Windows.Forms.TextBox txtPurchaseInvoice;
        private System.Windows.Forms.Label lblItemActivity;
        private System.Windows.Forms.ComboBox cmbItemActivity;
        private System.Windows.Forms.Label lblReturnPeriod;
        private System.Windows.Forms.TextBox txtReturnPeriod;
        private System.Windows.Forms.Label lblRank;
        private System.Windows.Forms.ComboBox cmbRank;
        private System.Windows.Forms.Label lblLength;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.Label lblSize;
        private System.Windows.Forms.TextBox txtSize;
        private System.Windows.Forms.Label lblWidth;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.Label lblColor;
        private System.Windows.Forms.TextBox txtColor;
        private System.Windows.Forms.Label lblHeight;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label lblSeason;
        private System.Windows.Forms.ComboBox cmbSeason;
        private System.Windows.Forms.Label lblVolume;
        private System.Windows.Forms.TextBox txtVolume;
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.TextBox txtMaterial;
        private System.Windows.Forms.Label lblArea;
        private System.Windows.Forms.TextBox txtArea;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.TextBox txtBrand;
        private System.Windows.Forms.Label lblManufacturer;
        private System.Windows.Forms.TextBox txtManufacturer;
        private System.Windows.Forms.Label lblMfgNo;
        private System.Windows.Forms.TextBox txtMfgNo;
        private System.Windows.Forms.Label lblCountryMain;
        private System.Windows.Forms.TextBox txtCountryMain;
        private System.Windows.Forms.Label lblStorage;
        private System.Windows.Forms.TextBox txtStorage;
        private System.Windows.Forms.Label lblOpenCost;
        private System.Windows.Forms.TextBox txtOpenCost;
        private System.Windows.Forms.Label lblOpeningStock;
        private System.Windows.Forms.TextBox txtOpeningStock;
        private System.Windows.Forms.Label lblStopDate;
        private System.Windows.Forms.TextBox txtStopDate;
        private System.Windows.Forms.Label lblAvailableQty;
        private System.Windows.Forms.TextBox txtAvailableQty;
        private System.Windows.Forms.Label lblStopReason;
        private System.Windows.Forms.TextBox txtStopReason;

        private System.Windows.Forms.DataGridView dgvUnits;
        private System.Windows.Forms.CheckBox chkMainUnit;
        private System.Windows.Forms.CheckBox chkSalesUnit;
        private System.Windows.Forms.CheckBox chkPurchaseUnit;
        private System.Windows.Forms.CheckBox chkInventoryUnit;
        private System.Windows.Forms.CheckBox chkNotForSale;
        private System.Windows.Forms.CheckBox chkStop;
        private System.Windows.Forms.Label lblMainUnit;
        private System.Windows.Forms.Label lblSalesUnit;
        private System.Windows.Forms.Label lblPurchaseUnit;
        private System.Windows.Forms.Label lblInventoryUnit;
        private System.Windows.Forms.Label lblNotForSale;
        private System.Windows.Forms.Label lblStop;

        private System.Windows.Forms.DataGridView dgvWeightUnits;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeightUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeightValue;

        private System.Windows.Forms.DataGridView dgvCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWarehouse;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWarehouseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colInitialCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvgCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvailableQty;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colStatus;

        private System.Windows.Forms.CheckBox chkAllowFreeQty;
        private System.Windows.Forms.CheckBox chkHasWarranty;
        private System.Windows.Forms.Label lblWarrantyPeriod;
        private System.Windows.Forms.TextBox txtWarrantyPeriod;
        private System.Windows.Forms.Label lblWarrantyDay;
        private System.Windows.Forms.CheckBox chkHasCommission;
        private System.Windows.Forms.Label lblCommissionType;
        private System.Windows.Forms.ComboBox cmbCommissionType;
        private System.Windows.Forms.TextBox txtCommissionType;
        private System.Windows.Forms.Label lblFreeQtyPercent;
        private System.Windows.Forms.TextBox txtFreeQtyPercent;
        private System.Windows.Forms.Label lblExpiryDuration;
        private System.Windows.Forms.TextBox txtExpiryDuration;
        private System.Windows.Forms.Label lblExpiryDay;
        private System.Windows.Forms.Label lblMinCostPercent;
        private System.Windows.Forms.TextBox txtMinCostPercent;
        private System.Windows.Forms.Label lblMinCostSign;
        private System.Windows.Forms.Label lblMaxCostPercent;
        private System.Windows.Forms.TextBox txtMaxCostPercent;
        private System.Windows.Forms.Label lblMaxCostSign;
        private System.Windows.Forms.Panel pnlSeparator;
        private System.Windows.Forms.CheckBox chkIsWeighted;
        private System.Windows.Forms.Label lblScaleName;
        private System.Windows.Forms.ComboBox cmbScaleName;
        private System.Windows.Forms.TextBox txtScaleName;
        private System.Windows.Forms.Button lblScaleBtn;
        private System.Windows.Forms.Label lblKey;
        private System.Windows.Forms.TextBox txtKey;
        private System.Windows.Forms.Label lblDocType;
        private System.Windows.Forms.TextBox txtDocType;
        private System.Windows.Forms.Label lblDocRefNo;
        private System.Windows.Forms.TextBox txtDocRefNo;
        private System.Windows.Forms.Label lblDocSeq;
        private System.Windows.Forms.TextBox txtDocSeq;
    }
}