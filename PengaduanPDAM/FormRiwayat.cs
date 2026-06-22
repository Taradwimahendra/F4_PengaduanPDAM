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
        private Label lblTotal;
        private BindingSource bindingSource = new BindingSource();

        public FormRiwayat()
        {
            InitializeComponent();
            button1.Click += BtnCari_Click;
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.RowPostPaint += DataGridView1_RowPostPaint;
            btnBatal.Click += btnBatal_Click;
            btnHapus.Click += btnHapus_Click;
            btnEdit.Click += btnEdit_Click;

            this.Load += FormRiwayat_Load;

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

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();
            
            var centerFormat = new StringFormat() 
            { 
                Alignment = StringAlignment.Center, 
                LineAlignment = StringAlignment.Center
            };
            
            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void FormRiwayat_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        // =====================================================================
        // LoadData - Memanfaatkan SP sp_GetPengaduanByUserID
        // =====================================================================
        private void LoadData(string searchQuery = "")
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Memanggil Stored Procedure sp_GetPengaduanByUserID
                    SqlCommand cmd = new SqlCommand("sp_GetPengaduanByUserID", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", SessionManager.UserID);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        // Filter client-side jika ada kata kunci pencarian
                        if (!string.IsNullOrEmpty(searchQuery))
                        {
                            DataView dv = dt.DefaultView;
                            dv.RowFilter = "Judul_Laporan LIKE '%" + searchQuery + "%'";
                            dt = dv.ToTable();
                        }

                        bindingSource.DataSource = dt;
                        dataGridView1.DataSource = bindingSource;
                        bindingNavigator1.BindingSource = bindingSource;
                        dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }

                    if (dataGridView1.Columns.Contains("PengaduanID"))
                    {
                        dataGridView1.Columns["PengaduanID"].Visible = false;
                    }

                    if (dataGridView1.Columns["Deskripsi_Laporan"] != null)
                        dataGridView1.Columns["Deskripsi_Laporan"].Visible = false;

                    HitungTotal();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat riwayat: " + ex.Message, "Error");
            }
        }

        // =====================================================================
        // HitungTotal - Parameterized query (count per UserID)
        // =====================================================================
        private void HitungTotal()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    // Parameterized query untuk hitung total pengaduan user ini
                    SqlCommand cmd = new SqlCommand(
                        "SELECT COUNT(*) FROM Pengaduan WHERE UserID = @UserID", conn);
                    cmd.Parameters.AddWithValue("@UserID", SessionManager.UserID);

                    int total = Convert.ToInt32(cmd.ExecuteScalar());
                    lblTotal.Text = "Total Pengaduan: " + total;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menghitung total: " + ex.Message, "Error");
            }
        }

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
                textBox1.Text = judul;
            }
        }

        // =====================================================================
        // btnBatal - SP sp_UpdateStatusPengaduan + SqlTransaction + SimpanLog
        // =====================================================================
        private void btnBatal_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex    = dataGridView1.SelectedCells[0].RowIndex;
                int id          = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);
                string currentStatus = dataGridView1.Rows[rowIndex].Cells["StatusPengaduan"].Value?.ToString();

                if (currentStatus == "selesai" || currentStatus == "ditolak")
                {
                    MessageBox.Show("Laporan yang sudah selesai atau ditolak tidak dapat dibatalkan.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (MessageBox.Show("Yakin ingin membatalkan pengaduan ini?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SqlConnection conn = new SqlConnection(connString);
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    try
                    {
                        // Memanggil SP sp_UpdateStatusPengaduan
                        SqlCommand cmd = new SqlCommand("sp_UpdateStatusPengaduan", conn, trans);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@PengaduanID", id);
                        cmd.Parameters.AddWithValue("@StatusBaru", "dibatalkan");
                        cmd.ExecuteNonQuery();

                        // Log aktivitas ke LogAktivitas
                        SqlCommand cmdLog = new SqlCommand(
                            "INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@aktivitas,GETDATE())",
                            conn, trans);
                        cmdLog.Parameters.AddWithValue("@aktivitas", "BATAL PENGADUAN ID: " + id);
                        cmdLog.ExecuteNonQuery();

                        trans.Commit();

                        MessageBox.Show("Pengaduan berhasil dibatalkan.", "Sukses",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadData();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        SimpanLog("ROLLBACK BATAL PENGADUAN : " + ex.Message);
                        MessageBox.Show(ex.Message, "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SimpanLog("GENERAL ERROR BATAL : " + ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data laporan terlebih dahulu!", "Peringatan");
            }
        }

        // =====================================================================
        // btnHapus - SP sp_DeletePengaduan + SqlTransaction + SimpanLog
        // =====================================================================
        private void btnHapus_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                int id       = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);

                if (MessageBox.Show("Yakin ingin menghapus secara permanen riwayat pengaduan ini?",
                    "Konfirmasi", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    SqlConnection conn = new SqlConnection(connString);
                    conn.Open();
                    SqlTransaction trans = conn.BeginTransaction();

                    try
                    {
                        // Memanggil SP sp_DeletePengaduan
                        SqlCommand cmd = new SqlCommand("sp_DeletePengaduan", conn, trans);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("@PengaduanID", SqlDbType.Int).Value = id;
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Log aktivitas ke LogAktivitas
                        SqlCommand cmdLog = new SqlCommand(
                            "INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@aktivitas,GETDATE())",
                            conn, trans);
                        cmdLog.Parameters.AddWithValue("@aktivitas", "HAPUS PENGADUAN ID: " + id);
                        cmdLog.ExecuteNonQuery();

                        trans.Commit();

                        if (rowsAffected > 0)
                            MessageBox.Show("Riwayat berhasil dihapus secara permanen!", "Sukses",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                        else
                            MessageBox.Show("Data tidak ditemukan!", "Peringatan",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        LoadData();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        SimpanLog("ROLLBACK HAPUS PENGADUAN : " + ex.Message);
                        MessageBox.Show(ex.Message, "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SimpanLog("GENERAL ERROR HAPUS : " + ex.Message);
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
            }
            else
            {
                MessageBox.Show("Pilih data laporan terlebih dahulu!", "Peringatan");
            }
        }

        // =====================================================================
        // btnEdit - Lookup KategoriID terpisah karena SP tidak mengembalikannya
        // =====================================================================
        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex    = dataGridView1.SelectedCells[0].RowIndex;
                int id          = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);
                string currentStatus = dataGridView1.Rows[rowIndex].Cells["StatusPengaduan"].Value?.ToString();

                if (currentStatus != "menunggu" && currentStatus != "diproses")
                {
                    MessageBox.Show("Hanya laporan dengan status 'menunggu' atau 'diproses' yang bisa diedit.",
                        "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string judul     = dataGridView1.Rows[rowIndex].Cells["Judul_Laporan"].Value?.ToString();
                string deskripsi = dataGridView1.Rows[rowIndex].Cells["Deskripsi_Laporan"].Value?.ToString();

                // Lookup KategoriID menggunakan parameterized query
                // (SP sp_GetPengaduanByUserID tidak mengembalikan kolom KategoriID)
                string kategoriID = "1";
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        SqlCommand cmdKat = new SqlCommand(
                            "SELECT KategoriID FROM Pengaduan WHERE PengaduanID = @ID", conn);
                        cmdKat.Parameters.AddWithValue("@ID", id);
                        object result = cmdKat.ExecuteScalar();
                        if (result != null) kategoriID = result.ToString();
                    }
                }
                catch { }

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

        // =====================================================================
        // SimpanLog - Helper log ke LogAktivitas (koneksi terpisah)
        // =====================================================================
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
    }
}
