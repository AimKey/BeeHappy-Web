using MongoDB.Bson;

namespace CommonObjects.ViewModels.PaymentVMs;

public class SubscriptionPlanVM
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DurationInDays { get; set; } // null for lifetime plans
    public decimal Price { get; set; }
    public bool IsPopular { get; set; }
    public int? SaveAmount { get; set; }
}
