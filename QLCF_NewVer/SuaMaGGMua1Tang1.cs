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
            // (Copy y hệt từ form ThemMaGG)
            if (dgvSanPham.Columns["chkChon"] == null)
            {
                chkChon = new DataGridViewCheckBoxColumn();
                chkChon.Name = "chkChon";
                chkChon.HeaderText = "Chọn";
                chkChon.Width = 50;
                dgvSanPham.Columns.Add(chkChon);
            }

            var query = from spkc in db.SanPhamKichCos
                        join sp in db.SanPhams on spkc.MaSP equals sp.MaSP
                        join size in db.KichCos on spkc.MaKichCo equals size.MaKichCo
                        where spkc.TrangThaiSP == true
                        select new
                        {
                            spkc.IdSPKC,
                            sp.TenSP,
                            KichCo = size.KichCo1, // Sửa thành KichCo1
                            spkc.GiaBan
                        };

            dgvSanPham.DataSource = query.ToList();
            dgvSanPham.Columns["IdSPKC"].Visible = false;
            dgvSanPham.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            dgvSanPham.Columns["GiaBan"].HeaderText = "Giá Bán";
        }

        // 6. HÀM TẢI DỮ LIỆU CŨ LÊN FORM
        private void LoadDataToForm()
        {
            if (_voucherToEdit == null) return;

            // Tải thông tin chung
            txtMaGG.Text = _voucherToEdit.Code;
            txtTenMaGG.Text = _voucherToEdit.TenMaGiamGia;
            cbbLoaiMaGG.SelectedValue = _voucherToEdit.MaLoaiVC;
            dtpNgayBatDau.Value = _voucherToEdit.NgayBD;
            dtpNgayKetThuc.Value = _voucherToEdit.NgayKT;
            txtGiaTriDonToiThieu.Text = _voucherToEdit.DieuKien?.ToString("F0"); // F0 = không lẻ

            // Vô hiệu hóa các ô không được sửa
            txtMaGG.Enabled = false;
            cbbLoaiMaGG.Enabled = false;

            // Tải thông tin chi tiết và cập nhật giao diện
            int maLoaiVC = _voucherToEdit.MaLoaiVC;

            if (maLoaiVC == 1) // Giảm %
            {
                numGiamPhanTram.Value = _voucherToEdit.GiaTri;
                numGiamPhanTram.Enabled = true;
                txtGiaTriGiam.Enabled = false;
                dgvSanPham.Enabled = false;
            }
            else if (maLoaiVC == 3) // Giảm giá trị thực
            {
                txtGiaTriGiam.Text = _voucherToEdit.GiaTri.ToString("F0");
                txtGiaTriGiam.Enabled = true;
                numGiamPhanTram.Enabled = false;
                dgvSanPham.Enabled = false;
            }
            else if (maLoaiVC == 2 || maLoaiVC == 4) // BOGO (Mua 1 Tặng 1)
            {
                numGiamPhanTram.Enabled = false;
                txtGiaTriGiam.Enabled = false;
                dgvSanPham.Enabled = true;

                // QUAN TRỌNG: Tự động check các sản phẩm đã lưu
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
            }
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
    }
}