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