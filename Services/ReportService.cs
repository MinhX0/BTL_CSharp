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

            var vm = new ProductSalesReportViewModel
            {
                Filter = filter,
                Items = grouped,
                TotalQuantity = grouped.Sum(x => x.TotalQuantity),
                TotalRevenue = grouped.Sum(x => x.TotalRevenue)
            };

            return vm;
        }
    }
}