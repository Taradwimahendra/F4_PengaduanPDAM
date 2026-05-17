using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormRegister : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        public FormRegister()
        {
            InitializeComponent();
            btnRegister.Click += BtnRegister_Click;
            btnLogin.Click += BtnLogin_Click;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string nama = txtNama.Text.Trim();
            string email = txtEmail.Text.Trim();
            string password = txtPassword.Text.Trim();
            string telepon = txtTelepon.Text.Trim();
            string alamat = txtAlamat.Text.Trim();

            if (string.IsNullOrEmpty(nama) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(telepon))
            {
                MessageBox.Show("Semua kolom (kecuali alamat jika opsional) wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (telepon.Length < 10 || telepon.Length > 13)
            {
                MessageBox.Show("No Telepon harus memiliki panjang 10 hingga 13 digit!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Cek apakah email sudah terdaftar
                    string checkQuery = "SELECT COUNT(*) FROM UserLogin WHERE Email = @Email";
                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        int existing = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (existing > 0)
                        {
                            MessageBox.Show("Email sudah digunakan. Silakan login atau gunakan email lain.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                    }

                    // Tambahkan user baru
                    string insertQuery = @"
                        INSERT INTO UserLogin (NamaLengkap, Email, Password, NoTelepon, Alamat, RoleUser) 
                        VALUES (@NamaLengkap, @Email, @Password, @NoTelepon, @Alamat, 'pelanggan')";

                    using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@NamaLengkap", nama);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@NoTelepon", telepon);
                        cmd.Parameters.AddWithValue("@Alamat", string.IsNullOrEmpty(alamat) ? DBNull.Value : (object)alamat);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Registrasi berhasil! Silakan login.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    
                    // Kembali ke halaman Login
                    FormLogin login = new FormLogin();
                    login.Show();
                    this.Hide();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Terjadi kesalahan saat registrasi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            FormLogin login = new FormLogin();
            login.Show();
            this.Hide();
        }
    }
}
