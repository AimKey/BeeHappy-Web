namespace CommonObjects.ViewModels.EmoteVMs;

public class EmotesViewModel
{
    List<EmoteViewModel> Emotes { get; set; } = new List<EmoteViewModel>();
    public sealed class EmoteViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }    
        public string Owner { get; set; }
        public string Thumbnail { get; set; } 
        public bool IsAnimated { get; set; }
        public List<String> Tags { get; set; }
    }
}
