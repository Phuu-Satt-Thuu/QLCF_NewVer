namespace QLCF_NewVer
{
    partial class QuanLyKho
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
            this.btnLamMoi = new System.Windows.Forms.Button();
            this.btnThemTonKho = new System.Windows.Forms.Button();
            this.dgvTonKho = new System.Windows.Forms.DataGridView();
            this.btnThoat = new System.Windows.Forms.Button();
            this.btnTimKiem = new System.Windows.Forms.Button();
            this.cbbLocDuLieu = new System.Windows.Forms.ComboBox();
            this.txtTimKiem = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnXuatTonKho = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTonKho)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLamMoi
            // 
            this.btnLamMoi.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnLamMoi.BackColor = System.Drawing.Color.Red;
            this.btnLamMoi.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLamMoi.ForeColor = System.Drawing.Color.White;
            this.btnLamMoi.Location = new System.Drawing.Point(519, 392);
            this.btnLamMoi.Name = "btnLamMoi";
            this.btnLamMoi.Size = new System.Drawing.Size(119, 35);
            this.btnLamMoi.TabIndex = 72;
            this.btnLamMoi.Text = "Làm mới";
            this.btnLamMoi.UseVisualStyleBackColor = false;
            this.btnLamMoi.Click += new System.EventHandler(this.btnLamMoi_Click);
            // 
            // btnThemTonKho
            // 
            this.btnThemTonKho.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThemTonKho.BackColor = System.Drawing.Color.Red;
            this.btnThemTonKho.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThemTonKho.ForeColor = System.Drawing.Color.White;
            this.btnThemTonKho.Location = new System.Drawing.Point(49, 392);
            this.btnThemTonKho.Name = "btnThemTonKho";
            this.btnThemTonKho.Size = new System.Drawing.Size(119, 35);
            this.btnThemTonKho.TabIndex = 69;
            this.btnThemTonKho.Text = "Thêm tồn kho";
            this.btnThemTonKho.UseVisualStyleBackColor = false;
            this.btnThemTonKho.Click += new System.EventHandler(this.btnThemTonKho_Click);
            // 
            // dgvTonKho
            // 
            this.dgvTonKho.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.dgvTonKho.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTonKho.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTonKho.Location = new System.Drawing.Point(49, 119);
            this.dgvTonKho.Name = "dgvTonKho";
            this.dgvTonKho.Size = new System.Drawing.Size(707, 252);
            this.dgvTonKho.TabIndex = 68;
            this.dgvTonKho.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvTonKho_CellClick);
            // 
            // btnThoat
            // 
            this.btnThoat.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnThoat.BackColor = System.Drawing.Color.Blue;
            this.btnThoat.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnThoat.ForeColor = System.Drawing.Color.White;
            this.btnThoat.Location = new System.Drawing.Point(659, 392);
            this.btnThoat.Name = "btnThoat";
            this.btnThoat.Size = new System.Drawing.Size(97, 35);
            this.btnThoat.TabIndex = 67;
            this.btnThoat.Text = "Thoát";
            this.btnThoat.UseVisualStyleBackColor = false;
            this.btnThoat.Click += new System.EventHandler(this.btnThoat_Click);
            // 
            // btnTimKiem
            // 
            this.btnTimKiem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnTimKiem.BackColor = System.Drawing.Color.Red;
            this.btnTimKiem.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTimKiem.ForeColor = System.Drawing.Color.White;
            this.btnTimKiem.Location = new System.Drawing.Point(367, 36);
            this.btnTimKiem.Name = "btnTimKiem";
            this.btnTimKiem.Size = new System.Drawing.Size(115, 57);
            this.btnTimKiem.TabIndex = 66;
            this.btnTimKiem.Text = "Tìm kiếm";
            this.btnTimKiem.UseVisualStyleBackColor = false;
            this.btnTimKiem.Click += new System.EventHandler(this.btnTimKiem_Click);
            // 
            // cbbLocDuLieu
            // 
            this.cbbLocDuLieu.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.cbbLocDuLieu.FormattingEnabled = true;
            this.cbbLocDuLieu.Location = new System.Drawing.Point(130, 73);
            this.cbbLocDuLieu.Name = "cbbLocDuLieu";
            this.cbbLocDuLieu.Size = new System.Drawing.Size(198, 21);
            this.cbbLocDuLieu.TabIndex = 65;
            // 
            // txtTimKiem
            // 
            this.txtTimKiem.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtTimKiem.Location = new System.Drawing.Point(130, 23);
            this.txtTimKiem.Name = "txtTimKiem";
            this.txtTimKiem.Size = new System.Drawing.Size(198, 20);
            this.txtTimKiem.TabIndex = 64;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label2.Location = new System.Drawing.Point(45, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(79, 20);
            this.label2.TabIndex = 63;
            this.label2.Text = "Lọc theo";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(163)));
            this.label1.Location = new System.Drawing.Point(45, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 20);
            this.label1.TabIndex = 62;
            this.label1.Text = "Tìm kiếm";
            // 
            // btnXuatTonKho
            // 
            this.btnXuatTonKho.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnXuatTonKho.BackColor = System.Drawing.Color.Red;
            this.btnXuatTonKho.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnXuatTonKho.ForeColor = System.Drawing.Color.White;
            this.btnXuatTonKho.Location = new System.Drawing.Point(381, 392);
            this.btnXuatTonKho.Name = "btnXuatTonKho";
            this.btnXuatTonKho.Size = new System.Drawing.Size(119, 35);
            this.btnXuatTonKho.TabIndex = 73;
            this.btnXuatTonKho.Text = "Xuất tồn kho";
            this.btnXuatTonKho.UseVisualStyleBackColor = false;
            this.btnXuatTonKho.Click += new System.EventHandler(this.btnXuatTonKho_Click);
            // 
            // QuanLyKho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Wheat;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnXuatTonKho);
            this.Controls.Add(this.btnLamMoi);
            this.Controls.Add(this.btnThemTonKho);
            this.Controls.Add(this.dgvTonKho);
            this.Controls.Add(this.btnThoat);
            this.Controls.Add(this.btnTimKiem);
            this.Controls.Add(this.cbbLocDuLieu);
            this.Controls.Add(this.txtTimKiem);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "QuanLyKho";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QuanLyKho";
            this.Load += new System.EventHandler(this.QuanLyKho_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTonKho)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnLamMoi;
        private System.Windows.Forms.Button btnThemTonKho;
        private System.Windows.Forms.DataGridView dgvTonKho;
        private System.Windows.Forms.Button btnThoat;
        private System.Windows.Forms.Button btnTimKiem;
        private System.Windows.Forms.ComboBox cbbLocDuLieu;
        private System.Windows.Forms.TextBox txtTimKiem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnXuatTonKho;
    }
}