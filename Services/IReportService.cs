using System;
using System.Threading.Tasks;
using backend.ViewModels.Admin;

namespace backend.Services
{
    public interface IReportService
    {
        Task<ProductSalesReportViewModel> GetProductSalesReportAsync(ProductSalesReportFilter filter);
    }
}