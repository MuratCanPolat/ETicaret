using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ETicaret.Core.Entities
{
    public class Category
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Kategori adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kategori adı en fazla 50 karakter olabilir.")]
        public string? Name { get; set; }
        public ICollection<Product>? Products { get; set; }
    }
}
