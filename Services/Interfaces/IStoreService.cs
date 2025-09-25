using BusinessObjects;
using CommonObjects.ViewModels.PaymentVMs;

namespace Services.Interfaces;

public interface IStoreService
{
    Task<StoreIndexVM> GetStoreIndexVmAsync(User? currentUser);
}