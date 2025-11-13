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
    public partial class ThemMaGG : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        DataGridViewCheckBoxColumn chkChon;
        public ThemMaGG()
        {
            InitializeComponent();
        }

        private void ThemMaGG_Load(object sender, EventArgs e)
        {
            cbbLoaiMaGG.DataSource = db.KieuVCs.ToList();
            cbbLoaiMaGG.DisplayMember = "TenLoai";
            cbbLoaiMaGG.ValueMember = "MaLoaiVC";
            cbbLoaiMaGG.SelectedIndex = -1; // Để trống lúc đầu

            // Tải danh sách Sản phẩm vào DataGridView (cho BOGO)
            LoadSanPhamDataGrid();

            // Đặt ngày mặc định
            dtpNgayBatDau.Value = DateTime.Today;
            dtpNgayKetThuc.Value = DateTime.Today.AddMonths(1);

            // Cập nhật giao diện lần đầu
            CapNhatGiaoDienTheoLoai();
            this.dgvSanPham.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSanPham_CellContentClick);
        }
        private void LoadSanPhamDataGrid()
        {
            // Tạo cột CheckBox nếu chưa có
            if (dgvSanPham.Columns["chkChon"] == null)
            {
                chkChon = new DataGridViewCheckBoxColumn();
                chkChon.Name = "chkChon";
                chkChon.HeaderText = "Chọn";
                chkChon.Width = 50;
                dgvSanPham.Columns.Add(chkChon);
            }

            // Tải danh sách sản phẩm (chỉ những sản phẩm đang bán)
            var query = db.SanPhamKichCos
        .Where(spkc => spkc.TrangThaiSP == true) // Lọc trước
        .Select(spkc => new // Chọn (Select)
        {
            spkc.IdSPKC,
            TenSP = spkc.SanPham.TenSP,     // <-- Lấy từ bảng SanPhams qua Navigation
            KichCo = spkc.KichCo.KichCo1, // <-- Lấy từ bảng KichCos qua Navigation
            spkc.GiaBan
        });

            dgvSanPham.DataSource = query.ToList();

            // Ẩn cột ID
            dgvSanPham.Columns["IdSPKC"].Visible = false;
            dgvSanPham.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán";
        }

        private void cbbLoaiMaGG_SelectedIndexChanged(object sender, EventArgs e)
        {
            CapNhatGiaoDienTheoLoai();
        }
        private void CapNhatGiaoDienTheoLoai()
        {
            if (cbbLoaiMaGG.SelectedItem == null)
            {
                // (Code vô hiệu hóa cũ của bạn)
                txtGiaTriGiam.Enabled = false;
                numGiamPhanTram.Enabled = false;
                dgvSanPham.Enabled = false;
                btnThemMaGG.Enabled = false;
                btnThemMaGGMua1Tang1.Enabled = false;
                return;
            }

            KieuVC loaiVoucherDaChon = (KieuVC)cbbLoaiMaGG.SelectedItem;
            int maLoaiVC = loaiVoucherDaChon.MaLoaiVC;

            if (maLoaiVC == 1) // Giảm %
            {
                txtGiaTriGiam.Enabled = false; txtGiaTriGiam.Clear();
                numGiamPhanTram.Enabled = true;
                dgvSanPham.Enabled = false;
                btnThemMaGG.Enabled = true;
                btnThemMaGGMua1Tang1.Enabled = false;
            }
            else if (maLoaiVC == 3) // Giảm giá trị thực
            {
                txtGiaTriGiam.Enabled = true;
                numGiamPhanTram.Enabled = false; numGiamPhanTram.Value = 0;
                dgvSanPham.Enabled = false;
                btnThemMaGG.Enabled = true;
                btnThemMaGGMua1Tang1.Enabled = false;
            }
            else if (maLoaiVC == 2) // BOGO CÙNG LOẠI (Yêu cầu 1)
            {
                txtGiaTriGiam.Enabled = false; txtGiaTriGiam.Clear();
                numGiamPhanTram.Enabled = false; numGiamPhanTram.Value = 0;
                dgvSanPham.Enabled = true;
                btnThemMaGG.Enabled = false;
                btnThemMaGGMua1Tang1.Enabled = true;

                // === LOGIC MỚI: TỰ ĐỘNG CHECK TẤT CẢ VÀ KHÓA ===
                dgvSanPham.Columns["chkChon"].ReadOnly = true; // Khóa cột CheckBox
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (row.Cells["chkChon"] != null)
                    {
                        row.Cells["chkChon"].Value = true;
                    }
                }
            }
            else if (maLoaiVC == 4) // BOGO KHÁC LOẠI (Yêu cầu 2)
            {
                txtGiaTriGiam.Enabled = false; txtGiaTriGiam.Clear();
                numGiamPhanTram.Enabled = false; numGiamPhanTram.Value = 0;
                dgvSanPham.Enabled = true;
                btnThemMaGG.Enabled = false;
                btnThemMaGGMua1Tang1.Enabled = true;

                // === LOGIC MỚI: MỞ KHÓA VÀ BỎ CHECK TẤT CẢ ===
                dgvSanPham.Columns["chkChon"].ReadOnly = false; // Mở khóa cột CheckBox
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (row.Cells["chkChon"] != null)
                    {
                        row.Cells["chkChon"].Value = false;
                    }
                }
            }
        }

        private void btnThemMaGG_Click(object sender, EventArgs e)
        {
            if (!ValidateInputChung()) return; // Kiểm tra chung

            try
            {
                int maLoaiVC = (int)cbbLoaiMaGG.SelectedValue;

                Voucher v = new Voucher();
                v.Code = txtMaGG.Text.Trim();
                v.TenMaGiamGia = txtTenMaGG.Text.Trim();
                v.NgayBD = dtpNgayBatDau.Value;
                v.NgayKT = dtpNgayKetThuc.Value;
                v.MaLoaiVC = maLoaiVC;

                // Điều kiện (giá trị đơn tối thiểu)
                v.DieuKien = decimal.TryParse(txtGiaTriDonToiThieu.Text, out decimal dk) ? dk : 0;

                // Giá trị voucher
                if (maLoaiVC == 1) // Giảm %
                {
                    v.GiaTri = numGiamPhanTram.Value;
                }
                else if (maLoaiVC == 3) // Giảm tiền
                {
                    v.GiaTri = decimal.TryParse(txtGiaTriGiam.Text, out decimal gt) ? gt : 0;
                }

                db.Vouchers.InsertOnSubmit(v);
                db.SubmitChanges();

                MessageBox.Show("Thêm mã giảm giá thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnThemMaGGMua1Tang1_Click(object sender, EventArgs e)
        {
            if (!ValidateInputChung()) return; // Kiểm tra chung

            // Lấy danh sách sản phẩm được tặng
            List<int> dsSPTang = new List<int>();
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (row.Cells["chkChon"] != null && Convert.ToBoolean(row.Cells["chkChon"].Value) == true)
                {
                    int idSPKC = (int)row.Cells["IdSPKC"].Value;
                    dsSPTang.Add(idSPKC);
                }
            }

            if (dsSPTang.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm (check vào ô) để áp dụng cho mã Mua 1 Tặng 1.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // === LOGIC MỚI: KIỂM TRA LẦN CUỐI ===
            int maLoaiVC = (int)cbbLoaiMaGG.SelectedValue;

            // Nếu là "Khác loại" (Loại 4) và user (bằng cách nào đó) chọn nhiều hơn 1
            if (maLoaiVC == 4 && dsSPTang.Count > 1)
            {
                MessageBox.Show("Với loại 'Mua 1 Tặng 1 Khác Loại', bạn chỉ được chọn DUY NHẤT 1 sản phẩm để tặng.", "Lỗi Logic", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // (Nếu là Loại 2 (Cùng loại), dsSPTang.Count > 1 là đúng)
            // ======================================

            try
            {
                // (Code lưu CSDL của bạn (A, B, C) giữ nguyên)

                // A. Lưu bảng Voucher (bảng cha)
                Voucher v = new Voucher();
                v.Code = txtMaGG.Text.Trim();
                v.TenMaGiamGia = txtTenMaGG.Text.Trim();
                v.NgayBD = dtpNgayBatDau.Value;
                v.NgayKT = dtpNgayKetThuc.Value;
                v.MaLoaiVC = (int)cbbLoaiMaGG.SelectedValue;
                v.DieuKien = decimal.TryParse(txtGiaTriDonToiThieu.Text, out decimal dk) ? dk : 0;
                v.GiaTri = 0;

                db.Vouchers.InsertOnSubmit(v);
                db.SubmitChanges();

                // B. Lấy MaVC vừa tạo
                int maVCVuaTao = v.MaVC;

                // C. Lưu bảng ChiTietVC (bảng con)
                List<ChiTietVC> dsChiTiet = new List<ChiTietVC>();
                foreach (int idSPKC in dsSPTang)
                {
                    dsChiTiet.Add(new ChiTietVC
                    {
                        MaVC = maVCVuaTao,
                        IdSPKC = idSPKC
                    });
                }

                db.ChiTietVCs.InsertAllOnSubmit(dsChiTiet);
                db.SubmitChanges();

                MessageBox.Show("Thêm mã Mua 1 Tặng 1 thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private bool ValidateInputChung()
        {
            if (string.IsNullOrEmpty(txtMaGG.Text) || string.IsNullOrEmpty(txtTenMaGG.Text) || cbbLoaiMaGG.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập Mã GG, Tên GG và chọn Loại GG.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (dtpNgayKetThuc.Value < dtpNgayBatDau.Value)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", "Lỗi logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Kiểm tra Mã GG (Code) đã tồn tại chưa
            if (db.Vouchers.Any(v => v.Code == txtMaGG.Text.Trim()))
            {
                MessageBox.Show("Mã giảm giá này đã tồn tại. Vui lòng chọn mã khác.", "Lỗi trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtMaGG.Clear();
            txtTenMaGG.Clear();
            cbbLoaiMaGG.SelectedIndex = -1;
            txtGiaTriGiam.Clear();
            numGiamPhanTram.Value = 0;
            txtGiaTriDonToiThieu.Clear();

            // Bỏ check tất cả
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (row.Cells["chkChon"] != null)
                {
                    row.Cells["chkChon"].Value = false;
                }
            }

            dtpNgayBatDau.Value = DateTime.Today;
            dtpNgayKetThuc.Value = DateTime.Today.AddMonths(1);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvSanPham_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != dgvSanPham.Columns["chkChon"].Index || e.RowIndex < 0)
                return;

            // 2. Kiểm tra xem có phải mode "BOGO Khác Loại" không
            if (cbbLoaiMaGG.SelectedItem == null) return;
            KieuVC loaiVoucherDaChon = (KieuVC)cbbLoaiMaGG.SelectedItem;
            if (loaiVoucherDaChon.MaLoaiVC != 4) // Chỉ chạy logic này cho "Khác loại" (Loại 4)
                return;

            // 3. LOGIC ĐỘC QUYỀN CHỌN 1:

            // Bỏ check tất cả các hàng
            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                if (row.Index != e.RowIndex) // Bỏ check tất cả, TRỪ hàng vừa click
                {
                    row.Cells["chkChon"].Value = false;
                }
            }

            // Commit (lưu) ngay lập tức thay đổi của ô vừa click
            dgvSanPham.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }
    }
}
