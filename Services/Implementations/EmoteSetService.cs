using BusinessObjects;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class EmoteSetService(IEmoteSetRepository emoteSetRepository) : IEmoteSetService
    {
        public async Task<List<EmoteSet>> GetAllEmoteSetsAsync(CancellationToken ct = default)
        {
            return await emoteSetRepository.GetAllAsync(ct);
        }

        public async Task<List<EmoteSet>> GetEmoteSetsAsync(System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter, CancellationToken ct = default)
        {
            return await emoteSetRepository.GetAsync(filter, ct);
        }

        public async Task<EmoteSet?> GetEmoteSetByIdAsync(string id, CancellationToken ct = default)
        {
            return await emoteSetRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default)
        {
            await emoteSetRepository.InsertAsync(emoteSet, ct);
        }

        public async Task<bool> ReplaceEmoteSetAsync(EmoteSet emoteSet, bool upsert = false, CancellationToken ct = default)
        {
            return await emoteSetRepository.ReplaceAsync(emoteSet, upsert, ct);
        }

        public async Task<bool> DeleteEmoteSetByIdAsync(string id, CancellationToken ct = default)
        {
            return await emoteSetRepository.DeleteByIdAsync(id, ct);
        }

        public async Task<bool> DeleteEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default)
        {
            return await emoteSetRepository.DeleteAsync(emoteSet, ct);
        }

        public async Task<long> CountEmoteSetsAsync(System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter = null, CancellationToken ct = default)
        {
            return await emoteSetRepository.CountAsync(filter, ct);
        }
    }
}