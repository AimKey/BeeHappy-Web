using BusinessObjects.Base;

namespace BusinessObjects;

public class PremiumPlan : MongoEntity
{
    public string Name { get; set; } = string.Empty;
    public int DurationInDays { get; set; } // null for lifetime plans
    public int Price { get; set; }
    public bool IsPopular { get; set; }
    public int? SaveAmount { get; set; }
}