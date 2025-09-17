using BusinessObjects;

namespace Services.Interfaces
{
    public interface IEmoteService
    {
        Task<List<Emote>> GetAllEmotesAsync(CancellationToken ct = default);
        Task<List<Emote>> GetEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter, CancellationToken ct = default);
        Task<Emote?> GetEmoteByIdAsync(string id, CancellationToken ct = default);
        Task InsertEmoteAsync(Emote emote, CancellationToken ct = default);
        Task<bool> ReplaceEmoteAsync(Emote emote, bool upsert = false, CancellationToken ct = default);
        Task<bool> DeleteEmoteByIdAsync(string id, CancellationToken ct = default);
        Task<bool> DeleteEmoteAsync(Emote emote, CancellationToken ct = default);
        Task<long> CountEmotesAsync(System.Linq.Expressions.Expression<Func<Emote, bool>>? filter = null, CancellationToken ct = default);
    }
}