using BusinessObjects.Base;

namespace BusinessObjects
{
    public class Paint : MongoEntity
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }
}
