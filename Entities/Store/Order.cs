using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Entities.Store
{
    [Table("Order")]
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public long TotalAmount { get; set; }

        [MaxLength(50)]
        public string? PaymentMethod { get; set; }

        [MaxLength(255)]
        public string? ShippingAddress { get; set; }

        [MaxLength(50)]
        public string Status { get; set; } = "Pending";

        public Customer? Customer { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
