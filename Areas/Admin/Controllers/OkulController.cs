using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using deneme.Data;
using deneme.Models;
using deneme.Services;

namespace deneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class OkulController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OkulController> _logger;
        
        // Dependency Injection - Service interface'i inject edilir
        private readonly IOkulService _okulService;

        // Constructor Injection - IOkulService otomatik olarak DI container'dan gelir
        public OkulController(
            AppDbContext context, 
            ILogger<OkulController> logger,
            IOkulService okulService)
        {
            _context = context;
            _logger = logger;
            _okulService = okulService;
        }

        // GET: Admin/Okul
        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string search = "")
        {
            var query = _context.Okullar
                .Include(o => o.Ulke)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o => 
                    o.OkulAd.Contains(search) ||
                    o.Ulke!.UlkeIsim.Contains(search) ||
                    o.InternetSitesi.Contains(search));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var okullar = await query
                .OrderBy(o => o.OkulId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Search = search;
            ViewBag.TotalCount = totalCount;

            return View(okullar);
        }

        // GET: Admin/Okul/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var okul = await _context.Okullar
                .Include(o => o.Ulke)
                .Include(o => o.ErasmusProgramlari)
                    .ThenInclude(e => e.Dil)
                .Include(o => o.ErasmusProgramlari)
                    .ThenInclude(e => e.EgitimSeviyeleri)
                .FirstOrDefaultAsync(m => m.OkulId == id);

            if (okul == null)
            {
                return NotFound();
            }

            return View(okul);
        }

        // GET: Admin/Okul/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Ulkeler = new SelectList(await _context.Ulkeler.OrderBy(u => u.UlkeIsim).ToListAsync(), "UlkeId", "UlkeIsim");
            return View();
        }

        // POST: Admin/Okul/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OkulAd,InternetSitesi,UlkeId,Latitude,Longitude")] Okul okul)
        {
            // Navigation property ve UlkeId hatalarını kaldır (manuel kontrol yapacağız)
            ModelState.Remove("Ulke");
            ModelState.Remove("UlkeId");
            
            // Form'dan gelen UlkeId değerini kontrol et ve logla
            string ulkeIdFromForm = Request.Form["UlkeId"].ToString();
            _logger.LogInformation("Form'dan gelen UlkeId: '{UlkeId}', Model'deki UlkeId: {ModelUlkeId}", ulkeIdFromForm, okul.UlkeId);
            
            // Form'dan gelen değeri parse et ve kontrol et
            int selectedUlkeId = 0;
            if (!string.IsNullOrWhiteSpace(ulkeIdFromForm) && int.TryParse(ulkeIdFromForm, out int parsedId))
            {
                selectedUlkeId = parsedId;
            }
            else if (okul.UlkeId > 0)
            {
                // Model binding çalıştıysa onu kullan
                selectedUlkeId = okul.UlkeId;
            }
            
            // UlkeId kontrolü
            if (selectedUlkeId <= 0)
            {
                ModelState.AddModelError("UlkeId", "Ülke seçimi zorunludur.");
            }
            else
            {
                // Seçilen ülkenin veritabanında var olup olmadığını kontrol et
                var ulkeExists = await _context.Ulkeler.AnyAsync(u => u.UlkeId == selectedUlkeId);
                if (!ulkeExists)
                {
                    ModelState.AddModelError("UlkeId", $"Seçilen ülke (ID: {selectedUlkeId}) veritabanında bulunamadı.");
                }
                else
                {
                    // Değeri model'e ata
                    okul.UlkeId = selectedUlkeId;
                    _logger.LogInformation("UlkeId başarıyla ayarlandı: {UlkeId}", okul.UlkeId);
                }
            }

            // Koordinat kontrolü - Zorunlu
            if (!okul.Latitude.HasValue || okul.Latitude.Value == 0)
            {
                ModelState.AddModelError("Latitude", "Enlem (Latitude) zorunludur. Haritaya tıklayarak veya manuel olarak girebilirsiniz.");
            }
            else if (okul.Latitude.Value < -90 || okul.Latitude.Value > 90)
            {
                ModelState.AddModelError("Latitude", "Enlem değeri -90 ile 90 arasında olmalıdır.");
            }
            
            if (!okul.Longitude.HasValue || okul.Longitude.Value == 0)
            {
                ModelState.AddModelError("Longitude", "Boylam (Longitude) zorunludur. Haritaya tıklayarak veya manuel olarak girebilirsiniz.");
            }
            else if (okul.Longitude.Value < -180 || okul.Longitude.Value > 180)
            {
                ModelState.AddModelError("Longitude", "Boylam değeri -180 ile 180 arasında olmalıdır.");
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
                    ModelState.AddModelError("InternetSitesi", "Geçerli bir URL giriniz (örn: http://www.example.com veya https://www.example.com)");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(okul);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Okul başarıyla eklendi.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Okul eklenirken hata oluştu");
                    ModelState.AddModelError("", "Okul eklenirken bir hata oluştu: " + ex.Message);
                }
            }

            // ModelState hatalarını logla
            foreach (var error in ModelState)
            {
                foreach (var errorMessage in error.Value.Errors)
                {
                    _logger.LogWarning("ModelState Error - Key: {Key}, Error: {Error}", error.Key, errorMessage.ErrorMessage);
                }
            }

            ViewBag.Ulkeler = new SelectList(await _context.Ulkeler.OrderBy(u => u.UlkeIsim).ToListAsync(), "UlkeId", "UlkeIsim", okul.UlkeId);
            return View(okul);
        }

        // GET: Admin/Okul/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var okul = await _context.Okullar.FindAsync(id);
            if (okul == null)
            {
                return NotFound();
            }

            ViewBag.Ulkeler = new SelectList(await _context.Ulkeler.OrderBy(u => u.UlkeIsim).ToListAsync(), "UlkeId", "UlkeIsim", okul.UlkeId);
            return View(okul);
        }

        // POST: Admin/Okul/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OkulId,OkulAd,InternetSitesi,UlkeId,Latitude,Longitude")] Okul okul)
        {
            if (id != okul.OkulId)
            {
                return NotFound();
            }

            // Navigation property ve UlkeId hatalarını kaldır (manuel kontrol yapacağız)
            ModelState.Remove("Ulke");
            ModelState.Remove("UlkeId");
            
            // Form'dan gelen UlkeId değerini kontrol et
            string ulkeIdFromForm = Request.Form["UlkeId"].ToString();
            _logger.LogInformation("Edit - Form'dan gelen UlkeId: '{UlkeId}', Model'deki UlkeId: {ModelUlkeId}", ulkeIdFromForm, okul.UlkeId);
            
            // Form'dan gelen değeri parse et ve kontrol et
            int selectedUlkeId = 0;
            if (!string.IsNullOrWhiteSpace(ulkeIdFromForm) && int.TryParse(ulkeIdFromForm, out int parsedId))
            {
                selectedUlkeId = parsedId;
            }
            else if (okul.UlkeId > 0)
            {
                // Model binding çalıştıysa onu kullan
                selectedUlkeId = okul.UlkeId;
            }
            
            // UlkeId kontrolü
            if (selectedUlkeId <= 0)
            {
                ModelState.AddModelError("UlkeId", "Ülke seçimi zorunludur.");
            }
            else
            {
                // Seçilen ülkenin veritabanında var olup olmadığını kontrol et
                var ulkeExists = await _context.Ulkeler.AnyAsync(u => u.UlkeId == selectedUlkeId);
                if (!ulkeExists)
                {
                    ModelState.AddModelError("UlkeId", $"Seçilen ülke (ID: {selectedUlkeId}) veritabanında bulunamadı.");
                }
                else
                {
                    // Değeri model'e ata
                    okul.UlkeId = selectedUlkeId;
                    _logger.LogInformation("Edit - UlkeId başarıyla ayarlandı: {UlkeId}", okul.UlkeId);
                }
            }

            // Koordinat kontrolü - Zorunlu
            if (!okul.Latitude.HasValue || okul.Latitude.Value == 0)
            {
                ModelState.AddModelError("Latitude", "Enlem (Latitude) zorunludur. Haritaya tıklayarak veya manuel olarak girebilirsiniz.");
            }
            else if (okul.Latitude.Value < -90 || okul.Latitude.Value > 90)
            {
                ModelState.AddModelError("Latitude", "Enlem değeri -90 ile 90 arasında olmalıdır.");
            }
            
            if (!okul.Longitude.HasValue || okul.Longitude.Value == 0)
            {
                ModelState.AddModelError("Longitude", "Boylam (Longitude) zorunludur. Haritaya tıklayarak veya manuel olarak girebilirsiniz.");
            }
            else if (okul.Longitude.Value < -180 || okul.Longitude.Value > 180)
            {
                ModelState.AddModelError("Longitude", "Boylam değeri -180 ile 180 arasında olmalıdır.");
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
                    ModelState.AddModelError("InternetSitesi", "Geçerli bir URL giriniz (örn: http://www.example.com veya https://www.example.com)");
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(okul);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Okul başarıyla güncellendi.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OkulExists(okul.OkulId))
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

            ViewBag.Ulkeler = new SelectList(await _context.Ulkeler.OrderBy(u => u.UlkeIsim).ToListAsync(), "UlkeId", "UlkeIsim", okul.UlkeId);
            return View(okul);
        }

        // GET: Admin/Okul/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var okul = await _context.Okullar
                .Include(o => o.Ulke)
                .Include(o => o.ErasmusProgramlari)
                .FirstOrDefaultAsync(m => m.OkulId == id);

            if (okul == null)
            {
                return NotFound();
            }

            return View(okul);
        }

        // POST: Admin/Okul/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var okul = await _context.Okullar
                .Include(o => o.ErasmusProgramlari)
                .FirstOrDefaultAsync(o => o.OkulId == id);
            
            if (okul == null)
            {
                return NotFound();
            }

            // Önce bu okula ait Erasmus programlarını sil
            if (okul.ErasmusProgramlari != null && okul.ErasmusProgramlari.Any())
            {
                _context.ErasmusProgramlari.RemoveRange(okul.ErasmusProgramlari);
            }

            _context.Okullar.Remove(okul);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Okul ve bağlı Erasmus programları başarıyla silindi.";

            return RedirectToAction(nameof(Index));
        }

        private bool OkulExists(int id)
        {
            return _context.Okullar.Any(e => e.OkulId == id);
        }
    }
}

