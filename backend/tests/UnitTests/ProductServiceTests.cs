using Application.Services;
using Application.Interfaces;
using Application.DTOs.Products;
using Application.Validators;
using Domain.Entities;
using FluentValidation;
using Moq;
using Xunit;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly CreateProductValidator _createValidator; 
    private readonly UpdateProductValidator _updateValidator;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _createValidator = new CreateProductValidator();
        _updateValidator = new UpdateProductValidator();
        
        _productService = new ProductService(
            _productRepositoryMock.Object, 
            _createValidator, 
            _updateValidator);
    }

    #region Create Tests
    [Fact]
    public async Task CreateAsync_ShouldThrowValidationException_WhenStockIsNegative()
    {
        var dto = new CreateProductDto 
        { 
            Name = "Producto Invalido", 
            Price = 100, 
            Stock = -5, 
            Category = "Herramientas" 
        };

        await Assert.ThrowsAsync<ValidationException>(() => _productService.CreateAsync(dto));
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnDto_WhenDataIsValid()
    {
        var dto = new CreateProductDto { Name = "Laptop", Price = 1500, Stock = 10, Category = "Tech" };
        var product = new Product { Id = 1, Name = "Laptop", Price = 1500, Stock = 10, Category = "Tech" };
        
        _productRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(product);

        var result = await _productService.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Name, result.Name);
        _productRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
    }
    #endregion

    #region Update Tests
    [Fact]
    public async Task UpdateAsync_ShouldThrowKeyNotFoundException_WhenProductDoesNotExist()
    {
        var dto = new UpdateProductDto { Name = "Editado", Price = 10, Stock = 10, Category = "C1" };
        _productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Product?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _productService.UpdateAsync(99, dto));
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowValidationException_WhenDataIsInvalid()
    {
        var dto = new UpdateProductDto { Name = "", Price = -1, Stock = 10, Category = "" };
        
        await Assert.ThrowsAsync<ValidationException>(() => _productService.UpdateAsync(1, dto));
    }
    #endregion

    #region Delete Tests
    [Fact]
    public async Task DeleteAsync_ShouldCallRepository_WhenIdIsProvided()
    {
        int idToDelete = 1;

        await _productService.DeleteAsync(idToDelete);

        _productRepositoryMock.Verify(x => x.DeleteAsync(idToDelete), Times.Once);
    }
    #endregion

    #region Low Stock Tests
    [Fact]
    public async Task GetLowStockProductsAsync_ShouldReturnOnlyFilteredProducts()
    {
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Bajo", Stock = 2 },
            new Product { Id = 2, Name = "Alto", Stock = 20 }
        };
        _productRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

        var result = await _productService.GetLowStockProductsAsync(5);

        Assert.Single(result);
        Assert.Equal("Bajo", result.First().Name);
    }
    #endregion
}