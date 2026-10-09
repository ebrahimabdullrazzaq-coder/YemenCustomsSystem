using System;
using System.Windows.Forms;

namespace YemenCustomsSystem
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // ✅ 1. تهيئة قاعدة البيانات المحلية (سينشئ الملف والجداول تلقائياً إذا لم تكن موجودة)
            DatabaseHelper.InitializeDatabase();

            // ✅ 2. تحميل بيانات المستخدمين من الملف المحلي
            UserManager.LoadUsers();

            // ✅ 3. عرض شاشة تسجيل الدخول
            LoginForm loginForm = new LoginForm();
            DialogResult result = loginForm.ShowDialog();

            if (result == DialogResult.OK && loginForm.Tag is UserManager.User user)
            {
                Application.Run(new MainForm(user));
            }
            else
            {
                Application.Exit();
            }
        }
    }
}