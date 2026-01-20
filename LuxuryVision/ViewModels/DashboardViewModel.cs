using LuxuryVision.Models;
using System.Collections.Generic;

namespace LuxuryVision.ViewModels
{
    public class DashboardViewModel
    {
        public int ProductCount { get; set; }
        public int CategoryCount { get; set; }
        public int ShippingZoneCount { get; set; }
        public int OrderCount { get; set; }

        public IEnumerable<Order> RecentOrders { get; set; }
    }
}
