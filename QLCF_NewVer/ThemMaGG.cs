using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCF_NewVer
{
    public partial class ThemMaGG : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        DataGridViewCheckBoxColumn chkChon;
        public ThemMaGG()
        {
            InitializeComponent();
        }

        private void ThemMaGG_Load(object sender, EventArgs e)
        {
            cbbLoaiMaGG.DataSource = db.KieuVCs.ToList();
            cbbLoaiMaGG.DisplayMember = "TenLoai";
            cbbLoaiMaGG.ValueMember = "MaLoaiVC";
            cbbLoaiMaGG.SelectedIndex = -1; // Để trống lúc đầu

            // Tải danh sách Sản phẩm vào DataGridView (cho BOGO)
            LoadSanPhamDataGrid();

            // Đặt ngày mặc định
            dtpNgayBatDau.Value = DateTime.Today;
            dtpNgayKetThuc.Value = DateTime.Today.AddMonths(1);

            // Cập nhật giao diện lần đầu
            CapNhatGiaoDienTheoLoai();
        }
        private void LoadSanPhamDataGrid()
        {
            // Tạo cột CheckBox nếu chưa có
            if (dgvSanPham.Columns["chkChon"] == null)
            {
                chkChon = new DataGridViewCheckBoxColumn();
                chkChon.Name = "chkChon";
                chkChon.HeaderText = "Chọn";
                chkChon.Width = 50;
                dgvSanPham.Columns.Add(chkChon);
            }

            // Tải danh sách sản phẩm (chỉ những sản phẩm đang bán)
            var query = db.SanPhamKichCos
        .Where(spkc => spkc.TrangThaiSP == true) // Lọc trước
        .Select(spkc => new // Chọn (Select)
        {
            spkc.IdSPKC,
            TenSP = spkc.SanPham.TenSP,     // <-- Lấy từ bảng SanPhams qua Navigation
            KichCo = spkc.KichCo.KichCo1, // <-- Lấy từ bảng KichCos qua Navigation
            spkc.GiaBan
        });

            dgvSanPham.DataSource = query.ToList();

            // Ẩn cột ID
            dgvSanPham.Columns["IdSPKC"].Visible = false;
            dgvSanPham.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán";
        }

        private void cbbLoaiMaGG_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhatGiaoDienTheoLoai();
        }
        private void CapNhatGiaoDienTheoLoai()
        {
            // 1. Kiểm tra bằng SelectedItem (an toàn hơn)
            if (cbbLoaiMaGG.SelectedItem == null)
            {
                // Vô hiệu hóa tất cả
                txtGiaTriGiam.Enabled = false;
                numGiamPhanTram.Enabled = false;
                dgvSanPham.Enabled = false;
                btnThemMaGG.Enabled = false;
                btnThemMaGGMua1Tang1.Enabled = false;
                return;
            }

            // 2. Ép kiểu SelectedItem về đúng đối tượng KieuVC
            KieuVC loaiVoucherDaChon = (KieuVC)cbbLoaiMaGG.SelectedItem;

            // 3. Lấy MaLoaiVC từ đối tượng đó
            int maLoaiVC = loaiVoucherDaChon.MaLoaiVC; // <-- KHÔNG CÒN LỖI

            // 4. Code logic của bạn giữ nguyên
            if (maLoaiVC == 1) // Giảm %
            {
                txtGiaTriGiam.Enabled = false; txtGiaTriGiam.Clear();
                numGiamPhanTram.Enabled = true;
                dgvSanPham.Enabled = false;
                btnThemMaGG.Enabled = true;
                btnThemMaGGMua1Tang1.Enabled = false;
            }
            else if (maLoaiVC == 3) // Giảm giá trị thực
            {
                txtGiaTriGiam.Enabled = true;
                numGiamPhanTram.Enabled = false; numGiamPhanTram.Value = 0;
                dgvSanPham.Enabled = false;
                btnThemMaGG.Enabled = true;
                btnThemMaGGMua1Tang1.Enabled = false;
            }
            else if (maLoaiVC == 2 || maLoaiVC == 4) // BOGO (Mua 1 Tặng 1)
            {
                txtGiaTriGiam.Enabled = false; txtGiaTriGiam.Clear();
                numGiamPhanTram.Enabled = false; numGiamPhanTram.Value = 0;
                dgvSanPham.Enabled = true;
                btnThemMaGG.Enabled = false;
                btnThemMaGGMua1Tang1.Enabled = true;
            }
        }

        private void btnThemMaGG_Click(object sender, EventArgs e)
        {
            if (!ValidateInputChung()) return; // Kiểm tra chung

            try
            {
                int maLoaiVC = (int)cbbLoaiMaGG.SelectedValue;

                Voucher v = new Voucher();
                v.Code = txtMaGG.Text.Trim();
                v.TenMaGiamGia = txtTenMaGG.Text.Trim();
                v.NgayBD = dtpNgayBatDau.Value;
                v.NgayKT = dtpNgayKetThuc.Value;
                v.MaLoaiVC = maLoaiVC;

                // Điều kiện (giá trị đơn tối thiểu)
                v.DieuKien = decimal.TryParse(txtGiaTriDonToiThieu.Text, out decimal dk) ? dk : 0;

                // Giá trị voucher
                if (maLoaiVC == 1) // Giảm %
                {
                    v.GiaTri = numGiamPhanTram.Value;
                }
                else if (maLoaiVC == 3) // Giảm tiền
                {
                    v.GiaTri = decimal.TryParse(txtGiaTriGiam.Text, out decimal gt) ? gt : 0;
                }

                db.Vouchers.InsertOnSubmit(v);
                db.SubmitChanges();

                MessageBox.Show("Thêm mã giảm giá thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemMaGGMua1Tang1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputChung()) return; // Kiểm tra chung

            // Lấy danh sách sản phẩm được tặng
            List<int> dsSPTang = new List<int>();
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                // Kiểm tra ô checkbox có được check không
                if (row.Cells["chkChon"] != null && Convert.ToBoolean(row.Cells["chkChon"].Value) == true)
                {
                    int idSPKC = (int)row.Cells["IdSPKC"].Value;
                    dsSPTang.Add(idSPKC);
                }
            }

            if (dsSPTang.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm (check vào ô) để áp dụng cho mã Mua 1 Tặng 1.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // A. Lưu bảng Voucher (bảng cha)
                Voucher v = new Voucher();
                v.Code = txtMaGG.Text.Trim();
                v.TenMaGiamGia = txtTenMaGG.Text.Trim();
                v.NgayBD = dtpNgayBatDau.Value;
                v.NgayKT = dtpNgayKetThuc.Value;
                v.MaLoaiVC = (int)cbbLoaiMaGG.SelectedValue;
                v.DieuKien = decimal.TryParse(txtGiaTriDonToiThieu.Text, out decimal dk) ? dk : 0;
                v.GiaTri = 0; // Theo CSDL, BOGO có GiaTri = 0

                db.Vouchers.InsertOnSubmit(v);
                db.SubmitChanges(); // Gửi lệnh để lấy MaVC (PK)

                // B. Lấy MaVC vừa tạo
                int maVCVuaTao = v.MaVC;

                // C. Lưu bảng ChiTietVC (bảng con)
                List<ChiTietVC> dsChiTiet = new List<ChiTietVC>();
                foreach (int idSPKC in dsSPTang)
                {
                    dsChiTiet.Add(new ChiTietVC
                    {
                        MaVC = maVCVuaTao,
                        IdSPKC = idSPKC
                    });
                }

                db.ChiTietVCs.InsertAllOnSubmit(dsChiTiet);
                db.SubmitChanges(); // Gửi lệnh lưu bảng con

                MessageBox.Show("Thêm mã Mua 1 Tặng 1 thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidateInputChung()
        {
            if (string.IsNullOrEmpty(txtMaGG.Text) || string.IsNullOrEmpty(txtTenMaGG.Text) || cbbLoaiMaGG.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập Mã GG, Tên GG và chọn Loại GG.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpNgayKetThuc.Value < dtpNgayBatDau.Value)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", "Lỗi logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra Mã GG (Code) đã tồn tại chưa
            if (db.Vouchers.Any(v => v.Code == txtMaGG.Text.Trim()))
            {
                MessageBox.Show("Mã giảm giá này đã tồn tại. Vui lòng chọn mã khác.", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaGG.Clear();
            txtTenMaGG.Clear();
            cbbLoaiMaGG.SelectedIndex = -1;
            txtGiaTriGiam.Clear();
            numGiamPhanTram.Value = 0;
            txtGiaTriDonToiThieu.Clear();

            // Bỏ check tất cả
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (row.Cells["chkChon"] != null)
                {
                    row.Cells["chkChon"].Value = false;
                }
            }

            dtpNgayBatDau.Value = DateTime.Today;
            dtpNgayKetThuc.Value = DateTime.Today.AddMonths(1);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
