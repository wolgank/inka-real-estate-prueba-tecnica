using Application.DTOs.Products;
using Application.Interfaces;
using Domain.Entities;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(p => MapToDto(p));
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            Stock = dto.Stock,
            Category = dto.Category
        };

        var createdProduct = await _productRepository.AddAsync(product);
        
        // Regla de negocio: Notificación si el stock inicial es bajo
        if (createdProduct.Stock < 5)
        {
            // TODO: Llamar a servicio de notificación (Email/Alert)
            Console.WriteLine($"ALERTA: Producto {createdProduct.Name} creado con stock bajo: {createdProduct.Stock}");
        }

        return MapToDto(createdProduct);
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(id);
        if (existingProduct == null) throw new KeyNotFoundException("Producto no encontrado");

        existingProduct.Name = dto.Name;
        existingProduct.Description = dto.Description;
        existingProduct.Price = dto.Price;
        existingProduct.Stock = dto.Stock;
        existingProduct.Category = dto.Category;

        await _productRepository.UpdateAsync(existingProduct);

        // Regla de negocio: Validar stock tras actualización
        if (existingProduct.Stock < 5)
        {
            // TODO: Notificar al administrador
            Console.WriteLine($"ALERTA: El stock de {existingProduct.Name} ha bajado a {existingProduct.Stock}");
        }
    }

    public async Task DeleteAsync(int id)
    {
        await _productRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync(int threshold)
    {
        var allProducts = await _productRepository.GetAllAsync();
        return allProducts
            .Where(p => p.Stock < threshold)
            .Select(p => MapToDto(p));
    }

    // Método helper de mapeo manual (para no añadir AutoMapper aún y mantenerlo simple)
    private static ProductDto MapToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Price = p.Price,
        Stock = p.Stock,
        Category = p.Category
    };
}