using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class CurrenciesForm : Form
    {
        private List<CurrencyItem> currenciesList = new List<CurrencyItem>();
        private List<AuditLogItem> auditLogList = new List<AuditLogItem>();
        private CurrenciesViewForm viewForm = null;

        public CurrenciesForm()
        {
            InitializeComponent();
            SetupForm();
            LoadCurrenciesFromDatabase();
            LoadAuditLogFromDatabase();
            SetupAuditGrid();
            LoadAuditLogToGrid();
            ApplyTranslations();

            // ✅ قفل جميع الحقول افتراضياً عند فتح الشاشة
            SetFieldsReadOnly(true);

            LanguageManager.LanguageChanged += (s, e) => RefreshAllTranslations();
        }

        // ==================== ✅ التحكم في حالة الحقول ====================
        private void SetFieldsReadOnly(bool readOnly)
        {
            txtNumber.ReadOnly = readOnly;
            txtName.ReadOnly = readOnly;
            txtForeignName.ReadOnly = readOnly;
            txtSymbol.ReadOnly = readOnly;
            txtSubunit.ReadOnly = readOnly;
            txtExchangeRate.ReadOnly = readOnly;
            txtMinRate.ReadOnly = readOnly;
            txtMaxRate.ReadOnly = readOnly;
            txtDecimalPlaces.ReadOnly = readOnly;

            radioLocal.Enabled = !readOnly;
            radioForeign.Enabled = !readOnly;
            chkStored.Enabled = !readOnly;

            Color bgColor = readOnly ? Color.FromArgb(245, 245, 245) : Color.White;
            txtNumber.BackColor = bgColor;
            txtName.BackColor = bgColor;
            txtForeignName.BackColor = bgColor;
            txtSymbol.BackColor = bgColor;
            txtSubunit.BackColor = bgColor;
            txtExchangeRate.BackColor = bgColor;
            txtMinRate.BackColor = bgColor;
            txtMaxRate.BackColor = bgColor;
            txtDecimalPlaces.BackColor = bgColor;
        }

        private void SetupForm()
        {
            this.WindowState = FormWindowState.Maximized;
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;
            this.Text = LanguageManager.Get("currencies_form_title");
        }

        private void ApplyTranslations()
        {
            this.Text = LanguageManager.Get("currencies_form_title");
            this.RightToLeft = LanguageManager.RightToLeft;
            this.RightToLeftLayout = LanguageManager.IsRTL;

            TrySetText(FindControl<GroupBox>("grpCurrencyData"), LanguageManager.Get("grp_currency_data"));
            TrySetText(FindControl<Label>("lblNumber"), LanguageManager.Get("lbl_currency_number"));
            TrySetText(FindControl<Label>("lblName"), LanguageManager.Get("lbl_currency_name"));
            TrySetText(FindControl<Label>("lblSymbol"), LanguageManager.Get("lbl_currency_symbol"));
            TrySetText(FindControl<Label>("lblForeignName"), LanguageManager.Get("lbl_foreign_name"));
            TrySetText(FindControl<Label>("lblSubunit"), LanguageManager.Get("lbl_subunit"));
            TrySetText(FindControl<Label>("lblExchangeRate"), LanguageManager.Get("lbl_exchange_rate"));
            TrySetText(FindControl<Label>("lblMinRate"), LanguageManager.Get("lbl_min_rate"));
            TrySetText(FindControl<Label>("lblMaxRate"), LanguageManager.Get("lbl_max_rate"));
            TrySetText(FindControl<Label>("lblDecimalPlaces"), LanguageManager.Get("lbl_decimal_places"));
            TrySetText(FindControl<CheckBox>("chkStored"), LanguageManager.Get("chk_stored"));
            TrySetText(FindControl<RadioButton>("radioLocal"), LanguageManager.Get("radio_local"));
            TrySetText(FindControl<RadioButton>("radioForeign"), LanguageManager.Get("radio_foreign"));

            TrySetText(FindToolStripButton("btnHome"), LanguageManager.Get("btn_home"));
            TrySetText(FindToolStripButton("btnSave"), LanguageManager.Get("btn_save"));
            TrySetText(FindToolStripButton("btnNew"), LanguageManager.Get("btn_new"));
            TrySetText(FindToolStripButton("btnEdit"), LanguageManager.Get("btn_edit"));
            TrySetText(FindToolStripButton("btnDelete"), LanguageManager.Get("btn_delete"));
            TrySetText(FindToolStripButton("btnView"), LanguageManager.Get("btn_view"));
            TrySetText(FindToolStripButton("btnSearch"), LanguageManager.Get("btn_search"));
            TrySetText(FindToolStripButton("btnPrint"), LanguageManager.Get("btn_print"));
            TrySetText(FindToolStripButton("btnExit"), LanguageManager.Get("btn_exit"));

            UpdateAuditGridHeaders();
        }

        private T FindControl<T>(string name) where T : Control
        {
            try { return this.Controls.Find(name, true).FirstOrDefault() as T; }
            catch { return null; }
        }

        private ToolStripButton FindToolStripButton(string name)
        {
            try
            {
                var ts = this.Controls.Find("toolStrip1", true).FirstOrDefault() as ToolStrip;
                if (ts != null)
                {
                    foreach (ToolStripItem item in ts.Items)
                    {
                        if (item.Name == name) return item as ToolStripButton;
                    }
                }
                return null;
            }
            catch { return null; }
        }

        private void TrySetText(Control control, string text)
        {
            if (control != null && !string.IsNullOrEmpty(text)) control.Text = text;
        }

        private void TrySetText(ToolStripButton button, string text)
        {
            if (button != null && !string.IsNullOrEmpty(text)) button.Text = text;
        }

        private void RefreshAllTranslations()
        {
            ApplyTranslations();
            LoadAuditLogToGrid();
        }

        private void UpdateAuditGridHeaders()
        {
            if (dgvAudit == null || dgvAudit.Columns.Count == 0) return;

            if (dgvAudit.Columns["Currency"] != null) dgvAudit.Columns["Currency"].HeaderText = LanguageManager.Get("col_currency");
            if (dgvAudit.Columns["Action"] != null) dgvAudit.Columns["Action"].HeaderText = LanguageManager.Get("col_action");
            if (dgvAudit.Columns["OldRate"] != null) dgvAudit.Columns["OldRate"].HeaderText = LanguageManager.Get("col_old_rate");
            if (dgvAudit.Columns["NewRate"] != null) dgvAudit.Columns["NewRate"].HeaderText = LanguageManager.Get("col_new_rate");
            if (dgvAudit.Columns["OldMinRate"] != null) dgvAudit.Columns["OldMinRate"].HeaderText = LanguageManager.Get("col_old_min_rate");
            if (dgvAudit.Columns["NewMinRate"] != null) dgvAudit.Columns["NewMinRate"].HeaderText = LanguageManager.Get("col_new_min_rate");
            if (dgvAudit.Columns["OldMaxRate"] != null) dgvAudit.Columns["OldMaxRate"].HeaderText = LanguageManager.Get("col_old_max_rate");
            if (dgvAudit.Columns["NewMaxRate"] != null) dgvAudit.Columns["NewMaxRate"].HeaderText = LanguageManager.Get("col_new_max_rate");
            if (dgvAudit.Columns["UserNumber"] != null) dgvAudit.Columns["UserNumber"].HeaderText = LanguageManager.Get("col_user");
            if (dgvAudit.Columns["ModifiedDate"] != null) dgvAudit.Columns["ModifiedDate"].HeaderText = LanguageManager.Get("col_modified_date");
        }

        public class CurrencyItem
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Type { get; set; }
            public decimal ExchangeRate { get; set; }
            public decimal MinRate { get; set; }
            public decimal MaxRate { get; set; }
            public string UserNumber { get; set; }
            public string ModifiedDate { get; set; }
            public string DisplayName => $"{Name} ({Type})";
        }

        public class AuditLogItem
        {
            public string CurrencyName { get; set; }
            public string Action { get; set; }
            public decimal OldRate { get; set; }
            public decimal NewRate { get; set; }
            public decimal OldMinRate { get; set; }
            public decimal NewMinRate { get; set; }
            public decimal OldMaxRate { get; set; }
            public decimal NewMaxRate { get; set; }
            public string UserNumber { get; set; }
            public string ModifiedDate { get; set; }
        }

        private void InitializeCurrenciesList()
        {
            currenciesList = new List<CurrencyItem>
            {
                new CurrencyItem { Name = "الريال اليمني (جنوبي)", Code = "YER-S", Type = LanguageManager.Get("type_local"), ExchangeRate = 750.0000m, MinRate = 740.0000m, MaxRate = 760.0000m, UserNumber = "1", ModifiedDate = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd") },
                new CurrencyItem { Name = "الريال اليمني (شمالي)", Code = "YER-N", Type = LanguageManager.Get("type_local"), ExchangeRate = 535.0000m, MinRate = 530.0000m, MaxRate = 540.0000m, UserNumber = "1", ModifiedDate = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd") },
                new CurrencyItem { Name = "الدولار الأمريكي", Code = "USD", Type = LanguageManager.Get("type_foreign"), ExchangeRate = 1.0000m, MinRate = 0.9900m, MaxRate = 1.0100m, UserNumber = "1", ModifiedDate = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd") }
            };
        }

        private void LoadCurrenciesFromDatabase()
        {
            currenciesList.Clear();
            try
            {
                // ✅ تم التصحيح: CurrencyNo -> CurrencyNumber
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM Currencies ORDER BY CurrencyNumber");
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        currenciesList.Add(new CurrencyItem
                        {
                            Code = row["CurrencyNumber"].ToString(), // ✅ تم التصحيح
                            Name = row["CurrencyName"].ToString(),
                            Type = row["CurrencyType"].ToString(),
                            ExchangeRate = row["ExchangeRate"] != DBNull.Value ? Convert.ToDecimal(row["ExchangeRate"]) : 0,
                            MinRate = row["MinRate"] != DBNull.Value ? Convert.ToDecimal(row["MinRate"]) : 0,
                            MaxRate = row["MaxRate"] != DBNull.Value ? Convert.ToDecimal(row["MaxRate"]) : 0,
                            // ✅ تم التصحيح: استخدام رقم المستخدم الحالي أو 1 كقيمة افتراضية لأن العمود غير موجود في الجدول
                            UserNumber = UserManager.GetCurrentUser()?.UserNumber ?? "1",
                            ModifiedDate = row["ModifiedDate"] != DBNull.Value ? Convert.ToDateTime(row["ModifiedDate"]).ToString("HH:mm:ss yyyy-MM-dd") : DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd")
                        });
                    }
                }
                else
                {
                    InitializeCurrenciesList();
                    foreach (var c in currenciesList) SaveCurrencyToDb(c, "إضافة", 0, 0, 0);
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ في تحميل العملات:\n{ex.Message}", "خطأ"); }
        }

        private void LoadAuditLogFromDatabase()
        {
            auditLogList.Clear();
            try
            {
                // ✅ تم التصحيح: ترتيب حسب ChangeDate بدلاً من ModifiedDate
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM CurrenciesAuditLog ORDER BY ChangeDate DESC");
                if (dt != null)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        auditLogList.Add(new AuditLogItem
                        {
                            CurrencyName = row["CurrencyName"].ToString(),
                            Action = row["Action"].ToString(), // ✅ تم التصحيح: OperationType -> Action
                            OldRate = row["OldRate"] != DBNull.Value ? Convert.ToDecimal(row["OldRate"]) : 0,
                            NewRate = row["NewRate"] != DBNull.Value ? Convert.ToDecimal(row["NewRate"]) : 0,
                            OldMinRate = row["OldMinRate"] != DBNull.Value ? Convert.ToDecimal(row["OldMinRate"]) : 0,
                            NewMinRate = row["NewMinRate"] != DBNull.Value ? Convert.ToDecimal(row["NewMinRate"]) : 0,
                            OldMaxRate = row["OldMaxRate"] != DBNull.Value ? Convert.ToDecimal(row["OldMaxRate"]) : 0,
                            NewMaxRate = row["NewMaxRate"] != DBNull.Value ? Convert.ToDecimal(row["NewMaxRate"]) : 0,
                            UserNumber = row["UserName"] != DBNull.Value ? row["UserName"].ToString() : "1", // ✅ تم التصحيح: UserNumber -> UserName
                            ModifiedDate = row["ChangeDate"] != DBNull.Value ? Convert.ToDateTime(row["ChangeDate"]).ToString("HH:mm:ss yyyy-MM-dd") : DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd") // ✅ تم التصحيح: ModifiedDate -> ChangeDate
                        });
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"❌ خطأ في تحميل سجل المراجعة:\n{ex.Message}", "خطأ"); }
        }

        private void SaveCurrencyToDb(CurrencyItem currency, string action, decimal oldRate, decimal oldMin, decimal oldMax)
        {
            try
            {
                // ✅ تم التصحيح: CurrencyNo -> CurrencyNumber
                var check = DatabaseHelper.ExecuteScalar(
                    "SELECT COUNT(*) FROM Currencies WHERE CurrencyNumber = @Code",
                    DatabaseHelper.CreateParameter("@Code", currency.Code)
                );
                bool exists = Convert.ToInt32(check) > 0;

                if (exists && action == "إضافة") action = "تعديل";

                if (exists)
                {
                    // ✅ تم التصحيح: إزالة UserNumber وتحديث CurrencyNumber
                    DatabaseHelper.ExecuteNonQuery(@"UPDATE Currencies SET 
                        CurrencyName = @Name, CurrencyType = @Type, ExchangeRate = @Rate, 
                        MinRate = @MinRate, MaxRate = @MaxRate, ModifiedDate = datetime('now')
                        WHERE CurrencyNumber = @Code",
                        DatabaseHelper.CreateParameter("@Name", currency.Name),
                        DatabaseHelper.CreateParameter("@Type", currency.Type),
                        DatabaseHelper.CreateParameter("@Rate", currency.ExchangeRate),
                        DatabaseHelper.CreateParameter("@MinRate", currency.MinRate),
                        DatabaseHelper.CreateParameter("@MaxRate", currency.MaxRate),
                        DatabaseHelper.CreateParameter("@Code", currency.Code)
                    );
                }
                else
                {
                    // ✅ تم التصحيح: إزالة UserNumber وتحديث CurrencyNumber
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO Currencies (CurrencyNumber, CurrencyName, CurrencyType, ExchangeRate, MinRate, MaxRate, ModifiedDate) 
                        VALUES (@Code, @Name, @Type, @Rate, @MinRate, @MaxRate, datetime('now'))",
                        DatabaseHelper.CreateParameter("@Code", currency.Code),
                        DatabaseHelper.CreateParameter("@Name", currency.Name),
                        DatabaseHelper.CreateParameter("@Type", currency.Type),
                        DatabaseHelper.CreateParameter("@Rate", currency.ExchangeRate),
                        DatabaseHelper.CreateParameter("@MinRate", currency.MinRate),
                        DatabaseHelper.CreateParameter("@MaxRate", currency.MaxRate)
                    );
                }

                // ✅ تم التصحيح: مطابقة أسماء أعمدة جدول CurrenciesAuditLog في DatabaseHelper
                DatabaseHelper.ExecuteNonQuery(@"INSERT INTO CurrenciesAuditLog 
                    (CurrencyName, Action, OldRate, NewRate, OldMinRate, NewMinRate, OldMaxRate, NewMaxRate, UserName, ChangeDate) 
                    VALUES (@CurName, @OpType, @ORate, @NRate, @OMin, @NMin, @OMax, @NMax, @User, datetime('now'))",
                    DatabaseHelper.CreateParameter("@CurName", currency.Name + " (" + currency.Type + ")"),
                    DatabaseHelper.CreateParameter("@OpType", action),
                    DatabaseHelper.CreateParameter("@ORate", oldRate),
                    DatabaseHelper.CreateParameter("@NRate", currency.ExchangeRate),
                    DatabaseHelper.CreateParameter("@OMin", oldMin),
                    DatabaseHelper.CreateParameter("@NMin", currency.MinRate),
                    DatabaseHelper.CreateParameter("@OMax", oldMax),
                    DatabaseHelper.CreateParameter("@NMax", currency.MaxRate),
                    DatabaseHelper.CreateParameter("@User", UserManager.GetCurrentUser()?.FullName ?? "System")
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ العملة:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        private void SetupAuditGrid()
        {
            dgvAudit.Columns.Clear();
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "Currency", HeaderText = LanguageManager.Get("col_currency"), Width = 200 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "Action", HeaderText = LanguageManager.Get("col_action"), Width = 80 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "OldRate", HeaderText = LanguageManager.Get("col_old_rate"), Width = 100 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "NewRate", HeaderText = LanguageManager.Get("col_new_rate"), Width = 100 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "OldMinRate", HeaderText = LanguageManager.Get("col_old_min_rate"), Width = 110 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "NewMinRate", HeaderText = LanguageManager.Get("col_new_min_rate"), Width = 110 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "OldMaxRate", HeaderText = LanguageManager.Get("col_old_max_rate"), Width = 110 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "NewMaxRate", HeaderText = LanguageManager.Get("col_new_max_rate"), Width = 110 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "UserNumber", HeaderText = LanguageManager.Get("col_user"), Width = 70 });
            dgvAudit.Columns.Add(new DataGridViewTextBoxColumn { Name = "ModifiedDate", HeaderText = LanguageManager.Get("col_modified_date"), Width = 150 });

            dgvAudit.ReadOnly = true;
            dgvAudit.AllowUserToAddRows = false;
            dgvAudit.AllowUserToDeleteRows = false;
            dgvAudit.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAudit.MultiSelect = false;
            dgvAudit.Dock = DockStyle.Fill;
            dgvAudit.RightToLeft = LanguageManager.RightToLeft;
        }

        private void LoadAuditLogToGrid()
        {
            dgvAudit.Rows.Clear();
            foreach (var log in auditLogList.OrderByDescending(x => x.ModifiedDate))
            {
                string action = log.Action;
                if (action == "إضافة") action = LanguageManager.Get("action_add");
                else if (action == "تعديل") action = LanguageManager.Get("action_edit");
                else if (action == "حذف") action = LanguageManager.Get("action_delete");

                dgvAudit.Rows.Add(log.CurrencyName, action,
                    log.OldRate.ToString("N4", CultureInfo.InvariantCulture),
                    log.NewRate.ToString("N4", CultureInfo.InvariantCulture),
                    log.OldMinRate.ToString("N4", CultureInfo.InvariantCulture),
                    log.NewMinRate.ToString("N4", CultureInfo.InvariantCulture),
                    log.OldMaxRate.ToString("N4", CultureInfo.InvariantCulture),
                    log.NewMaxRate.ToString("N4", CultureInfo.InvariantCulture),
                    log.UserNumber, log.ModifiedDate);
            }
        }

        public bool DeleteCurrencyPermanently(string currencyName)
        {
            try
            {
                // البحث عن العملة في القائمة
                var item = currenciesList.FirstOrDefault(c => c.Name == currencyName || c.Code == currencyName);
                if (item != null)
                {
                    // ✅ 1. حذف العملة من جدول العملات
                    DatabaseHelper.ExecuteNonQuery(
                        "DELETE FROM Currencies WHERE CurrencyNumber = @Code",
                        DatabaseHelper.CreateParameter("@Code", item.Code)
                    );

                    // ✅ 2. تسجيل عملية الحذف في سجل المراجعة باستخدام بيانات 'item' الصحيحة
                    DatabaseHelper.ExecuteNonQuery(@"INSERT INTO CurrenciesAuditLog 
                        (CurrencyId, CurrencyName, Action, OldRate, NewRate, OldMinRate, NewMinRate, OldMaxRate, NewMaxRate, UserName, ChangeDate) 
                        VALUES (@CurrencyId, @CurName, @OpType, @ORate, @NRate, @OMin, @NMin, @OMax, @NMax, @User, datetime('now'))",
                        DatabaseHelper.CreateParameter("@CurrencyId", 0),
                        DatabaseHelper.CreateParameter("@CurName", item.Name + " (" + item.Type + ")"), // ✅ تم التصحيح: استخدام item
                        DatabaseHelper.CreateParameter("@OpType", "حذف"),                               // ✅ تم التصحيح: تحديد الإجراء كـ "حذف"
                        DatabaseHelper.CreateParameter("@ORate", item.ExchangeRate),                    // ✅ تم التصحيح: استخدام item.ExchangeRate
                        DatabaseHelper.CreateParameter("@NRate", 0),                                    // ✅ السعر الجديد بعد الحذف هو 0
                        DatabaseHelper.CreateParameter("@OMin", item.MinRate),                          // ✅ تم التصحيح: استخدام item.MinRate
                        DatabaseHelper.CreateParameter("@NMin", 0),                                     // ✅ الحد الأدنى الجديد هو 0
                        DatabaseHelper.CreateParameter("@OMax", item.MaxRate),                          // ✅ تم التصحيح: استخدام item.MaxRate
                        DatabaseHelper.CreateParameter("@NMax", 0),                                     // ✅ الحد الأقصى الجديد هو 0
                        DatabaseHelper.CreateParameter("@User", UserManager.GetCurrentUser()?.FullName ?? "System") // ✅ استخدام اسم المستخدم الكامل
                    );

                    // ✅ 3. تحديث القائمة المحلية وإعادة تحميل الجداول
                    currenciesList.Remove(item);
                    LoadAuditLogFromDatabase();
                    LoadAuditLogToGrid();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ {LanguageManager.Get("msg_error")}: {ex.Message}", LanguageManager.Get("msg_error"));
                return false;
            }
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
            txtNumber.Focus();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (ValidateInputs())
            {
                AddToAuditLog();
                SetFieldsReadOnly(true);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvAudit.SelectedRows.Count > 0)
            {
                LoadSelectedRowData();
                SetFieldsReadOnly(false);
                txtNumber.ReadOnly = true;
                txtNumber.BackColor = Color.FromArgb(245, 245, 245);
            }
            else
            {
                MessageBox.Show(LanguageManager.Get("msg_select_record"), LanguageManager.Get("msg_warning"));
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvAudit.SelectedRows.Count == 0) { MessageBox.Show(LanguageManager.Get("msg_select_currency"), LanguageManager.Get("msg_warning")); return; }
            string name = dgvAudit.SelectedRows[0].Cells["Currency"].Value?.ToString();
            if (MessageBox.Show(string.Format(LanguageManager.Get("msg_confirm_delete_curr"), name), LanguageManager.Get("msg_confirm"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (DeleteCurrencyPermanently(name)) MessageBox.Show(LanguageManager.Get("msg_delete_success_curr"), LanguageManager.Get("msg_success"));
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (viewForm != null && !viewForm.IsDisposed) { viewForm.BringToFront(); return; }
            viewForm = new CurrenciesViewForm(deleteMode: true);
            viewForm.StartPosition = FormStartPosition.CenterScreen;
            viewForm.FormClosed += (s, a) => viewForm = null;
            viewForm.Show(this);
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string term = txtName.Text?.Trim();
            if (string.IsNullOrEmpty(term)) { MessageBox.Show(LanguageManager.Get("msg_enter_currency_name"), LanguageManager.Get("btn_search")); return; }
            foreach (DataGridViewRow row in dgvAudit.Rows)
            {
                if ((row.Cells["Currency"].Value?.ToString() ?? "").IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    dgvAudit.CurrentCell = row.Cells[0];
                    row.Selected = true;
                    return;
                }
            }
            MessageBox.Show(string.Format(LanguageManager.Get("msg_not_found_curr"), term), LanguageManager.Get("btn_search"));
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            MessageBox.Show(LanguageManager.Get("msg_printing"));
        }

        private void btnPrevious_Click(object sender, EventArgs e) { NavigateCurrency(-1); }
        private void btnNext_Click(object sender, EventArgs e) { NavigateCurrency(1); }
        private void btnExit_Click(object sender, EventArgs e) => btnHome_Click(sender, e);

        private void NavigateCurrency(int direction)
        {
            try
            {
                if (viewForm == null || viewForm.IsDisposed)
                {
                    viewForm = new CurrenciesViewForm(deleteMode: true);
                    viewForm.StartPosition = FormStartPosition.CenterScreen;
                    viewForm.FormClosed += (s, a) => viewForm = null;
                    viewForm.Show(this);
                }
                var data = direction < 0 ? viewForm.GetPreviousCurrency() : viewForm.GetNextCurrency();
                if (data != null) LoadCurrencyDataFromView(data);
            }
            catch (Exception ex) { MessageBox.Show($"❌ {ex.Message}", "خطأ"); }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtNumber.Text)) { MessageBox.Show(LanguageManager.Get("msg_currency_number_required"), LanguageManager.Get("msg_warning")); txtNumber.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show(LanguageManager.Get("msg_currency_name_required"), LanguageManager.Get("msg_warning")); txtName.Focus(); return false; }
            if (!decimal.TryParse(txtExchangeRate.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal rate) || rate <= 0) { MessageBox.Show(LanguageManager.Get("msg_invalid_rate"), LanguageManager.Get("msg_warning")); txtExchangeRate.Focus(); return false; }
            return true;
        }

        private void AddToAuditLog()
        {
            string type = radioLocal.Checked ? LanguageManager.Get("type_local") : LanguageManager.Get("type_foreign");
            decimal newRate = decimal.Parse(txtExchangeRate.Text, CultureInfo.InvariantCulture);
            decimal newMin = string.IsNullOrEmpty(txtMinRate.Text) ? newRate : decimal.Parse(txtMinRate.Text, CultureInfo.InvariantCulture);
            decimal newMax = string.IsNullOrEmpty(txtMaxRate.Text) ? newRate : decimal.Parse(txtMaxRate.Text, CultureInfo.InvariantCulture);

            var existing = currenciesList.FirstOrDefault(c => c.Code == txtSymbol.Text);
            decimal oldRate = existing?.ExchangeRate ?? 0;
            decimal oldMin = existing?.MinRate ?? 0;
            decimal oldMax = existing?.MaxRate ?? 0;
            string action = existing != null ? "تعديل" : "إضافة";

            var currencyToSave = new CurrencyItem
            {
                Code = txtSymbol.Text,
                Name = txtName.Text,
                Type = type,
                ExchangeRate = newRate,
                MinRate = newMin,
                MaxRate = newMax,
                UserNumber = txtNumber.Text,
                ModifiedDate = DateTime.Now.ToString("HH:mm:ss yyyy-MM-dd")
            };

            try
            {
                SaveCurrencyToDb(currencyToSave, action, oldRate, oldMin, oldMax);

                if (existing != null)
                {
                    var index = currenciesList.IndexOf(existing);
                    currenciesList[index] = currencyToSave;
                    MessageBox.Show(LanguageManager.Get("msg_edit_success"), LanguageManager.Get("msg_success"));
                }
                else
                {
                    currenciesList.Add(currencyToSave);
                    MessageBox.Show(LanguageManager.Get("msg_add_success_curr"), LanguageManager.Get("msg_success"));
                }

                LoadAuditLogFromDatabase();
                LoadAuditLogToGrid();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ فشل الحفظ: {ex.Message}", "خطأ");
            }
        }

        private void LoadSelectedRowData()
        {
            if (dgvAudit.CurrentRow?.Cells == null) return;
            string name = dgvAudit.CurrentRow.Cells["Currency"].Value?.ToString() ?? "";
            if (name.Contains('('))
            {
                txtName.Text = name.Substring(0, name.IndexOf('(')).Trim();
                if (name.Contains(LanguageManager.Get("type_local")) || name.Contains("محلية"))
                    radioLocal.Checked = true;
                else
                    radioForeign.Checked = true;
            }
            txtExchangeRate.Text = dgvAudit.CurrentRow.Cells["NewRate"].Value?.ToString() ?? "";
            txtMinRate.Text = dgvAudit.CurrentRow.Cells["NewMinRate"].Value?.ToString() ?? "";
            txtMaxRate.Text = dgvAudit.CurrentRow.Cells["NewMaxRate"].Value?.ToString() ?? "";
        }

        private void ClearFields()
        {
            txtNumber.Clear();
            txtName.Clear();
            txtForeignName.Clear();
            txtSymbol.Clear();
            txtSubunit.Clear();
            txtExchangeRate.Clear();
            txtMinRate.Clear();
            txtMaxRate.Clear();
            txtDecimalPlaces.Text = "2";
            radioLocal.Checked = true;
            chkStored.Checked = false;

            SetFieldsReadOnly(true);
        }

        public List<CurrencyItem> GetCurrenciesList() => currenciesList;

        public void LoadCurrencyDataFromView(CurrenciesViewForm.CurrencyDisplayData data)
        {
            if (data == null) return;
            txtName.Text = data.Name;
            txtSymbol.Text = data.Code;

            txtExchangeRate.Text = data.ExchangeRate.ToString("N4", CultureInfo.InvariantCulture);
            txtMinRate.Text = data.MinRate.ToString("N4", CultureInfo.InvariantCulture);
            txtMaxRate.Text = data.MaxRate.ToString("N4", CultureInfo.InvariantCulture);

            if (data.Type == LanguageManager.Get("type_local") || data.Type == "محلية")
                radioLocal.Checked = true;
            else
                radioForeign.Checked = true;

            var c = currenciesList.FirstOrDefault(x => x.Name == data.Name && x.Code == data.Code);
            if (c != null) { txtNumber.Text = c.UserNumber; }

            SetFieldsReadOnly(true);
        }

        private void CurrenciesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (viewForm != null && !viewForm.IsDisposed) viewForm.Close();
        }

        private void txtExchangeRate_TextChanged(object sender, EventArgs e)
        {
            if (sender is TextBox tb)
                tb.BackColor = decimal.TryParse(tb.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out _) ? Color.FromArgb(232, 245, 233) : Color.FromArgb(255, 255, 224);
        }

        private void grpCurrencyData_Enter(object sender, EventArgs e) { }
        private void radioLocal_CheckedChanged(object sender, EventArgs e) { }
        private void radioForeign_CheckedChanged(object sender, EventArgs e) { }
    }
}