using System.Security.Claims;
using BusinessObjects;
using CommonObjects.AppConstants;
using CommonObjects.DTOs;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers.Users;

public class UserController(IUserService userService) : Controller
{
    // View the current user profile (Not other user's profile)
    public async Task<IActionResult> Index()
    {
        var u = await GetCurrentUserAsync();
        if (u == null)
        {
            return RedirectToAction("Index", "Home");
        }
        return View(u);
    }

    // API
    public async Task<IActionResult> UpdateUserAvatar(IFormFile userImage)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                TempData[MessageConstants.MESSAGE] = "Người dùng không tồn tại";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                return RedirectToAction("Index", "Home");
            }
            await userService.UpdateUserAvatar(userImage, user);
            return RedirectToAction("Index", "User");
        }
        catch (Exception e)
        {
            TempData[MessageConstants.MESSAGE] = e.Message;
            TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
            return RedirectToAction("Index", "User");
        }
    }
    
    public async Task<IActionResult> UpdateUsername(string newUsername)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                TempData[MessageConstants.MESSAGE] = "Người dùng không tồn tại";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(newUsername) || newUsername.Length < 3 || newUsername.Length > 50)
            {
                TempData[MessageConstants.MESSAGE] = "Tên người dùng phải từ 3 đến 50 ký tự";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                return RedirectToAction("Index", "User");
            }

            var existingUser = await userService.GetUserByNameAsync(newUsername);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                TempData[MessageConstants.MESSAGE] = "Tên người dùng đã được sử dụng";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                return RedirectToAction("Index", "User");
            }

            user.Username = newUsername;
            var result = await userService.ReplaceUserAsync(user);
            if (!result)
            {
                throw new Exception("Cập nhật tên người dùng thất bại");
            }

            TempData[MessageConstants.MESSAGE] = "Cập nhật tên người dùng thành công";
            TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.SUCCESS;
            return RedirectToAction("Index", "User");
        }
        catch (Exception e)
        {
            TempData[MessageConstants.MESSAGE] = e.Message;
            TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
            return RedirectToAction("Index", "User");
        }
    }
    
    public async Task<IActionResult> UpdateStreamerName(string streamerName)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                TempData[MessageConstants.MESSAGE] = "Người dùng không tồn tại";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                return RedirectToAction("Index", "Home");
            }

            if (string.IsNullOrEmpty(streamerName) || streamerName.Length < 3 || streamerName.Length > 50)
            {
                TempData[MessageConstants.MESSAGE] = "Tên người dùng phải từ 3 đến 50 ký tự";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                return RedirectToAction("Index", "User");
            }

            var existingUser = await userService.GetUserByNameAsync(streamerName);
            user.Username = streamerName;
            var result = await userService.ReplaceUserAsync(user);
            if (!result)
            {
                throw new Exception("Cập nhật tên người dùng thất bại");
            }

            TempData[MessageConstants.MESSAGE] = "Cập nhật tên người dùng thành công";
            TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.SUCCESS;
            return RedirectToAction("Index", "User");
        }
        catch (Exception e)
        {
            TempData[MessageConstants.MESSAGE] = e.Message;
            TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
            return RedirectToAction("Index", "User");
        }
    }
    
    private async Task<User?> GetCurrentUserAsync()
    {
        // var userId = HttpContext.Session.GetString(UserConstants.UserId);
        var userId = User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            return null;
        }

        var user = await userService.GetUserByIdAsync(ObjectId.Parse(userId));
        return user;
    }
}