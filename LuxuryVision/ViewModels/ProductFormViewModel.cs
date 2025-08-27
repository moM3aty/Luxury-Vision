using LuxuryVision.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace LuxuryVision.ViewModels
{
    public class ProductFormViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
