using AutoMapper;
using BusinessObjects;
using BusinessObjects.NestedObjects;
using CommonObjects.AppConstants;
using CommonObjects.ViewModels.StoreVMs;
using MongoDB.Bson;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations;

public class StoreService(
    IPaymentService paymentService,
    IPremiumPlanRepository premiumPlanRepository,
    IPaintService paintService,
    IUserService userService,
    IMapper mapper)
    : IStoreService
{
    public async Task<StoreIndexVM> GetStoreIndexVmAsync(User? currentUser)
    {
        await AddStaticPremiumPlans();
        await AddStaticPaints();
        var existingPlans = await premiumPlanRepository.GetAllAsync();
        var existingPaints = await paintService.GetAllPaintsAsync();
        var plansVm = mapper.Map<List<SubscriptionPlanVM>>(existingPlans);
        var paintsVm = mapper.Map<List<ThemeVM>>(existingPaints);
        paintsVm = paintsVm.Slice(0, 7).ToList();
        var userCurrentPlan = await GetUserCurrentPlanVm(currentUser);
        var userCurrentPaints = currentUser?.Paints;
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
                NextLevel = "Premium Supporter - 1 Năm",
                ProgressPercentage = 0
            },
            MonthlyThemes = paintsVm,
            CurrentUserPlan = userCurrentPlan,
            CurrentUserPaints = userCurrentPaints ?? new List<UserPaint>()
        };
        return viewModel;
    }

    private async Task AddStaticPaints()
    {
        var allPaints = await paintService.GetAllPaintsAsync();
        if (!allPaints.Any())
        {
            var paints = new List<Paint>
            {
                new Paint { Name = "Cáo tò mò", Color = "#FF6B35" },
                new Paint { Name = "Tất lập trình", Color = "#4ECDC4" },
                new Paint { Name = "Phô mai mốc", Color = "#45B7D1" },
                new Paint { Name = "Bạch tuộc", Color = "#96CEB4" },
                new Paint { Name = "Tinh vân", Color = "#FFEAA7" }
            };
            foreach (var p in paints)
            {
                await paintService.InsertPaintAsync(p);
            }
        }
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
            // Get user newest purchased and check if it's expired
            var userPurchases = await paymentService.GetUserPurchaseHistories(currentUser.Id);
            var isPremium = await paymentService.CheckUserHasActivePremium(currentUser);

            if (isPremium)
            {
                var newestPurchase = userPurchases
                    .OrderByDescending(p => p.PurchasedDate)
                    .FirstOrDefault(p => p.Status == PaymentConstants.PAYMENT_SUCCESS && p.ExpireDate > DateTime.Now);
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
        }

        return userCurrentPlan;
    }
}