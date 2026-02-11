using Application.DTOs.Products;

namespace Application.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto> CreateAsync(CreateProductDto createProductDto);
    Task UpdateAsync(int id, UpdateProductDto updateProductDto);
    Task DeleteAsync(int id);
    
    // Para el requerimiento del reporte PDF
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold);
}