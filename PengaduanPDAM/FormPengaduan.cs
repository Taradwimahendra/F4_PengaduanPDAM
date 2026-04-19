using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormPengaduan : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        private Button btnBrowse;
        private PictureBox pictureBox1;
        private string selectedImagePath = "";
        public FormPengaduan()
        {
            InitializeComponent();
            this.Load += FormPengaduan_Load;
            button1.Click += BtnSimpan_Click;
            button2.Click += BtnKembali_Click;


            textBox1.Text = SessionManager.Email;
            btnBrowse = new Button();
            btnBrowse.Text = "Pilih Lampiran Gambar";
            btnBrowse.AutoSize = true;
            btnBrowse.Location = new Point(400, 80);
            btnBrowse.Click += BtnBrowse_Click;
            this.Controls.Add(btnBrowse);

            pictureBox1 = new PictureBox();
            pictureBox1.Location = new Point(400, 110);
            pictureBox1.Size = new Size(200, 200);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(pictureBox1);
        }
        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                ofd.Title = "Pilih Bukti Lampiran";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedImagePath = ofd.FileName;
                    pictureBox1.Image = Image.FromFile(selectedImagePath);
                }
            }
        }
        private void FormPengaduan_Load(object sender, EventArgs e)
        {
            LoadKategori();

            if (textBox6 != null)
            {
                btnBrowse.Location = new Point(textBox6.Right + 20, textBox6.Top);
                pictureBox1.Location = new Point(btnBrowse.Left, btnBrowse.Bottom + 10);
            }
        }
        private void LoadKategori()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT KategoriID, NamaKategori FROM KategoriPengaduan", conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        comboBox1.DataSource = dt;
                        comboBox1.DisplayMember = "NamaKategori";
                        comboBox1.ValueMember = "KategoriID";
                        comboBox1.SelectedIndex = -1;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat kategori: " + ex.Message, "Error");
            }
        }
        private void BtnSimpan_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                comboBox1.SelectedValue == null ||
                string.IsNullOrWhiteSpace(textBox5.Text) ||
                string.IsNullOrWhiteSpace(textBox6.Text))
            {
                MessageBox.Show("Semua kolom harus diisi!", "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }






