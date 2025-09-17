using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using BusinessObjects.Base;
using DataAccessObjects;
using MongoDB.Driver;

namespace Repositories.Generics
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : MongoEntity
    {
        private readonly IMongoCollection<TEntity> _collection;
        
        public GenericRepository(MongoDBContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            _collection = context.Database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public GenericRepository()
        {
        }

        public Task<List<TEntity>> GetAllAsync(CancellationToken ct = default)
            => GetAsync(filter: null, ct);

        public Task<List<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? filter,
            CancellationToken ct = default)
        {
            var f = filter ?? (_ => true);
            return _collection.Find(f).ToListAsync(ct);
        }

        public Task<TEntity> GetByIdAsync(string id, CancellationToken ct = default)
            => _collection.Find(x => x.Id == id).FirstOrDefaultAsync(ct);

        public Task InsertAsync(TEntity entity, CancellationToken ct = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            return _collection.InsertOneAsync(entity, cancellationToken: ct);
        }

        public async Task<bool> ReplaceAsync(TEntity entity, bool upsert = false, CancellationToken ct = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var replaceOpts = new ReplaceOptions { IsUpsert = upsert };
            var result = await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity,
                replaceOpts, ct);
            return result.IsAcknowledged && (result.ModifiedCount > 0 || (upsert && result.UpsertedId != null));
        }

        public async Task<bool> DeleteByIdAsync(string id, CancellationToken ct = default)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id, ct);
            return result.DeletedCount > 0;
        }

        public async Task<bool> DeleteAsync(TEntity entity, CancellationToken ct = default)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            var result = await DeleteByIdAsync(entity.Id, ct);
            return result;
        }

        public Task<long> CountAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            CancellationToken ct = default)
        {
            var f = filter ?? (_ => true);
            return _collection.CountDocumentsAsync(f, cancellationToken: ct);
        }
    }
}