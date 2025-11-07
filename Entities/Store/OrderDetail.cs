using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Entities.Store
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public Order? Order { get; set; }

        public Product? Product { get; set; }
    }
}
