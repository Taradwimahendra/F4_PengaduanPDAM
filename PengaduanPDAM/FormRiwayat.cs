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
    public partial class FormRiwayat : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        private Label lblDetail;

        public FormRiwayat()
        {
            InitializeComponent();
            this.Load += FormRiwayat_Load;
            button1.Click += BtnCari_Click;
            button2.Click += BtnKembali_Click;
            dataGridView1.CellClick += DataGridView1_CellClick;

            // Optional buttons: Edit and Hapus for user if applicable
            button3.Click += BtnEdit_Click;
            button4.Click += BtnHapus_Click;

            // Detail Label
            lblDetail = new Label();
            lblDetail.AutoSize = true;
            lblDetail.Location = new Point(20, dataGridView1.Bottom + 20);
            lblDetail.Font = new Font("Segoe UI", 10F);
            this.Controls.Add(lblDetail);
        }

        private void FormRiwayat_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData(string searchQuery = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT p.PengaduanID, p.Judul_Laporan, p.KategoriID, p.StatusPengaduan, p.Deskripsi_Laporan, r.Keterangan
                                     FROM LaporanPengaduan p
                                     LEFT JOIN RiwayatStatus r ON p.PengaduanID = r.LaporanID
                                     WHERE p.UserID = @UserID";

                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query += " AND p.Judul_Laporan LIKE @Search";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", SessionManager.UserID);
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + searchQuery + "%");
                        }

                        // Menggunakan SqlDataReader untuk menampilkan data
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataGridView1.DataSource = dt;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }

                        // Hide description and keterangan column if they take too much space
                        if (dataGridView1.Columns["Deskripsi_Laporan"] != null)
                            dataGridView1.Columns["Deskripsi_Laporan"].Visible = false;
                        if (dataGridView1.Columns["Keterangan"] != null)
                            dataGridView1.Columns["Keterangan"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat riwayat: " + ex.Message, "Error");
            }
        }

        private void BtnCari_Click(object sender, EventArgs e)
        {
            LoadData(textBox1.Text.Trim());
        }

        private void BtnKembali_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string judul = row.Cells["Judul_Laporan"].Value?.ToString();
                string deskripsi = row.Cells["Deskripsi_Laporan"].Value?.ToString();
                string status = row.Cells["StatusPengaduan"].Value?.ToString();
                string keterangan = row.Cells["Keterangan"].Value?.ToString();

                lblDetail.Text = $"Judul: {judul}\nDeskripsi: {deskripsi}\nStatus: {status}\nKeterangan Admin: {keterangan}";
                lblDetail.Top = dataGridView1.Bottom + 10; // Ensure position is correct if resized

                // Pilih data dari DataGridView ke TextBox
                textBox1.Text = judul;
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // Edit is only for admin based on specs, but UI has the button. Show message.
            MessageBox.Show("Fitur Edit hanya dapat diakses melalui Dashboard Admin untuk mengupdate status.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnHapus_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Fitur Hapus hanya dapat diakses melalui Dashboard Admin.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
