using BusinessObjects;
using MongoDB.Bson;

namespace Services.Interfaces
{
    public interface IPaintService
    {
        Task<List<Paint>> GetAllPaintsAsync(CancellationToken ct = default);
        Task<List<Paint>> GetPaintsAsync(System.Linq.Expressions.Expression<Func<Paint, bool>>? filter, CancellationToken ct = default);
        Task<Paint?> GetPaintByIdAsync(ObjectId id, CancellationToken ct = default);
        Task InsertPaintAsync(Paint paint, CancellationToken ct = default);
        Task<bool> ReplacePaintAsync(Paint paint, bool upsert = false, CancellationToken ct = default);
        Task<bool> DeletePaintByIdAsync(ObjectId id, CancellationToken ct = default);
        Task<bool> DeletePaintAsync(Paint paint, CancellationToken ct = default);
        Task<long> CountPaintsAsync(System.Linq.Expressions.Expression<Func<Paint, bool>>? filter = null, CancellationToken ct = default);
    }
}