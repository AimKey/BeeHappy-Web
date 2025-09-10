using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class TestObject
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}
