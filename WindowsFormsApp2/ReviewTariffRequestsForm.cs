using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text.Json;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class ReviewTariffRequestsForm : Form
    {
        private DataGridView dgvRequests;

        public ReviewTariffRequestsForm()
        {
            this.Text = "📋 مراجعة طلبات الترسيم";
            this.Size = new Size(1400, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;

            dgvRequests = new DataGridView();
            dgvRequests.Dock = DockStyle.Fill;
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.EnableHeadersVisualStyles = false;
            dgvRequests.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            dgvRequests.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRequests.ColumnHeadersHeight = 40;
            dgvRequests.RowTemplate.Height = 35;

            dgvRequests.AllowUserToOrderColumns = true;
            dgvRequests.AllowDrop = true;

            dgvRequests.Columns.Add("colRealId", "م");
            dgvRequests.Columns["colRealId"].Visible = false;

            dgvRequests.Columns.Add("colRowNum", "م");
            dgvRequests.Columns.Add("colStatementNo", "رقم البيان");
            dgvRequests.Columns.Add("colPortName", "المنفذ");
            dgvRequests.Columns.Add("colTariffType", "نوع الترسيم");
            dgvRequests.Columns.Add("colCreatedBy", "المخلص");
            dgvRequests.Columns.Add("colCreatedDate", "تاريخ الإنشاء");
            dgvRequests.Columns.Add("colStatus", "الحالة");

            DataGridViewButtonColumn btnViewColumn = new DataGridViewButtonColumn();
            btnViewColumn.Name = "colView";
            btnViewColumn.HeaderText = "عرض التفاصيل";
            btnViewColumn.Text = "👁️ عرض";
            btnViewColumn.UseColumnTextForButtonValue = true;
            btnViewColumn.Width = 120;
            btnViewColumn.DefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
            btnViewColumn.DefaultCellStyle.ForeColor = Color.White;
            btnViewColumn.DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvRequests.Columns.Add(btnViewColumn);

            dgvRequests.Columns["colStatus"].DefaultCellStyle.ForeColor = Color.Blue;
            dgvRequests.Columns["colStatus"].DefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvRequests.Columns["colStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvRequests.Columns["colRowNum"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvRequests.Columns["colRowNum"].Width = 50;
            dgvRequests.Columns["colStatementNo"].Width = 150;
            dgvRequests.Columns["colPortName"].Width = 120;
            dgvRequests.Columns["colTariffType"].Width = 150;
            dgvRequests.Columns["colCreatedBy"].Width = 120;
            dgvRequests.Columns["colCreatedDate"].Width = 150;
            dgvRequests.Columns["colStatus"].Width = 120;

            dgvRequests.CellContentClick += DgvRequests_CellContentClick;

            Panel pnlButtons = new Panel();
            pnlButtons.Height = 60;
            pnlButtons.Dock = DockStyle.Bottom;
            pnlButtons.BackColor = Color.FromArgb(240, 248, 255);

            Button btnApprove = new Button
            {
                Text = "✅ اعتماد",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(400, 10)
            };
            btnApprove.Click += BtnApprove_Click;

            Button btnReject = new Button
            {
                Text = "❌ رفض",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(244, 67, 54),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(260, 10)
            };
            btnReject.Click += BtnReject_Click;

            Button btnClose = new Button
            {
                Text = "إغلاق",
                Font = new Font("Tahoma", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(158, 158, 158),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(120, 10)
            };
            btnClose.Click += (s, ev) => this.Close();

            pnlButtons.Controls.Add(btnApprove);
            pnlButtons.Controls.Add(btnReject);
            pnlButtons.Controls.Add(btnClose);

            this.Controls.Add(dgvRequests);
            this.Controls.Add(pnlButtons);

            LoadPendingRequests();
        }

        private void DgvRequests_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvRequests.Columns[e.ColumnIndex].Name == "colView")
            {
                int realId = Convert.ToInt32(dgvRequests.Rows[e.RowIndex].Cells["colRealId"].Value);
                ShowRequestDetails(realId);
            }
        }

        private void ShowRequestDetails(int requestId)
        {
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM TariffRequests WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@Id", requestId));

                if (dt != null && dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];

                    Form detailsForm = new Form();
                    detailsForm.Text = $"📄 تفاصيل البيان رقم: {row["StatementNo"]}";
                    detailsForm.Size = new Size(1400, 850);
                    detailsForm.StartPosition = FormStartPosition.CenterParent;
                    detailsForm.RightToLeft = RightToLeft.Yes;
                    detailsForm.RightToLeftLayout = true;
                    detailsForm.WindowState = FormWindowState.Maximized;

                    TabControl tabControl = new TabControl();
                    tabControl.Dock = DockStyle.Fill;
                    tabControl.RightToLeft = RightToLeft.Yes;
                    tabControl.RightToLeftLayout = true;

                    // التبويب الرئيسي
                    TabPage tabMain = new TabPage("البيانات الأساسية");
                    Panel pnlInfo = new Panel();
                    pnlInfo.Dock = DockStyle.Fill;
                    pnlInfo.Padding = new Padding(20);
                    pnlInfo.AutoScroll = true;

                    int yPos = 20;
                    int startX = 900;

                    AddLabelAndValue(pnlInfo, "رقم البيان:", row["StatementNo"].ToString(), 20, ref yPos, startX);
                    AddLabelAndValue(pnlInfo, "المنفذ:", row["PortName"].ToString(), 20, ref yPos, startX);
                    AddLabelAndValue(pnlInfo, "نوع الترسيم:", row["TariffType"]?.ToString() ?? "-", 20, ref yPos, startX);
                    AddLabelAndValue(pnlInfo, "تاريخ البيان:", row["StatementDate"] != DBNull.Value ? Convert.ToDateTime(row["StatementDate"]).ToString("yyyy-MM-dd") : "-", 20, ref yPos, startX);
                    AddLabelAndValue(pnlInfo, "المخلص:", row["CreatedBy"]?.ToString() ?? "-", 20, ref yPos, startX);
                    AddLabelAndValue(pnlInfo, "تاريخ الإنشاء:", row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-dd HH:mm") : "-", 20, ref yPos, startX);
                    AddLabelAndValue(pnlInfo, "الحالة:", GetStatusText(row["Status"]?.ToString()), 20, ref yPos, startX);

                    if (!string.IsNullOrEmpty(row["ReviewNotes"]?.ToString()))
                    {
                        AddLabelAndValue(pnlInfo, "ملاحظات المراجعة:", row["ReviewNotes"].ToString(), 20, ref yPos, startX, true);
                    }

                    tabMain.Controls.Add(pnlInfo);
                    tabControl.TabPages.Add(tabMain);

                    // تبويب الأصناف
                    TabPage tabItems = new TabPage("الأصناف");
                    DataGridView dgvItems = new DataGridView();
                    dgvItems.Dock = DockStyle.Fill;
                    dgvItems.ReadOnly = true;
                    dgvItems.AllowUserToAddRows = false;
                    dgvItems.EnableHeadersVisualStyles = false;
                    dgvItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                    dgvItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgvItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
                    dgvItems.ColumnHeadersHeight = 40;
                    dgvItems.AllowUserToOrderColumns = true;
                    dgvItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                    dgvItems.Columns.Add("colId", "م");
                    dgvItems.Columns.Add("colProductName", "اسم السلعة");
                    dgvItems.Columns.Add("colUnit", "الوحدة");
                    dgvItems.Columns.Add("colWeightUnit", "وحدة الوزن");
                    dgvItems.Columns.Add("colQuantity", "الكمية");
                    dgvItems.Columns.Add("colTotalWeight", "إجمالي الوزن");
                    dgvItems.Columns.Add("colAgriculturePermit", "تصريح الزراعة");
                    dgvItems.Columns.Add("colPaymentRelease", "إفراج المدفوعات");
                    dgvItems.Columns.Add("colCustomsFeeTotal", "الرسوم الجمركية");
                    dgvItems.Columns.Add("colSTTotal", "ض/المبيعات");
                    dgvItems.Columns.Add("colVATTotal", "ض/القيمة المضافة");
                    dgvItems.Columns.Add("colPTTotal", "ض/الأرباح");
                    dgvItems.Columns.Add("colVFTotal", "التحسين");
                    dgvItems.Columns.Add("colTEFTotal", "المعلم");
                    dgvItems.Columns.Add("colMFFTotal", "الشهيد");
                    dgvItems.Columns.Add("colOther", "أخرى");
                    dgvItems.Columns.Add("colTotal", "الإجمالي");

                    foreach (DataGridViewColumn col in dgvItems.Columns)
                    {
                        if (col.Name.Contains("Total") || col.Name.Contains("Quantity") ||
                            col.Name.Contains("Weight") || col.Name == "colOther")
                        {
                            col.DefaultCellStyle.Format = "N2";
                            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                        }
                    }

                    dgvItems.Columns["colId"].Width = 50;
                    dgvItems.Columns["colProductName"].Width = 200;
                    dgvItems.Columns["colAgriculturePermit"].Width = 130;
                    dgvItems.Columns["colPaymentRelease"].Width = 140;

                    string itemsJson = row["ItemsJson"]?.ToString();
                    if (!string.IsNullOrEmpty(itemsJson))
                    {
                        try
                        {
                            var items = JsonSerializer.Deserialize<List<JsonElement>>(itemsJson);
                            int idx = 1;
                            foreach (var item in items)
                            {
                                dgvItems.Rows.Add(
                                    idx++,
                                    GetJsonValue(item, "ProductName"),
                                    GetJsonValue(item, "Unit"),
                                    GetJsonValue(item, "WeightUnit"),
                                    GetJsonDecimal(item, "Quantity"),
                                    GetJsonDecimal(item, "TotalWeight"),
                                    GetJsonValue(item, "AgriculturePermit"),
                                    GetJsonValue(item, "PaymentRelease"),
                                    GetJsonDecimal(item, "CustomsFeeTotal"),
                                    GetJsonDecimal(item, "STTotal"),
                                    GetJsonDecimal(item, "VATTotal"),
                                    GetJsonDecimal(item, "PTTotal"),
                                    GetJsonDecimal(item, "VFTotal"),
                                    GetJsonDecimal(item, "TEFTotal"),
                                    GetJsonDecimal(item, "MFFTotal"),
                                    GetJsonDecimal(item, "Other"),
                                    GetJsonDecimal(item, "Total")
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"خطأ في تحميل الأصناف: {ex.Message}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    tabItems.Controls.Add(dgvItems);
                    tabControl.TabPages.Add(tabItems);

                    // تبويب السائقين
                    TabPage tabDrivers = new TabPage("بيانات السائقين");
                    DataGridView dgvDrivers = new DataGridView();
                    dgvDrivers.Dock = DockStyle.Fill;
                    dgvDrivers.ReadOnly = true;
                    dgvDrivers.AllowUserToAddRows = false;
                    dgvDrivers.EnableHeadersVisualStyles = false;
                    dgvDrivers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                    dgvDrivers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgvDrivers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    dgvDrivers.ColumnHeadersHeight = 40;
                    dgvDrivers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    dgvDrivers.Columns.Add("colId", "م");
                    dgvDrivers.Columns.Add("colDriverName", "اسم السائق");
                    dgvDrivers.Columns.Add("colVehicleNo", "رقم السيارة");

                    string driversJson = row["DriversJson"]?.ToString();
                    if (!string.IsNullOrEmpty(driversJson))
                    {
                        try
                        {
                            var drivers = JsonSerializer.Deserialize<List<JsonElement>>(driversJson);
                            int idx = 1;
                            foreach (var driver in drivers)
                            {
                                string driverName = driver.TryGetProperty("DriverName", out var nameProp) && nameProp.ValueKind == JsonValueKind.String ? nameProp.GetString() : "";
                                string vehicleNo = driver.TryGetProperty("VehicleNo", out var vehicleProp) && vehicleProp.ValueKind == JsonValueKind.String ? vehicleProp.GetString() : "";
                                dgvDrivers.Rows.Add(idx++, driverName, vehicleNo);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"خطأ في تحميل السائقين: {ex.Message}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    tabDrivers.Controls.Add(dgvDrivers);
                    tabControl.TabPages.Add(tabDrivers);

                    // تبويب المبالغ الإضافية
                    TabPage tabAdditional = new TabPage("المبالغ الإضافية");
                    DataGridView dgvAdditional = new DataGridView();
                    dgvAdditional.Dock = DockStyle.Fill;
                    dgvAdditional.ReadOnly = true;
                    dgvAdditional.AllowUserToAddRows = false;
                    dgvAdditional.EnableHeadersVisualStyles = false;
                    dgvAdditional.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 150, 243);
                    dgvAdditional.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    dgvAdditional.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    dgvAdditional.ColumnHeadersHeight = 40;
                    dgvAdditional.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    dgvAdditional.Columns.Add("colId", "م");
                    dgvAdditional.Columns.Add("colItemName", "البند");
                    dgvAdditional.Columns.Add("colAmount", "المبلغ");

                    string additionalJson = row["AdditionalAmountsJson"]?.ToString();
                    if (!string.IsNullOrEmpty(additionalJson))
                    {
                        try
                        {
                            var amounts = JsonSerializer.Deserialize<List<JsonElement>>(additionalJson);
                            int idx = 1;
                            foreach (var amt in amounts)
                            {
                                string itemName = amt.TryGetProperty("ItemName", out var nameProp) && nameProp.ValueKind == JsonValueKind.String ? nameProp.GetString() : "";
                                decimal amount = GetJsonDecimal(amt, "Amount");
                                dgvAdditional.Rows.Add(idx++, itemName, amount.ToString("N2"));
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"خطأ في تحميل المبالغ الإضافية: {ex.Message}", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }

                    tabAdditional.Controls.Add(dgvAdditional);
                    tabControl.TabPages.Add(tabAdditional);

                    Panel pnlButtonsDetails = new Panel();
                    pnlButtonsDetails.Height = 70;
                    pnlButtonsDetails.Dock = DockStyle.Bottom;
                    pnlButtonsDetails.BackColor = Color.FromArgb(240, 248, 255);

                    Button btnCloseDetails = new Button
                    {
                        Text = "إغلاق",
                        Font = new Font("Tahoma", 12, FontStyle.Bold),
                        BackColor = Color.FromArgb(158, 158, 158),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat,
                        Size = new Size(150, 45),
                        Location = new Point(550, 12)
                    };
                    btnCloseDetails.Click += (s, ev) => detailsForm.Close();

                    pnlButtonsDetails.Controls.Add(btnCloseDetails);

                    detailsForm.Controls.Add(tabControl);
                    detailsForm.Controls.Add(pnlButtonsDetails);
                    detailsForm.ShowDialog();
                }
            }
            catch (Exception ex) { MessageBox.Show($"خطأ في عرض التفاصيل: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void AddLabelAndValue(Panel panel, string labelText, string value, int x, ref int y, int startX, bool isMultiline = false)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.Font = new Font("Tahoma", 10, FontStyle.Bold);
            lbl.Location = new Point(x, y);
            lbl.Size = new Size(150, 25);
            lbl.TextAlign = ContentAlignment.MiddleRight;

            Control valueControl;
            if (isMultiline)
            {
                TextBox txt = new TextBox();
                txt.Text = value;
                txt.Multiline = true;
                txt.ReadOnly = true;
                txt.Height = 80;
                txt.Location = new Point(startX - 300, y);
                txt.Size = new Size(300, 80);
                txt.BackColor = Color.FromArgb(240, 248, 255);
                valueControl = txt;
                y += 85;
            }
            else
            {
                TextBox txt = new TextBox();
                txt.Text = value;
                txt.ReadOnly = true;
                txt.Location = new Point(startX - 300, y);
                txt.Size = new Size(300, 27);
                txt.BackColor = Color.FromArgb(240, 248, 255);
                valueControl = txt;
                y += 40;
            }

            panel.Controls.Add(lbl);
            panel.Controls.Add(valueControl);
        }

        private string GetStatusText(string status)
        {
            switch (status)
            {
                case "approved": return "✅ معتمد";
                case "rejected": return "❌ مرفوض";
                default: return "⏳ قيد المراجعة";
            }
        }

        private void LoadPendingRequests()
        {
            dgvRequests.Rows.Clear();
            try
            {
                DataTable dt = DatabaseHelper.ExecuteQuery("SELECT * FROM TariffRequests WHERE Status = 'pending' ORDER BY CreatedDate DESC");
                if (dt != null)
                {
                    int rowNum = 1;
                    foreach (DataRow row in dt.Rows)
                    {
                        dgvRequests.Rows.Add(
                            row["Id"].ToString(),
                            rowNum++,
                            row["StatementNo"].ToString(),
                            row["PortName"].ToString(),
                            row["TariffType"]?.ToString() ?? "-",
                            row["CreatedBy"]?.ToString() ?? "-",
                            row["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(row["CreatedDate"]).ToString("yyyy-MM-dd HH:mm") : "-",
                            "⏳ قيد المراجعة"
                        );
                    }
                }
            }
            catch (Exception ex) { MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        // ==================== ✅ زر الاعتماد مع خصم الكميات من التصاريح ====================
        private void BtnApprove_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار طلب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int realId = Convert.ToInt32(dgvRequests.SelectedRows[0].Cells["colRealId"].Value);
            string reviewer = UserManager.GetCurrentUser()?.FullName ?? "Admin";

            try
            {
                // 1️⃣ جلب بيانات الطلب بما فيها ItemsJson
                DataTable dt = DatabaseHelper.ExecuteQuery(
                    "SELECT * FROM TariffRequests WHERE Id = @Id",
                    DatabaseHelper.CreateParameter("@Id", realId));

                if (dt == null || dt.Rows.Count == 0)
                {
                    MessageBox.Show("❌ لم يتم العثور على الطلب", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DataRow requestRow = dt.Rows[0];
                string itemsJson = requestRow["ItemsJson"]?.ToString() ?? "";
                string statementNo = requestRow["StatementNo"]?.ToString() ?? "";

                // 2️⃣ ✅ التحقق من توفر الكميات في التصاريح/الإفراجات وخصمها
                if (!TariffRequestForm.ApplyPermitDeductions(itemsJson, out string deductError))
                {
                    MessageBox.Show(
                        $"⚠️ لا يمكن اعتماد البيان رقم {statementNo}:\n\n{deductError}\n\n💡 يجب على المخلص مراجعة أرقام التصاريح/الإفراجات أو الكميات.",
                        "فشل الاعتماد - كمية غير كافية",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return; // ❌ لا يتم الاعتماد
                }

                // 3️⃣ ✅ تحديث حالة الطلب إلى معتمد
                string query = @"UPDATE TariffRequests SET 
                                 Status = 'approved', 
                                 ReviewedBy = @Reviewer, 
                                 ReviewedDate = datetime('now') 
                                 WHERE Id = @Id";

                DatabaseHelper.ExecuteNonQuery(query,
                    DatabaseHelper.CreateParameter("@Reviewer", reviewer),
                    DatabaseHelper.CreateParameter("@Id", realId));

                MessageBox.Show(
                    $"✅ تم اعتماد البيان رقم {statementNo} بنجاح\n\n" +
                    $"🔻 تم خصم إجمالي الأوزان من التصاريح/الإفراجات المستخدمة\n" +
                    $"الآن يمكن للمخلص استخدامه في شاشة التخليص",
                    "نجاح الاعتماد", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadPendingRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== ✅ زر الرفض ====================
        private void BtnReject_Click(object sender, EventArgs e)
        {
            if (dgvRequests.SelectedRows.Count == 0)
            {
                MessageBox.Show("⚠️ الرجاء اختيار طلب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int realId = Convert.ToInt32(dgvRequests.SelectedRows[0].Cells["colRealId"].Value);

            // تأكيد الرفض
            DialogResult confirm = MessageBox.Show(
                "❌ هل أنت متأكد من رفض هذا الطلب؟\n\nسيتم إرجاعه للمخلص دون خصم أي كميات من التصاريح.",
                "تأكيد الرفض", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            try
            {
                // ✅ لا خصم عند الرفض (لأن الطلب لم يُعتمد أصلاً)
                string query = @"UPDATE TariffRequests SET 
                                 Status = 'rejected', 
                                 ReviewedDate = datetime('now') 
                                 WHERE Id = @Id";

                DatabaseHelper.ExecuteNonQuery(query,
                    DatabaseHelper.CreateParameter("@Id", realId));

                MessageBox.Show("❌ تم رفض الطلب بنجاح", "تم", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadPendingRequests();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== ✅ الدوال المساعدة لقراءة JSON ====================
        private string GetJsonValue(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.String)
                    return prop.GetString() ?? "";
                else if (prop.ValueKind == JsonValueKind.Number)
                    return prop.GetRawText();
                else if (prop.ValueKind == JsonValueKind.True)
                    return "true";
                else if (prop.ValueKind == JsonValueKind.False)
                    return "false";
            }
            return "";
        }

        private decimal GetJsonDecimal(JsonElement element, string propertyName)
        {
            if (element.TryGetProperty(propertyName, out var prop))
            {
                if (prop.ValueKind == JsonValueKind.Number)
                {
                    return prop.GetDecimal();
                }
                else if (prop.ValueKind == JsonValueKind.String && decimal.TryParse(prop.GetString(), out var result))
                {
                    return result;
                }
            }
            return 0;
        }
    }
}