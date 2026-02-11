using Application.Services;
using Application.Interfaces;
using Application.DTOs.Products;
using Application.Validators; // Importar validador
using Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CreateProductValidator _validator; 
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _validator = new CreateProductValidator(); // Validador real
        _productService = new ProductService(_productRepositoryMock.Object, _validator);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenStockIsNegative()
    {
        // Arrange
        var dto = new CreateProductDto 
        { 
            Name = "Producto Invalido", 
            Price = 100, 
            Stock = -5, // <--- Stock inválido
            Category = "Herramientas" 
        };

        // Act & Assert
        await Assert.ThrowsAsync<ValidationException>(() => _productService.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenDataIsValid()
    {
        // Arrange
        var dto = new CreateProductDto { Name = "Laptop", Price = 1500, Stock = 10, Category = "Tech" };
        var product = new Product { Id = 1, Name = "Laptop", Price = 1500, Stock = 10 };
        
        _productRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        // Act
        var result = await _productService.CreateAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
    }
}