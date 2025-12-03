using System;
using System.Linq;
using System.Threading.Tasks;
using backend.Persistance;
using backend.ViewModels.Admin;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class ReportService : IReportService
    {
        private readonly IdentityDbContext _context;

        public ReportService(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<ProductSalesReportViewModel> GetProductSalesReportAsync(ProductSalesReportFilter filter)
        {
            var query = _context.OrderDetails
                .Include(od => od.Product)
                .Include(od => od.Order)
                .AsQueryable();

            if (filter.StartDate.HasValue)
            {
                var start = filter.StartDate.Value.Date;
                query = query.Where(od => od.Order != null && od.Order.OrderDate >= start);
            }

            if (filter.EndDate.HasValue)
            {
                var end = filter.EndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(od => od.Order != null && od.Order.OrderDate <= end);
            }

            if (filter.CategoryId.HasValue)
            {
                var catId = filter.CategoryId.Value;
                query = query.Where(od => od.Product != null && od.Product.CategoryId == catId);
            }

            var grouped = await query
                .GroupBy(od => new { od.ProductId, od.Product!.ProductName })
                .Select(g => new ProductSalesItem
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => (decimal)x.Quantity * (decimal)x.Price)
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();

            // Daily aggregation within selected range (typically one month)
            var daily = await query
                .GroupBy(od => od.Order!.OrderDate.Date)
                .Select(g => new DailyRevenuePoint
                {
                    Date = g.Key,
                    TotalQuantity = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => (decimal)x.Quantity * (decimal)x.Price)
                })
                .OrderBy(p => p.Date)
                .ToListAsync();

            // Ensure full days of the selected month are present, even with zero values
            if (filter.StartDate.HasValue && filter.EndDate.HasValue)
            {
                var start = filter.StartDate.Value.Date;
                var end = filter.EndDate.Value.Date;
                var map = daily.ToDictionary(d => d.Date, d => d);
                var filled = new System.Collections.Generic.List<DailyRevenuePoint>();
                for (var d = start; d <= end; d = d.AddDays(1))
                {
                    if (map.TryGetValue(d, out var point))
                    {
                        filled.Add(point);
                    }
                    else
                    {
                        filled.Add(new DailyRevenuePoint { Date = d, TotalQuantity = 0, TotalRevenue = 0 });
                    }
                }
                daily = filled;
            }

            var vm = new ProductSalesReportViewModel
            {
                Filter = filter,
                Items = grouped,
                TotalQuantity = grouped.Sum(x => x.TotalQuantity),
                TotalRevenue = grouped.Sum(x => x.TotalRevenue),
                DailyPoints = daily
            };

            return vm;
        }
    }
}