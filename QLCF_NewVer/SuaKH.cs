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
    public partial class SuaKH : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private int _maKH; // ID của khách hàng đang sửa
        private KhachHang _khachHangToEdit; // Đối tượng khách hàng

        public SuaKH(int? maKH)
        {
            InitializeComponent();
            if (maKH == null)
            {
                MessageBox.Show("Không có khách hàng nào được chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Đóng form ngay lập tức nếu không có MaKH
                this.Load += (s, e) => this.Close();
                return;
            }

            _maKH = maKH.Value;

            // Tìm khách hàng trong CSDL
            _khachHangToEdit = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == _maKH);

            if (_khachHangToEdit == null)
            {
                MessageBox.Show("Không tìm thấy khách hàng trong CSDL.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close();
            }
        }

        private void btnSuaKH_Click(object sender, EventArgs e)
        {
            string tenKH = txtTenKH.Text.Trim();
            string sdt = txtSDT.Text.Trim();
            string diaChi = txtDiaChi.Text.Trim();
            string tichDiemStr = txtTichDiem.Text.Trim();

            if (string.IsNullOrEmpty(tenKH) || string.IsNullOrEmpty(sdt))
            {
                MessageBox.Show("Tên khách hàng và Số điện thoại là bắt buộc.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sdt.Length != 10 || !sdt.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại phải là 10 chữ số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int tichDiem = 0;
            if (!int.TryParse(tichDiemStr, out tichDiem) || tichDiem < 0)
            {
                MessageBox.Show("Tích điểm phải là một con số hợp lệ (lớn hơn hoặc bằng 0).", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: KIỂM TRA TỒN TẠI (UNIQUE) ---
            try
            {
                // Kiểm tra xem SĐT mới có bị trùng với người KHÁC không
                // (cho phép SĐT giữ nguyên)
                if (db.KhachHangs.Any(kh => kh.SDT == sdt && kh.MaKH != _maKH))
                {
                    MessageBox.Show("Số điện thoại này đã tồn tại trong hệ thống.", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- BƯỚC 3: CẬP NHẬT VÀ LƯU VÀO CSDL ---
                // (Không cần tìm lại _khachHangToEdit vì đã có ở bước 2)
                _khachHangToEdit.TenKH = tenKH;
                _khachHangToEdit.SDT = sdt;
                _khachHangToEdit.DiaChi = diaChi;
                _khachHangToEdit.TichDiem = tichDiem;

                db.SubmitChanges(); // Gửi lệnh UPDATE

                MessageBox.Show("Cập nhật khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; // Đặt kết quả để form cha (QuanLyKhachHang) biết
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào CSDL: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SuaKH_Load(object sender, EventArgs e)
        {
            LoadDataToForm();
        }
        private void LoadDataToForm()
        {
            if (_khachHangToEdit == null) return;

            txtTenKH.Text = _khachHangToEdit.TenKH;
            txtSDT.Text = _khachHangToEdit.SDT;
            txtDiaChi.Text = _khachHangToEdit.DiaChi;
            txtTichDiem.Text = _khachHangToEdit.TichDiem.ToString();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDataToForm();
        }
    }
}
