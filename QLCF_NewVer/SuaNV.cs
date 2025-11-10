using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCF_NewVer
{
    public partial class SuaNV : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

        // Biến này BẮT BUỘC phải có để biết sửa ai
        private string _maND_ToEdit;
        public SuaNV(string maND)
        {
            InitializeComponent();
            if (string.IsNullOrEmpty(maND))
            {
                MessageBox.Show("Không có mã nhân viên nào được chọn để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                // Ngăn form mở lên nếu không có mã
                this.Load += (s, e) => this.Close();
                return;
            }
            _maND_ToEdit = maND; // Lưu lại mã nhân viên cần sửa
        }

        // 3. HÀM BĂM MẬT KHẨU (Copy từ form Đăng nhập)
        private string MaHoaSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void SuaNV_Load(object sender, EventArgs e)
        {
            cbbViTri.Items.Add("Admin");
            cbbViTri.Items.Add("NhanVien");

            cbbNganHang.Items.Add("VCB");
            cbbNganHang.Items.Add("MB");
            cbbNganHang.Items.Add("AGR");
            cbbNganHang.Items.Add("OCB");
            cbbNganHang.Items.Add("SCB");
        }
        private void LoadThongTinNhanVien()
        {
            var nv = db.NguoiDungs.FirstOrDefault(n => n.MaND == _maND_ToEdit);
            if (nv == null)
            {
                MessageBox.Show("Không tìm thấy nhân viên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            // Đổ dữ liệu vào các controls
            txtMaNV.Text = nv.MaND;
            txtTenNV.Text = nv.HoTen;
            txtSDT.Text = nv.SDT;
            txtDiaChi.Text = nv.DiaChi;
            txtEmail.Text = nv.Email;
            dtpNgaySinh.Value = nv.NgaySinh ?? DateTime.Now.AddYears(-18);
            txtTenTK.Text = nv.TaiKhoan;
            txtLuong.Text = nv.Luong?.ToString("F0");
            cbbViTri.SelectedItem = nv.ViTri;
            cbbNganHang.SelectedItem = nv.NganHang;
            txtMaThe.Text = nv.STK;

            // Xóa ô mật khẩu
            txtMatKhau.Clear();
            txtNhapLaiMK.Clear();
        }

        private void btnSuaNV_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenNV.Text) ||
                cbbViTri.SelectedItem == null ||
                string.IsNullOrEmpty(txtLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường bắt buộc:\nTên NV, Vị trí, Lương.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtMatKhau.Text != txtNhapLaiMK.Text)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            decimal luong;
            if (!decimal.TryParse(txtLuong.Text, out luong))
            {
                MessageBox.Show("Lương phải là một con số.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: XỬ LÝ CẬP NHẬT ---
            try
            {
                // Tìm nhân viên trong CSDL
                var nvSua = db.NguoiDungs.FirstOrDefault(n => n.MaND == _maND_ToEdit);
                if (nvSua == null)
                {
                    MessageBox.Show("Không tìm thấy nhân viên để cập nhật!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Cập nhật thông tin
                nvSua.HoTen = txtTenNV.Text.Trim();
                nvSua.ViTri = cbbViTri.SelectedItem.ToString();
                nvSua.Luong = luong;
                nvSua.SDT = txtSDT.Text.Trim();
                nvSua.DiaChi = txtDiaChi.Text.Trim();
                nvSua.Email = txtEmail.Text.Trim();
                nvSua.NganHang = cbbNganHang.SelectedItem?.ToString();
                nvSua.STK = txtMaThe.Text.Trim();
                nvSua.NgaySinh = dtpNgaySinh.Value;

                // Chỉ đổi mật khẩu NẾU người dùng nhập vào ô mật khẩu
                if (!string.IsNullOrEmpty(txtMatKhau.Text))
                {
                    nvSua.MatKhau = MaHoaSHA256(txtMatKhau.Text);
                }

                // --- BƯỚC 3: LƯU THAY ĐỔI VÀ ĐÓNG FORM ---
                db.SubmitChanges(); // Gửi lệnh UPDATE xuống CSDL

                MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; // Đặt kết quả để form cha (QuanLyNhanVien) biết
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi cập nhật: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadThongTinNhanVien();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
