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
    public partial class QuanLySanPham : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

        // Biến lưu trữ MaND đang được chọn trong grid
        private int? _selectedIdSPKC = null;
        public QuanLySanPham()
        {
            InitializeComponent();
        }

        private void QuanLySanPham_Load(object sender, EventArgs e)
        {
            LoadComboBoxLoc();
            LoadData();
        }
        private void LoadComboBoxLoc()
        {
            // Tải danh sách Loại sản phẩm (Cà phê, Trà, Bánh...)
            var danhSachLoai = db.LoaiSPs.ToList();

            // Tạo một đối tượng "Tất cả"
            LoaiSP tatCa = new LoaiSP { MaLoai = 0, TenLoai = "Tất cả" };
            danhSachLoai.Insert(0, tatCa);

            // Gán vào ComboBox
            cbbLocDuLieu.DataSource = danhSachLoai;
            cbbLocDuLieu.DisplayMember = "TenLoai"; // Hiển thị TenLoai
            cbbLocDuLieu.ValueMember = "MaLoai";   // Lấy giá trị là MaLoai
        }

        // 4. HÀM TẢI DỮ LIỆU LÊN DATAGRIDVIEW
        private void LoadData()
        {
            var query = db.SanPhamKichCos.Select(spkc => new
            {
                spkc.IdSPKC,
                spkc.MaSP,
                spkc.SanPham.TenSP,                  // Lấy TenSP từ bảng SanPham
                KichCo1 = spkc.KichCo.KichCo1,       // Lấy KichCo1 từ bảng KichCo
                spkc.GiaBan,
                spkc.SoLuongTon,
                TenLoai = spkc.SanPham.LoaiSP.TenLoai, // Lấy TenLoai từ bảng LoaiSP
                spkc.TrangThaiSP
            });

            dgvSanPham.DataSource = query.ToList();

            // Đặt lại tên cột
            dgvSanPham.Columns["IdSPKC"].Visible = false; // Ẩn cột ID
            dgvSanPham.Columns["MaSP"].HeaderText = "Mã SP";
            dgvSanPham.Columns["TenSP"].HeaderText = "Tên sản phẩm";
            dgvSanPham.Columns["KichCo1"].HeaderText = "Size";
            dgvSanPham.Columns["GiaBan"].HeaderText = "Giá bán";
            dgvSanPham.Columns["SoLuongTon"].HeaderText = "Tồn kho";
            dgvSanPham.Columns["TenLoai"].HeaderText = "Loại SP";
            dgvSanPham.Columns["TrangThaiSP"].HeaderText = "Trạng thái";

            dgvSanPham.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            _selectedIdSPKC = null; // Reset lựa chọn
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string searchTerm = txtTimKiem.Text.Trim().ToLower();
            int maLoaiDuocChon = (int)cbbLocDuLieu.SelectedValue;

            // Bắt đầu với query cơ bản (không lọc trạng thái)
            var query = db.SanPhamKichCos.AsQueryable();

            // 1. Lọc theo Loại (ComboBox)
            if (maLoaiDuocChon != 0) // 0 là "Tất cả"
            {
                query = query.Where(spkc => spkc.SanPham.MaLoai == maLoaiDuocChon);
            }

            // 2. Lọc theo Từ khóa (TextBox)
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(spkc =>
                    spkc.SanPham.TenSP.ToLower().Contains(searchTerm) ||
                    spkc.MaSP.ToLower().Contains(searchTerm)
                );
            }

            // Chạy query cuối cùng để hiển thị (Join 4 bảng)
            var result = query.Select(spkc => new
            {
                spkc.IdSPKC,
                spkc.MaSP,
                spkc.SanPham.TenSP,
                KichCo1 = spkc.KichCo.KichCo1,
                spkc.GiaBan,
                spkc.SoLuongTon,
                TenLoai = spkc.SanPham.LoaiSP.TenLoai,
                spkc.TrangThaiSP
            });

            dgvSanPham.DataSource = result.ToList();
        }

        private void dgvSanPham_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvSanPham.Rows.Count)
            {
                // Lấy IdSPKC từ hàng được chọn
                _selectedIdSPKC = Convert.ToInt32(dgvSanPham.Rows[e.RowIndex].Cells["IdSPKC"].Value);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXoaSP_Click(object sender, EventArgs e)
        {
            if (_selectedIdSPKC == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm (với kích cỡ) để ngừng bán.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Tìm sản phẩm-kích-cỡ trong CSDL
                var spkc = db.SanPhamKichCos.FirstOrDefault(s => s.IdSPKC == _selectedIdSPKC);

                if (spkc == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm trong CSDL.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra nếu đã "ngừng bán" rồi thì thôi
                if (spkc.TrangThaiSP == false)
                {
                    MessageBox.Show("Sản phẩm này đã ở trạng thái 'Ngừng bán'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Xác nhận
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn 'ngừng bán' sản phẩm này không?\n\n" +
                                              $"(Sản phẩm sẽ được cập nhật trạng thái thành 'False')",
                                              "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // ĐÂY LÀ LỆNH "XÓA MỀM"
                    spkc.TrangThaiSP = false; // Đặt TrangThaiSP = 0 (Ngừng bán)

                    db.SubmitChanges(); // Gửi lệnh UPDATE

                    MessageBox.Show("Ngừng bán sản phẩm thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadData(); // Tải lại grid để thấy cột "TrangThaiSP" đổi thành False
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemSP_Click(object sender, EventArgs e)
        {
            ThemSP frm = new ThemSP();
            frm.ShowDialog();
        }

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            SuaSP frm = new SuaSP(_selectedIdSPKC);
            frm.ShowDialog();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();

            // 2. (Tùy chọn) Xóa các bộ lọc
            txtTimKiem.Clear();
            cbbLocDuLieu.SelectedValue = 0; // Giả sử 0 là giá trị của "Tất cả"

            // 3. Reset lựa chọn đang chọn
            _selectedIdSPKC = null;
        }

        private void btnMoBanLai_Click(object sender, EventArgs e)
        {
            if (_selectedIdSPKC == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm (với kích cỡ) để mở bán lại.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 2. Tìm sản phẩm-kích-cỡ trong CSDL
                var spkc = db.SanPhamKichCos.FirstOrDefault(s => s.IdSPKC == _selectedIdSPKC);

                if (spkc == null)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm trong CSDL.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // 3. Kiểm tra xem đã "đang bán" chưa
                if (spkc.TrangThaiSP == true)
                {
                    MessageBox.Show("Sản phẩm này đã ở trạng thái 'Đang bán'.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 4. Xác nhận (chỉ khi sản phẩm đang false)
                var confirm = MessageBox.Show($"Bạn có chắc chắn muốn 'mở bán lại' sản phẩm này không?\n\n" +
                                              $"(Trạng thái sản phẩm sẽ được đổi thành 'True')",
                                              "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    // 5. ĐÂY LÀ LỆNH "MỞ BÁN LẠI"
                    spkc.TrangThaiSP = true; // Đặt TrangThaiSP = 1 (Đang bán)

                    db.SubmitChanges(); // Gửi lệnh UPDATE

                    MessageBox.Show("Mở bán lại sản phẩm thành công.", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tải lại grid để thấy cột "TrangThaiSP" đổi thành True
                    LoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
