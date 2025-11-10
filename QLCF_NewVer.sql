CREATE DATABASE QLCF_NewVer
GO

USE QLCF_NewVer
GO

-- 2. BẢNG NGƯỜI DÙNG (NHÂN VIÊN/ADMIN)
CREATE TABLE NguoiDung
(
    MaND VARCHAR(10) NOT NULL, 
    TaiKhoan VARCHAR(50) NOT NULL, 
    MatKhau VARCHAR(200) NOT NULL, -- Nên lưu SHA256
    Luong DECIMAL(18,2) NULL,
    HoTen NVARCHAR(100) NOT NULL,
    ViTri NVARCHAR(20) NOT NULL,
    SDT VARCHAR(20) NULL,
    DiaChi NVARCHAR(100) NULL,
    Email VARCHAR(50) NULL,
    NganHang VARCHAR(10) NULL,
    STK VARCHAR(20) NULL,
    NgaySinh DATE NULL,
	TrangThai BIT NOT NULL DEFAULT 1,

    CONSTRAINT PK_NguoiDung PRIMARY KEY (MaND),
    CONSTRAINT UNQ_TaiKhoan UNIQUE (TaiKhoan),
    CONSTRAINT CK_ViTri CHECK (ViTri IN ('Admin', 'NhanVien')),
    CONSTRAINT CK_NganHang CHECK (NganHang IN ('VCB', 'MB', 'AGR', 'OCB', 'SCB'))
);
GO

-- 3. BẢNG CHẤM CÔNG
CREATE TABLE ChamCong
(
    Id INT IDENTITY(1,1) NOT NULL,
    MaND VARCHAR(10) NOT NULL, 
    Ngay DATE NOT NULL, 
    GioBatDau DATETIME NOT NULL, 
    GioKetThuc DATETIME NULL, 
    -- Tự động tính tổng thời gian làm (theo phút)
    TongThoiGian AS DATEDIFF(minute, GioBatDau, GioKetThuc) PERSISTED, 

    CONSTRAINT PK_ChamCong PRIMARY KEY (Id),
    CONSTRAINT FK_ChamCong_NguoiDung FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND) ON DELETE CASCADE
);
GO 

-- 4. BẢNG KHÁCH HÀNG
CREATE TABLE KhachHang 
(
    MaKH INT IDENTITY(1,1) NOT NULL,
    TenKH NVARCHAR(50) NOT NULL,
    SDT VARCHAR(20) NOT NULL,
    TichDiem INT NOT NULL DEFAULT 0,
	DiaChi NVARCHAR(100) NULL,
	NgayTao DATE NOT NULL DEFAULT GETDATE(),

    CONSTRAINT PK_KhachHang PRIMARY KEY (MaKH),
    CONSTRAINT UNQ_SDT UNIQUE (SDT)
);
GO

-- 5. BẢNG LOẠI SẢN PHẨM (Danh mục)
CREATE TABLE LoaiSP
(
    MaLoai INT IDENTITY(1,1) NOT NULL,
    TenLoai NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_LoaiSP PRIMARY KEY (MaLoai)
);
GO

-- 6. BẢNG KÍCH CỠ (S, M, L)
CREATE TABLE KichCo
(
    MaKichCo INT IDENTITY(1,1) NOT NULL,
    KichCo CHAR(1) NOT NULL,
    CONSTRAINT PK_KichCo PRIMARY KEY (MaKichCo),
    CONSTRAINT CK_KichCo_Values CHECK (KichCo IN ('S', 'M', 'L'))
);
GO

-- 7. BẢNG SẢN PHẨM (Thông tin chung)
CREATE TABLE SanPham
(
    MaSP VARCHAR(10) NOT NULL,
    TenSP NVARCHAR(50) NOT NULL,
    MaLoai INT NOT NULL,
    DuongDanAnh NVARCHAR(255) NULL,

    CONSTRAINT PK_SanPham PRIMARY KEY (MaSP),
    CONSTRAINT FK_SanPham_LoaiSP FOREIGN KEY (MaLoai) REFERENCES LoaiSP(MaLoai) ON DELETE CASCADE
);
GO

