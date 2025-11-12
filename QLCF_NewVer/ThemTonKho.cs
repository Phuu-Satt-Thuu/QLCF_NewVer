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
    public partial class ThemTonKho : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private int? _selectedIdSPKC = null;

        // Biến này chỉ dùng để lưu ID được gửi từ form cha
        private int? _idSPKCToLoad;
        public ThemTonKho(int? idSPKC) // Thêm tham số int? idSPKC
        {
            InitializeComponent();
            _idSPKCToLoad = idSPKC; // Lưu ID được gửi qua
        }

        private void btnThemTonKho_Click(object sender, EventArgs e)
        {
            if (cbbNhaCungCap.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_selectedIdSPKC == null) // Kiểm tra biến chung
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm (Mã SP + Size) hợp lệ.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap) || giaNhap <= 0)
            {
                MessageBox.Show("Giá nhập không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtSoLuongNhap.Text, out int soLuongNhap) || soLuongNhap <= 0)
            {
                MessageBox.Show("Số lượng nhập không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: LƯU VÀO CSDL ---
            try
            {
                NhapKho phieuNhap = new NhapKho();
                phieuNhap.MaNCC = (int)cbbNhaCungCap.SelectedValue;
                phieuNhap.NgayNhap = DateTime.Now;
                db.NhapKhos.InsertOnSubmit(phieuNhap);
                db.SubmitChanges();

                int maNKvuaTao = phieuNhap.MaNK;

                ChiTietNhapKho chiTiet = new ChiTietNhapKho();
                chiTiet.MaNK = maNKvuaTao;
                chiTiet.IdSPKC = _selectedIdSPKC.Value; // Dùng biến chung
                chiTiet.SoLuongNhap = soLuongNhap;
                chiTiet.GiaNhap = giaNhap;

                db.ChiTietNhapKhos.InsertOnSubmit(chiTiet);
                db.SubmitChanges(); // Trigger SQL sẽ tự động cập nhật tồn kho

                MessageBox.Show("Nhập kho thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ThemTonKho_Load(object sender, EventArgs e)
        {
            cbbNhaCungCap.DataSource = db.NhaCungCaps.ToList();
            cbbNhaCungCap.DisplayMember = "TenNCC";
            cbbNhaCungCap.ValueMember = "MaNCC";

            txtTongTien.Enabled = false;

            // KIỂM TRA: Form được mở theo cách nào?
            if (_idSPKCToLoad != null)
            {
                // CÁCH 1: Mở từ nút "Thêm tồn kho" (Đã chọn sản phẩm)
                // -> Tải thông tin có sẵn và KHÓA
                LoadDataForSelectedProduct();
            }
            else
            {
                // CÁCH 2: Mở từ nút "Thêm tồn kho" (Không chọn gì cả) - (Logic dự phòng)
                // -> Để form trống và cho phép nhập tay MaSP
                txtTenSP.Enabled = false;
                cbbSize.Enabled = false;
            }
            this.txtGiaNhap.TextChanged += new System.EventHandler(this.txtGiaNhap_TextChanged);
            this.txtSoLuongNhap.TextChanged += new System.EventHandler(this.txtSoLuongNhap_TextChanged);
        }
        private void LoadDataForSelectedProduct()
        {
            // 1. Lấy thông tin SP từ ID
            var productInfo = db.SanPhamKichCos
                .Where(spkc => spkc.IdSPKC == _idSPKCToLoad)
                .Select(spkc => new {
                    spkc.MaSP,
                    spkc.SanPham.TenSP,
                    spkc.MaKichCo
                })
                .FirstOrDefault();

            if (productInfo != null)
            {
                // 2. Điền thông tin vào các ô
                txtMaSP.Text = productInfo.MaSP;
                txtTenSP.Text = productInfo.TenSP;

                // 3. Tải TẤT CẢ size của SP đó vào ComboBox
                var allSizes = db.SanPhamKichCos
                                .Where(s => s.MaSP == productInfo.MaSP)
                                .Select(s => new {
                                    s.IdSPKC,
                                    TenHienThi = s.KichCo.KichCo1 // Dùng KichCo1
                                }).ToList();

                cbbSize.DataSource = allSizes;
                cbbSize.DisplayMember = "TenHienThi";
                cbbSize.ValueMember = "IdSPKC";

                // 4. Chọn đúng Size đã được gửi qua
                cbbSize.SelectedValue = _idSPKCToLoad;
                _selectedIdSPKC = _idSPKCToLoad; // Cập nhật biến _selectedIdSPKC

                // 5. KHÓA các ô không cho sửa
                txtMaSP.Enabled = false;
                txtTenSP.Enabled = false;
                cbbSize.Enabled = false;
            }
        }

        private void cbbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_idSPKCToLoad != null) return; // Bỏ qua nếu form đã tải sẵn

            if (cbbSize.SelectedValue != null)
            {
                _selectedIdSPKC = (int)cbbSize.SelectedValue;
            }
            else
            {
                _selectedIdSPKC = null;
            }
        }
        private void TinhTongTien()
        {
            // Thử chuyển đổi
            bool giaHopLe = decimal.TryParse(txtGiaNhap.Text, out decimal gia);
            bool soLuongHopLe = int.TryParse(txtSoLuongNhap.Text, out int soLuong);

            if (giaHopLe && soLuongHopLe)
            {
                // Nếu cả 2 hợp lệ, nhân và hiển thị
                txtTongTien.Text = (gia * soLuong).ToString("F0");
            }
            else
            {
                // Nếu 1 trong 2 không phải là số, xóa ô tổng tiền
                txtTongTien.Clear();
            }
        }


        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtGiaNhap_TextChanged(object sender, EventArgs e)
        {
            TinhTongTien();
        }



        private void txtSoLuongNhap_TextChanged(object sender, EventArgs e)
        {
            TinhTongTien();
        }
    }
}
