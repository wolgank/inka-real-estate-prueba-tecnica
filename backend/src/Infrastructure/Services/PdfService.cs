using Application.DTOs.Products;
using Application.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Infrastructure.Services;

public class PdfService : IPdfService
{
    public byte[] GenerateLowStockPdf(IEnumerable<ProductDto> products)
    {
        // QuestPDF requiere configurar la licencia (Community es gratis para este caso)
        QuestPDF.Settings.License = LicenseType.Community;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Header().Text("REPORTE DE STOCK BAJO").FontSize(20).SemiBold().FontColor(Colors.Red.Medium);

                page.Content().PaddingVertical(10).Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.ConstantColumn(30);  // ID
                        columns.RelativeColumn(3);   // Nombre
                        columns.RelativeColumn(2);   // Categoría
                        columns.ConstantColumn(50);  // Cantidad
                    });

                    table.Header(header =>
                    {
                        header.Cell().Element(CellStyle).Text("ID");
                        header.Cell().Element(CellStyle).Text("Producto");
                        header.Cell().Element(CellStyle).Text("Categoría");
                        header.Cell().Element(CellStyle).Text("Stock");

                        static IContainer CellStyle(IContainer container) => 
                            container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    });

                    foreach (var item in products)
                    {
                        table.Cell().Element(ItemStyle).Text(item.Id.ToString());
                        table.Cell().Element(ItemStyle).Text(item.Name);
                        table.Cell().Element(ItemStyle).Text(item.Category);
                        table.Cell().Element(ItemStyle).Text(item.Stock.ToString()).FontColor(Colors.Red.Medium);

                        static IContainer ItemStyle(IContainer container) => 
                            container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5);
                    }
                });
            });
        }).GeneratePdf();
    }
}