-- 8. BẢNG CHI TIẾT SẢN PHẨM (Theo kích cỡ, giá, tồn kho)
CREATE TABLE SanPhamKichCo
(
    IdSPKC INT IDENTITY(1,1) NOT NULL,
    MaSP VARCHAR(10) NOT NULL,
    MaKichCo INT NOT NULL,
    GiaBan DECIMAL(18,2) NOT NULL,
    SoLuongTon INT NOT NULL DEFAULT 0,
    CanhBaoTonKho INT NOT NULL,
    TrangThaiSP BIT NOT NULL DEFAULT 1, -- 1: Đang bán, 0: Ngừng bán

    CONSTRAINT PK_SanPhamKichCo PRIMARY KEY (IdSPKC),
    CONSTRAINT UQ_SanPham_KichCo UNIQUE (MaSP, MaKichCo),
    CONSTRAINT FK_SPKC_SanPham FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP) ON DELETE CASCADE,
    CONSTRAINT FK_SPKC_KichCo FOREIGN KEY (MaKichCo) REFERENCES KichCo(MaKichCo) ON DELETE CASCADE,
    CONSTRAINT CK_SPKC_GiaBan CHECK (GiaBan >= 0),
    CONSTRAINT CK_SPKC_SoLuongTon CHECK (SoLuongTon >= 0)
);
GO

-- 9. BẢNG KIỂU VOUCHER
CREATE TABLE KieuVC
(
    MaLoaiVC INT IDENTITY(1,1) NOT NULL,
    TenLoai NVARCHAR(50) NOT NULL,
    CONSTRAINT PK_KieuVC PRIMARY KEY (MaLoaiVC)
);
GO

-- 10. BẢNG VOUCHER
CREATE TABLE Voucher
(
    MaVC INT IDENTITY(1,1) NOT NULL,
    TenMaGiamGia NVARCHAR(100) NULL, 
    Code NVARCHAR(50) NOT NULL,
    GiaTri DECIMAL(18,2) NOT NULL, -- 10 (cho 10%) hoặc 10000 (cho 10k VNĐ)
    NgayBD DATE NOT NULL,
    NgayKT DATE NOT NULL,
    DieuKien DECIMAL(18,2) NULL, -- Giá trị tối thiểu của hóa đơn
    MaLoaiVC INT NOT NULL, -- Liên kết với loại voucher
    MaLoai INT NULL, -- Áp dụng cho loại sản phẩm nào (nếu có)

    CONSTRAINT PK_Voucher PRIMARY KEY (MaVC),
    CONSTRAINT UNQ_Code UNIQUE (Code),
    CONSTRAINT FK_Voucher_KieuVC FOREIGN KEY (MaLoaiVC) REFERENCES KieuVC(MaLoaiVC), 
    CONSTRAINT FK_Voucher_LoaiSP FOREIGN KEY (MaLoai) REFERENCES LoaiSP(MaLoai) ON DELETE CASCADE,
    CONSTRAINT CK_Voucher_GiaTri -- Logic kiểm tra giá trị của bạn
        CHECK (
            (MaLoaiVC IN (1, 3) AND GiaTri > 0) OR -- Giảm % hoặc Giảm tiền
            (MaLoaiVC IN (2, 4) AND GiaTri = 0)  -- Mua 1 Tặng 1
        ),
    CONSTRAINT CK_Voucher_Ngay CHECK (NgayKT >= NgayBD)
);
GO

-- 11. BẢNG CHI TIẾT VOUCHER (Áp dụng cho sản phẩm nào, vd: Mua 1 tặng 1)
CREATE TABLE ChiTietVC
(
    ID INT IDENTITY(1,1) NOT NULL,
    MaVC INT NOT NULL,
    IdSPKC INT NOT NULL, -- Sản phẩm được tặng
    CONSTRAINT PK_ChiTietVC PRIMARY KEY (ID),
    CONSTRAINT FK_ChiTietVC_Voucher FOREIGN KEY (MaVC) REFERENCES Voucher(MaVC) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietVC_SPKC FOREIGN KEY (IdSPKC) REFERENCES SanPhamKichCo(IdSPKC) ON DELETE NO ACTION,
    CONSTRAINT UQ_ChiTietVC UNIQUE (MaVC, IdSPKC)
);
GO

-- 12. BẢNG HÓA ĐƠN
CREATE TABLE HoaDon
(
    MaHD INT IDENTITY(1,1) NOT NULL,
    NgayLap DATETIME NOT NULL DEFAULT GETDATE(),
    MaKH INT NULL,
    MaND VARCHAR(10) NOT NULL,
    TongTienGoc DECIMAL(18,2) NOT NULL DEFAULT 0,
    TienGiam DECIMAL(18,2) NOT NULL DEFAULT 0,
    TongTienSauGiam AS (TongTienGoc - TienGiam) PERSISTED, -- Cột tự động tính

    CONSTRAINT PK_HoaDon PRIMARY KEY (MaHD),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH) ON DELETE SET NULL, -- Giữ HĐ dù xóa KH
    CONSTRAINT FK_HoaDon_NguoiDung FOREIGN KEY (MaND) REFERENCES NguoiDung(MaND) ON DELETE NO ACTION -- Giữ HĐ dù xóa NV
);
GO

