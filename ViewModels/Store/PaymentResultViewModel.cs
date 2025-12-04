namespace backend.ViewModels.Store
{
    public class PaymentResultViewModel
    {
        public int? OrderId { get; set; }
        public bool Success { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public string TransactionNo { get; set; } = string.Empty;
        public long? Amount { get; set; }
    }
}