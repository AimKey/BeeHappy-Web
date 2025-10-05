using BusinessObjects;
using CommonObjects.DTOs.API.EmoteResponse;
using CommonObjects.DTOs.EmoteSetDTOs.API;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers.API;

[Route("api/emotes")]
[ApiController]
public class EmoteApiController : ControllerBase
{
    private readonly IEmoteService _emoteService;

    public EmoteApiController(IEmoteService emoteService)
    {
        _emoteService = emoteService;
    }

    [HttpGet]
    public async Task<IActionResult>GetAll()
    {
        var emotes = await _emoteService.GetAllEmotesAsync();
        return Ok(emotes);
    }
    
    [HttpGet("name/{emoteName}")]
    public async Task<EmoteResponseObject> GetByName(string emoteName)
    {
        throw new NotImplementedException();
    }
    
    [HttpGet("id/{emoteId}")]
    public async Task<EmoteResponseObject> GetById(ObjectId emoteId)
    {
        throw new NotImplementedException();
    }
}
