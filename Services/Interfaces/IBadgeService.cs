using BusinessObjects;

namespace Services.Interfaces
{
    public interface IBadgeService
    {
        Task<List<Badge>> GetAllBadgesAsync(CancellationToken ct = default);
        Task<List<Badge>> GetBadgesAsync(System.Linq.Expressions.Expression<Func<Badge, bool>>? filter, CancellationToken ct = default);
        Task<Badge?> GetBadgeByIdAsync(string id, CancellationToken ct = default);
        Task InsertBadgeAsync(Badge badge, CancellationToken ct = default);
        Task<bool> ReplaceBadgeAsync(Badge badge, bool upsert = false, CancellationToken ct = default);
        Task<bool> DeleteBadgeByIdAsync(string id, CancellationToken ct = default);
        Task<bool> DeleteBadgeAsync(Badge badge, CancellationToken ct = default);
        Task<long> CountBadgesAsync(System.Linq.Expressions.Expression<Func<Badge, bool>>? filter = null, CancellationToken ct = default);
    }
}