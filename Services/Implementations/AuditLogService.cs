using BusinessObjects;
using Repositories.Interfaces;
using Services.Interfaces;
using System.Linq.Expressions;

namespace Services.Implementations
{
    public class AuditLogService(IAuditLogRepository auditLogRepository) : IAuditLogService
    {
        public async Task<List<AuditLog>> GetAllAuditLogsAsync(CancellationToken ct = default)
        {
            return await auditLogRepository.GetAllAsync(ct);
        }

        public async Task<List<AuditLog>> GetAuditLogsAsync(Expression<Func<AuditLog, bool>>? filter, CancellationToken ct = default)
        {
            return await auditLogRepository.GetAsync(filter, ct);
        }

        public async Task<AuditLog?> GetAuditLogByIdAsync(string id, CancellationToken ct = default)
        {
            return await auditLogRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertAuditLogAsync(AuditLog auditLog, CancellationToken ct = default)
        {
            await auditLogRepository.InsertAsync(auditLog, ct);
        }

        public async Task<bool> ReplaceAuditLogAsync(AuditLog auditLog, bool upsert = false, CancellationToken ct = default)
        {
            return await auditLogRepository.ReplaceAsync(auditLog, upsert, ct);
        }

        public async Task<bool> DeleteAuditLogByIdAsync(string id, CancellationToken ct = default)
        {
            return await auditLogRepository.DeleteByIdAsync(id, ct);
        }

        public async Task<bool> DeleteAuditLogAsync(AuditLog auditLog, CancellationToken ct = default)
        {
            return await auditLogRepository.DeleteAsync(auditLog, ct);
        }

        public async Task<long> CountAuditLogsAsync(Expression<Func<AuditLog, bool>>? filter = null, CancellationToken ct = default)
        {
            return await auditLogRepository.CountAsync(filter, ct);
        }
    }
}