-- 13. BẢNG ÁP DỤNG MÃ VOUCHER (Lịch sử)
CREATE TABLE ApMaVC
(
    MaAP INT IDENTITY(1,1) NOT NULL,
    MaVC INT NOT NULL,
    MaHD INT NOT NULL,

    CONSTRAINT PK_ApMaVC PRIMARY KEY (MaAP),
    CONSTRAINT FK_ApMaVC_Voucher FOREIGN KEY (MaVC) REFERENCES Voucher(MaVC) ON DELETE NO ACTION, -- Giữ lịch sử
    CONSTRAINT FK_ApMaVC_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    CONSTRAINT UNQ_Voucher_HoaDon UNIQUE (MaVC, MaHD)
);
GO

-- 14. BẢNG CHI TIẾT HÓA ĐƠN
CREATE TABLE ChiTietHD 
(
    MaHD INT NOT NULL,
    IdSPKC INT NOT NULL,
    SoLuong INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL, -- Giá tại thời điểm bán
    IsTang BIT NOT NULL DEFAULT 0, -- 0: Hàng bán, 1: Hàng tặng (cho BOGO)
    ThanhTien AS (SoLuong * DonGia) PERSISTED,

    CONSTRAINT PK_ChiTietHD PRIMARY KEY (MaHD, IdSPKC, IsTang), -- Khóa chính composite
    CONSTRAINT FK_ChiTietHD_SPKC FOREIGN KEY (IdSPKC) REFERENCES SanPhamKichCo(IdSPKC) ON DELETE NO ACTION, -- Giữ HĐ
    CONSTRAINT FK_ChiTietHD_HoaDon FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    CONSTRAINT CK_ChiTietHD_SoLuong CHECK (SoLuong >= 0),
    CONSTRAINT CK_ChiTietHD_DonGia CHECK (DonGia >= 0)
);
GO

-- 15. BẢNG NHÀ CUNG CẤP
CREATE TABLE NhaCungCap
(
    MaNCC INT IDENTITY(1,1) NOT NULL,
    TenNCC NVARCHAR(50) NOT NULL, 
    CONSTRAINT PK_NhaCungCap PRIMARY KEY (MaNCC)
);
GO

-- 16. BẢNG NHẬP KHO
CREATE TABLE NhapKho
(
    MaNK INT IDENTITY(1,1) NOT NULL,
    MaNCC INT NOT NULL,
    NgayNhap DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_NhapKho PRIMARY KEY (MaNK),
    CONSTRAINT FK_NhapKho_NhaCungCap FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC)
);
GO

-- 17. BẢNG CHI TIẾT NHẬP KHO
CREATE TABLE ChiTietNhapKho
(
    MaCTNK INT IDENTITY(1,1) NOT NULL,
    MaNK INT NOT NULL,
    IdSPKC INT NOT NULL,
    SoLuongNhap INT NOT NULL,
    GiaNhap DECIMAL(18,2) NOT NULL,
    ThanhTien AS (SoLuongNhap * GiaNhap) PERSISTED,

    CONSTRAINT PK_ChiTietNhapKho PRIMARY KEY (MaCTNK),
    CONSTRAINT FK_ChiTietNK_NhapKho FOREIGN KEY (MaNK) REFERENCES NhapKho(MaNK) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietNK_SPKC FOREIGN KEY (IdSPKC) REFERENCES SanPhamKichCo(IdSPKC) ON DELETE CASCADE,
    CONSTRAINT CK_ChiTietNK_SoLuongNhap CHECK (SoLuongNhap >= 0),
    CONSTRAINT CK_ChiTietNK_GiaNhap CHECK (GiaNhap >= 0)
);
GO

-- 18. BẢNG XUẤT KHO (Dùng cho hủy hàng, hàng hỏng,...)
CREATE TABLE XuatKho
(
    MaXK INT IDENTITY(1,1) NOT NULL,
    NgayXuat DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT PK_XuatKho PRIMARY KEY (MaXK)
);
GO

