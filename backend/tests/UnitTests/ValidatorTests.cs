using Application.DTOs.Products;
using Application.Validators;
using Xunit;

namespace UnitTests;

public class ValidatorTests
{
    private readonly CreateProductValidator _createValidator = new();

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void CreateProduct_ShouldHaveError_WhenStockIsNegative(int invalidStock)
    {
        // Arrange
        var model = new CreateProductDto { Name = "Test", Price = 10, Stock = invalidStock, Category = "C1" };

        // Act
        var result = _createValidator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Stock");
    }

    [Fact]
    public void CreateProduct_ShouldHaveError_WhenPriceIsZeroOrNegative()
    {
        var model = new CreateProductDto { Name = "Test", Price = 0, Stock = 10, Category = "C1" };
        var result = _createValidator.Validate(model);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Price");
    }
}