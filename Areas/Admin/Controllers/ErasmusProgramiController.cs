using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using deneme.Data;
using deneme.Models;

namespace deneme.Areas.Admin.Controllers
{
    // madde 23 - authorization
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ErasmusProgramiController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ErasmusProgramiController> _logger;

        public ErasmusProgramiController(AppDbContext context, ILogger<ErasmusProgramiController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Admin/ErasmusProgrami
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string search = "")
        {
            var query = _context.ErasmusProgramlari
                .Include(e => e.Okul)
                    .ThenInclude(o => o!.Ulke)
                .Include(e => e.Dil)
                .Include(e => e.EgitimSeviyeleri)
                .AsQueryable();

            // Arama işlemi - case-insensitive ve tüm ilgili alanlarda
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                // SQL Server'da case-insensitive arama için EF.Functions.Like kullanıyoruz
                // Not: EgitimSeviyeleri koleksiyonu üzerinde arama yapılamıyor (EF Core çeviri hatası)
                // Bu yüzden sadece temel alanlarda arama yapıyoruz
                query = query.Where(e => 
                    (e.BolumAdi != null && EF.Functions.Like(e.BolumAdi, $"%{search}%")) || 
                    (e.ErasmusKodu != null && EF.Functions.Like(e.ErasmusKodu, $"%{search}%")) ||
                    (e.Okul != null && e.Okul.OkulAd != null && EF.Functions.Like(e.Okul.OkulAd, $"%{search}%")) ||
                    (e.Okul != null && e.Okul.Ulke != null && e.Okul.Ulke.UlkeIsim != null && EF.Functions.Like(e.Okul.Ulke.UlkeIsim, $"%{search}%")) ||
                    (e.Dil != null && e.Dil.DilAdi != null && EF.Functions.Like(e.Dil.DilAdi, $"%{search}%"))
                );
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

            return View(programs);
        }

        // GET: Admin/ErasmusProgrami/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var erasmusProgrami = await _context.ErasmusProgramlari
                .Include(e => e.Okul)
                    .ThenInclude(o => o!.Ulke)
                .Include(e => e.Dil)
                .Include(e => e.EgitimSeviyeleri)
                .Include(e => e.Yorumlar)
                .FirstOrDefaultAsync(m => m.ErasmusId == id);

            if (erasmusProgrami == null)
            {
                return NotFound();
            }

