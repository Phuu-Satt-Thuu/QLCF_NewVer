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
    public partial class QuanLyTaiKhoan : Form
    {
        public QuanLyTaiKhoan()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;

            Load += QuanLyTaiKhoan_Load;

            // password change UI wiring (existing)
            btnChangePwd.Click += btnChangePwd_Click;

            // search / filter wiring
            btnTimKiem.Click += btnTimKiem_Click;
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) ApplyFilterSearch(); };
            cbbLocDuLieu.SelectedIndexChanged += (s, e) => ApplyFilterSearch();
        }

        private void QuanLyTaiKhoan_Load(object sender, EventArgs e)
        {
            SetupFilterCombo();
            ReloadUsers();
        }

        private void SetupFilterCombo()
        {
            // Populate filter combo with a small set of useful filters.
            // Adjust the strings to match values stored in your DB if different.
            cbbLocDuLieu.Items.Clear();
            cbbLocDuLieu.Items.Add("Tất cả");
            cbbLocDuLieu.Items.Add("Admin");
            cbbLocDuLieu.Items.Add("NhanVien");
            cbbLocDuLieu.Items.Add("Kích hoạt");
            cbbLocDuLieu.Items.Add("Đã khóa");
            cbbLocDuLieu.SelectedIndex = 0;
        }

        private void ReloadUsers()
        {
            // Load all users once (we'll filter in memory for the small dataset typical of admin screens)
            using (var db = new QLCF_NewVerDataContext())
            {
                var data = db.NguoiDungs
                             .Select(x => new
                             {
                                 x.MaND,
                                 x.TaiKhoan,
                                 x.HoTen,
                                 x.ViTri,     // Admin / NhanVien
                                 x.TrangThai
                             })
                             .OrderBy(x => x.ViTri)
                             .ThenBy(x => x.HoTen)
                             .ToList();

                // Cache the full list in the DataSource; then ApplyFilterSearch will rebind a filtered subset.
                dgvUsers.AutoGenerateColumns = true;
                dgvUsers.DataSource = data;
            }
        }

        private void ApplyFilterSearch()
        {
            // Read source (full list) from current DataSource
            var full = (dgvUsers.DataSource as IEnumerable<dynamic>)?.ToList();
            if (full == null)
            {
                // fallback: reload and re-run
                ReloadUsers();
                full = (dgvUsers.DataSource as IEnumerable<dynamic>)?.ToList();
                if (full == null) return;
            }

            // Read UI inputs
            var filter = (cbbLocDuLieu.SelectedItem ?? "Tất cả").ToString();
            var search = (txtTimKiem.Text ?? "").Trim();

            IEnumerable<dynamic> q = full;

            // Apply filter by role or status
            if (!string.Equals(filter, "Tất cả", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(filter, "Admin", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(filter, "NhanVien", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(x => string.Equals((string)x.ViTri, filter, StringComparison.OrdinalIgnoreCase));
                }
                else if (string.Equals(filter, "Kích hoạt", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(x => (x.TrangThai is bool) ? (bool)x.TrangThai == true : false);
                }
                else if (string.Equals(filter, "Đã khóa", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(x => (x.TrangThai is bool) ? (bool)x.TrangThai == false : true);
                }
            }

            // Apply text search (case-insensitive) against TaiKhoan, HoTen or MaND
            if (!string.IsNullOrEmpty(search))
            {
                var low = search.ToLowerInvariant();
                q = q.Where(x =>
                {
                    var tai = (x.TaiKhoan ?? "").ToString();
                    var ho = (x.HoTen ?? "").ToString();
                    var id = (x.MaND ?? "").ToString();
                    return tai.IndexOf(low, StringComparison.OrdinalIgnoreCase) >= 0
                        || ho.IndexOf(low, StringComparison.OrdinalIgnoreCase) >= 0
                        || id.IndexOf(low, StringComparison.OrdinalIgnoreCase) >= 0;
                });
            }

            var result = q.OrderBy(x => x.ViTri).ThenBy(x => x.HoTen).ToList();
            dgvUsers.AutoGenerateColumns = true;
            dgvUsers.DataSource = result;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            ApplyFilterSearch();
        }

        private void btnChangePwd_Click(object sender, EventArgs e)
        {
            // Check if a row is selected
            if (dgvUsers.CurrentRow == null)
            {
                MessageBox.Show("Vui lòng chọn tài khoản cần đổi mật khẩu.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Get the selected user's ID
            string maND = dgvUsers.CurrentRow.Cells["MaND"].Value?.ToString();
            if (string.IsNullOrEmpty(maND))
            {
                MessageBox.Show("Không lấy được mã người dùng.");
                return;
            }

            // 🔹 Open the password dialog
            using (var dlg = new NhapMatKhau())
            {
                var result = dlg.ShowDialog(this);

                if (result == DialogResult.OK)
                {
                    string newPwd = dlg.NewPassword;
                    string confirm = dlg.ConfirmPassword;

                    if (newPwd != confirm)
                    {
                        MessageBox.Show("Mật khẩu xác nhận không khớp!");
                        return;
                    }

                    using (var db = new QLCF_NewVerDataContext())
                    {
                        // Update with LINQ lambda
                        var user = db.NguoiDungs.SingleOrDefault(u => u.MaND == maND);
                        if (user != null)
                        {
                            user.MatKhau = Sha256Hex(newPwd);
                            db.SubmitChanges();
                            MessageBox.Show("Đã đổi mật khẩu thành công!");
                        }
                    }
                }
                else
                {
                    // User pressed Cancel or Esc
                    MessageBox.Show("Đã hủy đổi mật khẩu.");
                }
            }
        }

        private static string Sha256Hex(string s)
        {
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(s));
                var sb = new StringBuilder(hash.Length * 2);
                foreach (var b in hash) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private void btnDoiMatKhau_Click(object sender, EventArgs e)
        {

        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            // reset filter UI and reload
            SetupFilterCombo();
            txtTimKiem.Clear();
            ReloadUsers();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
