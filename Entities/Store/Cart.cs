using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Entities.Store
{
    [Table("Cart")]
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [ForeignKey(nameof(Customer))]
        public int CustomerId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime AddedDate { get; set; } = DateTime.UtcNow;

        public Customer? Customer { get; set; }

        public Product? Product { get; set; }
    }
}
