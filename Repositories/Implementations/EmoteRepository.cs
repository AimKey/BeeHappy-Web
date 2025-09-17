using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class EmoteRepository(MongoDBContext context) : GenericRepository<Emote>(context), IEmoteRepository
    {
    }
}