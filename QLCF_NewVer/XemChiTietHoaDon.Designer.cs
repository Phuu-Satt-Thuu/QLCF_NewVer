namespace QLCF_NewVer
{
    partial class XemChiTietHoaDon
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dgvChiTiet = new System.Windows.Forms.DataGridView();
            this.dgvThongTinChung = new System.Windows.Forms.DataGridView();
            this.btnThoat = new System.Windows.Forms.Button();
            this.btnXuatHoaDon = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThongTinChung)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvChiTiet
            // 
            this.dgvChiTiet.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvChiTiet.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvChiTiet.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChiTiet.Location = new System.Drawing.Point(12, 13);
            this.dgvChiTiet.Name = "dgvChiTiet";
            this.dgvChiTiet.Size = new System.Drawing.Size(530, 386);
            this.dgvChiTiet.TabIndex = 103;
            // 
            // dgvThongTinChung
            // 
            this.dgvThongTinChung.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvThongTinChung.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvThongTinChung.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvThongTinChung.Location = new System.Drawing.Point(592, 13);
            this.dgvThongTinChung.Name = "dgvThongTinChung";
            this.dgvThongTinChung.Size = new System.Drawing.Size(530, 386);
            this.dgvThongTinChung.TabIndex = 104;
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThoat.BackColor = System.Drawing.Color.Blue;
            this.btnThoat.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.ForeColor = System.Drawing.Color.White;
            this.btnThoat.Location = new System.Drawing.Point(1003, 405);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(119, 34);
            this.btnThoat.TabIndex = 107;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnXuatHoaDon
            // 
            this.btnXuatHoaDon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnXuatHoaDon.BackColor = System.Drawing.Color.Red;
            this.btnXuatHoaDon.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXuatHoaDon.ForeColor = System.Drawing.Color.White;
            this.btnXuatHoaDon.Location = new System.Drawing.Point(822, 405);
            this.btnXuatHoaDon.Name = "btnXuatHoaDon";
            this.btnXuatHoaDon.Size = new System.Drawing.Size(167, 34);
            this.btnXuatHoaDon.TabIndex = 106;
            this.btnXuatHoaDon.Text = "Xuất hóa đơn chi tiết";
            this.btnXuatHoaDon.UseVisualStyleBackColor = false;
            this.btnXuatHoaDon.Click += new System.EventHandler(this.btnXuatHoaDon_Click);
            // 
            // XemChiTietHoaDon
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Wheat;
            this.ClientSize = new System.Drawing.Size(1134, 450);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnXuatHoaDon);
            this.Controls.Add(this.dgvThongTinChung);
            this.Controls.Add(this.dgvChiTiet);
            this.Name = "XemChiTietHoaDon";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "XemChiTietHoaDon";
            this.Load += new System.EventHandler(this.XemChiTietHoaDon_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChiTiet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvThongTinChung)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvChiTiet;
        private System.Windows.Forms.DataGridView dgvThongTinChung;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Button btnXuatHoaDon;
    }
}