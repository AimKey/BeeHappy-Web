using BusinessObjects;
using BusinessObjects.NestedObjects;
using CommonObjects.AppConstants;
using MongoDB.Bson;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class PaintService(
        IPaintRepository paintRepository,
        IUserService userService,
        IPaymentService paymentService) : IPaintService
    {
        public async Task<List<Paint>> GetAllPaintsAsync(CancellationToken ct = default)
        {
            return await paintRepository.GetAllAsync(ct);
        }

        public async Task<List<Paint>> GetPaintsAsync(System.Linq.Expressions.Expression<Func<Paint, bool>>? filter,
            CancellationToken ct = default)
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

        public async Task<long> CountPaintsAsync(System.Linq.Expressions.Expression<Func<Paint, bool>>? filter = null,
            CancellationToken ct = default)
        {
            return await paintRepository.CountAsync(filter, ct);
        }

        public async Task ActivePaintForUserAsync(User currentUser, ObjectId paintId)
        {
            if (currentUser.Paints == null || !currentUser.Paints.Any(up => up.PaintId == paintId))
            {
                throw new Exception("Người dùng chưa sở hữu màu này.");
            }

            // Check if user is premium
            bool isPremium = await paymentService.CheckUserHasActivePremium(currentUser);
            if (!isPremium)
            {
                throw new Exception("Chỉ người dùng Premium mới có thể kích hoạt màu.");
            }

            // Deactivate all other paints, enable only the selected one
            foreach (var userPaint in currentUser.Paints)
            {
                userPaint.IsActivated = userPaint.PaintId == paintId;
            }

            // Update
            await userService.ReplaceUserAsync(currentUser);
        }

        public async Task AddPaintToUserAsync(User currentUser, ObjectId paintId)
        {
            if (currentUser.Paints == null)
            {
                currentUser.Paints = new List<UserPaint>();
            }

            // Check if user already has the paint
            if (currentUser.Paints.Any(up => up.PaintId == paintId))
            {
                throw new Exception("Bạn đã sở hữu màu này.");
            }

            // Check if user is a premium user
            bool isPremium = await paymentService.CheckUserHasActivePremium(currentUser);
            if (!isPremium)
            {
                throw new Exception("Chỉ người dùng Premium mới có thể thêm màu.");
            }

            // Add paint to user
            currentUser.Paints.Add(new UserPaint
            {
                PaintId = paintId,
                Id = ObjectId.GenerateNewId(),
                IsActivated = false,
            });
            await userService.ReplaceUserAsync(currentUser);
        }

        public async Task DeactivateAllPaintsForUserAsync(User user)
        {
            // Deactivate all paints
            if (user.Paints != null && user.Paints.Any())
            {
                foreach (var userPaint in user.Paints)
                {
                    userPaint.IsActivated = false;
                }
                // Update
                await userService.ReplaceUserAsync(user);
            }
        }
        
        public async Task<string> GetActivePaintColorForUserAsync(User user)
        {
            if (!user.IsPremium || (user.Paints != null && !user.Paints.Any()))
            {
                return UserConstants.DEFAUT_USER_NAME_COLOR;
            }

            var activePaint = user.Paints?.FirstOrDefault(up => up.IsActivated);
            if (activePaint == null)
            {
                return UserConstants.DEFAUT_USER_NAME_COLOR;
            }

            var paint = await GetPaintByIdAsync(activePaint.PaintId);
            if (paint == null)
            {
                return UserConstants.DEFAUT_USER_NAME_COLOR;
            }

            return paint.Color;
        }
    }
}