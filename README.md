# 🌍 Erasmus Koordinatörlüğü Yönetim Sistemi

Bu proje, **Isparta Uygulamalı Bilimler Üniversitesi (ISUBÜ)** Erasmus Koordinatörlüğü için geliştirilmiş kapsamlı bir web uygulamasıdır.  

Öğrencilerin anlaşmalı üniversiteleri ve Erasmus programlarını kolayca inceleyip yorum yapabilmesini sağlarken, yetkili personelin tüm süreci tek bir merkezden güvenli ve verimli şekilde yönetmesine olanak tanır.

---

## 🚀 Proje Hakkında

Uygulama, **ASP.NET Core MVC** mimarisi kullanılarak modern **.NET 9.0** platformunda geliştirilmiştir.

Sistem iki ana modülden oluşmaktadır:

### 1️⃣ Kullanıcı (Public) Arayüzü
- Erasmus programlarını filtreleme
- Üniversiteleri harita üzerinde görüntüleme
- Program detaylarını inceleme
- Deneyim paylaşımı ve yorum yapma

### 2️⃣ Yönetim (Admin) Paneli
- Rol bazlı yetkilendirme
- Okul ve program yönetimi (CRUD)
- Güvenli veri kontrolü ve içerik yönetimi

---

## ✨ Öne Çıkan Özellikler

### 👥 Kullanıcı (Public) Tarafı

- **Gelişmiş Filtreleme:** Ülke, bölüm, dil ve eğitim seviyesine göre hızlı arama
- **İnteraktif Harita:** Leaflet JS & OpenStreetMap entegrasyonu
- **Detaylı Program Bilgileri:** Kontenjan, geçerlilik tarihi, dil şartı ve kabul kriterleri
- **Yorum Sistemi:** Öğrencilerin deneyim paylaşımı ve geri bildirim mekanizması

---

### 🛡️ Yönetim (Admin) Tarafı

- **Güvenli Erişim:** Cookie tabanlı Authentication + Rol bazlı yetkilendirme (Area mimarisi)
- **Dinamik Veri Yönetimi:** Okul ve program ekleme, düzenleme, silme (anlık yansıma)
- **SweetAlert2 Entegrasyonu:** Güvenli işlem onay pencereleri
- **Server-side Paging:** Yüksek performanslı veri listeleme

---

## 🛠️ Kullanılan Teknolojiler ve Mimari

### 🔹 Backend
- .NET 9.0
- C#
- ASP.NET Core MVC

### 🔹 Veritabanı
- MS SQL Server
- Entity Framework Core (Code First)

### 🔹 Frontend
- HTML5
- Bootstrap 5
- jQuery

### 🔹 Entegrasyonlar
- DataTables
- SweetAlert2
- Leaflet (OpenStreetMap)

### 🔹 Yazılım Prensipleri
- Dependency Injection (DI)
- Repository Pattern
- ViewComponents
- Custom Html Helpers & Tag Helpers
- Client-side & Server-side Validation

---

## 📸 Ekran Görüntüleri

### 🏠 Ana Sayfa & Harita

<p align="center">
  <img src="https://github.com/user-attachments/assets/e5a13ade-2700-4bc5-9ba4-5b4a9165a737" width="80%" />
</p>

---

### 🔎 Gelişmiş Filtreleme

<p align="center">
  <img src="https://github.com/user-attachments/assets/97a3746a-9eaf-4f7a-affa-22626f89e674" width="70%" />
</p>

---

### 📚 Program Detayları & Yorumlar

<p align="center">
  <img src="https://github.com/user-attachments/assets/73e78077-42c0-4b01-92a4-5fcdded94bd7" width="80%" />
</p>

---

## 🛠️ Admin Paneli

### 📊 Dashboard

<p align="center">
  <img src="https://github.com/user-attachments/assets/d84a0f23-1b48-4f60-972e-90a77ec98e5d" width="48%" />
  <img src="https://github.com/user-attachments/assets/88ab90dc-80aa-459b-bce6-051305a408bc" width="48%" />
</p>

<p align="center">
  <img src="https://github.com/user-attachments/assets/32e90bbe-365b-4cbf-bfc8-a021d89052f3" width="48%" />
  <img src="https://github.com/user-attachments/assets/9a714e15-6d35-4311-82d7-da751dcc3300" width="48%" />
</p>

---

### 🗂 Veri Yönetimi

<p align="center">
  <img src="https://github.com/user-attachments/assets/6ed5a838-b43e-42ff-960e-272283c9aaa6" width="48%" />
  <img src="https://github.com/user-attachments/assets/4f32a2b8-8e80-47d9-a0cd-1c38f734520d" width="48%" />
</p>

---

<p align="center">
Bu platform sayesinde yeni üniversiteler ve Erasmus programları sisteme eklenebilir, 
harita üzerinde konumlandırılabilir ve ilgili okul hakkında ihtiyaç duyulan tüm bilgilere 
sade ve anlaşılır bir arayüz üzerinden erişilebilir.  

Kullanıcılar deneyimlerini paylaşarak yorum yapabilir, diğer öğrencilerin görüşlerinden faydalanabilir.  
Rol bazlı yapı sayesinde yöneticiler içerik yönetimini güvenli şekilde gerçekleştirirken, 
öğrenciler etkileşimli ve bilgilendirici bir deneyim yaşar.
</p>

---

## 💻 Kurulum ve Çalıştırma

### 1️⃣ Projeyi Klonlayın

```bash
git clone https://github.com/AhmetSudeys/ISUBU-Erasmus-Web.git
cd ISUBU-Erasmus-Web
```

### 2️⃣ Bağımlılıkları Yükleyin

```bash
dotnet restore
```

> ⚠️ `.csproj` dosyası alt klasördeyse ilgili dizine geçmeyi unutmayın.

---

### 3️⃣ Veritabanını Oluşturun

`appsettings.json` içindeki `DefaultConnection` alanını kendi SQL Server ayarlarınıza göre güncelleyin.

```bash
dotnet ef database update
```

EF CLI yüklü değilse:

```bash
dotnet tool install --global dotnet-ef
```

---

### 4️⃣ Uygulamayı Başlatın

```bash
dotnet run
```

Terminal çıktısı:

```
Now listening on: http://localhost:XXXX
```

---

## 🛑 Uygulamayı Durdurma

```bash
CTRL + C
```

---

## ✅ Gereksinimler

- .NET 9 SDK
- MS SQL Server
- Entity Framework Core Tools
