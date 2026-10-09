using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    public partial class GroupsForm : Form
    {
        private GroupsRepository groupsRepo = new GroupsRepository();
        private List<GroupData> groups = new List<GroupData>();
        private int currentIndex = -1;

        public GroupsForm()
        {
            InitializeComponent();
            InitializeGrid();
            LoadDataFromFile();
            ClearFields();
            UpdateNavigation();
        }

        private void InitializeGrid()
        {
            dgvGroups.Columns.Clear();
            dgvGroups.Columns.Add("GroupNo", "رقم المجموعة");
            dgvGroups.Columns.Add("GroupName", "اسم المجموعة");
            dgvGroups.Columns.Add("ForeignName", "الاسم الأجنبي");
            dgvGroups.Columns.Add("IsActive", "الحالة");

            dgvGroups.Columns["GroupNo"].Width = 150;
            dgvGroups.Columns["GroupName"].Width = 250;
            dgvGroups.Columns["ForeignName"].Width = 200;
            dgvGroups.Columns["IsActive"].Width = 100;

            dgvGroups.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvGroups.ReadOnly = true;
            dgvGroups.AllowUserToAddRows = false;
        }

        private void LoadDataFromFile()
        {
            try
            {
                groups = groupsRepo.GetAll();
                RefreshGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل البيانات: {ex.Message}", "خطأ",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGrid()
        {
            dgvGroups.Rows.Clear();
            foreach (var group in groups)
            {
                dgvGroups.Rows.Add(
                    group.GroupNo,
                    group.GroupName,
                    group.ForeignName ?? "",
                    group.IsActive ? "✅ نشط" : "❌ موقف"
                );
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtGroupNo.Text))
            {
                MessageBox.Show("⚠️ رقم المجموعة مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGroupNo.Focus();
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtGroupName.Text))
            {
                MessageBox.Show("⚠️ اسم المجموعة مطلوب", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGroupName.Focus();
                return false;
            }

            string currentGroupNo = currentIndex >= 0 && currentIndex < groups.Count ? groups[currentIndex].GroupNo : "";
            if (groupsRepo.Exists(txtGroupNo.Text.Trim()) && txtGroupNo.Text.Trim() != currentGroupNo)
            {
                MessageBox.Show("⚠️ رقم المجموعة موجود مسبقاً", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtGroupNo.Focus();
                return false;
            }

            // ✅ هذا هو السطر المفقود الذي يسبب الخطأ
            return true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return;

            try
            {
                var group = new GroupData
                {
                    GroupNo = txtGroupNo.Text.Trim(),
                    GroupName = txtGroupName.Text.Trim(),
                    ForeignName = txtGroupNameEn != null ? txtGroupNameEn.Text.Trim() : "",
                    IsActive = chkActive != null ? chkActive.Checked : true,
                    Description = ""
                };

                if (currentIndex >= 0 && currentIndex < groups.Count)
                {
                    if (groupsRepo.Update(group))
                    {
                        MessageBox.Show("✅ تم التعديل بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("❌ فشل التعديل", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    if (groupsRepo.Add(group))
                    {
                        MessageBox.Show("✅ تمت الإضافة بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("❌ فشل الإضافة", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                LoadDataFromFile();
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ: {ex.Message}", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currentIndex < 0 || currentIndex >= groups.Count)
            {
                MessageBox.Show("⚠️ لا توجد مجموعة محددة", "تنبيه", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var group = groups[currentIndex];

            if (MessageBox.Show($"🗑️ هل تريد حذف المجموعة:\n{group.GroupNo} - {group.GroupName}؟",
                "تأكيد الحذف", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (groupsRepo.Delete(group.GroupNo))
                {
                    MessageBox.Show("✅ تم الحذف بنجاح", "نجاح", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadDataFromFile();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show("❌ فشل الحذف", "خطأ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnNew_Click(object sender, EventArgs e) => ClearFields();

        private void btnSearch_Click(object sender, EventArgs e)
        {
            using (var f = new Form { Text = "🔍 بحث", Size = new Size(350, 120), StartPosition = FormStartPosition.CenterParent, RightToLeft = RightToLeft.Yes })
            {
                var txt = new TextBox { Location = new Point(20, 40), Size = new Size(280, 23) };
                var btn = new Button { Text = "بحث", Location = new Point(220, 70), DialogResult = DialogResult.OK };
                f.Controls.AddRange(new Control[] {
                    new Label { Text = "أدخل رقم أو اسم المجموعة:", Location = new Point(20, 15), AutoSize = true },
                    txt, btn
                });
                f.AcceptButton = btn;

                if (f.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(txt.Text))
                {
                    var found = groups.FirstOrDefault(g => g.GroupNo.Contains(txt.Text) || g.GroupName.Contains(txt.Text));
                    if (found != null)
                    {
                        currentIndex = groups.IndexOf(found);
                        LoadGroupToForm(found);
                        UpdateNavigation();
                        dgvGroups.ClearSelection();
                        dgvGroups.Rows[currentIndex].Selected = true;
                    }
                    else
                    {
                        MessageBox.Show("❌ لم يتم العثور", "بحث", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void dgvGroups_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < groups.Count)
            {
                currentIndex = e.RowIndex;
                LoadGroupToForm(groups[currentIndex]);
                UpdateNavigation();
            }
        }

        private void NavigateTo(int index)
        {
            if (index < 0 || index >= groups.Count) return;
            currentIndex = index;
            LoadGroupToForm(groups[currentIndex]);
            UpdateNavigation();
            dgvGroups.ClearSelection();
            dgvGroups.Rows[currentIndex].Selected = true;
        }

        private void btnFirst_Click(object sender, EventArgs e) => NavigateTo(0);
        private void btnPrevious_Click(object sender, EventArgs e) => NavigateTo(currentIndex - 1);
        private void btnNext_Click(object sender, EventArgs e) => NavigateTo(currentIndex + 1);
        private void btnLast_Click(object sender, EventArgs e) => NavigateTo(groups.Count - 1);

        private void UpdateNavigation()
        {
            if (btnFirst != null) btnFirst.Enabled = groups.Count > 0 && currentIndex > 0;
            if (btnPrevious != null) btnPrevious.Enabled = groups.Count > 0 && currentIndex > 0;
            if (btnNext != null) btnNext.Enabled = groups.Count > 0 && currentIndex < groups.Count - 1;
            if (btnLast != null) btnLast.Enabled = groups.Count > 0 && currentIndex < groups.Count - 1;
        }

        private void LoadGroupToForm(GroupData group)
        {
            txtGroupNo.Text = group.GroupNo;
            txtGroupName.Text = group.GroupName;

            if (txtGroupNameEn != null)
                txtGroupNameEn.Text = group.ForeignName ?? "";

            if (chkActive != null)
                chkActive.Checked = group.IsActive;
        }

        private void ClearFields()
        {
            txtGroupNo.Clear();
            txtGroupName.Clear();

            if (txtGroupNameEn != null)
                txtGroupNameEn.Clear();

            if (chkActive != null)
                chkActive.Checked = true;

            currentIndex = -1;
            UpdateNavigation();
            txtGroupNo.Focus();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public List<GroupData> GetGroups() => groups;
        public GroupData GetGroupByNo(string groupNo) => groups.FirstOrDefault(g => g.GroupNo == groupNo);
    }

    public class GroupData
    {
        public string GroupNo { get; set; }
        public string GroupName { get; set; }
        public string ForeignName { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}