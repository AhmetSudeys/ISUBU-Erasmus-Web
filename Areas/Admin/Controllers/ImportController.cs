using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using deneme.Data;
using deneme.Models;

namespace deneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ImportController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ImportController> _logger;
        private readonly IWebHostEnvironment _environment;

        public ImportController(AppDbContext context, ILogger<ImportController> logger, IWebHostEnvironment environment)
        {
            _context = context;
            _logger = logger;
            _environment = environment;
        }

        // GET: Admin/Import/EgitimSeviyeleri
        public IActionResult EgitimSeviyeleri()
        {
            // Proje klasöründeki Excel dosyasını kontrol et
            var excelFilePath = Path.Combine(_environment.ContentRootPath, "İkili Anlaşmalar Listesi (Güncel).xlsx");
            ViewBag.ExcelFileExists = System.IO.File.Exists(excelFilePath);
            ViewBag.ExcelFilePath = excelFilePath;
            return View();
        }

        // POST: Admin/Import/EgitimSeviyeleriFromFile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EgitimSeviyeleriFromFile()
        {
            // Proje klasöründeki Excel dosyasını kullan
            var excelFilePath = Path.Combine(_environment.ContentRootPath, "İkili Anlaşmalar Listesi (Güncel).xlsx");
            
            if (!System.IO.File.Exists(excelFilePath))
            {
                ViewBag.Error = "Excel dosyası bulunamadı. Lütfen 'İkili Anlaşmalar Listesi (Güncel).xlsx' dosyasının proje klasöründe olduğundan emin olun.";
                return View("EgitimSeviyeleri");
            }

            using (var stream = System.IO.File.OpenRead(excelFilePath))
            {
                return await ProcessExcelFile(stream);
            }
        }

        // POST: Admin/Import/EgitimSeviyeleri
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EgitimSeviyeleri(IFormFile excelFile)
        {
            if (excelFile == null || excelFile.Length == 0)
            {
                ViewBag.Error = "Lütfen bir Excel dosyası seçin.";
                return View();
            }

            // Excel dosyası uzantısını kontrol et
            var extension = Path.GetExtension(excelFile.FileName).ToLower();
            if (extension != ".xlsx" && extension != ".xls")
            {
                ViewBag.Error = "Lütfen geçerli bir Excel dosyası (.xlsx veya .xls) yükleyin.";
                return View();
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await excelFile.CopyToAsync(stream);
                    stream.Position = 0;
                    return await ProcessExcelFile(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel import hatası");
                ViewBag.Error = $"Hata oluştu: {ex.Message}";
                return View();
            }
        }

        private async Task<IActionResult> ProcessExcelFile(Stream stream)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    var rowCount = worksheet.Dimension?.Rows ?? 0;
                    var colCount = worksheet.Dimension?.Columns ?? 0;

                    if (rowCount == 0)
                    {
                        ViewBag.Error = "Excel dosyası boş görünüyor.";
                        return View("EgitimSeviyeleri");
                    }

                    // Başlık satırını bul
                    int headerRow = 1;
                    int erasmusKoduCol = -1;
                    int lisansCol = -1;
                    int yuksekLisansCol = -1;
                    int doktoraCol = -1;

                    for (int row = 1; row <= Math.Min(10, rowCount); row++)
                    {
                        for (int col = 1; col <= colCount; col++)
                        {
                            var cellValue = worksheet.Cells[row, col].Value?.ToString()?.Trim() ?? "";
                            
                            if (cellValue.Contains("Erasmus Kodu", StringComparison.OrdinalIgnoreCase) || 
                                cellValue.Contains("ErasmusKodu", StringComparison.OrdinalIgnoreCase))
                            {
                                erasmusKoduCol = col;
                                headerRow = row;
                            }
                            
                            if (cellValue.Equals("Lisans", StringComparison.OrdinalIgnoreCase))
                                lisansCol = col;
                            
                            if (cellValue.Contains("Yüksek Lisans", StringComparison.OrdinalIgnoreCase))
                                yuksekLisansCol = col;
                            
                            if (cellValue.Equals("Doktora", StringComparison.OrdinalIgnoreCase))
                                doktoraCol = col;
                        }
                        
                        if (erasmusKoduCol > 0 && lisansCol > 0 && yuksekLisansCol > 0 && doktoraCol > 0)
                            break;
                    }

                    if (erasmusKoduCol == -1 || lisansCol == -1 || yuksekLisansCol == -1 || doktoraCol == -1)
                    {
                        ViewBag.Error = "Excel dosyasında gerekli sütunlar bulunamadı.";
                        return View("EgitimSeviyeleri");
                    }

                    // Eğitim seviyesi ID'lerini al
                    var lisansId = await _context.EgitimSeviyeleri
                        .Where(es => es.EgitimSeviyesiAdi == "Lisans")
                        .Select(es => es.EgitimSeviyesiId)
                        .FirstOrDefaultAsync();
                    
                    var yuksekLisansId = await _context.EgitimSeviyeleri
                        .Where(es => es.EgitimSeviyesiAdi == "Yüksek Lisans")
                        .Select(es => es.EgitimSeviyesiId)
                        .FirstOrDefaultAsync();
                    
                    var doktoraId = await _context.EgitimSeviyeleri
                        .Where(es => es.EgitimSeviyesiAdi == "Doktora")
                        .Select(es => es.EgitimSeviyesiId)
                        .FirstOrDefaultAsync();

                    if (lisansId == 0 || yuksekLisansId == 0 || doktoraId == 0)
                    {
                        ViewBag.Error = "Veritabanında eğitim seviyeleri bulunamadı.";
                        return View("EgitimSeviyeleri");
                    }

                    int successCount = 0;
                    int errorCount = 0;
                    var errors = new List<string>();

                    for (int row = headerRow + 1; row <= rowCount; row++)
                    {
                        var erasmusKodu = worksheet.Cells[row, erasmusKoduCol].Value?.ToString()?.Trim();
                        
                        if (string.IsNullOrWhiteSpace(erasmusKodu))
                            continue;

                        var program = await _context.ErasmusProgramlari
                            .FirstOrDefaultAsync(p => p.ErasmusKodu == erasmusKodu);

                        if (program == null)
                        {
                            errorCount++;
                            errors.Add($"Erasmus Kodu '{erasmusKodu}' bulunamadı (Satır {row})");
                            continue;
                        }

                        var lisansValue = worksheet.Cells[row, lisansCol].Value?.ToString()?.Trim() ?? "";
                        var yuksekLisansValue = worksheet.Cells[row, yuksekLisansCol].Value?.ToString()?.Trim() ?? "";
                        var doktoraValue = worksheet.Cells[row, doktoraCol].Value?.ToString()?.Trim() ?? "";

                        await _context.Entry(program)
                            .Collection(p => p.EgitimSeviyeleri)
                            .LoadAsync();

                        if (lisansValue == "*" || lisansValue.Contains("*"))
                        {
                            var lisansSeviyesi = await _context.EgitimSeviyeleri.FindAsync(lisansId);
                            if (lisansSeviyesi != null && !program.EgitimSeviyeleri.Any(es => es.EgitimSeviyesiId == lisansId))
                            {
                                program.EgitimSeviyeleri.Add(lisansSeviyesi);
                            }
                        }

                        if (yuksekLisansValue == "*" || yuksekLisansValue.Contains("*"))
                        {
                            var yuksekLisansSeviyesi = await _context.EgitimSeviyeleri.FindAsync(yuksekLisansId);
                            if (yuksekLisansSeviyesi != null && !program.EgitimSeviyeleri.Any(es => es.EgitimSeviyesiId == yuksekLisansId))
                            {
                                program.EgitimSeviyeleri.Add(yuksekLisansSeviyesi);
                            }
                        }

                        if (doktoraValue == "*" || doktoraValue.Contains("*"))
                        {
                            var doktoraSeviyesi = await _context.EgitimSeviyeleri.FindAsync(doktoraId);
                            if (doktoraSeviyesi != null && !program.EgitimSeviyeleri.Any(es => es.EgitimSeviyesiId == doktoraId))
                            {
                                program.EgitimSeviyeleri.Add(doktoraSeviyesi);
                            }
                        }

                        successCount++;
                        
                        if (successCount % 100 == 0)
                        {
                            await _context.SaveChangesAsync();
                        }
                    }

                    await _context.SaveChangesAsync();

                    ViewBag.Success = $"İşlem tamamlandı! {successCount} program başarıyla işlendi.";
                    if (errorCount > 0)
                    {
                        ViewBag.Warning = $"{errorCount} program işlenirken hata oluştu.";
                        ViewBag.Errors = errors.Take(20).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excel import hatası");
                ViewBag.Error = $"Hata oluştu: {ex.Message}";
            }

            return View("EgitimSeviyeleri");
        }
    }
}
