using System;
using System.Threading.Tasks;
using backend.Services;
using backend.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Admin
{
    [Route("Admin/Report")] 
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("Sales")]
        public async Task<IActionResult> Sales([FromQuery] int? month, [FromQuery] int? year, [FromQuery] int? categoryId)
        {
            var filter = new ProductSalesReportFilter { Month = month, Year = year, CategoryId = categoryId };

            // Compute date range if month/year provided
            if (month.HasValue && year.HasValue && month.Value >= 1 && month.Value <= 12)
            {
                var first = new DateTime(year.Value, month.Value, 1);
                var last = first.AddMonths(1).AddTicks(-1);
                filter.StartDate = first;
                filter.EndDate = last;
            }

            var vm = await _reportService.GetProductSalesReportAsync(filter);
            return View("~/Views/Admin/SalesReport.cshtml", vm);
        }
    }
}