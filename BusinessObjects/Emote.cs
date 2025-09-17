using BusinessObjects.Base;
using BusinessObjects.NestedObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Emote : MongoEntity
    {
        public string Name { get; set; }
        public ObjectId OwnerId { get; set; }
        public List<EmoteFile> Files { get; set; }
        public List<string> Tags { get; set; }
        public List<string> Visibility { get; set; }
        public List<string> Status { get; set; }
        public bool IsOverlaying { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
