using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Data;

namespace YemenCustomsSystem
{
    public partial class PortsViewForm : Form
    {
        public string SelectedPortName { get; private set; }
        public string SelectedPortNature { get; private set; }
        public string SelectedPortClassification { get; private set; }
        public string SelectedPortType { get; private set; }
        public string SelectedExchangeRate { get; private set; }
        public bool IsDeleteMode { get; private set; } = false;

        public static List<PortViewData> AllPorts = new List<PortViewData>();

        public PortsViewForm(bool deleteMode = false)
        {
            InitializeComponent();
            IsDeleteMode = deleteMode;

            if (deleteMode)
            {
                this.Text = "🗑 حذف منفذ";
                btnOK.Text = "🗑 حذف";
                btnOK.BackColor = Color.FromArgb(244, 67, 54);
            }
            else
            {
                this.Text = "🏛️ جميع المنافذ";
                btnOK.Text = "✅ اختيار";
                btnOK.BackColor = Color.FromArgb(76, 175, 80);
            }

            LoadPortsData();
        }

        public class PortViewData
        {
            public string Name { get; set; }
            public string Nature { get; set; }
            public string Classification { get; set; }
            public string Type { get; set; }
            public string Rate { get; set; }
        }

        private void LoadPortsData()
        {
            dgvPortsView.Columns.Clear();
            dgvPortsView.Columns.Add(new DataGridViewTextBoxColumn { Name = "colRowNum", HeaderText = "م", Width = 40, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter } });
            dgvPortsView.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPortName", HeaderText = "اسم المنفذ", Width = 200, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleRight } });
            dgvPortsView.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPortNature", HeaderText = "طبيعة المنفذ", Width = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, BackColor = Color.FromArgb(232, 245, 233), Font = new Font("Segoe UI", 9, FontStyle.Bold) } });
            dgvPortsView.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPortClassification", HeaderText = "تصنيف المنفذ", Width = 150, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 8.5F, FontStyle.Bold) } });
            dgvPortsView.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPortType", HeaderText = "نوع المنفذ", Width = 80, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, BackColor = Color.FromArgb(227, 242, 253), Font = new Font("Segoe UI", 9, FontStyle.Bold) } });
            dgvPortsView.Columns.Add(new DataGridViewTextBoxColumn { Name = "colExchangeRate", HeaderText = "سعر الصرف", Width = 100, DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(211, 47, 47) } });

