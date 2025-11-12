using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;

namespace QLCF_NewVer
{
    public partial class QuanLyKho : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private int? _selectedIdSPKC = null;
        public QuanLyKho()
        {
            InitializeComponent();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            FilterAndLoadData();
        }

        private void QuanLyKho_Load(object sender, EventArgs e)
        {
            LoadComboBoxLoc();
            FilterAndLoadData();
        }
        private void LoadComboBoxLoc()
        {
            var loaiSPs = db.LoaiSPs.Select(l => l.TenLoai).ToList();
            foreach (var tenLoai in loaiSPs)
            {
                cbbLocDuLieu.Items.Add(tenLoai);
            }
            cbbLocDuLieu.Items.Add("Tất cả");
            cbbLocDuLieu.Items.Add("Hàng sắp hết (dưới mức cảnh báo)");
            cbbLocDuLieu.Items.Add("Hàng tồn nhiều");
        }

        private void FilterAndLoadData()
        {
            string searchTerm = txtTimKiem.Text.Trim().ToLower();
            string filter = cbbLocDuLieu.SelectedItem?.ToString() ?? "Tất cả";

            // Bắt đầu với query cơ bản (Join 4 bảng bằng Lambda)
            var query = db.SanPhamKichCos
                .Join(db.SanPhams, // Join SanPham
                    spkc => spkc.MaSP,
                    sp => sp.MaSP,
                    (spkc, sp) => new { spkc, sp })
                .Join(db.KichCos, // Join KichCo
                    j1 => j1.spkc.MaKichCo,
                    size => size.MaKichCo,
                    (j1, size) => new { j1.spkc, j1.sp, size })
                .Join(db.LoaiSPs, // Join LoaiSP
                    j2 => j2.sp.MaLoai,
                    loai => loai.MaLoai,
                    (j2, loai) => new { j2.spkc, j2.sp, j2.size, loai });

            // 1. Lọc theo ComboBox
            if (filter == "Hàng sắp hết (dưới mức cảnh báo)")
            {
                query = query.Where(r => r.spkc.SoLuongTon <= r.spkc.CanhBaoTonKho);
            }
            else if (filter == "Hàng tồn nhiều")
            {
                query = query.Where(r => r.spkc.SoLuongTon > r.spkc.CanhBaoTonKho);
            }
            else if (filter != "Tất cả") // Lọc theo Tên Loại SP
            {
                query = query.Where(r => r.loai.TenLoai == filter);
            }

            // 2. Lọc theo Từ khóa
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r =>
                    r.sp.TenSP.ToLower().Contains(searchTerm) ||
                    r.sp.MaSP.ToLower().Contains(searchTerm)
                );
            }

            // 3. Chọn ra các cột cuối cùng để hiển thị
            var result = query.Select(r => new
            {
                r.spkc.IdSPKC,
                r.sp.MaSP,
                r.sp.TenSP,
                KichCo = r.size.KichCo1,
                r.loai.TenLoai,
                r.spkc.SoLuongTon,
                r.spkc.CanhBaoTonKho,

                TinhTrang = (r.spkc.SoLuongTon == 0) ? "Hết hàng" :
                (r.spkc.SoLuongTon <= r.spkc.CanhBaoTonKho) ? "Sắp hết" :
                "Còn hàng"

            }).ToList();

            dgvTonKho.DataSource = result;

            // Đặt lại tên cột (nếu cần)
            dgvTonKho.Columns["IdSPKC"].Visible = false;
            dgvTonKho.Columns["MaSP"].HeaderText = "Mã SP";
            dgvTonKho.Columns["TenSP"].HeaderText = "Tên Sản Phẩm";
            dgvTonKho.Columns["KichCo"].HeaderText = "Size";
            dgvTonKho.Columns["TenLoai"].HeaderText = "Loại";
            dgvTonKho.Columns["SoLuongTon"].HeaderText = "Tồn Kho";
            dgvTonKho.Columns["CanhBaoTonKho"].HeaderText = "Mức Cảnh Báo";
            dgvTonKho.Columns["TinhTrang"].HeaderText = "Tình Trạng";
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Clear();
            cbbLocDuLieu.SelectedIndex = 0;
            FilterAndLoadData();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnXuatTonKho_Click(object sender, EventArgs e)
        {
            if (dgvTonKho.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để xuất.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 1. CHỌN NƠI LƯU FILE
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "PDF files (*.pdf)|*.pdf";
            saveFile.FileName = $"BaoCaoTonKho_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // 2. KHỞI TẠO FONT TIẾNG VIỆT
                    // (Đây là bước quan trọng nhất để không bị lỗi font)
                    // Bạn cần copy file "arial.ttf" (font Arial) vào thư mục Debug của project
                    string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "arial.ttf");

                    // Kiểm tra xem có font Arial trong máy không
                    if (!File.Exists(fontPath))
                    {
                        MessageBox.Show("Không tìm thấy font 'arial.ttf' trong hệ thống. Vui lòng cài đặt font Arial.", "Lỗi Font", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                    iTextSharp.text.Font fontTieuDe = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontHeader = new iTextSharp.text.Font(bf, 12, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font fontBody = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                    // 3. TẠO TÀI LIỆU PDF
                    Document doc = new Document(PageSize.A4.Rotate()); // Xoay ngang
                    PdfWriter.GetInstance(doc, new FileStream(saveFile.FileName, FileMode.Create));
                    doc.Open();

                    // Thêm Tiêu đề
                    Paragraph tieuDe = new Paragraph("BÁO CÁO TỒN KHO", fontTieuDe);
                    tieuDe.Alignment = Element.ALIGN_CENTER;
                    tieuDe.SpacingAfter = 20;
                    doc.Add(tieuDe);

                    // Thêm ngày giờ
                    Paragraph ngayTao = new Paragraph($"Ngày tạo: {DateTime.Now}", fontBody);
                    ngayTao.Alignment = Element.ALIGN_RIGHT;
                    ngayTao.SpacingAfter = 10;
                    doc.Add(ngayTao);

                    // 4. TẠO BẢNG DỮ LIỆU
                    PdfPTable pdfTable = new PdfPTable(dgvTonKho.Columns.GetColumnCount(DataGridViewElementStates.Visible));
                    pdfTable.WidthPercentage = 100;

                    // Thêm Header (Tên cột)
                    foreach (DataGridViewColumn column in dgvTonKho.Columns)
                    {
                        if (column.Visible)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, fontHeader));
                            cell.BackgroundColor = new BaseColor(240, 240, 240); // Màu xám nhạt
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }
                    }

                    // Thêm Dữ liệu (Các hàng)
                    foreach (DataGridViewRow row in dgvTonKho.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            if (cell.Visible)
                            {
                                // Lấy giá trị và chuyển sang string, xử lý null
                                string cellValue = cell.Value?.ToString() ?? "";
                                PdfPCell dataCell = new PdfPCell(new Phrase(cellValue, fontBody));
                                pdfTable.AddCell(dataCell);
                            }
                        }
                    }

                    // 5. THÊM BẢNG VÀO TÀI LIỆU VÀ ĐÓNG LẠI
                    doc.Add(pdfTable);
                    doc.Close();

                    MessageBox.Show("Xuất file PDF thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Tự động mở file PDF sau khi lưu
                    System.Diagnostics.Process.Start(saveFile.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xuất PDF: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnThemTonKho_Click(object sender, EventArgs e)
        {
            if (_selectedIdSPKC == null)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ danh sách bên trên trước.",
                                "Chưa chọn sản phẩm",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return; // Dừng lại, không mở form
            }

            // 2. Nếu đã chọn, mở form ThemTonKho và "gửi" ID qua
            ThemTonKho f = new ThemTonKho(_selectedIdSPKC);
            var result = f.ShowDialog();

            // 3. Tải lại dữ liệu nếu nhập kho thành công
            if (result == DialogResult.OK)
            {
                FilterAndLoadData();
            }

            // 4. Reset lựa chọn sau khi form đóng
            _selectedIdSPKC = null;
        }

        private void btnThemTonKhoMoi_Click(object sender, EventArgs e)
        {
            string message = "Bạn sắp mở chức năng 'Thêm tồn kho MỚI'.\n\n" +
                     "Chức năng này dùng để **TẠO MỘT SẢN PHẨM MỚI** (chưa từng có) với giá bán và size (ví dụ: tạo 'Bánh Pateso' lần đầu tiên).\n\n" +
                     "Bạn có muốn tiếp tục không?";

            var confirmResult = MessageBox.Show(message,
                                                "Xác nhận: Tạo sản phẩm mới",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Question);

            if (confirmResult == DialogResult.Yes)
            {
                // === CODE CŨ CỦA BẠN ===
                ThemTonKhoMoi f = new ThemTonKhoMoi();
                var result = f.ShowDialog();

                if (result == DialogResult.OK)
                {
                    FilterAndLoadData();
                }
            }
        }

        private void dgvTonKho_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dgvTonKho.Rows.Count)
            {
                try
                {
                    // Lấy IdSPKC (là khóa chính của SanPhamKichCo)
                    _selectedIdSPKC = Convert.ToInt32(dgvTonKho.Rows[e.RowIndex].Cells["IdSPKC"].Value);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn dòng: " + ex.Message);
                    _selectedIdSPKC = null;
                }
            }
        }
    }
}
