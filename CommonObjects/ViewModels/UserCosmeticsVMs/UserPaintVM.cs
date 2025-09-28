using MongoDB.Bson;

namespace CommonObjects.ViewModels.UserCosmeticsVMs;

public class UserPaintVM
{
    public ObjectId PaintId { get; set; }
    public ObjectId OwnerId { get; set; }
    public string Name { get; set; }
    public string ColorCode { get; set; }
    public bool IsActive { get; set; }
}