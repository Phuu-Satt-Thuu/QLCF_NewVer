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
    public partial class QuanLyKhachHang : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private int? _selectedMaKH = null;
        public QuanLyKhachHang()
        {
            InitializeComponent();
        }

        private void QuanLyKhachHang_Load(object sender, EventArgs e)
        {
            cbbLocDuLieu.Items.Add("Tất cả");
            cbbLocDuLieu.Items.Add("Khách hàng Kim Cương (>= 100000)");
            cbbLocDuLieu.Items.Add("Khách hàng Bạch Kim (>= 20000)");
            cbbLocDuLieu.Items.Add("Khách hàng Vàng (>= 5000)");
            cbbLocDuLieu.Items.Add("Khách hàng Bạc (>= 1000)");
            cbbLocDuLieu.Items.Add("Khách hàng Đồng (< 1000)");
            cbbLocDuLieu.SelectedIndex = 0;

            LoadData();
        }
        private string XepLoaiKhachHang(int diem)
        {
            if (diem >= 100000) return "Kim Cương";
            if (diem >= 20000) return "Bạch Kim";
            if (diem >= 5000) return "Vàng";
            if (diem >= 1000) return "Bạc";
            return "Đồng";
        }
        private void LoadData()
        {
            // Dùng 'let' để gọi hàm XepLoaiKhachHang
            var query = from kh in db.KhachHangs
                        let loaiKH = XepLoaiKhachHang(kh.TichDiem)
                        select new
                        {
                            kh.MaKH,
                            kh.TenKH,
                            kh.SDT,
                            kh.TichDiem,
                            Loai = loaiKH, // <-- CỘT MỚI
                            kh.DiaChi,    // <-- CỘT MỚI
                            kh.NgayTao    // <-- CỘT MỚI
                        };

            dgvKhachHang.DataSource = query.ToList();

            // Đặt lại tên cột
            dgvKhachHang.Columns["MaKH"].HeaderText = "Mã KH";
            dgvKhachHang.Columns["TenKH"].HeaderText = "Tên Khách Hàng";
            dgvKhachHang.Columns["SDT"].HeaderText = "Số Điện Thoại";
            dgvKhachHang.Columns["TichDiem"].HeaderText = "Tích Điểm";
            dgvKhachHang.Columns["Loai"].HeaderText = "Loại"; // <-- MỚI
            dgvKhachHang.Columns["DiaChi"].HeaderText = "Địa Chỉ"; // <-- MỚI
            dgvKhachHang.Columns["NgayTao"].HeaderText = "Ngày Tạo"; // <-- MỚI

            dgvKhachHang.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            _selectedMaKH = null;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim().ToLower();
            string filter = cbbLocDuLieu.SelectedItem.ToString();

            var query = db.KhachHangs.AsQueryable();

            // 1. Lọc theo điểm (ComboBox)
            if (filter.Contains("Kim Cương"))
                query = query.Where(kh => kh.TichDiem >= 100000);
            else if (filter.Contains("Bạch Kim"))
                query = query.Where(kh => kh.TichDiem >= 20000 && kh.TichDiem < 100000);
            else if (filter.Contains("Vàng"))
                query = query.Where(kh => kh.TichDiem >= 5000 && kh.TichDiem < 20000);
            else if (filter.Contains("Bạc"))
                query = query.Where(kh => kh.TichDiem >= 1000 && kh.TichDiem < 5000);
            else if (filter.Contains("Đồng"))
                query = query.Where(kh => kh.TichDiem < 1000);

            // 2. Lọc theo Từ khóa (TextBox)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(kh =>
                    kh.TenKH.ToLower().Contains(searchTerm) ||
                    kh.SDT.Contains(searchTerm) ||
                    (kh.DiaChi != null && kh.DiaChi.ToLower().Contains(searchTerm)) // <-- TÌM THEO ĐỊA CHỈ
                );
            }

            // Chạy query và hiển thị kết quả
            var result = from kh in query
                         let loaiKH = XepLoaiKhachHang(kh.TichDiem)
                         select new
                         {
                             kh.MaKH,
                             kh.TenKH,
                             kh.SDT,
                             kh.TichDiem,
                             Loai = loaiKH,
                             kh.DiaChi,
                             kh.NgayTao
                         };

            dgvKhachHang.DataSource = result.ToList();
        }

        private void dgvKhachHang_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvKhachHang.Rows.Count)
            {
                _selectedMaKH = Convert.ToInt32(dgvKhachHang.Rows[e.RowIndex].Cells["MaKH"].Value);
            }
        }

        private void btnThemKH_Click(object sender, EventArgs e)
        {
            ThemKH frm = new ThemKH();
            frm.ShowDialog();
        }

        private void btnSuaKH_Click(object sender, EventArgs e)
        {
            if (_selectedMaKH == null)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để sửa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Gọi form 'SuaKH' và truyền MaKH qua
            SuaKH f = new SuaKH(_selectedMaKH);
            var result = f.ShowDialog();

            // Nếu form SuaKH báo OK, tải lại dữ liệu
            if (result == DialogResult.OK)
            {
                LoadData();
            }
        }

        private void btnXoaKH_Click(object sender, EventArgs e)
        {
            if (_selectedMaKH == null)
            {
                MessageBox.Show("Vui lòng chọn một khách hàng để xóa.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == _selectedMaKH);
                if (khachHang == null)
                {
                    MessageBox.Show("Không tìm thấy khách hàng.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Xác nhận
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn XÓA VĨNH VIỄN khách hàng này?\n\n" +
                                              $"Tên: {khachHang.TenKH}\n" +
                                              $"SĐT: {khachHang.SDT}\n\n" +
                                              $"(Các hóa đơn cũ của họ sẽ bị 'mồ côi' - mất liên kết)",
                                              "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // ĐÂY LÀ LỆNH "XÓA CỨNG"
                    // (An toàn vì CSDL của bạn đã đặt ON DELETE SET NULL)
                    db.KhachHangs.DeleteOnSubmit(khachHang);
                    db.SubmitChanges(); // Gửi lệnh DELETE

                    MessageBox.Show("Xóa khách hàng thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Tải lại grid
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // 3. Reset lựa chọn đang chọn
            _selectedMaKH = null;
        }
    }
}
