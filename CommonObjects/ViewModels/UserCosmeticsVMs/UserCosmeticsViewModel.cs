using BusinessObjects;

namespace CommonObjects.ViewModels.UserCosmeticsVMs;

public class UserCosmeticsViewModel
{
    public List<UserPaintVM> OwnedPaints { get; set; }
    public List<UserBadgeVM> OwnedBadges { get; set; }
}