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
            string email    = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Email dan Password tidak boleh kosong!", "Peringatan");
                return;
            }

            SqlConnection conn = new SqlConnection(connString);
            conn.Open();

            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                // Panggil Stored Procedure sp_LoginUser
                SqlCommand cmd = new SqlCommand("sp_LoginUser", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Email",    email);
                cmd.Parameters.AddWithValue("@Password", password);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    int    userId = reader.GetInt32(0);
                    string nama   = reader.GetString(1);
                    string role   = reader.IsDBNull(2) ? "pelanggan"
                                                       : reader.GetString(2).ToLower();
                    reader.Close();

                    SessionManager.UserID = userId;
                    SessionManager.Email  = email;

                    // Log aktivitas login berhasil ke LogAktivitas
                    SqlCommand cmdLog = new SqlCommand(
                        "INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@aktivitas,GETDATE())",
                        conn, trans);
                    cmdLog.Parameters.AddWithValue("@aktivitas", "LOGIN BERHASIL : " + email);
                    cmdLog.ExecuteNonQuery();

                    trans.Commit();

                    MessageBox.Show("Login berhasil! Selamat datang, " + nama);

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
                }
                else
                {
                    reader.Close();
                    trans.Rollback();
                    SimpanLog("LOGIN GAGAL : " + email);
                    MessageBox.Show("Email atau Password salah!", "Login Gagal",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (SqlException ex)
            {
                trans.Rollback();
                SimpanLog("ROLLBACK LOGIN : " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                SimpanLog("GENERAL ERROR : " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        // Helper: simpan log error ke tabel LogAktivitas (koneksi terpisah)
        private void SimpanLog(string aktivitas)
        {
            try
            {
                using (SqlConnection connLog = new SqlConnection(connString))
                {
                    connLog.Open();
                    SqlCommand cmdLog = new SqlCommand(
                        "INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@aktivitas,GETDATE())",
                        connLog);
                    cmdLog.Parameters.AddWithValue("@aktivitas", aktivitas);
                    cmdLog.ExecuteNonQuery();
                }
            }
            catch { /* abaikan error logging */ }
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


 
        private void btnTestInjection_Click(object sender, EventArgs e)
        {
            // Payload SQL Injection - hardcoded agar langsung jalan tanpa perlu isi textBox
            string payloadEmail    = "' OR '1'='1'--";
            string payloadPassword = "";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string queryRentan =
                        "SELECT COUNT(*) FROM UserLogin " +
                        "WHERE Email = '" + payloadEmail + "' " +
                        "AND Password = '" + payloadPassword + "'";

                    using (SqlCommand cmdInject = new SqlCommand(queryRentan, conn))
                    {
                        int hasil = Convert.ToInt32(cmdInject.ExecuteScalar());

                        if (hasil > 0)
                        {
                            MessageBox.Show(
                                "⚠️ LOGIN BERHASIL tanpa password!\n\n",
                                "SQL Injection Berhasil",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                            FormDashboardUser formData = new FormDashboardUser();
                            formData.Show();
                            this.Hide();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Injection tidak berhasil. Tabel kosong atau query tidak cocok.",
                                "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

       

        private void label1_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
    }
}




 
