using System.Collections.Generic;
using backend.Entities.Store;

namespace backend.ViewModels.Admin
{
    public class ProductImportViewModel
    {
        public int? SelectedProductId { get; set; }
        public int QuantityToAdd { get; set; }
        public IList<Product> Products { get; set; } = new List<Product>();
        public string? Message { get; set; }
        public string? Error { get; set; }
    }
}