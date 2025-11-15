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
    public partial class MenuQuanLy : Form
    {
        public MenuQuanLy()
        {
            InitializeComponent();
        }

        private void danhSáchNhânViênToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyNhanVien frm = new QuanLyNhanVien();
            frm.ShowDialog();
        }

        private void quảnLýSảnPhẩmToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLySanPham frm = new QuanLySanPham();
            frm.ShowDialog();
        }

        private void quảnLýMãGiảmGiáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyMaGiamGia frm = new QuanLyMaGiamGia();
            frm.ShowDialog();
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

        private void quảnLýKháchHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyKhachHang frm = new QuanLyKhachHang();
            frm.ShowDialog();
        }

        private void quảnLýKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyKho frm = new QuanLyKho();
            frm.ShowDialog();
        }

        private void đổiMậtKhẩuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoiMatKhau frm = new DoiMatKhau();
            frm.ShowDialog();
        }

        private void bánHàngToolStripMenuItem_Click(object sender, EventArgs e)
        {
           BanHang frm = new BanHang();
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

        private void MenuQuanLy_Load(object sender, EventArgs e)
        {
            timer1.Interval = 1000; // 1000ms = 1 giây
            timer1.Start(); // Bắt đầu chạy Timer

            // 2. Cập nhật thời gian lần đầu tiên
            // (Giả sử bạn đã kéo 1 Label tên là lblThoiGian vào góc)
            timer1_Tick(sender, e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lblThoiGian.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private void lịchSửNhậpKhoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LichSuNhapKho frm = new LichSuNhapKho();
            frm.ShowDialog();
        }

        private void lịchSửChấmCôngToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LichSuChamCong frm = new LichSuChamCong();
            frm.ShowDialog();
        }
    }
}
