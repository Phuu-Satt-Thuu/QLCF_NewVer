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
    public partial class FormLichSuNhap : Form
    {

        private QLCF_NewVerDataContext db;

        public FormLichSuNhap()
        {
            InitializeComponent();

            this.Load += FormLichSuNhap_Load;
            btnReload.Click += (s, e) => RefreshGrid();
            cbSupplier.SelectedIndexChanged += (s, e) => RefreshGrid();
            chkUseDate.CheckedChanged += (s, e) => RefreshGrid();
            dtFrom.ValueChanged += (s, e) => { if (chkUseDate.Checked) RefreshGrid(); };
            dtTo.ValueChanged += (s, e) => { if (chkUseDate.Checked) RefreshGrid(); };
            txtSearch.TextChanged += (s, e) => RefreshGrid();
            btnClear.Click += (s, e) => ClearHistory();

        }
        public class ImportHistoryRow
        {
            public int MaNK { get; set; }
            public DateTime NgayNhap { get; set; }
            public int MaCTNK { get; set; }

            public int MaNCC { get; set; }
            public string TenNCC { get; set; }

            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public string KichCo { get; set; }

            public int SoLuongNhap { get; set; }
            public decimal GiaNhap { get; set; }
            public decimal ThanhTien { get; set; }
        }

        private void FormLichSuNhap_Load(object sender, EventArgs e)
        {
            db = new QLCF_NewVerDataContext(); // tên DataContext trùng với .dbml của bạn
            InitControls();
            RefreshGrid();
        }

        private void InitControls()
        {
            // Mặc định lọc theo tháng hiện tại
            var now = DateTime.Now;
            dtFrom.Value = new DateTime(now.Year, now.Month, 1);
            dtTo.Value = dtFrom.Value.AddMonths(1).AddDays(-1);
            chkUseDate.Checked = true;

            // Load nhà cung cấp
            var nccList = db.NhaCungCaps
                            .OrderBy(n => n.TenNCC)
                            .Select(n => new { n.MaNCC, n.TenNCC })
                            .ToList();

            cbSupplier.DisplayMember = "TenNCC";
            cbSupplier.ValueMember = "MaNCC";

            // Thêm "Tất cả"
            cbSupplier.Items.Clear();
            cbSupplier.Items.Add(new { MaNCC = 0, TenNCC = "— Tất cả nhà cung cấp —" });
            foreach (var n in nccList) cbSupplier.Items.Add(n);
            cbSupplier.SelectedIndex = 0;

            // DataGridView basic
            dgv.AutoGenerateColumns = true;   // hoặc tạo cột tay để format
            dgv.ReadOnly = true;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void RefreshGrid()
        {
            // Base query (lambda joins)
            var q = db.ChiTietNhapKhos
                      .Join(db.NhapKhos,
                            ct => ct.MaNK,
                            nk => nk.MaNK,
                            (ct, nk) => new { ct, nk })
                      .Join(db.NhaCungCaps,
                            t => t.nk.MaNCC,
                            ncc => ncc.MaNCC,
                            (t, ncc) => new { t.ct, t.nk, ncc })
                      .Join(db.SanPhamKichCos,
                            t => t.ct.IdSPKC,
                            spkc => spkc.IdSPKC,
                            (t, spkc) => new { t.ct, t.nk, t.ncc, spkc })
                      .Join(db.SanPhams,
                            t => t.spkc.MaSP,
                            sp => sp.MaSP,
                            (t, sp) => new { t.ct, t.nk, t.ncc, t.spkc, sp })
                      .Join(db.KichCos,
                            t => t.spkc.MaKichCo,
                            kc => kc.MaKichCo,
                            (t, kc) => new ImportHistoryRow
                            {
                                MaNK = t.nk.MaNK,
                                NgayNhap = t.nk.NgayNhap,
                                MaCTNK = t.ct.MaCTNK,

                                MaNCC = t.nk.MaNCC,
                                TenNCC = t.ncc.TenNCC,

                                MaSP = t.sp.MaSP,
                                TenSP = t.sp.TenSP,
                                KichCo = kc.KichCo1.ToString(),

                                SoLuongNhap = t.ct.SoLuongNhap,
                                GiaNhap = t.ct.GiaNhap,
                                ThanhTien = t.ct.ThanhTien ?? (t.ct.SoLuongNhap * t.ct.GiaNhap)
                            });

            // Lọc theo ngày (tùy chọn)
            if (chkUseDate.Checked)
            {
                var from = dtFrom.Value.Date;
                var to = dtTo.Value.Date.AddDays(1); // < to for inclusive day
                q = q.Where(r => r.NgayNhap >= from && r.NgayNhap < to);
            }

            // Lọc theo NCC
            var sel = cbSupplier.SelectedItem;
            int nccId = 0;
            if (sel != null)
            {
                // dynamic vì Items có thể là anonymous
                dynamic d = sel;
                nccId = (int)d.MaNCC;
            }
            if (nccId > 0)
            {
                q = q.Where(r => r.MaNCC == nccId);
            }

            // Tìm kiếm theo mã / tên SP
            var key = (txtSearch.Text ?? "").Trim();
            if (!string.IsNullOrEmpty(key))
            {
                q = q.Where(r => r.MaSP.Contains(key) || r.TenSP.Contains(key));
            }

            // Sắp xếp mới nhất lên đầu
            var list = q.OrderByDescending(r => r.NgayNhap)
                        .ThenByDescending(r => r.MaNK)
                        .ThenByDescending(r => r.MaCTNK)
                        .ToList();

            dgv.DataSource = list;

            // Tổng hợp nhanh
            var totalRows = list.Count;
            var totalQty = list.Sum(r => (int?)r.SoLuongNhap) ?? 0;
            var totalAmount = list.Sum(r => (decimal?)r.ThanhTien) ?? 0m;

            lblSummary.Text = $"Dòng: {totalRows} | Tổng SL: {totalQty} | Tổng tiền: {totalAmount:N0} đ";
        }

        private void ClearHistory()
        {
            if (MessageBox.Show(
                "Bạn có chắc chắn muốn xóa toàn bộ lịch sử nhập kho không?\n" +
                "Thao tác này không thể hoàn tác!",
                "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    // Xóa toàn bộ chi tiết trước (do ràng buộc khóa ngoại)
                    db.ChiTietNhapKhos.DeleteAllOnSubmit(db.ChiTietNhapKhos);

                    // Sau đó xóa bảng NhậpKho
                    db.NhapKhos.DeleteAllOnSubmit(db.NhapKhos);

                    db.SubmitChanges();

                    MessageBox.Show("Đã xóa toàn bộ lịch sử nhập kho.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    RefreshGrid(); // Cập nhật lại bảng hiển thị
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi xóa dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
