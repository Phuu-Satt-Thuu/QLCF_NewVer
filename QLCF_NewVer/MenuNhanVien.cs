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
        public MenuNhanVien()
        {
            InitializeComponent();
        }

        private void quảnLýMãGiảmGiáToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QuanLyMaGiamGia frm = new QuanLyMaGiamGia();
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

        private void xemSảnPhẩmBánChạyToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void lịchSửHóaĐơnToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void đổiMậtKhẩuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoiMatKhau frm = new DoiMatKhau();
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
    }
}
