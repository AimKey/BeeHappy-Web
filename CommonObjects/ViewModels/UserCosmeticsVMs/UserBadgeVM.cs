using MongoDB.Bson;

namespace CommonObjects.ViewModels.UserCosmeticsVMs;

public class UserBadgeVM
{
    public ObjectId BadgeId { get; set; }
    public ObjectId OwnerId { get; set; }
    public string Name { get; set; }
    public string IconUrl { get; set; }
    public bool IsActivated { get; set; }
}