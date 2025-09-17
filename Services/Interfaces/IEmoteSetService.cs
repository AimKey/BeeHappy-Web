using BusinessObjects;

namespace Services.Interfaces
{
    public interface IEmoteSetService
    {
        Task<List<EmoteSet>> GetAllEmoteSetsAsync(CancellationToken ct = default);
        Task<List<EmoteSet>> GetEmoteSetsAsync(System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter, CancellationToken ct = default);
        Task<EmoteSet?> GetEmoteSetByIdAsync(string id, CancellationToken ct = default);
        Task InsertEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default);
        Task<bool> ReplaceEmoteSetAsync(EmoteSet emoteSet, bool upsert = false, CancellationToken ct = default);
        Task<bool> DeleteEmoteSetByIdAsync(string id, CancellationToken ct = default);
        Task<bool> DeleteEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default);
        Task<long> CountEmoteSetsAsync(System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter = null, CancellationToken ct = default);
    }
}