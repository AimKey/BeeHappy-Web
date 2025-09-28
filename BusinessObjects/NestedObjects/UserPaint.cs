using BusinessObjects.Base;
using MongoDB.Bson;

namespace BusinessObjects.NestedObjects;

public class UserPaint : MongoEntity
{
    public ObjectId PaintId { get; set; }
    public bool IsActivated { get; set; }
}