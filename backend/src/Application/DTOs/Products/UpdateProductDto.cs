using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Products;

public class UpdateProductDto : CreateProductDto
{
    // Heredamos de CreateProductDto porque las validaciones son las mismas.
}