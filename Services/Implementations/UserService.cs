using BusinessObjects;
using CommonObjects.AppConstants;
using Microsoft.AspNetCore.Http;
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

        public async Task UpdateUserAvatar(IFormFile file, User user, CancellationToken ct = default)
        {
            // First check for file type, if it is animated image type, check if user is premium
            var permittedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                throw new Exception("Loại tệp không được phép. Vui lòng tải lên tệp hình ảnh hợp lệ.");
            }
            if (ext == ".gif" || ext == ".webp")
            {
                if (user.IsPremium == false)
                {
                    throw new Exception("Chỉ người dùng Premium mới có thể tải lên hình đại diện động.");
                }
            }
            // Save file to wwwroot/images/avatars with a user objectId as the file name
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "userAvatars");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = user.Id.ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            // Overwrite if exists
            await using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fileStream);
            }
            // In case user does not have a profile, create one
            if (user.Profile == null)
            {
                user.Profile = new ();
            }
            user.Profile.AvatarUrl = $"/uploads/userAvatars/{uniqueFileName}";
            await ReplaceUserAsync(user, false, ct);
        }
    }
}