using LuxuryVision.Models;
using System.Collections.Generic;

namespace LuxuryVision.ViewModels
{
    public class ShopViewModel
    {
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string SearchString { get; set; }
        public int? CategoryId { get; set; }
        public decimal? MinPrice { get; set; } 
        public decimal? MaxPrice { get; set; } 
    }
}