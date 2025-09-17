using BusinessObjects;
using MongoDB.Bson;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class PaintService(IPaintRepository paintRepository) : IPaintService
    {
        public async Task<List<Paint>> GetAllPaintsAsync(CancellationToken ct = default)
        {
            return await paintRepository.GetAllAsync(ct);
        }

        public async Task<List<Paint>> GetPaintsAsync(System.Linq.Expressions.Expression<Func<Paint, bool>>? filter, CancellationToken ct = default)
        {
            return await paintRepository.GetAsync(filter, ct);
        }

        public async Task<Paint?> GetPaintByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await paintRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertPaintAsync(Paint paint, CancellationToken ct = default)
        {
            await paintRepository.InsertAsync(paint, ct);
        }

        public async Task<bool> ReplacePaintAsync(Paint paint, bool upsert = false, CancellationToken ct = default)
        {
            return await paintRepository.ReplaceAsync(paint, upsert, ct);
        }

        public async Task<bool> DeletePaintByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await paintRepository.DeleteByIdAsync(id, ct);
        }

        public async Task<bool> DeletePaintAsync(Paint paint, CancellationToken ct = default)
        {
            return await paintRepository.DeleteAsync(paint, ct);
        }

        public async Task<long> CountPaintsAsync(System.Linq.Expressions.Expression<Func<Paint, bool>>? filter = null, CancellationToken ct = default)
        {
            return await paintRepository.CountAsync(filter, ct);
        }
    }
}