using BusinessObjects;
using CommonObjects.ViewModels.UserCosmeticsVMs;
using Services.Interfaces;

namespace Services.Implementations;

public class CosmeticsService(
    IUserService userService,
    IBadgeService badgeService,
    IPaintService paintService) : ICosmeticsService
{
    public async Task<UserCosmeticsViewModel> GetUserCosmeticsViewModels(User currentUser)
    {
        var userBadges = new List<UserBadgeVM>();
        var userPaints = new List<UserPaintVM>();
        foreach (var userPaint in currentUser.Paints)
        {
            var paintDetails = await paintService.GetPaintByIdAsync(userPaint.PaintId);
            if (paintDetails != null)
            {
                userPaints.Add(new UserPaintVM
                {
                    PaintId = userPaint.PaintId,
                    Name = paintDetails.Name,
                    ColorCode= paintDetails.Color,
                    IsActive = userPaint.IsActivated
                });
            }
        }
        return new UserCosmeticsViewModel
        {
            OwnedBadges = userBadges,
            OwnedPaints = userPaints
        };
    }
}