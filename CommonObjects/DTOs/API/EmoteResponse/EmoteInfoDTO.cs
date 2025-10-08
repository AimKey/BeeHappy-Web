using BusinessObjects;
using BusinessObjects.NestedObjects;
using MongoDB.Bson;

namespace CommonObjects.DTOs.API.EmoteResponse;

public class EmoteInfoDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public User ByUser { get; set; }
    public List<EmoteFile> Files { get; set; }
    public List<string> Visibility { get; set; }
    public List<string> Status { get; set; }
    public bool IsOverlaying { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}