using Microsoft.AspNetCore.Mvc;

namespace BeeHappy.Controllers.API;

[Route("api/users")]
[ApiController]
public class UserApiController : ControllerBase
{
    [HttpGet("id/{userId}")]
    public async Task GetUserById([FromQuery] string userId)
    {
    }

    [HttpGet("name/{userName}")]
    public async Task GetUserByName([FromQuery] string userName)
    {
    }
}