using System.ComponentModel.DataAnnotations;
namespace Pronia.Models;

public class Card
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Bos olmasin!")]
    public string ImageUrl { get; set; } = null!;
    [Required(ErrorMessage = "Yaz daaaa !!!!")]
    [MaxLength(32)]
    [MinLength(5)]
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}