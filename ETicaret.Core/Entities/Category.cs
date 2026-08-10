using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ETicaret.Core.Entities
{
    internal class Category
    {
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
        public string? Name { get; set; }

        [StringLength(200, ErrorMessage = "Açıklama en fazla 200 karakter olabilir.")]
        public string? Description { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
