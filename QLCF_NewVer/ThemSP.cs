using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace QLCF_NewVer
{
    public partial class ThemSP : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        public ThemSP()
        {
            InitializeComponent();
        }

        private void ThemSP_Load(object sender, EventArgs e)
        {
            cbbLoaiSP.DataSource = db.LoaiSPs.ToList();
            cbbLoaiSP.DisplayMember = "TenLoai";
            cbbLoaiSP.ValueMember = "MaLoai";
            cbbLoaiSP.SelectedIndex = -1; // Để trống lúc đầu

            // Vô hiệu hóa các ô giá tiền
            txtGiaS.Enabled = false;
            txtGiaM.Enabled = false;
            txtGiaL.Enabled = false;
        }

        private void cbS_CheckedChanged(object sender, EventArgs e)
        {
            txtGiaS.Enabled = cbS.Checked;
            if (!cbS.Checked) txtGiaS.Clear();
        }

        private void cbM_CheckedChanged(object sender, EventArgs e)
        {
            txtGiaM.Enabled = cbM.Checked;
            if (!cbM.Checked) txtGiaM.Clear();
        }

        private void cbL_CheckedChanged(object sender, EventArgs e)
        {
            txtGiaL.Enabled = cbL.Checked;
            if (!cbL.Checked) txtGiaL.Clear();
        }

        private void btnThemAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";
            if (open.ShowDialog() == DialogResult.OK)
            {
                // Hiển thị ảnh
                picAnh.Image = Image.FromFile(open.FileName);
                // Lưu đường dẫn gốc của file ảnh vào Tag để tí xử lý
                picAnh.Tag = open.FileName;
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            cbbLoaiSP.SelectedIndex = -1;
            picAnh.Image = null;
            picAnh.Tag = null;

            cbS.Checked = false;
            cbM.Checked = false;
            cbL.Checked = false;

            // Các ô giá tiền sẽ tự động bị Clear và Disable
            // do sự kiện CheckedChanged ở trên
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            string maSP = txtMaSP.Text.Trim();
            string tenSP = txtTenSP.Text.Trim();

            if (string.IsNullOrEmpty(maSP) || string.IsNullOrEmpty(tenSP) || cbbLoaiSP.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập Mã SP, Tên SP và chọn Loại SP.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra Mã SP đã tồn tại chưa
            if (db.SanPhams.Any(sp => sp.MaSP == maSP))
            {
                MessageBox.Show("Mã sản phẩm này đã tồn tại. Vui lòng chọn mã khác.", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!cbS.Checked && !cbM.Checked && !cbL.Checked)
            {
                MessageBox.Show("Phải chọn ít nhất một kích cỡ (S, M, hoặc L).", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra giá tiền
            decimal giaS = 0, giaM = 0, giaL = 0;
            if (cbS.Checked && (!decimal.TryParse(txtGiaS.Text, out giaS) || giaS <= 0))
            {
                MessageBox.Show("Giá tiền Size S không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbM.Checked && (!decimal.TryParse(txtGiaM.Text, out giaM) || giaM <= 0))
            {
                MessageBox.Show("Giá tiền Size M không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbL.Checked && (!decimal.TryParse(txtGiaL.Text, out giaL) || giaL <= 0))
            {
                MessageBox.Show("Giá tiền Size L không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: TIẾN HÀNH THÊM VÀO CSDL ---
            try
            {
                // A. Thêm vào bảng SanPham (Bảng cha)
                SanPham spMoi = new SanPham();
                spMoi.MaSP = maSP;
                spMoi.TenSP = tenSP;
                spMoi.MaLoai = (int)cbbLoaiSP.SelectedValue;

                // Xử lý lưu hình ảnh
                string duongDanAnhGoc = picAnh.Tag as string;
                if (!string.IsNullOrEmpty(duongDanAnhGoc))
                {
                    // Tạo một tên file duy nhất
                    string tenFileMoi = $"{maSP}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(duongDanAnhGoc)}";

                    // Đường dẫn thư mục lưu ảnh (ví dụ: Images/products/ trong thư mục chạy .exe)
                    string thuMucLuu = Path.Combine(Application.StartupPath, "Images", "products");

                    // Đảm bảo thư mục tồn tại
                    Directory.CreateDirectory(thuMucLuu);

                    string duongDanMoi = Path.Combine(thuMucLuu, tenFileMoi);

                    // Copy file ảnh vào
                    File.Copy(duongDanAnhGoc, duongDanMoi, true);

                    // Lưu đường dẫn TƯƠNG ĐỐI vào CSDL
                    spMoi.DuongDanAnh = Path.Combine("Images", "products", tenFileMoi);
                }
                else
                {
                    spMoi.DuongDanAnh = null; // Hoặc một ảnh mặc định
                }

                db.SanPhams.InsertOnSubmit(spMoi);

                // B. Thêm vào bảng SanPhamKichCo (Bảng con)
                List<SanPhamKichCo> danhSachSize = new List<SanPhamKichCo>();

                if (cbS.Checked)
                {
                    danhSachSize.Add(new SanPhamKichCo
                    {
                        MaSP = maSP,
                        MaKichCo = 1, // Giả định 1 = S
                        GiaBan = giaS,
                        SoLuongTon = 0, // Mới thêm nên tồn kho = 0
                        CanhBaoTonKho = 10, // Mặc định cảnh báo
                        TrangThaiSP = true // Mặc định là đang bán
                    });
                }
                if (cbM.Checked)
                {
                    danhSachSize.Add(new SanPhamKichCo
                    {
                        MaSP = maSP,
                        MaKichCo = 2, // Giả định 2 = M
                        GiaBan = giaM,
                        SoLuongTon = 0,
                        CanhBaoTonKho = 10,
                        TrangThaiSP = true
                    });
                }
                if (cbL.Checked)
                {
                    danhSachSize.Add(new SanPhamKichCo
                    {
                        MaSP = maSP,
                        MaKichCo = 3, // Giả định 3 = L
                        GiaBan = giaL,
                        SoLuongTon = 0,
                        CanhBaoTonKho = 10,
                        TrangThaiSP = true
                    });
                }

                db.SanPhamKichCos.InsertAllOnSubmit(danhSachSize);

                // C. Gửi tất cả thay đổi xuống CSDL
                db.SubmitChanges();

                MessageBox.Show("Thêm sản phẩm mới thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; // Đặt kết quả để form cha (QuanLySanPham) biết
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi lưu vào CSDL: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
