using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class PortsForm : Form
    {
        private List<PortData> _allPorts = new List<PortData>();
        private int _currentPortIndex = -1;
        private bool _isNavigating = false;
        private PortsViewForm viewForm = null;

        public PortsForm()
        {
            InitializeComponent();
            SetupAuditGrid();
            LoadPortGroupsToComboBox(); // ✅ تحميل مجموعات المنافذ أولاً
            LoadAllPortsData();
            LoadAuditLog();
            ApplyTranslations();

            SetFieldsReadOnly(true);
            this.FormClosing += PortsForm_FormClosing;
            LanguageManager.LanguageChanged += (s, e) => RefreshAllTranslations();
        }

        // ==================== ✅ تحميل مجموعات المنافذ إلى الـ ComboBox ====================
        private void LoadPortGroupsToComboBox()
        {
            cmbPortGroup.Items.Clear();
            cmbPortGroup.Items.Add("-- اختر مجموعة المنافذ --");
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT GroupName FROM PortGroups ORDER BY GroupName");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        cmbPortGroup.Items.Add(row["GroupName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"خطأ في تحميل مجموعات المنافذ: {ex.Message}");
            }
            cmbPortGroup.SelectedIndex = 0;
        }

        private void SetFieldsReadOnly(bool readOnly)
        {
            txtPortNumber.ReadOnly = true;
            txtPortName.ReadOnly = readOnly;
            txtPortName.BackColor = readOnly ? Color.FromArgb(245, 245, 245) : Color.White;

            radioLand.Enabled = !readOnly;
            radioAir.Enabled = !readOnly;
            radioSea.Enabled = !readOnly;
            if (comboPortType != null) comboPortType.Enabled = !readOnly;
            if (comboCurrency != null) comboCurrency.Enabled = !readOnly;

            // ✅ تفعيل/تعطيل ComboBox مجموعة المنافذ
            if (cmbPortGroup != null) cmbPortGroup.Enabled = !readOnly;

            if (txtExchangeRateUSD != null)
            {
                txtExchangeRateUSD.ReadOnly = readOnly;
                txtExchangeRateUSD.BackColor = readOnly ? Color.FromArgb(245, 245, 245) : Color.FromArgb(255, 248, 225);
            }
            if (txtMinRate != null)
            {
                txtMinRate.ReadOnly = readOnly;
                txtMinRate.BackColor = readOnly ? Color.FromArgb(245, 245, 245) : Color.FromArgb(232, 245, 233);
            }
            if (txtMaxRate != null)
            {
                txtMaxRate.ReadOnly = readOnly;
                txtMaxRate.BackColor = readOnly ? Color.FromArgb(245, 245, 245) : Color.FromArgb(255, 235, 238);
            }
        }

        private string FormatNumber(decimal value, int decimals = 2)
        {
            return value.ToString($"N{decimals}", CultureInfo.InvariantCulture);
        }

        public class PortData
        {
            public int PortId { get; set; }
            public string Name { get; set; }
            public string Nature { get; set; }
            public string PortType { get; set; }
            public string Classification { get; set; } // سيحتوي الآن على اسم مجموعة المنافذ
            public string Rate { get; set; }
            public string Currency { get; set; }
            public string ExchangeRateType { get; set; }
            public string UserNumber { get; set; }
            public string ModifiedDate { get; set; }
            public decimal ExchangeRateUSD { get; set; }
            public decimal MinRate { get; set; }
            public decimal MaxRate { get; set; }
        }

        private void ApplyTranslations()
        {
            this.Text = LanguageManager.Get("ports_form_title");
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;

            if (grpPortData != null) grpPortData.Text = LanguageManager.Get("grp_port_data");
            if (grpClassification != null) grpClassification.Text = "🗂️ التصنيف (مجموعة المنافذ)";
            if (grpNature != null) grpNature.Text = LanguageManager.Get("grp_nature");
            if (grpRate != null) grpRate.Text = LanguageManager.Get("grp_rate");
            if (grpPayment != null) grpPayment.Text = LanguageManager.Get("grp_payment");

            if (lblPortNumber != null) lblPortNumber.Text = LanguageManager.Get("lbl_port_number");
            if (lblPortName != null) lblPortName.Text = LanguageManager.Get("lbl_port_name");
            if (lblPortAddress != null) lblPortAddress.Text = LanguageManager.Get("lbl_port_address");
            if (lblPortType != null) lblPortType.Text = LanguageManager.Get("lbl_port_type");
            if (lblCurrency != null) lblCurrency.Text = LanguageManager.Get("lbl_currency");

            if (radioLand != null) radioLand.Text = LanguageManager.Get("radio_land");
            if (radioAir != null) radioAir.Text = LanguageManager.Get("radio_air");
            if (radioSea != null) radioSea.Text = LanguageManager.Get("radio_sea");
            if (radioCash != null) radioCash.Text = LanguageManager.Get("radio_cash");
            if (radioCheck != null) radioCheck.Text = LanguageManager.Get("radio_check");

            if (lblExchangeRateUSD != null) lblExchangeRateUSD.Text = "💵 سعر الصرف مقابل الدولار:";
            if (lblMinRate != null) lblMinRate.Text = "⬇️ الحد الأدنى:";
            if (lblMaxRate != null) lblMaxRate.Text = "⬆️ الحد الأعلى:";

            UpdateComboBoxItems();
            UpdateToolStripButtons();
            UpdateAuditGridHeaders();
        }

        private void RefreshAllTranslations()
        {
            ApplyTranslations();
            LoadAuditLog();
        }

        private void UpdateToolStripButtons()
        {
            if (toolStrip1 == null) return;
            foreach (ToolStripItem item in toolStrip1.Items)
            {
                if (item is ToolStripButton btn)
                {
                    switch (btn.Name)
                    {
                        case "btnHome": btn.Text = "🏠\n" + LanguageManager.Get("btn_home"); break;
                        case "btnNew": btn.Text = "➕\n" + LanguageManager.Get("btn_new"); break;
                        case "btnSave": btn.Text = "💾\n" + LanguageManager.Get("btn_save"); break;
                        case "btnEdit": btn.Text = "✏️\n" + LanguageManager.Get("btn_edit"); break;
                        case "btnDelete": btn.Text = "🗑️\n" + LanguageManager.Get("btn_delete"); break;
                        case "btnView": btn.Text = "👁️\n" + LanguageManager.Get("btn_view"); break;
                        case "btnSearch": btn.Text = "🔍\n" + LanguageManager.Get("btn_search"); break;
                        case "btnPrint": btn.Text = "🖨️\n" + LanguageManager.Get("btn_print"); break;
                        case "btnPrevious": btn.Text = "⬅️\n" + LanguageManager.Get("btn_previous"); break;
                        case "btnNext": btn.Text = "➡️\n" + LanguageManager.Get("btn_next"); break;
                        case "btnExit": btn.Text = "❌\n" + LanguageManager.Get("btn_exit"); break;
                    }
                }
            }
        }

        private void UpdateComboBoxItems()
        {
            if (comboPortType != null)
            {
                var selectedType = comboPortType.SelectedItem?.ToString();
                comboPortType.Items.Clear();
                comboPortType.Items.Add(LanguageManager.Get("port_type_main"));
                comboPortType.Items.Add(LanguageManager.Get("port_type_supervisory"));
                if (!string.IsNullOrEmpty(selectedType))
                {
                    if (selectedType == "رئيسي") comboPortType.SelectedItem = LanguageManager.Get("port_type_main");
                    else if (selectedType == "رقابي") comboPortType.SelectedItem = LanguageManager.Get("port_type_supervisory");
                }
                else comboPortType.SelectedIndex = 0;
            }

            if (comboCurrency != null)
            {
                var selectedCurrency = comboCurrency.SelectedItem?.ToString();
                comboCurrency.Items.Clear();
                comboCurrency.Items.Add(LanguageManager.Get("currency_yer"));
                comboCurrency.Items.Add(LanguageManager.Get("currency_yer_south"));
                if (!string.IsNullOrEmpty(selectedCurrency))
                {
                    if (selectedCurrency == "ريال يمني") comboCurrency.SelectedItem = LanguageManager.Get("currency_yer");
                    else if (selectedCurrency == "ريال جنوبي") comboCurrency.SelectedItem = LanguageManager.Get("currency_yer_south");
                }
                else comboCurrency.SelectedIndex = 0;
            }
        }

        private void UpdateAuditGridHeaders()
        {
            if (dgvPorts == null || dgvPorts.Columns.Count == 0) return;
            if (dgvPorts.Columns["PortName"] != null) dgvPorts.Columns["PortName"].HeaderText = LanguageManager.Get("col_port_name");
            if (dgvPorts.Columns["PortNature"] != null) dgvPorts.Columns["PortNature"].HeaderText = LanguageManager.Get("col_port_nature");
            if (dgvPorts.Columns["PortType"] != null) dgvPorts.Columns["PortType"].HeaderText = LanguageManager.Get("col_port_type");
            if (dgvPorts.Columns["Classification"] != null) dgvPorts.Columns["Classification"].HeaderText = "مجموعة المنافذ";
            if (dgvPorts.Columns["ExchangeRate"] != null) dgvPorts.Columns["ExchangeRate"].HeaderText = LanguageManager.Get("col_exchange_rate");
            if (dgvPorts.Columns["Currency"] != null) dgvPorts.Columns["Currency"].HeaderText = LanguageManager.Get("col_currency");
            if (dgvPorts.Columns["ExchangeRateType"] != null) dgvPorts.Columns["ExchangeRateType"].HeaderText = LanguageManager.Get("col_exchange_rate_type");
            if (dgvPorts.Columns["UserNumber"] != null) dgvPorts.Columns["UserNumber"].HeaderText = LanguageManager.Get("col_user");
            if (dgvPorts.Columns["ModifiedDate"] != null) dgvPorts.Columns["ModifiedDate"].HeaderText = LanguageManager.Get("col_modified_date");
            if (dgvPorts.Columns["OperationType"] != null) dgvPorts.Columns["OperationType"].HeaderText = LanguageManager.Get("col_operation_type");
        }

        private void SetupAuditGrid()
        {
            dgvPorts.Columns.Clear();
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "PortName", HeaderText = LanguageManager.Get("col_port_name"), Width = 200 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "PortNature", HeaderText = LanguageManager.Get("col_port_nature"), Width = 100 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "PortType", HeaderText = LanguageManager.Get("col_port_type"), Width = 100 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Classification", HeaderText = "مجموعة المنافذ", Width = 200 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ExchangeRate", HeaderText = LanguageManager.Get("col_exchange_rate"), Width = 120 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Currency", HeaderText = LanguageManager.Get("col_currency"), Width = 120 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ExchangeRateType", HeaderText = LanguageManager.Get("col_exchange_rate_type"), Width = 200 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "UserNumber", HeaderText = LanguageManager.Get("col_user"), Width = 70 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "ModifiedDate", HeaderText = LanguageManager.Get("col_modified_date"), Width = 150 });
            dgvPorts.Columns.Add(new DataGridViewTextBoxColumn { Name = "OperationType", HeaderText = LanguageManager.Get("col_operation_type"), Width = 100 });

            dgvPorts.EnableHeadersVisualStyles = false;
            dgvPorts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvPorts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvPorts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvPorts.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            dgvPorts.RowsDefaultCellStyle.BackColor = Color.White;
            dgvPorts.RowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(33, 150, 243);
            dgvPorts.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            dgvPorts.BackgroundColor = Color.White;
            dgvPorts.BorderStyle = BorderStyle.None;
            dgvPorts.RowHeadersVisible = false;
            dgvPorts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPorts.AllowUserToAddRows = false;
            dgvPorts.ReadOnly = true;
            dgvPorts.Dock = DockStyle.Fill;
            dgvPorts.RowTemplate.Height = 35;
            dgvPorts.RightToLeft = LanguageManager.RightToLeft;
        }

        public void LoadAllPortsData()
        {
            _allPorts.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Ports ORDER BY PortName");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        _allPorts.Add(new PortData
                        {
                            PortId = Convert.ToInt32(row["PortId"]),
                            Name = row["PortName"].ToString(),
                            Nature = row["PortNature"] != DBNull.Value ? row["PortNature"].ToString() : "",
                            PortType = row["PortType"] != DBNull.Value ? row["PortType"].ToString() : "",
                            Classification = row["Classification"] != DBNull.Value ? row["Classification"].ToString() : "",
                            Rate = row["ExchangeRate"] != DBNull.Value ? row["ExchangeRate"].ToString() : "0",
                            Currency = row["Currency"] != DBNull.Value ? row["Currency"].ToString() : "",
                            ExchangeRateType = row["ExchangeRateType"] != DBNull.Value ? row["ExchangeRateType"].ToString() : "",
                            UserNumber = row["UserNumber"] != DBNull.Value ? row["UserNumber"].ToString() : "1",
                            ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]).ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            ExchangeRateUSD = row.Table.Columns.Contains("ExchangeRateUSD") && row["ExchangeRateUSD"] != DBNull.Value ? Convert.ToDecimal(row["ExchangeRateUSD"]) : 0,
                            MinRate = row.Table.Columns.Contains("MinRate") && row["MinRate"] != DBNull.Value ? Convert.ToDecimal(row["MinRate"]) : 0,
                            MaxRate = row.Table.Columns.Contains("MaxRate") && row["MaxRate"] != DBNull.Value ? Convert.ToDecimal(row["MaxRate"]) : 0
                        });
                    }

                    if (_allPorts.Count > 0)
                    {
                        _currentPortIndex = 0;
                        DisplayPortData(_allPorts[0]);
                    }
                }
                else
                {
                    LoadDefaultPorts();
                    foreach (var port in _allPorts) SavePortToDatabase(port, "إضافة");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في تحميل المنافذ:\n{ex.Message}", "خطأ");
                LoadDefaultPorts();
            }
        }

        private void LoadAuditLog()
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM PortsAuditLog ORDER BY ModifiedDate DESC");
                if (dt != null && dt.Rows.Count > 0)
                {
                    dgvPorts.Rows.Clear();
                    foreach (DataRow row in dt.Rows)
                    {
                        string operationType = row["OperationType"].ToString();
                        if (operationType == "إضافة") operationType = LanguageManager.Get("action_add");
                        else if (operationType == "تعديل") operationType = LanguageManager.Get("action_edit");
                        else if (operationType == "حذف") operationType = LanguageManager.Get("action_delete");

                        dgvPorts.Rows.Add(
                            row["PortName"].ToString(),
                            row["PortNature"].ToString(),
                            row["PortType"].ToString(),
                            row["Classification"].ToString(),
                            row["ExchangeRate"].ToString(),
                            row["Currency"].ToString(),
                            row["ExchangeRateType"].ToString(),
                            row["UserNumber"] != DBNull.Value ? row["UserNumber"].ToString() : "1",
                            row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]).ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            operationType
                        );
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void LoadDefaultPorts()
        {
            _allPorts = new List<PortData>
            {
                new PortData { Name = "منفذ عدن البحري", Nature = "بحري", PortType = "رئيسي", Classification = "خارجي من مناطق الجنوب", Rate = "750", Currency = "ريال جنوبي", ExchangeRateType = "جنوبي (750 ريال/دولار)", UserNumber = "1", ExchangeRateUSD = 750, MinRate = 740, MaxRate = 760 },
                new PortData { Name = "منفذ المكلا البحري", Nature = "بحري", PortType = "رئيسي", Classification = "خارجي من مناطق الجنوب", Rate = "750", Currency = "ريال جنوبي", ExchangeRateType = "جنوبي (750 ريال/دولار)", UserNumber = "1", ExchangeRateUSD = 750, MinRate = 740, MaxRate = 760 },
                new PortData { Name = "منفذ صنعاء الجوي", Nature = "جوي", PortType = "رئيسي", Classification = "داخلي من مناطق الشمال", Rate = "535", Currency = "ريال يمني", ExchangeRateType = "شمالي (535 ريال/دولار)", UserNumber = "1", ExchangeRateUSD = 535, MinRate = 530, MaxRate = 540 },
                new PortData { Name = "منفذ الحديدة البحري", Nature = "بحري", PortType = "رئيسي", Classification = "داخلي من مناطق الشمال", Rate = "535", Currency = "ريال يمني", ExchangeRateType = "شمالي (535 ريال/دولار)", UserNumber = "1", ExchangeRateUSD = 535, MinRate = 530, MaxRate = 540 }
            };
        }

        private void SavePortToDatabase(PortData port, string operationType)
        {
            try
            {
                if (port.PortId > 0)
                {
                    DatabaseHelper.ExecuteNonQuery(@"UPDATE Ports SET 
                        PortName = @Name, PortNature = @Nature, PortType = @Type, 
                        Classification = @Classification, ExchangeRate = @Rate, 
                        Currency = @Currency, ExchangeRateType = @RateType,
                        ExchangeRateUSD = @RateUSD, MinRate = @MinRate, MaxRate = @MaxRate,
                        UserNumber = @User, ModifiedDate = datetime('now')
                        WHERE PortId = @PortId",
                        DatabaseHelper.CreateParameter("@Name", port.Name),
                        DatabaseHelper.CreateParameter("@Nature", port.Nature),
                        DatabaseHelper.CreateParameter("@Type", port.PortType),
                        DatabaseHelper.CreateParameter("@Classification", port.Classification),
                        DatabaseHelper.CreateParameter("@Rate", port.Rate),
                        DatabaseHelper.CreateParameter("@Currency", port.Currency),
                        DatabaseHelper.CreateParameter("@RateType", port.ExchangeRateType),
                        DatabaseHelper.CreateParameter("@RateUSD", port.ExchangeRateUSD),
                        DatabaseHelper.CreateParameter("@MinRate", port.MinRate),
                        DatabaseHelper.CreateParameter("@MaxRate", port.MaxRate),
                        DatabaseHelper.CreateParameter("@User", port.UserNumber ?? "1"),
                        DatabaseHelper.CreateParameter("@PortId", port.PortId)
                    );
                }
                else
                {
                    var newId = DatabaseHelper.ExecuteScalar(@"INSERT INTO Ports 
                        (PortName, PortNature, PortType, Classification, ExchangeRate, Currency, ExchangeRateType,
                        ExchangeRateUSD, MinRate, MaxRate, UserNumber, ModifiedDate) 
                        VALUES (@Name, @Nature, @Type, @Classification, @Rate, @Currency, @RateType,
                        @RateUSD, @MinRate, @MaxRate, @User, datetime('now'));
                        SELECT last_insert_rowid();",
                        DatabaseHelper.CreateParameter("@Name", port.Name),
                        DatabaseHelper.CreateParameter("@Nature", port.Nature),
                        DatabaseHelper.CreateParameter("@Type", port.PortType),
                        DatabaseHelper.CreateParameter("@Classification", port.Classification),
                        DatabaseHelper.CreateParameter("@Rate", port.Rate),
                        DatabaseHelper.CreateParameter("@Currency", port.Currency),
                        DatabaseHelper.CreateParameter("@RateType", port.ExchangeRateType),
                        DatabaseHelper.CreateParameter("@RateUSD", port.ExchangeRateUSD),
                        DatabaseHelper.CreateParameter("@MinRate", port.MinRate),
                        DatabaseHelper.CreateParameter("@MaxRate", port.MaxRate),
                        DatabaseHelper.CreateParameter("@User", port.UserNumber ?? "1")
                    );
                    port.PortId = Convert.ToInt32(newId);
                }

                DatabaseHelper.ExecuteNonQuery(@"INSERT INTO PortsAuditLog 
                    (PortName, PortNature, PortType, Classification, ExchangeRate, Currency, ExchangeRateType, UserNumber, ModifiedDate, OperationType) 
                    VALUES (@Name, @Nature, @Type, @Classification, @Rate, @Currency, @RateType, @User, datetime('now'), @OpType)",
                    DatabaseHelper.CreateParameter("@Name", port.Name),
                    DatabaseHelper.CreateParameter("@Nature", port.Nature),
                    DatabaseHelper.CreateParameter("@Type", port.PortType),
                    DatabaseHelper.CreateParameter("@Classification", port.Classification),
                    DatabaseHelper.CreateParameter("@Rate", port.Rate),
                    DatabaseHelper.CreateParameter("@Currency", port.Currency),
                    DatabaseHelper.CreateParameter("@RateType", port.ExchangeRateType),
                    DatabaseHelper.CreateParameter("@User", port.UserNumber ?? "1"),
                    DatabaseHelper.CreateParameter("@OpType", operationType)
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ المنفذ:\n{ex.Message}", "خطأ");
                throw;
            }
        }

        private void DisplayPortData(PortData port)
        {
            _isNavigating = true;
            txtPortNumber.Text = (_currentPortIndex + 1).ToString();
            txtPortName.Text = port.Name;

            if (port.Nature == "بري") radioLand.Checked = true;
            else if (port.Nature == "جوي") radioAir.Checked = true;
            else if (port.Nature == "بحري") radioSea.Checked = true;

            if (comboPortType != null)
            {
                if (port.PortType == "رئيسي") comboPortType.SelectedItem = LanguageManager.Get("port_type_main");
                else if (port.PortType == "رقابي") comboPortType.SelectedItem = LanguageManager.Get("port_type_supervisory");
            }

            // ✅ عرض مجموعة المنافذ في الـ ComboBox
            if (cmbPortGroup != null)
            {
                if (!string.IsNullOrEmpty(port.Classification) && cmbPortGroup.Items.Contains(port.Classification))
                {
                    cmbPortGroup.SelectedItem = port.Classification;
                }
                else
                {
                    cmbPortGroup.SelectedIndex = 0;
                }
            }

            if (comboCurrency != null)
            {
                if (port.Currency == "ريال يمني") comboCurrency.SelectedItem = LanguageManager.Get("currency_yer");
                else if (port.Currency == "ريال جنوبي") comboCurrency.SelectedItem = LanguageManager.Get("currency_yer_south");
            }

            if (txtExchangeRateUSD != null) txtExchangeRateUSD.Text = FormatNumber(port.ExchangeRateUSD, 2);
            if (txtMinRate != null) txtMinRate.Text = FormatNumber(port.MinRate, 2);
            if (txtMaxRate != null) txtMaxRate.Text = FormatNumber(port.MaxRate, 2);

            _isNavigating = false;
        }

        private void PortsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewForm != null && !viewForm.IsDisposed) viewForm.Close();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            try
            {
                MainForm mainForm = this.MdiParent as MainForm ?? Application.OpenForms.OfType<MainForm>().FirstOrDefault();
                if (mainForm != null && !mainForm.IsDisposed)
                {
                    foreach (Form child in mainForm.MdiChildren.ToList())
                        if (!child.IsDisposed && child != this) child.Close();
                    this.Close();
                    mainForm.ShowWelcome();
                }
                else this.Close();
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "خطأ"); this.Close(); }
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            ClearFields();
            SetFieldsReadOnly(false);
            txtPortName.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                string portName = txtPortName.Text.Trim();
                string portNature = radioLand.Checked ? "بري" : (radioAir.Checked ? "جوي" : "بحري");
                string portType = comboPortType.SelectedItem?.ToString() ?? LanguageManager.Get("port_type_main");

                // ✅ أخذ القيمة من ComboBox مجموعة المنافذ
                string classification = (cmbPortGroup != null && cmbPortGroup.SelectedIndex > 0)
                                        ? cmbPortGroup.SelectedItem.ToString()
                                        : "";

                string exchangeRate = (txtExchangeRateUSD?.Text ?? "0").Trim();
                string currency = comboCurrency.SelectedItem?.ToString() ?? (cmbPortGroup.SelectedIndex > 0 ? "ريال يمني" : "ريال جنوبي");
                string exchangeRateType = exchangeRate + " ريال/دولار";

                decimal rateUSD = 0, minRate = 0, maxRate = 0;
                decimal.TryParse(txtExchangeRateUSD?.Text ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out rateUSD);
                decimal.TryParse(txtMinRate?.Text ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out minRate);
                decimal.TryParse(txtMaxRate?.Text ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out maxRate);

                var existing = _allPorts.FirstOrDefault(p => p.Name == portName);
                if (existing != null)
                {
                    existing.Nature = portNature;
                    existing.PortType = portType;
                    existing.Classification = classification;
                    existing.Rate = exchangeRate;
                    existing.Currency = currency;
                    existing.ExchangeRateType = exchangeRateType;
                    existing.ExchangeRateUSD = rateUSD;
                    existing.MinRate = minRate;
                    existing.MaxRate = maxRate;
                    SavePortToDatabase(existing, "تعديل");
                    MessageBox.Show(LanguageManager.Get("msg_edit_success"), LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var newPort = new PortData
                    {
                        Name = portName,
                        Nature = portNature,
                        PortType = portType,
                        Classification = classification,
                        Rate = exchangeRate,
                        Currency = currency,
                        ExchangeRateType = exchangeRateType,
                        ExchangeRateUSD = rateUSD,
                        MinRate = minRate,
                        MaxRate = maxRate,
                        UserNumber = "1"
                    };
                    SavePortToDatabase(newPort, "إضافة");
                    _allPorts.Add(newPort);
                    _currentPortIndex = _allPorts.Count - 1;
                    MessageBox.Show(LanguageManager.Get("msg_save_success"), LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                LoadAuditLog();
                ClearFields();
                SetFieldsReadOnly(true);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPortName.Text))
            {
                MessageBox.Show(LanguageManager.Get("msg_select_port_edit"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetFieldsReadOnly(false);
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPortName.Text))
            {
                MessageBox.Show(LanguageManager.Get("msg_select_port_delete"), LanguageManager.Get("msg_warning"), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(string.Format(LanguageManager.Get("msg_confirm_delete"), txtPortName.Text), LanguageManager.Get("msg_confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                string portName = txtPortName.Text.Trim();
                var port = _allPorts.FirstOrDefault(p => p.Name == portName);
                if (port != null)
                {
                    try
                    {
                        DatabaseHelper.ExecuteNonQuery("DELETE FROM Ports WHERE PortId = @PortId",
                            DatabaseHelper.CreateParameter("@PortId", port.PortId));

                        DatabaseHelper.ExecuteNonQuery(@"INSERT INTO PortsAuditLog 
                            (PortName, PortNature, PortType, Classification, ExchangeRate, Currency, ExchangeRateType, UserNumber, ModifiedDate, OperationType) 
                            VALUES (@Name, @Nature, @Type, @Classification, @Rate, @Currency, @RateType, @User, datetime('now'), @OpType)",
                            DatabaseHelper.CreateParameter("@Name", port.Name),
                            DatabaseHelper.CreateParameter("@Nature", port.Nature),
                            DatabaseHelper.CreateParameter("@Type", port.PortType),
                            DatabaseHelper.CreateParameter("@Classification", port.Classification),
                            DatabaseHelper.CreateParameter("@Rate", port.Rate),
                            DatabaseHelper.CreateParameter("@Currency", port.Currency),
                            DatabaseHelper.CreateParameter("@RateType", port.ExchangeRateType),
                            DatabaseHelper.CreateParameter("@User", port.UserNumber ?? "1"),
                            DatabaseHelper.CreateParameter("@OpType", "حذف")
                        );

                        _allPorts.Remove(port);
                        LoadAuditLog();
                        ClearFields();
                        MessageBox.Show(LanguageManager.Get("msg_delete_success"), LanguageManager.Get("msg_success"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"❌ خطأ في الحذف:\n{ex.Message}", "خطأ");
                    }
                }
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            using (PortsViewForm viewForm = new PortsViewForm(false))
            {
                viewForm.StartPosition = FormStartPosition.CenterParent;
                viewForm.ShowDialog(this);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchTerm = txtPortName.Text?.Trim();
            if (string.IsNullOrEmpty(searchTerm))
            {
                MessageBox.Show(LanguageManager.Get("msg_enter_port_name"), LanguageManager.Get("search_result"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            for (int i = 0; i < _allPorts.Count; i++)
            {
                if (_allPorts[i].Name.Contains(searchTerm))
                {
                    _currentPortIndex = i;
                    DisplayPortData(_allPorts[_currentPortIndex]);
                    MessageBox.Show(string.Format(LanguageManager.Get("msg_found"), _allPorts[_currentPortIndex].Name), LanguageManager.Get("search_result"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
            }
            MessageBox.Show(string.Format(LanguageManager.Get("msg_not_found"), searchTerm), LanguageManager.Get("search_result"), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show(LanguageManager.Get("msg_printing"));
        }

        private void btnPrevious_Click(object sender, EventArgs e)
        {
            if (_allPorts.Count == 0) { MessageBox.Show(LanguageManager.Get("msg_no_ports"), LanguageManager.Get("msg_warning")); return; }
            if (_currentPortIndex > 0)
            {
                _currentPortIndex--;
                DisplayPortData(_allPorts[_currentPortIndex]);
            }
            else
            {
                MessageBox.Show(LanguageManager.Get("msg_first_port"), LanguageManager.Get("msg_warning"));
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (_allPorts.Count == 0) { MessageBox.Show(LanguageManager.Get("msg_no_ports"), LanguageManager.Get("msg_warning")); return; }
            if (_currentPortIndex < _allPorts.Count - 1)
            {
                _currentPortIndex++;
                DisplayPortData(_allPorts[_currentPortIndex]);
            }
            else
            {
                MessageBox.Show(LanguageManager.Get("msg_last_port"), LanguageManager.Get("msg_warning"));
            }
        }

        private void btnExit_Click(object sender, EventArgs e) => btnHome_Click(sender, e);

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtPortName.Text))
            {
                MessageBox.Show(LanguageManager.Get("msg_port_name_required"), LanguageManager.Get("msg_warning"));
                txtPortName.Focus();
                return false;
            }

            // ✅ التحقق من اختيار مجموعة المنافذ
            if (cmbPortGroup != null && cmbPortGroup.SelectedIndex == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار مجموعة المنافذ", LanguageManager.Get("msg_warning"));
                cmbPortGroup.Focus();
                return false;
            }

            decimal rateUSD = 0, minRate = 0, maxRate = 0;
            bool validUSD = decimal.TryParse(txtExchangeRateUSD?.Text ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out rateUSD);
            bool validMin = decimal.TryParse(txtMinRate?.Text ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out minRate);
            bool validMax = decimal.TryParse(txtMaxRate?.Text ?? "0", NumberStyles.Any, CultureInfo.InvariantCulture, out maxRate);

            if (!validUSD || !validMin || !validMax)
            {
                MessageBox.Show("⚠️ الرجاء إدخال قيم صحيحة لسعر الصرف", LanguageManager.Get("msg_warning"));
                return false;
            }

            if (minRate >= maxRate)
            {
                MessageBox.Show("⚠️ الحد الأدنى يجب أن يكون أقل من الحد الأعلى", LanguageManager.Get("msg_warning"));
                txtMinRate.Focus();
                return false;
            }

            if (rateUSD < minRate || rateUSD > maxRate)
            {
                MessageBox.Show($"⚠️ سعر الصرف ({FormatNumber(rateUSD, 2)}) يجب أن يكون بين الحد الأدنى ({FormatNumber(minRate, 2)}) والحد الأعلى ({FormatNumber(maxRate, 2)})",
                    LanguageManager.Get("msg_warning"));
                txtExchangeRateUSD.Focus();
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            _isNavigating = true;
            txtPortNumber.Clear();
            txtPortName.Clear();
            radioLand.Checked = true;
            if (comboPortType != null) comboPortType.SelectedIndex = 0;
            if (comboCurrency != null) comboCurrency.SelectedIndex = 0;

            // ✅ إعادة تعيين ComboBox مجموعة المنافذ
            if (cmbPortGroup != null) cmbPortGroup.SelectedIndex = 0;

            if (txtExchangeRateUSD != null) txtExchangeRateUSD.Clear();
            if (txtMinRate != null) txtMinRate.Clear();
            if (txtMaxRate != null) txtMaxRate.Clear();

            _currentPortIndex = -1;
            _isNavigating = false;
            SetFieldsReadOnly(true);
            txtPortName.Focus();
        }

        public void LoadPortData(string name, string nature, string portType, string classification, string rate, string selectedExchangeRate)
        {
            for (int i = 0; i < _allPorts.Count; i++)
            {
                if (_allPorts[i].Name == name)
                {
                    _currentPortIndex = i;
                    _isNavigating = true;
                    DisplayPortData(_allPorts[_currentPortIndex]);
                    _isNavigating = false;
                    return;
                }
            }
        }

        public void DeletePortFromAllLists(string portName)
        {
            for (int i = _allPorts.Count - 1; i >= 0; i--)
            {
                if (_allPorts[i].Name == portName)
                {
                    _allPorts.RemoveAt(i);
                    break;
                }
            }
        }

        public static (decimal min, decimal max) GetPortRateLimits(string portName)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT MinRate, MaxRate FROM Ports WHERE PortName = @Name",
                    DatabaseHelper.CreateParameter("@Name", portName));
                if (dt != null && dt.Rows.Count > 0)
                {
                    decimal min = dt.Rows[0]["MinRate"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["MinRate"]) : 0;
                    decimal max = dt.Rows[0]["MaxRate"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["MaxRate"]) : 0;
                    return (min, max);
                }
            }
            catch { }
            return (0, 0);
        }
    }
}