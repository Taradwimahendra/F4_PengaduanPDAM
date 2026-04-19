using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormLogin : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        public FormLogin()
        {
            InitializeComponent();

            BtnLogin.Click += BtnLogin_Click;      // LOGIN
            button2.Click += BtnCekKoneksi_Click; // CEK KONEKSI
            textBox2.PasswordChar = '*';
        }

        // ================= LOGIN =================
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string email = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email dan Password tidak boleh kosong!", "Peringatan");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // ===== ADMIN =====
                    string queryAdmin = @"
                        SELECT AdminID 
                        FROM AdminLogin 
                        WHERE LOWER(Email)=LOWER(@Email) 
                        AND Password=@Password";

                    using (SqlCommand cmd = new SqlCommand(queryAdmin, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            SessionManager.UserID = Convert.ToInt32(result);
                            SessionManager.Email = email;
                            SessionManager.Role = "Admin";

                            new FormDashboardAdmin().Show();
                            this.Hide();
                            return;
                        }
                    }

                    // ===== USER =====
                    string queryUser = @"
                        SELECT UserID 
                        FROM UserLogin 
                        WHERE LOWER(Email)=LOWER(@Email) 
                        AND Password=@Password";

                    using (SqlCommand cmd = new SqlCommand(queryUser, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        object result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            SessionManager.UserID = Convert.ToInt32(result);
                            SessionManager.Email = email;
                            SessionManager.Role = "User";

                            new FormDashboardUser().Show();
                            this.Hide();
                            return;
                        }
                    }

                    MessageBox.Show("Email atau Password salah!", "Error");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan: " + ex.Message);
            }
        }

        // ================= CEK KONEKSI =================
        private void BtnCekKoneksi_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    MessageBox.Show("Koneksi Database Berhasil!", "Status");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Koneksi Database Gagal: " + ex.Message, "Error");
            }
        }

        // kosong biar aman
        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    }
}
