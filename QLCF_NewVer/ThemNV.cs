using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace QLCF_NewVer
{
    public partial class ThemNV : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

        // Biến này để lưu MaND khi ở chế độ Sửa. 
        // Nếu là Thêm mới, nó sẽ là null.
        private string _maND_ToEdit;
        public ThemNV(string maND)
        {
            InitializeComponent();
            _maND_ToEdit = maND; // Lưu lại mã nhân viên cần sửa
        }
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

        private void ThemNV_Load(object sender, EventArgs e)
        {
            cbbViTri.Items.Add("Admin");
            cbbViTri.Items.Add("NhanVien");

            cbbNganHang.Items.Add("VCB");
            cbbNganHang.Items.Add("MB");
            cbbNganHang.Items.Add("AGR");
            cbbNganHang.Items.Add("OCB");
            cbbNganHang.Items.Add("SCB");
        }

        private void btnThemNV_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtMaNV.Text) ||
                string.IsNullOrEmpty(txtTenNV.Text) ||
                string.IsNullOrEmpty(txtTenTK.Text) ||
                cbbViTri.SelectedItem == null ||
                string.IsNullOrEmpty(txtLuong.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ các trường bắt buộc:\nMã NV, Tên NV, Tên TK, Vị trí, Lương.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // --- BƯỚC 2: XỬ LÝ LƯU (THÊM HOẶC SỬA) ---
            try
            {
                if (string.IsNullOrEmpty(_maND_ToEdit))
                {
                    // === LOGIC THÊM MỚI ===
                    if (string.IsNullOrEmpty(txtMatKhau.Text))
                    {
                        MessageBox.Show("Mật khẩu là bắt buộc khi thêm nhân viên mới.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Kiểm tra Mã NV hoặc Tài khoản đã tồn tại chưa
                    if (db.NguoiDungs.Any(n => n.MaND == txtMaNV.Text.Trim()))
                    {
                        MessageBox.Show("Mã nhân viên này đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    if (db.NguoiDungs.Any(n => n.TaiKhoan == txtTenTK.Text.Trim()))
                    {
                        MessageBox.Show("Tên tài khoản này đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Tạo đối tượng NguoiDung mới
                    NguoiDung nvMoi = new NguoiDung
                    {
                        MaND = txtMaNV.Text.Trim(),
                        TaiKhoan = txtTenTK.Text.Trim(),
                        MatKhau = MaHoaSHA256(txtMatKhau.Text), // Hash mật khẩu mới
                        HoTen = txtTenNV.Text.Trim(),
                        ViTri = cbbViTri.SelectedItem.ToString(),
                        Luong = luong,
                        SDT = txtSDT.Text.Trim(),
                        DiaChi = txtDiaChi.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        NganHang = cbbNganHang.SelectedItem?.ToString(), // Dùng ?. để tránh lỗi nếu không chọn
                        STK = txtMaThe.Text.Trim(),
                        NgaySinh = dtpNgaySinh.Value
                    };

                    db.NguoiDungs.InsertOnSubmit(nvMoi);
                    MessageBox.Show("Thêm nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    // === LOGIC CẬP NHẬT ===
                    var nvSua = db.NguoiDungs.FirstOrDefault(n => n.MaND == _maND_ToEdit);
                    if (nvSua == null) return; // (Đã check ở Form_Load nhưng check lại cho an toàn)

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

                    // Chỉ đổi mật khẩu NẾU người dùng nhập vào
                    if (!string.IsNullOrEmpty(txtMatKhau.Text))
                    {
                        nvSua.MatKhau = MaHoaSHA256(txtMatKhau.Text);
                    }

                    MessageBox.Show("Cập nhật thông tin thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // --- BƯỚC 3: LƯU THAY ĐỔI VÀ ĐÓNG FORM ---
                db.SubmitChanges(); // Gửi lệnh INSERT hoặc UPDATE xuống CSDL
                this.DialogResult = DialogResult.OK; // Đặt kết quả để form cha (QuanLyNhanVien) biết
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi lưu: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTenNV.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            txtEmail.Clear();
            txtLuong.Clear();
            txtMatKhau.Clear();
            txtNhapLaiMK.Clear();
            txtMaThe.Clear();
            cbbViTri.SelectedIndex = -1; // Bỏ chọn
            cbbNganHang.SelectedIndex = -1; // Bỏ chọn
            dtpNgaySinh.Value = DateTime.Now.AddYears(-18);

            // Không xóa txtMaNV và txtTenTK nếu đang ở chế độ Sửa
            if (string.IsNullOrEmpty(_maND_ToEdit))
            {
                txtMaNV.Clear();
                txtTenTK.Clear();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
