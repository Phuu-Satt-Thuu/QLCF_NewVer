using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCF_NewVer
{
    public partial class QuanLyNhanVien : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

        // Biến lưu trữ MaND đang được chọn trong grid
        private string _selectedMaND = null;
        public QuanLyNhanVien()
        {
            InitializeComponent();
        }

        private void QuanLyNhanVien_Load(object sender, EventArgs e)
        {
            // Tải dữ liệu lên ComboBox Lọc
            cbbLocDuLieu.Items.Add("Tất cả");
            cbbLocDuLieu.Items.Add("Admin");
            cbbLocDuLieu.Items.Add("NhanVien");
            cbbLocDuLieu.SelectedIndex = 0; // Chọn "Tất cả" làm mặc định

            // Tải dữ liệu lên DataGridView
            LoadData();
        }
        private void LoadData()
        {
            // Truy vấn LINQ to SQL để lấy NguoiDung
            // DÙNG SELECT ĐỂ CHỈ LẤY CÁC CỘT CẦN THIẾT
            // **TUYỆT ĐỐI KHÔNG LẤY CỘT MẬT KHẨU**
            var query = from nv in db.NguoiDungs
                        select new
                        {
                            nv.MaND,
                            nv.TaiKhoan,
                            nv.HoTen,
                            nv.ViTri,
                            nv.Luong,
                            nv.SDT,
                            nv.DiaChi,
                            nv.Email,
                            nv.NgaySinh,
                            nv.TrangThai
                        };

            // Đặt tên cột cho dễ nhìn
            dgvNhanVien.DataSource = query.ToList();
            dgvNhanVien.Columns["MaND"].HeaderText = "Mã NV";
            dgvNhanVien.Columns["TaiKhoan"].HeaderText = "Tài khoản";
            dgvNhanVien.Columns["HoTen"].HeaderText = "Họ tên";
            dgvNhanVien.Columns["ViTri"].HeaderText = "Vị trí";
            dgvNhanVien.Columns["Luong"].HeaderText = "Lương";
            dgvNhanVien.Columns["SDT"].HeaderText = "Điện thoại";
            dgvNhanVien.Columns["DiaChi"].HeaderText = "Địa chỉ";
            dgvNhanVien.Columns["Email"].HeaderText = "Email";
            dgvNhanVien.Columns["NgaySinh"].HeaderText = "Ngày sinh";
            dgvNhanVien.Columns["TrangThai"].HeaderText = "Trạng thái";

            dgvNhanVien.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            _selectedMaND = null; // Reset lựa chọn
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim().ToLower();
            string filter = cbbLocDuLieu.SelectedItem.ToString();

            // Bắt đầu với query cơ bản
            var query = db.NguoiDungs.AsQueryable();

            // 1. Lọc theo Vị trí (ComboBox)
            if (filter != "Tất cả")
            {
                query = query.Where(nv => nv.ViTri == filter);
            }

            // 2. Lọc theo Từ khóa (TextBox)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(nv =>
                    nv.HoTen.ToLower().Contains(searchTerm) ||
                    nv.TaiKhoan.ToLower().Contains(searchTerm) ||
                    nv.SDT.Contains(searchTerm) ||
                    nv.Email.ToLower().Contains(searchTerm)
                );
            }

            // Chạy query và hiển thị kết quả (vẫn giấu mật khẩu)
            dgvNhanVien.DataSource = query.Select(nv => new
            {
                nv.MaND,
                nv.TaiKhoan,
                nv.HoTen,
                nv.ViTri,
                nv.Luong,
                nv.SDT,
                nv.DiaChi,
                nv.Email,
                nv.NgaySinh,
                nv.TrangThai
            }).ToList();
        }

        private void dgvNhanVien_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvNhanVien.Rows.Count)
            {
                // Lấy MaND từ hàng được chọn
                _selectedMaND = dgvNhanVien.Rows[e.RowIndex].Cells["MaND"].Value.ToString();
            }
        }

        private void btnThemNV_Click(object sender, EventArgs e)
        {
            ThemNV frm = new ThemNV(null); // Truyền null để báo cho form ThemNV biết đây là chế độ "Thêm mới"
            var result = frm.ShowDialog();
            if (result == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void btnXoaNV_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaND))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. KIỂM TRA QUAN TRỌNG: Không cho phép tự xóa mình
            if (_selectedMaND == Session.CurrentUser.MaND)
            {
                MessageBox.Show("Bạn không thể xóa chính tài khoản đang đăng nhập!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // 3. Tìm nhân viên được chọn trong CSDL
                NguoiDung userToDelete = db.NguoiDungs.FirstOrDefault(nv => nv.MaND == _selectedMaND);

                if (userToDelete == null)
                {
                    MessageBox.Show("Không tìm thấy nhân viên. Vui lòng tải lại danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 4. KIỂM TRA RÀNG BUỘC (Theo yêu cầu của bạn "trừ admin")
                if (userToDelete.ViTri == "Admin")
                {
                    MessageBox.Show("Không thể xóa tài khoản Quản trị viên (Admin).", "Bị từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                // 5. HIỂN THỊ HỘP THOẠI XÁC NHẬN
                var confirmResult = MessageBox.Show($"Bạn có thực sự muốn xóa nhân viên:\n\n" +
                                                    $"Họ tên: {userToDelete.HoTen}\n" +
                                                    $"Tài khoản: {userToDelete.TaiKhoan}\n\n" +
                                                    $"(Lưu ý: Hành động này không thể hoàn tác)",
                                                    "Xác nhận xóa",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Question);

                if (confirmResult == DialogResult.Yes) { 
                    // 6. TIẾN HÀNH XÓA
                    db.NguoiDungs.DeleteOnSubmit(userToDelete);
                    db.SubmitChanges(); // Gửi lệnh DELETE xuống CSDL

                    MessageBox.Show("Xóa nhân viên thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    _selectedMaND = null; // Xóa lựa chọn hiện tại
                    LoadData(); // Tải lại DataGridView
                }
                // else: Người dùng nhấn "No", không làm gì cả
            }
            catch (SqlException sqlEx)
            {
                // 7. BẮT LỖI NẾU KHÔNG XÓA ĐƯỢC (do đã lập hóa đơn)
                if (sqlEx.Number == 547) // 547 là mã lỗi của Foreign Key Constraint
                {
                    MessageBox.Show("Không thể xóa nhân viên này.\n\n" +
                                    "Lý do: Nhân viên này đã có lịch sử lập hóa đơn hoặc chấm công. " +
                                    "Để đảm bảo toàn vẹn dữ liệu, hệ thống không cho phép xóa.",
                                    "Lỗi ràng buộc dữ liệu",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi SQL: " + sqlEx.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSuaNV_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedMaND))
            {
                MessageBox.Show("Vui lòng chọn một nhân viên để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra không cho Admin tự sửa mình ở form này
            // (Vì có thể tự đổi từ Admin -> NhanVien và mất quyền)
            if (_selectedMaND == Session.CurrentUser.MaND)
            {
                MessageBox.Show("Bạn không thể sửa thông tin của chính mình tại đây. Vui lòng sử dụng mục 'Thông tin cá nhân'.", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi form SuaNV và truyền mã nhân viên đã chọn
            SuaNV frm = new SuaNV(_selectedMaND);

            var result = frm.ShowDialog();

            // Chỉ tải lại dữ liệu nếu Form SuaNV trả về kết quả OK
            if (result == DialogResult.OK)
            {
                LoadData(); // Tải lại grid để thấy thay đổi
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
            txtTimKiem.Clear();
            cbbLocDuLieu.SelectedIndex = 0; // Chọn lại "Tất cả"
            _selectedMaND = null;
        }
    }
}
