using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using deneme.Data;
using deneme.Models;

namespace deneme.Services
{
    public class OkulService : IOkulService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OkulService> _logger;

        // Constructor Injection
        public OkulService(AppDbContext context, ILogger<OkulService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Okul>> GetAllOkullarAsync()
        {
            return await _context.Okullar
                .Include(o => o.Ulke)
                .OrderBy(o => o.OkulAd)
                .ToListAsync();
        }

        public async Task<Okul?> GetOkulByIdAsync(int id)
        {
            return await _context.Okullar
                .Include(o => o.Ulke)
                .FirstOrDefaultAsync(o => o.OkulId == id);
        }

        public async Task<Okul> OkulEkleAsync(string okulAd, int ulkeId, string internetSitesi, double? latitude, double? longitude)
        {
            var okul = new Okul
            {
                OkulAd = okulAd.Trim(),
                UlkeId = ulkeId,
                InternetSitesi = internetSitesi.Trim(),
                Latitude = latitude,
                Longitude = longitude
            };

            _context.Okullar.Add(okul);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yeni okul eklendi: {OkulId} - {OkulAd}", okul.OkulId, okul.OkulAd);

            return okul;
        }

        public async Task<bool> OkulExistsAsync(int id)
        {
            return await _context.Okullar.AnyAsync(o => o.OkulId == id);
        }
    }
}

