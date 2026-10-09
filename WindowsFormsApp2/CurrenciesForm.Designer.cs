namespace YemenCustomsSystem
{
    partial class CurrenciesForm
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

        #region Windows Form Designer generated code
        private void InitializeComponent()
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

            this.grpCurrencyData = new System.Windows.Forms.GroupBox();
            this.chkStored = new System.Windows.Forms.CheckBox();
            this.radioForeign = new System.Windows.Forms.RadioButton();
            this.radioLocal = new System.Windows.Forms.RadioButton();
            this.lblNumber = new System.Windows.Forms.Label();
            this.txtNumber = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblForeignName = new System.Windows.Forms.Label();
            this.txtForeignName = new System.Windows.Forms.TextBox();
            this.lblSymbol = new System.Windows.Forms.Label();
            this.txtSymbol = new System.Windows.Forms.TextBox();
            this.lblSubunit = new System.Windows.Forms.Label();
            this.txtSubunit = new System.Windows.Forms.TextBox();
            this.lblExchangeRate = new System.Windows.Forms.Label();
            this.txtExchangeRate = new System.Windows.Forms.TextBox();
            this.lblMinRate = new System.Windows.Forms.Label();
            this.txtMinRate = new System.Windows.Forms.TextBox();
            this.lblMaxRate = new System.Windows.Forms.Label();
            this.txtMaxRate = new System.Windows.Forms.TextBox();
            this.lblDecimalPlaces = new System.Windows.Forms.Label();
            this.txtDecimalPlaces = new System.Windows.Forms.TextBox();

            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.dgvAudit = new System.Windows.Forms.DataGridView();

            this.toolStrip1.SuspendLayout();
            this.grpCurrencyData.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAudit)).BeginInit();
            this.SuspendLayout();

            // ==========================================
            // toolStrip1
            // ==========================================
        
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnHome, this.btnNew, this.btnSave, this.btnEdit, this.btnDelete,
                this.btnView, this.btnSearch, this.btnPrint, this.btnPrevious, this.btnNext, this.btnExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStrip1.Size = new System.Drawing.Size(1651, 37);
            this.toolStrip1.TabIndex = 3;

            this.btnHome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnHome.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnHome.ForeColor = System.Drawing.Color.DarkBlue;
            this.btnHome.Name = "btnHome";
            this.btnHome.Size = new System.Drawing.Size(116, 32);
            this.btnHome.Text = "🏠 الرئيسية";
            this.btnHome.Click += new System.EventHandler(this.btnHome_Click);

            this.btnNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNew.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnNew.ForeColor = System.Drawing.Color.Green;
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(89, 32);
            this.btnNew.Text = "➕ جديد";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);

            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.Green;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(89, 32);
            this.btnSave.Text = "💾 حفظ";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEdit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnEdit.ForeColor = System.Drawing.Color.Orange;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(99, 32);
            this.btnEdit.Text = "✏️ تعديل";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);

            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDelete.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.ForeColor = System.Drawing.Color.Red;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(92, 32);
            this.btnDelete.Text = "🗑️ حذف";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);

            this.btnView.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnView.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(93, 32);
            this.btnView.Text = "👁️ عرض";
            this.btnView.Click += new System.EventHandler(this.btnView_Click);

            this.btnSearch.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnSearch.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(85, 32);
            this.btnSearch.Text = "🔍 بحث";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);

            this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(99, 32);
            this.btnPrint.Text = "🖨️ طباعة";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);

            this.btnPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnPrevious.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(85, 32);
            this.btnPrevious.Text = "⬅️ سابق";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);

            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNext.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(84, 32);
            this.btnNext.Text = "تالي ➡️";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);

            this.btnExit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.btnExit.ForeColor = System.Drawing.Color.Gray;
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(90, 32);
            this.btnExit.Text = "❌ خروج";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // ==========================================
            // grpCurrencyData
            // ==========================================
            this.grpCurrencyData.Controls.Add(this.chkStored);
            this.grpCurrencyData.Controls.Add(this.radioForeign);
            this.grpCurrencyData.Controls.Add(this.radioLocal);
            this.grpCurrencyData.Controls.Add(this.lblNumber);
            this.grpCurrencyData.Controls.Add(this.txtNumber);
            this.grpCurrencyData.Controls.Add(this.lblName);
            this.grpCurrencyData.Controls.Add(this.txtName);
            this.grpCurrencyData.Controls.Add(this.lblForeignName);
            this.grpCurrencyData.Controls.Add(this.txtForeignName);
            this.grpCurrencyData.Controls.Add(this.lblSymbol);
            this.grpCurrencyData.Controls.Add(this.txtSymbol);
            this.grpCurrencyData.Controls.Add(this.lblSubunit);
            this.grpCurrencyData.Controls.Add(this.txtSubunit);
            this.grpCurrencyData.Controls.Add(this.lblExchangeRate);
            this.grpCurrencyData.Controls.Add(this.txtExchangeRate);
            this.grpCurrencyData.Controls.Add(this.lblMinRate);
            this.grpCurrencyData.Controls.Add(this.txtMinRate);
            this.grpCurrencyData.Controls.Add(this.lblMaxRate);
            this.grpCurrencyData.Controls.Add(this.txtMaxRate);
            this.grpCurrencyData.Controls.Add(this.lblDecimalPlaces);
            this.grpCurrencyData.Controls.Add(this.txtDecimalPlaces);
            this.grpCurrencyData.Location = new System.Drawing.Point(15, 50);
            this.grpCurrencyData.Name = "grpCurrencyData";
            this.grpCurrencyData.Size = new System.Drawing.Size(1620, 212);
            this.grpCurrencyData.TabIndex = 1;
            this.grpCurrencyData.TabStop = false;
            this.grpCurrencyData.Text = "📋 بيانات العملة";

            this.chkStored.AutoSize = true;
            this.chkStored.Location = new System.Drawing.Point(193, 35);
            this.chkStored.Name = "chkStored";
            this.chkStored.Size = new System.Drawing.Size(105, 24);
            this.chkStored.TabIndex = 0;
            this.chkStored.Text = "عملة المخزون";

            this.radioForeign.AutoSize = true;
            this.radioForeign.Location = new System.Drawing.Point(360, 35);
            this.radioForeign.Name = "radioForeign";
            this.radioForeign.Size = new System.Drawing.Size(92, 24);
            this.radioForeign.TabIndex = 1;
            this.radioForeign.Text = "عملة أجنبية";

            this.radioLocal.AutoSize = true;
            this.radioLocal.Checked = true;
            this.radioLocal.Location = new System.Drawing.Point(514, 35);
            this.radioLocal.Name = "radioLocal";
            this.radioLocal.Size = new System.Drawing.Size(88, 24);
            this.radioLocal.TabIndex = 2;
            this.radioLocal.TabStop = true;
            this.radioLocal.Text = "عملة محلية";

            this.lblNumber.AutoSize = true;
            this.lblNumber.Location = new System.Drawing.Point(1479, 35);
            this.lblNumber.Name = "lblNumber";
            this.lblNumber.Size = new System.Drawing.Size(62, 20);
            this.lblNumber.TabIndex = 3;
            this.lblNumber.Text = "رقم العملة:";

            this.txtNumber.Location = new System.Drawing.Point(1286, 32);
            this.txtNumber.Name = "txtNumber";
            this.txtNumber.Size = new System.Drawing.Size(179, 26);
            this.txtNumber.TabIndex = 4;

            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(1157, 35);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(63, 20);
            this.lblName.TabIndex = 5;
            this.lblName.Text = "اسم العملة:";

            this.txtName.Location = new System.Drawing.Point(964, 32);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(179, 26);
            this.txtName.TabIndex = 6;

            this.lblForeignName.AutoSize = true;
            this.lblForeignName.Location = new System.Drawing.Point(836, 35);
            this.lblForeignName.Name = "lblForeignName";
            this.lblForeignName.Size = new System.Drawing.Size(82, 20);
            this.lblForeignName.TabIndex = 7;
            this.lblForeignName.Text = "الاسم الأجنبي:";

            this.txtForeignName.Location = new System.Drawing.Point(643, 32);
            this.txtForeignName.Name = "txtForeignName";
            this.txtForeignName.Size = new System.Drawing.Size(179, 26);
            this.txtForeignName.TabIndex = 8;

            this.lblSymbol.AutoSize = true;
            this.lblSymbol.Location = new System.Drawing.Point(1479, 82);
            this.lblSymbol.Name = "lblSymbol";
            this.lblSymbol.Size = new System.Drawing.Size(66, 20);
            this.lblSymbol.TabIndex = 9;
            this.lblSymbol.Text = "رمز العملة:";

            this.txtSymbol.Location = new System.Drawing.Point(1286, 79);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(179, 26);
            this.txtSymbol.TabIndex = 10;

            this.lblSubunit.AutoSize = true;
            this.lblSubunit.Location = new System.Drawing.Point(1157, 82);
            this.lblSubunit.Name = "lblSubunit";
            this.lblSubunit.Size = new System.Drawing.Size(60, 20);
            this.lblSubunit.TabIndex = 11;
            this.lblSubunit.Text = "اسم الفكة:";

            this.txtSubunit.Location = new System.Drawing.Point(964, 79);
            this.txtSubunit.Name = "txtSubunit";
            this.txtSubunit.Size = new System.Drawing.Size(179, 26);
            this.txtSubunit.TabIndex = 12;

            this.lblExchangeRate.BackColor = System.Drawing.Color.FromArgb(255, 255, 224);
            this.lblExchangeRate.Location = new System.Drawing.Point(1479, 129);
            this.lblExchangeRate.Name = "lblExchangeRate";
            this.lblExchangeRate.Size = new System.Drawing.Size(129, 27);
            this.lblExchangeRate.TabIndex = 13;
            this.lblExchangeRate.Text = "سعر التحويل:";

            this.txtExchangeRate.Location = new System.Drawing.Point(1286, 126);
            this.txtExchangeRate.Name = "txtExchangeRate";
            this.txtExchangeRate.Size = new System.Drawing.Size(179, 26);
            this.txtExchangeRate.TabIndex = 14;
            this.txtExchangeRate.TextChanged += new System.EventHandler(this.txtExchangeRate_TextChanged);

            this.lblMinRate.BackColor = System.Drawing.Color.FromArgb(255, 255, 224);
            this.lblMinRate.Location = new System.Drawing.Point(1157, 129);
            this.lblMinRate.Name = "lblMinRate";
            this.lblMinRate.Size = new System.Drawing.Size(129, 27);
            this.lblMinRate.TabIndex = 15;
            this.lblMinRate.Text = "أدنى سعر:";

            this.txtMinRate.Location = new System.Drawing.Point(964, 126);
            this.txtMinRate.Name = "txtMinRate";
            this.txtMinRate.Size = new System.Drawing.Size(179, 26);
            this.txtMinRate.TabIndex = 16;

            this.lblMaxRate.BackColor = System.Drawing.Color.FromArgb(255, 255, 224);
            this.lblMaxRate.Location = new System.Drawing.Point(836, 129);
            this.lblMaxRate.Name = "lblMaxRate";
            this.lblMaxRate.Size = new System.Drawing.Size(129, 27);
            this.lblMaxRate.TabIndex = 17;
            this.lblMaxRate.Text = "أعلى سعر:";

            this.txtMaxRate.Location = new System.Drawing.Point(643, 126);
            this.txtMaxRate.Name = "txtMaxRate";
            this.txtMaxRate.Size = new System.Drawing.Size(179, 26);
            this.txtMaxRate.TabIndex = 18;

            this.lblDecimalPlaces.AutoSize = true;
            this.lblDecimalPlaces.Location = new System.Drawing.Point(836, 82);
            this.lblDecimalPlaces.Name = "lblDecimalPlaces";
            this.lblDecimalPlaces.Size = new System.Drawing.Size(85, 20);
            this.lblDecimalPlaces.TabIndex = 19;
            this.lblDecimalPlaces.Text = "خانات الكسور:";

            this.txtDecimalPlaces.Location = new System.Drawing.Point(643, 79);
            this.txtDecimalPlaces.Name = "txtDecimalPlaces";
            this.txtDecimalPlaces.Size = new System.Drawing.Size(102, 26);
            this.txtDecimalPlaces.TabIndex = 20;
            this.txtDecimalPlaces.Text = "2";

            // ==========================================
            // tabControl1
            // ==========================================
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(15, 270);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1620, 494);
            this.tabControl1.TabIndex = 2;

            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(1612, 461);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "📄 البيانات الرئيسية";

            this.tabPage2.Controls.Add(this.dgvAudit);
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Size = new System.Drawing.Size(1612, 461);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "📊 جدول رقابي للتعديلات";

            // ==========================================
            // dgvAudit
            // ==========================================
            this.dgvAudit.AllowUserToAddRows = false;
            this.dgvAudit.AllowUserToDeleteRows = false;
            this.dgvAudit.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAudit.ColumnHeadersHeight = 34;
            this.dgvAudit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvAudit.Location = new System.Drawing.Point(0, 0);
            this.dgvAudit.Name = "dgvAudit";
            this.dgvAudit.ReadOnly = true;
            this.dgvAudit.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvAudit.RowHeadersWidth = 62;
            this.dgvAudit.Size = new System.Drawing.Size(1612, 461);
            this.dgvAudit.TabIndex = 0;

            // ==========================================
            // CurrenciesForm
            // ==========================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.ClientSize = new System.Drawing.Size(1651, 826);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.grpCurrencyData);
            this.Controls.Add(this.toolStrip1);
            this.Name = "CurrenciesForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Text = "💱 تهيئة العملات";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CurrenciesForm_FormClosing);

            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.grpCurrencyData.ResumeLayout(false);
            this.grpCurrencyData.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvAudit)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnHome, btnNew, btnSave, btnEdit, btnDelete, btnView, btnSearch, btnPrint, btnPrevious, btnNext, btnExit;
        private System.Windows.Forms.GroupBox grpCurrencyData;
        private System.Windows.Forms.CheckBox chkStored;
        private System.Windows.Forms.RadioButton radioForeign, radioLocal;
        private System.Windows.Forms.Label lblNumber, lblName, lblForeignName, lblSymbol, lblSubunit, lblExchangeRate, lblMinRate, lblMaxRate, lblDecimalPlaces;
        private System.Windows.Forms.TextBox txtNumber, txtName, txtForeignName, txtSymbol, txtSubunit, txtExchangeRate, txtMinRate, txtMaxRate, txtDecimalPlaces;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1, tabPage2;
        private System.Windows.Forms.DataGridView dgvAudit;
    }
}