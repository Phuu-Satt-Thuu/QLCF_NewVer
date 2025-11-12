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
    public partial class NhapMatKhau : Form
    {
        public NhapMatKhau()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
        }

        // Public properties to read the entered passwords
        public string NewPassword => txtNewPass.Text.Trim();
        public string ConfirmPassword => txtConfirm.Text.Trim();

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NewPassword))
            {
                MessageBox.Show("Vui lòng nhập mật khẩu mới.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None; // keep dialog open
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                MessageBox.Show("Mật khẩu xác nhận không khớp.", "Thông báo",
                                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            // If OK, form closes automatically with DialogResult.OK
        }
    }
}
