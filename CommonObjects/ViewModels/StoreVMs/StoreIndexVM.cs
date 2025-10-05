using BusinessObjects;
using BusinessObjects.NestedObjects;

namespace CommonObjects.ViewModels.StoreVMs;

public class StoreIndexVM
{
    public List<SubscriptionPlanVM> SubscriptionPlans { get; set; } = new();
    public List<BenefitVM> SubscriptionBenefits { get; set; } = new();
    public BadgeProgressVM BadgeProgress { get; set; } = new();
    public List<ThemeVM> MonthlyThemes { get; set; } = new();
    public CurrentUserPlanVm? CurrentUserPlan { get; set; } = new();
    public List<UserPaint> CurrentUserPaints { get; set; }
}