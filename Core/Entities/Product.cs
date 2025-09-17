using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
        public string Brand { get; set; }
        public int Quantity { get; set; } = 0;

        [Required]
        public bool IsVisible { get; set; }

        //public DateTime CreatedAt { get; set; }
        //public required string UpdatedBy { get; set; }
    }
}