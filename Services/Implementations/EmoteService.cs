using BusinessObjects;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class EmoteService(IEmoteRepository emoteRepository) : IEmoteService
    {
        public async Task<List<Emote>> GetAllEmotesAsync(CancellationToken ct = default)
        {
            return await emoteRepository.GetAllAsync(ct);
        }

        public async Task<List<Emote>> GetEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter, CancellationToken ct = default)
        {
            return await emoteRepository.GetAsync(filter, ct);
        }

        public async Task<Emote?> GetEmoteByIdAsync(string id, CancellationToken ct = default)
        {
            return await emoteRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertEmoteAsync(Emote emote, CancellationToken ct = default)
        {
            await emoteRepository.InsertAsync(emote, ct);
        }

        public async Task<bool> ReplaceEmoteAsync(Emote emote, bool upsert = false, CancellationToken ct = default)
        {
            return await emoteRepository.ReplaceAsync(emote, upsert, ct);
        }

        public async Task<bool> DeleteEmoteByIdAsync(string id, CancellationToken ct = default)
        {
            return await emoteRepository.DeleteByIdAsync(id, ct);
        }

        public async Task<bool> DeleteEmoteAsync(Emote emote, CancellationToken ct = default)
        {
            return await emoteRepository.DeleteAsync(emote, ct);
        }

        public async Task<long> CountEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter = null, CancellationToken ct = default)
        {
            return await emoteRepository.CountAsync(filter, ct);
        }
    }
}