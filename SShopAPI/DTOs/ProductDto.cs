namespace SShopAPI.DTOs
{
    public class ProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string? Type { get; set; }
        public int Quantity { get; set; } = 0;
        public bool IsVisible { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}