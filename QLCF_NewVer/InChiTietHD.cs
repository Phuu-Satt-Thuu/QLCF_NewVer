using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// THÊM 2 THƯ VIỆN NÀY VÀO
using Microsoft.Reporting.WinForms;
using System.Linq;

namespace QLCF_NewVer
{
    public partial class InChiTietHD : Form
    {
        private int _maHD;

        // 2. Sửa hàm khởi tạo (Constructor)
        // Nó sẽ nhận MaHD khi được gọi
        public InChiTietHD(int maHoaDon)
        {
            InitializeComponent();
            _maHD = maHoaDon; // Lưu MaHD lại
        }
        
        private void InChiTietHD_Load(object sender, EventArgs e)
        {
            try
            {
                QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();

                // LẤY DỮ LIỆU TỪ VIEW (dùng _maHD đã lưu)
                var thongTinHD = db.VW_ThongTinHoaDons
                                   .Where(hd => hd.MaHD == _maHD)
                                   .ToList();

                var chiTietHD = db.VW_ChiTietHoaDons
                                  .Where(ct => ct.MaHD == _maHD)
                                  .ToList();

                // NẠP DỮ LIỆU VÀO REPORT
                this.reportViewer1.LocalReport.DataSources.Clear();

                ReportDataSource rdsHoaDon = new ReportDataSource("VW_ThongTinHoaDon", thongTinHD);
                ReportDataSource rdsChiTiet = new ReportDataSource("VW_ChiTietHoaDon", chiTietHD);

                this.reportViewer1.LocalReport.DataSources.Add(rdsHoaDon);
                this.reportViewer1.LocalReport.DataSources.Add(rdsChiTiet);

                // Report này không có Parameter, nên không cần set
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải báo cáo: " + ex.Message);
            }

            // Dòng này phải nằm cuối cùng
            this.reportViewer1.RefreshReport();
        }
    }
}
