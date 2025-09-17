using BusinessObjects;
using MongoDB.Bson;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BadgeService(IBadgeRepository badgeRepository) : IBadgeService
    {
        public async Task<List<Badge>> GetAllBadgesAsync(CancellationToken ct = default)
        {
            return await badgeRepository.GetAllAsync(ct);
        }

        public async Task<List<Badge>> GetBadgesAsync(System.Linq.Expressions.Expression<Func<Badge, bool>>? filter, CancellationToken ct = default)
        {
            return await badgeRepository.GetAsync(filter, ct);
        }

        public async Task<Badge?> GetBadgeByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await badgeRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertBadgeAsync(Badge badge, CancellationToken ct = default)
        {
            await badgeRepository.InsertAsync(badge, ct);
        }

        public async Task<bool> ReplaceBadgeAsync(Badge badge, bool upsert = false, CancellationToken ct = default)
        {
            return await badgeRepository.ReplaceAsync(badge, upsert, ct);
        }

        public async Task<bool> DeleteBadgeByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await badgeRepository.DeleteByIdAsync(id, ct);
        }

        public async Task<bool> DeleteBadgeAsync(Badge badge, CancellationToken ct = default)
        {
            return await badgeRepository.DeleteAsync(badge, ct);
        }

        public async Task<long> CountBadgesAsync(System.Linq.Expressions.Expression<Func<Badge, bool>>? filter = null, CancellationToken ct = default)
        {
            return await badgeRepository.CountAsync(filter, ct);
        }
    }
}