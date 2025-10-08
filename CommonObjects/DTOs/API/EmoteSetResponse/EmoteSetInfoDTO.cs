using CommonObjects.DTOs.API.EmoteResponse;
using MongoDB.Bson;

namespace CommonObjects.DTOs.API.EmoteSetResponse;

public class EmoteSetInfoDTO
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string OwnerId { get; set; }
    public string OwnerName { get; set; }
    public List<EmoteInfoDTO> Emotes { get; set; }
    public int Capacity { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}