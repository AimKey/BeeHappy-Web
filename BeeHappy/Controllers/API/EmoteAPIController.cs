using BusinessObjects;
using CommonObjects.DTOs.API.EmoteResponse;
using CommonObjects.DTOs.EmoteSetDTOs.API;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers.API;

[Route("api/emotes")]
[ApiController]
public class EmoteApiController(IUserService userService, IEmoteSetService emoteSetService, IEmoteService emoteService)
    : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var emotes =
            await emoteService.GetEmotesInfoDtos(e => e.Visibility.Count > 0 && e.Visibility[0].ToLower() == "public");
        return Ok(emotes);
    }

    [HttpGet("name/{emoteName}")]
    public async Task<EmoteInfoDTO> GetByName(string emoteName)
    {
        throw new NotImplementedException();
    }

    [HttpGet("id/{emoteId}")]
    public async Task<EmoteInfoDTO> GetById(ObjectId emoteId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("sets/user/{userName}")]
    public async Task<IActionResult> GetActiveEmoteSetsFromUser(string userName)
    {
        try
        {
            var user = await userService.GetUserByNameAsync(userName);
            if (user == null)
            {
                return BadRequest("Không tìm thấy người dùng");
            }

            var emoteSet = await emoteSetService.GetActiveEmoteSetInfoFromUser(user);
            return Ok(emoteSet);
        }
        catch (Exception e)
        {
            return BadRequest($"Lỗi khi lấy bộ biểu cảm: {e.Message}");
        }
    }

    // This endpoint sole purpose is to ease the burden on the frontend by returning a list of all emotes containing
    // an indicator of whether the emote is belong to the streamer or is global using.
    // [HttpGet("display/{streamerName}")]
    // public async Task<IActionResult> GetAvailableEmotesOfAStreamerForDisplay(string streamerName)
    // {
    //     
    // }
}