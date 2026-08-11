using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ETicaret.Core.Entities
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Ürün adedi zorunludur.")]
        [Range(1, 100, ErrorMessage = "Lütfen en az 1, en fazla 100 adet seçiniz.")]
        public int Quantity { get; set; }

        [Required]
        public int CartId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }
}
