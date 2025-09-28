using BusinessObjects;
using CommonObjects.ViewModels.UserCosmeticsVMs;

namespace Services.Interfaces;

public interface ICosmeticsService
{
    Task<UserCosmeticsViewModel> GetUserCosmeticsViewModels(User currentUser);
}