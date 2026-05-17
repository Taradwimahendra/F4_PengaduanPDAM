namespace PengaduanPDAM
{
    partial class FormDashboardUser
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Panel panelMain;
        private System.Windows.Forms.Button btnInputPengaduan;
        private System.Windows.Forms.Button btnRiwayatPengaduan;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Label lblWelcome;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblMainContent;

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
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnRiwayatPengaduan = new System.Windows.Forms.Button();
            this.btnInputPengaduan = new System.Windows.Forms.Button();
            this.panelHeader = new System.Windows.Forms.Panel();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.panelMain = new System.Windows.Forms.Panel();
            this.lblMainContent = new System.Windows.Forms.Label();
            this.panelSidebar.SuspendLayout();
            this.panelHeader.SuspendLayout();
            this.panelMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(53)))), ((int)(((byte)(65)))));
            this.panelSidebar.Controls.Add(this.btnLogout);
            this.panelSidebar.Controls.Add(this.btnRiwayatPengaduan);
            this.panelSidebar.Controls.Add(this.btnInputPengaduan);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(0, 70);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(250, 530);
            this.panelSidebar.TabIndex = 0;
            // 
            // btnLogout
            // 
            this.btnLogout.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.ForeColor = System.Drawing.Color.White;
            this.btnLogout.Location = new System.Drawing.Point(0, 160);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(250, 60);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnRiwayatPengaduan
            // 
            this.btnRiwayatPengaduan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRiwayatPengaduan.FlatAppearance.BorderSize = 0;
            this.btnRiwayatPengaduan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRiwayatPengaduan.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRiwayatPengaduan.ForeColor = System.Drawing.Color.White;
            this.btnRiwayatPengaduan.Location = new System.Drawing.Point(0, 100);
            this.btnRiwayatPengaduan.Name = "btnRiwayatPengaduan";
            this.btnRiwayatPengaduan.Size = new System.Drawing.Size(250, 60);
            this.btnRiwayatPengaduan.TabIndex = 1;
            this.btnRiwayatPengaduan.Text = "Riwayat Pengaduan";
            this.btnRiwayatPengaduan.UseVisualStyleBackColor = true;
            this.btnRiwayatPengaduan.Click += new System.EventHandler(this.btnRiwayatPengaduan_Click);
            // 
            // btnInputPengaduan
            // 
            this.btnInputPengaduan.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnInputPengaduan.FlatAppearance.BorderSize = 0;
            this.btnInputPengaduan.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnInputPengaduan.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnInputPengaduan.ForeColor = System.Drawing.Color.White;
            this.btnInputPengaduan.Location = new System.Drawing.Point(0, 40);
            this.btnInputPengaduan.Name = "btnInputPengaduan";
            this.btnInputPengaduan.Size = new System.Drawing.Size(250, 60);
            this.btnInputPengaduan.TabIndex = 0;
            this.btnInputPengaduan.Text = "Input Pengaduan";
            this.btnInputPengaduan.UseVisualStyleBackColor = true;
            this.btnInputPengaduan.Click += new System.EventHandler(this.btnInputPengaduan_Click);
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            this.panelHeader.Controls.Add(this.lblWelcome);
            this.panelHeader.Controls.Add(this.lblTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1000, 70);
            this.panelHeader.TabIndex = 1;
            // 
            // lblWelcome
            // 
            this.lblWelcome.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.ForeColor = System.Drawing.Color.White;
            this.lblWelcome.Location = new System.Drawing.Point(588, 24);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblWelcome.Size = new System.Drawing.Size(400, 21);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "Selamat datang, ...";
            this.lblWelcome.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(262, 32);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Dashboard Pelanggan";
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panelMain.Controls.Add(this.lblMainContent);
            this.panelMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMain.Location = new System.Drawing.Point(250, 70);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(750, 530);
            this.panelMain.TabIndex = 2;
            // 
            // lblMainContent
            // 
            this.lblMainContent.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblMainContent.AutoSize = true;
            this.lblMainContent.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainContent.ForeColor = System.Drawing.Color.Gray;
            this.lblMainContent.Location = new System.Drawing.Point(120, 220);
            this.lblMainContent.Name = "lblMainContent";
            this.lblMainContent.Size = new System.Drawing.Size(519, 30);
            this.lblMainContent.TabIndex = 0;
            this.lblMainContent.Text = "Silakan pilih menu di samping untuk mulai beraktivitas.";
            this.lblMainContent.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormDashboardUser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelHeader);
            this.Name = "FormDashboardUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dashboard Pelanggan";
            this.Load += new System.EventHandler(this.FormDashboardUser_Load);
            this.panelSidebar.ResumeLayout(false);
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelMain.ResumeLayout(false);
            this.panelMain.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}