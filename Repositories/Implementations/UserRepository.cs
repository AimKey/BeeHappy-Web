using BusinessObjects;
using BusinessObjects.Base;
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
    public class UserRepository(MongoDBContext context) : GenericRepository<User>(context), IUserRepository
    {
    }
}
