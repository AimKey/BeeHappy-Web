using System.Security.Claims;
using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.ViewComponents;

public class UserSidebarViewComponent(IUserService userService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        // The case where user is not logged in should never happen because this view component is only used in authenticated pages
        var userId = UserClaimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userService.GetUserByIdAsync(ObjectId.Parse(userId));
        return View(user);
    }
}