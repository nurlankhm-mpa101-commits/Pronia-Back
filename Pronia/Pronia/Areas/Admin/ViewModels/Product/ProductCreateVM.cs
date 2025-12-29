using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Admin.ViewModels.Product
{
    public class ProductCreateVM
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required, Range(0.01, 1000000)]
        public decimal Price { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;

        [Required]
        public int CategoryId { get; set; }

        public List<int> TagIds { get; set; } = new();
    }
}