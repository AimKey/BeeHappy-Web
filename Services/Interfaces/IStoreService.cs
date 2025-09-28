using BusinessObjects;
using CommonObjects.ViewModels.StoreVMs;
using MongoDB.Bson;

namespace Services.Interfaces;

public interface IStoreService
{
    Task<StoreIndexVM> GetStoreIndexVmAsync(User? currentUser);
}