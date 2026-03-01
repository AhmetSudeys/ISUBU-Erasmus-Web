
# 🌍 Erasmus Koordinatörlüğü Yönetim Sistemi

Bu proje, Isparta Uygulamalı Bilimler Üniversitesi (ISUBÜ) Erasmus Koordinatörlüğü için geliştirilmiş kapsamlı bir web uygulamasıdır. Öğrencilerin anlaşmalı okulları ve Erasmus programlarını kolayca inceleyip yorum yapabilmesini sağlarken, yetkili personelin tüm bu süreci tek bir merkezden güvenle yönetmesine olanak tanır.

## 🚀 Proje Hakkında

Uygulama, **ASP.NET Core MVC** mimarisi kullanılarak modern **.NET 9.0** platformunda geliştirilmiştir. Temel olarak iki ana modülden oluşur:
1. **Kullanıcı (Public) Arayüzü:** Öğrencilerin programları filtreleyebildiği, harita üzerinden okulların konumlarını görebildiği ve deneyimlerini yorum olarak paylaşabildiği alan.
2. **Yönetim (Admin) Paneli:** Rol bazlı yetkilendirme ile korunan, okulların ve programların (CRUD işlemleri) yönetildiği kontrol merkezi.

## ✨ Öne Çıkan Özellikler

### 👥 Kullanıcı (Public) Tarafı
* **Gelişmiş Filtreleme ve Arama:** Binlerce Erasmus anlaşması arasında ülke, bölüm, dil veya eğitim seviyesine göre hızlı arama.
* **İnteraktif Harita:** Leaflet JS ve OpenStreetMap altyapısıyla okulların coğrafi konumlarını harita üzerinde görüntüleme.
* **Detaylı Program Bilgileri:** Kontenjan, geçerlilik tarihi, dil şartı ve kabul kriterlerinin şeffaf sunumu.
* **Etkileşimli Yorum Sistemi:** Öğrencilerin programlar hakkında geri bildirim bırakabilmesi ve diğer yorumları okuyabilmesi.

### 🛡️ Yönetim (Admin) Tarafı
* **Güvenli Erişim:** Cookie tabanlı Authentication ve "Admin" rolü ile sınırlandırılmış güvenli yönetim alanı (Area mimarisi).
* **Dinamik Veri Yönetimi:** Yeni okullar ve Erasmus programları ekleme, düzenleme ve silme işlemleri. (Değişiklikler anlık olarak public tarafa yansır).
* **Kullanıcı Dostu Arayüz:** SweetAlert2 entegrasyonu ile silme işlemlerinde güvenli onay pencereleri ve işlem bildirimleri.
* **Sunucu Taraflı Sayfalama (Server-side Paging):** Yüksek veri performansını sağlamak için listelemelerde veritabanı seviyesinde sayfalama.

## 🛠️ Kullanılan Teknolojiler ve Mimari

* **Backend:** .NET 9.0, C#, ASP.NET Core MVC.
* **Veritabanı & ORM:** MS SQL Server, Entity Framework Core (Code First Yaklaşımı).
* **Frontend:** HTML5, Bootstrap 5, jQuery.
* **Kütüphaneler & Eklentiler:** * Tablo yönetimi için **DataTables**.
  * Kullanıcı bildirimleri için **SweetAlert2**.
  * Harita entegrasyonu için **Leaflet**.
* **Tasarım Desenleri ve Pratikler:** Dependency Injection (DI), Repository Pattern, ViewComponents, Custom Html Helpers & Tag Helpers.
* **Doğrulama (Validation):** Hem Client-side (jQuery Validation) hem de Server-side doğrulama sistemleri.

## 📸 Ekran Görüntüleri

**1. Ana Sayfa ve Etkileşimli Harita**
> ![Ana Sayfa](Buraya_ana_sayfanin_resim_linkini_ekleyin_ve_bu_metni_silin)
> Öğrencilerin anlaşmalı okulları coğrafi olarak inceleyebildiği ana sayfa görünümü.

**2. Gelişmiş Filtreleme (İkili Anlaşmalar)**
> ![Filtreleme](Buraya_ikili_anlasmalar_resim_linkini_ekleyin_ve_bu_metni_silin)
> Erasmus programları arasında ülke, dil ve bölüm bazlı hızlı arama imkanı.

**3. Program Detayları ve Yorumlar**
> ![Program Detayları](Buraya_program_detay_resim_linkini_ekleyin_ve_bu_metni_silin)
> Öğrencilerin program detaylarını inceleyip, diğer öğrencilerin tecrübelerini okuyabildiği alan.

**4. Admin Paneli - Dashboard**
> ![Admin Dashboard](Buraya_admin_dashboard_resim_linkini_ekleyin_ve_bu_metni_silin)
> Sistemin genel durumunun ve son eklenen öğrenci yorumlarının takip edildiği yönetici paneli.

**5. Admin Paneli - Veri Yönetimi**
> ![Admin Liste](Buraya_admin_liste_resim_linkini_ekleyin_ve_bu_metni_silin)
> DataTables ve SweetAlert2 entegrasyonu ile okulların ve programların güvenle yönetilmesi.

## 💻 Kurulum ve Çalıştırma

# 1. Projeyi bilgisayarınıza klonlayın
git clone https://github.com/AhmetSudeys/ISUBU-Erasmus-Web.git

# 2. Klonlanan projenin ana dizinine geçiş yapın
cd ISUBU-Erasmus-Web

# ÖNEMLİ NOT: Eğer projenizin .csproj (C# proje dosyası) klasör yapınızda bir alt 
# dizindeyse (örneğin "deneme" klasöründeyse), o dizine girmeniz gerekir:
# cd deneme 
# (Eğer .csproj ana dizindeyse bu adımı atlayabilirsiniz.)

# 3. Projedeki gerekli kütüphaneleri (NuGet paketlerini) indirin ve kurun
dotnet restore

# 4. Veritabanını yerel (localhost) sunucunuzda oluşturun.
# DİKKAT: Bu adımı çalıştırmadan önce kod editörünüzde 'appsettings.json' dosyasını açıp 
# "DefaultConnection" kısmındaki SQL Server veritabanı bağlantı cümlenizin 
# bilgisayarınızdaki yerel SQL Server ayarlarıyla eşleştiğinden emin olun.
dotnet ef database update

# 5. Projeyi derleyin ve yayına alın (çalıştırın)
dotnet run

# Proje başarıyla çalıştığında terminalde "Now listening on: http://localhost:XXXX" 
# şeklinde bir mesaj göreceksiniz. Bu linke tıklayarak (veya tarayıcıya kopyalayarak) 
# projenizi görüntüleyebilirsiniz. İşlemi durdurmak için terminalde CTRL + C yapabilirsiniz.

Ufak bir hatırlatma: dotnet ef database update komutunun çalışması için bilgisayarınızda Entity Framework Core CLI araçlarının yüklü olması gerekir. Eğer bu komutta "komut bulunamadı" gibi bir hata alırsanız, önce şu kodu çalıştırarak aracı global olarak kurabilirsiniz: dotnet tool install --global dotnet-ef

