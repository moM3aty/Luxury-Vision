using System;
using System.ComponentModel.DataAnnotations;

namespace LuxuryVision.Models
{
    // نموذج بيانات مقال المدونة
    public class BlogPost
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان المقال مطلوب")]
        [Display(Name = "العنوان")]
        [StringLength(200)]
        public string Title { get; set; }

        [Required(ErrorMessage = "محتوى المقال مطلوب")]
        [Display(Name = "المحتوى")]
        public string Content { get; set; }

        [Display(Name = "وصف مختصر")]
        [StringLength(500)]
        public string Summary { get; set; }

        [Display(Name = "صورة المقال")]
        public string ImageUrl { get; set; }

        [Display(Name = "تاريخ النشر")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "نُشر بواسطة")]
        public string Author { get; set; }
    }
}