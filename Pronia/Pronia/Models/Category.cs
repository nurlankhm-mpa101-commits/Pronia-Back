using System.ComponentModel.DataAnnotations;

namespace Pronia.Models
{
    public class Category
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [MaxLength(50)]
        public string Name { get; set; } = null!;
    }
}