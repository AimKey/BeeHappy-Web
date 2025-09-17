using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BadgeRepository(MongoDBContext context) : GenericRepository<Badge>(context), IBadgeRepository
    {
    }
}