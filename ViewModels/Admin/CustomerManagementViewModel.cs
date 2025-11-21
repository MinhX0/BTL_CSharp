namespace backend.ViewModels.Admin
{
    public class CustomerListViewModel
    {
        public List<CustomerItemViewModel> Customers { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string? SearchTerm { get; set; }
    }

    public class CustomerItemViewModel
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Username { get; set; }
        public string? Phone { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
