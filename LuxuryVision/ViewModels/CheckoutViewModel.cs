using LuxuryVision.Models;
using System.Collections.Generic;
using System.Linq;

namespace LuxuryVision.ViewModels
{
    public class CheckoutViewModel
    {
        public Order Order { get; set; }
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public IEnumerable<ShippingZone> ShippingZones { get; set; } = new List<ShippingZone>();

        public decimal ItemsTotal
        {
            get { return CartItems.Sum(x => x.Total); }
        }
    }
}
