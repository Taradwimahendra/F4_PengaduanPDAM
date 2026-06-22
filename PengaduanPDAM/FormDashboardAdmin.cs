using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ExcelDataReader;

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

            comboBox1.Items.AddRange(new string[] { "diproses", "selesai", "ditolak" });
            dataGridView1.CellClick += DataGridView1_CellClick;
            dataGridView1.RowPostPaint += DataGridView1_RowPostPaint;
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

        private void FormDashboardAdmin_Load(object sender, EventArgs e)
        {
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.RowTemplate.Height = 65;

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

                    SqlCommand cmd = new SqlCommand("sp_GetAllPengaduan", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        dtPengaduan = new DataTable();
                        da.Fill(dtPengaduan);

                        DataView dv = dtPengaduan.DefaultView;
                        dv.RowFilter = !string.IsNullOrEmpty(searchQuery)
                            ? "Judul_Laporan LIKE '%" + searchQuery + "%'"
                            : "";

                        bindingSource.DataSource = dv.ToTable();
                        dataGridView1.DataSource = bindingSource;

                        if (dataGridView1.Columns.Contains("PengaduanID"))
                        {
                            dataGridView1.Columns["PengaduanID"].Visible = false;
                        }

                        BindControls();

                        TambahKolomFoto();
                        IsiKolomFoto();
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
            
        }

        private void LoadStats()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();


                    SqlCommand cmdTotal = new SqlCommand("sp_CountTotalPengaduan", conn);
                    cmdTotal.CommandType = CommandType.StoredProcedure;
                    SqlParameter paramTotal = new SqlParameter("@Total", SqlDbType.Int);
                    paramTotal.Direction = ParameterDirection.Output;
                    cmdTotal.Parameters.Add(paramTotal);
                    cmdTotal.ExecuteNonQuery();
                    int total = Convert.ToInt32(paramTotal.Value);

                    SqlCommand cmdStatus = new SqlCommand(
                        "SELECT COUNT(*) FROM Pengaduan WHERE StatusPengaduan = @Status", conn);

                    cmdStatus.Parameters.AddWithValue("@Status", "diproses");
                    int diproses = Convert.ToInt32(cmdStatus.ExecuteScalar());

                    cmdStatus.Parameters["@Status"].Value = "selesai";
                    int selesai = Convert.ToInt32(cmdStatus.ExecuteScalar());

                    lblStats.Text = $"Total Laporan: {total}   |   Diproses: {diproses}   |   Selesai: {selesai}";
                }
            }
            catch { }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            LoadData(textBox1.Text.Trim());
        }
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex  = dataGridView1.SelectedCells[0].RowIndex;
                int id        = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);
                string newStatus = comboBox1.Text;

                if (string.IsNullOrEmpty(newStatus))
                {
                    MessageBox.Show("Pilih status terlebih dahulu!", "Peringatan");
                    return;
                }

                if (MessageBox.Show("Yakin ingin mengubah status?", "Konfirmasi",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                        cmd.Parameters.AddWithValue("@StatusBaru", newStatus);
                        cmd.ExecuteNonQuery();

                        // Log aktivitas ke LogAktivitas
                        SqlCommand cmdLog = new SqlCommand(
                            "INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@aktivitas,GETDATE())",
                            conn, trans);
                        cmdLog.Parameters.AddWithValue("@aktivitas",
                            "UPDATE STATUS PENGADUAN ID: " + id + " → " + newStatus);
                        cmdLog.ExecuteNonQuery();

                        trans.Commit();

                        MessageBox.Show("Status berhasil diupdate!", "Sukses");
                        LoadData();
                        LoadStats();
                    }
                    catch (SqlException ex)
                    {
                        trans.Rollback();
                        SimpanLog("ROLLBACK UPDATE STATUS : " + ex.Message);
                        MessageBox.Show(ex.Message, "Gagal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        SimpanLog("GENERAL ERROR UPDATE : " + ex.Message);
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

        private void TambahKolomFoto()
        {
            // Pastikan kolom belum ada (hindari duplikat saat LoadData dipanggil ulang)
            if (dataGridView1.Columns.Contains("FotoLampiran")) return;

            DataGridViewImageColumn imgCol = new DataGridViewImageColumn();
            imgCol.Name = "FotoLampiran";
            imgCol.HeaderText = "Foto";
            imgCol.Width = 80;
            imgCol.ImageLayout = DataGridViewImageCellLayout.Zoom;
            imgCol.ValuesAreIcons = false;

            // Taruh setelah kolom terakhir
            dataGridView1.Columns.Add(imgCol);
        }

        private void IsiKolomFoto()
        {
            // Gambar placeholder X merah untuk file tidak ditemukan
            Image imgError = new Bitmap(60, 60);
            using (Graphics g = Graphics.FromImage(imgError))
            {
                g.Clear(Color.White);
                using (Pen p = new Pen(Color.Red, 4))
                {
                    g.DrawLine(p, 8, 8, 52, 52);
                    g.DrawLine(p, 52, 8, 8, 52);
                }
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (row.IsNewRow) continue;

                        int id = Convert.ToInt32(row.Cells["PengaduanID"].Value);

                        SqlCommand cmd = new SqlCommand(
                            "SELECT TOP 1 PathFile FROM Lampiran WHERE PengaduanID = @id", conn);
                        cmd.Parameters.AddWithValue("@id", id);

                        string pathFile = cmd.ExecuteScalar()?.ToString();

                        if (!string.IsNullOrEmpty(pathFile) && File.Exists(pathFile))
                        {
                            // Load thumbnail supaya tidak boros memori
                            using (Image original = Image.FromFile(pathFile))
                            {
                                row.Cells["FotoLampiran"].Value = original.GetThumbnailImage(60, 60, null, IntPtr.Zero);
                            }
                        }
                        else
                        {
                            row.Cells["FotoLampiran"].Value = imgError;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Gagal memuat foto: " + ex.Message, "Error");
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
            {
                int rowIndex = dataGridView1.SelectedCells[0].RowIndex;
                int id       = Convert.ToInt32(dataGridView1.Rows[rowIndex].Cells["PengaduanID"].Value);

                if (MessageBox.Show("Yakin ingin menghapus laporan ini?", "Konfirmasi",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
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
                            MessageBox.Show("Data berhasil dihapus!", "Sukses");
                        else
                            MessageBox.Show("Data tidak ditemukan!", "Peringatan",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        LoadData();
                        LoadStats();
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

        private void BtnResetData_Click(object sender, EventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    string query = @"
                 IF OBJECT_ID('dbo.Pengaduan_Backup') IS NOT NULL
                 BEGIN
                     DELETE FROM dbo.Lampiran;
                     DELETE FROM dbo.Pengaduan;
                     SET IDENTITY_INSERT dbo.Pengaduan ON;
                     INSERT INTO dbo.Pengaduan (PengaduanID, UserID, KategoriID, Judul_Laporan, Deskripsi_Laporan, Tanggal_Pengaduan, StatusPengaduan)
                     SELECT PengaduanID, UserID, KategoriID, Judul_Laporan, Deskripsi_Laporan, Tanggal_Pengaduan, StatusPengaduan 
                     FROM dbo.Pengaduan_Backup;
                     SET IDENTITY_INSERT dbo.Pengaduan OFF;
                 END";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                    MessageBox.Show("Data berhasil direset dari backup!", "Info",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData();
                    LoadStats();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Reset gagal: " + ex.Message, "Error");
            }
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Apakah Anda yakin ingin logout?",
                "Konfirmasi Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                SessionManager.ClearSession();
                FormLogin login = new FormLogin();
                login.Show();
                this.Close();
            }
        }

        private void BtnRekap_Click(object sender, EventArgs e)
        {
            RekapPengaduan rekapForm = new RekapPengaduan();
            rekapForm.ShowDialog();
        }

        private void btnImpExcel_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls|All Files (*.*)|*.*",
                Title = "Pilih File Excel"
            })
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string filePath = openFileDialog.FileName;

                        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            var result = reader.AsDataSet(new ExcelDataSetConfiguration
                            {
                                ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                                {
                                    UseHeaderRow = true
                                }
                            });

                            if (result.Tables.Count == 0)
                            {
                                MessageBox.Show("File Excel kosong atau tidak valid.", "Peringatan",
                                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }

                            DataTable dt = result.Tables[0];

                            // Tampilkan preview di DataGridView (readonly)
                            dataGridView1.DataSource = dt;
                            if (dataGridView1.Columns.Contains("PengaduanID"))
                            {
                                dataGridView1.Columns["PengaduanID"].Visible = false;
                            }
                            dataGridView1.Enabled = true; // Biarkan tetap aktif agar bisa di-scroll
                            dataGridView1.ReadOnly = true; // Cegah user mengetik/mengubah data
                            dataGridView1.AllowUserToAddRows = false;

                            // Aktifkan tombol Import To Database, nonaktifkan yang lain
                            btnImpDb.Enabled      = true;
                            btnResetData.Enabled  = false;
                        }

                        MessageBox.Show("File Excel berhasil dimuat.",
                            "Berhasil", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Gagal membaca file Excel: " + ex.Message,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // =====================================================================
        // btnImpDb_Click - Import data dari DataGridView ke tabel Pengaduan
        // =====================================================================
        private void btnImpDb_Click(object sender, EventArgs e)
        {
            DataTable dt = dataGridView1.DataSource as DataTable;

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("Tidak ada data untuk diimport. Silakan Import Excel terlebih dahulu.",
                    "Peringatan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show(
                    $"Akan mengimport {dt.Rows.Count} baris data ke database.\nLanjutkan?",
                    "Konfirmasi Import",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int sukses = 0;
            int gagal  = 0;

            SqlConnection conn = new SqlConnection(connString);
            conn.Open();
            SqlTransaction trans = conn.BeginTransaction();

            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    try
                    {
                        // Ambil nilai kolom dari Excel (secara fleksibel)
                        string judul = "", deskripsi = "", status = "diproses", userIdStr = "", katIdStr = "", tglStr = "", namaPelapor = "", namaKategori = "";
                        string noTelepon = "", alamat = "";
                        foreach (DataColumn col in row.Table.Columns)
                        {
                            string cName = col.ColumnName.ToLower().Replace(" ", "").Replace("_", "");
                            if (cName.Contains("judul")) judul = row[col].ToString().Trim();
                            else if (cName.Contains("deskripsi")) deskripsi = row[col].ToString().Trim();
                            else if (cName.Contains("status")) status = row[col].ToString().Trim();
                            else if (cName == "userid") userIdStr = row[col].ToString().Trim();
                            else if (cName == "kategoriid") katIdStr = row[col].ToString().Trim();
                            else if (cName.Contains("tanggal")) tglStr = row[col].ToString().Trim();
                            else if (cName.Contains("nama") && cName.Contains("pelapor")) namaPelapor = row[col].ToString().Trim();
                            else if (cName.Contains("kategori") && !cName.Contains("id")) namaKategori = row[col].ToString().Trim();
                            else if (cName.Contains("telepon") || cName.Contains("hp")) noTelepon = row[col].ToString().Trim();
                            else if (cName.Contains("alamat")) alamat = row[col].ToString().Trim();
                        }

                        if (string.IsNullOrEmpty(judul)) { gagal++; continue; }

                        int userId = 1;
                        if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int u)) {
                            userId = u;
                        } else if (!string.IsNullOrEmpty(namaPelapor)) {
                            SqlCommand cmdUser = new SqlCommand("SELECT TOP 1 UserID FROM UserLogin WHERE NamaLengkap LIKE '%' + @Nama + '%'", conn, trans);
                            cmdUser.Parameters.AddWithValue("@Nama", namaPelapor);
                            object resUser = cmdUser.ExecuteScalar();
                            if (resUser != null) {
                                userId = Convert.ToInt32(resUser);
                            } else {
                                // AUTO REGISTRASI: Jika user belum terdaftar, buat akun baru
                                string emailRandom = namaPelapor.Replace(" ", "").ToLower() + Guid.NewGuid().ToString().Substring(0, 4) + "@email.com";
                                SqlCommand cmdInsertUser = new SqlCommand(
                                    "INSERT INTO UserLogin (NamaLengkap, Email, Password, NoTelepon, Alamat, RoleUser) " +
                                    "VALUES (@Nama, @Email, '123456', @Telp, @Alamat, 'pelanggan'); SELECT SCOPE_IDENTITY();", conn, trans);
                                cmdInsertUser.Parameters.AddWithValue("@Nama", namaPelapor);
                                cmdInsertUser.Parameters.AddWithValue("@Email", emailRandom);
                                cmdInsertUser.Parameters.AddWithValue("@Telp", string.IsNullOrEmpty(noTelepon) ? "-" : noTelepon);
                                cmdInsertUser.Parameters.AddWithValue("@Alamat", string.IsNullOrEmpty(alamat) ? "-" : alamat);
                                
                                object newUserId = cmdInsertUser.ExecuteScalar();
                                if (newUserId != null) {
                                    userId = Convert.ToInt32(newUserId);
                                }
                            }
                        }

                        int kategoriId = 1;
                        if (!string.IsNullOrEmpty(katIdStr) && int.TryParse(katIdStr, out int k)) {
                            kategoriId = k;
                        } else if (!string.IsNullOrEmpty(namaKategori)) {
                            SqlCommand cmdKat = new SqlCommand("SELECT TOP 1 KategoriID FROM KategoriPengaduan WHERE NamaKategori LIKE '%' + @Kategori + '%'", conn, trans);
                            cmdKat.Parameters.AddWithValue("@Kategori", namaKategori);
                            object resKat = cmdKat.ExecuteScalar();
                            if (resKat != null) kategoriId = Convert.ToInt32(resKat);
                        }

                        // Debug log to LogAktivitas (via standard SqlCommand, within the same trans)
                        string dbgMsg = $"DEBUG: Pelapor='{namaPelapor}', Kat='{namaKategori}', UID={userId}, KID={kategoriId}";
                        SqlCommand cmdDbg = new SqlCommand("INSERT INTO LogAktivitas (aktivitas,waktu) VALUES (@msg, GETDATE())", conn, trans);
                        cmdDbg.Parameters.AddWithValue("@msg", dbgMsg);
                        cmdDbg.ExecuteNonQuery();

                        DateTime tgl  = DateTime.TryParse(tglStr, out DateTime d) ? d : DateTime.Now;
                        if (string.IsNullOrEmpty(status)) status = "diproses";

                        SqlCommand cmd = new SqlCommand("sp_ImportPengaduan", conn, trans);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@UserID",            userId);
                        cmd.Parameters.AddWithValue("@KategoriID",         kategoriId);
                        cmd.Parameters.AddWithValue("@Judul_Laporan",      judul);
                        cmd.Parameters.AddWithValue("@Deskripsi_Laporan",  deskripsi);
                        cmd.Parameters.AddWithValue("@Tanggal_Pengaduan",  tgl);
                        cmd.Parameters.AddWithValue("@StatusPengaduan",    status);
                        cmd.ExecuteNonQuery();
                        sukses++;
                    }
                    catch
                    {
                        gagal++;
                    }
                }

                // Log aktivitas
                SqlCommand cmdLog = new SqlCommand(
                    "INSERT INTO LogAktivitas (aktivitas, waktu) VALUES (@aktivitas, GETDATE())",
                    conn, trans);
                cmdLog.Parameters.AddWithValue("@aktivitas",
                    $"IMPORT EXCEL: {sukses} data berhasil, {gagal} data gagal");
                cmdLog.ExecuteNonQuery();

                trans.Commit();

                MessageBox.Show($"Import selesai!\n✅ Berhasil : {sukses} baris\n❌ Gagal    : {gagal} baris",
                    "Hasil Import", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Kembalikan tampilan ke mode normal
                dataGridView1.Enabled = true;
                btnImpDb.Enabled      = false;
                button1.Enabled       = true;
                btnUpdate.Enabled       = true;
                button3.Enabled       = true;
                btnResetData.Enabled  = true;

                LoadData();
                LoadStats();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                SimpanLog("ROLLBACK IMPORT EXCEL: " + ex.Message);
                MessageBox.Show("Import gagal: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void panelMain_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
