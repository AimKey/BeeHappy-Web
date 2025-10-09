using BusinessObjects;
using MongoDB.Bson;
using Net.payOS.Types;

namespace Services.Interfaces;

public interface IPaymentService
{
    Task<List<PurchaseHistory>> GetUserPurchaseHistories(ObjectId userId);

    Task<CreatePaymentResult> CreatePayOSPaymentObject(ObjectId userId, ObjectId planId, string returnUrl,
        string cancelUrl);

    Task<bool> CompletePurchaseHistoryForUser(long orderCode);
    Task<bool> CancelPurchaseHistoryForUser(long orderCode);
    Task<bool> CheckUserHasActivePremium(User currentUser);
    Task<PurchaseHistory?> GetPaymentByOrderCodeAsync(long orderCode);
}