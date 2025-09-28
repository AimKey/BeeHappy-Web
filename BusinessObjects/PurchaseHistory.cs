using BusinessObjects.Base;
using MongoDB.Bson;

namespace BusinessObjects;

public class PurchaseHistory : MongoEntity
{
    public ObjectId OwnerId { get; set; }
    public ObjectId PlanId { get; set; } // The ID of the purchased plan
    public int OrderCode { get; set; }// The id of the transaction (from payOS)
    public DateTime PurchasedDate { get; set; } // The time of purchase
    public DateTime ExpireDate { get; set; } // The time the plan expires
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}