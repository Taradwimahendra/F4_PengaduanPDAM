CREATE DATABASE DBPengaduanPDAM;
GO
USE DBPengaduanPDAM;
GO

-- =========================================================
-- 1. PEMBUATAN TABEL UTAMA
-- =========================================================
CREATE TABLE UserLogin (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    NamaLengkap VARCHAR(100) NOT NULL,
    Email NVARCHAR(50) UNIQUE NOT NULL, 
    Password VARCHAR(255) NOT NULL,     
    NoTelepon VARCHAR(15) CHECK (LEN(NoTelepon) BETWEEN 10 AND 13),
    Alamat VARCHAR(200),
    RoleUser VARCHAR(20) CHECK (RoleUser IN ('admin', 'pelanggan')) DEFAULT 'pelanggan',
    TanggalRegistrasi DATETIME DEFAULT GETDATE()
);

CREATE TABLE KategoriPengaduan (
    KategoriID INT IDENTITY(1,1) PRIMARY KEY,
    NamaKategori VARCHAR(100) NOT NULL,
    Deskripsi VARCHAR(255) NOT NULL
);

CREATE TABLE Pengaduan (
    PengaduanID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT NOT NULL, 
    KategoriID INT NOT NULL,
    Judul_Laporan VARCHAR(100),
    Deskripsi_Laporan VARCHAR(500) NOT NULL,
    Tanggal_Pengaduan DATETIME DEFAULT GETDATE(),
    StatusPengaduan VARCHAR(20) CHECK (StatusPengaduan IN ('menunggu', 'diproses', 'selesai', 'ditolak')) DEFAULT 'menunggu',
    CONSTRAINT FK_Pengaduan_User FOREIGN KEY (UserID) REFERENCES UserLogin(UserID),
    CONSTRAINT FK_Pengaduan_Kategori FOREIGN KEY (KategoriID) REFERENCES KategoriPengaduan(KategoriID)
);

CREATE TABLE Lampiran (
    LampiranID INT IDENTITY(1,1) PRIMARY KEY,
    PengaduanID INT NOT NULL,
    NamaFile VARCHAR(255),
    PathFile VARCHAR(255),
    CONSTRAINT FK_Lampiran_Pengaduan FOREIGN KEY (PengaduanID) REFERENCES Pengaduan(PengaduanID)
);

CREATE TABLE RiwayatStatus (
    RiwayatID INT IDENTITY(1,1) PRIMARY KEY,
    PengaduanID INT NOT NULL, 
    StatusPengaduan VARCHAR(20) CHECK (StatusPengaduan IN ('diproses', 'selesai', 'ditolak')),
    Keterangan VARCHAR(255) NOT NULL, 
    TanggalPerubahan DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Riwayat_Pengaduan FOREIGN KEY (PengaduanID) REFERENCES Pengaduan(PengaduanID)
);

CREATE TABLE LogError (
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    waktu DATETIME DEFAULT GETDATE(),
    pesan_error VARCHAR(MAX)
);

CREATE TABLE LogAktivitas (
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    aktivitas VARCHAR(100),
    waktu DATETIME DEFAULT GETDATE()
);

CREATE TABLE LogKeamanan (
    id_log INT IDENTITY(1,1) PRIMARY KEY,
    aktivitas VARCHAR(200),
    jumlah_data INT,
    waktu DATETIME DEFAULT GETDATE()
);
GO


CREATE TRIGGER trg_InsertPengaduan
ON Pengaduan
AFTER INSERT
AS
BEGIN
    INSERT INTO LogAktivitas (aktivitas, waktu)
    VALUES ('Tambah data pengaduan', GETDATE());
END;
GO

select* from LogAktivitas 

CREATE TRIGGER trg_DeletePengaduan
ON Pengaduan
AFTER DELETE
AS
BEGIN
    INSERT INTO LogAktivitas (aktivitas, waktu)
    VALUES ('Hapus data pengaduan', GETDATE());
END;
GO

