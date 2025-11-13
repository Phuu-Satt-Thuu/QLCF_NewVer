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
    public partial class SuaMaGGMua1Tang1 : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private int _maVC; // ID của voucher đang sửa
        private Voucher _voucherToEdit; // Đối tượng voucher
        private List<int> _originalCheckedSPKC = new List<int>();

        DataGridViewCheckBoxColumn chkChon;
        public SuaMaGGMua1Tang1(int? maVC)
        {
            InitializeComponent();
            if (maVC == null)
            {
                MessageBox.Show("Không có mã giảm giá nào được chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close();
                return;
            }

            _maVC = maVC.Value;

            // Tìm voucher trong CSDL
            _voucherToEdit = db.Vouchers.FirstOrDefault(v => v.MaVC == _maVC);
            if (_voucherToEdit == null || (_voucherToEdit.MaLoaiVC != 2 && _voucherToEdit.MaLoaiVC != 4))
            {
                MessageBox.Show("Không tìm thấy mã BOGO hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close();
            }
        }

        private void SuaMaGGMua1Tang1_Load(object sender, EventArgs e)
        {
            LoadComboBoxes();
            LoadDataGridView();
            LoadDataToForm();

            this.dgvSanPham.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvSanPham_CellContentClick);
        }
        private void LoadComboBoxes()
        {
            cbbLoaiMaGG.DataSource = db.KieuVCs.ToList();
            cbbLoaiMaGG.DisplayMember = "TenLoai";
            cbbLoaiMaGG.ValueMember = "MaLoaiVC";
        }

        // 5. HÀM TẢI DATAGRIDVIEW (Sản phẩm)
        private void LoadDataGridView()
        {
            if (dgvSanPham.Columns["chkChon"] == null)
            {
                chkChon = new DataGridViewCheckBoxColumn();
                chkChon.Name = "chkChon";
                chkChon.HeaderText = "Chọn";
                chkChon.Width = 50;
                dgvSanPham.Columns.Add(chkChon);
            }

            // === SỬA LẠI QUERY ĐỂ DÙNG JOIN (AN TOÀN HƠN) ===
            var query = from spkc in db.SanPhamKichCos
                        join sp in db.SanPhams on spkc.MaSP equals sp.MaSP
                        join size in db.KichCos on spkc.MaKichCo equals size.MaKichCo
                        where spkc.TrangThaiSP == true
                        select new
                        {
                            spkc.IdSPKC,
                            sp.TenSP,
                            KichCo = size.KichCo1, // Dùng KichCo1
                            spkc.GiaBan
                        };
            // =============================================

            dgvSanPham.DataSource = query.ToList();
            dgvSanPham.Columns["IdSPKC"].Visible = false;
            dgvSanPham.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán";
        }

        // 6. HÀM TẢI DỮ LIỆU CŨ LÊN FORM
        private void LoadDataToForm()
        {
            if (_voucherToEdit == null) return;

            // (Code cũ của bạn: Tải thông tin chung)
            txtMaGG.Text = _voucherToEdit.Code;
            txtTenMaGG.Text = _voucherToEdit.TenMaGiamGia;
            cbbLoaiMaGG.SelectedValue = _voucherToEdit.MaLoaiVC;
            dtpNgayBatDau.Value = _voucherToEdit.NgayBD;
            dtpNgayKetThuc.Value = _voucherToEdit.NgayKT;
            txtGiaTriDonToiThieu.Text = _voucherToEdit.DieuKien?.ToString("F0");
            txtMaGG.Enabled = false;
            cbbLoaiMaGG.Enabled = false; // (Bạn đã khóa, tốt)

            int maLoaiVC = _voucherToEdit.MaLoaiVC;

            // (Code cũ của bạn: Vô hiệu hóa các ô không liên quan)
            numGiamPhanTram.Enabled = false;
            txtGiaTriGiam.Enabled = false;
            dgvSanPham.Enabled = true;

            // (Code cũ của bạn: Tự động check các sản phẩm đã lưu)
            _originalCheckedSPKC = db.ChiTietVCs
                                     .Where(ct => ct.MaVC == _maVC)
                                     .Select(ct => ct.IdSPKC)
                                     .ToList();

            foreach (DataGridViewRow row in dgvSanPham.Rows)
            {
                int idSPKC = (int)row.Cells["IdSPKC"].Value;
                if (_originalCheckedSPKC.Contains(idSPKC))
                {
                    row.Cells["chkChon"].Value = true;
                }
            }

            // === LOGIC MỚI: KHÓA/MỞ CỘT CHECKBOX ===
            if (maLoaiVC == 2) // Cùng loại
            {
                // Khóa cột check, người dùng không được sửa
                dgvSanPham.Columns["chkChon"].ReadOnly = true;
            }
            else if (maLoaiVC == 4) // Khác loại
            {
                // Mở khóa, cho phép chọn 1
                dgvSanPham.Columns["chkChon"].ReadOnly = false;
            }
            // ======================================
        }

        private void btnSuaMaGG_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenMaGG.Text) || cbbLoaiMaGG.SelectedValue == null)
            {
                MessageBox.Show("Tên mã giảm giá và Loại không được để trống.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (dtpNgayKetThuc.Value < dtpNgayBatDau.Value)
            {
                MessageBox.Show("Ngày kết thúc không được nhỏ hơn ngày bắt đầu.", "Lỗi logic", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // --- BƯỚC 2: CẬP NHẬT BẢNG VOUCHER (CHA) ---
                _voucherToEdit.TenMaGiamGia = txtTenMaGG.Text.Trim();
                _voucherToEdit.MaLoaiVC = (int)cbbLoaiMaGG.SelectedValue;
                _voucherToEdit.NgayBD = dtpNgayBatDau.Value;
                _voucherToEdit.NgayKT = dtpNgayKetThuc.Value;
                _voucherToEdit.DieuKien = decimal.TryParse(txtGiaTriDonToiThieu.Text, out decimal dk) ? dk : 0;
                _voucherToEdit.GiaTri = 0; // BOGO luôn có giá trị 0

                // --- BƯỚC 3: CẬP NHẬT BẢNG CHITIETVC (CON) ---
                // Lấy danh sách mới
                List<int> newlyCheckedSPKC = new List<int>();
                foreach (DataGridViewRow row in dgvSanPham.Rows)
                {
                    if (row.Cells["chkChon"] != null && Convert.ToBoolean(row.Cells["chkChon"].Value) == true)
                    {
                        newlyCheckedSPKC.Add((int)row.Cells["IdSPKC"].Value);
                    }
                }

                if (newlyCheckedSPKC.Count == 0)
                {
                    MessageBox.Show("Voucher BOGO phải được áp dụng cho ít nhất 1 sản phẩm.", "Lỗi logic", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                int maLoaiVC = _voucherToEdit.MaLoaiVC;

                // Nếu là "Khác loại" (Loại 4) và user chọn nhiều hơn 1
                if (maLoaiVC == 4 && newlyCheckedSPKC.Count > 1)
                {
                    MessageBox.Show("Với loại 'Mua 1 Tặng 1 Khác Loại', bạn chỉ được chọn DUY NHẤT 1 sản phẩm để tặng.", "Lỗi Logic", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // 1. Tìm những SP cần THÊM MỚI
                List<int> toAdd = newlyCheckedSPKC.Except(_originalCheckedSPKC).ToList();
                foreach (int idSPKC_ToAdd in toAdd)
                {
                    ChiTietVC ctMoi = new ChiTietVC { MaVC = _maVC, IdSPKC = idSPKC_ToAdd };
                    db.ChiTietVCs.InsertOnSubmit(ctMoi);
                }

                // 2. Tìm những SP cần XÓA BỎ
                List<int> toRemove = _originalCheckedSPKC.Except(newlyCheckedSPKC).ToList();
                var chiTietCanXoa = db.ChiTietVCs.Where(ct => ct.MaVC == _maVC && toRemove.Contains(ct.IdSPKC));
                db.ChiTietVCs.DeleteAllOnSubmit(chiTietCanXoa);

                // --- BƯỚC 4: LƯU TẤT CẢ THAY ĐỔI ---
                db.SubmitChanges();

                MessageBox.Show("Cập nhật mã BOGO thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadDataToForm();
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
            if (_voucherToEdit == null || _voucherToEdit.MaLoaiVC != 4) // Chỉ chạy cho "Khác loại" (Loại 4)
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