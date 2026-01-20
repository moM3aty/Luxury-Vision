using LuxuryVision.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LuxuryVision.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BlogController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context.BlogPosts
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
            return View(posts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var post = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }
    }
}