using AutoMapper;
using BusinessObjects;
using CommonObjects.AppConstants;
using CommonObjects.ViewModels.PaymentVMs;
using MongoDB.Bson;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations;

public class StoreService(
    IPaymentService paymentService,
    IPremiumPlanRepository premiumPlanRepository,
    IMapper mapper)
    : IStoreService
{
    public async Task<StoreIndexVM> GetStoreIndexVmAsync(User? currentUser)
    {
        await AddStaticPremiumPlans();
        var existingPlans = await premiumPlanRepository.GetAllAsync();
        var plansVm = mapper.Map<List<SubscriptionPlanVM>>(existingPlans);
        var userCurrentPlan = await GetUserCurrentPlanVm(currentUser);
        // Ngoài premium plan ra thì các giá trị còn lại tạm thời hardcode
        var viewModel = new StoreIndexVM
        {
            SubscriptionPlans = plansVm,
            SubscriptionBenefits = new List<BenefitVM>
            {
                new BenefitVM { Name = "Ảnh đại diện động", Icon = "user" },
                new BenefitVM { Name = "Emote cá nhân", Icon = "smile" },
                new BenefitVM { Name = "Màu tên đặc biệt", Icon = "paint" },
                new BenefitVM { Name = "Huy hiệu thành viên", Icon = "badge" }
            },
            BadgeProgress = new BadgeProgressVM
            {
                CurrentLevel = "Miễn phí",
                NextLevel = "BeeHappy Premium - 1 Tháng",
                ProgressPercentage = 0
            },
            MonthlyThemes = new List<ThemeVM>
            {
                new ThemeVM { Name = "Cáo tò mò", Color = "#FF6B35" },
                new ThemeVM { Name = "Tất lập trình", Color = "#4ECDC4" },
                new ThemeVM { Name = "Phô mai mốc", Color = "#45B7D1" },
                new ThemeVM { Name = "Bạch tuộc", Color = "#96CEB4" },
                new ThemeVM { Name = "Tinh vân", Color = "#FFEAA7" }
            },
            CurrentUserPlan = userCurrentPlan,
        };
        return viewModel;
    }

    private async Task AddStaticPremiumPlans()
    {
        var existingPlans = await premiumPlanRepository.GetAllAsync();
        if (!existingPlans.Any())
        {
            var plans = new List<PremiumPlan>
            {
                new PremiumPlan
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = "Gói tháng",
                    Price = 10000,
                    DurationInDays = 31,
                    IsPopular = true,
                    SaveAmount = null,
                },
                new PremiumPlan
                {
                    Id = ObjectId.GenerateNewId(),
                    Name = "Gói năm",
                    Price = 100000,
                    DurationInDays = 365,
                    IsPopular = false,
                    SaveAmount = 20000,
                }
            };

            foreach (var plan in plans)
            {
                await premiumPlanRepository.InsertAsync(plan);
            }
        }
    }

    private async Task<CurrentUserPlanVm?> GetUserCurrentPlanVm(User? currentUser)
    {
        CurrentUserPlanVm? userCurrentPlan = null;
        if (currentUser != null)
        {
            // Get user newest purchased
            var userPurchases = await paymentService.GetUserPurchaseHistories(currentUser.Id);
            var newestPurchase = userPurchases
                .OrderByDescending(p => p.PurchasedDate)
                .FirstOrDefault(p => p.Status == PaymentConstants.PAYMENT_SUCCESS);
            
            if (newestPurchase != null)
            {
                // Get the plan
                var plan = await premiumPlanRepository.GetByIdAsync(newestPurchase.PlanId);
                userCurrentPlan = new CurrentUserPlanVm
                {
                    PlanId = plan.Id,
                    PlanName = plan.Name,
                    ExpiryDate = newestPurchase.ExpireDate,
                };
            }
        }

        return userCurrentPlan;
    }
}