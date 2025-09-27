using BusinessObjects;
using CommonObjects.AppConstants;
using MongoDB.Bson;
using Net.payOS;
using Net.payOS.Types;
using Repositories.Interfaces;
using Services.HelperServices;
using Services.Interfaces;

namespace Services.Implementations;

public class PaymentService(
    IPurchaseHistoryRepository purchaseHistoryRepository,
    IPremiumPlanRepository premiumPlanRepository,
    IUserService userService,
    PayOS payOs) : IPaymentService
{
    public async Task<List<PurchaseHistory>> GetUserPurchaseHistories(ObjectId userId)
    {
        var histories = await purchaseHistoryRepository.GetAsync(p => p.OwnerId == userId);
        return histories.ToList();
    }

    public async Task<CreatePaymentResult> CreatePayOSPaymentObject(ObjectId userId, ObjectId planId, string returnUrl,
        string cancelUrl)
    {
        var plan = await premiumPlanRepository.GetByIdAsync(planId);
        var user = await userService.GetUserByIdAsync(userId);
        if (plan == null)
        {
            throw new Exception("Gói dịch vụ không tồn tại.");
        }

        if (user == null)
        {
            throw new Exception("Người dùng không tồn tại.");
        }

        var rand = new Random();
        var orderCode = int.Parse($"{DateTime.UtcNow:HHmmss}{rand.Next(100, 999)}");
        // var purchaseHistoryTransactionId = ObjectId.GenerateNewId();
        var amount = plan.Price;
        var description = $"{plan.Name}-{orderCode}";
        var payOsDesc = StringUtils.RemoveDiacritics(description);
        ItemData item = new ItemData(plan.Name, 1, plan.Price);
        List<ItemData> items = new List<ItemData> { item };

        PaymentData paymentData = new PaymentData(
            orderCode: orderCode,
            amount: amount,
            description: payOsDesc,
            items: items,
            returnUrl: returnUrl,
            cancelUrl: cancelUrl,
            buyerName: user.Username,
            buyerEmail: user.Email,
            buyerPhone: null,
            buyerAddress: null,
            signature: null,
            expiredAt: null
        );

        CreatePaymentResult createPayment = await payOs.createPaymentLink(paymentData);

        // Create a pending purchase history record
        // TODO: Use hangfire to handle cancelled and failed payments
        await LogPurchaseHistory(userId, planId, amount, orderCode, description);

        return createPayment;
    }

    private async Task LogPurchaseHistory(ObjectId userId, ObjectId planId, int amount, int orderCode,
        string description)
    {
        var plan = await premiumPlanRepository.GetByIdAsync(planId);
        var expireDate = DateTime.Now.AddDays(plan.DurationInDays);
        var history = new PurchaseHistory
        {
            Id = ObjectId.GenerateNewId(),
            OwnerId = userId,
            OrderCode = orderCode,
            PlanId = planId,
            Amount = amount,
            Status = PaymentConstants.PAYMENT_PENDING,
            Currency = "VND",
            PaymentMethod = PaymentConstants.ONLINE_PAYMENT_QR,
            Description = description,
            ExpireDate = expireDate,
            PurchasedDate = DateTime.Now,
        };
        await purchaseHistoryRepository.InsertAsync(history);
    }

    public async Task<bool> CompletePurchaseHistoryForUser(long orderCode)
    {
        var history = await purchaseHistoryRepository.GetAsync(h => h.OrderCode == orderCode);
        var userHistory = history.FirstOrDefault();
        if (userHistory == null) throw new Exception("Lịch sử mua hàng không tồn tại.");
        userHistory.Status = PaymentConstants.PAYMENT_SUCCESS;
        var status = await purchaseHistoryRepository.ReplaceAsync(userHistory);
        return status;
    }
    
    public async Task<bool> CancelPurchaseHistoryForUser(long orderCode)
    {
        var history = await purchaseHistoryRepository.GetAsync(h => h.OrderCode == orderCode);
        var userHistory = history.FirstOrDefault();
        if (userHistory == null) throw new Exception("Lịch sử mua hàng không tồn tại.");
        userHistory.Status = PaymentConstants.PAYMENT_CANCELED;
        var status = await purchaseHistoryRepository.ReplaceAsync(userHistory);
        return status;
    }
    
    public async Task<bool> CheckUserHasActivePremium(User currentUser)
    {
        // Check if user has an active premium plan
        var userPurchases = await GetUserPurchaseHistories(currentUser.Id);
        var activePurchase = userPurchases
            .OrderByDescending(p => p.PurchasedDate)
            .FirstOrDefault(p => p.Status == PaymentConstants.PAYMENT_SUCCESS && p.ExpireDate > DateTime.Now);

        if (activePurchase == null)
        {
            // Update user premium status
            currentUser.IsPremium = false;
            await userService.ReplaceUserAsync(currentUser);
            return false;
        }
        return true;
    }

}