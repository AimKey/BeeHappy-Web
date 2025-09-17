using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class EmoteSetRepository(MongoDBContext context) : GenericRepository<EmoteSet>(context), IEmoteSetRepository
    {
    }
}