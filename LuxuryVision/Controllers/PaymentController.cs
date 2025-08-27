using LuxuryVision.Data;
using LuxuryVision.Extensions;
using LuxuryVision.Models;
using LuxuryVision.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LuxuryVision.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMoyasarService _moyasarService;

        public PaymentController(ApplicationDbContext context, IMoyasarService moyasarService)
        {
            _context = context;
            _moyasarService = moyasarService;
        }

        public IActionResult Index()
        {
            var pendingOrder = HttpContext.Session.Get<Order>("PendingOrder");
            if (pendingOrder == null)
            {
                return RedirectToAction("Index", "Cart");
            }
            return View(pendingOrder);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayment(string paymentMethod)
        {
            var order = HttpContext.Session.Get<Order>("PendingOrder");
            if (order == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            order.PaymentMethod = paymentMethod;

            if (paymentMethod == "Moyasar")
            {
                string redirectUrl = await _moyasarService.CreatePaymentAsync(order);
                if (!string.IsNullOrEmpty(redirectUrl))
                {
                    return Redirect(redirectUrl);
                }
                else
                {
                    TempData["PaymentError"] = "حدث خطأ أثناء تهيئة عملية الدفع. يرجى المحاولة مرة أخرى.";
                    return View("Index", order);
                }
            }
            else // Handle other simulated payment methods (like QR, Saree, etc.)
            {
                // UPDATED: Change status to "Awaiting Payment" for simulated methods
                order.OrderStatus = "بانتظار الدفع";
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("PendingOrder");

                return RedirectToAction("OrderConfirmation", "Checkout", new { id = order.Id });
            }
        }

        // Moyasar redirects back to this URL after payment attempt.
        public async Task<IActionResult> PaymentCallback(string id, string status, string message)
        {
            var order = HttpContext.Session.Get<Order>("PendingOrder");
            if (order == null)
            {
                return RedirectToAction("Index", "Cart");
            }

            if (status == "paid")
            {
                order.OrderStatus = "قيد المراجعة";
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("Cart");
                HttpContext.Session.Remove("PendingOrder");

                return RedirectToAction("OrderConfirmation", "Checkout", new { id = order.Id });
            }
            else
            {
                TempData["PaymentError"] = $"فشلت عملية الدفع: {message}. يرجى المحاولة مرة أخرى.";
                return RedirectToAction("Index");
            }
        }
    }
}