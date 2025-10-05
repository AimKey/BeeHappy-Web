using BusinessObjects.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Badge : MongoEntity
    {
        public string Name { get; set; } = "Not specified";
        public string Image { get; set; } = "Not specified";
        public string StyleString { get; set; } = "Not specified"; // CSS style string for customizing badge appearance
    }
}
