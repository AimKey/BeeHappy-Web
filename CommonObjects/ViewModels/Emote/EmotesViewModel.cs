using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.ViewModels.Emote;

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
