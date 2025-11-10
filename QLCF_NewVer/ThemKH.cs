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
    public partial class ThemKH : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        public ThemKH()
        {
            InitializeComponent();
        }



        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTenKH.Clear();
            txtSDT.Clear();
            txtDiaChi.Clear();
            txtTichDiem.Clear();
            txtTenKH.Focus(); // Di chuyển con trỏ về ô Tên
        }

        private void btnThemKH_Click(object sender, EventArgs e)
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

            // Kiểm tra SĐT (ví dụ: 10 số, chỉ chứa số)
            if (sdt.Length != 10 || !sdt.All(char.IsDigit))
            {
                MessageBox.Show("Số điện thoại phải là 10 chữ số.", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Kiểm tra Tích điểm
            int tichDiem = 0; // Mặc định là 0
            if (!string.IsNullOrEmpty(tichDiemStr)) // Nếu người dùng có nhập
            {
                if (!int.TryParse(tichDiemStr, out tichDiem) || tichDiem < 0)
                {
                    MessageBox.Show("Tích điểm phải là một con số hợp lệ (lớn hơn hoặc bằng 0).", "Lỗi định dạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            // --- BƯỚC 2: KIỂM TRA TỒN TẠI (UNIQUE) ---
            try
            {
                if (db.KhachHangs.Any(kh => kh.SDT == sdt))
                {
                    MessageBox.Show("Số điện thoại này đã tồn tại trong hệ thống.", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // --- BƯỚC 3: TẠO VÀ LƯU VÀO CSDL ---
                KhachHang khMoi = new KhachHang();
                khMoi.TenKH = tenKH;
                khMoi.SDT = sdt;
                khMoi.DiaChi = diaChi; // diaChi có thể null
                khMoi.TichDiem = tichDiem;
                // khMoi.NgayTao sẽ được CSDL tự động gán

                db.KhachHangs.InsertOnSubmit(khMoi);
                db.SubmitChanges(); // Gửi lệnh INSERT

                MessageBox.Show("Thêm khách hàng mới thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; // Đặt kết quả để form cha (QuanLyKhachHang) biết
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu vào CSDL: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
