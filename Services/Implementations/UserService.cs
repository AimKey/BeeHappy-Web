using BusinessObjects;
using MongoDB.Bson;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        public async Task<List<User>> GetAllUsersAsync(CancellationToken ct = default)
        {
            return await userRepository.GetAllAsync(ct);
        }

        public async Task<List<User>> GetUsersAsync(System.Linq.Expressions.Expression<Func<User, bool>>? filter, CancellationToken ct = default)
        {
            return await userRepository.GetAsync(filter, ct);
        }

        public async Task<User?> GetUserByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await userRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertUserAsync(User user, CancellationToken ct = default)
        {
            await userRepository.InsertAsync(user, ct);
        }

        public async Task<bool> ReplaceUserAsync(User user, bool upsert = false, CancellationToken ct = default)
        {
            return await userRepository.ReplaceAsync(user, upsert, ct);
        }

        public async Task<bool> DeleteUserByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await userRepository.DeleteByIdAsync(id, ct);
        }

        public async Task<bool> DeleteUserAsync(User user, CancellationToken ct = default)
        {
            return await userRepository.DeleteAsync(user, ct);
        }

        public async Task<long> CountUsersAsync(System.Linq.Expressions.Expression<Func<User, bool>>? filter = null, CancellationToken ct = default)
        {
            return await userRepository.CountAsync(filter, ct);
        }
    }
}