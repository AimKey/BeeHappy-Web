using BusinessObjects.NestedObjects;
using MongoDB.Bson;

namespace CommonObjects.DTOs.UserDTOs;

public class UserInfoDTO
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string GoogleId { get; set; }
    public List<string> Roles { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Profile Profile { get; set; }
    public List<Editor> Editors { get; set; }
    public List<UserBadgeInfoDTO> Badges { get; set; }
    public List<UserPaintInfoDTO> Paints { get; set; }
    public bool IsPremium { get; set; }
}