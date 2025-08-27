using LuxuryVision.Models;
using System.Collections.Generic;

namespace LuxuryVision.ViewModels
{
    public class HomeViewModel
    {
        public IEnumerable<Product> FeaturedProducts { get; set; } = new List<Product>();
        public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    }
}
