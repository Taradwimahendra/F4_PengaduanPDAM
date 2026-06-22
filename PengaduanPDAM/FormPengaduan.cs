using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormPengaduan : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";
        private string selectedImagePath = "";

        public FormPengaduan()  
        {
            InitializeComponent();
            btnSubmit.Click += BtnSubmit_Click;
            btnUpload.Click += BtnUpload_Click;

            cmbKategori.Items.AddRange(new string[] { "Teknis", "Non Teknis" });
            cmbKategori.DropDownStyle = ComboBoxStyle.DropDownList;

            this.Load += FormPengaduan_Load;
        }

        private void FormPengaduan_Load(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    // Parameterized query untuk ambil nama user yang sedang login
                    using (SqlCommand cmd = new SqlCommand(
                        "SELECT NamaLengkap FROM UserLogin WHERE UserID=@ID", conn))
                    {
                        cmd.Parameters.AddWithValue("@ID", SessionManager.UserID);
                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            txtNama.Text = result.ToString();
                            txtNama.ReadOnly = true;
                            txtNama.BackColor = SystemColors.Control;
                        }
                    }
                }
            }
            catch { }
        }

        private void BtnUpload_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                ofd.Title  = "Pilih Gambar Bukti Laporan";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    lblFileName.Text  = Path.GetFileName(selectedImagePath);
                    pictureBox1.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        // =====================================================================
        // BtnSubmit - SP sp_InsertPengaduan + SqlTransaction + SimpanLog
        // =====================================================================
        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string namaLengkap = txtNama.Text.Trim();
            string judul       = txtJudul.Text.Trim();
            string kategori    = cmbKategori.Text;
            string deskripsi   = txtDeskripsi.Text.Trim();

            if (string.IsNullOrEmpty(namaLengkap) || string.IsNullOrEmpty(judul) ||
                string.IsNullOrEmpty(kategori)    || string.IsNullOrEmpty(deskripsi))
            {
                MessageBox.Show("Semua kolom teks harus diisi!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("Harap lampirkan foto bukti laporan terlebih dahulu!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int katId = kategori.Equals("Teknis", StringComparison.OrdinalIgnoreCase) ? 1 : 2;

            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                // Memanggil SP sp_InsertPengaduan
                SqlCommand cmd = new SqlCommand("sp_InsertPengaduan", conn, trans);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID",           SessionManager.UserID);
                cmd.Parameters.AddWithValue("@KategoriID",       katId);
                cmd.Parameters.AddWithValue("@Judul_Laporan",    judul);
                cmd.Parameters.AddWithValue("@Deskripsi_Laporan", deskripsi);
                cmd.ExecuteNonQuery();

                // Ambil PengaduanID yang baru diinsert dalam transaksi ini
                // CATATAN: @@IDENTITY tidak bisa dipakai karena trigger trg_InsertPengaduan
                //          menyebabkan @@IDENTITY berubah ke ID LogAktivitas, bukan PengaduanID.
                //          Solusi: SELECT MAX(PengaduanID) dalam transaksi yang sama.
                SqlCommand cmdGetId = new SqlCommand(
                    "SELECT MAX(PengaduanID) FROM Pengaduan WHERE UserID = @UserID",
                    conn, trans);
                cmdGetId.Parameters.AddWithValue("@UserID", SessionManager.UserID);
                int laporanID = Convert.ToInt32(cmdGetId.ExecuteScalar());

                // Insert lampiran dalam transaksi yang sama
                if (laporanID > 0 && !string.IsNullOrEmpty(selectedImagePath))
                {
                    SqlCommand cmdLampiran = new SqlCommand(
                        "INSERT INTO Lampiran (PengaduanID, NamaFile, PathFile) " +
                        "VALUES (@PengaduanID, @NamaFile, @PathFile)",
                        conn, trans);
                    cmdLampiran.Parameters.AddWithValue("@PengaduanID", laporanID);
                    cmdLampiran.Parameters.AddWithValue("@NamaFile",    Path.GetFileName(selectedImagePath));
                    cmdLampiran.Parameters.AddWithValue("@PathFile",    selectedImagePath);
                    cmdLampiran.ExecuteNonQuery();
                }

                // Log aktivitas ke LogAktivitas
                SqlCommand cmdLog = new SqlCommand(
                    "INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@aktivitas,GETDATE())",
                    conn, trans);
                cmdLog.Parameters.AddWithValue("@aktivitas",
                    "INSERT PENGADUAN oleh UserID: " + SessionManager.UserID + " - " + judul);
                cmdLog.ExecuteNonQuery();

                trans.Commit();

                MessageBox.Show("Pengaduan berhasil dikirim!", "Sukses",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form
                txtNama.Clear();
                txtJudul.Clear();
                cmbKategori.SelectedIndex = -1;
                txtDeskripsi.Clear();
                pictureBox1.Image = null;
                lblFileName.Text  = "Tidak ada file dipilih";
                selectedImagePath = "";
            }
            catch (SqlException ex)
            {
                trans.Rollback();
                SimpanLog("ROLLBACK INSERT PENGADUAN : " + ex.Message);
                MessageBox.Show(ex.Message, "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                trans.Rollback();
                SimpanLog("GENERAL ERROR INSERT : " + ex.Message);
                MessageBox.Show(ex.Message);
            }
            finally
            {
                conn.Close();
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
