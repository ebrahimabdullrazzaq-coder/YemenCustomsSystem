namespace YemenCustomsSystem
{
    partial class GroupsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing) { if (disposing && (components != null)) components.Dispose(); base.Dispose(disposing); }

        private void InitializeComponent()
        {
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNew = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFirst = new System.Windows.Forms.ToolStripButton();
            this.btnPrevious = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnLast = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSearch = new System.Windows.Forms.ToolStripButton();
            this.btnExit = new System.Windows.Forms.ToolStripButton();

            this.grpData = new System.Windows.Forms.GroupBox();
            this.lblGroupNo = new System.Windows.Forms.Label();
            this.txtGroupNo = new System.Windows.Forms.TextBox();
            this.lblGroupName = new System.Windows.Forms.Label();
            this.txtGroupName = new System.Windows.Forms.TextBox();
            this.lblGroupNameEn = new System.Windows.Forms.Label();
            this.txtGroupNameEn = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.dgvGroups = new System.Windows.Forms.DataGridView();

            this.toolStrip1.SuspendLayout();
            this.grpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).BeginInit();
            this.SuspendLayout();

            // toolStrip1
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.btnNew, this.btnSave, this.btnDelete, this.toolStripSeparator1,
                this.btnFirst, this.btnPrevious, this.btnNext, this.btnLast, this.toolStripSeparator2,
                this.btnSearch, this.btnExit});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(900, 25);
            this.toolStrip1.TabIndex = 0;
            this.btnNew.Text = "جديد"; this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            this.btnSave.Text = "حفظ"; this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.btnDelete.Text = "حذف"; this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            this.btnFirst.Text = "الأول"; this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            this.btnPrevious.Text = "السابق"; this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            this.btnNext.Text = "التالي"; this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnLast.Text = "الأخير"; this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            this.btnSearch.Text = "بحث"; this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            this.btnExit.Text = "خروج"; this.btnExit.Click += new System.EventHandler(this.btnExit_Click);

            // grpData
            this.grpData.Controls.Add(this.lblGroupNo); this.grpData.Controls.Add(this.txtGroupNo);
            this.grpData.Controls.Add(this.lblGroupName); this.grpData.Controls.Add(this.txtGroupName);
            this.grpData.Controls.Add(this.lblGroupNameEn); this.grpData.Controls.Add(this.txtGroupNameEn);
            this.grpData.Controls.Add(this.lblDescription); this.grpData.Controls.Add(this.txtDescription);
            this.grpData.Controls.Add(this.chkActive);
            this.grpData.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpData.Location = new System.Drawing.Point(0, 25);
            this.grpData.Name = "grpData";
            this.grpData.Size = new System.Drawing.Size(900, 150);
            this.grpData.TabIndex = 1;
            this.grpData.TabStop = false;
            this.grpData.Text = "بيانات المجموعة";

            this.lblGroupNo.AutoSize = true; this.lblGroupNo.Location = new System.Drawing.Point(750, 30); this.lblGroupNo.Text = "رقم المجموعة ⭐:";
            this.txtGroupNo.Location = new System.Drawing.Point(600, 27); this.txtGroupNo.Size = new System.Drawing.Size(140, 23); this.txtGroupNo.TabIndex = 0;

            this.lblGroupName.AutoSize = true; this.lblGroupName.Location = new System.Drawing.Point(750, 60); this.lblGroupName.Text = "اسم المجموعة ⭐:";
            this.txtGroupName.Location = new System.Drawing.Point(600, 57); this.txtGroupName.Size = new System.Drawing.Size(140, 23); this.txtGroupName.TabIndex = 1;

            this.lblGroupNameEn.AutoSize = true; this.lblGroupNameEn.Location = new System.Drawing.Point(450, 60); this.lblGroupNameEn.Text = "الاسم الأجنبي:";
            this.txtGroupNameEn.Location = new System.Drawing.Point(300, 57); this.txtGroupNameEn.Size = new System.Drawing.Size(140, 23); this.txtGroupNameEn.TabIndex = 2;

            this.lblDescription.AutoSize = true; this.lblDescription.Location = new System.Drawing.Point(750, 90); this.lblDescription.Text = "الوصف:";
            this.txtDescription.Location = new System.Drawing.Point(300, 87); this.txtDescription.Size = new System.Drawing.Size(440, 23); this.txtDescription.TabIndex = 3;

            this.chkActive.AutoSize = true; this.chkActive.Checked = true; this.chkActive.Location = new System.Drawing.Point(600, 90); this.chkActive.Size = new System.Drawing.Size(55, 19); this.chkActive.Text = "نشط"; this.chkActive.TabIndex = 4; this.chkActive.UseVisualStyleBackColor = true;

            // dgvGroups
            this.dgvGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvGroups.Location = new System.Drawing.Point(0, 175);
            this.dgvGroups.Name = "dgvGroups";
            this.dgvGroups.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.dgvGroups.Size = new System.Drawing.Size(900, 300);
            this.dgvGroups.TabIndex = 2;
            this.dgvGroups.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvGroups_CellClick);

            // GroupsForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(900, 475);
            this.Controls.Add(this.dgvGroups);
            this.Controls.Add(this.grpData);
            this.Controls.Add(this.toolStrip1);
            this.Name = "GroupsForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.Text = "تهيئة المجموعات";

            this.toolStrip1.ResumeLayout(false); this.toolStrip1.PerformLayout();
            this.grpData.ResumeLayout(false); this.grpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvGroups)).EndInit();
            this.ResumeLayout(false); this.PerformLayout();
        }

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnNew, btnSave, btnDelete, btnFirst, btnPrevious, btnNext, btnLast, btnSearch, btnExit;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1, toolStripSeparator2;
        private System.Windows.Forms.GroupBox grpData;
        private System.Windows.Forms.Label lblGroupNo, lblGroupName, lblGroupNameEn, lblDescription;
        private System.Windows.Forms.TextBox txtGroupNo, txtGroupName, txtGroupNameEn, txtDescription;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.DataGridView dgvGroups;
    }
}