-- 19. BẢNG CHI TIẾT XUẤT KHO
CREATE TABLE ChiTietXuatKho
(
    MaCTXK INT IDENTITY(1,1) NOT NULL,
    MaXK INT NOT NULL,
    IdSPKC INT NOT NULL,
    SoLuongXuat INT NOT NULL,
    
    CONSTRAINT PK_ChiTietXuatKho PRIMARY KEY (MaCTXK),
    CONSTRAINT FK_ChiTietXK_XuatKho FOREIGN KEY (MaXK) REFERENCES XuatKho(MaXK) ON DELETE CASCADE,
    CONSTRAINT FK_ChiTietXK_SPKC FOREIGN KEY (IdSPKC) REFERENCES SanPhamKichCo(IdSPKC) ON DELETE NO ACTION,
    CONSTRAINT CK_ChiTietXK_SoLuongXuat CHECK (SoLuongXuat >= 0)
);
GO

-- -----------------------------------------------------------------
-- PHẦN THÊM DỮ LIỆU MẪU (ĐÃ CẬP NHẬT TÊN BẢNG VÀ CỘT)
-- -----------------------------------------------------------------

INSERT INTO LoaiSP (TenLoai) VALUES
 (N'Cà phê'),
 (N'Trà'),
 (N'Freeze'),
 (N'Bánh'),
 (N'Khác');
GO

INSERT INTO KichCo (KichCo) VALUES
 (N'S'),
 (N'M'),
 (N'L');
GO

INSERT INTO SanPham (MaSP, TenSP, MaLoai, DuongDanAnh) VALUES
 ('SP001', N'Espresso', 1, N'Images/products/espresso.jpg'),
 ('SP002', N'Americano', 1, N'Images/products/americano.jpg'),
 ('SP003', N'Latte', 1, N'Images/products/latte.jpg'),
 ('SP004', N'Cappuccino', 1, N'Images/products/cappuccino.jpg'),
 ('SP005', N'Trà Oolong', 2, N'Images/products/traoolong.jpg'),
 ('SP006', N'Trà Đen', 2, N'Images/products/traden.jpg'),
 ('SP007', N'Trà Xanh', 2, N'Images/products/traxanh.jpg'),
 ('SP008', N'Matcha Freeze', 3, N'Images/products/matchafreeze.jpg'),
 ('SP009', N'Chocolate Freeze', 3, N'Images/products/chocolatefreeze.jpg'),
 ('SP010', N'Fruit Freeze', 3, N'Images/products/fruitfreeze.jpg'),
 ('SP011', N'Bánh Muffin', 4, N'Images/products/muffin.jpg'),
 ('SP012', N'Bánh Croissant', 4, N'Images/products/croissants.jpg'),
 ('SP013', N'Nước Suối', 5, N'Images/products/nuocsuoi.jpg'),
 ('SP014', N'Sữa Tươi', 5, N'Images/products/suatuoi.jpg');
GO

INSERT INTO SanPhamKichCo (MaSP, MaKichCo, GiaBan, SoLuongTon, CanhBaoTonKho)
VALUES
    ('SP001', 1, 25000.00, 60, 10),
    ('SP001', 2, 30000.00, 50, 10),
    ('SP001', 3, 35000.00, 30, 10),
    ('SP002', 1, 22000.00, 40, 10),
    ('SP002', 2, 27000.00, 35, 10),
    ('SP002', 3, 32000.00, 20, 10),
    ('SP003', 1, 28000.00, 45, 10),
    ('SP003', 2, 33000.00, 30, 10),
    ('SP003', 3, 38000.00, 18, 10),
    ('SP004', 1, 27000.00, 40, 10),
    ('SP004', 2, 32000.00, 28, 10),
    ('SP004', 3, 37000.00, 15, 10),
    ('SP005', 1, 20000.00, 50, 10),
    ('SP005', 2, 25000.00, 40, 10),
    ('SP005', 3, 30000.00, 20, 10),
    ('SP006', 2, 23000.00, 35, 10),
    ('SP007', 1, 18000.00, 60, 10),
    ('SP007', 2, 22000.00, 45, 10),
    ('SP008', 2, 45000.00, 25, 10),
    ('SP009', 2, 43000.00, 22, 10),
    ('SP010', 2, 42000.00, 20, 10),
    ('SP011', 1, 45000.00, 20, 20),
    ('SP011', 2, 50000.00, 12, 20),
    ('SP012', 1, 38000.00, 18, 10),
    ('SP012', 2, 42000.00, 10, 10),
    ('SP013', 1, 10000.00, 100, 10),
    ('SP014', 1, 28000.00, 40, 10);
GO

