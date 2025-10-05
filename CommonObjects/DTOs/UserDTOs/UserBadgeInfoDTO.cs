using MongoDB.Bson;

namespace CommonObjects.DTOs.UserDTOs;

public class UserBadgeInfoDTO
{
    public ObjectId Id { get; set; }
    public string Name { get; set; } = "Not specified";
    public string Image { get; set; } = "Not specified";
    public string StyleString { get; set; } = "Not specified"; // CSS style string for customizing badge appearance
    public bool IsActive { get; set; }
}