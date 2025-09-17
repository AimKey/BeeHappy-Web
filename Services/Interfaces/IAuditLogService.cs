using BusinessObjects;

namespace Services.Interfaces
{
    public interface IAuditLogService
    {
        Task<List<AuditLog>> GetAllAuditLogsAsync(CancellationToken ct = default);
        Task<List<AuditLog>> GetAuditLogsAsync(System.Linq.Expressions.Expression<Func<AuditLog, bool>>? filter, CancellationToken ct = default);
        Task<AuditLog?> GetAuditLogByIdAsync(string id, CancellationToken ct = default);
        Task InsertAuditLogAsync(AuditLog auditLog, CancellationToken ct = default);
        Task<bool> ReplaceAuditLogAsync(AuditLog auditLog, bool upsert = false, CancellationToken ct = default);
        Task<bool> DeleteAuditLogByIdAsync(string id, CancellationToken ct = default);
        Task<bool> DeleteAuditLogAsync(AuditLog auditLog, CancellationToken ct = default);
        Task<long> CountAuditLogsAsync(System.Linq.Expressions.Expression<Func<AuditLog, bool>>? filter = null, CancellationToken ct = default);
    }
}