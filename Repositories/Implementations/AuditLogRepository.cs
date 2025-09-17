using BusinessObjects;
using DataAccessObjects;
using Repositories.Generics;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class AuditLogRepository(MongoDBContext context) : GenericRepository<AuditLog>(context), IAuditLogRepository
    {
    }
}