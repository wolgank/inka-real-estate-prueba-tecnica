using Application.DTOs.Products;

namespace Application.Interfaces;

public interface IPdfService
{
    byte[] GenerateLowStockPdf(IEnumerable<ProductDto> products);
}