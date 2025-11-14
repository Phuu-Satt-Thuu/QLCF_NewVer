namespace QLCF_NewVer
{
    partial class BanHang
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
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.cbbLocDuLieu = new System.Windows.Forms.ComboBox();
            this.txtTimKiem = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cbbTenKH = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLoaiKH = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btnThanhToan = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.btnThemVaoGioHang = new System.Windows.Forms.Button();
            this.dgvSanPham = new System.Windows.Forms.DataGridView();
            this.dgvSanPhamDaChon = new System.Windows.Forms.DataGridView();
            this.txtLuaChonMaGG = new System.Windows.Forms.TextBox();
            this.btnLuaChonMaGG = new System.Windows.Forms.Button();
            this.txtTienKhachDua = new System.Windows.Forms.TextBox();
            this.txtTienThoiLai = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTongTien = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanPham)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanPhamDaChon)).BeginInit();
            this.SuspendLayout();
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnTimKiem.BackColor = System.Drawing.Color.Red;
            this.btnTimKiem.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimKiem.ForeColor = System.Drawing.Color.White;
            this.btnTimKiem.Location = new System.Drawing.Point(969, 28);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(115, 70);
            this.btnTimKiem.TabIndex = 37;
            this.btnTimKiem.Text = "Tìm kiếm";
            this.btnTimKiem.UseVisualStyleBackColor = false;
            this.btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            // 
            // cbbLocDuLieu
            // 
            this.cbbLocDuLieu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbbLocDuLieu.FormattingEnabled = true;
            this.cbbLocDuLieu.Location = new System.Drawing.Point(732, 78);
            this.cbbLocDuLieu.Name = "cbbLocDuLieu";
            this.cbbLocDuLieu.Size = new System.Drawing.Size(198, 21);
            this.cbbLocDuLieu.TabIndex = 36;
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTimKiem.Location = new System.Drawing.Point(732, 28);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(198, 20);
            this.txtTimKiem.TabIndex = 35;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label2.Location = new System.Drawing.Point(647, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 34;
            this.label2.Text = "Lọc theo";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.Location = new System.Drawing.Point(564, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(162, 20);
            this.label1.TabIndex = 33;
            this.label1.Text = "Tìm kiếm sản phẩm";
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label6.Location = new System.Drawing.Point(82, 31);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(137, 20);
            this.label6.TabIndex = 38;
            this.label6.Text = "Tên khách hàng";
            // 
            // cbbTenKH
            // 
            this.cbbTenKH.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbbTenKH.FormattingEnabled = true;
            this.cbbTenKH.Location = new System.Drawing.Point(234, 30);
            this.cbbTenKH.Name = "cbbTenKH";
            this.cbbTenKH.Size = new System.Drawing.Size(198, 21);
            this.cbbTenKH.TabIndex = 40;
            this.cbbTenKH.SelectedIndexChanged += new System.EventHandler(this.cbbTenKH_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label4.Location = new System.Drawing.Point(78, 79);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(141, 20);
            this.label4.TabIndex = 43;
            this.label4.Text = "Loại khách hàng";
            // 
            // txtLoaiKH
            // 
            this.txtLoaiKH.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtLoaiKH.Location = new System.Drawing.Point(234, 80);
            this.txtLoaiKH.Name = "txtLoaiKH";
            this.txtLoaiKH.Size = new System.Drawing.Size(198, 20);
            this.txtLoaiKH.TabIndex = 44;
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label7.Location = new System.Drawing.Point(135, 208);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 20);
            this.label7.TabIndex = 78;
            this.label7.Text = "Tổng tiền";
            // 
            // btnThanhToan
            // 
            this.btnThanhToan.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThanhToan.BackColor = System.Drawing.Color.Red;
            this.btnThanhToan.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThanhToan.ForeColor = System.Drawing.Color.White;
            this.btnThanhToan.Location = new System.Drawing.Point(886, 607);
            this.btnThanhToan.Name = "btnThanhToan";
            this.btnThanhToan.Size = new System.Drawing.Size(119, 35);
            this.btnThanhToan.TabIndex = 81;
            this.btnThanhToan.Text = "Thanh toán";
            this.btnThanhToan.UseVisualStyleBackColor = false;
            this.btnThanhToan.Click += new System.EventHandler(this.btnThanhToan_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThoat.BackColor = System.Drawing.Color.Blue;
            this.btnThoat.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.ForeColor = System.Drawing.Color.White;
            this.btnThoat.Location = new System.Drawing.Point(1011, 607);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(97, 35);
            this.btnThoat.TabIndex = 80;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnThemVaoGioHang
            // 
            this.btnThemVaoGioHang.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemVaoGioHang.BackColor = System.Drawing.Color.Red;
            this.btnThemVaoGioHang.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThemVaoGioHang.ForeColor = System.Drawing.Color.White;
            this.btnThemVaoGioHang.Location = new System.Drawing.Point(16, 607);
            this.btnThemVaoGioHang.Name = "btnThemVaoGioHang";
            this.btnThemVaoGioHang.Size = new System.Drawing.Size(167, 35);
            this.btnThemVaoGioHang.TabIndex = 82;
            this.btnThemVaoGioHang.Text = "Thêm vào giỏ hàng";
            this.btnThemVaoGioHang.UseVisualStyleBackColor = false;
            this.btnThemVaoGioHang.Click += new System.EventHandler(this.btnThemVaoGioHang_Click);
            // 
            // dgvSanPham
            // 
            this.dgvSanPham.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvSanPham.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSanPham.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSanPham.Location = new System.Drawing.Point(592, 259);
            this.dgvSanPham.Name = "dgvSanPham";
            this.dgvSanPham.Size = new System.Drawing.Size(516, 323);
            this.dgvSanPham.TabIndex = 83;
            // 
            // dgvSanPhamDaChon
            // 
            this.dgvSanPhamDaChon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvSanPhamDaChon.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvSanPhamDaChon.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSanPhamDaChon.Location = new System.Drawing.Point(16, 259);
            this.dgvSanPhamDaChon.Name = "dgvSanPhamDaChon";
            this.dgvSanPhamDaChon.Size = new System.Drawing.Size(516, 323);
            this.dgvSanPhamDaChon.TabIndex = 84;
            this.dgvSanPhamDaChon.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSanPhamDaChon_CellClick);
            // 
            // txtLuaChonMaGG
            // 
            this.txtLuaChonMaGG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtLuaChonMaGG.Location = new System.Drawing.Point(234, 136);
            this.txtLuaChonMaGG.Name = "txtLuaChonMaGG";
            this.txtLuaChonMaGG.Size = new System.Drawing.Size(198, 20);
            this.txtLuaChonMaGG.TabIndex = 85;
            // 
            // btnLuaChonMaGG
            // 
            this.btnLuaChonMaGG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLuaChonMaGG.BackColor = System.Drawing.Color.Red;
            this.btnLuaChonMaGG.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLuaChonMaGG.ForeColor = System.Drawing.Color.White;
            this.btnLuaChonMaGG.Location = new System.Drawing.Point(49, 110);
            this.btnLuaChonMaGG.Name = "btnLuaChonMaGG";
            this.btnLuaChonMaGG.Size = new System.Drawing.Size(170, 70);
            this.btnLuaChonMaGG.TabIndex = 86;
            this.btnLuaChonMaGG.Text = "Lựa chọn mã giảm giá (nếu có)";
            this.btnLuaChonMaGG.UseVisualStyleBackColor = false;
            this.btnLuaChonMaGG.Click += new System.EventHandler(this.btnLuaChonMaGG_Click);
            // 
            // txtTienKhachDua
            // 
            this.txtTienKhachDua.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTienKhachDua.Location = new System.Drawing.Point(732, 136);
            this.txtTienKhachDua.Name = "txtTienKhachDua";
            this.txtTienKhachDua.Size = new System.Drawing.Size(198, 20);
            this.txtTienKhachDua.TabIndex = 88;
            // 
            // txtTienThoiLai
            // 
            this.txtTienThoiLai.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTienThoiLai.Location = new System.Drawing.Point(732, 208);
            this.txtTienThoiLai.Name = "txtTienThoiLai";
            this.txtTienThoiLai.ReadOnly = true;
            this.txtTienThoiLai.Size = new System.Drawing.Size(198, 20);
            this.txtTienThoiLai.TabIndex = 87;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label3.Location = new System.Drawing.Point(625, 208);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(101, 20);
            this.label3.TabIndex = 90;
            this.label3.Text = "Tiền thối lại";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label5.Location = new System.Drawing.Point(495, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(231, 20);
            this.label5.TabIndex = 89;
            this.label5.Text = "Nhập vào số tiền khách đưa";
            // 
            // txtTongTien
            // 
            this.txtTongTien.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTongTien.Location = new System.Drawing.Point(234, 208);
            this.txtTongTien.Name = "txtTongTien";
            this.txtTongTien.ReadOnly = true;
            this.txtTongTien.Size = new System.Drawing.Size(198, 20);
            this.txtTongTien.TabIndex = 91;
            // 
            // BanHang
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Wheat;
            this.ClientSize = new System.Drawing.Size(1120, 654);
            this.Controls.Add(this.txtTongTien);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtTienKhachDua);
            this.Controls.Add(this.txtTienThoiLai);
            this.Controls.Add(this.btnLuaChonMaGG);
            this.Controls.Add(this.txtLuaChonMaGG);
            this.Controls.Add(this.dgvSanPhamDaChon);
            this.Controls.Add(this.dgvSanPham);
            this.Controls.Add(this.btnThemVaoGioHang);
            this.Controls.Add(this.btnThanhToan);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtLoaiKH);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbbTenKH);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.btnTimKiem);
            this.Controls.Add(this.cbbLocDuLieu);
            this.Controls.Add(this.txtTimKiem);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "BanHang";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "BanHang";
            this.Load += new System.EventHandler(this.BanHang_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanPham)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSanPhamDaChon)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.ComboBox cbbLocDuLieu;
        private System.Windows.Forms.TextBox txtTimKiem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cbbTenKH;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtLoaiKH;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btnThanhToan;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Button btnThemVaoGioHang;
        private System.Windows.Forms.DataGridView dgvSanPham;
        private System.Windows.Forms.DataGridView dgvSanPhamDaChon;
        private System.Windows.Forms.TextBox txtLuaChonMaGG;
        private System.Windows.Forms.Button btnLuaChonMaGG;
        private System.Windows.Forms.TextBox txtTienKhachDua;
        private System.Windows.Forms.TextBox txtTienThoiLai;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTongTien;
    }
}