using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows.Forms;

namespace PengaduanPDAM
{
    public partial class RekapPengaduan : Form
    {
        string connString = "Data Source=TARA\\TARA;Initial Catalog=DBPengaduanPDAM;Integrated Security=True";

        // ✅ INI YANG KURANG — field dtHasil dideklarasikan di level class
        private DataTable dtHasil = new DataTable();

        public RekapPengaduan()
        {
            InitializeComponent();
            cmbStatus.SelectedIndex = 0;
            dtpDari.Value = DateTime.Now.AddMonths(-1);
            dtpSampai.Value = DateTime.Now;

            this.Load += RekapPengaduan_Load;
            this.btnFilter.Click += BtnFilter_Click;
            this.btnExport.Click += BtnExport_Click;
            this.btnClose.Click += BtnClose_Click;

            dataGridView1.RowPostPaint += DataGridView1_RowPostPaint;
        }

        private void DataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();
            var centerFormat = new System.Drawing.StringFormat()
            {
                Alignment = System.Drawing.StringAlignment.Center,
                LineAlignment = System.Drawing.StringAlignment.Center
            };
            var headerBounds = new System.Drawing.Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, System.Drawing.SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        private void BtnClose_Click(object sender, EventArgs e) => this.Close();

        private void RekapPengaduan_Load(object sender, EventArgs e)
        {
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;
            dtpDari.Value = DateTime.Now.AddMonths(-1);
            dtpSampai.Value = DateTime.Now;
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand("sp_FilterRekapPengaduan", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Dari", dtpDari.Value.Date);
                        cmd.Parameters.AddWithValue("@Sampai", dtpSampai.Value.Date);

                        string status = cmbStatus.Text == "Semua" ? "" : cmbStatus.Text.ToLower();
                        cmd.Parameters.AddWithValue("@Status", status);

                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            dtHasil = new DataTable();
                            da.Fill(dtHasil);
                            dataGridView1.DataSource = dtHasil;

                            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                            dataGridView1.ReadOnly = true;
                            dataGridView1.AllowUserToAddRows = false;

                            // Sembunyikan PengaduanID, ganti pakai nomor di row header
                            if (dataGridView1.Columns.Contains("PengaduanID"))
                                dataGridView1.Columns["PengaduanID"].Visible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat data rekap: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnFilter_Click(object sender, EventArgs e) => LoadData();

        private void BtnExport_Click(object sender, EventArgs e)
        {
            if (dtHasil == null || dtHasil.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diekspor!", "Peringatan",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DataPengaduan rpt = new DataPengaduan();
                rpt.SetDataSource(dtHasil);

                string filePath = Path.Combine(
                    Path.GetTempPath(),
                    "LaporanPengaduan_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf"
                );

                rpt.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat, filePath);
                System.Diagnostics.Process.Start(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal generate laporan: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFilter_Click_1(object sender, EventArgs e)
        {

        }

        private void btnExport_Click_1(object sender, EventArgs e)
        {

        }
    }
}