using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxuryVision.Models
{
    public class ShippingZone
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "اسم المنطقة مطلوب")][StringLength(150)][Display(Name = "اسم المنطقة (المدينة)")] public string ZoneName { get; set; }
        [Required(ErrorMessage = "سعر الشحن مطلوب")][Range(0, 10000)][Display(Name = "سعر الشحن (ريال)")][Column(TypeName = "decimal(18, 2)")] public decimal ShippingCost { get; set; }
    }
}
