using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormDashboardUser : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        private Label lblTotalLaporan;

        public FormDashboardUser()
        {
            InitializeComponent();

            // Perbaikan: Form secara tidak sengaja ter-disable di file Designer.
            this.Enabled = true;

            this.Load += FormDashboardUser_Load;

            // Sembunyikan panel lama karena diganti menjadi Button
            if (panel2 != null) panel2.Visible = false;
            if (panel3 != null) panel3.Visible = false;

            // Membuat Button Buat Laporan
            Button btnBuatLaporan = new Button();
            btnBuatLaporan.Text = "Buat Laporan Baru";
            btnBuatLaporan.Size = new Size(200, 60);
            btnBuatLaporan.Location = new Point(450, 220); // Sesuaikan dengan posisi panel2 sebelumnya
            btnBuatLaporan.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBuatLaporan.BackColor = Color.LightBlue;
            btnBuatLaporan.Click += PanelLaporan_Click;
            this.Controls.Add(btnBuatLaporan);

            // Membuat Button Riwayat Laporan
            Button btnLihatRiwayat = new Button();
            btnLihatRiwayat.Text = "Lihat Riwayat Pengaduan";
            btnLihatRiwayat.Size = new Size(200, 60);
            btnLihatRiwayat.Location = new Point(700, 220); // Sesuaikan dengan posisi panel3 sebelumnya
            btnLihatRiwayat.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLihatRiwayat.BackColor = Color.LightGreen;
            btnLihatRiwayat.Click += PanelRiwayat_Click;
            this.Controls.Add(btnLihatRiwayat);

            // Logout
            button1.Click += BtnLogout_Click;

            // Create stats label
            lblTotalLaporan = new Label();
            lblTotalLaporan.AutoSize = true;
            lblTotalLaporan.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblTotalLaporan.Location = new Point(20, 80);
            this.Controls.Add(lblTotalLaporan);
        }
        private void FormDashboardUser_Load(object sender, EventArgs e)
        {
            LoadTotalLaporan();
        }

        private void LoadTotalLaporan()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM LaporanPengaduan WHERE UserID = @UserID";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", SessionManager.UserID);
                        int total = Convert.ToInt32(cmd.ExecuteScalar());
                        lblTotalLaporan.Text = $"Total Laporan Anda: {total}";
                    }
                }
            }
            catch { }
        }
        private void PanelLaporan_Click(object sender, EventArgs e)
        {
            FormPengaduan formPengaduan = new FormPengaduan();
            formPengaduan.ShowDialog();
            LoadTotalLaporan(); // Refresh total when returning
        }

        private void PanelRiwayat_Click(object sender, EventArgs e)
        {
            FormRiwayat formRiwayat = new FormRiwayat();
            formRiwayat.ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            SessionManager.ClearSession();
            FormLogin login = new FormLogin();
            login.Show();
            this.Close();
        }

        private void label2_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
    }
}


