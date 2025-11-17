using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Reporting.WinForms;
using System.Linq;

namespace QLCF_NewVer
{
    public partial class InLichSuNhapKho : Form
    {
        private DateTime _tuNgay;
        private DateTime _denNgay;
        private int _maNCC; // 0 nghĩa là "Tất cả"
        private string _tenNCC; // Tên để hiển thị
        public InLichSuNhapKho(DateTime tuNgay, DateTime denNgay, int maNhaCungCap, string tenNhaCungCap)
        {
            InitializeComponent();
            _tuNgay = tuNgay;
            _denNgay = denNgay;
            _maNCC = maNhaCungCap;
            _tenNCC = tenNhaCungCap; // Ví dụ: "Tất cả nhà cung cấp"
        }

        private void InLichSuNhapKho_Load(object sender, EventArgs e)
        {
            try
            {
                QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

                // 3. LỌC DỮ LIỆU BẰNG LINQ
                // Lọc theo ngày (chắc chắn phải có)
                var query = db.VW_LichSuNhapKhos
                              .Where(nk => nk.NgayNhap >= _tuNgay && nk.NgayNhap <= _denNgay);

                // Lọc thêm theo Nhà Cung Cấp (nếu _maNCC > 0)
                if (_maNCC > 0)
                {
                    query = query.Where(nk => nk.MaNCC == _maNCC);
                }

                var lichSuNhap = query.OrderByDescending(nk => nk.NgayNhap).ToList();

                // 4. NẠP DỮ LIỆU VÀO REPORT
                this.reportViewer1.LocalReport.DataSources.Clear();
                ReportDataSource rds = new ReportDataSource("VW_LichSuNhapKho", lichSuNhap);
                this.reportViewer1.LocalReport.DataSources.Add(rds);

                // 5. NẠP 3 PARAMETERS VÀO REPORT
                ReportParameter p1 = new ReportParameter("pTuNgay", _tuNgay.ToString("dd/MM/yyyy"));
                ReportParameter p2 = new ReportParameter("pDenNgay", _denNgay.ToString("dd/MM/yyyy"));
                ReportParameter p3 = new ReportParameter("pNhaCungCap", _tenNCC);

                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { p1, p2, p3 });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message);
            }

            this.reportViewer1.RefreshReport(); // Dòng này phải ở cuố
        }
    }
}
