using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    internal static class Program
    {
        // Variabel global sebagai pengganti SessionManager
        public static int UserID;
        public static string Email;
        public static string Role;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormLogin());
        }
    }
}
