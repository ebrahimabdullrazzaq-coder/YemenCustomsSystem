using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Text.Json;

namespace YemenCustomsSystem
{
    public partial class ItemsDataForm : Form
    {
        private List<ItemsRecord> records = new List<ItemsRecord>();
        private List<ItemDataForm.ItemData> allItems = new List<ItemDataForm.ItemData>();
        private List<PortsForm.PortData> allPorts = new List<PortsForm.PortData>();
        private List<PortGroupData> allPortGroups = new List<PortGroupData>(); // ✅ قائمة مجموعات المنافذ

        private ComboBox cmbPortGroup; // ✅ تم تغيير الاسم
        private ComboBox cmbPortName;
        private Button btnLoadItems;
        private Button btnSaveData;
        private Label lblStatus;
        private Label lblPortGroup; // ✅ تم تغيير الاسم
        private Label lblPortName;
        private DataGridView dgvData;

        private ToolStrip toolStrip1;
        private ToolStripButton btnSave, btnNew, btnEdit, btnDelete, btnSearch, btnExport, btnExit, btnHome;
        private ToolStripSeparator toolStripSeparator1, toolStripSeparator2, toolStripSeparator3, toolStripSeparator4, toolStripSeparatorHome;

        private readonly string[] percentageColumns = new string[]
        {
            "colCustomsRatePct", "colSTPct", "colVATPct", "colPTPct",
            "colVFPct", "colTEFPct", "colMFFPct"
        };

        private string currentPortGroup = ""; // ✅ تم تغيير الاسم
        private bool isEditingMode = false;
        private bool hasChanges = false;

        // ✅ كلاس بيانات مجموعات المنافذ
        public class PortGroupData
        {
            public int Id { get; set; }
            public string GroupNo { get; set; }
            public string GroupName { get; set; }
            public string Description { get; set; }
            public bool IsActive { get; set; }
        }

        public class ItemsRecord
        {
            public int Id { get; set; }
            public string PortGroup { get; set; } // ✅ تم تغيير الاسم
            public string PortName { get; set; }
            public string ProductName { get; set; }
            public string Unit { get; set; }
            public string WeightUnit { get; set; }
            public decimal Weight { get; set; }
            public decimal CustomsPriceUSD { get; set; }
            public decimal ExchangeRate { get; set; }
            public decimal CustomsPriceRY { get; set; }
            public decimal CustomsRatePct { get; set; }
            public decimal CustomsValue { get; set; }
            public decimal ST_Pct { get; set; }
            public decimal VAT_Pct { get; set; }
            public decimal PT_Pct { get; set; }
            public decimal VF_Pct { get; set; }
            public decimal TEF_Pct { get; set; }
            public decimal MFF_Pct { get; set; }
            public decimal TotalCustoms { get; set; }
            public decimal TotalST { get; set; }
            public decimal TotalVAT { get; set; }
            public decimal TotalPT { get; set; }
            public decimal TotalVF { get; set; }
            public decimal TotalTEF { get; set; }
            public decimal TotalMFF { get; set; }
            public string CountryOfOrigin { get; set; }
        }

        public ItemsDataForm()
        {
            CreateControlsManually();
            InitializeGrid();
            LoadItemsFromDatabase();
            LoadPortsFromDatabase();
            LoadPortGroupsFromDatabase(); // ✅ تحميل مجموعات المنافذ
            LoadPricingDataFromDatabase();
            ClearFields();
            SetControlsState(false);

            LanguageManager.ApplyLanguage(this);
            LanguageManager.LanguageChanged += (s, ev) => RefreshAllTranslations();
        }

        // ✅ تحميل مجموعات المنافذ من قاعدة البيانات
        private void LoadPortGroupsFromDatabase()
        {
            allPortGroups.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT Id, GroupNo, GroupName, Description, IsActive FROM PortGroups WHERE IsActive = 1 ORDER BY GroupName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allPortGroups.Add(new PortGroupData
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            GroupNo = row["GroupNo"].ToString(),
                            GroupName = row["GroupName"].ToString(),
                            Description = row["Description"]?.ToString() ?? "",
                            IsActive = Convert.ToBoolean(row["IsActive"])
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ تحميل مجموعات المنافذ: {ex.Message}");
            }
        }

        private void RefreshAllTranslations()
        {
            this.Text = LanguageManager.Get("items_form_title");
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;

            if (btnHome != null) btnHome.Text = LanguageManager.Get("btn_home");
            if (btnSave != null) btnSave.Text = LanguageManager.Get("btn_save");
            if (btnNew != null) btnNew.Text = LanguageManager.Get("btn_new");
            if (btnEdit != null) btnEdit.Text = LanguageManager.Get("btn_edit");
            if (btnDelete != null) btnDelete.Text = LanguageManager.Get("btn_delete");
            if (btnSearch != null) btnSearch.Text = LanguageManager.Get("btn_search");
            if (btnExport != null) btnExport.Text = LanguageManager.Get("btn_export");
            if (btnExit != null) btnExit.Text = LanguageManager.Get("btn_exit");

            if (lblPortGroup != null) lblPortGroup.Text = "مجموعة المنافذ:";
            if (lblPortName != null) lblPortName.Text = LanguageManager.Get("port_name");
            if (btnLoadItems != null) btnLoadItems.Text = LanguageManager.Get("btn_load_items");
            if (btnSaveData != null) btnSaveData.Text = LanguageManager.Get("btn_save_data");
            if (lblStatus != null) lblStatus.Text = "💡 اختر مجموعة المنافذ (اضغط F9 للاختيار)";

            UpdateGridColumnHeaders();

            if (dgvData != null)
            {
                dgvData.RightToLeft = LanguageManager.RightToLeft;
                dgvData.Refresh();
            }
        }

        private void UpdateGridColumnHeaders()
        {
            if (dgvData == null || dgvData.Columns.Count == 0) return;
            if (dgvData.Columns["colId"] != null) dgvData.Columns["colId"].HeaderText = LanguageManager.Get("col_id");
            if (dgvData.Columns["colProductName"] != null) dgvData.Columns["colProductName"].HeaderText = LanguageManager.Get("col_product_name");
            if (dgvData.Columns["colCountryOfOrigin"] != null) dgvData.Columns["colCountryOfOrigin"].HeaderText = "بلد المنشأ";
            if (dgvData.Columns["colUnit"] != null) dgvData.Columns["colUnit"].HeaderText = "وحدة الصنف";
            if (dgvData.Columns["colWeightUnit"] != null) dgvData.Columns["colWeightUnit"].HeaderText = "وحدة الوزن";
            if (dgvData.Columns["colWeight"] != null) dgvData.Columns["colWeight"].HeaderText = LanguageManager.Get("col_weight");
            if (dgvData.Columns["colCustomsPriceUSD"] != null) dgvData.Columns["colCustomsPriceUSD"].HeaderText = LanguageManager.Get("col_customs_price_usd");
            if (dgvData.Columns["colExchangeRate"] != null) dgvData.Columns["colExchangeRate"].HeaderText = LanguageManager.Get("col_exchange_rate");
            if (dgvData.Columns["colCustomsPriceRY"] != null) dgvData.Columns["colCustomsPriceRY"].HeaderText = LanguageManager.Get("col_customs_price_ry");
            if (dgvData.Columns["colCustomsRatePct"] != null) dgvData.Columns["colCustomsRatePct"].HeaderText = LanguageManager.Get("col_customs_rate_pct");
            if (dgvData.Columns["colSTPct"] != null) dgvData.Columns["colSTPct"].HeaderText = LanguageManager.Get("col_st_pct");
            if (dgvData.Columns["colVATPct"] != null) dgvData.Columns["colVATPct"].HeaderText = LanguageManager.Get("col_vat_pct");
            if (dgvData.Columns["colPTPct"] != null) dgvData.Columns["colPTPct"].HeaderText = LanguageManager.Get("col_pt_pct");
            if (dgvData.Columns["colVFPct"] != null) dgvData.Columns["colVFPct"].HeaderText = LanguageManager.Get("col_vf_pct");
            if (dgvData.Columns["colTEFPct"] != null) dgvData.Columns["colTEFPct"].HeaderText = LanguageManager.Get("col_tef_pct");
            if (dgvData.Columns["colMFFPct"] != null) dgvData.Columns["colMFFPct"].HeaderText = LanguageManager.Get("col_mff_pct");
            if (dgvData.Columns["colCustomsValue"] != null) dgvData.Columns["colCustomsValue"].HeaderText = LanguageManager.Get("col_customs_value");
            if (dgvData.Columns["colTotalCustoms"] != null) dgvData.Columns["colTotalCustoms"].HeaderText = LanguageManager.Get("col_total_customs");
            if (dgvData.Columns["colTotalST"] != null) dgvData.Columns["colTotalST"].HeaderText = LanguageManager.Get("col_total_st");
            if (dgvData.Columns["colTotalVAT"] != null) dgvData.Columns["colTotalVAT"].HeaderText = LanguageManager.Get("col_total_vat");
            if (dgvData.Columns["colTotalPT"] != null) dgvData.Columns["colTotalPT"].HeaderText = LanguageManager.Get("col_total_pt");
            if (dgvData.Columns["colTotalVF"] != null) dgvData.Columns["colTotalVF"].HeaderText = LanguageManager.Get("col_total_vf");
            if (dgvData.Columns["colTotalTEF"] != null) dgvData.Columns["colTotalTEF"].HeaderText = LanguageManager.Get("col_total_tef");
            if (dgvData.Columns["colTotalMFF"] != null) dgvData.Columns["colTotalMFF"].HeaderText = LanguageManager.Get("col_total_mff");
            dgvData.Refresh();
        }

