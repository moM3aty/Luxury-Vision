using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxuryVision.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required][StringLength(200)] public string Name { get; set; }
        [Required][Column(TypeName = "decimal(18, 2)")] public decimal Price { get; set; }
        [Column(TypeName = "decimal(18, 2)")] public decimal? OldPrice { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool IsFeatured { get; set; }
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string Material { get; set; }
        public string Weight { get; set; }
        public string Dimensions { get; set; }
        public string CountryOfOrigin { get; set; }
        public string Warranty { get; set; }
    }
}
