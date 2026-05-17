using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormEditPengaduan : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";
        int pengaduanId;

        public FormEditPengaduan(int id, string judul, string deskripsi, string kategoriID)
        {
            InitializeComponent();
            pengaduanId = id;
            
            cmbKategori.Items.AddRange(new string[] { "Teknis", "Non Teknis" });
            cmbKategori.DropDownStyle = ComboBoxStyle.DropDownList;

            txtJudul.Text = judul;
            txtDeskripsi.Text = deskripsi;
            
            if (kategoriID == "1") cmbKategori.SelectedIndex = 0;
            else if (kategoriID == "2") cmbKategori.SelectedIndex = 1;
            
            btnSimpan.Click += BtnSimpan_Click;
            btnBatal.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
        }

        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            string judul = txtJudul.Text.Trim();
            string kategori = cmbKategori.Text;
            string deskripsi = txtDeskripsi.Text.Trim();

            if (string.IsNullOrEmpty(judul) || string.IsNullOrEmpty(kategori) || string.IsNullOrEmpty(deskripsi))
            {
                MessageBox.Show("Semua kolom wajib diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int katId = kategori.Equals("Teknis", StringComparison.OrdinalIgnoreCase) ? 1 : 2;

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_UpdatePengaduan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Judul", judul);
                        cmd.Parameters.AddWithValue("@KatID", katId);
                        cmd.Parameters.AddWithValue("@Deskripsi", deskripsi);
                        cmd.Parameters.AddWithValue("@ID", pengaduanId);
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Pengaduan berhasil diperbarui!", "Sukses", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal menyimpan data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
