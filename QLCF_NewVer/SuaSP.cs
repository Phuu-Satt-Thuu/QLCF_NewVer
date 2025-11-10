using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCF_NewVer
{
    public partial class SuaSP : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private string _maSP; // Mã SP cha (Vd: SP001)
        private SanPham _parentSP; // Đối tượng SanPham cha
        public SuaSP(int? idSPKC)
        {
            InitializeComponent();
            if (idSPKC == null)
            {
                MessageBox.Show("Không có sản phẩm nào được chọn.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close(); // Đóng form ngay lập...
                return;
            }

            // A. Tìm MaSP cha từ IdSPKC con
            var spkc = db.SanPhamKichCos.FirstOrDefault(s => s.IdSPKC == idSPKC);
            if (spkc == null)
            {
                MessageBox.Show("Không tìm thấy dữ liệu sản phẩm.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close();
                return;
            }

            _maSP = spkc.MaSP; // Lấy được mã cha (Vd: SP001)

            // B. Lấy đối tượng SanPham cha
            _parentSP = db.SanPhams.FirstOrDefault(sp => sp.MaSP == _maSP);
            if (_parentSP == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm cha.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Load += (s, e) => this.Close();
            }
        }

        private void SuaSP_Load(object sender, EventArgs e)
        {
            cbbLoaiSP.DataSource = db.LoaiSPs.ToList();
            cbbLoaiSP.DisplayMember = "TenLoai";
            cbbLoaiSP.ValueMember = "MaLoai";

            // Tải dữ liệu lên form
            LoadData();
        }
        private void LoadData()
        {
            if (_parentSP == null) return;

            // A. Tải thông tin chung (bảng SanPham)
            txtMaSP.Text = _parentSP.MaSP;
            txtMaSP.Enabled = false; // Không cho sửa Mã SP
            txtTenSP.Text = _parentSP.TenSP;
            cbbLoaiSP.SelectedValue = _parentSP.MaLoai;

            // Tải hình ảnh
            if (!string.IsNullOrEmpty(_parentSP.DuongDanAnh))
            {
                string duongDanAnh = Path.Combine(Application.StartupPath, _parentSP.DuongDanAnh);
                if (File.Exists(duongDanAnh))
                {
                    picAnh.Image = Image.FromFile(duongDanAnh);
                    picAnh.Tag = duongDanAnh; // Lưu đường dẫn GỐC (full path)
                }
            }

            // B. Tải thông tin chi tiết (bảng SanPhamKichCo)
            var allSizes = db.SanPhamKichCos.Where(s => s.MaSP == _maSP).ToList();

            // Kích cỡ S (MaKichCo = 1)
            var spkS = allSizes.FirstOrDefault(s => s.MaKichCo == 1);
            if (spkS != null && spkS.TrangThaiSP == true)
            {
                cbS.Checked = true;
                txtGiaS.Text = spkS.GiaBan.ToString("F0");
                txtGiaS.Enabled = true;
                cbS.Tag = spkS; // Lưu lại đối tượng để tí so sánh
            }
            else
            {
                cbS.Checked = false;
                txtGiaS.Enabled = false;
            }

            // Kích cỡ M (MaKichCo = 2)
            var spkM = allSizes.FirstOrDefault(s => s.MaKichCo == 2);
            if (spkM != null && spkM.TrangThaiSP == true)
            {
                cbM.Checked = true;
                txtGiaM.Text = spkM.GiaBan.ToString("F0");
                txtGiaM.Enabled = true;
                cbM.Tag = spkM;
            }
            else
            {
                cbM.Checked = false;
                txtGiaM.Enabled = false;
            }

            // Kích cỡ L (MaKichCo = 3)
            var spkL = allSizes.FirstOrDefault(s => s.MaKichCo == 3);
            if (spkL != null && spkL.TrangThaiSP == true)
            {
                cbL.Checked = true;
                txtGiaL.Text = spkL.GiaBan.ToString("F0");
                txtGiaL.Enabled = true;
                cbL.Tag = spkL;
            }
            else
            {
                cbL.Checked = false;
                txtGiaL.Enabled = false;
            }
        }

        private void cbS_CheckedChanged(object sender, EventArgs e)
        {
            txtGiaS.Enabled = cbS.Checked;
            if (!cbS.Checked) txtGiaS.Clear();
        }

        private void cbM_CheckedChanged(object sender, EventArgs e)
        {
            txtGiaM.Enabled = cbM.Checked;
            if (!cbM.Checked) txtGiaM.Clear();
        }

        private void cbL_CheckedChanged(object sender, EventArgs e)
        {
            txtGiaL.Enabled = cbL.Checked;
            if (!cbL.Checked) txtGiaL.Clear();
        }

        private void btnThemAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.gif)|*.jpg; *.jpeg; *.png; *.gif";
            if (open.ShowDialog() == DialogResult.OK)
            {
                picAnh.Image = Image.FromFile(open.FileName);
                picAnh.Tag = open.FileName; // Lưu đường dẫn mới
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSuaSP_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtTenSP.Text) || cbbLoaiSP.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập Tên SP và chọn Loại SP.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!cbS.Checked && !cbM.Checked && !cbL.Checked)
            {
                MessageBox.Show("Phải chọn ít nhất một kích cỡ (S, M, hoặc L).", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Kiểm tra giá tiền
            decimal giaS = 0, giaM = 0, giaL = 0;
            if (cbS.Checked && (!decimal.TryParse(txtGiaS.Text, out giaS) || giaS <= 0))
            {
                MessageBox.Show("Giá tiền Size S không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbM.Checked && (!decimal.TryParse(txtGiaM.Text, out giaM) || giaM <= 0))
            {
                MessageBox.Show("Giá tiền Size M không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cbL.Checked && (!decimal.TryParse(txtGiaL.Text, out giaL) || giaL <= 0))
            {
                MessageBox.Show("Giá tiền Size L không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: TIẾN HÀNH CẬP NHẬT CSDL ---
            try
            {
                // A. Cập nhật bảng SanPham (Bảng cha)
                _parentSP.TenSP = txtTenSP.Text.Trim();
                _parentSP.MaLoai = (int)cbbLoaiSP.SelectedValue;

                // Xử lý cập nhật hình ảnh (chỉ khi ảnh thay đổi)
                string duongDanAnhMoi = picAnh.Tag as string;
                string duongDanAnhDB = Path.Combine(Application.StartupPath, _parentSP.DuongDanAnh ?? "");

                if (!string.IsNullOrEmpty(duongDanAnhMoi) && duongDanAnhMoi != duongDanAnhDB)
                {
                    // Copy ảnh mới vào và cập nhật đường dẫn
                    string tenFileMoi = $"{_maSP}_{Guid.NewGuid().ToString().Substring(0, 8)}{Path.GetExtension(duongDanAnhMoi)}";
                    string thuMucLuu = Path.Combine(Application.StartupPath, "Images", "products");
                    Directory.CreateDirectory(thuMucLuu);
                    string duongDanLuuMoi = Path.Combine(thuMucLuu, tenFileMoi);

                    File.Copy(duongDanAnhMoi, duongDanLuuMoi, true);
                    _parentSP.DuongDanAnh = Path.Combine("Images", "products", tenFileMoi);
                }

                // B. Cập nhật bảng SanPhamKichCo (Bảng con)
                // Xử lý từng Size (Thêm / Sửa / Xóa mềm)
                HandleSizeUpdate(1, cbS, giaS);
                HandleSizeUpdate(2, cbM, giaM);
                HandleSizeUpdate(3, cbL, giaL);

                // C. Gửi tất cả thay đổi xuống CSDL
                db.SubmitChanges();

                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi khi lưu vào CSDL: " + ex.Message, "Lỗi CSDL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void HandleSizeUpdate(int maKichCo, CheckBox cb, decimal giaBan)
        {
            // Lấy đối tượng gốc (lúc Load) lưu trong Tag
            var spkc_Goc = cb.Tag as SanPhamKichCo;

            if (cb.Checked) // NGƯỜI DÙNG MUỐN CÓ SIZE NÀY
            {
                if (spkc_Goc != null) // Case 1: Đã có từ trước -> UPDATE
                {
                    spkc_Goc.GiaBan = giaBan;
                    spkc_Goc.TrangThaiSP = true; // (Phòng trường hợp kích hoạt lại size đã xóa mềm)
                }
                else // Case 2: Size mới -> INSERT
                {
                    SanPhamKichCo spkcMoi = new SanPhamKichCo
                    {
                        MaSP = _maSP,
                        MaKichCo = maKichCo,
                        GiaBan = giaBan,
                        SoLuongTon = 0,
                        CanhBaoTonKho = 10,
                        TrangThaiSP = true
                    };
                    db.SanPhamKichCos.InsertOnSubmit(spkcMoi);
                }
            }
            else // NGƯỜI DÙNG KHÔNG MUỐN CÓ SIZE NÀY
            {
                if (spkc_Goc != null) // Case 3: Đã có và giờ muốn xóa -> SOFT DELETE
                {
                    spkc_Goc.TrangThaiSP = false; // Ngừng bán
                }
                // Case 4: Đã không có và giờ cũng không check -> Không làm gì cả
            }
        }
    }
}
