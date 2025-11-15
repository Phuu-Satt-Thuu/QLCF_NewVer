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
    public partial class XemChiTietHoaDon : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private int _maHD; // Biến để lưu Mã Hóa Đơn được truyền vào

        // SỬA: Lưu lại Hóa đơn và Tên Voucher để dùng cho PDF
        private HoaDon _hoaDonDuLieu;
        private string _tenVoucherDaApDung;
        public XemChiTietHoaDon(int maHD)
        {
            InitializeComponent();
            _maHD = maHD; // Lưu mã hóa đơn
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void XemChiTietHoaDon_Load(object sender, EventArgs e)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<HoaDon>(h => h.NguoiDung);
            dlo.LoadWith<HoaDon>(h => h.KhachHang);
            dlo.LoadWith<HoaDon>(h => h.ChiTietHDs);
            dlo.LoadWith<HoaDon>(h => h.ApMaVCs); // Tải voucher đã áp dụng
            dlo.LoadWith<ApMaVC>(av => av.Voucher); // Tải thông tin voucher
            dlo.LoadWith<ChiTietHD>(ct => ct.SanPhamKichCo);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.SanPham);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.KichCo);
            db.LoadOptions = dlo;

            // 3.2. Cài đặt cột cho 2 Grid
            SetupDgvChiTiet();
            SetupDgvThongTinChung();

            // 3.3. Tải dữ liệu
            LoadChiTietData();
        }
        private void SetupDgvChiTiet()
        {
            dgvChiTiet.AutoGenerateColumns = false;
            dgvChiTiet.ReadOnly = true;
            dgvChiTiet.AllowUserToAddRows = false;
            dgvChiTiet.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSP",
                HeaderText = "Tên sản phẩm",
                DataPropertyName = "TenSP",
                FillWeight = 40
            });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Size",
                HeaderText = "Size",
                DataPropertyName = "Size",
                FillWeight = 15
            });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong",
                HeaderText = "Số lượng",
                DataPropertyName = "SoLuong",
                FillWeight = 15
            });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonGia",
                HeaderText = "Đơn giá",
                DataPropertyName = "DonGia",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien",
                HeaderText = "Thành tiền",
                DataPropertyName = "ThanhTien",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
        }
        private void SetupDgvThongTinChung()
        {
            dgvThongTinChung.AutoGenerateColumns = false;
            dgvThongTinChung.ReadOnly = true;
            dgvThongTinChung.AllowUserToAddRows = false;
            dgvThongTinChung.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvThongTinChung.ColumnHeadersVisible = false; // Ẩn header
            dgvThongTinChung.RowHeadersVisible = false; // Ẩn cột đầu

            dgvThongTinChung.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThongTin",
                HeaderText = "Thông Tin",
                DataPropertyName = "ThongTin",
                FillWeight = 40,
                DefaultCellStyle = new DataGridViewCellStyle { Font = new System.Drawing.Font(dgvThongTinChung.Font, FontStyle.Bold) }
            });
            dgvThongTinChung.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GiaTri",
                HeaderText = "Giá Trị",
                DataPropertyName = "GiaTri",
                FillWeight = 60
            });
        }
        private void LoadChiTietData()
        {
            // LINQ Lambda: Tìm hóa đơn
            // Dùng biến toàn cục để lưu Hóa đơn
            _hoaDonDuLieu = db.HoaDons.FirstOrDefault(hd => hd.MaHD == _maHD);

            if (_hoaDonDuLieu == null)
            {
                MessageBox.Show("Không tìm thấy hóa đơn!");
                this.Close();
                return;
            }

            // 1. Điền thông tin chung (BÊN PHẢI)
            dgvThongTinChung.Rows.Clear();
            dgvThongTinChung.Rows.Add("Mã Hóa Đơn", _hoaDonDuLieu.MaHD);
            dgvThongTinChung.Rows.Add("Ngày Lập", _hoaDonDuLieu.NgayLap.ToString("dd/MM/yyyy HH:mm:ss"));
            dgvThongTinChung.Rows.Add("Nhân Viên", _hoaDonDuLieu.NguoiDung?.HoTen ?? "[Đã xóa]");
            dgvThongTinChung.Rows.Add("Khách Hàng", _hoaDonDuLieu.KhachHang?.TenKH ?? "Khách vãng lai");
            dgvThongTinChung.Rows.Add("SDT Khách", _hoaDonDuLieu.KhachHang?.SDT ?? "");

            // Tìm tên Voucher (nếu có)
            var voucherDaApDung = _hoaDonDuLieu.ApMaVCs.FirstOrDefault();
            _tenVoucherDaApDung = voucherDaApDung?.Voucher?.Code ?? "Không sử dụng"; // Lưu lại

            dgvThongTinChung.Rows.Add("Mã giảm giá", _tenVoucherDaApDung);
            dgvThongTinChung.Rows.Add("Tạm tính", _hoaDonDuLieu.TongTienGoc.ToString("N0") + " VNĐ");
            dgvThongTinChung.Rows.Add("Giảm giá", _hoaDonDuLieu.TienGiam.ToString("N0") + " VNĐ");

            // Tô đậm dòng TỔNG
            dgvThongTinChung.Rows.Add("TỔNG THANH TOÁN", _hoaDonDuLieu.TongTienSauGiam.Value.ToString("N0") + " VNĐ");
            dgvThongTinChung.Rows[dgvThongTinChung.Rows.Count - 1].DefaultCellStyle.Font =
                new System.Drawing.Font(dgvThongTinChung.Font, FontStyle.Bold);


            // 2. Điền chi tiết sản phẩm (BÊN TRÁI)
            var chiTiet = _hoaDonDuLieu.ChiTietHDs
                .Select(ct => new
                {
                    // Xử lý hàng tặng (hiển thị 0đ và (Hàng tặng))
                    TenSP = ct.IsTang ?
                            $"{ct.SanPhamKichCo.SanPham.TenSP} (Hàng tặng)" :
                            ct.SanPhamKichCo.SanPham.TenSP,

                    Size = ct.SanPhamKichCo.KichCo.KichCo1, // Dùng KichCo1
                    ct.SoLuong,
                    DonGia = ct.IsTang ? 0 : ct.DonGia, // Lấy giá (đã fix 0đ)
                    ThanhTien = ct.ThanhTien.Value // Dùng .Value vì nó là decimal?
                })
                .ToList();

            // 3. Hiển thị lên DataGridView
            dgvChiTiet.DataSource = chiTiet;
        }

        private void btnXuatHoaDon_Click(object sender, EventArgs e)
        {
            PhatSinhFilePDF(_hoaDonDuLieu);
        }
        private void PhatSinhFilePDF(HoaDon hoaDonToPrint)
        {
            try
            {
                if (hoaDonToPrint == null)
                {
                    MessageBox.Show("Không có dữ liệu hóa đơn để in!");
                    return;
                }

                // Lấy thông tin từ đối tượng (đã được Eager Load)
                string tenNhanVien = hoaDonToPrint.NguoiDung?.HoTen ?? "[Đã xóa]";
                string tenKhachHang = hoaDonToPrint.KhachHang?.TenKH ?? "Khách vãng lai";
                string loaiKhachHang = ""; // Ta không có TextBox ở đây
                if (hoaDonToPrint.KhachHang != null)
                {
                    // (Bạn có thể thêm hàm GetHangKhachHang(diem) vào đây nếu muốn)
                    loaiKhachHang = "Thành viên";
                }
                else
                {
                    loaiKhachHang = "Vãng lai";
                }

                string maGiamGia = _tenVoucherDaApDung; // Lấy từ biến toàn cục

                int tongSoLuong = hoaDonToPrint.ChiTietHDs.Sum(item => item.SoLuong);

                // CHUẨN BỊ FONT
                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font fontTieuDe = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontDam = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontThuong = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                // TẠO FILE
                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"ChiTietHD_{hoaDonToPrint.MaHD}.pdf");
                Document document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                // THÊM NỘI DUNG
                Paragraph tieuDe = new Paragraph("CHI TIẾT HÓA ĐƠN BÁN HÀNG", fontTieuDe);
                tieuDe.Alignment = Element.ALIGN_CENTER;
                document.Add(tieuDe);
                document.Add(new Paragraph($"Ngày lập: {hoaDonToPrint.NgayLap:dd/MM/yyyy HH:mm:ss}", fontThuong));
                document.Add(new Paragraph($"Mã HĐ: HD{hoaDonToPrint.MaHD}", fontThuong));
                document.Add(new Paragraph($"Nhân viên: {tenNhanVien}", fontThuong));
                document.Add(Chunk.NEWLINE);

                document.Add(new Paragraph($"Khách hàng: {tenKhachHang}", fontThuong));
                document.Add(new Paragraph($"Loại khách hàng: {loaiKhachHang}", fontThuong));
                document.Add(Chunk.NEWLINE);

                // Bảng chi tiết
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                float[] widths = new float[] { 3f, 1f, 1f, 1.5f, 1.5f };
                table.SetWidths(widths);

                table.AddCell(new Phrase("Tên Sản Phẩm", fontDam));
                table.AddCell(new Phrase("Size", fontDam));
                table.AddCell(new Phrase("SL", fontDam));
                table.AddCell(new Phrase("Đơn Giá", fontDam));
                table.AddCell(new Phrase("Thành Tiền", fontDam));

                foreach (var item in hoaDonToPrint.ChiTietHDs)
                {
                    string tenSP = item.SanPhamKichCo.SanPham.TenSP;
                    decimal donGia = item.DonGia;
                    decimal thanhTien = item.ThanhTien.Value;

                    if (item.IsTang)
                    {
                        tenSP += " (Hàng tặng)";
                    }

                    table.AddCell(new Phrase(tenSP, fontThuong));
                    table.AddCell(new Phrase(item.SanPhamKichCo.KichCo.KichCo1.ToString(), fontThuong));
                    table.AddCell(new Phrase(item.SoLuong.ToString(), fontThuong));
                    table.AddCell(new Phrase(donGia.ToString("N0"), fontThuong));
                    table.AddCell(new Phrase(thanhTien.ToString("N0"), fontThuong));
                }
                document.Add(table);
                document.Add(Chunk.NEWLINE);

                // Tổng kết
                document.Add(new Paragraph($"Tổng số lượng: {tongSoLuong}", fontThuong));
                document.Add(new Paragraph($"Tổng tiền hàng: {hoaDonToPrint.TongTienGoc:N0} đ", fontThuong));
                document.Add(new Paragraph($"Mã giảm giá: {maGiamGia}", fontThuong));
                document.Add(new Paragraph($"Tiền giảm: -{hoaDonToPrint.TienGiam:N0} đ", fontThuong));

                Paragraph tongThanhToan = new Paragraph($"TỔNG THANH TOÁN: {hoaDonToPrint.TongTienSauGiam:N0} đ", fontDam);
                tongThanhToan.Alignment = Element.ALIGN_RIGHT;
                document.Add(tongThanhToan);

                document.Close();
                writer.Close();

                System.Diagnostics.Process.Start(filePath); // Tự động mở file
            }
            catch (Exception ex)
            {
                if (ex is System.IO.IOException)
                {
                    MessageBox.Show($"Lỗi: Không thể ghi file PDF. File có thể đang được mở.\n{ex.Message}", "Lỗi In Hóa Đơn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("Lỗi khi tạo file PDF: " + ex.Message, "Lỗi In Hóa Đơn", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
