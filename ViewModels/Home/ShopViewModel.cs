using System.Collections.Generic;

namespace backend.ViewModels.Home
{
    public class ShopViewModel
    {
        public IReadOnlyList<ProductCardViewModel> Products { get; init; } = new List<ProductCardViewModel>();
    }
}
