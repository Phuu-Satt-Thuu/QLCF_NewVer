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
    public partial class QuanLyKho : Form
    {
        public QuanLyKho()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
            Load += QuanLyKho_Load;
            dgvKho.RowPrePaint += dgvKho_RowPrePaint;     // for coloring

            // search / filter wiring
            btnTimKiem.Click += btnTimKiem_Click;
            txtTimKiem.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) ApplyFilterSearch(); };
            cbbLocDuLieu.SelectedIndexChanged += (s, e) => ApplyFilterSearch();

            // refresh button
            btnLamMoi.Click += btnLamMoi_Click;
        }

        private void QuanLyKho_Load(object sender, EventArgs e)
        {
            SetupFilterCombo();
            LoadData();
        }

        private void SetupFilterCombo()
        {
            // Fill filter combo with common stock filters (Vietnamese labels)
            cbbLocDuLieu.Items.Clear();
            cbbLocDuLieu.Items.Add("Tất cả");
            cbbLocDuLieu.Items.Add("Hết hàng");       // SoLuongTon == 0
            cbbLocDuLieu.Items.Add("Sắp hết");        // SoLuongTon < CanhBaoTonKho
            cbbLocDuLieu.Items.Add("Còn hàng");       // SoLuongTon >= CanhBaoTonKho
            cbbLocDuLieu.SelectedIndex = 0;
        }

        private void LoadData()
        {
            using (var db = new QLCF_NewVerDataContext())
            {
                var data = db.SanPhamKichCos
                    .Where(v => v.TrangThaiSP) // optional
                    .Select(v => new
                    {
                        v.IdSPKC,
                        v.MaSP,
                        TenSP = v.SanPham.TenSP,
                        Size = v.KichCo.KichCo1,
                        v.SoLuongTon,
                        v.CanhBaoTonKho
                    })
                    .OrderBy(x => x.TenSP).ThenBy(x => x.Size)
                    .ToList();

                dgvKho.AutoGenerateColumns = true;   // or define columns manually
                dgvKho.DataSource = data;
            }
        }

        private void ApplyFilterSearch()
        {
            // Read source (full list) from current DataSource
            var full = (dgvKho.DataSource as IEnumerable<dynamic>)?.ToList();
            if (full == null)
            {
                // fallback: reload and re-run
                LoadData();
                full = (dgvKho.DataSource as IEnumerable<dynamic>)?.ToList();
                if (full == null) return;
            }

            var filter = (cbbLocDuLieu.SelectedItem ?? "Tất cả").ToString();
            var search = (txtTimKiem.Text ?? "").Trim();

            IEnumerable<dynamic> q = full;

            // Apply stock state filters
            if (!string.Equals(filter, "Tất cả", StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(filter, "Hết hàng", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(x => Convert.ToInt32(x.SoLuongTon) == 0);
                }
                else if (string.Equals(filter, "Sắp hết", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(x =>
                    {
                        int qty = Convert.ToInt32(x.SoLuongTon);
                        int warn = x.CanhBaoTonKho == null ? 0 : Convert.ToInt32(x.CanhBaoTonKho);
                        return qty > 0 && qty < warn;
                    });
                }
                else if (string.Equals(filter, "Còn hàng", StringComparison.OrdinalIgnoreCase))
                {
                    q = q.Where(x =>
                    {
                        int qty = Convert.ToInt32(x.SoLuongTon);
                        int warn = x.CanhBaoTonKho == null ? 0 : Convert.ToInt32(x.CanhBaoTonKho);
                        return qty >= warn;
                    });
                }
            }

            // Text search: MaSP, TenSP, Size
            if (!string.IsNullOrEmpty(search))
            {
                var low = search.ToLowerInvariant();
                q = q.Where(x =>
                {
                    var ma = (x.MaSP ?? "").ToString();
                    var ten = (x.TenSP ?? "").ToString();
                    var size = (x.Size ?? "").ToString();
                    return ma.IndexOf(low, StringComparison.OrdinalIgnoreCase) >= 0
                        || ten.IndexOf(low, StringComparison.OrdinalIgnoreCase) >= 0
                        || size.IndexOf(low, StringComparison.OrdinalIgnoreCase) >= 0;
                });
            }

            var result = q.OrderBy(x => x.TenSP).ThenBy(x => x.Size).ToList();
            dgvKho.AutoGenerateColumns = true;
            dgvKho.DataSource = result;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            ApplyFilterSearch();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            // reset UI and reload data
            SetupFilterCombo();
            txtTimKiem.Clear();
            LoadData();
        }

        private void dgvKho_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvKho.Rows[e.RowIndex];
            if (row.DataBoundItem == null) return;

            // Make sure your grid has these columns bound (SoLuongTon, CanhBaoTonKho)
            int qty = Convert.ToInt32(row.Cells["SoLuongTon"].Value);
            int warn = row.Cells["CanhBaoTonKho"]?.Value is null
                ? 10
                : Convert.ToInt32(row.Cells["CanhBaoTonKho"].Value);

            if (qty == 0)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            }
            else if (qty < warn)
            {
                row.DefaultCellStyle.BackColor = System.Drawing.Color.Khaki;
                row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                row.DefaultCellStyle.BackColor = dgvKho.DefaultCellStyle.BackColor;
                row.DefaultCellStyle.ForeColor = dgvKho.DefaultCellStyle.ForeColor;
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void cbbLocDuLieu_SelectedIndexChanged(object sender, EventArgs e)
        {
            // kept for designer wiring; actual behavior handled by constructor wiring
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnNhapKho_Click(object sender, EventArgs e)
        {
            using (var f = new FormNhapKho())
                if (f.ShowDialog(this) == DialogResult.OK)
                    LoadData(); // call your grid reload in QuanLyKho to reflect new stock
        }

        private void btnKhoLog_Click(object sender, EventArgs e)
        {
            using (var f = new FormLichSuNhap())
            {
                f.StartPosition = FormStartPosition.CenterParent;
                f.ShowDialog(this);
            }
        }
    }
}
