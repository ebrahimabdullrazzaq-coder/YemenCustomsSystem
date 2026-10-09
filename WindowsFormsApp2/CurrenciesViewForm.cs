using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class CurrenciesViewForm : Form
    {
        public string SelectedCurrencyCode { get; private set; }
        public string SelectedCurrencyName { get; private set; }
        public decimal SelectedExchangeRate { get; private set; }
        public decimal SelectedMinRate { get; private set; }
        public decimal SelectedMaxRate { get; private set; }

        public bool IsDeleteMode { get; private set; } = false;

        // قائمة العملات الحقيقية من النموذج الرئيسي (جميع العملات)
        private List<CurrenciesForm.CurrencyItem> currenciesList = new List<CurrenciesForm.CurrencyItem>();

        // كلاس لنقل بيانات العملة
        public class CurrencyDisplayData
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Type { get; set; }
            public decimal ExchangeRate { get; set; }
            public decimal MinRate { get; set; }
            public decimal MaxRate { get; set; }
            public string ModifiedDate { get; set; }
        }

        public CurrenciesViewForm(bool deleteMode = false)
        {
            InitializeComponent();
            IsDeleteMode = deleteMode;

            if (deleteMode)
            {
                this.Text = "🗑 حذف عملة";
                btnOK.Text = "🗑 حذف";
                btnOK.BackColor = Color.FromArgb(244, 67, 54);
            }
            else
            {
                this.Text = "📋 جميع العملات";
                btnOK.Text = "✅ موافق";
                btnOK.BackColor = Color.FromArgb(76, 175, 80);
            }

            // إعداد أعمدة الجدول
            SetupGridColumns();

            // تحميل جميع العملات من النموذج الرئيسي
            LoadDataFromMainForm();
        }

        // إعداد أعمدة الجدول
        private void SetupGridColumns()
        {
            dgvCurrenciesView.Columns.Clear();

            int colIndex = dgvCurrenciesView.Columns.Add("colRowNum", "م");
            dgvCurrenciesView.Columns[colIndex].Width = 40;

            int colNameIndex = dgvCurrenciesView.Columns.Add("colCurrencyName", "اسم العملة");
            dgvCurrenciesView.Columns[colNameIndex].Width = 200;

            int colCodeIndex = dgvCurrenciesView.Columns.Add("colCurrencyCode", "رمز العملة");
            dgvCurrenciesView.Columns[colCodeIndex].Width = 100;

            int colTypeIndex = dgvCurrenciesView.Columns.Add("colCurrencyType", "نوع العملة");
            dgvCurrenciesView.Columns[colTypeIndex].Width = 100;

            int colRateIndex = dgvCurrenciesView.Columns.Add("colExchangeRate", "سعر التحويل");
            dgvCurrenciesView.Columns[colRateIndex].Width = 120;

            int colMinRateIndex = dgvCurrenciesView.Columns.Add("colMinRate", "أدنى سعر");
            dgvCurrenciesView.Columns[colMinRateIndex].Width = 100;

            int colMaxRateIndex = dgvCurrenciesView.Columns.Add("colMaxRate", "أعلى سعر");
            dgvCurrenciesView.Columns[colMaxRateIndex].Width = 100;

            int colDateIndex = dgvCurrenciesView.Columns.Add("colModifiedDate", "آخر تعديل");
            dgvCurrenciesView.Columns[colDateIndex].Width = 150;

            dgvCurrenciesView.EnableHeadersVisualStyles = false;
            dgvCurrenciesView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(64, 171, 216);
            dgvCurrenciesView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvCurrenciesView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(242, 242, 242);
            dgvCurrenciesView.RowsDefaultCellStyle.BackColor = Color.White;
            dgvCurrenciesView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCurrenciesView.AllowUserToAddRows = false;
            dgvCurrenciesView.AllowUserToDeleteRows = false;
            dgvCurrenciesView.ReadOnly = true;
            dgvCurrenciesView.RowHeadersVisible = false;
            dgvCurrenciesView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        // تحميل البيانات من النموذج الرئيسي
        private void LoadDataFromMainForm()
        {
            try
            {
                var parent = Application.OpenForms.OfType<CurrenciesForm>().FirstOrDefault();
                if (parent != null)
                {
                    currenciesList = parent.GetCurrenciesList();
                    RefreshGridFromList();

                    // ربط حدث تغيير التحديد
                    dgvCurrenciesView.SelectionChanged += DgvCurrenciesView_SelectionChanged;

                    // تحديد أول صف إذا وجد
                    if (dgvCurrenciesView.Rows.Count > 0)
                    {
                        dgvCurrenciesView.CurrentCell = dgvCurrenciesView.Rows[0].Cells[1];
                    }
                }
                else
                {
                    MessageBox.Show("❌ لم يتم العثور على النموذج الرئيسي!",
                        "خطأ",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}",
                    "خطأ",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // تحديث الجدول من القائمة
        private void RefreshGridFromList()
        {
            dgvCurrenciesView.Rows.Clear();

            int rowNum = 1;
            foreach (var currency in currenciesList)
            {
                dgvCurrenciesView.Rows.Add(
                    rowNum++,
                    currency.Name,
                    currency.Code,
                    currency.Type,
                    currency.ExchangeRate.ToString("N4"),
                    currency.MinRate.ToString("N4"),
                    currency.MaxRate.ToString("N4"),
                    currency.ModifiedDate
                );
            }
        }

        // حدث تغيير التحديد في الجدول
        private void DgvCurrenciesView_SelectionChanged(object sender, EventArgs e)
        {
            // عند تغيير التحديد بالماوس، نحدث البيانات في CurrenciesForm
            try
            {
                var parent = Application.OpenForms.OfType<CurrenciesForm>().FirstOrDefault();
                if (parent != null && parent.Visible)
                {
                    var currencyData = GetCurrentCurrencyData();
                    if (currencyData != null)
                    {
                        // استدعاء الدالة في CurrenciesForm لتحديث الخانات
                        parent.LoadCurrencyDataFromView(currencyData);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في تحديث البيانات: {ex.Message}");
            }
        }

        /// <summary>
        /// دالة للحصول على بيانات العملة الحالية
        /// </summary>
        private CurrencyDisplayData GetCurrentCurrencyData()
        {
            if (dgvCurrenciesView.CurrentRow == null)
                return null;

            DataGridViewRow row = dgvCurrenciesView.CurrentRow;

            return new CurrencyDisplayData
            {
                Name = row.Cells["colCurrencyName"].Value?.ToString() ?? "",
                Code = row.Cells["colCurrencyCode"].Value?.ToString() ?? "",
                Type = row.Cells["colCurrencyType"].Value?.ToString() ?? "",
                ExchangeRate = decimal.TryParse(row.Cells["colExchangeRate"].Value?.ToString(), out decimal rate) ? rate : 0,
                MinRate = decimal.TryParse(row.Cells["colMinRate"].Value?.ToString(), out decimal min) ? min : 0,
                MaxRate = decimal.TryParse(row.Cells["colMaxRate"].Value?.ToString(), out decimal max) ? max : 0,
                ModifiedDate = row.Cells["colModifiedDate"].Value?.ToString() ?? ""
            };
        }

        /// <summary>
        /// دالة للحصول على العملة السابقة (تُستدعى من CurrenciesForm)
        /// </summary>
        public CurrencyDisplayData GetPreviousCurrency()
        {
            if (dgvCurrenciesView.Rows.Count == 0)
                return null;

            // إذا لم يكن هناك صف محدد، اختر الأول
            if (dgvCurrenciesView.CurrentRow == null)
            {
                if (dgvCurrenciesView.Rows.Count > 0)
                {
                    dgvCurrenciesView.CurrentCell = dgvCurrenciesView.Rows[0].Cells[1];
                    return GetCurrentCurrencyData();
                }
                return null;
            }

            int currentIndex = dgvCurrenciesView.CurrentRow.Index;
            if (currentIndex > 0)
            {
                // انتقل إلى الصف السابق
                dgvCurrenciesView.CurrentCell = dgvCurrenciesView.Rows[currentIndex - 1].Cells[1];
                return GetCurrentCurrencyData();
            }
            else
            {
                // إذا كان أول صف، انتقل إلى آخر صف (دوران)
                dgvCurrenciesView.CurrentCell = dgvCurrenciesView.Rows[dgvCurrenciesView.Rows.Count - 1].Cells[1];
                return GetCurrentCurrencyData();
            }
        }

        /// <summary>
        /// دالة للحصول على العملة التالية (تُستدعى من CurrenciesForm)
        /// </summary>
        public CurrencyDisplayData GetNextCurrency()
        {
            if (dgvCurrenciesView.Rows.Count == 0)
                return null;

            // إذا لم يكن هناك صف محدد، اختر الأول
            if (dgvCurrenciesView.CurrentRow == null)
            {
                if (dgvCurrenciesView.Rows.Count > 0)
                {
                    dgvCurrenciesView.CurrentCell = dgvCurrenciesView.Rows[0].Cells[1];
                    return GetCurrentCurrencyData();
                }
                return null;
            }

            int currentIndex = dgvCurrenciesView.CurrentRow.Index;
            if (currentIndex < dgvCurrenciesView.Rows.Count - 1)
            {
                // انتقل إلى الصف التالي
                dgvCurrenciesView.CurrentCell = dgvCurrenciesView.Rows[currentIndex + 1].Cells[1];
                return GetCurrentCurrencyData();
            }
            else
            {
                // إذا كان آخر صف، انتقل إلى أول صف (دوران)
                dgvCurrenciesView.CurrentCell = dgvCurrenciesView.Rows[0].Cells[1];
                return GetCurrentCurrencyData();
            }
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            string searchTerm = txtSearch.Text.Trim().ToLower();
            int count = 0;

            foreach (DataGridViewRow row in dgvCurrenciesView.Rows)
            {
                string nameText = row.Cells["colCurrencyName"].Value?.ToString()?.ToLower() ?? "";
                string codeText = row.Cells["colCurrencyCode"].Value?.ToString()?.ToLower() ?? "";

                row.Visible = nameText.Contains(searchTerm) || codeText.Contains(searchTerm);
                if (row.Visible) count++;
            }

            MessageBox.Show(count > 0 ? $"تم العثور على {count} نتيجة" : "لم يتم العثور على أي نتائج",
                            "نتيجة البحث", MessageBoxButtons.OK);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (dgvCurrenciesView.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار عملة من الجدول أولاً.", "تنبيه",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow selectedRow = dgvCurrenciesView.SelectedRows[0];

            string tempCode = selectedRow.Cells["colCurrencyCode"].Value.ToString();
            string tempName = selectedRow.Cells["colCurrencyName"].Value.ToString();
            string tempType = selectedRow.Cells["colCurrencyType"].Value.ToString();
            decimal tempRate = 0;
            decimal tempMinRate = 0;
            decimal tempMaxRate = 0;

            decimal.TryParse(selectedRow.Cells["colExchangeRate"].Value.ToString(), out tempRate);
            decimal.TryParse(selectedRow.Cells["colMinRate"].Value?.ToString() ?? "0", out tempMinRate);
            decimal.TryParse(selectedRow.Cells["colMaxRate"].Value?.ToString() ?? "0", out tempMaxRate);

            SelectedCurrencyCode = tempCode;
            SelectedCurrencyName = tempName;
            SelectedExchangeRate = tempRate;
            SelectedMinRate = tempMinRate;
            SelectedMaxRate = tempMaxRate;

            if (IsDeleteMode)
            {
                string message = $"هل أنت متأكد من حذف العملة التالية نهائياً؟\n\n" +
                                $"🏷️ اسم العملة: {tempName}\n" +
                                $"🔤 رمز العملة: {tempCode}\n" +
                                $"📊 نوع العملة: {tempType}\n" +
                                $"💰 سعر التحويل: {tempRate:N4}\n\n" +
                                $"⚠️ هذا الإجراء لا يمكن التراجع عنه!";

                DialogResult confirmResult = MessageBox.Show(message,
                    "تأكيد الحذف النهائي",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (confirmResult == DialogResult.Yes)
                {
                    try
                    {
                        var parent = Application.OpenForms.OfType<CurrenciesForm>().FirstOrDefault();
                        if (parent != null)
                        {
                            bool deleted = parent.DeleteCurrencyPermanently(tempName);

                            if (deleted)
                            {
                                // تحديث القائمة المحلية
                                currenciesList = parent.GetCurrenciesList();
                                RefreshGridFromList();

                                MessageBox.Show($"✅ تم حذف « {tempName} » نهائياً بنجاح!",
                                    "نجاح",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);

                                dgvCurrenciesView.ClearSelection();
                            }
                            else
                            {
                                MessageBox.Show($"❌ لم يتم العثور على العملة « {tempName} » في الجدول الرئيسي!",
                                    "خطأ",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ حدث خطأ أثناء الحذف:\n{ex.Message}",
                            "خطأ",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void DgvCurrenciesView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                btnOK_Click(sender, e);
        }
    }
}