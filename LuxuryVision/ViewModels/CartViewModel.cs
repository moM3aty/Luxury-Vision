using LuxuryVision.Models;
using System.Collections.Generic;
using System.Linq;

namespace LuxuryVision.ViewModels
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();

        public decimal GrandTotal
        {
            get { return CartItems.Sum(x => x.Total); }
        }
    }
}
