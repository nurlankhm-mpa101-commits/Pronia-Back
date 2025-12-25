using System.ComponentModel.DataAnnotations;

namespace Pronia.Models;

public class Card : BaseEntity
{
    [Required(ErrorMessage = "Поле ImageUrl не может быть пустым!")]
    public string ImageUrl { get; set; } = null!;

    [Required(ErrorMessage = "Поле Title обязательно!")]
    [MaxLength(32, ErrorMessage = "Title не может быть длиннее 32 символов.")]
    [MinLength(5, ErrorMessage = "Title не может быть короче 5 символов.")]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required(ErrorMessage = "Выберите категорию!")]
    public int CategoryId { get; set; }

    public Category? Category { get; set; } = null!;
}