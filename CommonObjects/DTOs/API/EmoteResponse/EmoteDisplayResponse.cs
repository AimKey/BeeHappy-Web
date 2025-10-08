using BusinessObjects.NestedObjects;

namespace CommonObjects.DTOs.API.EmoteResponse;

public class EmoteDisplayResponse
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string CreatedBy { get; set; }
    public EmoteFile File { get; set; }
    public string BelongToEmoteSet { get; set; }
    public bool IsOverlaying { get; set; }
}