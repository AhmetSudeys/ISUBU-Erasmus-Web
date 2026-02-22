using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deneme.Data;
using deneme.Models;

namespace deneme.ViewComponents
{
    public class SonYorumlarViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public SonYorumlarViewComponent(AppDbContext context)
        {
            _context = context;
        }

        // madde 24 - ViewComponent 
        public async Task<IViewComponentResult> InvokeAsync(int count = 5)
        {
            var yorumlar = await _context.Yorumlar
                .Include(y => y.ErasmusProgrami)
                    .ThenInclude(e => e!.Okul)
                .OrderByDescending(y => y.YorumId)
                .Take(count)
                .ToListAsync();

            return View(yorumlar);
        }
    }
}

