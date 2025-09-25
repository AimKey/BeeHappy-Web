using MongoDB.Bson;

namespace CommonObjects.ViewModels.PaymentVMs;

public class PaymentPlanVM
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Currency { get; set; } = "VND";
    public List<string> Features { get; set; } = new();
    public bool IsPopular { get; set; }
    public string ButtonText { get; set; } = "Chọn gói";
    public string PriceText => Price == 0 ? "Miễn phí" : $"{Price:N0} {Currency}";
}