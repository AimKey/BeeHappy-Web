using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.DTOs.EmoteSetDTOs
{
    public class CreateEmoteSetDto
    {
        public string Name { get; set; } = "Unnamed Emote Set";
        public ObjectId OwnerId { get; set; }
        public string TagsString { get; set; } = "";
        public int Capacity { get; set; }
        public bool IsActive { get; set; } = false;
    }
}