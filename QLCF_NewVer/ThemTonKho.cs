using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCF_NewVer
{
    public partial class ThemTonKho : Form
    {
        QLCF_NewVerDataContext db; // <-- Sửa: Khai báo ở đây
        private int? _selectedIdSPKC = null;

        // THÊM MỚI: Biến để lưu dsSPKC tìm được
        private List<SanPhamKichCo> dsSPKC_DaTim;

        private int? _idSPKCToLoad;

        // ===========================================
        // SỬA LỖI CS7036 (Thêm hàm khởi tạo rỗng)
        // ===========================================
        public ThemTonKho() : this(null)
        {
            // Dùng khi mở form ở chế độ "duyệt" (nhập tay)
        }

        public ThemTonKho(int? idSPKC) // Thêm tham số int? idSPKC
        {
            InitializeComponent();
            _idSPKCToLoad = idSPKC; // Lưu ID được gửi qua

            // SỬA: Thêm 2 dòng khởi tạo
            db = new QLCF_NewVerDataContext();
            dsSPKC_DaTim = new List<SanPhamKichCo>();
        }

        private void btnThemTonKho_Click(object sender, EventArgs e)
        {
            if (cbbNhaCungCap.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn Nhà cung cấp.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_selectedIdSPKC == null) // Kiểm tra biến chung
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm (Mã SP + Size) hợp lệ.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!decimal.TryParse(txtGiaNhap.Text, out decimal giaNhap) || giaNhap <= 0)
            {
                MessageBox.Show("Giá nhập không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtSoLuongNhap.Text, out int soLuongNhap) || soLuongNhap <= 0)
            {
                MessageBox.Show("Số lượng nhập không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // SỬA: Dùng tên control của bạn 'txtCanhBaoNhapKho'
            if (!int.TryParse(txtCanhBaoNhapKho.Text, out int canhBaoMoi) || canhBaoMoi < 0)
            {
                MessageBox.Show("Số lượng cảnh báo nhập không hợp lệ.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // --- BƯỚC 2: LƯU VÀO CSDL ---
            try
            {
                // ===========================================
                // SỬA LỖI: CẬP NHẬT CẢNH BÁO TRƯỚC (Giao dịch 1)
                // ===========================================
                var sanPhamCanCapNhat = db.SanPhamKichCos
                                        .FirstOrDefault(spkc => spkc.IdSPKC == _selectedIdSPKC.Value);

                if (sanPhamCanCapNhat != null)
                {
                    sanPhamCanCapNhat.CanhBaoTonKho = canhBaoMoi;
                }
                else
                {
                    MessageBox.Show("Lỗi: Không tìm thấy sản phẩm để cập nhật.");
                    return;
                }

                // Submit thay đổi Cảnh Báo
                db.SubmitChanges();
                // ===========================================


                // ===========================================
                // BÂY GIỜ MỚI THÊM PHIẾU NHẬP (Giao dịch 2)
                // ===========================================
                NhapKho phieuNhap = new NhapKho();
                phieuNhap.MaNCC = (int)cbbNhaCungCap.SelectedValue;
                phieuNhap.NgayNhap = DateTime.Now;
                db.NhapKhos.InsertOnSubmit(phieuNhap);
                db.SubmitChanges(); // Submit để lấy MaNK

                int maNKvuaTao = phieuNhap.MaNK;

                ChiTietNhapKho chiTiet = new ChiTietNhapKho();
                chiTiet.MaNK = maNKvuaTao;
                chiTiet.IdSPKC = _selectedIdSPKC.Value; // Dùng biến chung
                chiTiet.SoLuongNhap = soLuongNhap;
                chiTiet.GiaNhap = giaNhap;

                db.ChiTietNhapKhos.InsertOnSubmit(chiTiet);

                // Submit lần cuối để INSERT ChiTietNhapKho (sẽ kích hoạt Trigger)
                db.SubmitChanges();
                // ===========================================

                MessageBox.Show("Nhập kho và cập nhật cảnh báo thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu CSDL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ThemTonKho_Load(object sender, EventArgs e)
        {
            // SỬA: Thêm Eager Loading
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.SanPham);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.KichCo);
            db.LoadOptions = dlo;

            // SỬA: Đảo thứ tự gán DataSource
            cbbNhaCungCap.DisplayMember = "TenNCC";
            cbbNhaCungCap.ValueMember = "MaNCC";
            cbbNhaCungCap.DataSource = db.NhaCungCaps.OrderBy(n => n.TenNCC).ToList();

            txtTongTien.Enabled = false;
            txtTenSP.Enabled = false;

            // KIỂM TRA: Form được mở theo cách nào?
            if (_idSPKCToLoad != null)
            {
                // CÁCH 1: Mở từ form QuanLySanPham (Đã chọn sản phẩm)
                LoadDataForSelectedProduct();
            }
            else
            {
                // CÁCH 2: Mở tự do
                txtMaSP.Enabled = true;
                cbbSize.Enabled = true;

                // Gán sự kiện cho việc nhập tay
                txtMaSP.Leave += TxtMaSP_Leave;
                cbbSize.SelectedIndexChanged += cbbSize_SelectedIndexChanged;
            }
            this.txtGiaNhap.TextChanged += new System.EventHandler(this.txtGiaNhap_TextChanged);
            this.txtSoLuongNhap.TextChanged += new System.EventHandler(this.txtSoLuongNhap_TextChanged);
            // SỬA: Gán sự kiện cho txtCanhBaoNhapKho
            this.txtCanhBaoNhapKho.TextChanged += new System.EventHandler(this.txtCanhBaoNhapKho_TextChanged);
        }

        private void LoadDataForSelectedProduct()
        {
            // 1. Lấy thông tin SP từ ID
            var productInfo = db.SanPhamKichCos
                .Where(spkc => spkc.IdSPKC == _idSPKCToLoad)
                .Select(spkc => new {
                    spkc.MaSP,
                    spkc.SanPham.TenSP,
                    spkc.MaKichCo,
                    spkc.CanhBaoTonKho // <-- LẤY DỮ LIỆU MỚI
                })
                .FirstOrDefault();

            if (productInfo != null)
            {
                // 2. Điền thông tin vào các ô
                txtMaSP.Text = productInfo.MaSP;
                txtTenSP.Text = productInfo.TenSP;

                // 3. Tự động điền cảnh báo
                txtCanhBaoNhapKho.Text = productInfo.CanhBaoTonKho.ToString();

                // 4. Tải TẤT CẢ size của SP đó vào ComboBox
                var allSizes = db.SanPhamKichCos
                                .Where(s => s.MaSP == productInfo.MaSP)
                                .Select(s => new {
                                    s.IdSPKC,
                                    TenHienThi = s.KichCo.KichCo1 // Dùng KichCo1
                                }).ToList();

                cbbSize.DisplayMember = "TenHienThi";
                cbbSize.ValueMember = "IdSPKC";
                cbbSize.DataSource = allSizes;

                // 5. Chọn đúng Size đã được gửi qua
                cbbSize.SelectedValue = _idSPKCToLoad;
                _selectedIdSPKC = _idSPKCToLoad; // Cập nhật biến _selectedIdSPKC

                // 6. KHÓA các ô không cho sửa
                txtMaSP.Enabled = false;
                txtTenSP.Enabled = false;
                cbbSize.Enabled = false;
            }
        }

        // ===========================================
        // HÀM MỚI: Tự động "duyệt" khi nhập MaSP
        // ===========================================
        private void TxtMaSP_Leave(object sender, EventArgs e)
        {
            // Chỉ chạy nếu form không bị khóa (mở tự do)
            if (_idSPKCToLoad != null) return;

            string maSP = txtMaSP.Text.Trim();
            if (string.IsNullOrEmpty(maSP))
            {
                txtTenSP.Text = "";
                cbbSize.DataSource = null;
                dsSPKC_DaTim.Clear();
                return;
            }

            // LINQ Lambda: Tìm tất cả các size của MaSP này
            dsSPKC_DaTim = db.SanPhamKichCos
                                .Where(spkc => spkc.MaSP == maSP)
                                .ToList();

            if (dsSPKC_DaTim.Any())
            {
                // 1. Hiển thị Tên Sản Phẩm
                txtTenSP.Text = dsSPKC_DaTim.First().SanPham.TenSP;

                // 2. Load các Size vào ComboBox
                var dsSize = dsSPKC_DaTim
                    .Select(spkc => new {
                        spkc.IdSPKC, // Lưu IdSPKC để dùng khi bấm Thêm
                        TenHienThi = spkc.KichCo.KichCo1 // Dùng KichCo1
                    })
                    .ToList();

                cbbSize.DisplayMember = "TenHienThi";
                cbbSize.ValueMember = "IdSPKC";
                cbbSize.DataSource = dsSize;

                // 3. Tự động chọn size đầu tiên
                if (cbbSize.Items.Count > 0)
                {
                    cbbSize.SelectedIndex = 0;
                    // Kích hoạt sự kiện change để load CanhBaoTonKho
                    cbbSize_SelectedIndexChanged(null, null);
                }
            }
            else
            {
                MessageBox.Show("Mã sản phẩm không tồn tại!");
                txtTenSP.Text = "";
                cbbSize.DataSource = null;
                dsSPKC_DaTim.Clear();
            }
        }

        private void cbbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbSize.SelectedValue == null)
            {
                _selectedIdSPKC = null;
                return;
            }

            // SỬA: Cần TryParse vì .NET WinForms
            int selectedId = 0;
            bool parsed = int.TryParse(cbbSize.SelectedValue.ToString(), out selectedId);

            if (parsed)
            {
                _selectedIdSPKC = selectedId;

                // SỬA: Tự động điền CanhBaoTonKho khi chọn Size
                // (Chỉ chạy khi _idSPKCToLoad LÀ NULL - tức là đang ở chế độ duyệt)
                if (_idSPKCToLoad == null && dsSPKC_DaTim.Any())
                {
                    var selectedSPKC = dsSPKC_DaTim.FirstOrDefault(s => s.IdSPKC == selectedId);
                    if (selectedSPKC != null)
                    {
                        txtCanhBaoNhapKho.Text = selectedSPKC.CanhBaoTonKho.ToString();
                    }
                }
            }
            else
            {
                _selectedIdSPKC = null;
            }
        }

        private void TinhTongTien()
        {
            // Thử chuyển đổi
            bool giaHopLe = decimal.TryParse(txtGiaNhap.Text, out decimal gia);
            bool soLuongHopLe = int.TryParse(txtSoLuongNhap.Text, out int soLuong);

            if (giaHopLe && soLuongHopLe)
            {
                // Nếu cả 2 hợp lệ, nhân và hiển thị
                txtTongTien.Text = (gia * soLuong).ToString("N0"); // SỬA: Dùng N0
            }
            else
            {
                // Nếu 1 trong 2 không phải là số, xóa ô tổng tiền
                txtTongTien.Text = "0"; // SỬA: Hiển thị 0
            }
        }


        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtGiaNhap_TextChanged(object sender, EventArgs e)
        {
            TinhTongTien();
        }

        private void txtSoLuongNhap_TextChanged(object sender, EventArgs e)
        {
            TinhTongTien();
        }

        private void txtCanhBaoNhapKho_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
