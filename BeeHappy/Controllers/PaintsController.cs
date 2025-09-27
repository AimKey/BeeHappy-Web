using System.Security.Claims;
using BusinessObjects;
using CommonObjects.DTOs.PaintDTOs.API;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers;

public class PaintsController(
    IUserService userService,
    IPaintService paintService) : Controller
{
    // API
    [HttpPost]
    public async Task<IActionResult> ActivePaint(ObjectId paintId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return BadRequest(new PaintResponse
            {
                message = "Vui lòng đăng nhập để thực hiện chức năng này.",
                success = false
            });
        }

        try
        {
            await paintService.ActivePaintForUserAsync(currentUser, paintId);
            return Ok(new PaintResponse
            {
                message = "Kích hoạt màu thành công.",
                success = true
            });
        }
        catch (Exception e)
        {
            return BadRequest(new PaintResponse
            {
                message = "Có lỗi xảy ra: " + e.Message,
                success = false
            });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> DeactivateAllPaints()
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null)
        {
            return BadRequest(new PaintResponse
            {
                message = "Vui lòng đăng nhập để thực hiện chức năng này.",
                success = false
            });
        }

        try
        {
            await paintService.DeactivateAllPaintsForUserAsync(currentUser);
            return Ok(new PaintResponse
            {
                message = "Kích hoạt màu thành công.",
                success = true
            });
        }
        catch (Exception e)
        {
            return BadRequest(new PaintResponse
            {
                message = "Có lỗi xảy ra: " + e.Message,
                success = false
            });
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