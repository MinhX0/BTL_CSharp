namespace backend.ViewModels.Home
{
    public class ProductDetailViewModel
    {
        public int ProductId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? CategoryName { get; init; }
        public string? Brand { get; init; }
        public long Price { get; init; }
        public long? OriginalPrice { get; init; }
        public double Discount { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? Gender { get; init; }
        public string? Origin { get; init; }
        public string? MovementType { get; init; }
        public string? Material { get; init; }
        public int StockQuantity { get; init; }
        public IReadOnlyList<ProductSpecificationViewModel> Specifications { get; init; } = new List<ProductSpecificationViewModel>();
        public IReadOnlyList<ProductCardViewModel> RelatedProducts { get; init; } = new List<ProductCardViewModel>();

        public bool InStock => StockQuantity > 0;
        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice.Value > Price;
    }
}
