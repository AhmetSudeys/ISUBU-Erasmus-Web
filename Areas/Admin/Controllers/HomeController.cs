using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using deneme.Data;
using deneme.Models;

namespace deneme.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(AppDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var stats = new
            {
                UlkeSayisi = await _context.Ulkeler.CountAsync(),
                OkulSayisi = await _context.Okullar.CountAsync(),
                ProgramSayisi = await _context.ErasmusProgramlari.CountAsync(),
                YorumSayisi = await _context.Yorumlar.CountAsync()
            };

            ViewBag.Stats = stats;
            return View();
        }
    }
}

