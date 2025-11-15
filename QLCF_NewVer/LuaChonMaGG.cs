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
    public partial class LuaChonMaGG : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

        // 2. PROPERTIES (ĐỂ FORM BANHANG LẤY KẾT QUẢ)
        // =================================================================
        public int? SelectedMaVC { get; private set; } // Dùng int? (nullable)
        public string SelectedCode { get; private set; }

        // ----------------------------------------------------------------------
        // 3. KHỞI TẠO VÀ LOAD FORM
        // =================================================================
        public LuaChonMaGG()
        {
            InitializeComponent();
            db = new QLCF_NewVerDataContext();
            this.SelectedMaVC = null; // Khởi tạo là null
            this.SelectedCode = null;
        }

        private void LuaChonMaGG_Load(object sender, EventArgs e)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<Voucher>(v => v.KieuVC); // Load sẵn KieuVC
            db.LoadOptions = dlo;

            // Cài đặt các ComboBox và DataGridView
            LoadComboBoxLoai();
            SetupDgvMaGiamGia();

            // Tải dữ liệu lần đầu (chỉ tải các mã "Đang hoạt động")
            LoadDataGiamGia(null, null, true);
        }
        private void SetupDgvMaGiamGia()
        {
            dgvMaGiamGia.AutoGenerateColumns = false;
            dgvMaGiamGia.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMaGiamGia.MultiSelect = false;
            dgvMaGiamGia.ReadOnly = true;
            dgvMaGiamGia.AllowUserToAddRows = false;

            dgvMaGiamGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaVC",
                DataPropertyName = "MaVC",
                Visible = false
            });
            dgvMaGiamGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Code",
                HeaderText = "Mã Code",
                DataPropertyName = "Code",
                Width = 80
            });
            dgvMaGiamGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenMaGiamGia",
                HeaderText = "Tên",
                DataPropertyName = "TenMaGiamGia",
                Width = 150
            });
            dgvMaGiamGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TenLoai",
                HeaderText = "Loại",
                DataPropertyName = "TenLoai",
                Width = 100
            });
            dgvMaGiamGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GiaTri",
                HeaderText = "Giá trị",
                DataPropertyName = "GiaTri",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "N0" }
            });
            dgvMaGiamGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "NgayKT",
                HeaderText = "Ngày KT",
                DataPropertyName = "NgayKT",
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvMaGiamGia.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "TrangThai",
                HeaderText = "Trạng thái",
                DataPropertyName = "TrangThai",
                Width = 100
            });
        }
        private void LoadComboBoxLoai()
        {
            var dsLoaiVC = db.KieuVCs
                             .OrderBy(l => l.TenLoai)
                             .Select(l => new { l.MaLoaiVC, l.TenLoai })
                             .ToList();

            var displayList = new List<object>();
            displayList.Add(new { MaLoaiVC = 0, TenLoai = "--- Tất cả loại ---" });
            displayList.AddRange(dsLoaiVC);

            cbbLocDuLieu.DisplayMember = "TenLoai";
            cbbLocDuLieu.ValueMember = "MaLoaiVC";
            cbbLocDuLieu.DataSource = displayList;
        }
        private void LoadDataGiamGia(string keyword, int? maLoaiVC, bool chiHienThiHoatDong = false)
        {
            DateTime homNay = DateTime.Now.Date;

            // 1. Bắt đầu query
            IQueryable<Voucher> query = db.Vouchers;

            // 2. Lọc theo trạng thái (quan trọng)
            if (chiHienThiHoatDong)
            {
                // Chỉ lấy các mã đang trong thời gian áp dụng
                query = query.Where(v => v.NgayKT >= homNay && v.NgayBD <= homNay);
            }

            // 3. Lọc theo từ khóa (nếu có)
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.ToLower();
                query = query.Where(v => v.Code.ToLower().Contains(keyword) || v.TenMaGiamGia.ToLower().Contains(keyword));
            }

            // 4. Lọc theo loại voucher (nếu có)
            if (maLoaiVC.HasValue && maLoaiVC > 0)
            {
                query = query.Where(v => v.MaLoaiVC == maLoaiVC);
            }

            // 5. Join với KieuVC và Select ra kết quả
            // (Giống hệt code LoadData của bạn)
            var ketQua = query.Join(db.KieuVCs,
                                v => v.MaLoaiVC,
                                k => k.MaLoaiVC,
                                (v, k) => new
                                {
                                    v.MaVC,
                                    v.Code,
                                    v.TenMaGiamGia,
                                    k.TenLoai,
                                    v.GiaTri,
                                    v.DieuKien,
                                    v.NgayBD,
                                    v.NgayKT,
                                    // Tính toán trạng thái logic
                                    TrangThai = (v.NgayKT < homNay) ? "Đã hết hạn" :
                                                (v.NgayBD > homNay) ? "Chưa bắt đầu" : "Đang hoạt động"
                                })
                                .OrderBy(v => v.NgayKT) // Sắp xếp
                                .ToList();

            dgvMaGiamGia.DataSource = ketQua;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string keyword = txtTimKiem.Text;
            int? maLoai = null;

            if (cbbLocDuLieu.SelectedValue != null)
            {
                var selectedItem = cbbLocDuLieu.Items[cbbLocDuLieu.SelectedIndex];
                maLoai = (int)((dynamic)selectedItem).MaLoaiVC;
            }
            if (maLoai == 0) maLoai = null;

            // Tải lại (Bao gồm cả mã hết hạn)
            LoadDataGiamGia(keyword, maLoai, false);
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = "";
            cbbLocDuLieu.SelectedValue = 0;
            // Tải lại (Chỉ mã hoạt động)
            LoadDataGiamGia(null, null, true);
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnChonMaGG_Click(object sender, EventArgs e)
        {
            if (dgvMaGiamGia.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn một mã giảm giá từ danh sách!");
                return;
            }

            var selectedRow = dgvMaGiamGia.SelectedRows[0];
            string trangThai = selectedRow.Cells["TrangThai"].Value.ToString();

            // Check logic: Chỉ cho chọn mã "Đang hoạt động"
            if (trangThai != "Đang hoạt động")
            {
                MessageBox.Show("Mã giảm giá này đã hết hạn hoặc chưa bắt đầu. Vui lòng chọn mã khác!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Lấy thông tin
            this.SelectedMaVC = (int)selectedRow.Cells["MaVC"].Value;
            this.SelectedCode = selectedRow.Cells["Code"].Value.ToString();

            // Báo cho Form BanHang biết là đã OK
            this.DialogResult = DialogResult.OK;
            this.Close();
        }


    }
}
