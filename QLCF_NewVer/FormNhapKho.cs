using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace QLCF_NewVer
{
    public partial class FormNhapKho : Form
    {
        public class ItemInfo
        {
            public int IdSPKC { get; set; }     // <- add
            public string MaSP { get; set; }    // <- add
            public string TenSP { get; set; }   // <- add
            public string Size { get; set; }    // <- add

            // Legacy
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string Unit { get; set; }
            public decimal LastCost { get; set; }
            public string Display => $"{ItemName} ({ItemCode})"; // dùng hi?n th? trong combo
        }

        public class ImportLine
        {
            public string ItemCode { get; set; }
            public string ItemName { get; set; }
            public string Unit { get; set; }
            public int Amount { get; set; }
            public decimal UnitCost { get; set; }
            public decimal LineTotal => Amount * UnitCost;
        }

        // Bien form
        private QLCF_NewVerDataContext _db;
        private readonly BindingList<ImportLineVM> _lines = new BindingList<ImportLineVM>();

        public FormNhapKho()
        {
            InitializeComponent();

            StartPosition = FormStartPosition.CenterParent;
            dgvLines.ReadOnly = true;
            dgvLines.AllowUserToAddRows = false;
            dgvLines.AutoGenerateColumns = true;   // we’ll bind a projection so only 4 columns show
            dgvLines.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLines.MultiSelect = false;

            Load += FormNhapKho_Load;

            // Optional button names – wire them to your actual buttons
            btnAdd.Click += btnAdd_Click;
            btnRemove.Click += btnRemove_Click;
            btnSave.Click += btnSave_Click;
            btnClose.Click += (s, e) => Close();

            // Optional: enter presses Add
            txtAmount.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnAdd.PerformClick(); };
            txtCost.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnAdd.PerformClick(); };

            dgvLines.DataError += (s, e) => { e.ThrowException = false; };
        }

        private void FormNhapKho_Load(object sender, EventArgs e)
        {
            _db = new QLCF_NewVerDataContext(
                Properties.Settings.Default.QLCF_NewVerConnectionString
            );
            LoadSuppliers();
            SetupItemComboBehavior(); // option: autocomplete cho combo
            RefreshGrid();
            UpdateTotal();
        }

        // ===== Suppliers combo (txtSupplier) =====
        private void LoadSuppliers()
        {
            using (var db = new QLCF_NewVerDataContext())
            {
                var ncc = db.NhaCungCaps
                    .OrderBy(x => x.TenNCC)
                    .Select(x => new { x.MaNCC, x.TenNCC })
                    .ToList();

                txtSupplier.DisplayMember = "TenNCC";
                txtSupplier.ValueMember = "MaNCC";
                txtSupplier.DataSource = ncc;
            }

            // G?n s? ki?n ??i nhà cung c?p ? t? load s?n ph?m
            txtSupplier.SelectedIndexChanged -= txtSupplier_SelectedIndexChanged;
            txtSupplier.SelectedIndexChanged += txtSupplier_SelectedIndexChanged;

            // G?i luôn l?n ??u ?? load danh sách s?n ph?m ban ??u (n?u c?n)
            txtSupplier_SelectedIndexChanged(txtSupplier, EventArgs.Empty);
        }

        private void txtSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If supplier not ready, clear combo
            if (txtSupplier.SelectedValue == null)
            {
                BindItemsToCombo(new List<ItemInfo>());
                cboItem.Enabled = false;
                return;
            }

            var maNCC = txtSupplier.SelectedValue.ToString();

            using (var db = new QLCF_NewVerDataContext())
            {
                // Populate ItemInfo with identifying fields (IdSPKC, MaSP, TenSP, Size)
                var sp = db.SanPhamKichCos
                    .Select(v => new ItemInfo
                    {
                        IdSPKC = v.IdSPKC,
                        MaSP = v.MaSP,
                        TenSP = v.SanPham.TenSP,
                        Size = v.KichCo.KichCo1.ToString(),
                        ItemCode = v.MaSP,
                        ItemName = v.SanPham.TenSP + " - " + v.KichCo.KichCo1,
                        Unit = v.KichCo.KichCo1.ToString(),
                        LastCost = 0 // DB currently doesn't have cost column
                    })
                    .OrderBy(x => x.ItemName)
                    .ToList();

                // Bind to combo so items appear
                BindItemsToCombo(sp);

                // UX: enable only if there are items and reset selection
                cboItem.Enabled = sp.Any();
                cboItem.SelectedIndex = -1;
        }
        }

        private void BindItemsToCombo(List<ItemInfo> items)
        {
            // L?u ý: DataSource tr?c ti?p là list ItemInfo ?? truy c?p ?? tr??ng
            cboItem.DataSource = items;
            cboItem.DisplayMember = "Display";   // "Tên hàng (Mã)"
            cboItem.ValueMember = "IdSPKC";  // Id is most reliable if you need ValueMember
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // 1) L?y bi?n th? ?ang ch?n trong combo
            var variant = GetSelectedVariantFromCombo(); // returns ItemInfo (IdSPKC, MaSP, TenSP, Size)
            if (variant == null)
            {
                MessageBox.Show("Hãy ch?n s?n ph?m/bi?n th? t? danh sách.");
                cboItem.DroppedDown = true;
                return;
            }

            // 2) S? l??ng - accept "1", "1.000", "1,000" etc.
            var rawAmt = (txtAmount.Text ?? "").Trim();
            rawAmt = rawAmt.Replace("\u00A0", ""); // NBSP
            rawAmt = rawAmt.Replace(" ", "");
            rawAmt = rawAmt.Replace(".", "");
            rawAmt = rawAmt.Replace(",", "");
            if (!int.TryParse(rawAmt, NumberStyles.Integer, CultureInfo.InvariantCulture, out int qty) || qty <= 0)
            {
                MessageBox.Show("S? l??ng ph?i là s? nguyên d??ng.");
                txtAmount.Focus();
                return;
            }

            // 3) ??n giá (ch?p nh?n 12.345,67; 12345.67; 12345)
            if (!TryParseDecimalVN((txtCost.Text ?? "").Trim(), out decimal giaNhap) || giaNhap < 0)
            {
                MessageBox.Show("Giá nh?p không h?p l?.");
                txtCost.Focus();
                return;
            }

            // 4) G?p dòng n?u cùng bi?n th? + cùng giá
            var existed = _lines.FirstOrDefault(l => l.IdSPKC == variant.IdSPKC && l.GiaNhap == giaNhap);
            if (existed == null)
            {
                _lines.Add(new ImportLineVM
                {
                    IdSPKC = variant.IdSPKC,
                    MaSP = variant.MaSP,
                    TenSP = variant.TenSP,
                    Size = variant.Size,
                    SoLuong = qty,
                    GiaNhap = giaNhap
                });
            }
            else
            {
                existed.SoLuong += qty;
                existed.OnChanged(nameof(ImportLineVM.SoLuong));
                existed.OnChanged(nameof(ImportLineVM.ThanhTien));
            }

            RefreshGrid();
            UpdateTotal();

            // 5) Convenience
            txtAmount.Clear();
            txtAmount.Focus();
        }
        private bool TryParseDecimalVN(string s, out decimal value)
        {
            value = 0;
            if (string.IsNullOrWhiteSpace(s)) return false;

            // normalize spaces
            var t = s.Replace("\u00A0", "").Trim();

            // If contains both '.' and ',' assume '.' thousands and ',' decimal -> remove '.' and replace ',' with '.'
            if (t.Contains(".") && t.Contains(","))
            {
                t = t.Replace(".", "").Replace(",", ".");
            }
            else if (t.Contains(",") && !t.Contains("."))
            {
                // Vietnamese style: comma as decimal separator -> convert to invariant
                t = t.Replace(",", ".");
            }
            else
            {
                // leave as-is (invariant-style)
            }

            t = t.Replace(" ", "");

            return decimal.TryParse(t, NumberStyles.Number, CultureInfo.InvariantCulture, out value);
        }

        private ItemInfo GetSelectedVariantFromCombo()
        {
            // Return typed ItemInfo (we populate IdSPKC, MaSP, TenSP, Size when binding)
            return cboItem.SelectedItem as ItemInfo;
        }

        private class VariantPick { public int IdSPKC; public string MaSP; public string TenSP; public string Size; }

        private VariantPick FindVariantByInput(string input)
        {
            // Supports: "CF01" (first size) or "CF01-L" (exact size match)
            string ma = input;
            string size = null;

            var dashIdx = input.IndexOf('-');
            if (dashIdx > 0 && dashIdx < input.Length - 1)
            {
                ma = input.Substring(0, dashIdx).Trim();
                size = input.Substring(dashIdx + 1).Trim();
            }

            using (var db = new QLCF_NewVerDataContext())
            {
                var q = db.SanPhamKichCos
                    .Where(v => v.TrangThaiSP && v.MaSP == ma)
                    .Select(v => new VariantPick
                    {
                        IdSPKC = v.IdSPKC,
                        MaSP = v.MaSP,
                        TenSP = v.SanPham.TenSP,
                        Size = v.KichCo.KichCo1.ToString()
                    });

                if (!string.IsNullOrEmpty(size))
                {
                    q = q.Where(x => x.Size == size);
                    var exact = q.FirstOrDefault();
                    if (exact != null) return exact;
                    return null; // requested size not found
                }

                // If no size specified, pick the first variant (or choose your own rule)
                return q.OrderBy(x => x.Size).FirstOrDefault();
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (dgvLines.CurrentRow == null) return;
            var ma = dgvLines.CurrentRow.Cells["MaSP"]?.Value?.ToString();
            var size = dgvLines.CurrentRow.Cells["Size"]?.Value?.ToString();
            var target = _lines.FirstOrDefault(x => x.MaSP == ma && x.Size == size);
            if (target != null) _lines.Remove(target);

            RefreshGrid();
            UpdateTotal();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (txtSupplier.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng ch?n nhà cung c?p."); return;
            }
            if (_lines.Count == 0)
            {
                MessageBox.Show("Danh sách nh?p tr?ng."); return;
            }

            int maNcc = (int)txtSupplier.SelectedValue;

            try
            {
                using (var db = new QLCF_NewVerDataContext())
                {
                    // Parent NhapKho
                    var nk = new NhapKho { MaNCC = maNcc, NgayNhap = DateTime.Now };
                    db.NhapKhos.InsertOnSubmit(nk);

                    // Preload variants we’ll update
                    var ids = _lines.Select(l => l.IdSPKC).Distinct().ToList();
                    var variants = db.SanPhamKichCos.Where(v => ids.Contains(v.IdSPKC)).ToList();

                    foreach (var l in _lines)
                    {
                        db.ChiTietNhapKhos.InsertOnSubmit(new ChiTietNhapKho
                        {
                            NhapKho = nk,                 // FK via navigation
                            IdSPKC = l.IdSPKC,
                            SoLuongNhap = l.SoLuong,
                            GiaNhap = l.GiaNhap,
                            ThanhTien = l.GiaNhap * l.SoLuong
                        });

                        var v = variants.Single(x => x.IdSPKC == l.IdSPKC);
                        v.SoLuongTon = v.SoLuongTon + l.SoLuong; // strictly via entity + SubmitChanges
                    }

                    db.SubmitChanges();
                }

                MessageBox.Show("?ã l?u phi?u nh?p.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("L?i l?u nh?p kho: " + ex.Message, "L?i", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void RefreshGrid()
        {
            var view = _lines
            .Select(x => new {
                x.MaSP,
                x.TenSP,
                x.Size,
                x.SoLuong,
                GiaNhap = x.GiaNhap.ToString("N0"),
                ThanhTien = (x.SoLuong * x.GiaNhap).ToString("N0")
            })
            .ToList();

            dgvLines.AutoGenerateColumns = true;
            dgvLines.DataSource = null;
            dgvLines.DataSource = view;
        }

        public class ImportLineVM : INotifyPropertyChanged
        {
            public int IdSPKC { get; set; }     // variant PK (SanPhamKichCo)
            public string MaSP { get; set; }
            public string TenSP { get; set; }
            public string Size { get; set; }
            public int SoLuong { get; set; }
            public decimal GiaNhap { get; set; }
            public decimal ThanhTien => GiaNhap * SoLuong;

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
        }

        private void UpdateTotal()
        {
            txtTotal.Text = _lines.Sum(x => x.GiaNhap * x.SoLuong).ToString("0,0.00");
        }

        private void cboItem_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cboItem.SelectedItem is ItemInfo it)
            {
                if (string.IsNullOrWhiteSpace(txtCost.Text) && it.LastCost > 0)
                    txtCost.Text = it.LastCost.ToString("0");

                // N?u b?n có label ??n v? (vd. lblUnit) thì:
                // lblUnit.Text = it.Unit ?? "";
            }
        }

        private void SetupItemComboBehavior()
        {
            // If you want typing lookup:
            // cboItem.DropDownStyle = ComboBoxStyle.DropDown;
            // cboItem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            // cboItem.AutoCompleteSource = AutoCompleteSource.ListItems;

            cboItem.SelectedIndexChanged -= cboItem_SelectedIndexChanged;
            cboItem.SelectedIndexChanged += cboItem_SelectedIndexChanged;
        }
    }

}