        private void LoadItemsFromDatabase()
        {
            allItems.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT ItemName, UnitsJson, WeightUnitsJson FROM Items");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allItems.Add(new ItemDataForm.ItemData
                        {
                            ItemName = row["ItemName"].ToString(),
                            Units = string.IsNullOrEmpty(row["UnitsJson"]?.ToString()) ? new List<ItemDataForm.ItemUnit>() : JsonSerializer.Deserialize<List<ItemDataForm.ItemUnit>>(row["UnitsJson"].ToString()),
                            WeightUnits = string.IsNullOrEmpty(row["WeightUnitsJson"]?.ToString()) ? new List<ItemDataForm.ItemWeightUnit>() : JsonSerializer.Deserialize<List<ItemDataForm.ItemWeightUnit>>(row["WeightUnitsJson"].ToString())
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل الأصناف: {ex.Message}"); }
        }

        private void LoadPortsFromDatabase()
        {
            allPorts.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT PortName, Classification FROM Ports");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        allPorts.Add(new PortsForm.PortData
                        {
                            Name = row["PortName"].ToString(),
                            Classification = row["Classification"].ToString()
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ تحميل المنافذ: {ex.Message}"); }
        }

        private void LoadPricingDataFromDatabase()
        {
            records.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM ItemsPricingData");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        records.Add(new ItemsRecord
                        {
                            Id = Convert.ToInt32(row["Id"]),
                            PortGroup = row["PortClassification"].ToString(),
                            PortName = row["PortName"].ToString(),
                            ProductName = row["ProductName"].ToString(),
                            Unit = row["Unit"].ToString(),
                            WeightUnit = row["WeightUnit"].ToString(),
                            Weight = Convert.ToDecimal(row["Weight"]),
                            CustomsPriceUSD = Convert.ToDecimal(row["CustomsPriceUSD"]),
                            ExchangeRate = Convert.ToDecimal(row["ExchangeRate"]),
                            CustomsPriceRY = Convert.ToDecimal(row["CustomsPriceRY"]),
                            CustomsRatePct = Convert.ToDecimal(row["CustomsRatePct"]),
                            CustomsValue = Convert.ToDecimal(row["CustomsValue"]),
                            ST_Pct = Convert.ToDecimal(row["ST_Pct"]),
                            VAT_Pct = Convert.ToDecimal(row["VAT_Pct"]),
                            PT_Pct = Convert.ToDecimal(row["PT_Pct"]),
                            VF_Pct = Convert.ToDecimal(row["VF_Pct"]),
                            TEF_Pct = Convert.ToDecimal(row["TEF_Pct"]),
                            MFF_Pct = Convert.ToDecimal(row["MFF_Pct"]),
                            TotalCustoms = Convert.ToDecimal(row["TotalCustoms"]),
                            TotalST = Convert.ToDecimal(row["TotalST"]),
                            TotalVAT = Convert.ToDecimal(row["TotalVAT"]),
                            TotalPT = Convert.ToDecimal(row["TotalPT"]),
                            TotalVF = Convert.ToDecimal(row["TotalVF"]),
                            TotalTEF = Convert.ToDecimal(row["TotalTEF"]),
                            TotalMFF = Convert.ToDecimal(row["TotalMFF"]),
                            CountryOfOrigin = row["CountryOfOrigin"] != DBNull.Value ? row["CountryOfOrigin"].ToString() : ""
                        });
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine($"خطأ في التحميل: {ex.Message}"); }
        }

        private void InitializeGrid()
        {
            if (dgvData == null) return;
            dgvData.Columns.Clear();

            dgvData.Columns.Add("colId", LanguageManager.Get("col_id"));
            dgvData.Columns.Add("colProductName", LanguageManager.Get("col_product_name"));
            dgvData.Columns.Add("colCountryOfOrigin", "بلد المنشأ");
            dgvData.Columns.Add("colUnit", "وحدة الصنف");
            dgvData.Columns.Add("colWeightUnit", "وحدة الوزن");
            dgvData.Columns.Add("colWeight", LanguageManager.Get("col_weight"));
            dgvData.Columns.Add("colCustomsPriceUSD", LanguageManager.Get("col_customs_price_usd"));
            dgvData.Columns.Add("colExchangeRate", LanguageManager.Get("col_exchange_rate"));
            dgvData.Columns.Add("colCustomsPriceRY", LanguageManager.Get("col_customs_price_ry"));
            dgvData.Columns.Add("colCustomsRatePct", LanguageManager.Get("col_customs_rate_pct"));
            dgvData.Columns.Add("colCustomsValue", LanguageManager.Get("col_customs_value"));
            dgvData.Columns.Add("colSTPct", LanguageManager.Get("col_st_pct"));
            dgvData.Columns.Add("colVATPct", LanguageManager.Get("col_vat_pct"));
            dgvData.Columns.Add("colPTPct", LanguageManager.Get("col_pt_pct"));
            dgvData.Columns.Add("colVFPct", LanguageManager.Get("col_vf_pct"));
            dgvData.Columns.Add("colTEFPct", LanguageManager.Get("col_tef_pct"));
            dgvData.Columns.Add("colMFFPct", LanguageManager.Get("col_mff_pct"));
            dgvData.Columns.Add("colTotalCustoms", LanguageManager.Get("col_total_customs"));
            dgvData.Columns.Add("colTotalST", LanguageManager.Get("col_total_st"));
            dgvData.Columns.Add("colTotalVAT", LanguageManager.Get("col_total_vat"));
            dgvData.Columns.Add("colTotalPT", LanguageManager.Get("col_total_pt"));
            dgvData.Columns.Add("colTotalVF", LanguageManager.Get("col_total_vf"));
            dgvData.Columns.Add("colTotalTEF", LanguageManager.Get("col_total_tef"));
            dgvData.Columns.Add("colTotalMFF", LanguageManager.Get("col_total_mff"));

            dgvData.Columns["colId"].ReadOnly = true;
            dgvData.Columns["colProductName"].ReadOnly = true;
            dgvData.Columns["colWeight"].ReadOnly = true;
            dgvData.Columns["colUnit"].ReadOnly = true;
            dgvData.Columns["colWeightUnit"].ReadOnly = true;

            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                if (col.Name != "colId" && col.Name != "colProductName" && col.Name != "colWeight" && col.Name != "colUnit" && col.Name != "colWeightUnit")
                    col.ReadOnly = true;
            }

            dgvData.Columns["colProductName"].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvData.Columns["colProductName"].DefaultCellStyle.ForeColor = Color.Black;
            dgvData.Columns["colProductName"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvData.Columns["colId"].DefaultCellStyle.BackColor = Color.FromArgb(240, 240, 240);
            dgvData.Columns["colWeight"].DefaultCellStyle.BackColor = Color.FromArgb(240, 248, 255);
            dgvData.Columns["colUnit"].DefaultCellStyle.BackColor = Color.FromArgb(255, 248, 220);
            dgvData.Columns["colWeightUnit"].DefaultCellStyle.BackColor = Color.FromArgb(220, 255, 220);

            foreach (var colName in percentageColumns)
            {
                if (dgvData.Columns[colName] != null)
                {
                    dgvData.Columns[colName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    dgvData.Columns[colName].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    dgvData.Columns[colName].DefaultCellStyle.ForeColor = Color.DarkBlue;
                }
            }

            foreach (DataGridViewColumn col in dgvData.Columns)
            {
                if (col.Name.Contains("Price") || col.Name.Contains("Rate") || col.Name.Contains("Value") || col.Name.Contains("Total") || col.Name == "colWeight")
                {
                    col.DefaultCellStyle.Format = "N2";
                    col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }
            }

            dgvData.AllowUserToAddRows = false;
            dgvData.AllowUserToDeleteRows = false;
            dgvData.RightToLeft = LanguageManager.RightToLeft;
            dgvData.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvData.ScrollBars = ScrollBars.Both;
            dgvData.BackgroundColor = Color.White;
            dgvData.BorderStyle = BorderStyle.Fixed3D;
            dgvData.GridColor = Color.FromArgb(220, 220, 220);
            dgvData.RowHeadersVisible = false;
            dgvData.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvData.EnableHeadersVisualStyles = false;
            dgvData.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvData.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvData.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvData.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvData.ColumnHeadersHeight = 50;
            dgvData.RowsDefaultCellStyle.BackColor = Color.White;
            dgvData.RowsDefaultCellStyle.ForeColor = Color.Black;
            dgvData.RowsDefaultCellStyle.Font = new Font("Segoe UI", 9);
            dgvData.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvData.RowTemplate.Height = 35;

            // ✅ ربط حدث التحقق من صحة البيانات
            dgvData.CellValidating += DgvData_CellValidating;
            dgvData.CellEndEdit += DgvData_CellEndEdit;


            dgvData.CellFormatting += DgvData_CellFormatting;
            dgvData.CellValueChanged += DgvData_CellValueChanged;
            dgvData.RowsAdded += DgvData_RowsAdded;
            dgvData.CellDoubleClick += DgvData_CellDoubleClick;
            dgvData.SelectionChanged += DgvData_SelectionChanged;
        }

        private void DgvData_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvData != null) btnDelete.Enabled = dgvData.SelectedRows.Count > 0;
        }

