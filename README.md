
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
> ![Ana Sayfa]<img width="1103" height="1393" alt="anasayfa" src="https://github.com/user-attachments/assets/e5a13ade-2700-4bc5-9ba4-5b4a9165a737" />
> Öğrencilerin anlaşmalı okulları coğrafi olarak inceleyebildiği ana sayfa görünümü.

**2. Gelişmiş Filtreleme (İkili Anlaşmalar)**
> ![Filtreleme]<img width="858" height="832" alt="image" src="https://github.com/user-attachments/assets/97a3746a-9eaf-4f7a-affa-22626f89e674" />
> Erasmus programları arasında ülke, dil ve bölüm bazlı hızlı arama imkanı.

**3. Program Detayları ve Yorumlar**
> ![Program Detayları]<img width="1259" height="1545" alt="image" src="https://github.com/user-attachments/assets/73e78077-42c0-4b01-92a4-5fcdded94bd7" />
> Öğrencilerin program detaylarını inceleyip, diğer öğrencilerin tecrübelerini okuyabildiği alan.

**4. Admin Paneli - Dashboard**
> ![Admin Dashboard]
> <img width="1259" height="595" alt="image" src="https://github.com/user-attachments/assets/d84a0f23-1b48-4f60-972e-90a77ec98e5d" />
<img width="1259" height="600" alt="image" src="https://github.com/user-attachments/assets/88ab90dc-80aa-459b-bce6-051305a408bc" />
<img width="1259" height="599" alt="image" src="https://github.com/user-attachments/assets/32e90bbe-365b-4cbf-bfc8-a021d89052f3" />
<img width="649" height="485" alt="image" src="https://github.com/user-attachments/assets/9a714e15-6d35-4311-82d7-da751dcc3300" />
<img width="1259" height="552" alt="image" src="https://github.com/user-attachments/assets/e5d36072-4d9f-4838-a92e-ca15df8985af" 
 <img width="1259" height="578" alt="image" src="https://github.com/user-attachments/assets/fb9fff2a-f5dd-44cc-bb69-514d65c2a165" />
 <img width="1259" height="368" alt="image" src="https://github.com/user-attachments/assets/4ac065a4-9a73-4817-89a9-0d204e232dae" />
> Sistemin genel durumunun ve son eklenen öğrenci yorumlarının takip edildiği yönetici paneli.

**5. Admin Paneli - Veri Yönetimi**
> ![Admin Liste] <img width="1259" height="584" alt="image" src="https://github.com/user-attachments/assets/6ed5a838-b43e-42ff-960e-272283c9aaa6" />
<img width="1259" height="587" alt="image" src="https://github.com/user-attachments/assets/4f32a2b8-8e80-47d9-a0cd-1c38f734520d" />
<img width="1259" height="364" alt="image" src="https://github.com/user-attachments/assets/f3378106-9cd1-4067-a5ed-ca8e23e35055" />
<img width="1259" height="597" alt="image" src="https://github.com/user-attachments/assets/86813d62-df0a-4ce0-99fd-8aa7d36ab6e1" />
<img width="1259" height="268" alt="image" src="https://github.com/user-attachments/assets/cafcaa9e-e25d-4c90-bc32-20604df41106" />
> DataTables ve SweetAlert2 entegrasyonu ile okulların ve programların güvenle yönetilmesi.



## 💻 Kurulum ve Çalıştırma

Bu projeyi yerel ortamınızda ayağa kaldırmak için aşağıdaki adımları sırasıyla takip edebilirsiniz.

---

### 1️⃣ Projeyi Klonlayın

Terminalinizi açın ve projeyi bilgisayarınıza indirin:

```bash
git clone https://github.com/AhmetSudeys/ISUBU-Erasmus-Web.git
cd ISUBU-Erasmus-Web
```

---

### 2️⃣ Bağımlılıkları Yükleyin

Gerekli tüm NuGet paketlerini yüklemek için:

```bash
dotnet restore
```

> ⚠️ **Dizin Kontrolü**  
> Eğer `.csproj` dosyanız ana dizinde değilse (örneğin `/src` veya `/deneme` klasöründe ise), komutları çalıştırmadan önce ilgili klasöre geçmeyi unutmayın:
>
> ```bash
> cd <klasör-adı>
> ```

---

### 3️⃣ Veritabanı Yapılandırması

Migration işlemlerinden önce:

- `appsettings.json` dosyasını açın.
- `DefaultConnection` alanındaki **Connection String** bilgisini kendi yerel SQL Server ayarlarınıza göre güncelleyin.

Ardından veritabanını oluşturmak için:

```bash
dotnet ef database update
```

> 💡 **dotnet ef komutu çalışmıyorsa**
>
> Entity Framework Core araçlarını global olarak yükleyebilirsiniz:
>
> ```bash
> dotnet tool install --global dotnet-ef
> ```

---

### 4️⃣ Projeyi Çalıştırın

Her şey hazır! Uygulamayı başlatmak için:

```bash
dotnet run
```

Başarılı bir şekilde çalıştığında terminalde şu çıktıyı göreceksiniz:

```
Now listening on: http://localhost:XXXX
```

Tarayıcınızdan bu adresi ziyaret ederek projeyi test edebilirsiniz.

---

### 🛑 Uygulamayı Durdurma

Uygulama çalışırken terminalde:

```bash
CTRL + C
```

tuş kombinasyonunu kullanarak projeyi durdurabilirsiniz.

---

## ✅ Gereksinimler

- .NET SDK (6.0 veya üzeri önerilir)
- SQL Server
- Entity Framework Core Tools
