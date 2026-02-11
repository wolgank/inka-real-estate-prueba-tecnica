namespace Application.Interfaces;

public interface IEmailService
{
    Task SendLowStockAlertAsync(string productName, int currentStock, List<string> adminEmails);
}