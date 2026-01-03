using System.ComponentModel.DataAnnotations;

namespace Pronia.Areas.Admin.ViewModels.Product
{
    public class ProductUpdateVM
    {
        public int Id { get; set; }
        
        [Required, MaxLength(100)]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public decimal Price { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        // Список ID выбранных тегов
        public List<int>? TagIds { get; set; }
        
        // Путь к текущей картинке, чтобы показать её в форме
        public string? ExistingImagePath { get; set; }
        
        // Новая картинка, которую загрузит пользователь
        public IFormFile? ImageFile { get; set; }
    }
}