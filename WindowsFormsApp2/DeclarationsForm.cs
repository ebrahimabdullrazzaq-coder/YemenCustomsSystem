using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace YemenCustomsSystem
{
    public partial class DeclarationsForm : Form
    {
        public DeclarationsForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Close();

            if (this.MdiParent != null)
            {
                MainForm mainForm = this.MdiParent as MainForm;
                if (mainForm != null)
                {
                    foreach (Form child in mainForm.MdiChildren.ToList())
                    {
                        child.Close();
                    }
                    mainForm.ShowWelcome();
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            MessageBox.Show("✅ حفظ البيان - قيد التطوير", "معلومة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            MessageBox.Show("➕ إضافة بيان - قيد التطوير", "معلومة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            MessageBox.Show("✏ تعديل بيان - قيد التطوير", "معلومة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            MessageBox.Show("🗑 حذف بيان - قيد التطوير", "معلومة", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void DeclarationsForm_Load(object sender, EventArgs e)
        {

        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DeclarationsForm
            // 
            this.ClientSize = new System.Drawing.Size(278, 244);
            this.Name = "DeclarationsForm";
            this.Load += new System.EventHandler(this.DeclarationsForm_Load_1);
            this.ResumeLayout(false);

        }

        private void DeclarationsForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}