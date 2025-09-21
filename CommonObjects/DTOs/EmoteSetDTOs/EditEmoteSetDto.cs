using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonObjects.DTOs.EmoteSetDTOs
{
    public class EditEmoteSetDto
    {
        public ObjectId OwnerId { get; set; }
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string TagsString { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}