INSERT INTO NguoiDung (MaND, TaiKhoan, MatKhau, Luong, HoTen, ViTri, SDT, DiaChi, Email, NganHang, STK, NgaySinh) VALUES
 ('AD01', 'admin', 'eb61e866596795300e8ff64f7fc0b3459b9a43f3b087f51393f7641123d33972', 25000000.00, N'Quản Trị Viên', N'Admin', '0912345678', N'1 Đường A, Q1', 'admin@qlcf.vn', 'VCB', '123456789', '1985-05-15'), -- mk Adm!n2025
 ('NV01', 'nv_phuc', '3f0986d4259373d7944304792d52729956d22aab93b7c192255522fe2da95d4d', 7000000.00, N'Nguyễn Văn Phúc', N'NhanVien', '0987654321', N'12 Đường B, Q3', 'phuc@qlcf.vn', 'MB', '987654321', '1995-08-20'), -- mk Nv!12345
 ('NV02', 'nv_mai', '7063ae93ec51a0128e726f8f04e849ba56e3ddb21c573ceff853b2d6fae166ca', 7500000.00, N'Trần Thị Mai', N'NhanVien', '0977888999', N'45 Đường C, Q5', 'mai@qlcf.vn', 'OCB', '555666777', '1994-02-10'); --mk Nv!54321
GO

INSERT INTO KhachHang (TenKH, SDT, TichDiem, DiaChi) VALUES
(N'Nguyễn Văn A', '0911000111', 30, N'123 Đường 1, Q.Bình Thạnh'),
(N'Trần Thị B', '0922000222', 50, N'456 Đường 2, Q.Phú Nhuận'),
(N'Phạm Văn C', '0933000333', 10, N'789 Đường 3, Q.1');
GO

INSERT INTO KieuVC (TenLoai) VALUES
 (N'Giảm %'),
 (N'Mua X Tặng Y - Cùng loại'),
 (N'Giảm giá trị thực'),
 (N'Mua X Tặng Y - Khác loại');
GO

INSERT INTO Voucher (Code, GiaTri, NgayBD, NgayKT, DieuKien, MaLoaiVC, MaLoai) VALUES
 (N'TEA10P', 10.00, '2025-06-01', '2025-12-31', 0.00, 1, 2); -- 10% cho trà (MaLoai = 2)
GO

-- Thêm Hóa đơn. 
-- TongTienGoc và TienGiam sẽ do ứng dụng tính toán và INSERT vào
-- TongTienSauGiam sẽ tự động = 85000
INSERT INTO HoaDon (MaKH, MaND, TongTienGoc, TienGiam) VALUES (1, 'NV01', 95000.00, 10000.00);
-- TongTienSauGiam sẽ tự động = 95000
INSERT INTO HoaDon (MaKH, MaND, TongTienGoc, TienGiam) VALUES (2, 'NV02', 95000.00, 0.00);
GO

-- Áp mã giảm giá cho hóa đơn 1
INSERT INTO ApMaVC (MaVC, MaHD) VALUES (1, 1);
GO

-- Thêm chi tiết cho hóa đơn
-- (MaHD, IdSPKC, SoLuong, DonGia, IsTang)
INSERT INTO ChiTietHD (MaHD, IdSPKC, SoLuong, DonGia, IsTang)
VALUES
 (1, 2, 2, 30000.00, 0), -- 2x Espresso(M) = 60000
 (1, 14, 1, 25000.00, 0), -- 1x Trà Oolong(M) = 25000. (Tổng 85k? -> HĐ 1 Gốc 85k, Giảm 0?)
                         -- User data: HD1 TongTien 85k. 
                         -- User data: CTHD (1,2,2,30k), (1,14,1,25k). 60k + 25k = 85k.
                         -- OK, vậy HĐ 1 gốc 85k, giảm 0. Tôi sẽ sửa INSERT HĐ.
 (2, 19, 1, 45000.00, 0), -- 1x Matcha Freeze(M) = 45k
 (2, 23, 1, 50000.00, 0); -- 1x Bánh Muffin(M) = 50k (Tổng 95k. OK)
GO

-- Sửa lại INSERT Hóa đơn cho khớp với Chi Tiết Hóa Đơn
UPDATE HoaDon SET TongTienGoc = 85000, TienGiam = 0 WHERE MaHD = 1;
UPDATE HoaDon SET TongTienGoc = 95000, TienGiam = 0 WHERE MaHD = 2;
GO
 
INSERT INTO NhaCungCap (TenNCC) 
VALUES 
 (N'Công ty Cung Ứng ABC'), 
 (N'Công ty ZYCX');
GO

PRINT 'Tạo CSDL QLCF và chèn dữ liệu mẫu thành công!';
GO