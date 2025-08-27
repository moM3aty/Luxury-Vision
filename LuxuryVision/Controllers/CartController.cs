using LuxuryVision.Data;
using LuxuryVision.Extensions;
using LuxuryVision.Models;
using LuxuryVision.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuxuryVision.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CartSessionKey = "Cart";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            var viewModel = new CartViewModel
            {
                CartItems = cart
                // The line for GrandTotal has been removed because it's now calculated automatically.
            };

            return View(viewModel);
        }

        // ... (Other actions remain the same) ...
        [HttpPost]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            Product product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();

            CartItem cartItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (cartItem == null)
            {
                cart.Add(new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            HttpContext.Session.Set(CartSessionKey, cart);

            return RedirectToAction("Index");
        }

        public IActionResult Remove(int productId)
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>(CartSessionKey) ?? new List<CartItem>();
            CartItem itemToRemove = cart.FirstOrDefault(c => c.ProductId == productId);

            if (itemToRemove != null)
            {
                cart.Remove(itemToRemove);
                HttpContext.Session.Set(CartSessionKey, cart);
            }

            return RedirectToAction("Index");
        }
    }
}
