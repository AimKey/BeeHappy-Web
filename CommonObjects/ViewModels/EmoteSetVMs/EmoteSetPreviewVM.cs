using BusinessObjects;
using CommonObjects.ViewModels.EmoteSetVMs;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.ViewModels.EmoteSetVMs
{
    public class EmoteSetPreviewVM
    {
        public ObjectId EmoteSetId { get; set; }
        // First 10 emote preview
        public List<EmoteSetEmotePreviewVM> Emotes { get; set; }
        public int EmoteCapacity { get; set; }
        public int EmoteCount { get; set; }
        public string EmoteSetName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
