using BusinessObjects;
using CommonObjects.ViewModels.EmoteSetVMs;

namespace BeeHappy.ViewModels
{
    public class EmoteViewModel
    {
        public string? Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; } = string.Empty;
        public string? OwnerName { get; set; }
        public string? OwnerAvatar { get; set; }
        public List<ChannelViewModel>? Channels { get; set; } = new();
        public sealed class ChannelViewModel
        {
            public string? Name { get; set; }
            public string? Avatar { get; set; }
        }
        public List<EmoteFileViewModel> Files { get; set; } = new();
        public List<string> Tags { get; set; } = new();
        public List<string> Visibility { get; set; } = new();
        public List<string> Status { get; set; } = new();
        public bool IsOverlaying { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int TotalChannels { get; set; }
        public List<EmoteSet>? UserEmoteSets { get; set; }
    }
}
