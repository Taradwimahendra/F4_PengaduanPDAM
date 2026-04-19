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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace PengaduanPDAM
{
    public partial class FormDashboardAdmin: Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        private Label lblStats;

        public FormDashboardAdmin()
        {
            InitializeComponent();
            this.Load += FormDashboardAdmin_Load;
            button1.Click += BtnSearch_Click;
            button2.Click += BtnEdit_Click;
            button3.Click += BtnDelete_Click;
            button4.Click += BtnLogout_Click;
            dataGridView1.CellClick += DataGridView1_CellClick;

            comboBox1.Items.AddRange(new string[] { "diproses", "selesai", "ditolak" });

            lblStats = new Label();
            lblStats.AutoSize = true;
            lblStats.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblStats.Location = new Point(20, 80);
            this.Controls.Add(lblStats);
        }

        private void FormDashboardAdmin_Load(object sender, EventArgs e)
        {
            LoadData();
            LoadStats();
        }

        private void LoadData(string searchQuery = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"SELECT p.PengaduanID, u.Email, p.NamaLengkap, p.KategoriID, p.Judul_Laporan, p.StatusPengaduan 
                                     FROM LaporanPengaduan p
                                     INNER JOIN UserLogin u ON p.UserID = u.UserID";

                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        query += " WHERE p.Judul_Laporan LIKE @Search";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            cmd.Parameters.AddWithValue("@Search", "%" + searchQuery + "%");
                        }

                        // Menggunakan SqlDataReader sesuai persyaratan (Bagian E)
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dt = new DataTable();
                            dt.Load(reader);
                            dataGridView1.DataSource = dt;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error");
            }
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Memilih data dari DataGridView ke TextBox (Bagian E)
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                if (row.Cells["Judul_Laporan"].Value != null)
                {
                    textBox1.Text = row.Cells["Judul_Laporan"].Value.ToString();
                }
            }
        }

        private void LoadStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM LaporanPengaduan", conn))
                    {
                        int total = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.CommandText = "SELECT COUNT(*) FROM LaporanPengaduan WHERE StatusPengaduan = 'diproses'";
                        int diproses = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.CommandText = "SELECT COUNT(*) FROM LaporanPengaduan WHERE StatusPengaduan = 'selesai'";
                        int selesai = Convert.ToInt32(cmd.ExecuteScalar());

                        lblStats.Text = $"Total Laporan: {total} | Diproses: {diproses} | Selesai: {selesai}";
                    }
                }
            }
            catch { }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadData(textBox1.Text.Trim());
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);
                string newStatus = comboBox1.Text;

                if (string.IsNullOrEmpty(newStatus))
                {
                    MessageBox.Show("Pilih status terlebih dahulu!", "Peringatan");
                    return;
                }

                if (MessageBox.Show("Yakin ingin mengubah status?", "Konfirmasi", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            conn.Open();
                            // Update status in LaporanPengaduan
                            string query = "UPDATE LaporanPengaduan SET StatusPengaduan=@Status WHERE PengaduanID=@ID";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Status", newStatus);
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }

                            // Insert into RiwayatStatus
                            string queryRiwayat = "INSERT INTO RiwayatStatus (LaporanID, StatusPengaduan, Keterangan) VALUES (@LaporanID, @Status, @Keterangan)";
                            using (SqlCommand cmd = new SqlCommand(queryRiwayat, conn))
                            {
                                cmd.Parameters.AddWithValue("@LaporanID", id);
                                cmd.Parameters.AddWithValue("@Status", newStatus);
                                cmd.Parameters.AddWithValue("@Keterangan", "Status diubah menjadi " + newStatus);
                                cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Status berhasil diupdate!", "Sukses");
                            LoadData();
                            LoadStats();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal update status: " + ex.Message, "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data laporan terlebih dahulu!", "Peringatan");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);

                if (MessageBox.Show("Yakin ingin menghapus laporan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            conn.Open();

                            // Delete dependencies first
                            using (SqlCommand cmd = new SqlCommand("DELETE FROM RiwayatStatus WHERE LaporanID=@ID", conn))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }
                            using (SqlCommand cmd = new SqlCommand("DELETE FROM Lampiran WHERE LaporanID=@ID", conn))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }

                            // Delete Laporan
                            using (SqlCommand cmd = new SqlCommand("DELETE FROM LaporanPengaduan WHERE PengaduanID=@ID", conn))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }

                            MessageBox.Show("Data berhasil dihapus!", "Sukses");
                            LoadData();
                            LoadStats();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error");
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data laporan terlebih dahulu!", "Peringatan");
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            SessionManager.ClearSession();
            FormLogin login = new FormLogin();
            login.Show();
            this.Close();
        }

        // Empty event handlers
        private void label1_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
    }
}
