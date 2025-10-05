using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;

namespace Repositories.Implementations;

public class PremiumPlanRepository : GenericRepository<PremiumPlan>, IPremiumPlanRepository
{
    private readonly MongoDBContext context;
    public PremiumPlanRepository(MongoDBContext context) : base(context)
    {
        this.context = context;
    }
}