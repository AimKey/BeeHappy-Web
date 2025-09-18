using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.DTOs.EmoteSet
{
    public class EmoteSetPreviewVM
    {
        // First 10 emote preview
        public List<Emote> Emotes { get; set; }
        public int EmoteCapacityPercent { get; set; }

    }
}
