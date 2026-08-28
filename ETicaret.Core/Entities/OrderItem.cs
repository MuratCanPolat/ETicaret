using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ETicaret.Core.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public decimal UnitPrice { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [StringLength(50)]
        public string ItemStatus { get; set; } = "Hazırlanıyor";

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Order? Order { get; set; } 
        public Product? Product { get; set; }
    }
}
