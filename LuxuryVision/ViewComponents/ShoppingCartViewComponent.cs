using LuxuryVision.Extensions;
using LuxuryVision.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace LuxuryVision.ViewComponents
{
    public class ShoppingCartViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            List<CartItem> cart = HttpContext.Session.Get<List<CartItem>>("Cart") ?? new List<CartItem>();
            int cartCount = cart.Sum(item => item.Quantity);

            return View(cartCount);
        }
    }
}
