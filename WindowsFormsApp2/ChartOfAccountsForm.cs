using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class ChartOfAccountsForm : Form
    {
        #region Controls
        private TreeView tvAccounts;
        private GroupBox grpDetails;
        private TextBox txtMain, txtCode, txtName, txtNameEn, txtOrder, txtDate;
        private ComboBox cmbGroup, cmbType, cmbNature, cmbReport, cmbCenter, cmbProject, cmbParentAccount; // ✅ تأكد من وجود هذا السطر
        private ComboBox cmbReportType;
        private Button btnNew, btnSave, btnDelete, btnRefresh, btnSearch, btnExit;
        private Button btnAdd, btnPrev, btnNext, btnDisplay, btnPrint, btnSettings;
        private CheckBox chkProd, chkStop;
        #endregion

        private List<Account> accounts = new List<Account>();
        private int currentIndex = -1;

        public ChartOfAccountsForm()
        {
            this.Text = "تهيئة النظام - المدخلات - الدليل المحاسبي";
            this.Size = new Size(1200, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;
            this.Font = new Font("Tahoma", 9F);

            CreateControls();
            LoadTree();
        }

        public class Account
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string NameEn { get; set; }
            public string MainAccount { get; set; }
            public string Group { get; set; }
            public string Type { get; set; }
            public string Nature { get; set; }
            public string Order { get; set; }
            public string ReportType { get; set; }
            public string Center { get; set; }
            public string Project { get; set; }
            public string LastMatch { get; set; }
            public bool IsDeactivated { get; set; }
            public bool LinkProduction { get; set; }
        }

        private void CreateControls()
        {
            int toolbarBtnStartX = 920;
            int toolbarBtnY = 8;
            int btnWidth = 65;
            int btnHeight = 32;
            int btnGap = 70;

            int treeX = 920, treeY = 55, treeWidth = 250, treeHeight = 520;
            int grpX = 30, grpY = 130, grpWidth = 870, grpHeight = 450;

            int lblMainX = 400, lblMainY = 60, txtMainX = 500, txtMainY = 60;
            int lblCodeX = 80, lblCodeY = 60, txtCodeX = 160, txtCodeY = 60;
            int lblNameX = 400, lblNameY = 100, txtNameX = 500, txtNameY = 100;
            int lblNameEnX = 80, lblNameEnY = 100, txtNameEnX = 160, txtNameEnY = 100;

            int lblGroupX = 580, lblGroupY = 25, cmbGroupX = 460, cmbGroupY = 25;
            int lblTypeX = 260, lblTypeY = 25, cmbTypeX = 140, cmbTypeY = 25;
            int lblNatureX = 580, lblNatureY = 65, cmbNatureX = 460, cmbNatureY = 65;
            int lblOrderX = 260, lblOrderY = 65, txtOrderX = 180, txtOrderY = 65;
            int lblCenterX = 580, lblCenterY = 105, pnlCenterX = 413, pnlCenterY = 105;
            int lblReportX = 253, lblReportY = 105, cmbReportX = 90, cmbReportY = 105;
            int lblProjectX = 580, lblProjectY = 145, pnlProjectX = 413, pnlProjectY = 145;
            int lblReportTypeX = 260, lblReportTypeY = 145, cmbReportTypeX = 110, cmbReportTypeY = 145;
            int lblDateX = 580, lblDateY = 185, txtDateX = 435, txtDateY = 185;
            int lblParentX = 80, lblParentY = 25, cmbParentX = 160, cmbParentY = 25;
            int cmbParentWidth = 200;

            // --- شريط الأدوات ---
            int currentBtnX = toolbarBtnStartX;

            btnSettings = CreateBtn("⚙️ إعدادات", Color.FromArgb(255, 193, 7), currentBtnX, toolbarBtnY, btnWidth + 10, btnHeight);
            currentBtnX -= btnGap + 10;

            btnAdd = CreateBtn("➕ إضافة", Color.FromArgb(100, 200, 100), currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnPrev = CreateBtn("◀️ السابق", Color.FromArgb(173, 216, 230), currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnNext = CreateBtn("▶️ التالي", Color.FromArgb(173, 216, 230), currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnDisplay = CreateBtn("👁️ عرض", Color.FromArgb(255, 228, 181), currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnPrint = CreateBtn("🖨️ طباعة", Color.FromArgb(221, 160, 221), currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            currentBtnX -= 10;
            btnExit = CreateBtn("خروج", Color.LightGray, currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnSearch = CreateBtn("بحث", Color.Moccasin, currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnRefresh = CreateBtn("تحديث", Color.Khaki, currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnDelete = CreateBtn("🗑️ حذف", Color.LightCoral, currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnSave = CreateBtn("حفظ", Color.LightBlue, currentBtnX, toolbarBtnY, btnWidth, btnHeight); currentBtnX -= btnGap;
            btnNew = CreateBtn("جديد", Color.LightGreen, currentBtnX, toolbarBtnY, btnWidth, btnHeight);

            this.Controls.AddRange(new Control[] {
                btnSettings, btnAdd, btnPrev, btnNext, btnDisplay, btnPrint,
                btnNew, btnSave, btnDelete, btnRefresh, btnSearch, btnExit
            });

            // --- الشجرة ---
            tvAccounts = new TreeView
            {
                Location = new Point(treeX, treeY),
                Size = new Size(treeWidth, treeHeight),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                Font = new Font("Tahoma", 9.5F),
                BorderStyle = BorderStyle.FixedSingle,
                RightToLeft = RightToLeft.Yes
            };
            tvAccounts.AfterSelect += TvAccounts_AfterSelect;
            this.Controls.Add(tvAccounts);

            // --- الحقول العلوية ---
            this.Controls.Add(CreateLabel("الحساب الرئيسي", lblMainX, lblMainY));
            txtMain = CreateTextBox(txtMainX, txtMainY, 120, Color.LightYellow, "0");
            this.Controls.Add(txtMain);

            this.Controls.Add(CreateLabel("رقم الحساب", lblCodeX, lblCodeY));
            txtCode = CreateTextBox(txtCodeX, txtCodeY, 120, Color.LightYellow, "");
            this.Controls.Add(txtCode);

            this.Controls.Add(CreateLabel("اسم الحساب", lblNameX, lblNameY));
            txtName = CreateTextBox(txtNameX, txtNameY, 250, Color.LightYellow, "");
            this.Controls.Add(txtName);

            this.Controls.Add(CreateLabel("الاسم الأجنبي", lblNameEnX, lblNameEnY));
            txtNameEn = CreateTextBox(txtNameEnX, txtNameEnY, 210, Color.White, "");
            this.Controls.Add(txtNameEn);

            // --- مجموعة الدليل المحاسبي ---
            grpDetails = new GroupBox
            {
                Text = "الدليل المحاسبي",
                Location = new Point(grpX, grpY),
                Size = new Size(grpWidth, grpHeight),
                Font = new Font("Tahoma", 9.5F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                RightToLeft = RightToLeft.Yes
            };
            this.Controls.Add(grpDetails);

           

            grpDetails.Controls.Add(CreateLabel("مجموعات الحسابات", lblGroupX, lblGroupY));
            cmbGroup = CreateComboBox(cmbGroupX, cmbGroupY, 110, new object[] { "" });
            cmbGroup.SelectedIndexChanged += CmbGroup_SelectedIndexChanged;
            grpDetails.Controls.Add(cmbGroup);

            grpDetails.Controls.Add(CreateLabel("تصنيف الحساب", lblTypeX, lblTypeY));
            cmbType = CreateComboBox(cmbTypeX, cmbTypeY, 110, new object[] { "" });
            grpDetails.Controls.Add(cmbType);

            grpDetails.Controls.Add(CreateLabel("النوع", lblNatureX, lblNatureY));
            cmbNature = CreateComboBox(cmbNatureX, cmbNatureY, 110, new[] { "رئيسي", "فرعي" });
            cmbNature.SelectedIndexChanged += CmbNature_SelectedIndexChanged;
            grpDetails.Controls.Add(cmbNature);

            grpDetails.Controls.Add(CreateLabel("الرتبة", lblOrderX, lblOrderY));
            txtOrder = CreateTextBox(txtOrderX, txtOrderY, 70, Color.LightYellow, "1");
            txtOrder.TextAlign = HorizontalAlignment.Center;
            grpDetails.Controls.Add(txtOrder);

            // ✅ حقول المراكز والمشاريع بدون أزرار
            grpDetails.Controls.Add(CreateLabel("إستخدام المراكز", lblCenterX, lblCenterY));
            cmbCenter = CreateComboBox(pnlCenterX, pnlCenterY, 160, new object[] { "" });
            cmbCenter.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCenter.RightToLeft = RightToLeft.Yes;
            grpDetails.Controls.Add(cmbCenter);

            grpDetails.Controls.Add(CreateLabel("طبيعة الحسابات", lblReportX, lblReportY));
            cmbReport = CreateComboBox(cmbReportX, cmbReportY, 160, new[] { "١. مدين", "٢. دائن" });
            cmbReport.BackColor = Color.LightYellow;
            grpDetails.Controls.Add(cmbReport);

            grpDetails.Controls.Add(CreateLabel("إستخدام المشاريع", lblProjectX, lblProjectY));
            cmbProject = CreateComboBox(pnlProjectX, pnlProjectY, 160, new object[] { "" });
            cmbProject.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProject.RightToLeft = RightToLeft.Yes;
            grpDetails.Controls.Add(cmbProject);

            grpDetails.Controls.Add(CreateLabel("نوع التقرير", lblReportTypeX, lblReportTypeY));
            cmbReportType = CreateComboBox(cmbReportTypeX, cmbReportTypeY, 140, new object[] { "" });
            grpDetails.Controls.Add(cmbReportType);

            grpDetails.Controls.Add(CreateLabel("تاريخ آخر مطابقة", lblDateX, lblDateY));
            txtDate = CreateTextBox(txtDateX, txtDateY, 140, Color.White, "");
            grpDetails.Controls.Add(txtDate);

            chkProd = new CheckBox { Text = "الربط مع نظام الإنتاج", Location = new Point(470, 320), AutoSize = true, RightToLeft = RightToLeft.Yes, Font = new Font("Tahoma", 9F) };
            chkStop = new CheckBox { Text = "موقف", Location = new Point(534, 340), AutoSize = true, RightToLeft = RightToLeft.Yes, ForeColor = Color.Red, Font = new Font("Tahoma", 9F, FontStyle.Bold) };
            grpDetails.Controls.Add(chkProd);
            grpDetails.Controls.Add(chkStop);

            // ==================== 🎯 أحداث الأزرار ====================
            btnNew.Click += (s, e) => ClearFields();
            btnSave.Click += (s, e) => SaveAccount();
            btnDelete.Click += (s, e) => DeleteSelected();
            btnRefresh.Click += (s, e) => { LoadTree(); UpdateParentAccountList(); GenerateAccountCode(); };
            btnExit.Click += (s, e) => this.Close();

            btnAdd.Click += (s, e) => AddAccount();
            btnPrev.Click += (s, e) => NavigateAccount(-1);
            btnNext.Click += (s, e) => NavigateAccount(1);
            btnDisplay.Click += (s, e) => ShowAllAccounts();
            btnPrint.Click += (s, e) => PrintAccounts();
            btnSearch.Click += (s, e) => SearchAccount();

            btnSettings.Click += (s, e) => OpenSettingsForm("Main");

            txtName.TextChanged += (s, e) => GenerateAccountCode();
        }

        // ==================== 🎯 الأحداث ====================

        private void CmbNature_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParentAccountList();
            GenerateAccountCode();
        }

        private void CmbParentAccount_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbParentAccount != null && cmbParentAccount.SelectedItem != null && !string.IsNullOrWhiteSpace(cmbParentAccount.SelectedItem.ToString()))
            {
                string selectedText = cmbParentAccount.SelectedItem.ToString();
                if (selectedText.Contains(" - "))
                {
                    string code = selectedText.Split(new[] { " - " }, StringSplitOptions.None)[0];
                    txtMain.Text = code;
                }
                else
                {
                    txtMain.Text = selectedText;
                }
                GenerateAccountCode();
            }
            else
            {
                txtMain.Text = "0";
                GenerateAccountCode();
            }
        }

        // ✅ توليد رقم الحساب تلقائياً مع فحص الأخطاء
        private void GenerateAccountCode()
        {
            if (!string.IsNullOrWhiteSpace(txtCode.Text) && txtCode.Text != "0")
                return;

            string parentCode = txtMain.Text;

            if (string.IsNullOrWhiteSpace(parentCode) || parentCode == "0")
            {
                var independentAccounts = accounts.Where(a => string.IsNullOrWhiteSpace(a.MainAccount) || a.MainAccount == "0").ToList();
                int maxCode = 0;

                foreach (var acc in independentAccounts)
                {
                    if (int.TryParse(acc.Code, out int code) && code > maxCode && code < 100)
                        maxCode = code;
                }

                txtCode.Text = (maxCode + 1).ToString();
                txtOrder.Text = "1";
            }
            else
            {
                var childAccounts = accounts.Where(a => a.MainAccount == parentCode).ToList();
                int maxSuffix = 0;

                foreach (var acc in childAccounts)
                {
                    if (int.TryParse(acc.Code, out int code))
                    {
                        string suffixStr = acc.Code.Substring(parentCode.Length);
                        if (int.TryParse(suffixStr, out int suffix) && suffix > maxSuffix)
                        {
                            maxSuffix = suffix;
                        }
                    }
                }

                int newSuffix = maxSuffix + 1;
                string newCode = parentCode + newSuffix.ToString();

                txtCode.Text = newCode;
                txtOrder.Text = "2";
            }
        }

        // ✅ دالة تحديث قائمة الحسابات الرئيسية مع فحص الأخطاء
        private void UpdateParentAccountList()
        {
            // ✅ فحص مهم لمنع الخطأ
            if (cmbParentAccount == null) return;

            cmbParentAccount.Items.Clear();
            cmbParentAccount.Items.Add("");

            var allAccounts = accounts.OrderBy(a => a.Code).ToList();
            foreach (var acc in allAccounts)
            {
                cmbParentAccount.Items.Add(acc.Code + " - " + acc.Name);
            }

            cmbParentAccount.SelectedIndex = 0;
        }

        // ==================== 🌳 دوال الشجرة ====================

        private void LoadTree()
        {
            tvAccounts.Nodes.Clear();

            var rootAccounts = accounts.Where(a => string.IsNullOrWhiteSpace(a.MainAccount) || a.MainAccount == "0").OrderBy(a => a.Code).ToList();

            foreach (var rootAcc in rootAccounts)
            {
                TreeNode rootNode = new TreeNode($"{rootAcc.Code} - {rootAcc.Name}")
                {
                    Tag = rootAcc.Code,
                    ForeColor = Color.Blue,
                    NodeFont = new Font("Tahoma", 9.5F, FontStyle.Bold)
                };

                AddChildNodes(rootNode, rootAcc.Code);
                tvAccounts.Nodes.Add(rootNode);
            }

            tvAccounts.ExpandAll();
        }

        private void AddChildNodes(TreeNode parentNode, string parentCode)
        {
            var children = accounts.Where(a => a.MainAccount == parentCode).OrderBy(a => a.Code).ToList();
            foreach (var child in children)
            {
                TreeNode childNode = new TreeNode($"{child.Code} - {child.Name}")
                {
                    Tag = child.Code,
                    ForeColor = Color.Black
                };
                parentNode.Nodes.Add(childNode);
                AddChildNodes(childNode, child.Code);
            }
        }

        private void TvAccounts_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag != null)
            {
                string code = e.Node.Tag.ToString();
                var acc = accounts.FirstOrDefault(a => a.Code == code);
                if (acc != null) LoadAccountToFields(acc);
            }
        }

        // ==================== 🎯 ربط شاشة الإعدادات ====================

        private void OpenSettingsForm(string context)
        {
            var settingsForm = new ChartOfAccountsSettingsForm();
            FillSettingsWithData(settingsForm);

            if (settingsForm.ShowDialog() == DialogResult.OK)
            {
                RefreshComboBoxesFromSettings(settingsForm);
            }
        }

        private void FillSettingsWithData(ChartOfAccountsSettingsForm settingsForm)
        {
            if (cmbGroup != null)
            {
                foreach (var item in cmbGroup.Items)
                {
                    if (!string.IsNullOrWhiteSpace(item?.ToString()))
                        settingsForm.AccountGroups.Add(new ChartOfAccountsSettingsForm.AccountGroupData { Id = settingsForm.AccountGroups.Count + 1, Name = item.ToString() });
                }
            }
            if (cmbType != null)
            {
                foreach (var item in cmbType.Items)
                {
                    if (!string.IsNullOrWhiteSpace(item?.ToString()))
                        settingsForm.AccountClassifications.Add(new ChartOfAccountsSettingsForm.AccountClassificationData { Id = settingsForm.AccountClassifications.Count + 1, Name = item.ToString() });
                }
            }
            if (cmbNature != null)
            {
                foreach (var item in cmbNature.Items)
                {
                    if (!string.IsNullOrWhiteSpace(item?.ToString()))
                        settingsForm.AccountTypes.Add(new ChartOfAccountsSettingsForm.AccountTypeData { Id = settingsForm.AccountTypes.Count + 1, Name = item.ToString() });
                }
            }
            if (cmbReportType != null)
            {
                foreach (var item in cmbReportType.Items)
                {
                    if (!string.IsNullOrWhiteSpace(item?.ToString()) && item.ToString() != "قائمة دخل" && item.ToString() != "ميزانية")
                        settingsForm.ReportTypes.Add(new ChartOfAccountsSettingsForm.ReportTypeData { Id = settingsForm.ReportTypes.Count + 1, Name = item.ToString() });
                }
            }
        }

        private void RefreshComboBoxesFromSettings(ChartOfAccountsSettingsForm settingsForm)
        {
            if (cmbGroup != null) { cmbGroup.Items.Clear(); cmbGroup.Items.Add(""); foreach (var g in settingsForm.AccountGroups) cmbGroup.Items.Add(g.Name); if (cmbGroup.Items.Count > 0) cmbGroup.SelectedIndex = 0; }
            if (cmbType != null) { cmbType.Items.Clear(); cmbType.Items.Add(""); foreach (var c in settingsForm.AccountClassifications) cmbType.Items.Add(c.Name); if (cmbType.Items.Count > 0) cmbType.SelectedIndex = 0; }
            if (cmbNature != null) { cmbNature.Items.Clear(); foreach (var t in settingsForm.AccountTypes) cmbNature.Items.Add(t.Name); if (cmbNature.Items.Count == 0) cmbNature.Items.AddRange(new[] { "رئيسي", "فرعي" }); if (cmbNature.Items.Count > 0) cmbNature.SelectedIndex = 0; }

            // ✅ تحديث حقل نوع التقرير مع فحص الأخطاء
            if (cmbReportType != null)
            {
                cmbReportType.Items.Clear();
                cmbReportType.Items.Add("");
                foreach (var r in settingsForm.ReportTypes) cmbReportType.Items.Add(r.Name);
                if (cmbReportType.Items.Count == 0) cmbReportType.Items.AddRange(new[] { "قائمة دخل", "ميزانية" });
                if (cmbReportType.Items.Count > 0) cmbReportType.SelectedIndex = 0;
            }

            MessageBox.Show($"✅ تم تحديث القوائم", "تحديث", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== 🎯 وظائف الأزرار والمنطق ====================

        private void AddAccount()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text) || string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("⚠️ يرجى إدخال رقم الحساب واسمه", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (accounts.Any(a => a.Code == txtCode.Text))
            {
                MessageBox.Show("⚠️ رقم الحساب موجود مسبقاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            accounts.Add(new Account
            {
                Code = txtCode.Text,
                Name = txtName.Text,
                NameEn = txtNameEn.Text,
                MainAccount = txtMain.Text,
                Group = cmbGroup?.SelectedItem?.ToString(),
                Type = cmbNature?.SelectedItem?.ToString(),
                Nature = cmbReport?.SelectedItem?.ToString(),
                Order = txtOrder.Text,
                ReportType = cmbReportType?.SelectedItem?.ToString(),
                Center = cmbCenter?.SelectedItem?.ToString(),
                Project = cmbProject?.SelectedItem?.ToString(),
                LastMatch = txtDate.Text,
                IsDeactivated = chkStop.Checked,
                LinkProduction = chkProd.Checked
            });

            LoadTree();
            UpdateParentAccountList();
            MessageBox.Show("✅ تم إضافة الحساب بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearFields();
        }

        private void NavigateAccount(int direction)
        {
            if (accounts.Count == 0) return;
            currentIndex += direction;
            if (currentIndex < 0) currentIndex = accounts.Count - 1;
            if (currentIndex >= accounts.Count) currentIndex = 0;
            LoadAccountToFields(accounts[currentIndex]);
        }

        private void SearchAccount()
        {
            using (var f = new Form { Text = "🔍 بحث", Size = new Size(350, 150), StartPosition = FormStartPosition.CenterParent, RightToLeft = RightToLeft.Yes, RightToLeftLayout = true, FormBorderStyle = FormBorderStyle.FixedDialog })
            {
                var txt = new TextBox { Location = new Point(20, 50), Size = new Size(280, 23) };
                var btn = new Button { Text = "بحث", Location = new Point(225, 80), Size = new Size(75, 25), DialogResult = DialogResult.OK };
                f.Controls.AddRange(new Control[] { new Label { Text = "أدخل رقم أو اسم الحساب:", Location = new Point(20, 20), AutoSize = true }, txt, btn });
                f.AcceptButton = btn;
                if (f.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txt.Text))
                {
                    var res = accounts.FirstOrDefault(a => a.Code.Contains(txt.Text) || a.Name.Contains(txt.Text));
                    if (res != null) { LoadAccountToFields(res); MessageBox.Show($"✅ تم العثور على: {res.Code} - {res.Name}", "نتيجة"); }
                    else MessageBox.Show("❌ لم يتم العثور", "بحث");
                }
            }
        }

        private void ShowAllAccounts()
        {
            using (var f = new Form { Text = "👁️ عرض الحسابات", Size = new Size(800, 500), StartPosition = FormStartPosition.CenterParent, RightToLeft = RightToLeft.Yes, RightToLeftLayout = true })
            {
                var dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true };
                dgv.Columns.Add("Code", "رقم الحساب");
                dgv.Columns.Add("Name", "الاسم");
                dgv.Columns.Add("Type", "النوع");
                dgv.Columns.Add("MainAccount", "الحساب الرئيسي");

                foreach (var a in accounts.OrderBy(x => x.Code))
                    dgv.Rows.Add(a.Code, a.Name, a.Type, a.MainAccount ?? "-");

                dgv.CellDoubleClick += (s, e) => {
                    if (dgv.CurrentRow != null)
                    {
                        var a = accounts.FirstOrDefault(x => x.Code == dgv.CurrentRow.Cells[0].Value?.ToString());
                        if (a != null) { LoadAccountToFields(a); f.Close(); }
                    }
                };
                f.Controls.Add(dgv); f.ShowDialog();
            }
        }

        private void PrintAccounts() => MessageBox.Show("🖨️ جاري الطباعة...", "طباعة", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void DeleteSelected()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text)) { MessageBox.Show("⚠️ أدخل رقم الحساب للحذف", "تنبيه"); return; }

            if (MessageBox.Show($"🗑️ حذف الحساب {txtCode.Text} - {txtName.Text}؟", "تأكيد", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                accounts.RemoveAll(a => a.Code == txtCode.Text);
                LoadTree();
                UpdateParentAccountList();
                ClearFields();
                MessageBox.Show("✅ تم الحذف", "نجاح");
            }
        }

        private void SaveAccount()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text)) { MessageBox.Show("⚠️ أدخل رقم الحساب", "تنبيه"); return; }
            var ex = accounts.FirstOrDefault(a => a.Code == txtCode.Text);
            if (ex != null)
            {
                ex.Name = txtName.Text; ex.NameEn = txtNameEn.Text; ex.MainAccount = txtMain.Text;
                ex.Group = cmbGroup?.SelectedItem?.ToString(); ex.Type = cmbNature?.SelectedItem?.ToString();
                ex.Nature = cmbReport?.SelectedItem?.ToString(); ex.Order = txtOrder.Text;
                ex.ReportType = cmbReportType?.SelectedItem?.ToString();
                ex.Center = cmbCenter?.SelectedItem?.ToString();
                ex.Project = cmbProject?.SelectedItem?.ToString(); ex.LastMatch = txtDate.Text;
                ex.IsDeactivated = chkStop.Checked; ex.LinkProduction = chkProd.Checked;
                LoadTree();
                UpdateParentAccountList();
                MessageBox.Show("✅ تم التحديث", "نجاح");
            }
            else AddAccount();
        }

        private void LoadAccountToFields(Account acc)
        {
            txtCode.Text = acc.Code; txtName.Text = acc.Name; txtNameEn.Text = acc.NameEn; txtMain.Text = acc.MainAccount ?? "0";
            if (cmbGroup != null) cmbGroup.SelectedItem = acc.Group;
            if (cmbNature != null) cmbNature.SelectedItem = acc.Type;
            if (cmbReport != null) cmbReport.SelectedItem = acc.Nature;
            txtOrder.Text = acc.Order ?? "1";
            if (cmbReportType != null) cmbReportType.SelectedItem = acc.ReportType;
            if (cmbCenter != null) cmbCenter.SelectedItem = acc.Center;
            if (cmbProject != null) cmbProject.SelectedItem = acc.Project;
            txtDate.Text = acc.LastMatch; chkStop.Checked = acc.IsDeactivated; chkProd.Checked = acc.LinkProduction;

            if (!string.IsNullOrWhiteSpace(acc.MainAccount) && acc.MainAccount != "0")
            {
                if (cmbParentAccount != null)
                {
                    foreach (var item in cmbParentAccount.Items)
                    {
                        if (item.ToString().StartsWith(acc.MainAccount + " - "))
                        {
                            cmbParentAccount.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (cmbParentAccount != null) cmbParentAccount.SelectedIndex = 0;
            }
        }

        private void CmbGroup_SelectedIndexChanged(object sender, EventArgs e) { }

        private void RefreshData()
        {
            ClearFields();
            MessageBox.Show("🔄 تم التحديث", "تحديث", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ClearFields()
        {
            txtMain.Text = "0"; txtCode.Clear(); txtName.Clear(); txtNameEn.Clear(); txtOrder.Clear(); txtDate.Clear();
            if (cmbGroup != null) cmbGroup.SelectedIndex = -1;
            if (cmbNature != null) cmbNature.SelectedIndex = -1;
            if (cmbReport != null) cmbReport.SelectedIndex = -1;
            if (cmbReportType != null) cmbReportType.SelectedIndex = -1;
            if (cmbParentAccount != null) cmbParentAccount.SelectedIndex = 0;
            if (cmbCenter != null) cmbCenter.SelectedIndex = -1;
            if (cmbProject != null) cmbProject.SelectedIndex = -1;
            chkProd.Checked = false; chkStop.Checked = false;
            txtCode.Focus();
            GenerateAccountCode();
        }

        // ==================== أدوات الإنشاء ====================
        private Label CreateLabel(string t, int x, int y) => new Label { Text = t, Location = new Point(x, y), AutoSize = true, Font = new Font("Tahoma", 9F), RightToLeft = RightToLeft.Yes };
        private TextBox CreateTextBox(int x, int y, int w, Color b, string t, Color? f = null, FontStyle s = FontStyle.Regular) => new TextBox { Location = new Point(x, y), Size = new Size(w, 23), BackColor = b, ForeColor = f ?? Color.Black, Font = new Font("Tahoma", 9F, s), RightToLeft = RightToLeft.Yes, Text = t };
        private ComboBox CreateComboBox(int x, int y, int w, object[] items) { var c = new ComboBox { Location = new Point(x, y), Size = new Size(w, 23), DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Tahoma", 9F), RightToLeft = RightToLeft.Yes }; c.Items.AddRange(items); return c; }
        private Button CreateBtn(string t, Color c, int x, int y, int w, int h) => new Button { Text = t, Location = new Point(x, y), Size = new Size(w, h), BackColor = c, ForeColor = Color.Black, FlatStyle = FlatStyle.Flat, Font = new Font("Tahoma", 9F, FontStyle.Bold) };
    }
}