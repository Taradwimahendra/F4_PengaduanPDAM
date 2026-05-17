using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormRiwayat : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";
        private Label lblDetail;
        private Label lblTotal;
        private BindingSource bindingSource = new BindingSource();

        public FormRiwayat()
        {
            InitializeComponent();
            button1.Click += BtnCari_Click;
            dataGridView1.CellClick += DataGridView1_CellClick;
            btnBatal.Click += btnBatal_Click;
            btnHapus.Click += btnHapus_Click;
            btnEdit.Click += btnEdit_Click;
            
            this.Load += FormRiwayat_Load;

            lblDetail = new Label();
            lblDetail.AutoSize = true;
            lblDetail.Location = new Point(30, dataGridView1.Bottom + 20);
            lblDetail.Font = new Font("Segoe UI", 10F);
            lblDetail.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.Controls.Add(lblDetail);

            lblTotal = new Label();
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(dataGridView1.Right - 200, dataGridView1.Bottom + 20);
            lblTotal.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblTotal.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTotal.ForeColor = Color.DarkBlue;
            this.Controls.Add(lblTotal);

            StyleDataGridView();
        }

        private void StyleDataGridView()
        {
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(238, 239, 249);
            dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.DarkTurquoise;
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.WhiteSmoke;
            dataGridView1.BackgroundColor = Color.White;
            
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(20, 25, 72);
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
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
                    string query = @"SELECT p.PengaduanID, p.Judul_Laporan, p.KategoriID, p.StatusPengaduan, p.Deskripsi_Laporan
                                     FROM Pengaduan p
                                     WHERE p.UserID = " + SessionManager.UserID;

                    if (!string.IsNullOrEmpty(searchQuery))
                    {
                        // Simulasi SQL Injection
                        query += " AND p.Judul_Laporan LIKE '%" + searchQuery + "%'";
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            bindingSource.DataSource = dt;
                            dataGridView1.DataSource = bindingSource;
                            bindingNavigator1.BindingSource = bindingSource;
                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        }

                        if (dataGridView1.Columns["Deskripsi_Laporan"] != null)
                            dataGridView1.Columns["Deskripsi_Laporan"].Visible = false;
                    }

                    HitungTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat riwayat: " + ex.Message, "Error");
            }
        }

        private void HitungTotal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_CountPengaduanUser", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserID", SessionManager.UserID);

        private void BtnCari_Click(object sender, EventArgs e)
        {
            LoadData(textBox1.Text.Trim());
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string judul = row.Cells["Judul_Laporan"].Value?.ToString();
                string deskripsi = row.Cells["Deskripsi_Laporan"].Value?.ToString();
                string status = row.Cells["StatusPengaduan"].Value?.ToString();

                lblDetail.Text = $"Judul: {judul}\nDeskripsi: {deskripsi}\nStatus: {status}";
                lblDetail.Top = dataGridView1.Bottom + 10; 
                
                textBox1.Text = judul;
            }
        }

        private void btnBatal_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);
                string currentStatus = dataGridView1.Rows[rowIndex].Cells["StatusPengaduan"].Value?.ToString();

                if (currentStatus == "selesai" || currentStatus == "ditolak")
                {
                    MessageBox.Show("Laporan yang sudah selesai atau ditolak tidak dapat dibatalkan.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (MessageBox.Show("Yakin ingin membatalkan pengaduan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            conn.Open();
                            
                            // Update Status
                            string query = "UPDATE Pengaduan SET StatusPengaduan = 'dibatalkan' WHERE PengaduanID = @ID";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID", id);
                                cmd.ExecuteNonQuery();
                            }
                            
                            MessageBox.Show("Pengaduan berhasil dibatalkan.", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            LoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal membatalkan laporan: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data laporan terlebih dahulu!", "Peringatan");
            }
        }

        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);

                if (MessageBox.Show("Yakin ingin menghapus secara permanen riwayat pengaduan ini?", "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    try
                    {
                        using (SqlConnection conn = new SqlConnection(connString))
                        {
                            conn.Open();

                            using (SqlCommand cmd = new SqlCommand("sp_DeletePengaduan", conn))
                            {
                                cmd.CommandType = CommandType.StoredProcedure;
                                cmd.Parameters.Add("@PengaduanID", SqlDbType.Int).Value = id;
                                int rowsAffected = cmd.ExecuteNonQuery();
                                
                                if (rowsAffected > 0)
                                    MessageBox.Show("Riwayat berhasil dihapus secara permanen!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                else
                                    MessageBox.Show("Data tidak ditemukan!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }

                            LoadData();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal menghapus data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data laporan terlebih dahulu!", "Peringatan");
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                int id = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);
                string currentStatus = dataGridView1.Rows[rowIndex].Cells["StatusPengaduan"].Value?.ToString();

                if (currentStatus != "menunggu" && currentStatus != "diproses")
                {
                    MessageBox.Show("Hanya laporan dengan status 'menunggu' atau 'diproses' yang bisa diedit.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string judul = dataGridView1.Rows[rowIndex].Cells["Judul_Laporan"].Value?.ToString();
                string deskripsi = dataGridView1.Rows[rowIndex].Cells["Deskripsi_Laporan"].Value?.ToString();
                string kategoriID = dataGridView1.Rows[rowIndex].Cells["KategoriID"].Value?.ToString();

                FormEditPengaduan formEdit = new FormEditPengaduan(id, judul, deskripsi, kategoriID);
                if (formEdit.ShowDialog() == DialogResult.OK)
                {
                    LoadData();
                }
            }
            else
            {
                MessageBox.Show("Pilih data laporan terlebih dahulu!", "Peringatan");
            }
        }

        private void btnHapusInjection_Click(object sender, EventArgs e)
        {
            string judul = textBox1.Text;

            if (string.IsNullOrEmpty(judul))
            {
                MessageBox.Show("Masukkan judul terlebih dahulu!", "Peringatan");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Simulasi SQL Injection (UPDATE Semua Kolom) - Sangat Tidak Aman
                    string query = "UPDATE Pengaduan SET Judul_Laporan = 'HACKED', Deskripsi_Laporan = 'HACKED', StatusPengaduan = 'HACKED' WHERE Judul_Laporan = '" + judul + "'";
                    
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        int rowsAffected = cmd.ExecuteNonQuery();
                        MessageBox.Show($"Simulasi eksekusi Injeksi selesai. {rowsAffected} baris terubah menjadi 'HACKED'!", "SQL Injection (Simulasi)", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error simulasi: " + ex.Message, "Error");
            }
        }
    }
}
