using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class PurchaseHistoryRepository : GenericRepository<PurchaseHistory>, IPurchaseHistoryRepository
{
    private readonly MongoDBContext context;
    public PurchaseHistoryRepository(MongoDBContext context) : base(context)
    {
        this.context = context;
    }
}

