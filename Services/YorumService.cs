using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using deneme.Data;
using deneme.Models;

namespace deneme.Services
{
    public class YorumService : IYorumService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<YorumService> _logger;

        // Constructor Injection
        public YorumService(AppDbContext context, ILogger<YorumService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Yorum> YorumEkleAsync(int erasmusId, string kullaniciAdi, string email, string yorumYazisi)
        {
            var yorum = new Yorum
            {
                KullaniciAdi = kullaniciAdi.Trim(),
                EmailAdresi = email.Trim(),
                ErasmusId = erasmusId,
                YorumYazisi = yorumYazisi.Trim()
            };

            _context.Yorumlar.Add(yorum);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Yeni yorum eklendi: {YorumId} - Program: {ErasmusId}", yorum.YorumId, erasmusId);

            return yorum;
        }

        public async Task<List<Yorum>> GetYorumlarByProgramIdAsync(int erasmusId)
        {
            return await _context.Yorumlar
                .Where(y => y.ErasmusId == erasmusId)
                .OrderByDescending(y => y.YorumId)
                .ToListAsync();
        }

        public async Task<int> GetYorumSayisiAsync(int erasmusId)
        {
            return await _context.Yorumlar
                .CountAsync(y => y.ErasmusId == erasmusId);
        }
    }
}

