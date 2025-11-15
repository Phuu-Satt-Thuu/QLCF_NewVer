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
    public partial class LichSuChamCong : Form
    {
        QLCF_NewVerDataContext db = new QLCF_NewVerDataContext();
        public LichSuChamCong()
        {
            InitializeComponent();
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void LichSuChamCong_Load(object sender, EventArgs e)
        {
            DataLoadOptions dlo = new DataLoadOptions();
            dlo.LoadWith<ChamCong>(cc => cc.NguoiDung);
            db.LoadOptions = dlo;

            // 2. Cài đặt DataGridView
            SetupDgvLichSuChamCong();

            // 3. Tải dữ liệu lần đầu (tất cả)
            LoadLichSuData(null, null);
        }
        private void SetupDgvLichSuChamCong()
        {
            dgvLichSuChamCong.AutoGenerateColumns = false;
            dgvLichSuChamCong.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLichSuChamCong.MultiSelect = false;
            dgvLichSuChamCong.ReadOnly = true;
            dgvLichSuChamCong.AllowUserToAddRows = false;
            dgvLichSuChamCong.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvLichSuChamCong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "MaND",
                HeaderText = "Mã NV",
                DataPropertyName = "MaND",
                FillWeight = 10
            });
            dgvLichSuChamCong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "HoTen",
                HeaderText = "Tên Nhân Viên",
                DataPropertyName = "HoTen",
                FillWeight = 20
            });
            dgvLichSuChamCong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Ngay",
                HeaderText = "Ngày Chấm",
                DataPropertyName = "Ngay",
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }
            });
            dgvLichSuChamCong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GioBatDau",
                HeaderText = "Giờ Bắt Đầu",
                DataPropertyName = "GioBatDau",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss" }
            });
            dgvLichSuChamCong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "GioKetThuc",
                HeaderText = "Giờ Kết Thúc",
                DataPropertyName = "GioKetThuc",
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "HH:mm:ss" }
            });
            dgvLichSuChamCong.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "ThoiGianLamTron",
                HeaderText = "Tổng Thời Gian",
                DataPropertyName = "ThoiGianLamTron",
                FillWeight = 15
            });
        }
        private void LoadLichSuData(DateTime? tuNgay, DateTime? denNgay)
        {
            try
            {
                // 1. Bắt đầu query
                IQueryable<ChamCong> query = db.ChamCongs;

                // 2. Lọc theo ngày (nếu có)
                if (tuNgay.HasValue)
                {
                    query = query.Where(cc => cc.Ngay >= tuNgay.Value.Date);
                }
                if (denNgay.HasValue)
                {
                    DateTime denNgayCuoiNgay = denNgay.Value.Date.AddDays(1).AddSeconds(-1);
                    query = query.Where(cc => cc.Ngay <= denNgayCuoiNgay);
                }

                // 3. Lấy kết quả và chiếu (Select) ra
                // (DataLoadOptions đã lo việc Join)
                var ketQua = query
                    .OrderByDescending(cc => cc.Ngay) // Mới nhất lên trên
                    .Select(cc => new
                    {
                        cc.MaND,
                        HoTen = cc.NguoiDung.HoTen,
                        cc.Ngay,
                        cc.GioBatDau,
                        cc.GioKetThuc,
                        // Áp dụng logic làm tròn của bạn
                        ThoiGianLamTron = TinhThoiGianLamTron(cc.TongThoiGian)
                    })
                    .ToList();

                dgvLichSuChamCong.DataSource = ketQua;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải dữ liệu: " + ex.Message);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            DateTime tuNgay = dtpTuNgay.Value.Date;
            DateTime denNgay = dtpDenNgay.Value.Date;

            LoadLichSuData(tuNgay, denNgay);
        }
        private string TinhThoiGianLamTron(int? tongPhut)
        {
            if (tongPhut == null)
            {
                return "Chưa kết thúc";
            }

            // 1. Lấy tổng số phút (ví dụ: 285 phút)
            int phut = tongPhut.Value;

            // 2. Chia cho 30 (ví dụ: 285 / 30 = 9.5)
            double soLan = (double)phut / 30;

            // 3. Làm tròn xuống (ví dụ: Math.Floor(9.5) = 9)
            double soLanLamTron = Math.Floor(soLan);

            // 4. Nhân lại (ví dụ: 9 * 30 = 270 phút)
            int phutLamTron = (int)(soLanLamTron * 30);

            // 5. Chuyển đổi về Giờ và Phút (ví dụ: 270 / 60 = 4 giờ)
            int gio = phutLamTron / 60;
            // (ví dụ: 270 % 60 = 30 phút)
            int phutConLai = phutLamTron % 60;

            return $"{gio} giờ {phutConLai} phút";
        }
    }
}
