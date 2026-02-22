-- Eski EgitimSeviyesiId sütununu kaldırma scripti
-- Many-to-Many ilişkiye geçtiğimiz için bu sütuna artık ihtiyacımız yok

USE WEB_PROJE;
GO

-- Önce foreign key constraint'i kaldır
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ErasmusProgramlari_EgitimSeviyeleri_EgitimSeviyesiId')
BEGIN
    ALTER TABLE ErasmusProgramlari DROP CONSTRAINT FK_ErasmusProgramlari_EgitimSeviyeleri_EgitimSeviyesiId;
    PRINT 'Eski Foreign Key kaldırıldı.';
END
ELSE
BEGIN
    PRINT 'Foreign Key bulunamadı (zaten kaldırılmış olabilir).';
END
GO

-- Index'i kaldır (eğer varsa)
IF EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ErasmusProgramlari_EgitimSeviyesiId')
BEGIN
    DROP INDEX IX_ErasmusProgramlari_EgitimSeviyesiId ON ErasmusProgramlari;
    PRINT 'Index kaldırıldı.';
END
GO

-- Eski EgitimSeviyesiId sütununu kaldır
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ErasmusProgramlari') AND name = 'EgitimSeviyesiId')
BEGIN
    ALTER TABLE ErasmusProgramlari DROP COLUMN EgitimSeviyesiId;
    PRINT 'Eski EgitimSeviyesiId sütunu başarıyla kaldırıldı.';
END
ELSE
BEGIN
    PRINT 'EgitimSeviyesiId sütunu bulunamadı (zaten kaldırılmış olabilir).';
END
GO

-- Kontrol et
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'ErasmusProgramlari'
ORDER BY ORDINAL_POSITION;
GO

PRINT 'Script başarıyla tamamlandı!';
GO

