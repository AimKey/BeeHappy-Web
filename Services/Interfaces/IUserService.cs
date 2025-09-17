using BusinessObjects;
using MongoDB.Bson;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync(CancellationToken ct = default);
        Task<List<User>> GetUsersAsync(System.Linq.Expressions.Expression<Func<User, bool>>? filter, CancellationToken ct = default);
        Task<User?> GetUserByIdAsync(ObjectId id, CancellationToken ct = default);
        Task InsertUserAsync(User user, CancellationToken ct = default);
        Task<bool> ReplaceUserAsync(User user, bool upsert = false, CancellationToken ct = default);
        Task<bool> DeleteUserByIdAsync(ObjectId id, CancellationToken ct = default);
        Task<bool> DeleteUserAsync(User user, CancellationToken ct = default);
        Task<long> CountUsersAsync(System.Linq.Expressions.Expression<Func<User, bool>>? filter = null, CancellationToken ct = default);
    }
}