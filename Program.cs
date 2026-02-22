using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using deneme.Data;
using deneme.Models;
using deneme.Services;
using deneme.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    //madde 3 MSSQL bağlantısı
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// madde 22 - Authentication ( Doğrulama )
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// madde 23 - Authorization ( Yetkilendirme )
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// madde 15 - Servisler - Scoped Lifetime (Her HTTP request için yeni instance)

// Interface ve Implementation'ları kaydet (Dependency Injection)
// Scoped: Her HTTP request için aynı instance, request bitince dispose edilir
builder.Services.AddScoped<IYorumService, YorumService>();
builder.Services.AddScoped<IOkulService, OkulService>();
builder.Services.AddScoped<IErasmusProgramiRepository, ErasmusProgramiRepository>();

// Singleton: Uygulama boyunca tek instance (stateless servisler için)
builder.Services.AddSingleton<IEmailValidationService, EmailValidationService>();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

var app = builder.Build();

// Development ortamında detaylı hata sayfası göster
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // Sadece Production'da HTTPS redirection kullan
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Area rotası (Admin)
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

// MVC rotası
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

// Veritabanı bağlantısını kontrol et ve temel seed işlemlerini yap
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Veritabanı bağlantısını kontrol et
    Console.WriteLine("========================================");
    Console.WriteLine("VERİTABANI BAĞLANTI KONTROLÜ");
    Console.WriteLine("========================================");
    
    try
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Connection String: {connectionString}");
        Console.WriteLine("Veritabanına bağlanılıyor...");
        
        if (!db.Database.CanConnect())
        {
            Console.WriteLine("❌ UYARI: MSSQL veritabanına bağlanılamıyor!");
            Console.WriteLine("Lütfen şunları kontrol edin:");
            Console.WriteLine("1. SQL Server servisinin çalıştığından emin olun");
            Console.WriteLine("2. WEB_PROJE veritabanının mevcut olduğunu kontrol edin");
            Console.WriteLine("3. Windows Authentication ile bağlanabildiğinizi doğrulayın");
        }
        else
        {
            Console.WriteLine("✅ MSSQL veritabanına başarıyla bağlanıldı!");
            Console.WriteLine($"Veritabanı: WEB_PROJE");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Veritabanı bağlantı hatası: {ex.Message}");
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine($"Detay: {ex}");
        }
    }
    
    Console.WriteLine("========================================");

    // Temel seed işlemleri (sadece boşsa ekle)
    try
    {
        Console.WriteLine("\n========================================");
        Console.WriteLine("VERİTABANI VERİ KONTROLÜ");
        Console.WriteLine("========================================");
        
        var ulkeSayisi = db.Ulkeler.Count();
        var okulSayisi = db.Okullar.Count();
        var programSayisi = db.ErasmusProgramlari.Count();
        
        Console.WriteLine($"Mevcut veriler:");
        Console.WriteLine($"  - Ülkeler: {ulkeSayisi}");
        Console.WriteLine($"  - Okullar: {okulSayisi}");
        Console.WriteLine($"  - Erasmus Programları: {programSayisi}");
        Console.WriteLine($"  - Yorumlar: {db.Yorumlar.Count()}");
        Console.WriteLine($"  - Eğitim Seviyeleri: {db.EgitimSeviyeleri.Count()}");
        Console.WriteLine($"  - Diller: {db.Diller.Count()}");
        Console.WriteLine($"  - Adminler: {db.Admins.Count()}");
        Console.WriteLine("========================================\n");

        // Temel lookupları hazırla (sadece boşsa)
        if (!db.EgitimSeviyeleri.Any())
        {
            Console.WriteLine("Eğitim seviyeleri ekleniyor...");
            db.EgitimSeviyeleri.AddRange(
                new EgitimSeviyesi { EgitimSeviyesiAdi = "Lisans" },
                new EgitimSeviyesi { EgitimSeviyesiAdi = "Yüksek Lisans" },
                new EgitimSeviyesi { EgitimSeviyesiAdi = "Doktora" }
            );
            db.SaveChanges();
            Console.WriteLine("✅ Eğitim seviyeleri eklendi.");
        }

        if (!db.Diller.Any() && db.EgitimSeviyeleri.Any())
        {
            Console.WriteLine("Diller ekleniyor...");
            db.Diller.AddRange(
                new Dil { DilAdi = "English", EgitimSeviyesiId = db.EgitimSeviyeleri.First().EgitimSeviyesiId },
                new Dil { DilAdi = "Turkish", EgitimSeviyesiId = db.EgitimSeviyeleri.First().EgitimSeviyesiId }
            );
            db.SaveChanges();
            Console.WriteLine("✅ Diller eklendi.");
        }

        if (!db.Admins.Any())
        {
            Console.WriteLine("Admin kullanıcısı ekleniyor...");
            db.Admins.Add(new Admin
            {
                AdminKullaniciAdi = "admin",
                AdminSifre = "admin123"
            });
            db.SaveChanges();
            Console.WriteLine("✅ Admin kullanıcısı eklendi.");
        }

        Console.WriteLine("\n✅ Veritabanı hazır. Tüm veriler veritabanından okunacak.\n");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Seed işlemi sırasında hata oluştu: {ex.Message}");
        if (app.Environment.IsDevelopment())
        {
            Console.WriteLine($"Detay: {ex}");
        }
    }
}

app.Run();
