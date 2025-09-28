using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.ViewModels.EmoteSetVMs
{
    public class EmoteSetDetailVM
    {
        public List<Emote> Emotes { get; set; }
        public EmoteSet EmoteSet { get; set; }
        public User Owner { get; set; }
    }
}
