using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ETicaret.Core.Entities
{
    internal class ApplicationUser
    {
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public ICollection<Product> Products { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
