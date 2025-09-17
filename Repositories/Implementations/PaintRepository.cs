using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class PaintRepository(MongoDBContext context) : GenericRepository<Paint>(context), IPaintRepository
    {
    }
}