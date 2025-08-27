using LuxuryVision.Models;
using Microsoft.AspNetCore.Http;

namespace LuxuryVision.ViewModels
{
    public class CategoryFormViewModel
    {
        public Category Category { get; set; }
        public IFormFile ImageFile { get; set; }
    }
}
