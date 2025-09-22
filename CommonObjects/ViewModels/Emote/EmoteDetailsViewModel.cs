using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.ViewModels.Emote;

public class EmoteDetailsViewModel
{
    public string EmoteName { get; set; }
    public string OwnerName { get; set; }
    public string OwnerAvatar { get; set; }
    public List<EmoteFileViewModel> EmoteFiles { get; set; }
    public List<string> Tags { get; set; }
    public List<string> Visibility { get; set; }
    public List<string> Status { get; set; }
    public bool IsOverlaying { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int TotalChannels { get; set; }
    public List<ChannelViewModel> Channels { get; set; }

    public EmoteDetailsViewModel()
    {
        InitializeDefaultData();
    }

    private void InitializeDefaultData()
    {
        EmoteName = "GIGACHAD";
        OwnerName = "Beutelino";
        OwnerAvatar = "/images/avatar.jpg";
        TotalChannels = 278;

        EmoteFiles = CreateEmoteFiles();
        Tags = new List<string> { "meme", "chad", "reaction" };
        Visibility = new List<string> { "public" };
        Status = new List<string> { "approved" };
        IsOverlaying = false;
        CreatedAt = DateTime.Now.AddDays(-30);
        UpdatedAt = DateTime.Now.AddDays(-1);
        Channels = CreateChannels();
    }

    private List<EmoteFileViewModel> CreateEmoteFiles()
    {
        return new List<EmoteFileViewModel>
            {
                new EmoteFileViewModel { Format = "png", Url = "/gifs/Dance.gif", Size = 32 },
                new EmoteFileViewModel { Format = "png", Url = "/gifs/Dance.gif", Size = 64 },
                new EmoteFileViewModel { Format = "png", Url = "/gifs/Dance.gif", Size = 96 },
                new EmoteFileViewModel { Format = "png", Url = "/gifs/Dance.gif", Size = 128 }
            };
    }

    private List<ChannelViewModel> CreateChannels()
    {
        var channels = new List<ChannelViewModel>();
        var channelData = GetChannelData();

        foreach (var data in channelData)
        {
            channels.Add(new ChannelViewModel
            {
                Name = data.name,
                Avatar = "/images/avatar.jpg",
                Color = data.color,
                IsOnline = data.isOnline
            });
        }

        return channels;
    }

    private List<(string name, string color, bool isOnline)> GetChannelData()
    {
        return new List<(string, string, bool)>
            {
                ("colladoor", "#ff6b6b", true),
                ("CupOfKa...", "#4ecdc4", false),
                ("7TVapp", "#45b7d1", true),
                ("KEKW", "#f9ca24", true),
            };
    }
}

public class EmoteFileViewModel
{
    public string Format { get; set; }
    public string Url { get; set; }
    public int Size { get; set; }
}

public class ChannelViewModel
{
    public string Name { get; set; }
    public string Avatar { get; set; }
    public string Color { get; set; }
    public bool IsOnline { get; set; }
}
