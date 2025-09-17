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
        public string Name { get; set; }
        public string Image { get; set; }
    }
}