            dgvPortsView.EnableHeadersVisualStyles = false;
            dgvPortsView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvPortsView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPortsView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPortsView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242);
            dgvPortsView.RowsDefaultCellStyle.BackColor = Color.White;
            dgvPortsView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPortsView.AllowUserToAddRows = false;
            dgvPortsView.AllowUserToDeleteRows = false;
            dgvPortsView.ReadOnly = true;
            dgvPortsView.RowHeadersVisible = false;

            // ✅ تم التغيير: التحميل من قاعدة البيانات بدلاً من ملف JSON
            LoadFromDatabase();
            RefreshGrid();
        }

        private void LoadFromDatabase()
        {
            AllPorts.Clear();
            try
            {
                // ✅ جلب البيانات من جدول Ports في قاعدة البيانات
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName, PortNature, PortType, Classification, ExchangeRate FROM Ports ORDER BY PortName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        AllPorts.Add(new PortViewData
                        {
                            Name = row["PortName"].ToString(),
                            Nature = row["PortNature"].ToString(),
                            Classification = row["Classification"].ToString(),
                            Type = row["PortType"].ToString(),
                            Rate = row["ExchangeRate"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحميل المنافذ:\n{ex.Message}", "خطأ");
            }
        }

        public void RefreshGrid()
        {
            dgvPortsView.Rows.Clear();
            int rowNum = 1;
            foreach (var port in AllPorts)
            {
                int rowIndex = dgvPortsView.Rows.Add(rowNum++, port.Name, port.Nature, port.Classification, port.Type, port.Rate);

                if (port.Classification.Contains("جنوب") || port.Classification.Contains("خارجي"))
                    dgvPortsView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                else if (port.Classification.Contains("شمال") || port.Classification.Contains("داخلي"))
                    dgvPortsView.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(227, 242, 253);

                if (port.Rate == "750")
                    dgvPortsView.Rows[rowIndex].Cells["colExchangeRate"].Style.ForeColor = Color.FromArgb(211, 47, 47);
                else if (port.Rate == "535")
                    dgvPortsView.Rows[rowIndex].Cells["colExchangeRate"].Style.ForeColor = Color.FromArgb(22, 160, 133);
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string term = txtSearch.Text.Trim().ToLower();
            int count = 0;
            foreach (DataGridViewRow row in dgvPortsView.Rows)
            {
                string name = row.Cells["colPortName"].Value?.ToString()?.ToLower() ?? "";
                row.Visible = name.Contains(term);
                if (row.Visible) count++;
            }
            MessageBox.Show(count > 0 ? $"✅ تم العثور على {count} نتيجة" : "❌ لم يتم العثور على نتائج", "بحث");
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvPortsView.SelectedRows.Count == 0) { MessageBox.Show("⚠️ اختر منفذاً أولاً", "تنبيه"); return; }

            var row = dgvPortsView.SelectedRows[0];
            SelectedPortName = row.Cells["colPortName"].Value?.ToString() ?? "";
            SelectedPortNature = row.Cells["colPortNature"].Value?.ToString() ?? "";
            SelectedPortClassification = row.Cells["colPortClassification"].Value?.ToString() ?? "";
            SelectedPortType = row.Cells["colPortType"].Value?.ToString() ?? "";
            SelectedExchangeRate = row.Cells["colExchangeRate"].Value?.ToString() ?? "";

            if (IsDeleteMode)
            {
                if (MessageBox.Show($"🗑️ حذف « {SelectedPortName} » نهائياً من قاعدة البيانات؟", "تأكيد", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        // ✅ الحذف من قاعدة البيانات
                        DatabaseHelper.ExecuteNonQuery("DELETE FROM Ports WHERE PortName = @Name",
                            DatabaseHelper.CreateParameter("@Name", SelectedPortName));

                        // تحديث القائمة المحلية
                        AllPorts.RemoveAll(p => p.Name == SelectedPortName);
                        RefreshGrid();

                        // تحديث الشاشة الرئيسية إذا كانت مفتوحة
                        var portsForm = Application.OpenForms.OfType<PortsForm>().FirstOrDefault();
                        if (portsForm != null)
                        {
                            portsForm.DeletePortFromAllLists(SelectedPortName);
                            portsForm.LoadAllPortsData(); // إعادة تحميل البيانات في الشاشة الرئيسية
                        }

                        MessageBox.Show($"✅ تم حذف « {SelectedPortName} » بنجاح", "تم");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ خطأ في الحذف:\n{ex.Message}", "خطأ");
                    }
                }
            }
            else
            {
                OpenPortsFormWithData();
            }
        }

        // ==================== ✅ الدالة المصححة - مطابقة تامة مع PortsForm ====================
        private void OpenPortsFormWithData()
        {
            try
            {
                var portsForm = Application.OpenForms.OfType<PortsForm>().FirstOrDefault();
                if (portsForm != null)
                {
                    // ✅ تصحيح ترتيب المعاملات لتطابق دالة LoadPortData في PortsForm.cs
                    // الترتيب الصحيح: (name, nature, portType, classification, rate, selectedExchangeRate)
                    portsForm.LoadPortData(
                        SelectedPortName,           // 1. name
                        SelectedPortNature,         // 2. nature (كان يُمرر Address هنا بالخطأ)
                        SelectedPortType,           // 3. portType
                        SelectedPortClassification, // 4. classification
                        SelectedExchangeRate,       // 5. rate
                        SelectedExchangeRate        // 6. selectedExchangeRate
                    );
                    portsForm.Activate();
                    portsForm.BringToFront();
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ:\n{ex.Message}", "خطأ");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DgvPortsView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnOK_Click(sender, e);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            RefreshGrid();
        }
    }
}