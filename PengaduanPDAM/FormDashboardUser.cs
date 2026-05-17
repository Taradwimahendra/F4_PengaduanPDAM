using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormDashboardUser : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        public FormDashboardUser()
        {
            InitializeComponent();
        }

        private void FormDashboardUser_Load(object sender, EventArgs e)
        {
            LoadUserName();
        }

        private void LoadUserName()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Assume table UserLogin and column Nama
                    // If NamaLengkap doesn't exist, we fall back to Email.
                    string query = "SELECT NamaLengkap FROM UserLogin WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", SessionManager.UserID);
                        object result = null;
                        try 
                        {
                            result = cmd.ExecuteScalar();
                        }
                        catch 
                        {
                            // In case 'Nama' column doesn't exist, ignore the query error
                        }

                        if (result != null && result != DBNull.Value)
                        {
                            lblWelcome.Text = "Selamat datang, " + result.ToString();
                        }
                        else
                        {
                            lblWelcome.Text = "Selamat datang, " + SessionManager.Email;
                        }
                    }
                }
            }
            catch (Exception)
            {
                lblWelcome.Text = "Selamat datang, " + SessionManager.Email;
            }
        }

        private void LoadFormIntoPanel(Form childForm)
        {
            if (this.panelMain.Controls.Count > 0)
                this.panelMain.Controls[0].Dispose(); // Dispose previous form to prevent memory leaks

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            this.panelMain.Controls.Clear();
            this.panelMain.Controls.Add(childForm);
            childForm.Show();
        }

        private void btnInputPengaduan_Click(object sender, EventArgs e)
        {
            LoadFormIntoPanel(new FormPengaduan());
        }

        private void btnRiwayatPengaduan_Click(object sender, EventArgs e)
        {
            LoadFormIntoPanel(new FormRiwayat());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                SessionManager.ClearSession();
                FormLogin formLogin = new FormLogin();
                formLogin.Show();
                this.Hide();
            }
        }
    }
}
