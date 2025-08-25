using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string PictureUrl { get; set; } = string.Empty;
         public required string Brand { get; set; }
        public int Quantity { get; set; } = 0;
        [Required]
        public bool IsVisible { get; set; }
        public DateTime CreatedAt { get; set; }
        //public required string UpdatedBy { get; set; }
    }
}