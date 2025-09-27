using BusinessObjects.NestedObjects;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using CommonObjects.Pagination;
using CommonObjects.ViewModels.Emote;

namespace BeeHappy.Controllers.Emotes;

public class EmotesController : Controller
{

    public IActionResult Directory(int page = 1, int pageSize = 20, string search = "", string tags = "", string[] filters = null)
    {
        ViewBag.Search = search;
        ViewBag.Tags = tags;
        ViewBag.Filters = filters ?? new string[0];
        ViewBag.CurrentPage = page;

        var emotesData = GetEmotesData(page, pageSize, search, tags, filters);
        return View(emotesData);
    }

    [HttpGet]
    public IActionResult GetEmotes(int page = 1, int pageSize = 20, string search = "", string tags = "", string[] filters = null)
    {
        var emotesData = GetEmotesData(page, pageSize, search, tags, filters);
        return Json(emotesData);
    }

    [HttpGet]
    public IActionResult Detail()
    {
        return View(new EmoteDetailsViewModel());
    }

    private PagedResult<object> GetEmotesData(int page, int pageSize, string search, string tags, string[] filters)
    {
        // Mock data for demonstration
        var mockEmotes = GenerateMockEmotes();

        // Apply filters
        var filteredEmotes = mockEmotes.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            filteredEmotes = filteredEmotes.Where(e => e.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(tags))
        {
            var tagArray = tags.Split(',', StringSplitOptions.RemoveEmptyEntries)
                              .Select(t => t.Trim())
                              .Where(t => !string.IsNullOrEmpty(t))
                              .ToArray();
            if (tagArray.Length > 0)
            {
                filteredEmotes = filteredEmotes.Where(e => e.Tags.Any(t => tagArray.Contains(t)));
            }
        }

        if (filters != null && filters.Length > 0)
        {
            foreach (var filter in filters)
            {
                switch (filter.ToLower())
                {
                    case "animated":
                        filteredEmotes = filteredEmotes.Where(e => e.Files.Any(f => f.Format == "gif"));
                        break;
                    case "static":
                        filteredEmotes = filteredEmotes.Where(e => e.Files.All(f => f.Format != "gif"));
                        break;
                    case "overlaying":
                        filteredEmotes = filteredEmotes.Where(e => e.IsOverlaying);
                        break;
                }
            }
        }

        var totalCount = filteredEmotes.Count();
        var emotes = filteredEmotes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new
            {
                id = e.Id.ToString(),
                name = e.Name,
                owner = GetOwnerName(e.OwnerId),
                thumbnail = e.Files.FirstOrDefault().Url,
                isAnimated = e.Files.Any(f => f.Format == "gif"),
                tags = e.Tags
            })
            .Cast<object>()
            .ToList();
       
        return new PagedResult<object>
        {
            Items = emotes,
            TotalCount = totalCount,
            PageSize = pageSize,
            CurrentPage = page
        };
    }

    private List<Emote> GenerateMockEmotes()
    {
        var mockEmotes = new List<Emote>();
        var emoteNames = new[] { "GIGACHAD", "NOOOO", "catJAM", "BOOBA", "COPIUM", "Prayge", "OMEGALUL", "catKISS", "Bedge", "NODDERS", "Sadge", "DIESOFCRINGE", "modCheck", "peppoHey", "donowall", "SUSSY", "Aware", "Clueless", "KEKW", "xdd", "Hmm", "ratJAM", "LETSGO", "SNIFFA", "muted", "peepClap", "AINTNOWAY", "peepoDJ", "pepeJAM", "peppoGiggles", "peppoLeave", "YEP", "RAGEY", "Pog", "Madge", "peppoShy" };
        var owners = new[] { "Beutelino", "JustRogan", "pascL_", "evilmessy", "Neowav", "Mauripiss", "deadYZ", "PARUNA", "ethamp", "babygarten", "laden", "JustRogan", "Ttoots", "EpicDonutDude_", "MegaKill3", "100mp", "niochek", "daledo", "PatrickMaybo", "DelphoxTube", "RoDrArGel2004", "eazylemmqeezy", "rSnowWolf", "ShadowTheHedge", "Simon38", "BoopsKat", "HotBear110", "KAFTNN", "Teyn", "aekng", "laden" };

        for (int i = 0; i < emoteNames.Length; i++)
        {
            mockEmotes.Add(new Emote
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Name = emoteNames[i],
                OwnerId = ObjectId.GenerateNewId(),
                Files = new List<EmoteFile>
                    {
                        new EmoteFile
                        {
                            Url = "/gifs/Dance.gif",
                            Format = "png",
                            Size = 455
                        }
                    },
                Tags = new List<string> { "popular", i % 2 == 0 ? "animated" : "static" },
                Visibility = new List<string> { "public" },
                Status = new List<string> { "approved" },
                IsOverlaying = i % 5 == 0,
                CreatedAt = DateTime.Now.AddDays(-i),
                UpdatedAt = DateTime.Now.AddDays(-i)
            });
        }

        return mockEmotes;
    }

    private string GetOwnerName(ObjectId ownerId)
    {
        var owners = new[] { "Beutelino", "JustRogan", "pascL_", "evilmessy", "Neowav", "Mauripiss", "deadYZ", "PARUNA" };
        return owners[Math.Abs(ownerId.GetHashCode()) % owners.Length];
    }
}
