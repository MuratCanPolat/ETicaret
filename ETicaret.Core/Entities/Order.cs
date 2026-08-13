using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ETicaret.Core.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı bilgisi eksik.")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Lütfen teslimat adresi giriniz.")]
        [StringLength(500, ErrorMessage = "Adres bilgisi en fazla 500 karakter olabilir.")]
        public string ShippingAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string OrderStatus { get; set; } = "Pending";

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
