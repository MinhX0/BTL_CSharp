namespace backend.ViewModels.Account
{
    public class OrderViewModel
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public long TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public int ItemCount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? ShippingAddress { get; set; }
    }
}
