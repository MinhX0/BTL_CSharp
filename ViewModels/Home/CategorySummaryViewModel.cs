namespace backend.ViewModels.Home
{
    public class CategorySummaryViewModel
    {
        public int CategoryId { get; init; }
        public string Name { get; init; } = string.Empty;
        public int ProductCount { get; init; }
        public string? ImageUrl { get; init; }
    }
}
