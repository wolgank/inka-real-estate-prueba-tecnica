using Application.DTOs.Products;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IPdfService _pdfService;

    public ProductsController(IProductService productService, IPdfService pdfService)
    {
        _productService = productService;
        _pdfService = pdfService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        return Ok(await _productService.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null) return NotFound(new { Message = "El producto no existe." });
        
        return Ok(product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductDto dto)
    {
        var product = await _productService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateProductDto dto)
    {
        try 
        {
            await _productService.UpdateAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _productService.DeleteAsync(id);
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("low-stock")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetLowStock([FromQuery] int threshold = 5)
    {
        return Ok(await _productService.GetLowStockProductsAsync(threshold));
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("low-stock/report")]
    public async Task<IActionResult> GetLowStockReport()
    {
        // Obtenemos los productos con stock < 5
        var lowStockProducts = await _productService.GetLowStockProductsAsync(5);
        
        if (!lowStockProducts.Any())
        {
            return BadRequest(new { Message = "No hay productos con stock bajo para generar el reporte." });
        }

        var pdfBytes = _pdfService.GenerateLowStockPdf(lowStockProducts);
        
        // Retornamos el archivo para descarga directa
        return File(pdfBytes, "application/pdf", $"Reporte_Stock_{DateTime.Now:yyyyMMdd}.pdf");
    }
}