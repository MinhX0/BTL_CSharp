using System;
using System.Threading.Tasks;
using backend.Services;
using backend.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers.Admin
{
    [Route("admin/report")] 
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("sales")]
        public async Task<IActionResult> Sales([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] int? categoryId)
        {
            var filter = new ProductSalesReportFilter
            {
                StartDate = startDate,
                EndDate = endDate,
                CategoryId = categoryId
            };

            var vm = await _reportService.GetProductSalesReportAsync(filter);
            return View("~/Views/Admin/SalesReport.cshtml", vm);
        }
    }
}