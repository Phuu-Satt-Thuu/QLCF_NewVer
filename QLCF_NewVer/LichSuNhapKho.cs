using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QLCF_NewVer
{
    public partial class LichSuNhapKho : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private List<dynamic> _ketQuaTimKiem;
        public LichSuNhapKho()
        {
            InitializeComponent();
            _ketQuaTimKiem = new List<dynamic>(); // Khởi tạo rỗng
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;
            int? maNCC = null;

            if (cbbNhaCungCap.SelectedValue != null)
            {
                var selectedItem = cbbNhaCungCap.Items[cbbNhaCungCap.SelectedIndex];
                maNCC = (int)((dynamic)selectedItem).MaNCC;
            }

            if (maNCC == 0) // 0 là key của "Tất cả"
            {
                maNCC = null; // null = không lọc
            }

            LoadLichSuData(tuNgay, denNgay, maNCC);
        }

        private void LichSuNhapKho_Load(object sender, EventArgs e)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<ChiTietNhapKho>(ct => ct.NhapKho);
            dlo.LoadWith<NhapKho>(nk => nk.NhaCungCap);
            dlo.LoadWith<ChiTietNhapKho>(ct => ct.SanPhamKichCo);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.SanPham);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.KichCo);
            db.LoadOptions = dlo;

            // 2. Load ComboBox
            LoadComboBoxNhaCungCap();

            // 3. Cài đặt DataGridView
            SetupDgvLichSuNhapKho();

            // 4. Cài đặt TextBox
            txtTongTienNhapKho.ReadOnly = true;
            txtTongTienNhapKho.Text = "0";

            // 5. Tải dữ liệu lần đầu
            LoadLichSuData(null, null, null);
        }
        private void LoadComboBoxNhaCungCap()
        {
            var dsNCC = db.NhaCungCaps
                          .OrderBy(n => n.TenNCC)
                          .Select(n => new { MaNCC = (int)n.MaNCC, TenNCC = n.TenNCC })
                          .ToList();

            var displayList = new List<object>();
            // Dùng MaNCC = 0 làm key cho "Tất cả"
            displayList.Add(new { MaNCC = 0, TenNCC = "--- Tất cả nhà cung cấp ---" });
            displayList.AddRange(dsNCC);

            // Gán DataSource (Đảo thứ tự để fix lỗi)
            cbbNhaCungCap.DisplayMember = "TenNCC";
            cbbNhaCungCap.ValueMember = "MaNCC";
            cbbNhaCungCap.DataSource = displayList;
        }
        private void SetupDgvLichSuNhapKho()
        {
            dgvLichSuNhapKho.AutoGenerateColumns = false;
            dgvLichSuNhapKho.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLichSuNhapKho.MultiSelect = false;
            dgvLichSuNhapKho.ReadOnly = true;
            dgvLichSuNhapKho.AllowUserToAddRows = false;
            dgvLichSuNhapKho.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvLichSuNhapKho.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayNhap",
                HeaderText = "Ngày Nhập",
                DataPropertyName = "NgayNhap",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm" }
            });
            dgvLichSuNhapKho.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenNCC",
                HeaderText = "Nhà Cung Cấp",
                DataPropertyName = "TenNCC",
                FillWeight = 25
            });
            dgvLichSuNhapKho.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSP",
                HeaderText = "Sản Phẩm",
                DataPropertyName = "TenSP",
                FillWeight = 25
            });
            dgvLichSuNhapKho.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Size",
                HeaderText = "Size",
                DataPropertyName = "Size",
                FillWeight = 8
            });
            dgvLichSuNhapKho.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongNhap",
                HeaderText = "SL Nhập",
                DataPropertyName = "SoLuongNhap",
                FillWeight = 8
            });
            dgvLichSuNhapKho.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GiaNhap",
                HeaderText = "Giá Nhập",
                DataPropertyName = "GiaNhap",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvLichSuNhapKho.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien",
                HeaderText = "Thành Tiền",
                DataPropertyName = "ThanhTien",
                FillWeight = 12,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
        }
        private void LoadLichSuData(DateTime? tuNgay, DateTime? denNgay, int? maNCC)
        {
            try
            {
                // 1. Bắt đầu query
                IQueryable<ChiTietNhapKho> query = db.ChiTietNhapKhos;

                // 2. Lọc theo ngày (nếu có)
                if (tuNgay.HasValue)
                {
                    query = query.Where(ct => ct.NhapKho.NgayNhap >= tuNgay.Value.Date);
                }
                if (denNgay.HasValue)
                {
                    DateTime denNgayCuoiNgay = denNgay.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(ct => ct.NhapKho.NgayNhap <= denNgayCuoiNgay);
                }

                // 3. Lọc theo Nhà cung cấp (nếu có)
                if (maNCC.HasValue && maNCC > 0)
                {
                    query = query.Where(ct => ct.NhapKho.MaNCC == maNCC);
                }

                // 4. Lấy kết quả và chiếu (Select) ra
                // (DataLoadOptions đã lo việc Join)
                var ketQua = query
                    .OrderByDescending(ct => ct.NhapKho.NgayNhap) // Mới nhất lên trên
                    .Select(ct => new
                    {
                        ct.NhapKho.NgayNhap,
                        TenNCC = ct.NhapKho.NhaCungCap.TenNCC,
                        TenSP = ct.SanPhamKichCo.SanPham.TenSP,
                        Size = ct.SanPhamKichCo.KichCo.KichCo1, // Dùng KichCo1
                        ct.SoLuongNhap,
                        ct.GiaNhap,
                        ThanhTien = ct.ThanhTien.Value // Dùng .Value vì là decimal?
                    })
                    .ToList();

                // 5. Gán vào Grid và tính Tổng tiền
                dgvLichSuNhapKho.DataSource = ketQua;
                _ketQuaTimKiem = ketQua.Cast<dynamic>().ToList(); // Lưu lại để xuất PDF

                // 6. Tính tổng tiền
                decimal tongTien = ketQua.Sum(kq => kq.ThanhTien);
                txtTongTienNhapKho.Text = tongTien.ToString("N0") + " VNĐ";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnXuatTonKho_Click(object sender, EventArgs e)
        {
            if (_ketQuaTimKiem == null || !_ketQuaTimKiem.Any())
            {
                MessageBox.Show("Không có dữ liệu để xuất!");
                return;
            }

            // Lấy thông tin lọc
            string tuNgay = dtpTuNgay.Value.ToString("dd/MM/yyyy");
            string denNgay = dtpDenNgay.Value.ToString("dd/MM/yyyy");
            string tenNCC = cbbNhaCungCap.Text;
            string tongTien = txtTongTienNhapKho.Text;

            try
            {
                // 1. CHUẨN BỊ FONT (Rất quan trọng cho Tiếng Việt)
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font fontTieuDe = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontDam = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontThuong = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                // 2. TẠO FILE
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"LichSuNhapKho_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");
                Document document = new Document(PageSize.A4.Rotate(), 40, 40, 40, 40); // Xoay ngang
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // 3. THÊM NỘI DUNG
                Paragraph tieuDe = new Paragraph("LỊCH SỬ NHẬP KHO", fontTieuDe);
                tieuDe.Alignment = Element.ALIGN_CENTER;
                document.Add(tieuDe);
                document.Add(new Paragraph($"Từ ngày: {tuNgay} - Đến ngày: {denNgay}", fontThuong));
                document.Add(new Paragraph($"Nhà cung cấp: {tenNCC}", fontThuong));
                document.Add(Chunk.NEWLINE);

                // 4. Bảng chi tiết
                PdfPTable table = new PdfPTable(7); // 7 cột
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 20f, 25f, 25f, 8f, 8f, 12f, 12f });

                // Header
                table.AddCell(new Phrase("Ngày Nhập", fontDam));
                table.AddCell(new Phrase("Nhà Cung Cấp", fontDam));
                table.AddCell(new Phrase("Sản Phẩm", fontDam));
                table.AddCell(new Phrase("Size", fontDam));
                table.AddCell(new Phrase("SL Nhập", fontDam));
                table.AddCell(new Phrase("Giá Nhập", fontDam));
                table.AddCell(new Phrase("Thành Tiền", fontDam));

                // Thêm các dòng
                foreach (var item in _ketQuaTimKiem)
                {
                    table.AddCell(new Phrase(item.NgayNhap.ToString("dd/MM/yyyy HH:mm"), fontThuong));
                    table.AddCell(new Phrase(item.TenNCC, fontThuong));
                    table.AddCell(new Phrase(item.TenSP, fontThuong));
                    table.AddCell(new Phrase(item.Size.ToString(), fontThuong));
                    table.AddCell(new Phrase(item.SoLuongNhap.ToString(), fontThuong));
                    table.AddCell(new Phrase(item.GiaNhap.ToString("N0"), fontThuong));
                    table.AddCell(new Phrase(item.ThanhTien.ToString("N0"), fontThuong));
                }
                document.Add(table);
                document.Add(Chunk.NEWLINE);

                // 5. Tổng kết
                Paragraph tongKet = new Paragraph($"TỔNG TIỀN NHẬP KHO: {tongTien}", fontDam);
                tongKet.Alignment = Element.ALIGN_RIGHT;
                document.Add(tongKet);

                document.Close();
                writer.Close();

                System.Diagnostics.Process.Start(filePath); // Tự động mở file
            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                {
                    MessageBox.Show($"Lỗi: Không thể ghi file PDF. File có thể đang được mở.\n{ex.Message}", "Lỗi Xuất PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi khi tạo file PDF: " + ex.Message, "Lỗi Xuất PDF", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
