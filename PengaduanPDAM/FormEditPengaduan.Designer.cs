namespace PengaduanPDAM
{
    partial class FormEditPengaduan
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label labelJudul;
        private System.Windows.Forms.TextBox txtJudul;
        private System.Windows.Forms.Label labelKategori;
        private System.Windows.Forms.ComboBox cmbKategori;
        private System.Windows.Forms.Label labelDeskripsi;
        private System.Windows.Forms.TextBox txtDeskripsi;
        private System.Windows.Forms.Button btnSimpan;
        private System.Windows.Forms.Button btnBatal;
        private System.Windows.Forms.Label labelTitle;

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
            this.labelJudul = new System.Windows.Forms.Label();
            this.txtJudul = new System.Windows.Forms.TextBox();
            this.labelKategori = new System.Windows.Forms.Label();
            this.cmbKategori = new System.Windows.Forms.ComboBox();
            this.labelDeskripsi = new System.Windows.Forms.Label();
            this.txtDeskripsi = new System.Windows.Forms.TextBox();
            this.btnSimpan = new System.Windows.Forms.Button();
            this.btnBatal = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.SuspendLayout();
            
            // labelTitle
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.labelTitle.Location = new System.Drawing.Point(30, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(200, 30);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "Edit Pengaduan";
            
            // labelJudul
            this.labelJudul.AutoSize = true;
            this.labelJudul.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelJudul.Location = new System.Drawing.Point(30, 70);
            this.labelJudul.Name = "labelJudul";
            this.labelJudul.Size = new System.Drawing.Size(115, 20);
            this.labelJudul.TabIndex = 1;
            this.labelJudul.Text = "Judul Laporan :";
            
            // txtJudul
            this.txtJudul.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtJudul.Location = new System.Drawing.Point(30, 95);
            this.txtJudul.Name = "txtJudul";
            this.txtJudul.Size = new System.Drawing.Size(400, 27);
            this.txtJudul.TabIndex = 2;
            
            // labelKategori
            this.labelKategori.AutoSize = true;
            this.labelKategori.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelKategori.Location = new System.Drawing.Point(30, 135);
            this.labelKategori.Name = "labelKategori";
            this.labelKategori.Size = new System.Drawing.Size(73, 20);
            this.labelKategori.TabIndex = 3;
            this.labelKategori.Text = "Kategori :";
            
            // cmbKategori
            this.cmbKategori.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cmbKategori.FormattingEnabled = true;
            this.cmbKategori.Location = new System.Drawing.Point(30, 160);
            this.cmbKategori.Name = "cmbKategori";
            this.cmbKategori.Size = new System.Drawing.Size(200, 28);
            this.cmbKategori.TabIndex = 4;
            
            // labelDeskripsi
            this.labelDeskripsi.AutoSize = true;
            this.labelDeskripsi.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.labelDeskripsi.Location = new System.Drawing.Point(30, 200);
            this.labelDeskripsi.Name = "labelDeskripsi";
            this.labelDeskripsi.Size = new System.Drawing.Size(135, 20);
            this.labelDeskripsi.TabIndex = 5;
            this.labelDeskripsi.Text = "Deskripsi Laporan :";
            
            // txtDeskripsi
            this.txtDeskripsi.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.txtDeskripsi.Location = new System.Drawing.Point(30, 225);
            this.txtDeskripsi.Multiline = true;
            this.txtDeskripsi.Name = "txtDeskripsi";
            this.txtDeskripsi.Size = new System.Drawing.Size(400, 120);
            this.txtDeskripsi.TabIndex = 6;
            
            // btnSimpan
            this.btnSimpan.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.btnSimpan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSimpan.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnSimpan.ForeColor = System.Drawing.Color.White;
            this.btnSimpan.Location = new System.Drawing.Point(30, 370);
            this.btnSimpan.Name = "btnSimpan";
            this.btnSimpan.Size = new System.Drawing.Size(120, 35);
            this.btnSimpan.TabIndex = 7;
            this.btnSimpan.Text = "Simpan";
            this.btnSimpan.UseVisualStyleBackColor = false;
            
            // btnBatal
            this.btnBatal.BackColor = System.Drawing.Color.LightGray;
            this.btnBatal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBatal.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnBatal.ForeColor = System.Drawing.Color.Black;
            this.btnBatal.Location = new System.Drawing.Point(160, 370);
            this.btnBatal.Name = "btnBatal";
            this.btnBatal.Size = new System.Drawing.Size(120, 35);
            this.btnBatal.TabIndex = 8;
            this.btnBatal.Text = "Batal";
            this.btnBatal.UseVisualStyleBackColor = false;
            
            // FormEditPengaduan
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(460, 440);
            this.Controls.Add(this.btnBatal);
            this.Controls.Add(this.btnSimpan);
            this.Controls.Add(this.txtDeskripsi);
            this.Controls.Add(this.labelDeskripsi);
            this.Controls.Add(this.cmbKategori);
            this.Controls.Add(this.labelKategori);
            this.Controls.Add(this.txtJudul);
            this.Controls.Add(this.labelJudul);
            this.Controls.Add(this.labelTitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormEditPengaduan";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Laporan";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
