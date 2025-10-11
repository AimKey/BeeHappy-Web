using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Implementations;
using Services.Interfaces;

namespace BeeHappy.Controllers.API;

[Route("api/users")]
[ApiController]
public class UserApiController(IJwtService jwt, IUserService userService) : ControllerBase
{
    // Get current user based on JWT token
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return BadRequest("Empty or invalid Authorization header");
            }

            var token = authHeader.Substring("Bearer ".Length).Trim();

            var userId = jwt.GetUserIdFromToken(token);
            if (string.IsNullOrEmpty(userId))
            {
                return NotFound("Invalid token: User ID not found");
            }
            var user = await userService.GetUserInfo(ObjectId.Parse(userId));
            return Ok(user);
        }
        catch (Exception e)
        {
            return BadRequest($"Error retrieving user: {e.Message}");
        }
    }

    [HttpGet("name/{userName}")]
    public async Task<IActionResult> GetUserByName([FromRoute] string userName)
    {
        try
        {
            var user = await userService.GetUserByNameAsync(userName);
            if (user == null)
            {
                return NotFound("User not found");
            }
            var userDto = await userService.GetUserInfo(user.Id);
            userDto.Paints = userDto.Paints.Where(p => p.IsActive).ToList(); // Only return active paints
            return Ok(userDto);
        }
        catch (Exception e)
        {
            return BadRequest($"Error retrieving user: {e.Message}");
        }
    }
}