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
    public partial class QuanLyMaGiamGia : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private int? _selectedMaVC = null;
        public QuanLyMaGiamGia()
        {
            InitializeComponent();
        }

        private void QuanLyMaGiamGia_Load(object sender, EventArgs e)
        {
            if (Session.CurrentUser.ViTri != "Admin")
            {
                MessageBox.Show("Bạn không có quyền truy cập chức năng này.");
                this.Close();
                return;
            }

            LoadComboBoxLoc();
            LoadData();
        }
        private void LoadComboBoxLoc()
        {
            // Tải danh sách Loại Voucher (KieuVC)
            var danhSachLoai = db.KieuVCs.ToList();

            // Tạo đối tượng "Tất cả"
            KieuVC tatCa = new KieuVC { MaLoaiVC = 0, TenLoai = "Tất cả" };
            danhSachLoai.Insert(0, tatCa);

            cbbLocDuLieu.DataSource = danhSachLoai;
            cbbLocDuLieu.DisplayMember = "TenLoai";
            cbbLocDuLieu.ValueMember = "MaLoaiVC";
        }

        private void LoadData()
        {
            var query = db.Vouchers.Join(db.KieuVCs, // 1. Bảng để join
        v => v.MaLoaiVC,    // 2. Khóa của bảng Vouchers
        k => k.MaLoaiVC,    // 3. Khóa của bảng KieuVCs
        (v, k) => new       // 4. Hàm Select kết quả
        {
            v.MaVC,
            v.Code,
            v.TenMaGiamGia,
            k.TenLoai, // Lấy TenLoai từ bảng KieuVCs (k)
            v.GiaTri,
            v.DieuKien,
            v.NgayBD,
            v.NgayKT,
            // Thêm cột trạng thái logic
            TrangThai = (v.NgayKT >= DateTime.Today) ? "Đang hoạt động" : "Đã hết hạn"
        });

            dgvMaGiamGia.DataSource = query.ToList();

            // Đặt lại tên cột
            dgvMaGiamGia.Columns["MaVC"].Visible = false; // Ẩn ID
            dgvMaGiamGia.Columns["Code"].HeaderText = "Mã Code";
            dgvMaGiamGia.Columns["TenMaGiamGia"].HeaderText = "Tên";
            dgvMaGiamGia.Columns["TenLoai"].HeaderText = "Loại";
            dgvMaGiamGia.Columns["GiaTri"].HeaderText = "Giá trị";
            dgvMaGiamGia.Columns["DieuKien"].HeaderText = "ĐK Tối thiểu";
            dgvMaGiamGia.Columns["NgayBD"].HeaderText = "Ngày BĐ";
            dgvMaGiamGia.Columns["NgayKT"].HeaderText = "Ngày KT";
            dgvMaGiamGia.Columns["TrangThai"].HeaderText = "Trạng thái";

            dgvMaGiamGia.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            _selectedMaVC = null; // Reset lựa chọn
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim().ToLower();
            int maLoaiVCDuocChon = (int)cbbLocDuLieu.SelectedValue;

            // Bắt đầu với query cơ bản
            var query = db.Vouchers.AsQueryable();

            // 1. Lọc theo Loại (ComboBox)
            if (maLoaiVCDuocChon != 0) // 0 là "Tất cả"
            {
                query = query.Where(v => v.MaLoaiVC == maLoaiVCDuocChon);
            }

            // 2. Lọc theo Từ khóa (TextBox)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(v =>
                    v.Code.ToLower().Contains(searchTerm) ||
                    v.TenMaGiamGia.ToLower().Contains(searchTerm)
                );
            }

            // Chạy query cuối cùng để hiển thị
            var result = query.Join(db.KieuVCs, // 1. Bảng để join
        v => v.MaLoaiVC,    // 2. Khóa của bảng 'query' (đã lọc)
        k => k.MaLoaiVC,    // 3. Khóa của bảng KieuVCs
        (v, k) => new       // 4. Hàm Select kết quả
        {
            v.MaVC,
            v.Code,
            v.TenMaGiamGia,
            k.TenLoai,
            v.GiaTri,
            v.DieuKien,
            v.NgayBD,
            v.NgayKT,
            TrangThai = (v.NgayKT >= DateTime.Today) ? "Đang hoạt động" : "Đã hết hạn"
        });

            dgvMaGiamGia.DataSource = result.ToList();
        }

        private void dgvMaGiamGia_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvMaGiamGia.Rows.Count)
            {
                _selectedMaVC = Convert.ToInt32(dgvMaGiamGia.Rows[e.RowIndex].Cells["MaVC"].Value);
            }
        }

        private void btnThemMaGG_Click(object sender, EventArgs e)
        {
            ThemMaGG frm = new ThemMaGG();
            frm.ShowDialog();
        }

        private void btnXoaMaGG_Click(object sender, EventArgs e)
        {
            if (_selectedMaVC == null)
            {
                MessageBox.Show("Vui lòng chọn một mã giảm giá để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 1. KIỂM TRA XEM VOUCHER ĐÃ ĐƯỢC SỬ DỤNG CHƯA
                // (Kiểm tra trong bảng 'ApMaVC')
                bool daDuocSuDung = db.ApMaVCs.Any(ap => ap.MaVC == _selectedMaVC);

                if (daDuocSuDung)
                {
                    // 2. NẾU ĐÃ DÙNG -> BÁO LỖI VÀ KHÔNG CHO XÓA
                    MessageBox.Show("Không thể xóa mã giảm giá này.\n\n" +
                                    "Lý do: Mã này đã được áp dụng vào ít nhất một hóa đơn.",
                                    "Lỗi ràng buộc dữ liệu",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                    return;
                }

                // 3. NẾU CHƯA DÙNG -> HỎI XÁC NHẬN XÓA CỨNG
                var voucher = db.Vouchers.FirstOrDefault(v => v.MaVC == _selectedMaVC);
                if (voucher == null)
                {
                    MessageBox.Show("Không tìm thấy voucher.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn XÓA VĨNH VIỄN mã này không?\n\n" +
                                              $"Code: {voucher.Code}\n" +
                                              $"Tên: {voucher.TenMaGiamGia}\n\n" +
                                              $"(Mã này chưa được sử dụng lần nào)",
                                              "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // 4. TIẾN HÀNH XÓA CỨNG (DELETE)

                    // Vì là BOGO, phải xóa ở bảng con (ChiTietVC) trước
                    if (voucher.MaLoaiVC == 2 || voucher.MaLoaiVC == 4)
                    {
                        var chiTiet = db.ChiTietVCs.Where(ct => ct.MaVC == _selectedMaVC);
                        db.ChiTietVCs.DeleteAllOnSubmit(chiTiet);
                    }

                    // Xóa ở bảng cha (Voucher)
                    db.Vouchers.DeleteOnSubmit(voucher);

                    // Gửi lệnh xuống CSDL
                    db.SubmitChanges();

                    MessageBox.Show("Xóa mã giảm giá thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Tải lại grid
                }
            }
            catch (Exception ex)
            {
                // Lỗi này không nên xảy ra, nhưng giữ lại để phòng ngừa
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSuaMaGG_Click(object sender, EventArgs e)
        {
            if (_selectedMaVC == null)
            {
                MessageBox.Show("Vui lòng chọn một mã giảm giá để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Lấy thông tin voucher
                var voucher = db.Vouchers.FirstOrDefault(v => v.MaVC == _selectedMaVC);
                if (voucher == null)
                {
                    MessageBox.Show("Không tìm thấy mã giảm giá.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // KIỂM TRA LOẠI: Chỉ cho phép loại 1 (Giảm %) hoặc 3 (Giảm Tiền)
                if (voucher.MaLoaiVC == 1 || voucher.MaLoaiVC == 3)
                {
                    SuaMaGG frm = new SuaMaGG(_selectedMaVC);
                    frm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Đây là voucher 'Mua 1 Tặng 1'. Vui lòng dùng nút 'Sửa mã giảm giá 1 tặng 1'.", "Sai chức năng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra voucher: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();

            // 2. Xóa các bộ lọc
            txtTimKiem.Clear();
            cbbLocDuLieu.SelectedValue = 0; // Giả sử 0 là giá trị của "Tất cả"

            // 3. Reset lựa chọn đang chọn
            _selectedMaVC = null;
        }

        private void btnSuaMaGGMua1Tang1_Click(object sender, EventArgs e)
        {
            if (_selectedMaVC == null)
            {
                MessageBox.Show("Vui lòng chọn một mã giảm giá BOGO để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Lấy thông tin voucher
                var voucher = db.Vouchers.FirstOrDefault(v => v.MaVC == _selectedMaVC);
                if (voucher == null)
                {
                    MessageBox.Show("Không tìm thấy mã giảm giá.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // KIỂM TRA LOẠI: Chỉ cho phép loại 2 hoặc 4 (BOGO)
                if (voucher.MaLoaiVC == 2 || voucher.MaLoaiVC == 4)
                {
                    SuaMaGGMua1Tang1 frm = new SuaMaGGMua1Tang1(_selectedMaVC);
                    frm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Đây là voucher 'Giảm %' hoặc 'Giảm tiền'. Vui lòng dùng nút 'Sửa mã giảm giá' (nút màu đỏ).", "Sai chức năng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kiểm tra voucher: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
