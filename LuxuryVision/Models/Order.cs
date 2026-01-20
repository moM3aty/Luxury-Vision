using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LuxuryVision.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "الاسم الأخير مطلوب")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        public string Address { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [StringLength(100)]
        public string City { get; set; }

        [Required(ErrorMessage = "رقم الجوال مطلوب")]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress]
        public string Email { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal ShippingCost { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }

        [Required(ErrorMessage = "الرجاء اختيار منطقة الشحن")]
        [Display(Name = "منطقة الشحن")]
        public int ShippingZoneId { get; set; }
        public virtual ShippingZone ShippingZone { get; set; }
        public string ApplicationUserId { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}