        private void DgvData_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            DataGridViewColumn col = dgvData.Columns[e.ColumnIndex];
            if (col == null) return;

            if (percentageColumns.Contains(col.Name))
            {
                if (e.Value != null && decimal.TryParse(e.Value.ToString(), out decimal value))
                {
                    e.Value = $"{value:0.00}%";
                    e.FormattingApplied = true;
                }
            }
        }

        // ==================== ✅ التحقق من نطاق سعر الصرف ====================
        private void DgvData_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgvData.Columns[e.ColumnIndex].Name;

            // ✅ التحقق من عمود سعر الصرف فقط
            if (colName == "colExchangeRate")
            {
                string valueText = e.FormattedValue?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(valueText)) return;

                if (!decimal.TryParse(valueText.Replace(',', '.'),
                                      System.Globalization.NumberStyles.Any,
                                      System.Globalization.CultureInfo.InvariantCulture,
                                      out decimal enteredRate))
                {
                    MessageBox.Show("⚠️ الرجاء إدخال رقم صحيح لسعر الصرف", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                // ✅ التحقق من النطاق بناءً على مجموعة المنافذ
                if (string.IsNullOrEmpty(currentPortGroup))
                {
                    MessageBox.Show("⚠️ يجب اختيار مجموعة المنافذ أولاً", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                var (minRate, maxRate, _) = GetPortGroupRateLimits(currentPortGroup);

                if (enteredRate < minRate || enteredRate > maxRate)
                {
                    MessageBox.Show(
                        $"⚠️ سعر الصرف خارج النطاق المسموح!\n\n" +
                        $"🏛️ مجموعة المنافذ: {currentPortGroup}\n" +
                        $"⬇️ الحد الأدنى: {minRate:N2} ريال/دولار\n" +
                        $"⬆️ الحد الأعلى: {maxRate:N2} ريال/دولار\n" +
                        $"📝 القيمة المدخلة: {enteredRate:N2}\n\n" +
                        $"💡 الرجاء إدخال قيمة بين {minRate:N2} و {maxRate:N2}",
                        "قيمة غير مقبولة", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }
            }

            // ✅ التحقق من عمود السعر بالدولار
            if (colName == "colCustomsPriceUSD")
            {
                string valueText = e.FormattedValue?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(valueText)) return;

                if (!decimal.TryParse(valueText.Replace(',', '.'),
                                      System.Globalization.NumberStyles.Any,
                                      System.Globalization.CultureInfo.InvariantCulture,
                                      out decimal price) || price < 0)
                {
                    MessageBox.Show("⚠️ الرجاء إدخال قيمة صحيحة للسعر بالدولار", "تنبيه",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        // ==================== ✅ عند الانتهاء من التعديل - تعبئة سعر الصرف الافتراضي ====================
        private void DgvData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dgvData.Columns[e.ColumnIndex].Name;
            DataGridViewRow row = dgvData.Rows[e.RowIndex];

            // ✅ إذا تم مسح سعر الصرف → استبداله بالقيمة الافتراضية للمجموعة
            if (colName == "colExchangeRate")
            {
                string val = row.Cells["colExchangeRate"].Value?.ToString() ?? "";
                if (string.IsNullOrWhiteSpace(val) && !string.IsNullOrEmpty(currentPortGroup))
                {
                    var (_, _, defaultRate) = GetPortGroupRateLimits(currentPortGroup);
                    row.Cells["colExchangeRate"].Value = defaultRate;
                }
            }
        }

        private void DgvData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!isEditingMode)
            {
                MessageBox.Show("⚠️ الرجاء الضغط على 'تعديل' أولاً للتعديل على البيانات", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dgvData.Columns[e.ColumnIndex].Name;
                if (columnName == "colUnit") OpenUnitSelectionForm(e.RowIndex);
                else if (columnName == "colWeightUnit") OpenWeightUnitSelectionForm(e.RowIndex);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // ✅ F9 في حقل مجموعة المنافذ
            if (keyData == Keys.F9 && cmbPortGroup != null && cmbPortGroup.Focused)
            {
                OpenPortGroupSelectionForm();
                return true;
            }

            if (keyData == Keys.F9 && dgvData != null && dgvData.Focused && dgvData.CurrentCell != null)
            {
                if (!isEditingMode)
                {
                    MessageBox.Show("⚠️ الرجاء الضغط على 'تعديل' أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return true;
                }
                string columnName = dgvData.Columns[dgvData.CurrentCell.ColumnIndex].Name;
                int rowIndex = dgvData.CurrentCell.RowIndex;

                if (columnName == "colUnit") { OpenUnitSelectionForm(rowIndex); return true; }
                else if (columnName == "colWeightUnit") { OpenWeightUnitSelectionForm(rowIndex); return true; }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ✅ نافذة اختيار مجموعة المنافذ
        private void OpenPortGroupSelectionForm()
        {
            if (allPortGroups == null || allPortGroups.Count == 0)
            {
                MessageBox.Show("⚠️ لا توجد مجموعات منافذ مضافة\n\nالرجاء إضافتها من شاشة 'مجموعات المنافذ'", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form selectForm = new Form();
            selectForm.Text = $"🔍 اختر مجموعة المنافذ ({allPortGroups.Count} مجموعة)";
            selectForm.Size = new Size(700, 500);
            selectForm.StartPosition = FormStartPosition.CenterParent;
            selectForm.RightToLeft = RightToLeft.Yes;
            selectForm.RightToLeftLayout = true;
            selectForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            selectForm.MaximizeBox = false;
            selectForm.MinimizeBox = false;

            Panel pnlSearch = new Panel();
            pnlSearch.Dock = DockStyle.Top;
            pnlSearch.Height = 50;
            pnlSearch.BackColor = Color.FromArgb(240, 248, 255);

            Label lblSearch = new Label();
            lblSearch.Text = "🔍 بحث:";
            lblSearch.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblSearch.Location = new Point(550, 15);
            lblSearch.Size = new Size(60, 25);
            lblSearch.TextAlign = ContentAlignment.MiddleRight;

            TextBox txtSearch = new TextBox();
            txtSearch.Font = new Font("Segoe UI", 10);
            txtSearch.Location = new Point(300, 15);
            txtSearch.Size = new Size(240, 25);

            Label lblCount = new Label();
            lblCount.Text = $"📊 المجموعات: {allPortGroups.Count}";
            lblCount.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCount.ForeColor = Color.FromArgb(33, 150, 243);
            lblCount.Location = new Point(20, 15);
            lblCount.Size = new Size(200, 25);

            pnlSearch.Controls.Add(lblSearch);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Controls.Add(lblCount);

            DataGridView dgvGroups = new DataGridView();
            dgvGroups.Dock = DockStyle.Fill;
            dgvGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGroups.MultiSelect = false;
            dgvGroups.ReadOnly = true;
            dgvGroups.AllowUserToAddRows = false;
            dgvGroups.BackgroundColor = Color.White;
            dgvGroups.RowHeadersVisible = false;
            dgvGroups.EnableHeadersVisualStyles = false;
            dgvGroups.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvGroups.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvGroups.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgvGroups.ColumnHeadersHeight = 40;
            dgvGroups.RowTemplate.Height = 35;

            dgvGroups.Columns.Add("colGroupNo", "رقم المجموعة");
            dgvGroups.Columns.Add("colGroupName", "اسم المجموعة");
            dgvGroups.Columns.Add("colDescription", "الوصف");
            dgvGroups.Columns.Add("colStatus", "الحالة");

            foreach (var group in allPortGroups.Where(g => g.IsActive).OrderBy(g => g.GroupName))
            {
                dgvGroups.Rows.Add(group.GroupNo, group.GroupName, group.Description, "✅ نشط");
            }

            dgvGroups.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex >= 0 && ev.ColumnIndex == 3)
                {
                    ev.CellStyle.ForeColor = Color.Green;
                    ev.CellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
            };

            dgvGroups.CellDoubleClick += (s, ev) =>
            {
                if (ev.RowIndex >= 0)
                {
                    string selectedGroupName = dgvGroups.Rows[ev.RowIndex].Cells["colGroupName"].Value?.ToString();
                    cmbPortGroup.Text = selectedGroupName;
                    currentPortGroup = selectedGroupName;
                    LoadDataToGrid();
                    selectForm.Close();
                }
            };

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnSelect = new Button();
            btnSelect.Text = "✅ اختيار";
            btnSelect.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnSelect.BackColor = Color.FromArgb(76, 175, 80);
            btnSelect.ForeColor = Color.White;
            btnSelect.FlatStyle = FlatStyle.Flat;
            btnSelect.Size = new Size(150, 45);
            btnSelect.Location = new Point(450, 8);
            btnSelect.Click += (s, ev) =>
            {
                if (dgvGroups.SelectedRows.Count > 0)
                {
                    string selectedGroupName = dgvGroups.SelectedRows[0].Cells["colGroupName"].Value?.ToString();
                    cmbPortGroup.Text = selectedGroupName;
                    currentPortGroup = selectedGroupName;
                    LoadDataToGrid();
                    selectForm.Close();
                }
            };

            Button btnCancel = new Button();
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(280, 8);
            btnCancel.Click += (s, ev) => selectForm.Close();

            pnlButtons.Controls.Add(btnSelect);
            pnlButtons.Controls.Add(btnCancel);

            selectForm.Controls.Add(dgvGroups);
            selectForm.Controls.Add(pnlSearch);
            selectForm.Controls.Add(pnlButtons);
            selectForm.AcceptButton = btnSelect;
            selectForm.CancelButton = btnCancel;

            txtSearch.TextChanged += (s, ev) =>
            {
                string searchText = txtSearch.Text.Trim().ToLower();
                int visibleCount = 0;
                foreach (DataGridViewRow row in dgvGroups.Rows)
                {
                    string groupNo = row.Cells["colGroupNo"].Value?.ToString()?.ToLower() ?? "";
                    string groupName = row.Cells["colGroupName"].Value?.ToString()?.ToLower() ?? "";

                    if (string.IsNullOrEmpty(searchText) || groupNo.Contains(searchText) || groupName.Contains(searchText))
                    {
                        row.Visible = true;
                        visibleCount++;
                    }
                    else
                    {
                        row.Visible = false;
                    }
                }
                lblCount.Text = $"📊 المعروض: {visibleCount} من {allPortGroups.Count}";
            };

            selectForm.ShowDialog();
        }

        private void OpenUnitSelectionForm(int rowIndex)
        {
            string productName = dgvData.Rows[rowIndex].Cells["colProductName"].Value?.ToString();
            if (string.IsNullOrEmpty(productName)) { MessageBox.Show("⚠️ الرجاء إدخال اسم الصنف أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var item = allItems.FirstOrDefault(i => i.ItemName == productName);
            if (item == null || item.Units == null || item.Units.Count == 0)
            {
                MessageBox.Show($"⚠️ لا توجد وحدات للصنف '{productName}'\n\nالرجاء إضافة وحدات في شاشة تهيئة الأصناف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form unitForm = new Form();
            unitForm.Text = $"🔍 اختر وحدة الصنف - {productName}";
            unitForm.Size = new Size(450, 350);
            unitForm.StartPosition = FormStartPosition.CenterParent;
            unitForm.RightToLeft = LanguageManager.RightToLeft;
            unitForm.RightToLeftLayout = LanguageManager.IsRTL;
            unitForm.FormBorderStyle = FormBorderStyle.FixedDialog;

            DataGridView dgvUnits = new DataGridView();
            dgvUnits.Dock = DockStyle.Fill;
            dgvUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUnits.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnits.MultiSelect = false;
            dgvUnits.ReadOnly = true;
            dgvUnits.AllowUserToAddRows = false;
            dgvUnits.BackgroundColor = Color.White;
            dgvUnits.RowHeadersVisible = false;
            dgvUnits.EnableHeadersVisualStyles = false;
            dgvUnits.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvUnits.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUnits.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUnits.ColumnHeadersHeight = 40;
            dgvUnits.RowTemplate.Height = 35;

            dgvUnits.Columns.Add("colUnitName", "اسم الوحدة");
            dgvUnits.Columns.Add("colPackage", "العبوة");
            foreach (var unit in item.Units) dgvUnits.Rows.Add(unit.UnitName, unit.Package);

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnOK = new Button();
            btnOK.Text = "✅ اختيار";
            btnOK.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnOK.BackColor = Color.FromArgb(76, 175, 80);
            btnOK.ForeColor = Color.White;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Size = new Size(150, 45);
            btnOK.Location = new Point(220, 8);
            btnOK.DialogResult = DialogResult.OK;

            Button btnCancel = new Button();
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(50, 8);
            btnCancel.DialogResult = DialogResult.Cancel;

            pnlButtons.Controls.Add(btnOK);
            pnlButtons.Controls.Add(btnCancel);
            unitForm.Controls.Add(dgvUnits);
            unitForm.Controls.Add(pnlButtons);
            unitForm.AcceptButton = btnOK;
            unitForm.CancelButton = btnCancel;
            dgvUnits.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) btnOK.PerformClick(); };

            if (unitForm.ShowDialog() == DialogResult.OK && dgvUnits.SelectedRows.Count > 0)
            {
                string selectedUnit = dgvUnits.SelectedRows[0].Cells["colUnitName"].Value?.ToString();
                dgvData.Rows[rowIndex].Cells["colUnit"].Value = selectedUnit;
            }
        }

        private void OpenWeightUnitSelectionForm(int rowIndex)
        {
            string productName = dgvData.Rows[rowIndex].Cells["colProductName"].Value?.ToString();
            if (string.IsNullOrEmpty(productName)) { MessageBox.Show("⚠️ الرجاء إدخال اسم الصنف أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            var item = allItems.FirstOrDefault(i => i.ItemName == productName);
            if (item == null || item.WeightUnits == null || item.WeightUnits.Count == 0)
            {
                MessageBox.Show($"⚠️ لا توجد وحدات وزن للصنف '{productName}'\n\nالرجاء إضافة وحدات الوزن في شاشة تهيئة الأصناف", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Form unitForm = new Form();
            unitForm.Text = $"⚖️ اختر وحدة الوزن - {productName}";
            unitForm.Size = new Size(500, 350);
            unitForm.StartPosition = FormStartPosition.CenterParent;
            unitForm.RightToLeft = LanguageManager.RightToLeft;
            unitForm.RightToLeftLayout = LanguageManager.IsRTL;
            unitForm.FormBorderStyle = FormBorderStyle.FixedDialog;

            DataGridView dgvUnits = new DataGridView();
            dgvUnits.Dock = DockStyle.Fill;
            dgvUnits.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUnits.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUnits.MultiSelect = false;
            dgvUnits.ReadOnly = true;
            dgvUnits.AllowUserToAddRows = false;
            dgvUnits.BackgroundColor = Color.White;
            dgvUnits.RowHeadersVisible = false;
            dgvUnits.EnableHeadersVisualStyles = false;
            dgvUnits.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvUnits.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUnits.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUnits.ColumnHeadersHeight = 40;
            dgvUnits.RowTemplate.Height = 35;

            dgvUnits.Columns.Add("colUnitName", "اسم الوحدة");
            dgvUnits.Columns.Add("colWeight", "الوزن");
            dgvUnits.Columns["colWeight"].DefaultCellStyle.Format = "N2";
            dgvUnits.Columns["colWeight"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            foreach (var weightUnit in item.WeightUnits) dgvUnits.Rows.Add(weightUnit.UnitName, weightUnit.Weight);

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnOK = new Button();
            btnOK.Text = "✅ اختيار";
            btnOK.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnOK.BackColor = Color.FromArgb(76, 175, 80);
            btnOK.ForeColor = Color.White;
            btnOK.FlatStyle = FlatStyle.Flat;
            btnOK.Size = new Size(150, 45);
            btnOK.Location = new Point(250, 8);
            btnOK.DialogResult = DialogResult.OK;

            Button btnCancel = new Button();
            btnCancel.Text = "❌ إلغاء";
            btnCancel.Font = new Font("Tahoma", 11, FontStyle.Bold);
            btnCancel.BackColor = Color.FromArgb(158, 158, 158);
            btnCancel.ForeColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Size = new Size(150, 45);
            btnCancel.Location = new Point(80, 8);
            btnCancel.DialogResult = DialogResult.Cancel;

            pnlButtons.Controls.Add(btnOK);
            pnlButtons.Controls.Add(btnCancel);
            unitForm.Controls.Add(dgvUnits);
            unitForm.Controls.Add(pnlButtons);
            unitForm.AcceptButton = btnOK;
            unitForm.CancelButton = btnCancel;
            dgvUnits.CellDoubleClick += (s, ev) => { if (ev.RowIndex >= 0) btnOK.PerformClick(); };

            if (unitForm.ShowDialog() == DialogResult.OK && dgvUnits.SelectedRows.Count > 0)
            {
                string selectedUnit = dgvUnits.SelectedRows[0].Cells["colUnitName"].Value?.ToString();
                decimal selectedWeight = ParseDecimal(dgvUnits.SelectedRows[0].Cells["colWeight"].Value?.ToString());
                dgvData.Rows[rowIndex].Cells["colWeightUnit"].Value = selectedUnit;
                dgvData.Rows[rowIndex].Cells["colWeight"].Value = selectedWeight;
            }
        }

        private void DgvData_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewRow row = dgvData.Rows[e.RowIndex];
                string columnName = dgvData.Columns[e.ColumnIndex].Name;

                if (columnName == "colCustomsPriceUSD" || columnName == "colExchangeRate")
                {
                    decimal usdPrice = ParseDecimal(row.Cells["colCustomsPriceUSD"].Value?.ToString());
                    decimal exchangeRate = ParseDecimal(row.Cells["colExchangeRate"].Value?.ToString());
                    row.Cells["colCustomsPriceRY"].Value = usdPrice * exchangeRate;
                }

                CalculateRowTotals(e.RowIndex);
                if (isEditingMode) hasChanges = true;
            }
        }

        private void DgvData_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < e.RowCount; i++) CalculateRowTotals(e.RowIndex + i);
        }

        private void CalculateRowTotals(int rowIndex)
        {
            DataGridViewRow row = dgvData.Rows[rowIndex];
            if (row.Cells["colCustomsPriceRY"].Value == null) return;

            decimal customsPriceRY = ParseDecimal(row.Cells["colCustomsPriceRY"].Value?.ToString());
            decimal customsRatePct = ParseDecimal(row.Cells["colCustomsRatePct"].Value?.ToString());
            decimal stPct = ParseDecimal(row.Cells["colSTPct"].Value?.ToString());
            decimal vatPct = ParseDecimal(row.Cells["colVATPct"].Value?.ToString());
            decimal ptPct = ParseDecimal(row.Cells["colPTPct"].Value?.ToString());
            decimal vfPct = ParseDecimal(row.Cells["colVFPct"].Value?.ToString());
            decimal tefPct = ParseDecimal(row.Cells["colTEFPct"].Value?.ToString());
            decimal mffPct = ParseDecimal(row.Cells["colMFFPct"].Value?.ToString());

            decimal totalCustoms = (customsPriceRY * (customsRatePct / 100)) / 2;
            decimal customsValue = (customsPriceRY * 2.139m) + (totalCustoms * 1.5m);
            decimal totalVF = customsPriceRY * (vfPct / 100);
            decimal totalTEF = customsPriceRY * (tefPct / 100);
            decimal totalMFF = customsPriceRY * (mffPct / 100);
            decimal totalST = customsValue * (stPct / 100);
            decimal totalVAT = customsValue * (vatPct / 100);
            decimal totalPT = customsValue * (ptPct / 100);

            row.Cells["colTotalCustoms"].Value = totalCustoms;
            row.Cells["colCustomsValue"].Value = customsValue;
            row.Cells["colTotalST"].Value = totalST;
            row.Cells["colTotalVAT"].Value = totalVAT;
            row.Cells["colTotalPT"].Value = totalPT;
            row.Cells["colTotalVF"].Value = totalVF;
            row.Cells["colTotalTEF"].Value = totalTEF;
            row.Cells["colTotalMFF"].Value = totalMFF;
        }

        private void SetControlsState(bool enabled)
        {
            isEditingMode = enabled;
            if (!enabled) hasChanges = false;

            cmbPortGroup.Enabled = true;
            cmbPortName.Enabled = false;
            cmbPortName.BackColor = Color.FromArgb(240, 240, 240);

            btnLoadItems.Enabled = true;
            btnSaveData.Enabled = enabled;

            if (dgvData != null)
            {
                foreach (DataGridViewColumn col in dgvData.Columns)
                {
                    if (col.Name == "colId" || col.Name == "colProductName" || col.Name == "colWeight" || col.Name == "colUnit" || col.Name == "colWeightUnit")
                    {
                        col.ReadOnly = true;
                    }
                    else
                    {
                        col.ReadOnly = !enabled;
                        col.DefaultCellStyle.BackColor = !col.ReadOnly ? Color.FromArgb(255, 255, 224) : Color.White;
                    }
                }
                dgvData.AllowUserToAddRows = false;
                dgvData.AllowUserToDeleteRows = false;
            }
            btnEdit.Enabled = !enabled;
            btnNew.Enabled = true;
        }

        private void CmbPortGroup_TextChanged(object sender, EventArgs e)
        {
            currentPortGroup = cmbPortGroup.Text.Trim();
            if (!string.IsNullOrEmpty(currentPortGroup))
            {
                lblStatus.Text = $"📋 تم اختيار: {currentPortGroup} - اضغط 'تحميل الأصناف' لعرض البيانات";
                lblStatus.ForeColor = Color.Blue;
            }
            else
            {
                lblStatus.Text = "💡 اختر مجموعة المنافذ (اضغط F9 للاختيار)";
                lblStatus.ForeColor = Color.Orange;
            }
            LoadDataToGrid();
        }

        private void btnLoadItems_Click(object sender, EventArgs e)
        {
            if (allItems == null || allItems.Count == 0) { MessageBox.Show(LanguageManager.Get("msg_no_items"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(currentPortGroup)) { MessageBox.Show("⚠️ الرجاء اختيار مجموعة المنافذ أولاً (اضغط F9)", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            dgvData.Rows.Clear();
            int idx = 1;
            int loadedCount = 0;
            int newCount = 0;

            var savedRecords = records.Where(r => r.PortGroup == currentPortGroup).ToList();
            HashSet<string> loadedProductNames = new HashSet<string>();

            foreach (var record in savedRecords)
            {
                if (!loadedProductNames.Contains(record.ProductName))
                {
                    AddRecordToGrid(record, idx++);
                    loadedProductNames.Add(record.ProductName);
                    loadedCount++;
                }
            }

            // ✅ جلب سعر الصرف الافتراضي للمجموعة الحالية
            var (_, _, defaultRate) = GetPortGroupRateLimits(currentPortGroup);

            foreach (var item in allItems)
            {
                if (!loadedProductNames.Contains(item.ItemName))
                {
                    var itemData = LoadItemDataFromDatabase(item.ItemName);
                    string countryOfOrigin = itemData?.CountryOfOrigin ?? "";

                    int rowIdx = dgvData.Rows.Add(
                        idx++, item.ItemName, countryOfOrigin,
                        "", "", "", "",             // Unit, WeightUnit, Weight, CustomsPriceUSD
                        defaultRate,                 // ✅ ExchangeRate = القيمة الافتراضية للمجموعة
                        "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""
                    );

                    loadedProductNames.Add(item.ItemName);
                    newCount++;
                }
            }

            lblStatus.Text = $"📋 تم إنزال {loadedCount + newCount} صنف لـ {currentPortGroup} (محفوظ: {loadedCount} | جديد: {newCount})";
            lblStatus.ForeColor = Color.Blue;

            MessageBox.Show($"✅ تم إنزال الأصناف بنجاح\n🏛️ مجموعة المنافذ: {currentPortGroup}\n\n📝 الأصناف المحفوظة: {loadedCount}\n🆕 الأصناف الجديدة: {newCount}\n\n💡 اضغط 'تعديل' لفتح الحقول للتعديل", "إنزال الأصناف", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private ItemDataForm.ItemData LoadItemDataFromDatabase(string itemName)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM Items WHERE ItemName = @ItemName",
                    DatabaseHelper.CreateParameter("@ItemName", itemName));

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    return new ItemDataForm.ItemData
                    {
                        ItemName = row["ItemName"]?.ToString(),
                        CountryOfOrigin = row["CountryOfOrigin"]?.ToString() ?? "",
                        Units = string.IsNullOrEmpty(row["UnitsJson"]?.ToString()) ?
                            new List<ItemDataForm.ItemUnit>() :
                            JsonSerializer.Deserialize<List<ItemDataForm.ItemUnit>>(row["UnitsJson"].ToString()),
                        WeightUnits = string.IsNullOrEmpty(row["WeightUnitsJson"]?.ToString()) ?
                            new List<ItemDataForm.ItemWeightUnit>() :
                            JsonSerializer.Deserialize<List<ItemDataForm.ItemWeightUnit>>(row["WeightUnitsJson"].ToString())
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب بيانات الصنف: {ex.Message}");
            }
            return null;
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            if (!isEditingMode) { MessageBox.Show("⚠️ الرجاء الضغط على 'تعديل' أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(currentPortGroup)) { MessageBox.Show("⚠️ الرجاء اختيار مجموعة المنافذ أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (dgvData.Rows.Count == 0) { MessageBox.Show("⚠️ لا توجد بيانات للحفظ", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            try
            {
                int updatedCount = 0;
                int addedCount = 0;

                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (row.IsNewRow || row.Cells["colProductName"].Value == null) continue;
                    bool hasData = ParseDecimal(row.Cells["colCustomsPriceUSD"].Value?.ToString()) > 0 || ParseDecimal(row.Cells["colCustomsRatePct"].Value?.ToString()) > 0;
                    if (!hasData) continue;

                    string productName = row.Cells["colProductName"].Value?.ToString();
                    string unit = row.Cells["colUnit"].Value?.ToString() ?? "";
                    string weightUnit = row.Cells["colWeightUnit"].Value?.ToString() ?? "";

                    var check = DatabaseHelper.ExecuteScalar(
                        "SELECT COUNT(*) FROM ItemsPricingData WHERE ProductName = @ProductName AND PortClassification = @PortGroup",
                        DatabaseHelper.CreateParameter("@ProductName", productName),
                        DatabaseHelper.CreateParameter("@PortGroup", currentPortGroup)
                    );
                    bool exists = Convert.ToInt32(check) > 0;

                    if (exists)
                    {
                        DatabaseHelper.ExecuteNonQuery(@"UPDATE ItemsPricingData SET 
        Unit = @Unit, WeightUnit = @WeightUnit, Weight = @Weight, CustomsPriceUSD = @CustomsPriceUSD,
        ExchangeRate = @ExchangeRate, CustomsPriceRY = @CustomsPriceRY, CustomsRatePct = @CustomsRatePct,
        CustomsValue = @CustomsValue, ST_Pct = @ST_Pct, VAT_Pct = @VAT_Pct, PT_Pct = @PT_Pct,
        VF_Pct = @VF_Pct, TEF_Pct = @TEF_Pct, MFF_Pct = @MFF_Pct, TotalCustoms = @TotalCustoms,
        TotalST = @TotalST, TotalVAT = @TotalVAT, TotalPT = @TotalPT, TotalVF = @TotalVF,
        TotalTEF = @TotalTEF, TotalMFF = @TotalMFF, CountryOfOrigin = @CountryOfOrigin, ModifiedDate = datetime('now')   
        WHERE ProductName = @ProductName AND PortClassification = @PortGroup",
                                DatabaseHelper.CreateParameter("@Unit", unit),
                                DatabaseHelper.CreateParameter("@WeightUnit", weightUnit),
                                DatabaseHelper.CreateParameter("@Weight", ParseDecimal(row.Cells["colWeight"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsPriceUSD", ParseDecimal(row.Cells["colCustomsPriceUSD"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@ExchangeRate", ParseDecimal(row.Cells["colExchangeRate"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsPriceRY", ParseDecimal(row.Cells["colCustomsPriceRY"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsRatePct", ParseDecimal(row.Cells["colCustomsRatePct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsValue", ParseDecimal(row.Cells["colCustomsValue"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@ST_Pct", ParseDecimal(row.Cells["colSTPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@VAT_Pct", ParseDecimal(row.Cells["colVATPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@PT_Pct", ParseDecimal(row.Cells["colPTPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@VF_Pct", ParseDecimal(row.Cells["colVFPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TEF_Pct", ParseDecimal(row.Cells["colTEFPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@MFF_Pct", ParseDecimal(row.Cells["colMFFPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalCustoms", ParseDecimal(row.Cells["colTotalCustoms"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalST", ParseDecimal(row.Cells["colTotalST"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalVAT", ParseDecimal(row.Cells["colTotalVAT"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalPT", ParseDecimal(row.Cells["colTotalPT"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalVF", ParseDecimal(row.Cells["colTotalVF"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalTEF", ParseDecimal(row.Cells["colTotalTEF"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalMFF", ParseDecimal(row.Cells["colTotalMFF"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@ProductName", productName),
                                DatabaseHelper.CreateParameter("@CountryOfOrigin", row.Cells["colCountryOfOrigin"].Value?.ToString() ?? ""),
                                DatabaseHelper.CreateParameter("@PortGroup", currentPortGroup)
                            );
                        updatedCount++;
                    }
                    else
                    {
                        DatabaseHelper.ExecuteNonQuery(@"INSERT INTO ItemsPricingData (
        PortClassification, PortName, ProductName, Unit, WeightUnit, Weight, CustomsPriceUSD,
        ExchangeRate, CustomsPriceRY, CustomsRatePct, CustomsValue, ST_Pct, VAT_Pct, PT_Pct,
        VF_Pct, TEF_Pct, MFF_Pct, TotalCustoms, TotalST, TotalVAT, TotalPT, TotalVF, TotalTEF, TotalMFF, CountryOfOrigin, CreatedDate, ModifiedDate)
        VALUES (@PortGroup, @PortName, @ProductName, @Unit, @WeightUnit, @Weight, @CustomsPriceUSD,
        @ExchangeRate, @CustomsPriceRY, @CustomsRatePct, @CustomsValue, @ST_Pct, @VAT_Pct, @PT_Pct,
        @VF_Pct, @TEF_Pct, @MFF_Pct, @TotalCustoms, @TotalST, @TotalVAT, @TotalPT, @TotalVF, @TotalTEF, @TotalMFF, @CountryOfOrigin, datetime('now'), datetime('now'))",
                                DatabaseHelper.CreateParameter("@PortGroup", currentPortGroup),
                                DatabaseHelper.CreateParameter("@PortName", cmbPortName.Text ?? ""),
                                DatabaseHelper.CreateParameter("@ProductName", productName),
                                DatabaseHelper.CreateParameter("@Unit", unit),
                                DatabaseHelper.CreateParameter("@WeightUnit", weightUnit),
                                DatabaseHelper.CreateParameter("@Weight", ParseDecimal(row.Cells["colWeight"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsPriceUSD", ParseDecimal(row.Cells["colCustomsPriceUSD"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@ExchangeRate", ParseDecimal(row.Cells["colExchangeRate"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsPriceRY", ParseDecimal(row.Cells["colCustomsPriceRY"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsRatePct", ParseDecimal(row.Cells["colCustomsRatePct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CustomsValue", ParseDecimal(row.Cells["colCustomsValue"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@ST_Pct", ParseDecimal(row.Cells["colSTPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@VAT_Pct", ParseDecimal(row.Cells["colVATPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@PT_Pct", ParseDecimal(row.Cells["colPTPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@VF_Pct", ParseDecimal(row.Cells["colVFPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TEF_Pct", ParseDecimal(row.Cells["colTEFPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@MFF_Pct", ParseDecimal(row.Cells["colMFFPct"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalCustoms", ParseDecimal(row.Cells["colTotalCustoms"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalST", ParseDecimal(row.Cells["colTotalST"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalVAT", ParseDecimal(row.Cells["colTotalVAT"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalPT", ParseDecimal(row.Cells["colTotalPT"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalVF", ParseDecimal(row.Cells["colTotalVF"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalTEF", ParseDecimal(row.Cells["colTotalTEF"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@TotalMFF", ParseDecimal(row.Cells["colTotalMFF"].Value?.ToString())),
                                DatabaseHelper.CreateParameter("@CountryOfOrigin", row.Cells["colCountryOfOrigin"].Value?.ToString() ?? "")
                            );
                        addedCount++;
                    }
                }

                LoadPricingDataFromDatabase();
                LoadDataToGrid();

                MessageBox.Show($"✅ تم الحفظ بنجاح في قاعدة البيانات\n🏛️ مجموعة المنافذ: {currentPortGroup}\n\n📝 تم التحديث: {updatedCount} سجل\n➕ تمت الإضافة: {addedCount} سجل", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                SetControlsState(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateControlsManually()
        {
            this.Text = LanguageManager.Get("items_form_title");
            this.Size = new Size(1500, 800);
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            toolStrip1 = new ToolStrip();
            toolStrip1.RightToLeft = RightToLeft.Yes;
            toolStrip1.BackColor = Color.FromArgb(52, 73, 94);
            toolStrip1.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            btnHome = new ToolStripButton(LanguageManager.Get("btn_home"));
            btnHome.ForeColor = Color.White;
            btnHome.Tag = "btn_home";
            btnHome.Click += btnHome_Click;
            toolStripSeparatorHome = new ToolStripSeparator();

            btnSave = new ToolStripButton(LanguageManager.Get("btn_save"));
            btnSave.ForeColor = Color.White;
            btnSave.Tag = "btn_save";
            btnSave.Click += btnSave_Click;

            btnNew = new ToolStripButton(LanguageManager.Get("btn_new"));
            btnNew.ForeColor = Color.White;
            btnNew.Tag = "btn_new";
            btnNew.Click += btnNew_Click;
            toolStripSeparator1 = new ToolStripSeparator();

            btnEdit = new ToolStripButton(LanguageManager.Get("btn_edit"));
            btnEdit.ForeColor = Color.White;
            btnEdit.Tag = "btn_edit";
            btnEdit.Click += btnEdit_Click;

            btnDelete = new ToolStripButton(LanguageManager.Get("btn_delete"));
            btnDelete.ForeColor = Color.White;
            btnDelete.Tag = "btn_delete";
            btnDelete.Click += btnDelete_Click;
            toolStripSeparator2 = new ToolStripSeparator();

            btnSearch = new ToolStripButton(LanguageManager.Get("btn_search"));
            btnSearch.ForeColor = Color.White;
            btnSearch.Tag = "btn_search";
            btnSearch.Click += btnSearch_Click;
            toolStripSeparator3 = new ToolStripSeparator();

            btnExport = new ToolStripButton(LanguageManager.Get("btn_export"));
            btnExport.ForeColor = Color.White;
            btnExport.Tag = "btn_export";
            btnExport.Click += btnExport_Click;
            toolStripSeparator4 = new ToolStripSeparator();

            btnExit = new ToolStripButton(LanguageManager.Get("btn_exit"));
            btnExit.ForeColor = Color.White;
            btnExit.Tag = "btn_exit";
            btnExit.Click += (s, ev) => this.Close();

            toolStrip1.Items.AddRange(new ToolStripItem[] {
                btnHome, toolStripSeparatorHome, btnSave, btnNew, toolStripSeparator1,
                btnEdit, btnDelete, toolStripSeparator2, btnSearch, toolStripSeparator3,
                btnExport, toolStripSeparator4, btnExit
            });

            Panel pnlFilter = new Panel();
            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Height = 110;
            pnlFilter.BackColor = Color.FromArgb(240, 248, 255);
            pnlFilter.Padding = new Padding(10);

            lblPortGroup = new Label();
            lblPortGroup.Text = "مجموعة المنافذ:";
            lblPortGroup.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblPortGroup.Location = new Point(1150, 15);
            lblPortGroup.Size = new Size(120, 25);
            lblPortGroup.TextAlign = ContentAlignment.MiddleRight;
            lblPortGroup.Tag = "port_group";

            cmbPortGroup = new ComboBox();
            cmbPortGroup.DropDownStyle = ComboBoxStyle.DropDown;
            cmbPortGroup.Font = new Font("Tahoma", 10);
            cmbPortGroup.Location = new Point(900, 15);
            cmbPortGroup.Size = new Size(240, 25);
            cmbPortGroup.TextChanged += CmbPortGroup_TextChanged;
            cmbPortGroup.KeyDown += (s, ev) =>
            {
                if (ev.KeyCode == Keys.F9)
                {
                    ev.SuppressKeyPress = true;
                    OpenPortGroupSelectionForm();
                }
            };

            lblPortName = new Label();
            lblPortName.Text = LanguageManager.Get("port_name") + " (للعرض فقط)";
            lblPortName.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lblPortName.ForeColor = Color.Gray;
            lblPortName.Location = new Point(1150, 50);
            lblPortName.Size = new Size(150, 25);
            lblPortName.TextAlign = ContentAlignment.MiddleRight;
            lblPortName.Tag = "port_name";

            cmbPortName = new ComboBox();
            cmbPortName.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPortName.Font = new Font("Tahoma", 10);
            cmbPortName.Location = new Point(900, 50);
            cmbPortName.Size = new Size(240, 25);
            cmbPortName.Enabled = false;
            cmbPortName.BackColor = Color.FromArgb(240, 240, 240);

            btnLoadItems = new Button();
            btnLoadItems.Text = LanguageManager.Get("btn_load_items");
            btnLoadItems.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnLoadItems.BackColor = Color.FromArgb(33, 150, 243);
            btnLoadItems.ForeColor = Color.White;
            btnLoadItems.FlatStyle = FlatStyle.Flat;
            btnLoadItems.Location = new Point(700, 15);
            btnLoadItems.Size = new Size(140, 30);
            btnLoadItems.Tag = "btn_load_items";
            btnLoadItems.Click += btnLoadItems_Click;

            btnSaveData = new Button();
            btnSaveData.Text = LanguageManager.Get("btn_save_data");
            btnSaveData.Font = new Font("Tahoma", 10, FontStyle.Bold);
            btnSaveData.BackColor = Color.FromArgb(76, 175, 80);
            btnSaveData.ForeColor = Color.White;
            btnSaveData.FlatStyle = FlatStyle.Flat;
            btnSaveData.Location = new Point(700, 55);
            btnSaveData.Size = new Size(140, 30);
            btnSaveData.Tag = "btn_save_data";
            btnSaveData.Click += btnSaveData_Click;

            lblStatus = new Label();
            lblStatus.Text = "💡 اختر مجموعة المنافذ (اضغط F9 للاختيار)";
            lblStatus.Font = new Font("Tahoma", 9, FontStyle.Bold);
            lblStatus.ForeColor = Color.Blue;
            lblStatus.Location = new Point(500, 90);
            lblStatus.Size = new Size(600, 25);
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            lblStatus.Tag = "status_select_group";

            pnlFilter.Controls.Add(lblPortGroup);
            pnlFilter.Controls.Add(cmbPortGroup);
            pnlFilter.Controls.Add(lblPortName);
            pnlFilter.Controls.Add(cmbPortName);
            pnlFilter.Controls.Add(btnLoadItems);
            pnlFilter.Controls.Add(btnSaveData);
            pnlFilter.Controls.Add(lblStatus);

            dgvData = new DataGridView();
            dgvData.Dock = DockStyle.Fill;

            this.Controls.Add(dgvData);
            this.Controls.Add(pnlFilter);
            this.Controls.Add(toolStrip1);
        }

        private bool CheckUnsavedChanges()
        {
            if (isEditingMode && hasChanges)
            {
                DialogResult result = MessageBox.Show("⚠️ لديك تغييرات غير محفوظة!\n\n• اضغط 'نعم' لحفظ التغييرات\n• اضغط 'لا' للتجاهل والخروج\n• اضغط 'إلغاء' للبقاء والتعديل", "تأكيد الخروج", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    btnSaveData_Click(this, EventArgs.Empty);
                    return !hasChanges;
                }
                else if (result == DialogResult.Cancel)
                {
                    return false;
                }
                hasChanges = false;
            }
            return true;
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            if (!CheckUnsavedChanges()) return;
            try
            {
                MainForm mainForm = this.MdiParent as MainForm ?? Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm != null && !mainForm.IsDisposed)
                {
                    foreach (Form child in mainForm.MdiChildren.ToList())
                        if (!child.IsDisposed && child != this) child.Close();
                    this.Close();
                    mainForm.ShowWelcome();
                    mainForm.WindowState = FormWindowState.Maximized;
                    mainForm.Activate();
                    mainForm.BringToFront();
                }
                else this.Close();
            }
            catch (Exception ex) { MessageBox.Show($"{LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error); this.Close(); }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count == 0) { MessageBox.Show("⚠️ لا توجد بيانات للتعديل\n\n💡 اضغط 'تحميل الأصناف' أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SetControlsState(true);
            lblStatus.Text = "✏️ وضع التعديل مفعل - عدّل البيانات ثم اضغط 'حفظ البيانات'";
            lblStatus.ForeColor = Color.DarkOrange;
        }

        private void btnSave_Click(object sender, EventArgs e) => btnSaveData_Click(sender, e);

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvData.SelectedRows.Count == 0) { MessageBox.Show(LanguageManager.Get("msg_select_row"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (string.IsNullOrEmpty(currentPortGroup)) { MessageBox.Show("⚠️ الرجاء اختيار مجموعة المنافذ أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }

            string productName = dgvData.SelectedRows[0].Cells["colProductName"].Value?.ToString();
            DialogResult result = MessageBox.Show($"🗑️ هل أنت متأكد من حذف الصنف '{productName}' من مجموعة {currentPortGroup}؟", LanguageManager.Get("msg_confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    DatabaseHelper.ExecuteNonQuery("DELETE FROM ItemsPricingData WHERE PortClassification = @PortGroup AND ProductName = @ProductName",
                        DatabaseHelper.CreateParameter("@PortGroup", currentPortGroup),
                        DatabaseHelper.CreateParameter("@ProductName", productName));

                    LoadPricingDataFromDatabase();
                    LoadDataToGrid();
                    MessageBox.Show($"✅ تم حذف الصنف '{productName}' من مجموعة {currentPortGroup}", LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"❌ خطأ في الحذف: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private decimal ParseDecimal(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return 0;
            text = text.Replace(',', '.');
            return decimal.TryParse(text, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal val) ? val : 0;
        }

        // ==================== ✅ جلب نطاق سعر الصرف لمجموعة المنافذ ====================
        private (decimal minRate, decimal maxRate, decimal defaultRate) GetPortGroupRateLimits(string portGroup)
        {
            try
            {
                // جلب الحد الأدنى والأعلى من جميع المنافذ التابعة لنفس مجموعة المنافذ
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    @"SELECT MIN(MinRate) AS MinR, MAX(MaxRate) AS MaxR, AVG(ExchangeRateUSD) AS AvgRate 
              FROM Ports 
              WHERE Classification = @Group AND MinRate > 0 AND MaxRate > 0",
                    DatabaseHelper.CreateParameter("@Group", portGroup));

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow r = dt.Rows[0];
                    decimal min = r["MinR"] != DBNull.Value ? Convert.ToDecimal(r["MinR"]) : 0;
                    decimal max = r["MaxR"] != DBNull.Value ? Convert.ToDecimal(r["MaxR"]) : 0;
                    decimal avg = r["AvgRate"] != DBNull.Value ? Convert.ToDecimal(r["AvgRate"]) : 0;

                    // إذا لم تكن هناك حدود محددة، استخدم القيم الافتراضية
                    if (min == 0 || max == 0)
                    {
                        if (portGroup.Contains("داخلي") || portGroup.Contains("شمال"))
                        {
                            min = 530; max = 540; if (avg == 0) avg = 535;
                        }
                        else
                        {
                            min = 740; max = 760; if (avg == 0) avg = 750;
                        }
                    }

                    return (min, max, avg);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ في جلب نطاق سعر الصرف: {ex.Message}");
            }

            // قيم افتراضية احتياطية
            if (portGroup.Contains("داخلي") || portGroup.Contains("شمال"))
                return (530, 540, 535);
            return (740, 760, 750);
        }

        private void ClearFields()
        {
            try
            {
                if (cmbPortGroup != null) cmbPortGroup.Text = "";
                if (cmbPortName != null) { cmbPortName.Items.Clear(); cmbPortName.Enabled = false; }
                if (dgvData != null) dgvData.Rows.Clear();
                lblStatus.Text = "💡 اختر مجموعة المنافذ (اضغط F9 للاختيار)";
                lblStatus.ForeColor = Color.Blue;
                currentPortGroup = "";
                isEditingMode = false;
                hasChanges = false;
            }
            catch (Exception ex) { MessageBox.Show($"{LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentPortGroup)) { MessageBox.Show("⚠️ الرجاء اختيار مجموعة المنافذ أولاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            ClearFields();
            SetControlsState(true);
            dgvData.Rows.Clear();
            lblStatus.Text = $"🆕 وضع إضافة صنف جديد لـ {currentPortGroup} - اضغط 'تحميل الأصناف' ثم عدّل البيانات";
            lblStatus.ForeColor = Color.DarkOrange;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count == 0)
            {
                MessageBox.Show(LanguageManager.Get("msg_no_data_search"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form viewForm = new Form())
            {
                viewForm.Text = "📋 عرض جميع الأصناف للبحث";
                viewForm.Size = new Size(900, 500);
                viewForm.StartPosition = FormStartPosition.CenterParent;
                viewForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                viewForm.MaximizeBox = false;
                viewForm.MinimizeBox = false;
                viewForm.RightToLeft = RightToLeft.Yes;
                viewForm.RightToLeftLayout = true;

                Panel pnlTop = new Panel();
                pnlTop.Height = 60;
                pnlTop.Dock = DockStyle.Top;
                pnlTop.BackColor = Color.FromArgb(240, 248, 255);

                Label lblSearch = new Label();
                lblSearch.Text = "🔍 بحث:";
                lblSearch.Font = new Font("Tahoma", 10, FontStyle.Bold);
                lblSearch.Location = new Point(750, 15);
                lblSearch.Size = new Size(60, 25);
                lblSearch.TextAlign = ContentAlignment.MiddleRight;

                TextBox txtSearch = new TextBox();
                txtSearch.Font = new Font("Tahoma", 11);
                txtSearch.Location = new Point(450, 15);
                txtSearch.Size = new Size(290, 28);
                txtSearch.TextAlign = HorizontalAlignment.Right;
                txtSearch.BorderStyle = BorderStyle.FixedSingle;

                pnlTop.Controls.Add(lblSearch);
                pnlTop.Controls.Add(txtSearch);

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

                dgvView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                dgvView.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                dgvView.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                dgvView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dgvView.ColumnHeadersHeight = 40;
                dgvView.RowTemplate.Height = 35;
                dgvView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

                dgvView.Columns.Add("colId", "م");
                dgvView.Columns.Add("colProductName", "اسم السلعة");
                dgvView.Columns.Add("colCountryOfOrigin", "بلد المنشأ");
                dgvView.Columns.Add("colWeight", "الوزن");
                dgvView.Columns.Add("colUnit", "الوحدة");

                dgvView.Columns["colId"].Width = 50;
                dgvView.Columns["colProductName"].Width = 400;
                dgvView.Columns["colWeight"].Width = 100;
                dgvView.Columns["colUnit"].Width = 100;

                foreach (DataGridViewRow row in dgvData.Rows)
                {
                    if (row.IsNewRow) continue;
                    dgvView.Rows.Add(
                        row.Cells["colId"].Value,
                        row.Cells["colProductName"].Value,
                        row.Cells["colCountryOfOrigin"].Value ?? "",
                        row.Cells["colWeight"].Value,
                        row.Cells["colUnit"].Value
                    );
                }

                txtSearch.TextChanged += (s, ev) =>
                {
                    string searchTerm = txtSearch.Text.Trim().ToLower();
                    foreach (DataGridViewRow row in dgvView.Rows)
                    {
                        if (row.IsNewRow) continue;

                        string name = row.Cells["colProductName"].Value?.ToString()?.ToLower() ?? "";
                        string id = row.Cells["colId"].Value?.ToString()?.ToLower() ?? "";

                        if (string.IsNullOrEmpty(searchTerm) || name.Contains(searchTerm) || id.Contains(searchTerm))
                        {
                            row.Visible = true;
                        }
                        else
                        {
                            row.Visible = false;
                        }
                    }
                };

                Panel pnlBottom = new Panel();
                pnlBottom.Height = 60;
                pnlBottom.Dock = DockStyle.Bottom;
                pnlBottom.BackColor = Color.FromArgb(240, 248, 255);

                Button btnClose = new Button();
                btnClose.Text = " إغلاق";
                btnClose.Font = new Font("Tahoma", 10, FontStyle.Bold);
                btnClose.BackColor = Color.FromArgb(158, 158, 158);
                btnClose.ForeColor = Color.White;
                btnClose.FlatStyle = FlatStyle.Flat;
                btnClose.Size = new Size(120, 40);
                btnClose.Location = new Point((pnlBottom.Width - btnClose.Width) / 2, 10);
                btnClose.Anchor = AnchorStyles.Bottom;
                btnClose.Click += (s, ev) => viewForm.Close();

                pnlBottom.Controls.Add(btnClose);

                viewForm.Controls.Add(dgvView);
                viewForm.Controls.Add(pnlTop);
                viewForm.Controls.Add(pnlBottom);

                dgvView.CellDoubleClick += (s, ev) =>
                {
                    if (ev.RowIndex >= 0)
                    {
                        string selectedId = dgvView.Rows[ev.RowIndex].Cells["colId"].Value?.ToString();

                        foreach (DataGridViewRow mainRow in dgvData.Rows)
                        {
                            if (mainRow.Cells["colId"].Value?.ToString() == selectedId)
                            {
                                dgvData.ClearSelection();
                                mainRow.Selected = true;
                                dgvData.FirstDisplayedScrollingRowIndex = mainRow.Index;
                                viewForm.Close();
                                break;
                            }
                        }
                    }
                };

                viewForm.ShowDialog();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (dgvData.Rows.Count == 0) { MessageBox.Show(LanguageManager.Get("msg_no_data_export"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            SaveFileDialog sfd = new SaveFileDialog() { Filter = "CSV ملف|*.csv", FileName = $"تقرير_بيانات_الاصناف_{DateTime.Now:yyyyMMdd_HHmmss}.csv" };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (System.IO.StreamWriter sw = new System.IO.StreamWriter(sfd.FileName, false, System.Text.Encoding.UTF8))
                    {
                        for (int i = 0; i < dgvData.Columns.Count; i++) { sw.Write(dgvData.Columns[i].HeaderText); if (i < dgvData.Columns.Count - 1) sw.Write(","); }
                        sw.WriteLine();
                        foreach (DataGridViewRow row in dgvData.Rows)
                        {
                            if (row.IsNewRow) continue;
                            for (int i = 0; i < dgvData.Columns.Count; i++)
                            {
                                string value = row.Cells[i].Value?.ToString() ?? "";
                                if (value.Contains(",")) value = $"\"{value}\"";
                                sw.Write(value);
                                if (i < dgvData.Columns.Count - 1) sw.Write(",");
                            }
                            sw.WriteLine();
                        }
                    }
                    MessageBox.Show(string.Format(LanguageManager.Get("msg_exported"), dgvData.Rows.Count), LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show($"{LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"), MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }
        }

        private void LoadDataToGrid()
        {
            if (string.IsNullOrEmpty(currentPortGroup))
            {
                dgvData.Rows.Clear();
                lblStatus.Text = "💡 اختر مجموعة المنافذ (اضغط F9 للاختيار)";
                lblStatus.ForeColor = Color.Orange;
                return;
            }
            LoadRecordsByPortGroup(currentPortGroup);
            UpdateStatusLabel();
        }

        private void LoadRecordsByPortGroup(string portGroup)
        {
            var groupedRecords = records.Where(r => r.PortGroup == portGroup).GroupBy(r => r.ProductName).Select(g => g.First()).OrderBy(r => r.ProductName).ToList();
            dgvData.Rows.Clear();

            if (groupedRecords.Count == 0)
            {
                lblStatus.Text = $"📭 لا توجد بيانات محفوظة لـ {portGroup} - اضغط 'إنزال الأصناف' لإضافة بيانات";
                lblStatus.ForeColor = Color.Orange;
                return;
            }

            int idx = 1;
            foreach (var r in groupedRecords) AddRecordToGrid(r, idx++);
        }

        private void AddRecordToGrid(ItemsRecord r, int idx)
        {
            dgvData.Rows.Add(idx, r.ProductName, r.CountryOfOrigin ?? "", r.Unit, r.WeightUnit, r.Weight, r.CustomsPriceUSD, r.ExchangeRate, r.CustomsPriceRY, r.CustomsRatePct,
                r.CustomsValue, r.ST_Pct, r.VAT_Pct, r.PT_Pct, r.VF_Pct, r.TEF_Pct, r.MFF_Pct,
                r.TotalCustoms, r.TotalST, r.TotalVAT, r.TotalPT, r.TotalVF, r.TotalTEF, r.TotalMFF);
        }

        private void UpdateStatusLabel()
        {
            if (dgvData.Rows.Count == 0) return;
            lblStatus.Text = $"✅ تم تحميل {dgvData.Rows.Count} صنف لـ {currentPortGroup} - اضغط 'تعديل' للتعديل";
            lblStatus.ForeColor = Color.Green;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!CheckUnsavedChanges()) { e.Cancel = true; return; }
            base.OnFormClosing(e);
        }
    }
}