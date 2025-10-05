using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BeeHappy.Controllers.API;

[Route("api/streamers")]
[ApiController]
public class StreamerApiController : ControllerBase
{
    
    [HttpGet("{streamerId}/active-emote-set")]
    public async Task GetActiveEmoteSetOfStreamer(ObjectId streamerId)
    {
        
    }
    
    [HttpGet("name/{streamer}")]
    public async Task GetStreamerByName(string streamer)
    {
        
    }
}