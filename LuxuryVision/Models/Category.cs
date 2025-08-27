using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuxuryVision.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Required][StringLength(100)] public string Name { get; set; }
        public string ImageUrl { get; set; } 
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
