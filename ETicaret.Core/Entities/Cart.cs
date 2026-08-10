using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ETicaret.Core.Entities
{
    internal class Cart
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Sepet bir kullanıcıya ait olmalıdır.")]
        public string UserId { get; set; } = string.Empty;

        public ICollection<CartItem> ?CartItems { get; set; }
    }
}
