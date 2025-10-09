using System.Globalization;
using System.Text;
using BusinessObjects;
using BusinessObjects.NestedObjects;
using CommonObjects.AppConstants;
using CommonObjects.DTOs.UserDTOs;
using CommonObjects.ViewModels.StoreVMs;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Repositories.Implementations;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserService(
        IUserRepository userRepository,
        IBadgeRepository badgeRepository,
        IPaintRepository paintRepository,
        IPaymentService paymentService,
        IPremiumPlanRepository premiumPlanRepository) : IUserService
    {
        public async Task<List<User>> GetAllUsersAsync(CancellationToken ct = default)
        {
            return await userRepository.GetAllAsync(ct);
        }

        public async Task<List<User>> GetUsersAsync(System.Linq.Expressions.Expression<Func<User, bool>>? filter,
            CancellationToken ct = default)
        {
            return await userRepository.GetAsync(filter, ct);
        }

        public async Task<User?> GetUserByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await userRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertUserAsync(User user, CancellationToken ct = default)
        {
            // Normalize username
            user.NormalizedName = NormalizeUserName(user.Username);
            await userRepository.InsertAsync(user, ct);
        }

        public async Task<bool> ReplaceUserAsync(User user, bool upsert = false, CancellationToken ct = default)
        {
            // Normalize username
            user.NormalizedName = NormalizeUserName(user.Username);
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

        public async Task<long> CountUsersAsync(System.Linq.Expressions.Expression<Func<User, bool>>? filter = null,
            CancellationToken ct = default)
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
                user.Profile = new();
            }

            user.Profile.AvatarUrl = $"/uploads/userAvatars/{uniqueFileName}";
            await ReplaceUserAsync(user, false, ct);
        }

        // public Task<UserInfoDTO> GetAllUserInfo(ObjectId userId)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task<UserInfoDTO> GetUserInfo(ObjectId userId)
        {
            // Get user badge
            var user = await GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("Không tìm thấy người dùng");
            }

            var userBadgesDTO = await GetUserBadgeInfoDtos(user);

            // Get user paints
            var userPaintsDTO = await GetUserPaintInfoDtos(user);

            // Return all info
            UserInfoDTO userInfo = new UserInfoDTO()
            {
                UserId = user.Id.ToString(),
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                Roles = user.Roles ?? new List<string>(),
                IsPremium = user.IsPremium,
                Profile = user.Profile ?? new Profile(),
                Badges = userBadgesDTO,
                Paints = userPaintsDTO
            };
            return userInfo;
        }

        public async Task<List<UserBadgeInfoDTO>> GetUserBadgeInfoDtos(User user)
        {
            var userBadgesDTO = new List<UserBadgeInfoDTO>();
            // Get badge details
            // TODO: Revamp the badge system later
            if (user.Badges != null && user.Badges.Any())
            {
                var userBadgesId = user.Badges;
                var userBadges = await badgeRepository.GetAsync(b => userBadgesId.Contains(b.Id));
                userBadgesDTO = userBadges.Select(b => new UserBadgeInfoDTO
                {
                    Id = b.Id,
                    Name = b.Name,
                    Image = b.Image,
                    StyleString = b.StyleString,
                    IsActive = user.Badges.Any(ub => ub == b.Id)
                }).ToList();
            }

            return userBadgesDTO;
        }

        public async Task<List<UserPaintInfoDTO>> GetUserPaintInfoDtos(User user)
        {
            var userPaintsDTO = new List<UserPaintInfoDTO>();
            if (user.Paints != null && user.Paints.Any())
            {
                var userPaintsId = user.Paints.Select(up => up.PaintId).ToList();
                var userPaints = await paintRepository.GetAsync(p => userPaintsId.Contains(p.Id));
                userPaintsDTO = user.Paints.Join(userPaints,
                    up => up.PaintId,
                    p => p.Id,
                    (up, p) => new UserPaintInfoDTO
                    {
                        Id = p.Id.ToString(),
                        Name = p.Name,
                        Color = p.Color,
                        IsActive = up.IsActivated && user.IsPremium
                    }).ToList();
            }

            return userPaintsDTO;
        }

        public async Task<User?> GetUserByNameAsync(string userName)
        {
            var users = await userRepository.GetAsync(u => u.Username.ToLowerInvariant()
                .Equals(userName.ToLowerInvariant()));
            var user = users.FirstOrDefault();
            return user;
        }

        public string NormalizeUserName(string userName)
        {
            return userName.Normalize(NormalizationForm.FormC)
                .ToLower(new CultureInfo("vi-VN"));
        }

        public async Task<CurrentUserPlanVm?> GetCurrentPlanAsync(ObjectId userId)
        {
            var userPurchases = await paymentService.GetUserPurchaseHistories(userId);
            var newestPurchase = userPurchases
                .OrderByDescending(p => p.PurchasedDate)
                .FirstOrDefault(p => p.Status == PaymentConstants.PAYMENT_SUCCESS && p.ExpireDate > DateTime.Now);

            if (newestPurchase == null) return null;

            var plan = await premiumPlanRepository.GetByIdAsync(newestPurchase.PlanId);
            if (plan == null) return null;

            return new CurrentUserPlanVm
            {
                PlanId = plan.Id,
                PlanName = plan.Name,
                ExpiryDate = newestPurchase.ExpireDate
            };
        }
    }
}