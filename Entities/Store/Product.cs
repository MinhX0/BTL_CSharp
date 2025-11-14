using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Entities.Store
{
    [Table("Product")]
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(150)]
        public string ProductName { get; set; } = string.Empty;

        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        [MaxLength(100)]
        public string? Brand { get; set; }

        public long Price { get; set; }

        public long? DiscountPrice { get; set; }

        public string? Description { get; set; }

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(50)]
        public string? Origin { get; set; }

        [MaxLength(50)]
        public string? MovementType { get; set; }

        [MaxLength(100)]
        public string? Material { get; set; }

        [MaxLength(255)]
        public string? Img { get; set; }

        public bool IsActive { get; set; } = true;

        public int StockQuantity { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Category? Category { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }
}
