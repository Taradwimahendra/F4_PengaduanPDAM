using System;

namespace PengaduanPDAM
{
    public static class SessionManager
    {
        public static int UserID { get; set; }
        public static string Email { get; set; }
        public static string Role { get; set; } 
        
        public static void ClearSession()
        {
            UserID = 0;
            Email = null;
            Role = null;
        }
    }
}