            return View(erasmusProgrami);
        }

        // GET: Admin/ErasmusProgrami/Create
        public async Task<IActionResult> Create()
        {
            // İngilizce dilinin ID'sini bul
            var ingilizceDilId = await _context.Diller
                .Where(d => d.DilAdi.ToLower().Contains("ingilizce") || d.DilAdi.ToLower().Contains("english"))
                .Select(d => d.DilId)
                .FirstOrDefaultAsync();
            
            // Eğer İngilizce bulunamazsa, ilk dili kullan (fallback)
            if (ingilizceDilId == 0)
            {
                ingilizceDilId = await _context.Diller
                    .OrderBy(d => d.DilId)
                    .Select(d => d.DilId)
                    .FirstOrDefaultAsync();
            }
            
            ViewBag.OkulId = new SelectList(await _context.Okullar.Include(o => o.Ulke).ToListAsync(), "OkulId", "OkulAd");
            
            // Sadece İngilizce dilini göster
            var ingilizceDil = await _context.Diller
                .Where(d => d.DilId == ingilizceDilId)
                .FirstOrDefaultAsync();
            
            if (ingilizceDil != null)
            {
                ViewBag.DilId = new SelectList(new List<Dil> { ingilizceDil }, "DilId", "DilAdi", ingilizceDilId);
            }
            else
            {
                ViewBag.DilId = new SelectList(new List<Dil>(), "DilId", "DilAdi");
            }
            
            return View();
        }

        // POST: Admin/ErasmusProgrami/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OkulId,BolumAdi,ErasmusKodu,GecerlilikTarihi,GuzNomisyon,BaharNomisyon,AnlasmayaAraciOlan,TopOgrKont,TopStajKont,PersonelDersVerme,PersonelEgitimAlma,DilId,AnlasmaDurumu,KontenjanDetayi")] ErasmusProgrami erasmusProgrami)
        {
            // Navigation property hatalarını kaldır
            ModelState.Remove("Okul");
            ModelState.Remove("Dil");
            ModelState.Remove("EgitimSeviyeleri");
            ModelState.Remove("DilId"); // DilId otomatik olarak İngilizce'ye ayarlanacak
            
            // ModelState hatalarını logla
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var errorMessage in error.Value.Errors)
                    {
                        _logger.LogWarning("Create ErasmusProgrami - ModelState Error - Key: {Key}, Error: {Error}", error.Key, errorMessage.ErrorMessage);
                    }
                }
            }
            
            if (ModelState.IsValid)
            {
                try
                {
                    // İngilizce dilinin ID'sini bul (yeni eklenenlerin hepsi İngilizce olacak)
                    var ingilizceDilId = await _context.Diller
                        .Where(d => d.DilAdi.ToLower().Contains("ingilizce") || d.DilAdi.ToLower().Contains("english"))
                        .Select(d => d.DilId)
                        .FirstOrDefaultAsync();
                    
                    // Eğer İngilizce bulunamazsa, ilk dili kullan (fallback)
                    if (ingilizceDilId == 0)
                    {
                        ingilizceDilId = await _context.Diller
                            .OrderBy(d => d.DilId)
                            .Select(d => d.DilId)
                            .FirstOrDefaultAsync();
                    }
                    
                    // DilId'yi İngilizce'ye ayarla
                    erasmusProgrami.DilId = ingilizceDilId;
                    
                    // Veritabanındaki eski EgitimSeviyesiId sütunu için geçici çözüm
                    // Raw SQL ile INSERT yaparak eski sütunu bypass ediyoruz
                    // Parametreleri oluştur (nullable değerler için DBNull.Value kullan)
                    var parameters = new List<Microsoft.Data.SqlClient.SqlParameter>
                    {
                        new Microsoft.Data.SqlClient.SqlParameter("@p0", erasmusProgrami.OkulId),
                        new Microsoft.Data.SqlClient.SqlParameter("@p1", (object?)erasmusProgrami.BolumAdi ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p2", erasmusProgrami.ErasmusKodu),
                        new Microsoft.Data.SqlClient.SqlParameter("@p3", (object?)erasmusProgrami.GecerlilikTarihi ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p4", (object?)erasmusProgrami.GuzNomisyon ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p5", (object?)erasmusProgrami.BaharNomisyon ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p6", (object?)erasmusProgrami.AnlasmayaAraciOlan ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p7", (object?)erasmusProgrami.TopOgrKont ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p8", (object?)erasmusProgrami.TopStajKont ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p9", (object?)erasmusProgrami.PersonelDersVerme ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p10", (object?)erasmusProgrami.PersonelEgitimAlma ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p11", ingilizceDilId),
                        new Microsoft.Data.SqlClient.SqlParameter("@p12", (object?)erasmusProgrami.AnlasmaDurumu ?? DBNull.Value),
                        new Microsoft.Data.SqlClient.SqlParameter("@p13", (object?)erasmusProgrami.KontenjanDetayi ?? DBNull.Value)
                    };
                    
                    var sqlWithParams = @"
                        INSERT INTO ErasmusProgramlari 
                        (OkulId, BolumAdi, ErasmusKodu, GecerlilikTarihi, GuzNomisyon, BaharNomisyon, 
                         AnlasmayaAraciOlan, TopOgrKont, TopStajKont, PersonelDersVerme, PersonelEgitimAlma, 
                         DilId, AnlasmaDurumu, KontenjanDetayi, EgitimSeviyesiId)
                        VALUES 
                        (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11, @p12, @p13, 1);";
                    
                    await _context.Database.ExecuteSqlRawAsync(sqlWithParams, parameters.ToArray());
                    
                    TempData["Success"] = "Erasmus programı başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError(dbEx, "Veritabanı güncelleme hatası");
                    var innerException = dbEx.InnerException?.Message ?? dbEx.Message;
                    _logger.LogError("Inner Exception: {InnerEx}", dbEx.InnerException?.ToString());
                    ModelState.AddModelError("", $"Erasmus programı eklenirken bir hata oluştu: {innerException}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erasmus programı eklenirken hata oluştu");
                    _logger.LogError("Exception Details: {Ex}", ex.ToString());
                    ModelState.AddModelError("", $"Erasmus programı eklenirken bir hata oluştu: {ex.Message}");
                    if (ex.InnerException != null)
                    {
                        ModelState.AddModelError("", $"Detay: {ex.InnerException.Message}");
                    }
                }
            }
            
            ViewBag.OkulId = new SelectList(await _context.Okullar.Include(o => o.Ulke).ToListAsync(), "OkulId", "OkulAd", erasmusProgrami.OkulId);
            ViewBag.DilId = new SelectList(await _context.Diller.ToListAsync(), "DilId", "DilAdi", erasmusProgrami.DilId);
            return View(erasmusProgrami);
        }

        // GET: Admin/ErasmusProgrami/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var erasmusProgrami = await _context.ErasmusProgramlari
                .Include(e => e.EgitimSeviyeleri)
                .FirstOrDefaultAsync(e => e.ErasmusId == id);
            if (erasmusProgrami == null)
            {
                return NotFound();
            }
            ViewData["OkulId"] = new SelectList(_context.Okullar.Include(o => o.Ulke), "OkulId", "OkulAd", erasmusProgrami.OkulId);
            ViewData["DilId"] = new SelectList(_context.Diller, "DilId", "DilAdi", erasmusProgrami.DilId);
            return View(erasmusProgrami);
        }

        // POST: Admin/ErasmusProgrami/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ErasmusId,OkulId,BolumAdi,ErasmusKodu,GecerlilikTarihi,GuzNomisyon,BaharNomisyon,AnlasmayaAraciOlan,TopOgrKont,TopStajKont,PersonelDersVerme,PersonelEgitimAlma,DilId,AnlasmaDurumu,KontenjanDetayi")] ErasmusProgrami erasmusProgrami)
        {
            if (id != erasmusProgrami.ErasmusId)
            {
                return NotFound();
            }

            // Navigation property hatalarını kaldır
            ModelState.Remove("Okul");
            ModelState.Remove("Dil");
            ModelState.Remove("EgitimSeviyeleri");

            if (ModelState.IsValid)
            {
                try
                {
                    // Mevcut programı yükle ve eğitim seviyelerini güncelle
                    var existingProgram = await _context.ErasmusProgramlari
                        .Include(e => e.EgitimSeviyeleri)
                        .FirstOrDefaultAsync(e => e.ErasmusId == id);
                    
                    if (existingProgram == null)
                    {
                        return NotFound();
                    }
                    
                    // Program bilgilerini güncelle
                    existingProgram.OkulId = erasmusProgrami.OkulId;
                    existingProgram.BolumAdi = erasmusProgrami.BolumAdi;
                    existingProgram.ErasmusKodu = erasmusProgrami.ErasmusKodu;
                    existingProgram.GecerlilikTarihi = erasmusProgrami.GecerlilikTarihi;
                    existingProgram.GuzNomisyon = erasmusProgrami.GuzNomisyon;
                    existingProgram.BaharNomisyon = erasmusProgrami.BaharNomisyon;
                    existingProgram.AnlasmayaAraciOlan = erasmusProgrami.AnlasmayaAraciOlan;
                    existingProgram.TopOgrKont = erasmusProgrami.TopOgrKont;
                    existingProgram.TopStajKont = erasmusProgrami.TopStajKont;
                    existingProgram.PersonelDersVerme = erasmusProgrami.PersonelDersVerme;
                    existingProgram.PersonelEgitimAlma = erasmusProgrami.PersonelEgitimAlma;
                    existingProgram.DilId = erasmusProgrami.DilId;
                    existingProgram.AnlasmaDurumu = erasmusProgrami.AnlasmaDurumu;
                    existingProgram.KontenjanDetayi = erasmusProgrami.KontenjanDetayi;
                    
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Erasmus programı başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ErasmusProgramiExists(erasmusProgrami.ErasmusId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OkulId"] = new SelectList(_context.Okullar.Include(o => o.Ulke), "OkulId", "OkulAd", erasmusProgrami.OkulId);
            ViewData["DilId"] = new SelectList(_context.Diller, "DilId", "DilAdi", erasmusProgrami.DilId);
            return View(erasmusProgrami);
        }

        // GET: Admin/ErasmusProgrami/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var erasmusProgrami = await _context.ErasmusProgramlari
                .Include(e => e.Okul)
                    .ThenInclude(o => o!.Ulke)
                .Include(e => e.Dil)
                .Include(e => e.EgitimSeviyeleri)
                .FirstOrDefaultAsync(m => m.ErasmusId == id);

            if (erasmusProgrami == null)
            {
                return NotFound();
            }

            return View(erasmusProgrami);
        }

        // POST: Admin/ErasmusProgrami/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var erasmusProgrami = await _context.ErasmusProgramlari.FindAsync(id);
            if (erasmusProgrami != null)
            {
                _context.ErasmusProgramlari.Remove(erasmusProgrami);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Erasmus programı başarıyla silindi.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ErasmusProgramiExists(int id)
        {
            return _context.ErasmusProgramlari.Any(e => e.ErasmusId == id);
        }

        // AJAX: Yeni okul ekle
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateOkul([FromBody] Okul okul)
        {
            // Navigation property hatasını kaldır (sadece UlkeId kontrol edilecek)
            ModelState.Remove("Ulke");
            ModelState.Remove("UlkeId");
            
            _logger.LogInformation("CreateOkul - Gelen UlkeId: {UlkeId}, OkulAd: {OkulAd}", okul.UlkeId, okul.OkulAd);
            
            // Koordinat kontrolü - Zorunlu
            if (!okul.Latitude.HasValue || okul.Latitude.Value == 0)
            {
                return Json(new { success = false, errors = new[] { "Enlem (Latitude) zorunludur. Haritaya tıklayarak veya manuel olarak girebilirsiniz." } });
            }
            if (okul.Latitude.Value < -90 || okul.Latitude.Value > 90)
            {
                return Json(new { success = false, errors = new[] { "Enlem değeri -90 ile 90 arasında olmalıdır." } });
            }
            
            if (!okul.Longitude.HasValue || okul.Longitude.Value == 0)
            {
                return Json(new { success = false, errors = new[] { "Boylam (Longitude) zorunludur. Haritaya tıklayarak veya manuel olarak girebilirsiniz." } });
            }
            if (okul.Longitude.Value < -180 || okul.Longitude.Value > 180)
            {
                return Json(new { success = false, errors = new[] { "Boylam değeri -180 ile 180 arasında olmalıdır." } });
            }
            
            // URL kontrolü - Basit kontrol (çok sıkı değil)
            if (!string.IsNullOrWhiteSpace(okul.InternetSitesi))
            {
                var url = okul.InternetSitesi.Trim();
                // Eğer http:// veya https:// ile başlamıyorsa ekle
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && 
                    !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "https://" + url;
                    okul.InternetSitesi = url;
                }
                
                // Basit URL format kontrolü (çok sıkı değil)
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? result) || 
                    (result.Scheme != Uri.UriSchemeHttp && result.Scheme != Uri.UriSchemeHttps))
                {
                    return Json(new { success = false, errors = new[] { "Geçerli bir URL giriniz (örn: http://www.example.com veya https://www.example.com)" } });
                }
            }

            // UlkeId kontrolü
            if (okul.UlkeId <= 0)
            {
                _logger.LogWarning("CreateOkul - UlkeId geçersiz: {UlkeId}", okul.UlkeId);
                return Json(new { success = false, errors = new[] { "Ülke seçimi zorunludur." } });
            }
            
            // Seçilen ülkenin veritabanında var olup olmadığını kontrol et
            var ulkeExists = await _context.Ulkeler.AnyAsync(u => u.UlkeId == okul.UlkeId);
            if (!ulkeExists)
            {
                _logger.LogWarning("CreateOkul - Ülke bulunamadı: {UlkeId}", okul.UlkeId);
                return Json(new { success = false, errors = new[] { $"Seçilen ülke (ID: {okul.UlkeId}) veritabanında bulunamadı." } });
            }
            
            // ModelState hatalarını logla
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState)
                {
                    foreach (var errorMessage in error.Value.Errors)
                    {
                        _logger.LogWarning("CreateOkul - ModelState Error - Key: {Key}, Error: {Error}", error.Key, errorMessage.ErrorMessage);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(okul);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, okulId = okul.OkulId, okulAd = okul.OkulAd });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Okul eklenirken hata oluştu");
                    return Json(new { success = false, errors = new[] { "Okul eklenirken bir hata oluştu: " + ex.Message } });
                }
            }
            
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            if (!errors.Any())
            {
                errors.Add("Form validasyonu başarısız oldu.");
            }
            return Json(new { success = false, errors = errors });
        }

        // AJAX: Ülkeleri getir
        [HttpGet]
        public async Task<IActionResult> GetUlkeler()
        {
            var ulkeler = await _context.Ulkeler.OrderBy(u => u.UlkeIsim).ToListAsync();
            return Json(ulkeler.Select(u => new { ulkeId = u.UlkeId, ulkeIsim = u.UlkeIsim }));
        }
    }
}

