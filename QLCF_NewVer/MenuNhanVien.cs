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
    public partial class MenuNhanVien : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private string _maND_DaDangNhap = "NV01";

        // Biến để lưu ID của ca làm việc HIỆN TẠI
        private int? _currentChamCongId = null;
        public MenuNhanVien()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void MenuNhanVien_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // 1000ms = 1 giây
            timer1.Start(); // Bắt đầu chạy Timer

            // 2. Cập nhật thời gian lần đầu tiên
            // (Giả sử bạn đã kéo 1 Label tên là lblThoiGian vào góc)
            timer1_Tick(sender, e);
            KiemTraTrangThaiChamCong();
        }
        private void KiemTraTrangThaiChamCong()
        {
            DateTime homNay = DateTime.Now.Date;

            // LINQ Lambda: Tìm ca làm việc hôm nay mà CHƯA CÓ Giờ kết thúc
            var caLamDangDo = db.ChamCongs
                .Where(cc => cc.MaND == _maND_DaDangNhap &&
                             cc.Ngay == homNay &&
                             cc.GioKetThuc == null) // Quan trọng
                .FirstOrDefault();

            if (caLamDangDo != null)
            {
                // Đã "Bắt đầu"
                _currentChamCongId = caLamDangDo.Id; // Lưu lại ID
                btnBatDau.Enabled = false; // Khóa nút Bắt đầu
                btnKetThuc.Enabled = true;  // Mở nút Kết thúc
            }
            else
            {
                // Chưa "Bắt đầu"
                _currentChamCongId = null;
                btnBatDau.Enabled = true;   // Mở nút Bắt đầu
                btnKetThuc.Enabled = false; // Khóa nút Kết thúc
            }
        }

        private void đăngXuấtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có chắc chắn muốn đăng xuất?", "Xác nhận đăng xuất", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Nếu chọn Yes thì thoát hẳn chương trình
                this.Close();
            }
            else
            {

            }
        }

        private void đổiMậtKhẩuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoiMatKhau frm =new DoiMatKhau();
            frm.ShowDialog();
        }

        private void thốngKêDoanhThuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ThongKeDoanhThu frm = new ThongKeDoanhThu();
            frm.ShowDialog();
        }

        private void lịchSửHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LichSuHoaDon frm = new LichSuHoaDon();
            frm.ShowDialog();
        }

        private void bánHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BanHang frm = new BanHang();
            frm.ShowDialog();
        }

        private void quảnLýKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyKhachHang frm = new QuanLyKhachHang();
            frm.ShowDialog();
        }

        private void quảnLýMãGiảmGiáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyMaGiamGia frm = new QuanLyMaGiamGia();
            frm.ShowDialog();
        }

        private void btnBatDau_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Tạo 1 dòng chấm công mới
                ChamCong phieuChamCong = new ChamCong
                {
                    MaND = _maND_DaDangNhap,
                    Ngay = DateTime.Now.Date,
                    GioBatDau = DateTime.Now,
                    GioKetThuc = null // Để trống
                };

                // 2. Lưu vào CSDL
                db.ChamCongs.InsertOnSubmit(phieuChamCong);
                db.SubmitChanges();

                // 3. Lấy ID vừa tạo
                _currentChamCongId = phieuChamCong.Id;

                // 4. Cập nhật UI
                btnBatDau.Enabled = false;
                btnKetThuc.Enabled = true;
                MessageBox.Show($"Bắt đầu ca làm việc lúc: {phieuChamCong.GioBatDau:HH:mm:ss}");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi bắt đầu ca: " + ex.Message);
            }
        }

        private void btnKetThuc_Click(object sender, EventArgs e)
        {
            if (_currentChamCongId == null)
            {
                MessageBox.Show("Lỗi: Không tìm thấy ca làm việc để kết thúc.");
                KiemTraTrangThaiChamCong(); // Tải lại trạng thái
                return;
            }

            try
            {
                // 1. LINQ: Tìm đúng ca làm việc đang dở
                var caLamCanKetThuc = db.ChamCongs
                    .FirstOrDefault(cc => cc.Id == _currentChamCongId.Value);

                if (caLamCanKetThuc != null)
                {
                    // 2. Cập nhật giờ kết thúc
                    caLamCanKetThuc.GioKetThuc = DateTime.Now;

                    // 3. Lưu CSDL
                    db.SubmitChanges();

                    // 4. Cập nhật UI
                    _currentChamCongId = null; // Reset
                    btnBatDau.Enabled = true;
                    btnKetThuc.Enabled = false;
                    MessageBox.Show($"Kết thúc ca làm việc lúc: {caLamCanKetThuc.GioKetThuc:HH:mm:ss}");
                }
                else
                {
                    MessageBox.Show("Lỗi: Không tìm thấy ID chấm công trong CSDL.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi kết thúc ca: " + ex.Message);
            }
        }
    }
}
