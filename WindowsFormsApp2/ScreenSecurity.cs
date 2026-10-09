using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    // ==================== ✅ فرض الصلاحيات على الشاشات ====================
    public static class ScreenSecurity
    {
        // ✅ الشاشة الحالية المفتوحة (يحددها MainForm)
        public static string CurrentTag = "";

        public static void Apply(Form form, string screenTag)
        {
            if (form == null || string.IsNullOrEmpty(screenTag)) return;
            if (UserManager.IsAdmin()) return;

            ApplyNow(form, screenTag);
            form.Shown += (s, e) => ApplyNow(form, screenTag);
        }

        private static void ApplyNow(Form form, string tag)
        {
            foreach (Control c in form.Controls)
            {
                try { ApplyControl(c, tag); } catch { }
            }
        }

        private static void ApplyControl(Control c, string tag)
        {
            bool canView = UserManager.Can(tag, "View");

            if (c is ToolStrip ts)
            {
                foreach (ToolStripItem item in ts.Items)
                    ApplyItem(item, tag);
            }
            else if (c is DataGridView dgv)
            {
                if (!canView)
                {
                    try { dgv.Rows.Clear(); } catch { }
                }
                if (!UserManager.Can(tag, "Edit")) dgv.ReadOnly = true;
                if (!UserManager.Can(tag, "Add")) dgv.AllowUserToAddRows = false;
                if (!UserManager.Can(tag, "Delete")) dgv.AllowUserToDeleteRows = false;
            }
            else if (c is Button)
            {
                string n = (c.Name ?? "").ToLower();
                bool isNav = n.Contains("next") || n.Contains("previous") ||
                             n.Contains("first") || n.Contains("last") ||
                             n.Contains("move") || n.Contains("nav");

                if (!canView && isNav)
                    c.Enabled = false;
                else
                    ApplyRules(c.Name, c.Text, tag, v => c.Enabled = v);
            }
            else if (!canView)
            {
                if (c is TextBox tb) { try { tb.Text = ""; } catch { } }
                else if (c is ListBox lb) { try { lb.Items.Clear(); } catch { } }
                else if (c is ComboBox cb && cb.DropDownStyle == ComboBoxStyle.DropDown)
                { try { cb.Text = ""; } catch { } }
            }

            foreach (Control child in c.Controls)
            {
                try { ApplyControl(child, tag); } catch { }
            }
        }

        private static void ApplyItem(ToolStripItem item, string tag)
        {
            ApplyRules(item.Name, item.Text, tag, v => item.Enabled = v);

            if (item is ToolStripDropDownItem dd)
                foreach (ToolStripItem child in dd.DropDownItems)
                    ApplyItem(child, tag);
        }

        private static void ApplyRules(string name, string text, string tag, Action<bool> set)
        {
            string n = (name ?? "").ToLower();
            string t = text ?? "";

            if (n.Contains("delete") || t.Contains("حذف"))
                set(UserManager.Can(tag, "Delete"));
            else if (n.Contains("edit") || t.Contains("تعديل"))
                set(UserManager.Can(tag, "Edit"));
            else if (n.Contains("new") || n.Contains("add") || t.Contains("جديد"))
                set(UserManager.Can(tag, "Add"));
            else if (n.Contains("save") || t.Contains("حفظ"))
                set(UserManager.Can(tag, "Add") || UserManager.Can(tag, "Edit"));
            else if (n.Contains("print") || t.Contains("طباعة"))
                set(UserManager.Can(tag, "Print"));
            else if (n.Contains("report") || t.Contains("تقرير"))
                set(UserManager.Can(tag, "ViewReport"));
            else if (n.Contains("search") || n.Contains("export") || t.Contains("بحث") || t.Contains("تصدير") || t.Contains("القائمة"))
                set(UserManager.Can(tag, "ViewList"));
            else if (t.Contains("عرض"))
                set(UserManager.Can(tag, "View"));
        }
    }

    // ==================== ✅ فلتر عام: يطبق الصلاحيات على أي نافذة تُفتح (بما فيها المنبثقة) ====================
    public class SecurityMessageFilter : IMessageFilter
    {
        private readonly HashSet<Form> _applied = new HashSet<Form>();

        public bool PreFilterMessage(ref Message m)
        {
            const int WM_SHOWWINDOW = 0x0018;
            if (m.Msg == WM_SHOWWINDOW)
            {
                Control c = Control.FromHandle(m.HWnd);
                Form f = c as Form;
                if (f != null && !_applied.Contains(f))
                {
                    _applied.Add(f);
                    f.FormClosed += (s, e) => _applied.Remove(f);

                    if (!string.IsNullOrEmpty(ScreenSecurity.CurrentTag))
                        ScreenSecurity.Apply(f, ScreenSecurity.CurrentTag);
                }
            }
            return false;
        }
    }
}