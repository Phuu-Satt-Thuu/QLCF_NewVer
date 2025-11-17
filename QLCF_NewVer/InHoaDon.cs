using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
// Thêm 2 thư viện này
using Microsoft.Reporting.WinForms;
using QLCF_NewVer; // Để dùng DataContext

namespace QLCF_NewVer
{
    public partial class InHoaDon : Form
    {
        public InHoaDon()
        {
            InitializeComponent();
        }
        public void HienThiHoaDon(int maHoaDon, decimal tienKhachDua, decimal tienThoiLai)
        {
            try
            {
                // 1. TẠO 1 DATACONTEXT MỚI (KHÔNG DÙNG LẠI db TỪ FORM BÁN HÀNG)
                // Điều này đảm bảo dữ liệu được lấy MỚI NHẤT từ database
                QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

                // 2. LẤY DỮ LIỆU TỪ VIEW BẰNG LINQ
                // Lấy thông tin chung (dùng .ToList() để thực thi query)
                var thongTinHD = db.VW_ThongTinHoaDons
                                   .Where(hd => hd.MaHD == maHoaDon)
                                   .ToList();

                // Lấy chi tiết hóa đơn (dùng .ToList() để thực thi query)
                var chiTietHD = db.VW_ChiTietHoaDons
                                  .Where(ct => ct.MaHD == maHoaDon)
                                  .ToList();

                // 3. XÓA DỮ LIỆU CŨ VÀ NẠP DỮ LIỆU MỚI
                this.reportViewer1.LocalReport.DataSources.Clear();

                // 4. TẠO REPORT DATASOURCE
                // Tên phải khớp 100% với tên DataSet trong Report Data của file .rdlc
                ReportDataSource rdsHoaDon = new ReportDataSource("VW_ThongTinHoaDon", thongTinHD);
                ReportDataSource rdsChiTiet = new ReportDataSource("VW_ChiTietHoaDon", chiTietHD);

                // 5. THÊM DATASOURCE VÀO REPORT
                this.reportViewer1.LocalReport.DataSources.Add(rdsHoaDon);
                this.reportViewer1.LocalReport.DataSources.Add(rdsChiTiet);

                // 6. NẠP PARAMETERS (Tiền khách đưa, Tiền thối)
                ReportParameter p1 = new ReportParameter("pTienKhachDua", tienKhachDua.ToString("N0"));
                ReportParameter p2 = new ReportParameter("pTienThoiLai", tienThoiLai.ToString("N0"));

                this.reportViewer1.LocalReport.SetParameters(new ReportParameter[] { p1, p2 });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message);
            }

            // 7. HIỂN THỊ REPORT
            this.reportViewer1.RefreshReport();
        }

        private void InHoaDon_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }
    }
}
