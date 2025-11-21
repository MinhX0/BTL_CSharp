using System.Collections.Generic;

namespace backend.ViewModels.Home
{
    public class ShopViewModel
    {
        public IReadOnlyList<ProductCardViewModel> Products { get; init; } = new List<ProductCardViewModel>();
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int TotalCount => Products.Count;
    }
}
