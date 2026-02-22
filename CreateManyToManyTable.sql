-- Many-to-Many ilişki için ara tablo oluşturma scripti
-- Bu scripti SQL Server Management Studio'da veya SQL Server'da çalıştırın

USE WEB_PROJE;
GO

-- Önce eski EgitimSeviyesiId sütununu ve foreign key'i kaldır (eğer varsa)
-- NOT: Bu işlem mevcut verileri etkileyebilir, önce yedek alın!

-- Foreign key constraint'i kontrol et ve kaldır
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ErasmusProgramlari_EgitimSeviyeleri_EgitimSeviyesiId')
BEGIN
    ALTER TABLE ErasmusProgramlari DROP CONSTRAINT FK_ErasmusProgramlari_EgitimSeviyeleri_EgitimSeviyesiId;
    PRINT 'Eski Foreign Key kaldırıldı.';
END
GO

-- Eski EgitimSeviyesiId sütununu kaldır (eğer varsa)
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ErasmusProgramlari') AND name = 'EgitimSeviyesiId')
BEGIN
    -- Önce mevcut verileri yeni tabloya taşıyalım (opsiyonel - eğer veri varsa)
    -- Bu kısım isteğe bağlı, eğer mevcut verileri korumak istiyorsanız açın
    
    -- INSERT INTO ErasmusProgramiEgitimSeviyesi (ErasmusId, EgitimSeviyesiId)
    -- SELECT ErasmusId, EgitimSeviyesiId FROM ErasmusProgramlari WHERE EgitimSeviyesiId IS NOT NULL;
    
    ALTER TABLE ErasmusProgramlari DROP COLUMN EgitimSeviyesiId;
    PRINT 'Eski EgitimSeviyesiId sütunu kaldırıldı.';
END
GO

-- Yeni Many-to-Many ara tablosunu oluştur
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ErasmusProgramiEgitimSeviyesi')
BEGIN
    CREATE TABLE ErasmusProgramiEgitimSeviyesi (
        ErasmusId INT NOT NULL,
        EgitimSeviyesiId INT NOT NULL,
        CONSTRAINT PK_ErasmusProgramiEgitimSeviyesi PRIMARY KEY (ErasmusId, EgitimSeviyesiId),
        CONSTRAINT FK_ErasmusProgramiEgitimSeviyesi_ErasmusProgrami 
            FOREIGN KEY (ErasmusId) REFERENCES ErasmusProgramlari(ErasmusId) ON DELETE CASCADE,
        CONSTRAINT FK_ErasmusProgramiEgitimSeviyesi_EgitimSeviyesi 
            FOREIGN KEY (EgitimSeviyesiId) REFERENCES EgitimSeviyeleri(EgitimSeviyesiId) ON DELETE CASCADE
    );
    
    -- Index ekle (performans için)
    CREATE INDEX IX_ErasmusProgramiEgitimSeviyesi_ErasmusId ON ErasmusProgramiEgitimSeviyesi(ErasmusId);
    CREATE INDEX IX_ErasmusProgramiEgitimSeviyesi_EgitimSeviyesiId ON ErasmusProgramiEgitimSeviyesi(EgitimSeviyesiId);
    
    PRINT 'ErasmusProgramiEgitimSeviyesi tablosu başarıyla oluşturuldu.';
END
ELSE
BEGIN
    PRINT 'ErasmusProgramiEgitimSeviyesi tablosu zaten mevcut.';
END
GO

-- Mevcut verileri kontrol et
SELECT 
    'ErasmusProgramlari' AS Tablo,
    COUNT(*) AS KayitSayisi
FROM ErasmusProgramlari
UNION ALL
SELECT 
    'ErasmusProgramiEgitimSeviyesi' AS Tablo,
    COUNT(*) AS KayitSayisi
FROM ErasmusProgramiEgitimSeviyesi;
GO

PRINT 'Script başarıyla tamamlandı!';
GO

