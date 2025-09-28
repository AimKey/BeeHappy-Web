using MongoDB.Bson;

namespace CommonObjects.DTOs.UserDTOs;

public class UserPaintInfoDTO
{
    public ObjectId Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
    public bool IsActive { get; set; }
}