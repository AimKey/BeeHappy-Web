using MongoDB.Bson;

namespace BusinessObjects.NestedObjects
{
    public class AuditMetadata
    {
        public string LogType { get; set; }
        public ObjectId EmoteId { get; set; }
        public ObjectId SetId { get; set; }
    }
}