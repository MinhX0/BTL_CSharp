namespace backend.ViewModels.Home
{
    public class ProductCardViewModel
    {
        public int ProductId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? CategoryName { get; init; }
        public string ImageUrl { get; init; } = string.Empty;
        public long Price { get; init; }
        public long? OriginalPrice { get; init; }
        public double Discount { get; init; }

        public bool HasDiscount => OriginalPrice.HasValue && OriginalPrice.Value > Price;
    }
}
