using LuxuryVision.Data;
using LuxuryVision.Models;
using LuxuryVision.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace LuxuryVision.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new HomeViewModel
            {
                FeaturedProducts = await _context.Products.Where(p => p.IsFeatured).Include(p => p.Category).ToListAsync(),
                Categories = await _context.Categories.ToListAsync()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Shop(string searchString, int? categoryId, decimal? minPrice, decimal? maxPrice)
        {
            var productsQuery = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(searchString));
            }

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            var viewModel = new ShopViewModel
            {
                Products = await productsQuery.ToListAsync(),
                Categories = await _context.Categories.Include(c => c.Products).ToListAsync(),
                SearchString = searchString,
                CategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice
            };
            return View(viewModel);
        }


        public async Task<IActionResult> ProductDetails(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }


    }
}