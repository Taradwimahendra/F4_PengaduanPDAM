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

            textBox2.PasswordChar = '*';
            button1.Click += BtnRegister_Click;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            FormRegister formReg = new FormRegister();
            formReg.Show();
            this.Hide();
        }


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

                    string query = @"
                        SELECT UserID, RoleUser 
                        FROM UserLogin 
                        WHERE LOWER(Email)=LOWER(@Email) 
                        AND Password=@Password";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = reader.GetInt32(0);
                                string role = reader.IsDBNull(1) ? "pelanggan" : reader.GetString(1).ToLower();

                                SessionManager.UserID = userId;
                                SessionManager.Email = email;

                                if (role == "admin")
                                {
                                    SessionManager.Role = "Admin";
                                    new FormDashboardAdmin().Show();
                                }
                                else
                                {
                                    SessionManager.Role = "User";
                                    new FormDashboardUser().Show();
                                }

                                this.Hide();
                                return;
                            }
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


        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }
    }
}
