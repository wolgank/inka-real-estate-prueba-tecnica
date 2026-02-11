using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public class CreateProductDto
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
    public int Stock { get; set; }

    [Required(ErrorMessage = "La categoría es obligatoria")]
    public string Category { get; set; } = string.Empty;
}