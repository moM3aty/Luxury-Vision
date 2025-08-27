using LuxuryVision.Data;
using LuxuryVision.Extensions;
using LuxuryVision.Models;
using LuxuryVision.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

[Authorize] 
public class CheckoutController : Controller
{
    private readonly ApplicationDbContext _context;

    public CheckoutController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        if (cart.Count == 0)
        {
            TempData["ErrorMessage"] = "سلة المشتريات فارغة.";
            return RedirectToAction("Index", "Cart");
        }

        var checkoutViewModel = new CheckoutViewModel
        {
            Order = new Order(),
            CartItems = cart,
            ShippingZones = await _context.ShippingZones.ToListAsync()
        };

        return View(checkoutViewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel checkoutViewModel)
    {
        ModelState.Remove("Order.OrderStatus");
        ModelState.Remove("Order.PaymentMethod");
        ModelState.Remove("Order.ApplicationUserId");
        ModelState.Remove("Order.ApplicationUser");
        ModelState.Remove("Order.ShippingZone");
        ModelState.Remove("Order.OrderDetails");
        List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
        checkoutViewModel.CartItems = cart;
        checkoutViewModel.ShippingZones = await _context.ShippingZones.ToListAsync();


        if (ModelState.IsValid)
        {
            var shippingZone = await _context.ShippingZones.FindAsync(checkoutViewModel.Order.ShippingZoneId);
            if (shippingZone == null)
            {
                ModelState.AddModelError("", "الرجاء اختيار منطقة شحن صالحة.");
                return View("Index", checkoutViewModel);
            }

            Order order = checkoutViewModel.Order;
            order.OrderDate = DateTime.Now;
            order.OrderStatus = "بانتظار الدفع";
            order.Subtotal = cart.Sum(x => x.Total);
            order.ShippingCost = shippingZone.ShippingCost;
            order.TotalAmount = order.Subtotal + order.ShippingCost;

            order.ApplicationUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            order.OrderDetails = new List<OrderDetail>();

            foreach (var item in cart)
            {
                order.OrderDetails.Add(new OrderDetail
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Price
                });
            }

            HttpContext.Session.Set("PendingOrder", order);
            return RedirectToAction("Index", "Payment");
        }

        return View("Index", checkoutViewModel);
    }

  
    public async Task<IActionResult> OrderConfirmation(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }
}
