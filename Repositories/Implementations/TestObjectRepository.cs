using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementations
{
    public class TestObjectRepository(MongoDBContext context) : GenericRepository<TestObject>(context), ITestObjectRepository
    {
    }
}
