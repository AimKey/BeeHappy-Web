using BusinessObjects.Base;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class EmoteSet : MongoEntity
    {
        public string Name { get; set; }
        public ObjectId OwnerId { get; set; }
        public List<string> Tags { get; set; }
        public List<ObjectId> Emotes { get; set; }
        public int Capacity { get; set; }
        public bool IsActive { get; set; }
    }
}
