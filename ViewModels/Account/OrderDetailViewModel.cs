namespace backend.ViewModels.Account
{
    public class OrderDetailViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public long TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string? ShippingAddress { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public List<OrderDetailItemViewModel> Items { get; set; } = new List<OrderDetailItemViewModel>();
    }

    public class OrderDetailItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductImage { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
        public long LineTotal => Price * Quantity;
    }
}
