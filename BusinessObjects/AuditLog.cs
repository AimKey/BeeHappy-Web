using BusinessObjects.Base;
using BusinessObjects.NestedObjects;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class AuditLog : MongoEntity
    {
        public ObjectId ActorId { get; set; }
        public string Action { get; set; }
        public ObjectId TargetId { get; set; }
        public AuditMetadata Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