CREATE TRIGGER trg_PreventMassUpdatePengaduan
ON Pengaduan
AFTER UPDATE
AS
BEGIN
    DECLARE @jumlah INT;
    SELECT @jumlah = COUNT(*) FROM inserted;

    IF @jumlah > 5
    BEGIN
        INSERT INTO LogKeamanan (aktivitas, jumlah_data, waktu)
        VALUES ('WARNING: Update massal terdeteksi', @jumlah, GETDATE());
        
        ROLLBACK TRANSACTION;
        RAISERROR('Update dibatalkan! Terlalu banyak data diubah sekaligus.', 16, 1);
    END
END;
GO

-- =========================================================
-- 4. PEMBUATAN VIEW
-- =========================================================
CREATE VIEW View_LaporanPengaduan AS
SELECT 
    p.PengaduanID,
    p.Tanggal_Pengaduan,
    u.NamaLengkap AS Nama_Pelapor,    
    u.NoTelepon,      
    u.Alamat,
    k.NamaKategori,   
    p.Judul_Laporan,
    p.Deskripsi_Laporan,
    p.StatusPengaduan
FROM Pengaduan p
JOIN UserLogin u ON p.UserID = u.UserID
JOIN KategoriPengaduan
k ON p.KategoriID = k.KategoriID;
GO

-- =========================================================
-- 5. PEMBUATAN STORED PROCEDURE (DILENGKAPI TRY...CATCH)
-- =========================================================

-- A. SP Login User
CREATE PROCEDURE sp_LoginUser
    @Email NVARCHAR(50),
    @Password VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DECLARE @UserID INT, @RoleUser VARCHAR(20), @NamaLengkap VARCHAR(100);

        SELECT @UserID = UserID, @RoleUser = RoleUser, @NamaLengkap = NamaLengkap
        FROM UserLogin
        WHERE Email = @Email AND Password = @Password;

        IF @UserID IS NULL
        BEGIN
            RAISERROR('Login Gagal! Email atau Password yang Anda masukkan salah.', 16, 1);
        END
        ELSE
        BEGIN
            SELECT UserID, NamaLengkap, RoleUser FROM UserLogin WHERE UserID = @UserID;
        END
    END TRY
    BEGIN CATCH
        INSERT INTO LogError (waktu, pesan_error) VALUES (GETDATE(), ERROR_MESSAGE());
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO

-- B. SP Select All Pengaduan
CREATE PROCEDURE sp_GetAllPengaduan
 AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM View_LaporanPengaduan;
END;
GO

-- C. SP Select Pengaduan By UserID
CREATE PROCEDURE sp_GetPengaduanByUserID
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT 
        PengaduanID, Tanggal_Pengaduan, NamaKategori, 
        Judul_Laporan, Deskripsi_Laporan, StatusPengaduan
    FROM Pengaduan p
    JOIN KategoriPengaduan k ON p.KategoriID = k.KategoriID
    WHERE p.UserID = @UserID;
END;
GO

-- D. SP Insert Pengaduan
CREATE PROCEDURE sp_InsertPengaduan
    @UserID INT,
    @KategoriID INT,
    @Judul_Laporan VARCHAR(100),
    @Deskripsi_Laporan VARCHAR(500)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        IF @Judul_Laporan = '' OR @Deskripsi_Laporan = ''
        BEGIN
            RAISERROR('Judul atau deskripsi laporan tidak boleh kosong!', 16, 1);
        END

        INSERT INTO Pengaduan (UserID, KategoriID, Judul_Laporan, Deskripsi_Laporan, Tanggal_Pengaduan, StatusPengaduan)
        VALUES (@UserID, @KategoriID, @Judul_Laporan, @Deskripsi_Laporan, GETDATE(), 'menunggu');
    END TRY
    BEGIN CATCH
        INSERT INTO LogError (waktu, pesan_error) VALUES (GETDATE(), ERROR_MESSAGE());
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO

-- E. SP Update Status Pengaduan
CREATE PROCEDURE sp_UpdateStatusPengaduan
    @PengaduanID INT,
    @StatusBaru VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        UPDATE Pengaduan
        SET StatusPengaduan = @StatusBaru
        WHERE PengaduanID = @PengaduanID;
    END TRY
    BEGIN CATCH
        INSERT INTO LogError (waktu, pesan_error) VALUES (GETDATE(), ERROR_MESSAGE());
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO

