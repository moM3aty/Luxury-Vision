using LuxuryVision.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LuxuryVision.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MyOrders()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context.Orders
                                       .Where(o => o.ApplicationUserId == userId)
                                       .OrderByDescending(o => o.OrderDate)
                                       .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> OrderDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                                      .Include(o => o.OrderDetails)
                                      .ThenInclude(od => od.Product)
                                      .Include(o => o.ShippingZone)
                                      .FirstOrDefaultAsync(o => o.Id == id && o.ApplicationUserId == userId);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }
    }
}