using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic; // <-- BẮT BUỘC (Và phải cài NuGet "Microsoft.VisualBasic")

// THÊM 3 THƯ VIỆN NÀY ĐỂ TẠO PDF (BẮT BUỘC CÀI NUGET "iTextSharp")
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Diagnostics.Eventing.Reader;

namespace QLCF_NewVer
{
    public partial class BanHang : Form
    {
        // 1. BIẾN TOÀN CỤC VÀ CSDL
        // =================================================================
        QLCF_NewVerDataContext db;
        private List<GioHangItem> gioHang;
        private int? selectedMaVC = null;
        private decimal giamGiaPhanTramTheoPhieu = 0;
        private decimal giamGiaTienMatTheoPhieu = 0;
        private List<int> dsIdSPKCTang = new List<int>();
        private string _maND_DaDangNhap = "AD01";

        // LỚP GIỮ DỮ LIỆU GIỎ HÀNG
        public class GioHangItem
        {
            public SanPhamKichCo SanPham { get; set; }
            public int SoLuong { get; set; }
            public bool LaHangTang { get; set; } = false;
        }

        // ----------------------------------------------------------------------
        // 2. KHỞI TẠO VÀ LOAD FORM
        // =================================================================
        public BanHang()
        {
            InitializeComponent();
            db = new QLCF_NewVerDataContext();
            gioHang = new List<GioHangItem>();
        }

        private void BanHang_Load(object sender, EventArgs e)
        {
            DataLoadOptions dlo = new DataLoadOptions();

            // Tải các liên kết của Sản phẩm
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.SanPham);
            dlo.LoadWith<SanPhamKichCo>(spkc => spkc.KichCo);
            dlo.LoadWith<SanPham>(sp => sp.LoaiSP);

            // Tải các liên kết của Voucher
            dlo.LoadWith<Voucher>(v => v.ChiTietVCs);
            dlo.LoadWith<ChiTietVC>(ct => ct.SanPhamKichCo);

            // Tải các liên kết của Hóa Đơn
            dlo.LoadWith<HoaDon>(h => h.NguoiDung);
            dlo.LoadWith<HoaDon>(h => h.KhachHang);
            dlo.LoadWith<HoaDon>(h => h.ChiTietHDs);
            dlo.LoadWith<ChiTietHD>(ct => ct.SanPhamKichCo);

            db.LoadOptions = dlo; // Áp dụng cấu hình

            LoadComboBoxKhachHang();
            LoadComboBoxLoaiSP();
            SetupDgvSanPham();
            SetupDgvSanPhamDaChon();
            LoadDgvSanPham(null, null);

            if (cbbTenKH.Items.Count > 0)
                cbbTenKH.SelectedIndex = 0;

            txtTienThoiLai.ReadOnly = true;
            txtTienKhachDua.TextChanged += TxtTienKhachDua_TextChanged;
        }

