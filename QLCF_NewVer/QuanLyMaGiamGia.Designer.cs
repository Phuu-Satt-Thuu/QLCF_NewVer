namespace QLCF_NewVer
{
    partial class QuanLyMaGiamGia
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
            this.dgvMaGiamGia = new System.Windows.Forms.DataGridView();
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.cbbLocDuLieu = new System.Windows.Forms.ComboBox();
            this.txtTimKiem = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnThemMaGG = new System.Windows.Forms.Button();
            this.btnSuaMaGG = new System.Windows.Forms.Button();
            this.btnXoaMaGG = new System.Windows.Forms.Button();
            this.btnThoat = new System.Windows.Forms.Button();
            this.btnSuaMaGGMua1Tang1 = new System.Windows.Forms.Button();
            this.btnLamMoi = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaGiamGia)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvMaGiamGia
            // 
            this.dgvMaGiamGia.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvMaGiamGia.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMaGiamGia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMaGiamGia.Location = new System.Drawing.Point(58, 119);
            this.dgvMaGiamGia.Name = "dgvMaGiamGia";
            this.dgvMaGiamGia.Size = new System.Drawing.Size(857, 252);
            this.dgvMaGiamGia.TabIndex = 30;
            this.dgvMaGiamGia.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMaGiamGia_CellClick);
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnTimKiem.BackColor = System.Drawing.Color.Red;
            this.btnTimKiem.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimKiem.ForeColor = System.Drawing.Color.White;
            this.btnTimKiem.Location = new System.Drawing.Point(376, 23);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(115, 70);
            this.btnTimKiem.TabIndex = 29;
            this.btnTimKiem.Text = "Tìm kiếm";
            this.btnTimKiem.UseVisualStyleBackColor = false;
            this.btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            // 
            // cbbLocDuLieu
            // 
            this.cbbLocDuLieu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbbLocDuLieu.FormattingEnabled = true;
            this.cbbLocDuLieu.Location = new System.Drawing.Point(139, 73);
            this.cbbLocDuLieu.Name = "cbbLocDuLieu";
            this.cbbLocDuLieu.Size = new System.Drawing.Size(198, 21);
            this.cbbLocDuLieu.TabIndex = 28;
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTimKiem.Location = new System.Drawing.Point(139, 23);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(198, 20);
            this.txtTimKiem.TabIndex = 27;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label2.Location = new System.Drawing.Point(54, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 26;
            this.label2.Text = "Lọc theo";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.Location = new System.Drawing.Point(54, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 25;
            this.label1.Text = "Tìm kiếm";
            // 
            // btnThemMaGG
            // 
            this.btnThemMaGG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemMaGG.BackColor = System.Drawing.Color.Red;
            this.btnThemMaGG.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThemMaGG.ForeColor = System.Drawing.Color.White;
            this.btnThemMaGG.Location = new System.Drawing.Point(58, 394);
            this.btnThemMaGG.Name = "btnThemMaGG";
            this.btnThemMaGG.Size = new System.Drawing.Size(133, 35);
            this.btnThemMaGG.TabIndex = 31;
            this.btnThemMaGG.Text = "Thêm mã giảm giá";
            this.btnThemMaGG.UseVisualStyleBackColor = false;
            this.btnThemMaGG.Click += new System.EventHandler(this.btnThemMaGG_Click);
            // 
            // btnSuaMaGG
            // 
            this.btnSuaMaGG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSuaMaGG.BackColor = System.Drawing.Color.Red;
            this.btnSuaMaGG.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSuaMaGG.ForeColor = System.Drawing.Color.White;
            this.btnSuaMaGG.Location = new System.Drawing.Point(336, 394);
            this.btnSuaMaGG.Name = "btnSuaMaGG";
            this.btnSuaMaGG.Size = new System.Drawing.Size(133, 35);
            this.btnSuaMaGG.TabIndex = 32;
            this.btnSuaMaGG.Text = "Sửa mã giảm giá";
            this.btnSuaMaGG.UseVisualStyleBackColor = false;
            this.btnSuaMaGG.Click += new System.EventHandler(this.btnSuaMaGG_Click);
            // 
            // btnXoaMaGG
            // 
            this.btnXoaMaGG.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnXoaMaGG.BackColor = System.Drawing.Color.Red;
            this.btnXoaMaGG.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXoaMaGG.ForeColor = System.Drawing.Color.White;
            this.btnXoaMaGG.Location = new System.Drawing.Point(197, 394);
            this.btnXoaMaGG.Name = "btnXoaMaGG";
            this.btnXoaMaGG.Size = new System.Drawing.Size(133, 35);
            this.btnXoaMaGG.TabIndex = 33;
            this.btnXoaMaGG.Text = "Xóa mã giảm giá";
            this.btnXoaMaGG.UseVisualStyleBackColor = false;
            this.btnXoaMaGG.Click += new System.EventHandler(this.btnXoaMaGG_Click);
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThoat.BackColor = System.Drawing.Color.Blue;
            this.btnThoat.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.ForeColor = System.Drawing.Color.White;
            this.btnThoat.Location = new System.Drawing.Point(818, 394);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(97, 35);
            this.btnThoat.TabIndex = 34;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnSuaMaGGMua1Tang1
            // 
            this.btnSuaMaGGMua1Tang1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSuaMaGGMua1Tang1.BackColor = System.Drawing.Color.Red;
            this.btnSuaMaGGMua1Tang1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSuaMaGGMua1Tang1.ForeColor = System.Drawing.Color.White;
            this.btnSuaMaGGMua1Tang1.Location = new System.Drawing.Point(475, 394);
            this.btnSuaMaGGMua1Tang1.Name = "btnSuaMaGGMua1Tang1";
            this.btnSuaMaGGMua1Tang1.Size = new System.Drawing.Size(210, 35);
            this.btnSuaMaGGMua1Tang1.TabIndex = 70;
            this.btnSuaMaGGMua1Tang1.Text = "Sửa mã giảm giá mua 1 tặng 1";
            this.btnSuaMaGGMua1Tang1.UseVisualStyleBackColor = false;
            this.btnSuaMaGGMua1Tang1.Click += new System.EventHandler(this.btnSuaMaGGMua1Tang1_Click);
            // 
            // btnLamMoi
            // 
            this.btnLamMoi.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLamMoi.BackColor = System.Drawing.Color.Red;
            this.btnLamMoi.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLamMoi.ForeColor = System.Drawing.Color.White;
            this.btnLamMoi.Location = new System.Drawing.Point(693, 394);
            this.btnLamMoi.Name = "btnLamMoi";
            this.btnLamMoi.Size = new System.Drawing.Size(119, 35);
            this.btnLamMoi.TabIndex = 71;
            this.btnLamMoi.Text = "Làm mới";
            this.btnLamMoi.UseVisualStyleBackColor = false;
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            // 
            // QuanLyMaGiamGia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Wheat;
            this.ClientSize = new System.Drawing.Size(984, 450);
            this.Controls.Add(this.btnLamMoi);
            this.Controls.Add(this.btnSuaMaGGMua1Tang1);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnXoaMaGG);
            this.Controls.Add(this.btnSuaMaGG);
            this.Controls.Add(this.btnThemMaGG);
            this.Controls.Add(this.dgvMaGiamGia);
            this.Controls.Add(this.btnTimKiem);
            this.Controls.Add(this.cbbLocDuLieu);
            this.Controls.Add(this.txtTimKiem);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "QuanLyMaGiamGia";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QuanLyMaGiamGia";
            this.Load += new System.EventHandler(this.QuanLyMaGiamGia_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMaGiamGia)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvMaGiamGia;
        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.ComboBox cbbLocDuLieu;
        private System.Windows.Forms.TextBox txtTimKiem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnThemMaGG;
        private System.Windows.Forms.Button btnSuaMaGG;
        private System.Windows.Forms.Button btnXoaMaGG;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Button btnSuaMaGGMua1Tang1;
        private System.Windows.Forms.Button btnLamMoi;
    }
}