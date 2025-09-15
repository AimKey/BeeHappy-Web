using BusinessObjects;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.BusinessObjects
{
    public class Dummy : MongoEntity
    {
        public string DummyName { get; set; }
        public int DummyQuantity { get; set; }
    }
}
