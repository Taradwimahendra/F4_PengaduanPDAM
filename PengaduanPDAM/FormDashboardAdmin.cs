using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class FormDashboardAdmin : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";
        private BindingSource bindingSource = new BindingSource();
        private DataTable dtPengaduan = new DataTable();

        public FormDashboardAdmin()
        {
            InitializeComponent();
            
            // Remove the dynamic creation of lblStats from here since it's now in Designer.
            // this.Load += FormDashboardAdmin_Load; is already mapped in Designer.
            // button events are mapped in designer too, but if not we can map them here.
            // Note: to be safe I will just ensure the events are handled correctly.
            
            comboBox1.Items.AddRange(new string[] { "diproses", "selesai", "ditolak" });
            dataGridView1.CellClick += DataGridView1_CellClick;
        }

        private void FormDashboardAdmin_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            
            bindingNavigator1.BindingSource = bindingSource;

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
                    string query = @"SELECT p.PengaduanID, u.Email, u.NamaLengkap, p.KategoriID, p.Judul_Laporan, p.StatusPengaduan 
                                     FROM Pengaduan p
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

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtPengaduan = new DataTable();
                            da.Fill(dtPengaduan);
                            
                            bindingSource.DataSource = dtPengaduan;
                            dataGridView1.DataSource = bindingSource;
                            
                            BindControls();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data: " + ex.Message, "Error");
            }
        }

        private void BindControls()
        {
            textBox1.DataBindings.Clear();
            textBox1.DataBindings.Add("Text", bindingSource, "Judul_Laporan");
        }

        private void DataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // CellClick logic removed because DataBinding automatically updates textBox1
        }

        private void LoadStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Pengaduan", conn))
                    {
                        int total = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.CommandText = "SELECT COUNT(*) FROM Pengaduan WHERE StatusPengaduan = 'diproses'";
                        int diproses = Convert.ToInt32(cmd.ExecuteScalar());
                        cmd.CommandText = "SELECT COUNT(*) FROM Pengaduan WHERE StatusPengaduan = 'selesai'";
                        int selesai = Convert.ToInt32(cmd.ExecuteScalar());

                        lblStats.Text = $"Total Laporan: {total}   |   Diproses: {diproses}   |   Selesai: {selesai}";
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
                            // Update status in Pengaduan
                            string query = "UPDATE Pengaduan SET StatusPengaduan=@Status WHERE PengaduanID=@ID";
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Status", newStatus);
                                cmd.Parameters.AddWithValue("@ID", id);
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

       
        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah Anda yakin ingin logout?", "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                SessionManager.ClearSession();
                FormLogin login = new FormLogin();
                login.Show();
                this.Close();
            }
        }
    }
}
