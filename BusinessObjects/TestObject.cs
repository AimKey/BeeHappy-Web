using System.ComponentModel.DataAnnotations;
using BusinessObjects.Base;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BusinessObjects
{
    public class TestObject : MongoEntity
    {
        // If you don't know where the Id is, look at MongoEntity class
        public string UserName { get; set; }
    }
}
