using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deneme.Data;
using deneme.Models;
using deneme.Services;
using deneme.Repositories;

namespace deneme.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AppDbContext _context;
    
    // Dependency Injection - Interface'ler üzerinden servisleri inject et
    private readonly IErasmusProgramiRepository _programRepository;
    private readonly IYorumService _yorumService;
    private readonly IEmailValidationService _emailValidationService;
    private readonly ICacheService _cacheService;

    // Constructor Injection - Tüm bağımlılıklar constructor üzerinden gelir
    public HomeController(
        ILogger<HomeController> logger, 
        AppDbContext context,
        IErasmusProgramiRepository programRepository,
        IYorumService yorumService,
        IEmailValidationService emailValidationService,
        ICacheService cacheService)
    {
        _logger = logger;
        _context = context;
        _programRepository = programRepository;
        _yorumService = yorumService;
        _emailValidationService = emailValidationService;
        _cacheService = cacheService;
    }

    public async Task<IActionResult> Index()
    {
        try
        {   
            // madde 16 - server side paging 
            // Erasmus programlarını getir
            var programs = await _context.ErasmusProgramlari
                .Include(e => e.Okul)
                    .ThenInclude(o => o!.Ulke)
                .Include(e => e.Dil)
                .Include(e => e.EgitimSeviyeleri)
                .Take(10)
                .ToListAsync();
            
            // Harita için okulları koordinatlarla getir ve her okul için ilk Erasmus program ID'sini ekle
            var okullarList = await _context.Okullar
                .Include(o => o.Ulke)
                .Where(o => o.Latitude.HasValue && o.Longitude.HasValue)
                .Select(o => new
                {
                    okulId = o.OkulId,
                    okulAd = o.OkulAd,
                    latitude = o.Latitude,
                    longitude = o.Longitude,
                    ulkeIsim = o.Ulke.UlkeIsim
                })
                .ToListAsync();
            
            // Her okul için ilk Erasmus program ID'sini toplu olarak al
            var okulIds = okullarList.Select(o => o.okulId).ToList();
            var firstPrograms = okulIds.Any() 
                ? await _context.ErasmusProgramlari
                    .Where(e => okulIds.Contains(e.OkulId))
                    .GroupBy(e => e.OkulId)
                    .Select(g => new { okulId = g.Key, erasmusId = g.Min(e => e.ErasmusId) })
                    .ToDictionaryAsync(x => x.okulId, x => x.erasmusId)
                : new Dictionary<int, int>();
            
            var okullarWithPrograms = okullarList.Select(o => new
            {
                okulId = o.okulId,
                okulAd = o.okulAd,
                latitude = o.latitude,
                longitude = o.longitude,
                ulkeIsim = o.ulkeIsim,
                erasmusId = firstPrograms.ContainsKey(o.okulId) ? firstPrograms[o.okulId] : (int?)null
            }).ToList();
            
            ViewBag.Okullar = okullarWithPrograms;
            
            return View(programs ?? new List<ErasmusProgrami>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Index sayfası yüklenirken hata oluştu");
            ViewBag.Error = "Veriler yüklenirken bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
            ViewBag.Okullar = new List<object>();
            return View(new List<ErasmusProgrami>());
        }
    }

    public async Task<IActionResult> IkiliAnlasmalar(int page = 1, int pageSize = 20, string search = "", int? ulkeId = null, int? dilId = null)
    {
        var query = _context.ErasmusProgramlari
            .Include(e => e.Okul)
                .ThenInclude(o => o!.Ulke)
            .Include(e => e.Dil)
            .Include(e => e.EgitimSeviyeleri)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => 
                e.BolumAdi!.Contains(search) || 
                e.ErasmusKodu.Contains(search) ||
                e.Okul!.OkulAd.Contains(search));
        }

        if (ulkeId.HasValue)
        {
            query = query.Where(e => e.Okul!.UlkeId == ulkeId.Value);
        }

        if (dilId.HasValue)
        {
            query = query.Where(e => e.DilId == dilId.Value);
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var programs = await query
            .OrderBy(e => e.ErasmusId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;
        ViewBag.Search = search;
        ViewBag.TotalCount = totalCount;
        ViewBag.Ulkeler = await _context.Ulkeler.ToListAsync();
        ViewBag.Diller = await _context.Diller.ToListAsync();
        ViewBag.SelectedUlkeId = ulkeId;
        ViewBag.SelectedDilId = dilId;

        return View(programs);
    }


    public async Task<IActionResult> ProgramDetails(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Dependency Injection kullanarak Repository Pattern ile veri çekme
        var program = await _programRepository.GetByIdAsync(id.Value);

        if (program == null)
        {
            return NotFound();
        }

        // Cache kullanımı örneği - Dependency Injection ile cache servisi kullanımı
        // Not: Bu örnekte cache kullanımını basitleştiriyoruz
        // Dependency Injection kullanarak YorumService'den yorum sayısını al
        var yorumSayisi = await _yorumService.GetYorumSayisiAsync(id.Value);
        ViewBag.YorumSayisi = yorumSayisi;

        return View(program);
    }

    // Not: Details ve GetPartners metodları kaldırıldı - artık sadece veritabanından okuma yapılıyor
    // Harita için okullar Index action'ında ViewBag ile gönderiliyor

    public async Task<IActionResult> Ara(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return RedirectToAction("Index");
        }

        var searchTerm = q.Trim();
        
        // Cache kullanımı - Arama sonuçlarını cache'le
        var cacheKey = $"search_{searchTerm}";
        var cachedResults = _cacheService.Get<List<ErasmusProgrami>>(cacheKey);
        
        List<ErasmusProgrami> programs;
        if (cachedResults != null)
        {
            _logger.LogInformation("Arama sonuçları cache'den alındı: {SearchTerm}", searchTerm);
            programs = cachedResults;
        }
        else
        {
            // Dependency Injection kullanarak Repository Pattern ile arama
            programs = await _programRepository.SearchAsync(searchTerm);
            
            // Sonuçları 5 dakika cache'le
            _cacheService.Set(cacheKey, programs, TimeSpan.FromMinutes(5));
            _logger.LogInformation("Arama sonuçları cache'lendi: {SearchTerm}, {Count} sonuç", searchTerm, programs.Count);
        }

        ViewBag.SearchTerm = searchTerm;
        ViewBag.ResultCount = programs.Count;

        return View(programs);
    }

    public IActionResult Iletisim()
    {
        return View();
    }

    // GET: Home/YorumEkle
    public async Task<IActionResult> YorumEkle(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var program = await _context.ErasmusProgramlari
            .Include(e => e.Okul)
            .FirstOrDefaultAsync(m => m.ErasmusId == id);

        if (program == null)
        {
            return NotFound();
        }

        ViewBag.ProgramId = id;
        ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
        return View();
    }

    // POST: Home/YorumEkle
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> YorumEkle(int? id, string adSoyad, string email, string yorumYazisi)
    {
        if (id == null)
        {
            return NotFound();
        }

        var program = await _context.ErasmusProgramlari
            .FirstOrDefaultAsync(m => m.ErasmusId == id);

        if (program == null)
        {
            return NotFound();
        }

        // Server-side Validation
        
        // Ad Soyad kontrolü - Boş olamaz
        if (string.IsNullOrWhiteSpace(adSoyad))
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Ad Soyad alanı boş bırakılamaz.";
            return View();
        }

        var adSoyadTrimmed = adSoyad.Trim();
        if (adSoyadTrimmed.Length < 2)
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Ad Soyad en az 2 karakter olmalıdır.";
            return View();
        }

        if (adSoyadTrimmed.Length > 200)
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Ad Soyad en fazla 200 karakter olabilir.";
            return View();
        }

        // E-posta kontrolü - Boş olamaz ve geçerli format olmalı
        if (string.IsNullOrWhiteSpace(email))
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "E-posta alanı boş bırakılamaz.";
            return View();
        }

        var emailTrimmed = email.Trim();
        if (emailTrimmed.Length > 200)
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "E-posta en fazla 200 karakter olabilir.";
            return View();
        }

        // Dependency Injection kullanarak E-posta validasyonu
        if (!_emailValidationService.IsValidEmail(emailTrimmed))
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Geçerli bir e-posta adresi giriniz (örn: ornek@email.com).";
            return View();
        }

        // E-postayı normalize et
        emailTrimmed = _emailValidationService.NormalizeEmail(emailTrimmed);

        // Yorum kontrolü - Boş olamaz, minimum 20, maksimum 200 karakter
        if (string.IsNullOrWhiteSpace(yorumYazisi))
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Yorum alanı boş bırakılamaz.";
            return View();
        }

        var yorumTrimmed = yorumYazisi.Trim();
        if (yorumTrimmed.Length < 20)
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Yorum en az 20 karakter olmalıdır.";
            return View();
        }

        if (yorumTrimmed.Length > 200)
        {
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Yorum en fazla 200 karakter olabilir.";
            return View();
        }

        try
        {
            // Dependency Injection kullanarak YorumService ile yorum ekleme
            var yorum = await _yorumService.YorumEkleAsync(
                program.ErasmusId,
                adSoyadTrimmed,
                emailTrimmed,
                yorumTrimmed
            );

            // Cache'i temizle (yeni yorum eklendiği için)
            _cacheService.Remove($"program_yorum_sayisi_{id}");

            TempData["Success"] = "Yorumunuz başarıyla eklendi.";
            return RedirectToAction("ProgramDetails", new { id = id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Yorum eklenirken hata oluştu");
            ViewBag.ProgramId = id;
            ViewBag.OkulAd = program.Okul?.OkulAd ?? "Bilinmeyen Okul";
            ViewBag.Error = "Yorum eklenirken bir hata oluştu. Lütfen tekrar deneyin.";
            return View();
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult Baglantilar()
    {
        return View();
    }

    public IActionResult DependencyInjectionDemo()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
