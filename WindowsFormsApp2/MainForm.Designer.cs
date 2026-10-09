namespace YemenCustomsSystem
{
    partial class MainForm
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.mnuSetup = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCurrencies = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPorts = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWarehouses = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItems = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuItemsData = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeclarations = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuChartOfAccounts = new System.Windows.Forms.ToolStripMenuItem();

            this.mnuOperations = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClearance = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNewDeclaration = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCustomsClearance = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTariffRequest = new System.Windows.Forms.ToolStripMenuItem();          // ✅ جديد
            this.mnuReviewTariffRequests = new System.Windows.Forms.ToolStripMenuItem();  // ✅ جديد
            this.mnuStockTransfer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWarehouseTransfer = new System.Windows.Forms.ToolStripMenuItem();

            this.mnuReports = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCurrencyReports = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClearanceReports = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuWarehouseReports = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();

            this.pnlMenu = new System.Windows.Forms.Panel();
            this.treeMenu = new System.Windows.Forms.TreeView();
            this.lblWelcome = new System.Windows.Forms.Label();

            this.menuStrip1.SuspendLayout();
            this.pnlMenu.SuspendLayout();
            this.SuspendLayout();

            // ==========================================
            // menuStrip1 - الشريط العلوي
            // ==========================================
            this.menuStrip1.BackColor = System.Drawing.Color.Silver;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuSetup,
                this.mnuOperations,
                this.mnuReports,
                this.mnuExit
            });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.menuStrip1.Size = new System.Drawing.Size(1384, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";

            // ==========================================
            // mnuSetup - ⚙️ تهيئة النظام
            // ==========================================
            this.mnuSetup.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuCurrencies,
                this.mnuPorts,
                this.mnuWarehouses,
                this.mnuItems,
                this.mnuItemsData,
                this.mnuDeclarations,
                this.mnuChartOfAccounts
            });
            this.mnuSetup.Name = "mnuSetup";
            this.mnuSetup.Size = new System.Drawing.Size(110, 20);
            this.mnuSetup.Text = "⚙️ تهيئة النظام";

            this.mnuCurrencies.Name = "mnuCurrencies";
            this.mnuCurrencies.Size = new System.Drawing.Size(180, 22);
            this.mnuCurrencies.Text = "💱 تهيئة العملات";
            this.mnuCurrencies.Click += new System.EventHandler(this.mnuCurrencies_Click);

            this.mnuPorts.Name = "mnuPorts";
            this.mnuPorts.Size = new System.Drawing.Size(180, 22);
            this.mnuPorts.Text = "🚢 تهيئة المنافذ";
            this.mnuPorts.Click += new System.EventHandler(this.mnuPorts_Click);

            this.mnuWarehouses.Name = "mnuWarehouses";
            this.mnuWarehouses.Size = new System.Drawing.Size(180, 22);
            this.mnuWarehouses.Text = "🏢 تهيئة المخازن";
            this.mnuWarehouses.Click += new System.EventHandler(this.mnuWarehouses_Click);

            this.mnuItems.Name = "mnuItems";
            this.mnuItems.Size = new System.Drawing.Size(180, 22);
            this.mnuItems.Text = "📦 تهيئة الأصناف";
            this.mnuItems.Click += new System.EventHandler(this.mnuItems_Click);

            this.mnuItemsData.Name = "mnuItemsData";
            this.mnuItemsData.Size = new System.Drawing.Size(180, 22);
            this.mnuItemsData.Text = "📋 بيانات الأصناف";
            this.mnuItemsData.Click += new System.EventHandler(this.mnuItemsData_Click);

            this.mnuDeclarations.Name = "mnuDeclarations";
            this.mnuDeclarations.Size = new System.Drawing.Size(180, 22);
            this.mnuDeclarations.Text = "📋 البيانات الجمركية";
            this.mnuDeclarations.Click += new System.EventHandler(this.mnuDeclarations_Click);

            this.mnuChartOfAccounts.Name = "mnuChartOfAccounts";
            this.mnuChartOfAccounts.Size = new System.Drawing.Size(180, 22);
            this.mnuChartOfAccounts.Text = "📒 الدليل المحاسبي";
            this.mnuChartOfAccounts.Click += new System.EventHandler(this.mnuChartOfAccounts_Click);

            // ==========================================
            // mnuOperations - 📋 العمليات
            // ==========================================
            this.mnuOperations.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuClearance,
                this.mnuNewDeclaration,
                this.mnuCustomsClearance,
                this.mnuTariffRequest,             // ✅ جديد
                this.mnuReviewTariffRequests,      // ✅ جديد
                new System.Windows.Forms.ToolStripSeparator(),
                this.mnuStockTransfer,
                this.mnuWarehouseTransfer
            });
            this.mnuOperations.Name = "mnuOperations";
            this.mnuOperations.Size = new System.Drawing.Size(75, 20);
            this.mnuOperations.Text = "📋 العمليات";

            this.mnuClearance.Name = "mnuClearance";
            this.mnuClearance.Size = new System.Drawing.Size(220, 22);
            this.mnuClearance.Text = "📝 بيانات التخليص والمنافذ";
            this.mnuClearance.Click += new System.EventHandler(this.mnuClearance_Click);

            this.mnuNewDeclaration.Name = "mnuNewDeclaration";
            this.mnuNewDeclaration.Size = new System.Drawing.Size(220, 22);
            this.mnuNewDeclaration.Text = "📄 إدخال بيان جمركي";
            this.mnuNewDeclaration.Click += new System.EventHandler(this.mnuNewDeclaration_Click);

            this.mnuCustomsClearance.Name = "mnuCustomsClearance";
            this.mnuCustomsClearance.Size = new System.Drawing.Size(220, 22);
            this.mnuCustomsClearance.Text = "🛃 شاشة التخليص الجمركي";
            this.mnuCustomsClearance.Click += new System.EventHandler(this.mnuCustomsClearance_Click);

            // ✅ جديد - طلب ترسيم
            this.mnuTariffRequest.Name = "mnuTariffRequest";
            this.mnuTariffRequest.Size = new System.Drawing.Size(220, 22);
            this.mnuTariffRequest.Text = "📝 طلب ترسيم";
            this.mnuTariffRequest.Click += new System.EventHandler(this.mnuTariffRequest_Click);

            // ✅ جديد - مراجعة طلبات الترسيم
            this.mnuReviewTariffRequests.Name = "mnuReviewTariffRequests";
            this.mnuReviewTariffRequests.Size = new System.Drawing.Size(220, 22);
            this.mnuReviewTariffRequests.Text = "👁️ مراجعة طلبات الترسيم";
            this.mnuReviewTariffRequests.Click += new System.EventHandler(this.mnuReviewTariffRequests_Click);

            // ✅ عمليات التحويل
            this.mnuStockTransfer.Name = "mnuStockTransfer";
            this.mnuStockTransfer.Size = new System.Drawing.Size(220, 22);
            this.mnuStockTransfer.Text = "🚚 نقل من المنفذ إلى المخزن";
            this.mnuStockTransfer.Click += new System.EventHandler(this.mnuStockTransfer_Click);

            this.mnuWarehouseTransfer.Name = "mnuWarehouseTransfer";
            this.mnuWarehouseTransfer.Size = new System.Drawing.Size(220, 22);
            this.mnuWarehouseTransfer.Text = "🏚️ نقل بين المخازن";
            this.mnuWarehouseTransfer.Click += new System.EventHandler(this.mnuWarehouseTransfer_Click);

            // ==========================================
            // mnuReports - 📊 التقارير
            // ==========================================
            this.mnuReports.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.mnuCurrencyReports,
                this.mnuClearanceReports,
                this.mnuWarehouseReports
            });
            this.mnuReports.Name = "mnuReports";
            this.mnuReports.Size = new System.Drawing.Size(67, 20);
            this.mnuReports.Text = "📊 التقارير";

            this.mnuCurrencyReports.Name = "mnuCurrencyReports";
            this.mnuCurrencyReports.Size = new System.Drawing.Size(180, 22);
            this.mnuCurrencyReports.Text = "📈 تقارير العملات";
            this.mnuCurrencyReports.Click += new System.EventHandler(this.mnuCurrencyReports_Click);

            this.mnuClearanceReports.Name = "mnuClearanceReports";
            this.mnuClearanceReports.Size = new System.Drawing.Size(180, 22);
            this.mnuClearanceReports.Text = "📋 تقارير التخليص";
            this.mnuClearanceReports.Click += new System.EventHandler(this.mnuClearanceReports_Click);

            this.mnuWarehouseReports.Name = "mnuWarehouseReports";
            this.mnuWarehouseReports.Size = new System.Drawing.Size(180, 22);
            this.mnuWarehouseReports.Text = "🏚️ تقارير حركة المخازن";
            this.mnuWarehouseReports.Click += new System.EventHandler(this.mnuWarehouseReports_Click);

            // ==========================================
            // mnuExit - 🚪 خروج
            // ==========================================
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(51, 20);
            this.mnuExit.Text = "🚪 خروج";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);

            // ==========================================
            // pnlMenu - لوحة القائمة الجانبية
            // ==========================================
            this.pnlMenu.BackColor = System.Drawing.Color.LightGray;
            this.pnlMenu.Controls.Add(this.treeMenu);
            this.pnlMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlMenu.Location = new System.Drawing.Point(1084, 24);
            this.pnlMenu.Name = "pnlMenu";
            this.pnlMenu.Size = new System.Drawing.Size(300, 661);
            this.pnlMenu.TabIndex = 2;
            this.pnlMenu.Visible = true;

            // ==========================================
            // treeMenu - الشجرة الجانبية
            // ==========================================
            this.treeMenu.BackColor = System.Drawing.Color.White;
            this.treeMenu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeMenu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeMenu.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.treeMenu.Location = new System.Drawing.Point(0, 0);
            this.treeMenu.Name = "treeMenu";
            this.treeMenu.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.treeMenu.Size = new System.Drawing.Size(300, 661);
            this.treeMenu.TabIndex = 0;
            this.treeMenu.Visible = true;
            this.treeMenu.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeMenu_AfterSelect);

            // ==========================================
            // lblWelcome - شاشة الترحيب
            // ==========================================
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Traditional Arabic", 28F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.ForeColor = System.Drawing.Color.FromArgb(0, 112, 192);
            this.lblWelcome.Location = new System.Drawing.Point(350, 250);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(500, 90);
            this.lblWelcome.TabIndex = 0;
            this.lblWelcome.Text = "نظام الجمارك اليمنية المتكامل";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblWelcome.Visible = true;

            // ==========================================
            // MainForm - النموذج الرئيسي
            // ==========================================
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1384, 685);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.pnlMenu);
            this.Controls.Add(this.lblWelcome);
            this.Font = new System.Drawing.Font("Tahoma", 9.5F);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "نظام الجمارك اليمنية المتكامل";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.MdiChildActivate += new System.EventHandler(this.MainForm_MdiChildActivate);

            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        // ==================== تعريفات العناصر ====================
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mnuSetup;
        private System.Windows.Forms.ToolStripMenuItem mnuCurrencies;
        private System.Windows.Forms.ToolStripMenuItem mnuPorts;
        private System.Windows.Forms.ToolStripMenuItem mnuWarehouses;
        private System.Windows.Forms.ToolStripMenuItem mnuItems;
        private System.Windows.Forms.ToolStripMenuItem mnuItemsData;
        private System.Windows.Forms.ToolStripMenuItem mnuDeclarations;
        private System.Windows.Forms.ToolStripMenuItem mnuChartOfAccounts;

        private System.Windows.Forms.ToolStripMenuItem mnuOperations;
        private System.Windows.Forms.ToolStripMenuItem mnuClearance;
        private System.Windows.Forms.ToolStripMenuItem mnuNewDeclaration;
        private System.Windows.Forms.ToolStripMenuItem mnuCustomsClearance;
        private System.Windows.Forms.ToolStripMenuItem mnuTariffRequest;             // ✅ جديد
        private System.Windows.Forms.ToolStripMenuItem mnuReviewTariffRequests;      // ✅ جديد
        private System.Windows.Forms.ToolStripMenuItem mnuStockTransfer;
        private System.Windows.Forms.ToolStripMenuItem mnuWarehouseTransfer;

        private System.Windows.Forms.ToolStripMenuItem mnuReports;
        private System.Windows.Forms.ToolStripMenuItem mnuCurrencyReports;
        private System.Windows.Forms.ToolStripMenuItem mnuClearanceReports;
        private System.Windows.Forms.ToolStripMenuItem mnuWarehouseReports;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;

        private System.Windows.Forms.Panel pnlMenu;
        private System.Windows.Forms.TreeView treeMenu;
        private System.Windows.Forms.Label lblWelcome;
    }
}