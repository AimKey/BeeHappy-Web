namespace CommonObjects.ViewModels.PaymentVMs;

public class PaymentResultVM
{
    public bool IsSuccess { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? OrderCode { get; set; }
    public string? TransactionId { get; set; }
}

