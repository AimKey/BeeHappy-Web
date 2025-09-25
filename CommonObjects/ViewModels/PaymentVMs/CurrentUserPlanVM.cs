using MongoDB.Bson;

namespace CommonObjects.ViewModels.PaymentVMs;

public class CurrentUserPlanVm
{
    public ObjectId PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
}