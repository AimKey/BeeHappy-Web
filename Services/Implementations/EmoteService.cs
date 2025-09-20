using BusinessObjects;
using MongoDB.Bson;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class EmoteService(IEmoteRepository emoteRepository) : IEmoteService
    {
        public async Task<List<Emote>> GetAllEmotesAsync(CancellationToken ct = default)
           => await emoteRepository.GetAllAsync(ct);

        public async Task<Emote?> GetEmoteByIdAsync(ObjectId id, CancellationToken ct = default)
            => await emoteRepository.GetByIdAsync(id, ct);

        public async Task InsertEmoteAsync(Emote emote, CancellationToken ct = default)
        {
            emote.Id = ObjectId.GenerateNewId();
            emote.CreatedAt = DateTime.UtcNow;
            emote.UpdatedAt = DateTime.UtcNow;

            // default values
            emote.Visibility = new List<string> { "public" };
            emote.Status = new List<string> { "active" };

            await emoteRepository.InsertAsync(emote, ct);
        }

        public async Task<bool> ReplaceEmoteAsync(Emote emote, bool upsert = false, CancellationToken ct = default)
        {
            var existing = await emoteRepository.GetByIdAsync(emote.Id, ct);
            if (existing == null) return false;

            
            existing.Name = emote.Name;
            existing.Tags = emote.Tags ?? new List<string>();
            existing.IsOverlaying = emote.IsOverlaying;
            existing.Visibility = emote.Visibility ?? new List<string> { "public" };
            existing.Status = emote.Status ?? new List<string> { "active" };
            existing.UpdatedAt = DateTime.UtcNow;

            return await emoteRepository.ReplaceAsync(existing, upsert, ct);
        }

        public Task<bool> DeleteEmoteByIdAsync(ObjectId id, CancellationToken ct = default)
            => emoteRepository.DeleteByIdAsync(id, ct);

        public Task<bool> DeleteEmoteAsync(Emote emote, CancellationToken ct = default)
            => emoteRepository.DeleteAsync(emote, ct);

        public Task<long> CountEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter = null, CancellationToken ct = default)
            => emoteRepository.CountAsync(filter, ct);

        public Task<List<Emote>> GetEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter, CancellationToken ct = default)
            => emoteRepository.GetAsync(filter, ct);
    }
}