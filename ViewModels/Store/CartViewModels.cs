using System.Collections.Generic;

namespace backend.ViewModels.Store
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int Quantity { get; set; }
        public long UnitPrice { get; set; }
        public long LineTotal { get; set; }
        public string? CategoryName { get; set; }
    }

    public class CartViewModel
    {
        public IList<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public long Subtotal => Items.Sum(i => i.LineTotal);
        public long Total => Subtotal; // extend for tax/shipping later
    }

    public class CheckoutViewModel
    {
        public IList<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public string ShippingAddress { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public long Total => Items.Sum(i => i.LineTotal);
    }
}