-- F. SP Delete Pengaduan
CREATE PROCEDURE sp_DeletePengaduan
    @PengaduanID INT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        DELETE FROM Pengaduan
        WHERE PengaduanID = @PengaduanID;
    END TRY
    BEGIN CATCH
        INSERT INTO LogError (waktu, pesan_error) VALUES (GETDATE(), ERROR_MESSAGE());
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
    END CATCH
END;
GO

-- G. SP Count Total Pengaduan
CREATE PROCEDURE sp_CountTotalPengaduan
    @Total INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT @Total = COUNT(*) FROM Pengaduan;
END;
GO

-

-- =========================================================
-- 6. INSERT DATA DUMMY UNTUK TESTING
-- =========================================================
INSERT INTO UserLogin (NamaLengkap, Email, Password, NoTelepon, Alamat, RoleUser) 
VALUES 
('Sekar Mayang', 'sekar@email.com', 'pass123', '081234567890', 'Sleman, DIY', 'pelanggan'),
('Admin PDAM', 'admin@pdam.com', 'admin123', '081122334455', 'Kantor Pusat', 'admin');

INSERT INTO KategoriPengaduan (NamaKategori,Deskripsi) 
VALUES 
('Teknis', 'Kerusakan pipa atau air mati'),
('Administrasi', 'Masalah tagihan meteran');

-- Trigger Insert akan otomatis mengisi tabel LogAktivitas di background setelah baris ini dieksekusi
INSERT INTO Pengaduan (UserID, KategoriID, Judul_Laporan, Deskripsi_Laporan) 
VALUES (1, 1, 'Air Keruh', 'Air di rumah saya berwarna coklat sejak pagi.');
GO

ALTER TABLE Pengaduan 
ADD Foto VARBINARY(MAX);



SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Lampiran'



ALTER PROCEDURE sp_FilterRekapPengaduan
    @Dari   DATE,
    @Sampai DATE,
    @Status VARCHAR(20) = ''
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        PengaduanID,
        Tanggal_Pengaduan,
        Nama_Pelapor,       -- bukan alias "Pelapor"
        NoTelepon,          -- bukan alias "No Telepon"
        Alamat,
        NamaKategori,       -- bukan alias "Kategori"
        Judul_Laporan,      -- bukan alias "Judul"
        Deskripsi_Laporan,  -- bukan alias "Deskripsi"
        StatusPengaduan     -- bukan alias "Status"
    FROM View_LaporanPengaduan
    WHERE 
        CAST(Tanggal_Pengaduan AS DATE) >= @Dari
        AND CAST(Tanggal_Pengaduan AS DATE) <= @Sampai
        AND (@Status = '' OR @Status = 'semua' OR StatusPengaduan = @Status)
    ORDER BY Tanggal_Pengaduan DESC;
END;
GO


-- Cek definisi SP yang sekarang aktif di database
USE DBPengaduanPDAM;
GO

CREATE PROCEDURE sp_FilterRekapPengaduan  
    @Dari   DATE,  
    @Sampai DATE,  
    @Status VARCHAR(20) = ''  
AS  
BEGIN  
    SET NOCOUNT ON;  
  
    SELECT   
        PengaduanID,  
        Tanggal_Pengaduan,  
        Nama_Pelapor,
        NoTelepon,
        Alamat,  
        NamaKategori,
        Judul_Laporan,
        Deskripsi_Laporan,
        StatusPengaduan
    FROM View_LaporanPengaduan  
    WHERE   
        CAST(Tanggal_Pengaduan AS DATE) >= @Dari  
        AND CAST(Tanggal_Pengaduan AS DATE) <= @Sampai  
        AND (@Status = '' OR @Status = 'semua' OR StatusPengaduan = @Status)  
    ORDER BY Tanggal_Pengaduan DESC;  
END;
GO


-- Harus muncul 1 baris hasilnya
SELECT name, create_date FROM sys.procedures 
WHERE name = 'sp_FilterRekapPengaduan';

-- Ganti tanggal sesuai range data dummy yang ada
EXEC sp_FilterRekapPengaduan 
    @Dari = '2024-01-01', 
    @Sampai = '2026-12-31', 
    @Status = '';
	

SELECT * FROM View_LaporanPengaduan;
select * from UserLogin;


USE DBPengaduanPDAM;
SELECT TOP 10 id_log, aktivitas, waktu
FROM LogAktivitas
ORDER BY waktu DESC;