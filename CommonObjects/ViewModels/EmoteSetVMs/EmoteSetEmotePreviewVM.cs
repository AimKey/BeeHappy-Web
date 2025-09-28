using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.ViewModels.EmoteSetVMs
{
    public class EmoteSetEmotePreviewVM
    {
        public ObjectId Id { get; set; }
        public string Url { get; set; }
    }
}
