using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Linq; // <-- BẮT BUỘC PHẢI CÓ
using System.Windows.Forms.DataVisualization.Charting;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace QLCF_NewVer
{
    public partial class ThongKeDoanhThu : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

        public ThongKeDoanhThu()
        {
            InitializeComponent();
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<HoaDon>(hd => hd.NguoiDung);
            dlo.LoadWith<HoaDon>(hd => hd.KhachHang);
            dlo.LoadWith<HoaDon>(hd => hd.ApMaVCs);
            dlo.LoadWith<HoaDon>(hd => hd.ChiTietHDs);

            dlo.LoadWith<ApMaVC>(ap => ap.Voucher);
            dlo.LoadWith<ChiTietHD>(ct => ct.SanPhamKichCo);

            dlo.LoadWith<Voucher>(v => v.KieuVC);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.SanPham);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.KichCo);

            dlo.LoadWith<SanPham>(sp => sp.LoaiSP);
            db.LoadOptions = dlo;
        }

        private void ThongKeDoanhThu_Load(object sender, EventArgs e)
        {
            dtpTuNgay.Value = DateTime.Today.AddDays(-30);
            dtpDenNgay.Value = DateTime.Today;
            LoadComboBoxLoaiSP();

            // === BẮT BUỘC: TẮT TỰ ĐỘNG TẠO CỘT ===
            dgvHoaDon.AutoGenerateColumns = false;
            // ===================================

            // Thêm các cột cho dgvHoaDon MỘT LẦN DUY NHẤT
            SetupDataGridView();

            btnTimKiem_Click(sender, e);
        }
        private void SetupDataGridView()
        {
            dgvHoaDon.Columns.Clear();

            // Thêm các cột bằng tay, đặt DataPropertyName khớp với query
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaHD", HeaderText = "Mã HĐ" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayLap", HeaderText = "Ngày Lập" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NhanVien", HeaderText = "Nhân Viên" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "KhachHang", HeaderText = "Khách Hàng" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoDienThoai", HeaderText = "Số điện thoại" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TongTienGoc", HeaderText = "Tổng tiền gốc", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TienGiam", HeaderText = "Tiền giảm", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TongTien", HeaderText = "Tổng Tiền", DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" } });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaGiamGia", HeaderText = "Mã voucher" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "PhanTramGiam", HeaderText = "Phần trăm giảm" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "LoaiVoucher", HeaderText = "Loại voucher" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SanPhamMua", HeaderText = "Sản phẩm mua" });
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SanPhamTang", HeaderText = "Sản phẩm tặng" });
            
            // Tự động chỉnh độ rộng
            dgvHoaDon.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        // 3. HÀM TẢI COMBOBOX (Dùng db chung)
        private void LoadComboBoxLoaiSP()
        {
            var loaiSPs = db.LoaiSPs
                            .Select(l => new { l.MaLoai, l.TenLoai })
                            .ToList();
            loaiSPs.Insert(0, new { MaLoai = 0, TenLoai = "Tất cả" });
            cbbLoaiSP.DataSource = loaiSPs;
            cbbLoaiSP.ValueMember = "MaLoai";
            cbbLoaiSP.DisplayMember = "TenLoai";
        }

        // 4. NÚT "TÌM KIẾM" (HÀM LOGIC CHÍNH - CẬP NHẬT CẢ 3)
        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date.AddDays(1).AddSeconds(-1);
            int maLoai = (int)cbbLoaiSP.SelectedValue;

            // === BẮT BUỘC: LÀM MỚI (REFRESH) DB CHUNG ===
            db.Refresh(RefreshMode.OverwriteCurrentValues, db.HoaDons);
            db.Refresh(RefreshMode.OverwriteCurrentValues, db.ChiTietHDs);
            db.Refresh(RefreshMode.OverwriteCurrentValues, db.ApMaVCs);
            // ============================================

            // --- BƯỚC 1: LỌC HÓA ĐƠN ---
            IQueryable<HoaDon> queryHoaDon = db.HoaDons
                .Where(hd => hd.NgayLap >= tuNgay && hd.NgayLap <= denNgay);

            if (maLoai != 0)
            {
                queryHoaDon = queryHoaDon
                    .Where(hd => hd.ChiTietHDs.Any(ct => ct.SanPhamKichCo.SanPham.MaLoai == maLoai));
            }

            var listHoaDonDaLoc = queryHoaDon.ToList();

            // --- BƯỚC 2: CẬP NHẬT DATAGRIDVIEW (LỊCH SỬ HÓA ĐƠN) ---
            var dataForGrid = listHoaDonDaLoc
                .Select(hd => new // Query C#
                {
                    hd.MaHD,
                    NgayLap = hd.NgayLap.ToString("dd/MM/yyyy"),
                    NhanVien = hd.NguoiDung.HoTen,
                    KhachHang = hd.KhachHang?.TenKH ?? "Vãng lai",
                    SoDienThoai = hd.KhachHang?.SDT ?? "",
                    hd.TongTienGoc,
                    hd.TienGiam,
                    TongTien = hd.TongTienSauGiam,
                    MaGiamGia = hd.ApMaVCs.FirstOrDefault()?.Voucher?.Code,
                    PhanTramGiam = (hd.ApMaVCs.FirstOrDefault()?.Voucher?.MaLoaiVC == 1) ? hd.ApMaVCs.FirstOrDefault().Voucher.GiaTri : (decimal?)null,
                    LoaiVoucher = hd.ApMaVCs.FirstOrDefault()?.Voucher?.KieuVC.TenLoai,
                    SanPhamMua = string.Join(", ", hd.ChiTietHDs
                                                .Where(ct => !ct.IsTang)
                                                .Select(ct => ct.SanPhamKichCo.SanPham.TenSP)),
                    SanPhamTang = string.Join(", ", hd.ChiTietHDs
                                                .Where(ct => ct.IsTang)
                                                .Select(ct => ct.SanPhamKichCo.SanPham.TenSP)),
                    SanPhamDuocGiam = ""
                })
                .OrderByDescending(x => x.MaHD)
                .ToList();

            dgvHoaDon.DataSource = dataForGrid;

            // --- BƯỚC 3: CẬP NHẬT TEXTBOX TỔNG DOANH THU ---
            decimal tongDoanhThuSauGiam = listHoaDonDaLoc.Sum(hd => (decimal?)hd.TongTienSauGiam) ?? 0;
            txtTongDoanhThu.Text = tongDoanhThuSauGiam.ToString("N0") + " VNĐ";

            // --- BƯỚC 4: LẤY DỮ LIỆU GỐC CHO CẢ 2 BIỂU ĐỒ ---
            IQueryable<ChiTietHD> queryGocChiTiet = db.ChiTietHDs
                .Where(ct => ct.HoaDon.NgayLap >= tuNgay &&
                             ct.HoaDon.NgayLap <= denNgay &&
                             ct.IsTang == false);
            if (maLoai != 0)
            {
                queryGocChiTiet = queryGocChiTiet
                    .Where(ct => ct.SanPhamKichCo.SanPham.MaLoai == maLoai);
            }

            var listChiTietDaLoc = queryGocChiTiet.ToList();

            // --- BƯỚC 5: CẬP NHẬT BIỂU ĐỒ 1 (THEO LOẠI SP) ---
            var dataForChartLoai = listChiTietDaLoc
                .GroupBy(ct => ct.SanPhamKichCo.SanPham.LoaiSP.TenLoai)
                .Select(g => new
                {
                    TenLoai = g.Key,
                    TongDoanhThu = g.Sum(ct => (decimal?)ct.ThanhTien) ?? 0
                })
                .ToList();

                chrThongKe.Series["Series1"].Points.Clear();
                chrThongKe.Series["Series1"].LegendText = "Doanh thu (Gốc) theo Loại";
                chrThongKe.DataSource = dataForChartLoai;
                chrThongKe.Series["Series1"].XValueMember = "TenLoai";
                chrThongKe.Series["Series1"].YValueMembers = "TongDoanhThu";
                chrThongKe.Series["Series1"].ChartType = SeriesChartType.Column;
                chrThongKe.Series["Series1"].IsValueShownAsLabel = true;
                chrThongKe.Series["Series1"].LabelFormat = "N0";
                chrThongKe.ChartAreas[0].AxisX.LabelStyle.Angle = 0; // Đặt lại
                chrThongKe.DataBind();

            var dataForChartBanChay = listChiTietDaLoc
                .GroupBy(ct => new 
                {
                    TenSP = ct.SanPhamKichCo.SanPham.TenSP,
                    KichCo = ct.SanPhamKichCo.KichCo.KichCo1
                })
                .Select(g => new
                {
                    TenHienThi = g.Key.TenSP + " (" + g.Key.KichCo + ")",
                    TongDoanhThu = g.Sum(ct => (decimal?)ct.ThanhTien) ?? 0
                })
                .OrderByDescending(x => x.TongDoanhThu)
                .Take(5)
                .ToList();

                chrBanChay.Series["Series1"].Points.Clear();
                chrBanChay.Series["Series1"].LegendText = "Top 5 Bán Chạy";
                chrBanChay.DataSource = dataForChartBanChay;
                chrBanChay.Series["Series1"].XValueMember = "TenHienThi";
                chrBanChay.Series["Series1"].YValueMembers = "TongDoanhThu";
                chrBanChay.Series["Series1"].ChartType = SeriesChartType.Column;
                chrBanChay.Series["Series1"].IsValueShownAsLabel = true;
                chrBanChay.Series["Series1"].LabelFormat = "N0";
                chrBanChay.ChartAreas[0].AxisX.LabelStyle.Interval = 1;
                chrBanChay.ChartAreas[0].AxisX.LabelStyle.Angle = -45; // Nghiêng nhãn
                chrBanChay.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                chrBanChay.DataBind();

        }


        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            dtpTuNgay.Value = DateTime.Today.AddDays(-30);
            dtpDenNgay.Value = DateTime.Today;
            cbbLoaiSP.SelectedIndex = 0;
            btnTimKiem_Click(sender, e); // Gọi lại TimKiem (đã bao gồm Refresh)
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // (HÀM XUẤT PDF - Giữ nguyên logic cũ, chỉ xuất dgvHoaDon)
        private void btnXuatThongKe_Click(object sender, EventArgs e)
        {
            if (dgvHoaDon.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "PDF files (*.pdf)|*.pdf";
            saveFile.FileName = $"ThongKeDoanhThu_{DateTime.Now:yyyyMMdd}.pdf";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // (Code khởi tạo Font giữ nguyên)
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");
                    if (!File.Exists(fontPath)) { /* ... (báo lỗi font) ... */ return; }
                    BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font fontTieuDe = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 11, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontBody = new iTextSharp.text.Font(bf, 9, iTextSharp.text.Font.NORMAL);
                    iTextSharp.text.Font fontHeaderNho = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);

                    Document doc = new Document(PageSize.A4.Rotate()); // XOAY NGANG VÌ NHIỀU CỘT
                    PdfWriter.GetInstance(doc, new FileStream(saveFile.FileName, FileMode.Create));
                    doc.Open();

                    // Thêm Tiêu đề
                    Paragraph tieuDe = new Paragraph("BÁO CÁO THỐNG KÊ DOANH THU", fontTieuDe);
                    tieuDe.Alignment = Element.ALIGN_CENTER;
                    tieuDe.SpacingAfter = 15;
                    doc.Add(tieuDe);

                    // Thêm thông tin lọc
                    doc.Add(new Paragraph($"Từ ngày: {dtpTuNgay.Value:dd/MM/yyyy}", fontBody));
                    doc.Add(new Paragraph($"Đến ngày: {dtpDenNgay.Value:dd/MM/yyyy}", fontBody));
                    doc.Add(new Paragraph($"Loại sản phẩm: {cbbLoaiSP.Text}", fontBody));
                    doc.Add(new Paragraph($"TỔNG DOANH THU (Đã tính ưu đãi): {txtTongDoanhThu.Text}", fontHeaderNho));
                    doc.Add(Chunk.NEWLINE);

                    // TẠO BẢNG DỮ LIỆU
                    int colCount = 0;
                    foreach (DataGridViewColumn col in dgvHoaDon.Columns)
                    {
                        if (col.Visible) colCount++;
                    }

                    PdfPTable pdfTable = new PdfPTable(colCount);
                    pdfTable.WidthPercentage = 100;

                    // (Code Thêm Header)
                    foreach (DataGridViewColumn column in dgvHoaDon.Columns)
                    {
                        if (column.Visible)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, fontHeader));
                            cell.BackgroundColor = new BaseColor(240, 240, 240);
                            pdfTable.AddCell(cell);
                        }
                    }

                    // (Code Thêm Dữ liệu)
                    foreach (DataGridViewRow row in dgvHoaDon.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Visible)
                            {
                                string cellValue = cell.Value?.ToString() ?? "";

                                // SỬA LẠI: Kiểm tra đúng tên cột tiền
                                if (cell.OwningColumn.Name == "TongTienGoc" ||
                                    cell.OwningColumn.Name == "TienGiam" ||
                                    cell.OwningColumn.Name == "TongTien")
                                {
                                    if (decimal.TryParse(cellValue, out decimal val))
                                    {
                                        cellValue = val.ToString("N0");
                                    }
                                }
                                pdfTable.AddCell(new Phrase(cellValue, fontBody));
                            }
                        }
                    }

                    doc.Add(pdfTable);
                    doc.Close();

                    MessageBox.Show("Xuất file PDF thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    System.Diagnostics.Process.Start(saveFile.FileName);
                }
                catch (IOException ioEx)
                {
                    MessageBox.Show("Lỗi: Không thể ghi file. \nFile PDF có thể đang được mở.", "Lỗi File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}