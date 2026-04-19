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


