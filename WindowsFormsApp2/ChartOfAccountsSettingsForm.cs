using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class ChartOfAccountsSettingsForm : Form
    {
        #region Controls
        private Panel pnlTop;
        private Panel pnlBottom;
        private TabControl tabControl;
        private TabPage tabAccountTypes;
        private TabPage tabAccountGroups;
        private TabPage tabAccountClassifications;
        private TabPage tabReportTypes;

        private DataGridView dgvAccountTypes;
        private DataGridView dgvAccountGroups;
        private DataGridView dgvAccountClassifications;
        private DataGridView dgvReportTypes;

        private TextBox txtUser, txtDate, txtMachine, txtModifiedUser, txtModifiedDate, txtModifiedMachine;
        private TextBox txtPrintCount, txtEditCount;

        private Button btnNew, btnSave, btnDelete, btnRefresh, btnExit;
        #endregion

        // ==================== 🗄️ قوائم البيانات ====================
        public List<AccountTypeData> AccountTypes { get; set; } = new List<AccountTypeData>();
        public List<AccountGroupData> AccountGroups { get; set; } = new List<AccountGroupData>();
        public List<AccountClassificationData> AccountClassifications { get; set; } = new List<AccountClassificationData>();
        public List<ReportTypeData> ReportTypes { get; set; } = new List<ReportTypeData>();

        // ==================== 🗄️ نماذج البيانات ====================
        public class AccountTypeData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string NameEn { get; set; }
            public bool AffectedByOperations { get; set; }
        }

        public class AccountGroupData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string NameEn { get; set; }
            public bool AffectedByOperations { get; set; }
        }

        public class AccountClassificationData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string NameEn { get; set; }
            public bool AffectedByOperations { get; set; }
        }

        public class ReportTypeData
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string NameEn { get; set; }
            public bool AffectedByOperations { get; set; }
        }

        // ==================== 🗄️ كلاس مساعد للتحميل من ملف ====================
        private class SettingsData
        {
            public List<AccountTypeData> AccountTypes { get; set; }
            public List<AccountGroupData> AccountGroups { get; set; }
            public List<AccountClassificationData> AccountClassifications { get; set; }
            public List<ReportTypeData> ReportTypes { get; set; }
        }

        public ChartOfAccountsSettingsForm()
        {
            this.Text = "تهيئة الدليل المحاسبي";
            this.Size = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterParent;
            this.RightToLeft = RightToLeft.Yes;
            this.RightToLeftLayout = true;
            this.BackColor = Color.White;
            this.Font = new Font("Tahoma", 9F);
            this.WindowState = FormWindowState.Maximized;

            CreateControls();
            LoadDataFromFile(); // ✅ تحميل البيانات المحفوظة من الملف عند البدء
        }

        private void CreateControls()
        {
            this.SuspendLayout();

            // ==================== 1. شريط الأدوات العلوي ====================
            pnlTop = new Panel
            {
                Dock = DockStyle.Top,
                Height = 45,
                BackColor = Color.FromArgb(240, 240, 240),
                BorderStyle = BorderStyle.FixedSingle
            };

            btnNew = CreateButton("➕ جديد", Color.LightGreen, 10, 8);
            btnSave = CreateButton("💾 حفظ", Color.LightBlue, 85, 8);
            btnDelete = CreateButton("🗑️ حذف", Color.LightCoral, 160, 8);
            btnRefresh = CreateButton("🔄 تحديث", Color.Khaki, 235, 8);
            btnExit = CreateButton("❌ خروج", Color.LightGray, 310, 8);

            pnlTop.Controls.AddRange(new Control[] { btnNew, btnSave, btnDelete, btnRefresh, btnExit });
            this.Controls.Add(pnlTop);

            // ==================== 2. الشريط السفلي ====================
            pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 70,
                BackColor = Color.FromArgb(230, 230, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            int labelX = 750, inputX = 600, labelX2 = 400, inputX2 = 250;
            int row1Y = 10, row2Y = 40;

            pnlBottom.Controls.Add(CreateLabel("مدخل السجل:", labelX, row1Y));
            txtUser = CreateTextBox(inputX, row1Y, 140, "1 - مدير النظام");
            pnlBottom.Controls.Add(txtUser);

            pnlBottom.Controls.Add(CreateLabel("تاريخ الإدخال:", labelX2, row1Y));
            txtDate = CreateTextBox(inputX2, row1Y, 140, DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            pnlBottom.Controls.Add(txtDate);

            pnlBottom.Controls.Add(CreateLabel("معدل السجل:", labelX, row2Y));
            txtModifiedUser = CreateTextBox(inputX, row2Y, 140, "");
            pnlBottom.Controls.Add(txtModifiedUser);

            pnlBottom.Controls.Add(CreateLabel("تاريخ آخر تعديل:", labelX2, row2Y));
            txtModifiedDate = CreateTextBox(inputX2, row2Y, 140, "");
            pnlBottom.Controls.Add(txtModifiedDate);

            pnlBottom.Controls.Add(CreateLabel("الجهاز المدخل:", 100, row1Y));
            txtMachine = CreateTextBox(10, row1Y, 80, "PC-01");
            pnlBottom.Controls.Add(txtMachine);

            pnlBottom.Controls.Add(CreateLabel("الجهاز المعدل:", 100, row2Y));
            txtModifiedMachine = CreateTextBox(10, row2Y, 80, "");
            pnlBottom.Controls.Add(txtModifiedMachine);

            pnlBottom.Controls.Add(CreateLabel("مرات الطباعة:", 550, row1Y));
            txtPrintCount = CreateTextBox(480, row1Y, 60, "0");
            pnlBottom.Controls.Add(txtPrintCount);

            pnlBottom.Controls.Add(CreateLabel("مرات التعديل:", 550, row2Y));
            txtEditCount = CreateTextBox(480, row2Y, 60, "0");
            pnlBottom.Controls.Add(txtEditCount);

            this.Controls.Add(pnlBottom);

            // ==================== 3. التبويبات والجدول ====================
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Alignment = TabAlignment.Top,
                ItemSize = new Size(150, 95),
                SizeMode = TabSizeMode.Normal,
                Appearance = TabAppearance.Normal,
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                Padding = new Point(8, 8)
            };

            tabAccountTypes = new TabPage("أنواع الحسابات");
            tabAccountGroups = new TabPage("مجموعات الحسابات");
            tabAccountClassifications = new TabPage("تصنيف الحسابات");
            tabReportTypes = new TabPage("أنواع تقارير الحسابات");

            tabControl.TabPages.Add(tabAccountTypes);
            tabControl.TabPages.Add(tabAccountGroups);
            tabControl.TabPages.Add(tabAccountClassifications);
            tabControl.TabPages.Add(tabReportTypes);

            SetupDataGridView(tabAccountTypes, out dgvAccountTypes);
            SetupDataGridView(tabAccountGroups, out dgvAccountGroups);
            SetupDataGridView(tabAccountClassifications, out dgvAccountClassifications);
            SetupDataGridView(tabReportTypes, out dgvReportTypes);

            this.Controls.Add(tabControl);

            // ==================== 4. الأحداث ====================
            btnNew.Click += (s, e) => AddNewRow();
            btnSave.Click += (s, e) => SaveData();
            btnDelete.Click += (s, e) => DeleteRow();
            btnRefresh.Click += (s, e) => RefreshData();
            btnExit.Click += (s, e) => this.Close();

            this.ResumeLayout(false);
        }

        // ==================== 💾 دوال الحفظ والقراءة من ملف ====================

        private string GetDataFilePath()
        {
            return Path.Combine(Application.StartupPath, "ChartOfAccountsSettings.json");
        }

        private void SaveDataToFile()
        {
            try
            {
                var data = new SettingsData
                {
                    AccountTypes = this.AccountTypes,
                    AccountGroups = this.AccountGroups,
                    AccountClassifications = this.AccountClassifications,
                    ReportTypes = this.ReportTypes
                };

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string json = JsonSerializer.Serialize(data, options);
                File.WriteAllText(GetDataFilePath(), json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في حفظ الملف:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataFromFile()
        {
            try
            {
                string filePath = GetDataFilePath();
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    var data = JsonSerializer.Deserialize<SettingsData>(json);

                    if (data != null)
                    {
                        this.AccountTypes = data.AccountTypes ?? new List<AccountTypeData>();
                        this.AccountGroups = data.AccountGroups ?? new List<AccountGroupData>();
                        this.AccountClassifications = data.AccountClassifications ?? new List<AccountClassificationData>();
                        this.ReportTypes = data.ReportTypes ?? new List<ReportTypeData>();

                        LoadDataToGrids();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في قراءة الملف:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==================== 📊 دوال الجداول ====================

        private void LoadDataToGrids()
        {
            LoadGrid(dgvAccountTypes, AccountTypes);
            LoadGrid(dgvAccountGroups, AccountGroups);
            LoadGrid(dgvAccountClassifications, AccountClassifications);
            LoadGrid(dgvReportTypes, ReportTypes);
        }

        private void LoadGrid<T>(DataGridView dgv, List<T> items) where T : class
        {
            dgv.Rows.Clear();
            int id = 1;

            foreach (var item in items)
            {
                var props = item.GetType().GetProperties();
                object[] values = new object[4];

                foreach (var prop in props)
                {
                    if (prop.Name == "Id") values[0] = prop.GetValue(item);
                    else if (prop.Name == "Name") values[1] = prop.GetValue(item);
                    else if (prop.Name == "NameEn") values[2] = prop.GetValue(item);
                    else if (prop.Name == "AffectedByOperations") values[3] = prop.GetValue(item);
                }
                dgv.Rows.Add(values);
                id++;
            }

            while (dgv.Rows.Count < 5)
            {
                dgv.Rows.Add(id, "", "", false);
                id++;
            }
        }

        private void SetupDataGridView(TabPage tabPage, out DataGridView dgv)
        {
            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RightToLeft = RightToLeft.Yes,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                RowTemplate = { Height = 28 },
                ColumnHeadersDefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleCenter,
                    Font = new Font("Tahoma", 9F, FontStyle.Bold),
                    BackColor = Color.FromArgb(200, 230, 255),
                    WrapMode = DataGridViewTriState.True
                },
                DefaultCellStyle =
                {
                    Alignment = DataGridViewContentAlignment.MiddleRight,
                    Font = new Font("Tahoma", 9F)
                },
                ColumnHeadersHeight = 35,
                ColumnHeadersVisible = true,
                AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = Color.FromArgb(255, 250, 220)
                },
                Padding = new Padding(0, 10, 0, 0)
            };

            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "الرقم", Width = 100 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "Name", HeaderText = "اسم النوع", Width = 300 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "NameEn", HeaderText = "الاسم الأجنبي", Width = 250 });
            dgv.Columns.Add(new DataGridViewCheckBoxColumn { Name = "AffectedByOperations", HeaderText = "يتأثر بالعمليات", Width = 150 });

            tabPage.Controls.Add(dgv);
        }

        private void AddNewRow()
        {
            DataGridView dgv = GetCurrentGridView();
            if (dgv != null)
            {
                int newId = dgv.Rows.Count + 1;
                dgv.Rows.Add(newId, "", "", false);
            }
        }

        private void DeleteRow()
        {
            DataGridView dgv = GetCurrentGridView();
            if (dgv != null && dgv.CurrentRow != null)
            {
                dgv.Rows.Remove(dgv.CurrentRow);
                ReNumberRows(dgv);
            }
        }

        private void ReNumberRows(DataGridView dgv)
        {
            for (int i = 0; i < dgv.Rows.Count; i++)
            {
                dgv.Rows[i].Cells["Id"].Value = i + 1;
            }
        }

        private DataGridView GetCurrentGridView()
        {
            if (tabControl.SelectedTab == tabAccountTypes) return dgvAccountTypes;
            if (tabControl.SelectedTab == tabAccountGroups) return dgvAccountGroups;
            if (tabControl.SelectedTab == tabAccountClassifications) return dgvAccountClassifications;
            if (tabControl.SelectedTab == tabReportTypes) return dgvReportTypes;
            return null;
        }

        private void SaveData()
        {
            try
            {
                // حفظ أنواع الحسابات
                AccountTypes.Clear();
                foreach (DataGridViewRow row in dgvAccountTypes.Rows)
                {
                    if (row.Cells["Name"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["Name"].Value.ToString()))
                    {
                        int id = 0; int.TryParse(row.Cells["Id"].Value?.ToString() ?? "0", out id);
                        AccountTypes.Add(new AccountTypeData
                        {
                            Id = id,
                            Name = row.Cells["Name"].Value?.ToString() ?? "",
                            NameEn = row.Cells["NameEn"].Value?.ToString() ?? "",
                            AffectedByOperations = Convert.ToBoolean(row.Cells["AffectedByOperations"].Value ?? false)
                        });
                    }
                }

                // حفظ مجموعات الحسابات
                AccountGroups.Clear();
                foreach (DataGridViewRow row in dgvAccountGroups.Rows)
                {
                    if (row.Cells["Name"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["Name"].Value.ToString()))
                    {
                        int id = 0; int.TryParse(row.Cells["Id"].Value?.ToString() ?? "0", out id);
                        AccountGroups.Add(new AccountGroupData
                        {
                            Id = id,
                            Name = row.Cells["Name"].Value?.ToString() ?? "",
                            NameEn = row.Cells["NameEn"].Value?.ToString() ?? "",
                            AffectedByOperations = Convert.ToBoolean(row.Cells["AffectedByOperations"].Value ?? false)
                        });
                    }
                }

                // حفظ تصنيف الحسابات
                AccountClassifications.Clear();
                foreach (DataGridViewRow row in dgvAccountClassifications.Rows)
                {
                    if (row.Cells["Name"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["Name"].Value.ToString()))
                    {
                        int id = 0; int.TryParse(row.Cells["Id"].Value?.ToString() ?? "0", out id);
                        AccountClassifications.Add(new AccountClassificationData
                        {
                            Id = id,
                            Name = row.Cells["Name"].Value?.ToString() ?? "",
                            NameEn = row.Cells["NameEn"].Value?.ToString() ?? "",
                            AffectedByOperations = Convert.ToBoolean(row.Cells["AffectedByOperations"].Value ?? false)
                        });
                    }
                }

                // حفظ أنواع التقارير
                ReportTypes.Clear();
                foreach (DataGridViewRow row in dgvReportTypes.Rows)
                {
                    if (row.Cells["Name"].Value != null && !string.IsNullOrWhiteSpace(row.Cells["Name"].Value.ToString()))
                    {
                        int id = 0; int.TryParse(row.Cells["Id"].Value?.ToString() ?? "0", out id);
                        ReportTypes.Add(new ReportTypeData
                        {
                            Id = id,
                            Name = row.Cells["Name"].Value?.ToString() ?? "",
                            NameEn = row.Cells["NameEn"].Value?.ToString() ?? "",
                            AffectedByOperations = Convert.ToBoolean(row.Cells["AffectedByOperations"].Value ?? false)
                        });
                    }
                }

                // تحديث معلومات التعديل
                txtModifiedDate.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                txtModifiedUser.Text = "1 - مدير النظام";
                int editCount = 0; int.TryParse(txtEditCount.Text, out editCount);
                txtEditCount.Text = (editCount + 1).ToString();

                // ✅ حفظ البيانات في ملف
                SaveDataToFile();

                MessageBox.Show($"✅ تم حفظ الإعدادات بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"❌ خطأ في الحفظ:\n{ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshData()
        {
            LoadDataFromFile();
            MessageBox.Show("🔄 تم تحديث البيانات", "تحديث", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ==================== أدوات الإنشاء ====================
        private Button CreateButton(string text, Color color, int x, int y)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(65, 30),
                BackColor = color,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Tahoma", 9F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Tahoma", 8.5F),
                RightToLeft = RightToLeft.Yes
            };
        }

        private TextBox CreateTextBox(int x, int y, int width, string text)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(width, 23),
                Text = text,
                ReadOnly = true,
                BackColor = Color.White,
                Font = new Font("Tahoma", 8.5F),
                RightToLeft = RightToLeft.Yes
            };
        }
    }
}