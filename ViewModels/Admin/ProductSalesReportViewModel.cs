using System;
using System.Collections.Generic;

namespace backend.ViewModels.Admin
{
    public class DailyRevenuePoint
    {
        public DateTime Date { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }
    public class ProductSalesItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class ProductSalesReportFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? CategoryId { get; set; }
    }

    public class ProductSalesReportViewModel
    {
        public ProductSalesReportFilter Filter { get; set; } = new ProductSalesReportFilter();
        public IList<ProductSalesItem> Items { get; set; } = new List<ProductSalesItem>();
        public int TotalQuantity { get; set; }
        public decimal TotalRevenue { get; set; }
        public IList<DailyRevenuePoint> DailyPoints { get; set; } = new List<DailyRevenuePoint>();
    }
}