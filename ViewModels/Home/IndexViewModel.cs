using System.Collections.Generic;

namespace backend.ViewModels.Home
{
    public class IndexViewModel
    {
        public IReadOnlyList<CategorySummaryViewModel> Categories { get; init; } = new List<CategorySummaryViewModel>();
        public IReadOnlyList<ProductCardViewModel> TrendingProducts { get; init; } = new List<ProductCardViewModel>();
        public IReadOnlyList<ProductCardViewModel> NewArrivals { get; init; } = new List<ProductCardViewModel>();
    }
}
