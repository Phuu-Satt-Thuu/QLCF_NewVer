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
    public partial class LichSuHoaDon : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        private List<object> dsNhanVien;
        public LichSuHoaDon()
        {
            InitializeComponent();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;
            string maNV = null;

            if (cbbTenNV.SelectedValue != null)
            {
                var selectedItem = cbbTenNV.Items[cbbTenNV.SelectedIndex];
                maNV = (string)((dynamic)selectedItem).MaND;
            }

            if (maNV == "ALL")
            {
                maNV = null; // null = không lọc
            }

            LoadHoaDonData(tuNgay, denNgay, maNV);
        }

        private void LichSuHoaDon_Load(object sender, EventArgs e)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<HoaDon>(hd => hd.NguoiDung);
            dlo.LoadWith<HoaDon>(hd => hd.KhachHang);
            db.LoadOptions = dlo;

            // 2. Load ComboBox Nhân Viên
            LoadComboBoxNhanVien();

            // 3. Cài đặt cột cho DataGridView
            SetupDgvHoaDon();

            // 4. Tải tất cả hóa đơn lên (lần đầu)
            LoadHoaDonData(null, null, null);
        }
        private void LoadComboBoxNhanVien()
        {
            // LINQ Lambda: Lấy MaND và HoTen
            var query = db.NguoiDungs
                          .OrderBy(nv => nv.HoTen)
                          .Select(nv => new { MaND = nv.MaND, HoTen = nv.HoTen })
                          .ToList();

            // Tạo danh sách mới để chèn "Tất cả"
            dsNhanVien = new List<object>();
            // Dùng MaND = "ALL" làm key cho "Tất cả nhân viên"
            dsNhanVien.Add(new { MaND = "ALL", HoTen = "--- Tất cả nhân viên ---" });
            dsNhanVien.AddRange(query);

            // Gán DataSource (Đảo thứ tự để fix lỗi)
            cbbTenNV.DisplayMember = "HoTen";
            cbbTenNV.ValueMember = "MaND";
            cbbTenNV.DataSource = dsNhanVien;
        }
        private void SetupDgvHoaDon()
        {
            dgvHoaDon.AutoGenerateColumns = false;
            dgvHoaDon.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHoaDon.MultiSelect = false;
            dgvHoaDon.ReadOnly = true;
            dgvHoaDon.AllowUserToAddRows = false;

            // Cột MaHD (ẩn đi)
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaHD",
                HeaderText = "Mã HĐ",
                DataPropertyName = "MaHD", // Phải khớp với tên thuộc tính của LINQ Select
                Visible = false
            });

            // Cột Ngày Lập
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayLap",
                HeaderText = "Ngày Lập",
                DataPropertyName = "NgayLap",
                Width = 150,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy HH:mm:ss" }
            });

            // Cột Tên Nhân Viên
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenNV",
                HeaderText = "Tên Nhân Viên",
                DataPropertyName = "TenNV",
                Width = 150
            });

            // Cột Tên Khách Hàng
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenKH",
                HeaderText = "Tên Khách Hàng",
                DataPropertyName = "TenKH",
                Width = 150
            });

            // Cột Tổng Tiền
            dgvHoaDon.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TongTienSauGiam",
                HeaderText = "Tổng Tiền",
                DataPropertyName = "TongTienSauGiam",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });

            // CỘT NÚT BẤM "XEM CHI TIẾT" (Theo yêu cầu)
            var colXemChiTiet = new DataGridViewButtonColumn
            {
                Name = "colXemChiTiet",
                HeaderText = "Chi Tiết",
                Text = "Xem chi tiết",
                UseColumnTextForButtonValue = true,
                Width = 100,
                FlatStyle = FlatStyle.Flat
            };
            colXemChiTiet.DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dgvHoaDon.Columns.Add(colXemChiTiet);
        }
        private void LoadHoaDonData(DateTime? tuNgay, DateTime? denNgay, string maNV)
        {
            // 1. Bắt đầu query
            IQueryable<HoaDon> query = db.HoaDons;

            // 2. Lọc theo ngày (nếu có)
            if (tuNgay.HasValue)
            {
                query = query.Where(hd => hd.NgayLap >= tuNgay.Value);
            }
            if (denNgay.HasValue)
            {
                // Phải lấy đến 23:59:59 của ngày kết thúc
                DateTime denNgayCuoiNgay = denNgay.Value.Date.AddDays(1).AddSeconds(-1);
                query = query.Where(hd => hd.NgayLap <= denNgayCuoiNgay);
            }

            // 3. Lọc theo nhân viên (nếu có)
            if (!string.IsNullOrEmpty(maNV) && maNV != "ALL")
            {
                query = query.Where(hd => hd.MaND == maNV);
            }

            // 4. Lấy kết quả và chiếu (Select) ra
            // (DataLoadOptions đã lo việc Join)
            var ketQua = query
                .OrderByDescending(hd => hd.NgayLap) // Mới nhất lên trên
                .Select(hd => new
                {
                    hd.MaHD,
                    hd.NgayLap,
                    TenNV = hd.NguoiDung.HoTen, // Lấy tên NV
                    // Xử lý khách vãng lai (MaKH có thể null)
                    TenKH = (hd.KhachHang == null ? "Khách vãng lai" : hd.KhachHang.TenKH),
                    hd.TongTienSauGiam
                })
                .ToList();

            dgvHoaDon.DataSource = ketQua;
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            dtpTuNgay.Value = DateTime.Now;
            dtpDenNgay.Value = DateTime.Now;
            if (cbbTenNV.Items.Count > 0)
                cbbTenNV.SelectedIndex = 0; // Chọn "Tất cả nhân viên"

            // Tải lại tất cả
            LoadHoaDonData(null, null, null);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvHoaDon_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // KIỂM TRA: Có phải người dùng đã click vào cột "colXemChiTiet" không?
            if (dgvHoaDon.Columns[e.ColumnIndex].Name == "colXemChiTiet")
            {
                // 1. Lấy MaHD từ cột "MaHD" (cột ẩn) của dòng ĐANG ĐƯỢC CLICK
                int maHD = (int)dgvHoaDon.Rows[e.RowIndex].Cells["MaHD"].Value;

                // 2. Mở form ChiTietHoaDon và truyền maHD qua
                XemChiTietHoaDon frmChiTiet = new XemChiTietHoaDon(maHD);
                frmChiTiet.ShowDialog();
            }
        }
    }
}
