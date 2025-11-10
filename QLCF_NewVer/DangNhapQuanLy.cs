using System;
using System.Linq;
using System.Security.Cryptography; // Cần cho SHA256
using System.Text;
using System.Windows.Forms;

namespace QLCF_NewVer
{
    public partial class DangNhapQuanLy : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        public DangNhapQuanLy()
        {
            InitializeComponent();
        }
        // 1. HÀM BĂM MẬT KHẨU SHA256
        // Hàm này phải giống hệt hàm bạn dùng để tạo mật khẩu mẫu
        private string MaHoaSHA256(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2")); // "x2" để format hex
                }
                return builder.ToString();
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDangNhap_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ textbox và trim
            string tenDangNhap = txtTenDangNhap.Text.Trim();
            string matKhau = txtMatKhau.Text.Trim();

            // Kiểm tra nhập liệu cơ bản
            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Băm mật khẩu nhập vào
            string matKhauDaHash = MaHoaSHA256(matKhau);

            // Truy vấn LINQ to SQL
            try
            {
                // Dùng FirstOrDefault để tìm người dùng
                // So sánh TAIKHOAN và MATKHAU (đã hash)
                var user = db.NguoiDungs.FirstOrDefault(nd =>
                               nd.TaiKhoan == tenDangNhap &&
                               nd.MatKhau == matKhauDaHash
                           );

                // Xử lý kết quả
                if (user != null)
                {
                    // ĐĂNG NHẬP THÀNH CÔNG
                    MessageBox.Show($"Đăng nhập thành công! Chào {user.ViTri}: {user.HoTen}", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // --- Đây là phần nâng cao: Lưu thông tin người dùng lại ---
                    Session.Login(user);
                    // --------------------------------------------------------

                    MenuQuanLy frm = new MenuQuanLy();
                    frm.ShowDialog();
                    this.Close();
                }
                else
                {
                    // ĐĂNG NHẬP THẤT BẠI
                    MessageBox.Show("Sai tên đăng nhập hoặc mật khẩu.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi kết nối CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
