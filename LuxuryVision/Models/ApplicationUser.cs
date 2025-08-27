using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace LuxuryVision.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