        private void SetupDgvSanPham()
        {
            dgvSanPham.AutoGenerateColumns = false;
            dgvSanPham.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanPham.MultiSelect = false;
            dgvSanPham.ReadOnly = true;
            dgvSanPham.AllowUserToAddRows = false;

            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IdSPKC",
                HeaderText = "ID",
                DataPropertyName = "IdSPKC",
                Visible = false
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSP",
                HeaderText = "Tên Sản Phẩm",
                DataPropertyName = "TenSP",
                Width = 150
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "KichCo1",
                HeaderText = "Size",
                DataPropertyName = "KichCo1",
                Width = 50
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GiaBan",
                HeaderText = "Giá Bán",
                DataPropertyName = "GiaBan",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvSanPham.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuongTon",
                HeaderText = "Tồn Kho",
                DataPropertyName = "SoLuongTon"
            });
        }

        private void SetupDgvSanPhamDaChon()
        {
            dgvSanPhamDaChon.AutoGenerateColumns = false;
            dgvSanPhamDaChon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSanPhamDaChon.ReadOnly = true;
            dgvSanPhamDaChon.AllowUserToAddRows = false;

            dgvSanPhamDaChon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "IdSPKC_Cart",
                HeaderText = "ID",
                Visible = false
            });
            dgvSanPhamDaChon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenSP_Cart",
                HeaderText = "Tên Sản Phẩm",
                Width = 130
            });
            dgvSanPhamDaChon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "KichCo_Cart",
                HeaderText = "Size",
                Width = 50
            });
            dgvSanPhamDaChon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "SoLuong_Cart",
                HeaderText = "SL",
                Width = 40
            });
            dgvSanPhamDaChon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "DonGia_Cart",
                HeaderText = "Đơn Giá",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvSanPhamDaChon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThanhTien_Cart",
                HeaderText = "Thành Tiền",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            var colXoa = new DataGridViewButtonColumn
            {
                Name = "colXoa",
                HeaderText = "",
                Text = "Xóa",
                UseColumnTextForButtonValue = true,
                Width = 50,
                FlatStyle = FlatStyle.Flat
            };
            colXoa.DefaultCellStyle.BackColor = Color.LightCoral;
            dgvSanPhamDaChon.Columns.Add(colXoa);
        }

        // ----------------------------------------------------------------------
        // 3. CÁC HÀM XỬ LÝ SỰ KIỆN (CLICK NÚT...)
        // =================================================================

        private void TxtTienKhachDua_TextChanged(object sender, EventArgs e)
        {
            try
            {
                decimal tienKhachDua = 0;
                if (!string.IsNullOrWhiteSpace(txtTienKhachDua.Text))
                {
                    tienKhachDua = decimal.Parse(txtTienKhachDua.Text.Replace(".", ""));
                }

                decimal tongTien = 0;
                if (!string.IsNullOrWhiteSpace(txtTongTien.Text))
                {
                    tongTien = decimal.Parse(txtTongTien.Text.Replace(".", ""));
                }

                decimal tienThoiLai = tienKhachDua - tongTien;
                txtTienThoiLai.Text = tienThoiLai.ToString("N0");
            }
            catch (Exception)
            {
                txtTienThoiLai.Text = "Lỗi";
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text;
            int? maLoai = null;
            if (cbbLocDuLieu.SelectedIndex > 0)
            {
                var selectedItem = cbbLocDuLieu.Items[cbbLocDuLieu.SelectedIndex];
                maLoai = (int)((dynamic)selectedItem).MaLoai;
            }
            LoadDgvSanPham(keyword, maLoai);
        }

        private void btnThemVaoGioHang_Click(object sender, EventArgs e)
        {
            if (dgvSanPham.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một sản phẩm từ danh sách bên trên!");
                return;
            }
            var selectedRow = dgvSanPham.SelectedRows[0];
            int idSPKC = (int)selectedRow.Cells["IdSPKC"].Value;

            var sp = db.SanPhamKichCos.FirstOrDefault(s => s.IdSPKC == idSPKC);
            if (sp == null) return;

            string input = Interaction.InputBox(
                $"Nhập số lượng cho {sp.SanPham.TenSP} ({sp.KichCo.KichCo1}):", // Dùng KichCo1
                "Chọn số lượng", "1");

            if (!int.TryParse(input, out int soLuong) || soLuong <= 0) { return; }

            // Check tồn kho (bao gồm cả hàng trong giỏ)
            int soLuongDaCoTrongGio = gioHang
                .Where(i => i.SanPham.IdSPKC == idSPKC && !i.LaHangTang)
                .Sum(i => i.SoLuong);

            if (soLuong + soLuongDaCoTrongGio > sp.SoLuongTon)
            {
                MessageBox.Show($"Số lượng vượt quá tồn kho! (Tồn: {sp.SoLuongTon}, Đã chọn: {soLuongDaCoTrongGio})");
                return;
            }

            ThemSanPhamVaoListGioHang(sp, soLuong, false);
        }

        private void dgvSanPhamDaChon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == dgvSanPhamDaChon.Columns["colXoa"].Index)
            {
                int idSPKC = (int)dgvSanPhamDaChon.Rows[e.RowIndex].Cells["IdSPKC_Cart"].Value;
                bool laHangTang = dgvSanPhamDaChon.Rows[e.RowIndex].DefaultCellStyle.BackColor == Color.LightYellow;

                // SỬA: Tìm và xóa item (phân biệt hàng tặng/bán)
                var itemCanXoa = gioHang.FirstOrDefault(i => i.SanPham.IdSPKC == idSPKC && i.LaHangTang == laHangTang);
                if (itemCanXoa != null)
                {
                    gioHang.Remove(itemCanXoa);
                }

                CapNhatTongTien();
            }
        }

        private void cbbTenKH_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = cbbTenKH.SelectedIndex;
            if (selectedIndex < 0) return;
            var selectedItem = cbbTenKH.Items[selectedIndex];
            if (selectedItem == null) return;
            int maKH = (int)((dynamic)selectedItem).MaKH;

            if (maKH == 0) { txtLoaiKH.Text = "Vãng lai"; }
            else
            {
                var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == maKH);
                if (khachHang != null)
                {
                    txtLoaiKH.Text = GetHangKhachHang(khachHang.TichDiem);
                }
            }
        }

        private void btnLuaChonMaGG_Click(object sender, EventArgs e)
        {
            var formChon = new LuaChonMaGG();
            if (formChon.ShowDialog() == DialogResult.OK)
            {
                this.selectedMaVC = formChon.SelectedMaVC;
                string selectedCode = formChon.SelectedCode;

                if (this.selectedMaVC == 0 || this.selectedMaVC == null)
                {
                    txtLuaChonMaGG.Text = "Không sử dụng";
                }
                else
                {
                    txtLuaChonMaGG.Text = selectedCode;
                }

                CapNhatTongTien();
            }
        }

        private void btnThanhToan_Click(object sender, EventArgs e)
        {
            if (!gioHang.Any())
            {
                MessageBox.Show("Giỏ hàng rỗng!");
                return;
            }

            decimal tongTienHang = gioHang.Where(i => !i.LaHangTang).Sum(i => i.SanPham.GiaBan * i.SoLuong);
            decimal tongPhanTramGiam = giamGiaPhanTramTheoPhieu;
            decimal soTienGiamTuTram = tongTienHang * tongPhanTramGiam;
            decimal soTienGiamTrucTiep = giamGiaTienMatTheoPhieu;
            decimal tongTienGiam = soTienGiamTuTram + soTienGiamTrucTiep;
            decimal tongTienCuoiCung = tongTienHang - tongTienGiam;

            decimal tienKhachDua = 0;
            if (!string.IsNullOrWhiteSpace(txtTienKhachDua.Text))
            {
                decimal.TryParse(txtTienKhachDua.Text.Replace(".", ""), out tienKhachDua);
            }

            if (tienKhachDua < tongTienCuoiCung)
            {
                MessageBox.Show("Tiền khách đưa không đủ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmResult = MessageBox.Show($"Tổng tiền: {tongTienCuoiCung:N0} đ\n\nXác nhận thanh toán?",
                                                "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirmResult == DialogResult.No)
            {
                return;
            }

            try
            {
                var selectedItem = cbbTenKH.Items[cbbTenKH.SelectedIndex];
                int maKH = (int)((dynamic)selectedItem).MaKH;

                var hoaDon = new HoaDon
                {
                    NgayLap = DateTime.Now,
                    MaND = _maND_DaDangNhap,
                    TongTienGoc = tongTienHang,
                    TienGiam = tongTienGiam,
                    MaKH = maKH == 0 ? (int?)null : maKH
                };

                db.HoaDons.InsertOnSubmit(hoaDon);
                db.SubmitChanges();

                foreach (var item in gioHang)
                {
                    var chiTietHD = new ChiTietHD
                    {
                        MaHD = hoaDon.MaHD,
                        IdSPKC = item.SanPham.IdSPKC,
                        SoLuong = item.SoLuong,
                        DonGia = item.LaHangTang ? 0 : item.SanPham.GiaBan, // Hàng tặng lưu giá 0đ
                        IsTang = item.LaHangTang
                    };
                    db.ChiTietHDs.InsertOnSubmit(chiTietHD);
                }

                int maVC = this.selectedMaVC ?? 0;
                if (maVC != 0)
                {
                    db.ApMaVCs.InsertOnSubmit(new ApMaVC { MaHD = hoaDon.MaHD, MaVC = maVC });
                }

                int? diemTichLuyMoi = null;
                if (maKH != 0)
                {
                    var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == maKH);
                    if (khachHang != null)
                    {
                        int diemThem = (int)(tongTienCuoiCung / 1000);
                        if (diemThem > 0)
                        {
                            khachHang.TichDiem += diemThem;
                            diemTichLuyMoi = khachHang.TichDiem;
                        }
                        else
                        {
                            diemTichLuyMoi = khachHang.TichDiem;
                        }
                    }
                }

                db.SubmitChanges();

                MessageBox.Show($"Thanh toán thành công! Mã hóa đơn: {hoaDon.MaHD}", "Thành công",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);

                var printResult = MessageBox.Show("Bạn có muốn in hóa đơn không?", "In hóa đơn",
                                                  MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (printResult == DialogResult.Yes)
                {
                    decimal tienThoiLai = 0;
                    if (!string.IsNullOrWhiteSpace(txtTienThoiLai.Text))
                    {
                        decimal.TryParse(txtTienThoiLai.Text.Replace(".", ""), out tienThoiLai);
                    }

                    PhatSinhFilePDF(hoaDon, gioHang, tienKhachDua, tienThoiLai, diemTichLuyMoi);
                }
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi thanh toán: " + ex.Message);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BanHang_FormClosing(object sender, FormClosingEventArgs e)
        {
            db.Dispose();
        }

        // ----------------------------------------------------------------------
        // 4. CÁC HÀM HỖ TRỢ (Helper Functions)
        // =================================================================

        private void PhatSinhFilePDF(HoaDon hoaDon, List<GioHangItem> gioHangPDF, decimal tienKhachDua, decimal tienThoiLai, int? diemTichLuyMoi)
        {
            try
            {
                if (hoaDon == null)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn để in!");
                    return;
                }

                string tenNhanVien = db.NguoiDungs.FirstOrDefault(nd => nd.MaND == hoaDon.MaND)?.HoTen ?? "N/A";
                string tenKhachHang = db.KhachHangs.FirstOrDefault(kh => kh.MaKH == hoaDon.MaKH)?.TenKH ?? "Khách vãng lai";
                string loaiKhachHang = txtLoaiKH.Text;
                string maGiamGia = txtLuaChonMaGG.Text;

                int tongSoLuong = gioHangPDF.Sum(item => item.SoLuong);

                string fontPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "Arial.ttf");
                BaseFont bf = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                iTextSharp.text.Font fontTieuDe = new iTextSharp.text.Font(bf, 16, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontDam = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.BOLD);
                iTextSharp.text.Font fontThuong = new iTextSharp.text.Font(bf, 10, iTextSharp.text.Font.NORMAL);

                string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), $"HoaDon_{hoaDon.MaHD}.pdf");
                Document document = new Document(PageSize.A4, 40, 40, 40, 40);
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
                document.Open();

                Paragraph tieuDe = new Paragraph("HÓA ĐƠN BÁN HÀNG", fontTieuDe);
                tieuDe.Alignment = Element.ALIGN_CENTER;
                document.Add(tieuDe);
                document.Add(new Paragraph($"Ngày lập: {hoaDon.NgayLap:dd/MM/yyyy HH:mm:ss}", fontThuong));
                document.Add(new Paragraph($"Mã HĐ: HD{hoaDon.MaHD}", fontThuong));
                document.Add(new Paragraph($"Nhân viên: {tenNhanVien}", fontThuong));
                document.Add(Chunk.NEWLINE);

                document.Add(new Paragraph($"Khách hàng: {tenKhachHang}", fontThuong));
                document.Add(new Paragraph($"Loại khách hàng: {loaiKhachHang}", fontThuong));
                document.Add(Chunk.NEWLINE);

                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                float[] widths = new float[] { 3f, 1f, 1f, 1.5f, 1.5f };
                table.SetWidths(widths);

                table.AddCell(new Phrase("Tên Sản Phẩm", fontDam));
                table.AddCell(new Phrase("Size", fontDam));
                table.AddCell(new Phrase("SL", fontDam));
                table.AddCell(new Phrase("Đơn Giá", fontDam));
                table.AddCell(new Phrase("Thành Tiền", fontDam));

                foreach (var item in gioHangPDF)
                {
                    string tenSP = item.SanPham.SanPham.TenSP;
                    decimal donGia = item.SanPham.GiaBan;
                    decimal thanhTien = item.SanPham.GiaBan * item.SoLuong;

                    if (item.LaHangTang)
                    {
                        tenSP += " (Hàng tặng)";
                        donGia = 0;
                        thanhTien = 0;
                    }

                    table.AddCell(new Phrase(tenSP, fontThuong));
                    table.AddCell(new Phrase(item.SanPham.KichCo.KichCo1.ToString(), fontThuong));
                    table.AddCell(new Phrase(item.SoLuong.ToString(), fontThuong));
                    table.AddCell(new Phrase(donGia.ToString("N0"), fontThuong));
                    table.AddCell(new Phrase(thanhTien.ToString("N0"), fontThuong));
                }
                document.Add(table);
                document.Add(Chunk.NEWLINE);

                // Tổng kết
                document.Add(new Paragraph($"Tổng số lượng: {tongSoLuong}", fontThuong));
                document.Add(new Paragraph($"Tổng tiền hàng: {hoaDon.TongTienGoc:N0} đ", fontThuong));
                document.Add(new Paragraph($"Mã giảm giá: {maGiamGia}", fontThuong));
                document.Add(new Paragraph($"Tiền giảm: -{hoaDon.TienGiam:N0} đ", fontThuong));

                Paragraph tongThanhToan = new Paragraph($"TỔNG THANH TOÁN: {hoaDon.TongTienSauGiam:N0} đ", fontDam);
                tongThanhToan.Alignment = Element.ALIGN_RIGHT;
                document.Add(tongThanhToan);

                document.Add(Chunk.NEWLINE);
                document.Add(new Paragraph($"Tiền khách đưa: {tienKhachDua:N0} đ", fontThuong));
                document.Add(new Paragraph($"Tiền thối lại: {tienThoiLai:N0} đ", fontThuong));

                if (diemTichLuyMoi.HasValue)
                {
                    document.Add(new Paragraph($"Điểm tích lũy (sau thanh toán): {diemTichLuyMoi.Value} điểm", fontThuong));
                }

                document.Close();
                writer.Close();

                System.Diagnostics.Process.Start(filePath);
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

        private void LoadDgvSanPham(string keyword, int? maLoai)
        {
            IQueryable<SanPhamKichCo> query = db.SanPhamKichCos
                                                .Where(spkc => spkc.TrangThaiSP == true);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(spkc => spkc.SanPham.TenSP.ToLower().Contains(keyword));
            }
            if (maLoai.HasValue && maLoai > 0)
            {
                query = query.Where(spkc => spkc.SanPham.MaLoai == maLoai);
            }

            var dsHienThi = query
                .OrderBy(spkc => spkc.SanPham.TenSP)
                .Select(spkc => new
                {
                    spkc.IdSPKC,
                    spkc.SanPham.TenSP,
                    spkc.KichCo.KichCo1, // Dùng KichCo1
                    spkc.GiaBan,
                    spkc.SoLuongTon
                }).ToList();

            dgvSanPham.DataSource = dsHienThi;
        }

        private string GetHangKhachHang(int diem)
        {
            if (diem >= 100000) return "Kim Cương";
            if (diem >= 20000) return "Bạch Kim";
            if (diem >= 5000) return "Vàng";
            if (diem >= 1000) return "Bạc";
            if (diem > 0) return "Đồng";
            return "Vãng lai";
        }

        // =================================================================
        // HÀM LOGIC "MUA 1 TẶNG 1" ĐÃ SỬA
        // =================================================================

        /// <summary>
        /// Hàm Thêm/Cập nhật SP vào List (KHÔNG TÍNH TOÁN)
        /// </summary>
        private void ThemVaoGioHang_NoUpdate(SanPhamKichCo sp, int soLuong, bool laHangTang)
        {
            var itemDaCo = gioHang.FirstOrDefault(i =>
                               i.SanPham.IdSPKC == sp.IdSPKC &&
                               i.LaHangTang == laHangTang
                          );

            if (itemDaCo != null)
            {
                itemDaCo.SoLuong += soLuong;
            }
            else
            {
                gioHang.Add(new GioHangItem { SanPham = sp, SoLuong = soLuong, LaHangTang = laHangTang });
            }

            // Nếu là hàng tặng, lưu lại ID để dễ xóa
            if (laHangTang && !dsIdSPKCTang.Contains(sp.IdSPKC))
            {
                dsIdSPKCTang.Add(sp.IdSPKC);
            }
        }

        /// <summary>
        /// Hàm "Xử lý" khi người dùng thêm SP (Sẽ gọi hàm tính toán)
        /// </summary>
        private void ThemSanPhamVaoListGioHang(SanPhamKichCo sp, int soLuong, bool laHangTang)
        {
            ThemVaoGioHang_NoUpdate(sp, soLuong, laHangTang);

            // Tính toán lại MỌI THỨ
            CapNhatTongTien();
        }

        /// <summary>
        /// (ĐÃ SỬA) Hàm "Xóa" hàng tặng (Chỉ xóa khỏi List)
        /// </summary>
        private void XoaTatCaHangTangKhoiGio()
        {
            // Chỉ xóa item trong List 'gioHang'
            gioHang.RemoveAll(i => i.LaHangTang == true);
            dsIdSPKCTang.Clear();
            // Không cập nhật UI vội, hàm CapNhatTongTien sẽ làm
        }

        /// <summary>
        /// Vẽ lại dgvSanPhamDaChon từ List 'gioHang'
        /// </summary>
        private void CapNhatGiaoDienGioHang()
        {
            dgvSanPhamDaChon.Rows.Clear();
            if (gioHang == null) return;

            foreach (var item in gioHang)
            {
                // SỬA: Thêm logic hiển thị 0đ
                decimal donGia = item.LaHangTang ? 0 : item.SanPham.GiaBan;
                decimal thanhTien = item.LaHangTang ? 0 : item.SanPham.GiaBan * item.SoLuong;
                string tenSP = item.LaHangTang ? $"{item.SanPham.SanPham.TenSP} (Hàng tặng)" : item.SanPham.SanPham.TenSP;

                dgvSanPhamDaChon.Rows.Add(
                    item.SanPham.IdSPKC,
                    tenSP, // SỬA
                    item.SanPham.KichCo.KichCo1, // Dùng KichCo1
                    item.SoLuong,
                    donGia, // SỬA
                    thanhTien // SỬA
                );
                if (item.LaHangTang)
                {
                    var rowMoi = dgvSanPhamDaChon.Rows[dgvSanPhamDaChon.Rows.Count - 1];
                    rowMoi.DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
        }

        /// <summary>
        /// (ĐÃ SỬA) "BỘ NÃO" MỚI: Tính toán lại toàn bộ
        /// </summary>
        private void CapNhatTongTien()
        {
            // BƯỚC 1: Xóa sạch hàng tặng và giảm giá cũ
            XoaTatCaHangTangKhoiGio();
            giamGiaPhanTramTheoPhieu = 0;
            giamGiaTienMatTheoPhieu = 0;

            // BƯỚC 2: Tính tổng tiền hàng (tổng bill)
            decimal tongTienHang = gioHang
                .Where(i => !i.LaHangTang)
                .Sum(i => i.SanPham.GiaBan * i.SoLuong);

            // BƯỚC 3: Kiểm tra nếu có voucher
            if (this.selectedMaVC.HasValue && this.selectedMaVC > 0)
            {
                var voucher = db.Vouchers.FirstOrDefault(v => v.MaVC == this.selectedMaVC);
                if (voucher != null)
                {
                    // Lấy điều kiện chung
                    decimal dieuKienTongTien = voucher.DieuKien ?? 0; // VD: 100.000

                    // KIỂM TRA ĐIỀU KIỆN TỔNG HÓA ĐƠN
                    // SỬA LỖI: Ta chỉ cần kiểm tra tổng bill (tongTienHang)
                    if (tongTienHang >= dieuKienTongTien)
                    {
                        // ĐỦ ĐIỀU KIỆN TỔNG BILL -> Áp dụng

                        // 3a. Xử lý giảm % và giảm tiền
                        if (voucher.MaLoaiVC == 1) { giamGiaPhanTramTheoPhieu = voucher.GiaTri / 100; }
                        else if (voucher.MaLoaiVC == 3) { giamGiaTienMatTheoPhieu = voucher.GiaTri; }

                        // 3b. Xử lý MUA 1 TẶNG 1 (M1T1)
                        if (voucher.MaLoaiVC == 2 || voucher.MaLoaiVC == 4)
                        {
                            int? maLoaiApDung = voucher.MaLoai; // VD: 3 (Freeze)

                            // TÌM CÁC SẢN PHẨM MUA KHỚP VỚI LOẠI ÁP DỤNG (Freeze)
                            var itemsDuocApDung = gioHang
                                .Where(i => !i.LaHangTang &&
                                            // CHỈ LẤY SP NẾU VOUCHER CÓ SET LOẠI VÀ SP NÀY KHỚP
                                            (maLoaiApDung == null || i.SanPham.SanPham.MaLoai == maLoaiApDung))
                                .ToList();

                            // TÍNH TỔNG SỐ LƯỢNG CỦA CÁC MÓN KHỚP NÀY
                            int soLuongTangQua = itemsDuocApDung.Sum(i => i.SoLuong);

                            // BẮT ĐẦU TẶNG QUÀ (Chỉ tặng nếu có SP khớp để tặng)
                            if (soLuongTangQua > 0)
                            {
                                if (voucher.MaLoaiVC == 2) // Mua 1 Tặng 1 CÙNG LOẠI
                                {
                                    // LOGIC FIX 100%: Tặng TỪNG MÓN CÙNG LOẠI ĐÃ MUA
                                    // Vd: Mua 3 Freeze -> Tặng 3 Freeze
                                    foreach (var item in itemsDuocApDung)
                                    {
                                        // Tặng cùng loại, cùng kích cỡ, cùng số lượng đã mua
                                        ThemVaoGioHang_NoUpdate(item.SanPham, item.SoLuong, true);
                                    }
                                }
                                else if (voucher.MaLoaiVC == 4) // Mua X Tặng Y KHÁC LOẠI
                                {
                                    // Logic này đã đúng: Tặng TỔNG SỐ LƯỢNG (soLuongTangQua) của MÓN QUÀ
                                    foreach (var itemTang in voucher.ChiTietVCs)
                                    {
                                        ThemVaoGioHang_NoUpdate(itemTang.SanPhamKichCo, soLuongTangQua, true);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // BƯỚC 4: Vẽ lại DataGridView (với hàng tặng mới nếu có)
            CapNhatGiaoDienGioHang();

            // BƯỚC 5: Tính tiền cuối cùng
            decimal tongTienHangSauLoc = gioHang
                .Where(i => !i.LaHangTang)
                .Sum(i => i.SanPham.GiaBan * i.SoLuong);

            decimal soTienGiamTuTram = tongTienHangSauLoc * giamGiaPhanTramTheoPhieu;
            decimal soTienGiamTrucTiep = giamGiaTienMatTheoPhieu;
            decimal thanhToanCuoiCung = tongTienHangSauLoc - soTienGiamTuTram - soTienGiamTrucTiep;

            txtTongTien.Text = thanhToanCuoiCung.ToString("N0");

            // BƯỚC 6: Tính lại tiền thối
            TxtTienKhachDua_TextChanged(null, null);
        }

        private void LoadComboBoxKhachHang()
        {
            var dsKhachHang = db.KhachHangs
                                .OrderBy(kh => kh.TenKH)
                                .Select(kh => new { MaKH = (int)kh.MaKH, khTieuDe = kh.TenKH });
            var khachVangLai = new[] { new { MaKH = 0, khTieuDe = "Khách vãng lai" } };
            var displayList = khachVangLai.Concat(dsKhachHang).ToList();

            cbbTenKH.DisplayMember = "khTieuDe";
            cbbTenKH.ValueMember = "MaKH";
            cbbTenKH.DataSource = displayList;
        }

        // (Hàm LoadComboBoxVoucher() không còn dùng)

        private void LoadComboBoxLoaiSP()
        {
            var dsLoaiSP = db.LoaiSPs
                             .OrderBy(l => l.TenLoai)
                             .Select(l => new { l.MaLoai, l.TenLoai })
                             .ToList();
            var displayList = new List<object>();
            displayList.Add(new { MaLoai = 0, TenLoai = "--- Lọc theo loại ---" });
            displayList.AddRange(dsLoaiSP);

            cbbLocDuLieu.DisplayMember = "TenLoai";
            cbbLocDuLieu.ValueMember = "MaLoai";
            cbbLocDuLieu.DataSource = displayList;
        }
    }
}