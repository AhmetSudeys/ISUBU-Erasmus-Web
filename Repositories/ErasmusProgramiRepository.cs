using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using deneme.Data;
using deneme.Models;

namespace deneme.Repositories
{
    public class ErasmusProgramiRepository : IErasmusProgramiRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ErasmusProgramiRepository> _logger;

        // Constructor Injection
        public ErasmusProgramiRepository(AppDbContext context, ILogger<ErasmusProgramiRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ErasmusProgrami?> GetByIdAsync(int id)
        {
            return await _context.ErasmusProgramlari
                .Include(e => e.Okul)
                    .ThenInclude(o => o!.Ulke)
                .Include(e => e.Dil)
                .Include(e => e.EgitimSeviyeleri)
                .Include(e => e.Yorumlar)
                .FirstOrDefaultAsync(e => e.ErasmusId == id);
        }

        public async Task<List<ErasmusProgrami>> GetAllAsync()
        {
            return await _context.ErasmusProgramlari
                .Include(e => e.Okul)
                    .ThenInclude(o => o!.Ulke)
                .Include(e => e.Dil)
                .Include(e => e.EgitimSeviyeleri)
                .OrderBy(e => e.ErasmusId)
                .ToListAsync();
        }

        public async Task<List<ErasmusProgrami>> SearchAsync(string searchTerm)
        {
            var term = searchTerm.Trim();
            return await _context.ErasmusProgramlari
                .Include(e => e.Okul)
                    .ThenInclude(o => o!.Ulke)
                .Include(e => e.Dil)
                .Include(e => e.EgitimSeviyeleri)
                .Where(e =>
                    (e.Okul!.OkulAd != null && e.Okul.OkulAd.Contains(term)) ||
                    (e.Okul.Ulke!.UlkeIsim != null && e.Okul.Ulke.UlkeIsim.Contains(term)) ||
                    (e.BolumAdi != null && e.BolumAdi.Contains(term)) ||
                    (e.ErasmusKodu != null && e.ErasmusKodu.Contains(term)) ||
                    (e.Dil!.DilAdi != null && e.Dil.DilAdi.Contains(term))
                    // Not: EgitimSeviyeleri koleksiyonu üzerinde arama yapılamıyor (EF Core çeviri hatası)
                )
                .ToListAsync();
        }

        public async Task<ErasmusProgrami> AddAsync(ErasmusProgrami program)
        {
            _context.ErasmusProgramlari.Add(program);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Yeni Erasmus programı eklendi: {ErasmusId}", program.ErasmusId);
            return program;
        }

        public async Task UpdateAsync(ErasmusProgrami program)
        {
            _context.ErasmusProgramlari.Update(program);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Erasmus programı güncellendi: {ErasmusId}", program.ErasmusId);
        }

        public async Task DeleteAsync(int id)
        {
            var program = await _context.ErasmusProgramlari.FindAsync(id);
            if (program != null)
            {
                _context.ErasmusProgramlari.Remove(program);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Erasmus programı silindi: {ErasmusId}", id);
            }
        }
    }
}

