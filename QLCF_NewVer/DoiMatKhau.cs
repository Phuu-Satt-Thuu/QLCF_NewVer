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
    public partial class DoiMatKhau : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private NguoiDung _currentUser;
        public DoiMatKhau()
        {
            InitializeComponent();
            if (Session.IsLoggedIn())
            {
                _currentUser = Session.CurrentUser;
            }
            else
            {
                // Nếu chưa đăng nhập, không cho mở form
                MessageBox.Show("Lỗi: Không tìm thấy phiên đăng nhập. Vui lòng đăng nhập lại.", "Lỗi Session", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close(); // Đóng form ngay
            }
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

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {
            string mkCu = txtMatKhauCu.Text;
            string mkMoi = txtMatKhauMoi.Text;
            string mkXacNhan = txtXacNhanMatKhauMoi.Text;

            if (string.IsNullOrEmpty(mkCu) || string.IsNullOrEmpty(mkMoi) || string.IsNullOrEmpty(mkXacNhan))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ cả 3 ô.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (mkMoi != mkXacNhan)
            {
                MessageBox.Show("Mật khẩu mới và mật khẩu xác nhận không khớp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (mkCu == mkMoi)
            {
                MessageBox.Show("Mật khẩu mới phải khác mật khẩu cũ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Băm mật khẩu cũ mà người dùng nhập
                string mkCu_Hashed = MaHoaSHA256(mkCu);

                // Tìm người dùng trong 'db' chung của form
                // (Chúng ta dùng MaND từ Session để tìm)
                var userToUpdate = db.NguoiDungs.FirstOrDefault(nd => nd.MaND == _currentUser.MaND);

                if (userToUpdate == null)
                {
                    MessageBox.Show("Lỗi: Không tìm thấy tài khoản trong CSDL.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // So sánh mật khẩu cũ (đã băm) với mật khẩu trong CSDL
                if (userToUpdate.MatKhau != mkCu_Hashed)
                {
                    MessageBox.Show("Mật khẩu cũ không chính xác!", "Sai mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- BƯỚC 3: CẬP NHẬT MẬT KHẨU MỚI ---
                // (Nếu mật khẩu cũ đã đúng)

                string mkMoi_Hashed = MaHoaSHA256(mkMoi);
                userToUpdate.MatKhau = mkMoi_Hashed;

                db.SubmitChanges(); // Gửi lệnh UPDATE

                MessageBox.Show("Đổi mật khẩu thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Cập nhật lại mật khẩu trong Session (Rất quan trọng)
                _currentUser.MatKhau = mkMoi_Hashed;

                this.DialogResult = DialogResult.OK; // Báo thành công
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtMatKhauCu.Clear();
            txtMatKhauMoi.Clear();
            txtXacNhanMatKhauMoi.Clear();
            txtMatKhauCu.Focus();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
