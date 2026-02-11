using Application.DTOs.Products;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;

namespace Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;

    public ProductService(IProductRepository productRepository, IValidator<CreateProductDto> createValidator, IValidator<UpdateProductDto> updateValidator, IEmailService emailService, IUserRepository userRepository)
    {
        _productRepository = productRepository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _emailService = emailService;
        _userRepository = userRepository;
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
        var validationResult = await _createValidator.ValidateAsync(dto);
        if (!validationResult.IsValid) 
            throw new ValidationException(validationResult.Errors);
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
            await NotifyAdminsIfStockLow(createdProduct);
            Console.WriteLine($"ALERTA: Producto {createdProduct.Name} creado con stock bajo: {createdProduct.Stock}");
        }

        return MapToDto(createdProduct);
    }

    public async Task UpdateAsync(int id, UpdateProductDto dto)
    {
        var validationResult = await _updateValidator.ValidateAsync(dto);
        if (!validationResult.IsValid) 
            throw new ValidationException(validationResult.Errors);
        
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
            await NotifyAdminsIfStockLow(existingProduct);
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
    private async Task NotifyAdminsIfStockLow(Product product)
    {
        if (product.Stock < 5)
        {
            var adminEmails = await _userRepository.GetAdminEmailsAsync();
            if (adminEmails.Any())
            {
                await _emailService.SendLowStockAlertAsync(product.Name, product.Stock, adminEmails);
            }
        }
    }
}