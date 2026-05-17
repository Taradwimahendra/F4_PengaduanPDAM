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
            
            // Setup Categories
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
                    using (SqlCommand cmd = new SqlCommand("SELECT NamaLengkap FROM UserLogin WHERE UserID=@ID", conn))
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
                ofd.Title = "Pilih Gambar Bukti Laporan";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    lblFileName.Text = Path.GetFileName(selectedImagePath);
                    pictureBox1.Image = Image.FromFile(selectedImagePath);
                }
            }
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            string namaLengkap = txtNama.Text.Trim();
            string judul = txtJudul.Text.Trim();
            string kategori = cmbKategori.Text;
            string deskripsi = txtDeskripsi.Text.Trim();

            if (string.IsNullOrEmpty(namaLengkap) || string.IsNullOrEmpty(judul) || string.IsNullOrEmpty(kategori) || string.IsNullOrEmpty(deskripsi))
            {
                MessageBox.Show("Semua kolom teks harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(selectedImagePath))
            {
                MessageBox.Show("Harap lampirkan foto bukti laporan terlebih dahulu!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                int katId = kategori.Equals("Teknis", StringComparison.OrdinalIgnoreCase) ? 1 : 2;

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    int laporanID = 0;

                    using (SqlCommand cmd = new SqlCommand("sp_InsertPengaduan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@UserID", SessionManager.UserID);
                        cmd.Parameters.AddWithValue("@KategoriID", katId);
                        cmd.Parameters.AddWithValue("@Judul_Laporan", judul);
                        cmd.Parameters.AddWithValue("@Deskripsi_Laporan", deskripsi);

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            laporanID = Convert.ToInt32(result);
                        }
                    }
         
                    if (laporanID > 0 && !string.IsNullOrEmpty(selectedImagePath))
                    {
                        try
                        {
                            string queryLampiran = "INSERT INTO Lampiran (PengaduanID, NamaFile, PathFile) VALUES (@PengaduanID, @NamaFile, @PathFile)";
                            using (SqlCommand cmdLampiran = new SqlCommand(queryLampiran, conn))
                            {
                                cmdLampiran.Parameters.AddWithValue("@PengaduanID", laporanID);
                                cmdLampiran.Parameters.AddWithValue("@NamaFile", Path.GetFileName(selectedImagePath));
                                cmdLampiran.Parameters.AddWithValue("@PathFile", selectedImagePath);
                                cmdLampiran.ExecuteNonQuery();
                            }
                        }
                        catch (Exception)
                        {
                            // Ignore missing table gently
                            MessageBox.Show("Pengaduan berhasil disimpan, namun lampiran gagal diunggah.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }


