namespace PengaduanPDAM
{
    partial class FormPengaduan
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label labelNama;
        private System.Windows.Forms.TextBox txtNama;
        private System.Windows.Forms.Label labelJudul;
        private System.Windows.Forms.TextBox txtJudul;
        private System.Windows.Forms.Label labelKategori;
        private System.Windows.Forms.ComboBox cmbKategori;
        private System.Windows.Forms.Label labelDeskripsi;
        private System.Windows.Forms.TextBox txtDeskripsi;
        private System.Windows.Forms.Label labelUpload;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label lblFileName;
        private System.Windows.Forms.Button btnSubmit;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.labelNama = new System.Windows.Forms.Label();
            this.txtNama = new System.Windows.Forms.TextBox();
            this.labelJudul = new System.Windows.Forms.Label();
            this.txtJudul = new System.Windows.Forms.TextBox();
            this.labelKategori = new System.Windows.Forms.Label();
            this.cmbKategori = new System.Windows.Forms.ComboBox();
            this.labelDeskripsi = new System.Windows.Forms.Label();
            this.txtDeskripsi = new System.Windows.Forms.TextBox();
            this.labelUpload = new System.Windows.Forms.Label();
            this.btnUpload = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lblFileName = new System.Windows.Forms.Label();
            this.btnSubmit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // labelNama
            // 
            this.labelNama.AutoSize = true;
            this.labelNama.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelNama.Location = new System.Drawing.Point(30, 20);
            this.labelNama.Name = "labelNama";
            this.labelNama.Size = new System.Drawing.Size(116, 20);
            this.labelNama.TabIndex = 11;
            this.labelNama.Text = "Nama Lengkap :";
            // 
            // txtNama
            // 
            this.txtNama.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtNama.Location = new System.Drawing.Point(30, 45);
            this.txtNama.Name = "txtNama";
            this.txtNama.Size = new System.Drawing.Size(400, 27);
            this.txtNama.TabIndex = 12;
            // 
            // labelJudul
            // 
            this.labelJudul.AutoSize = true;
            this.labelJudul.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelJudul.Location = new System.Drawing.Point(30, 85);
            this.labelJudul.Name = "labelJudul";
            this.labelJudul.Size = new System.Drawing.Size(115, 20);
            this.labelJudul.TabIndex = 0;
            this.labelJudul.Text = "Judul Laporan :";
            // 
            // txtJudul
            // 
            this.txtJudul.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtJudul.Location = new System.Drawing.Point(30, 110);
            this.txtJudul.Name = "txtJudul";
            this.txtJudul.Size = new System.Drawing.Size(400, 27);
            this.txtJudul.TabIndex = 1;
            // 
            // labelKategori
            // 
            this.labelKategori.AutoSize = true;
            this.labelKategori.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelKategori.Location = new System.Drawing.Point(30, 150);
            this.labelKategori.Name = "labelKategori";
            this.labelKategori.Size = new System.Drawing.Size(73, 20);
            this.labelKategori.TabIndex = 2;
            this.labelKategori.Text = "Kategori :";
            // 
            // cmbKategori
            // 
            this.cmbKategori.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbKategori.FormattingEnabled = true;
            this.cmbKategori.Location = new System.Drawing.Point(30, 175);
            this.cmbKategori.Name = "cmbKategori";
            this.cmbKategori.Size = new System.Drawing.Size(200, 28);
            this.cmbKategori.TabIndex = 3;
            // 
            // labelDeskripsi
            // 
            this.labelDeskripsi.AutoSize = true;
            this.labelDeskripsi.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelDeskripsi.Location = new System.Drawing.Point(30, 215);
            this.labelDeskripsi.Name = "labelDeskripsi";
            this.labelDeskripsi.Size = new System.Drawing.Size(135, 20);
            this.labelDeskripsi.TabIndex = 4;
            this.labelDeskripsi.Text = "Deskripsi Laporan :";
            // 
            // txtDeskripsi
            // 
            this.txtDeskripsi.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtDeskripsi.Location = new System.Drawing.Point(30, 240);
            this.txtDeskripsi.Multiline = true;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(400, 140);
            this.txtDeskripsi.TabIndex = 5;
            // 
            // labelUpload
            // 
            this.labelUpload.AutoSize = true;
            this.labelUpload.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelUpload.Location = new System.Drawing.Point(460, 20);
            this.labelUpload.Name = "labelUpload";
            this.labelUpload.Size = new System.Drawing.Size(124, 20);
            this.labelUpload.TabIndex = 6;
            this.labelUpload.Text = "Upload Gambar :";
            // 
            // btnUpload
            // 
            this.btnUpload.BackColor = System.Drawing.Color.LightGray;
            this.btnUpload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpload.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnUpload.Location = new System.Drawing.Point(460, 45);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(100, 27);
            this.btnUpload.TabIndex = 7;
            this.btnUpload.Text = "Pilih File...";
            this.btnUpload.UseVisualStyleBackColor = false;
            // 
            // lblFileName
            // 
            this.lblFileName.AutoSize = true;
            this.lblFileName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);
            this.lblFileName.ForeColor = System.Drawing.Color.DimGray;
            this.lblFileName.Location = new System.Drawing.Point(570, 50);
            this.lblFileName.Name = "lblFileName";
            this.lblFileName.Size = new System.Drawing.Size(103, 15);
            this.lblFileName.TabIndex = 8;
            this.lblFileName.Text = "Tidak ada file dipilih";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Gainsboro;
            this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox1.Location = new System.Drawing.Point(460, 85);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(250, 295);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSubmit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSubmit.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(30, 400);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(150, 40);
            this.btnSubmit.TabIndex = 10;
            this.btnSubmit.Text = "Kirim Pengaduan";
            this.btnSubmit.UseVisualStyleBackColor = false;
            // 
            // FormPengaduan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(750, 530);
            this.Controls.Add(this.txtNama);
            this.Controls.Add(this.labelNama);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.lblFileName);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.labelUpload);
            this.Controls.Add(this.txtDeskripsi);
            this.Controls.Add(this.labelDeskripsi);
            this.Controls.Add(this.cmbKategori);
            this.Controls.Add(this.labelKategori);
            this.Controls.Add(this.txtJudul);
            this.Controls.Add(this.labelJudul);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "FormPengaduan";
            this.Text = "FormPengaduan";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
