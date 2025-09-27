using MongoDB.Bson;

namespace CommonObjects.ViewModels.StoreVMs;

public class CurrentUserPlanVm
{
    public ObjectId PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
}