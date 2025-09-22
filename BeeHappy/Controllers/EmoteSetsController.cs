using BusinessObjects;
using BusinessObjects.NestedObjects;
using CommonObjects.AppConstants;
using CommonObjects.DTOs.EmoteSetDTOs;
using CommonObjects.DTOs.EmoteSetDTOs.API;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;
using System.Linq;

namespace BeeHappy.Controllers
{
    public class EmoteSetsController(IUserService userService,
                                     IEmoteSetService emoteSetService,
                                     IEmoteService emoteService) : Controller
    {
        // Get all emote sets of the current user
        public async Task<IActionResult> Index()
        {
            // TODO: DELETE THIS WHEN AUTHENTICATION IS IMPLEMENTED
            HttpContext.Session.SetString(UserConstants.UserId, "68caea0b352f76be3fd4972d");
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // TODO: DELETE THIS WHEN THE CREATE EMOTE SET IS IMPLEMENTED
            CreateDummyEmoteAndEmoteSetForUser();
            var userEmoteSets = await emoteSetService.GetEmoteSetPreviewsOfUserAsync(currentUser.Id);
            return View(userEmoteSets);
        }

        public void CreateDummyEmoteAndEmoteSetForUser()
        {
            // Check if user already has emote sets or emote
            var currentUser = GetCurrentUserAsync().Result;
            if (currentUser == null)
            {
                return;
            }
            var existingEmoteSets = emoteSetService.GetEmoteSetsAsync(es => es.OwnerId == currentUser.Id).Result;
            var existingEmotes = emoteService.GetEmotesAsync(e => e.OwnerId == currentUser.Id).Result;
            if (existingEmotes.Count == 0)
            {
                var emote = new Emote
                {
                    Name = "Emote thử nghiệm",
                    OwnerId = ObjectId.Parse("68caea0b352f76be3fd4972d"),
                    Files = new List<EmoteFile>
                {
                    new EmoteFile
                    {
                        Url = "https://cdn.7tv.app/emote/01FB0BQR2000033EKAWRHEXZ34/4x.avif",
                        Format = "png",
                        Size = 1024,
                    }
                },
                    Tags = new List<string> { "vui", "hài hước" },
                    Visibility = new List<string> { "public" },
                    Status = new List<string> { "active" },
                    IsOverlaying = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                emoteService.InsertEmoteAsync(emote).Wait();
                if (existingEmoteSets.Count == 0)
                {
                    var emoteSet = new EmoteSet
                    {
                        Name = "Bộ emote thử nghiệm",
                        OwnerId = ObjectId.Parse("68caea0b352f76be3fd4972d"),
                        Emotes = new List<ObjectId> { emote.Id },
                        Capacity = 50,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                    };
                    emoteSetService.InsertEmoteSetAsync(emoteSet).Wait();
                }
            }
        }

        // GET: EmoteSetsController/Details/5
        public async Task<ActionResult> DetailsAsync(ObjectId id)
        {
            try
            {
                var r = await emoteSetService.GetEmoteSetDetailByIdAsync(id);
                return View(r);
            }
            catch (Exception e)
            {
                TempData[MessageConstants.MESSAGE] = e.Message;
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.ERROR;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(CreateEmoteSetDto createDto)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                // Set the owner ID from the current user
                createDto.OwnerId = currentUser.Id;

                await emoteSetService.InsertEmoteSetAsync(createDto);
                TempData[MessageConstants.MESSAGE] = "Tạo bộ emote thành công";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.SUCCESS;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData[MessageConstants.MESSAGE] = $"Lỗi khi tạo bộ emote: {e.Message}";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.ERROR;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        public async Task<ActionResult> EditEmoteSetAsync(EditEmoteSetDto dto)
        {
            try
            {
                await emoteSetService.UpdateEmoteSetAsync(dto);
                TempData[MessageConstants.MESSAGE] = "Thông tin bộ emote đã được thay đổi!";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.SUCCESS;
                return RedirectToAction("Details", new { id = dto.Id });
            }
            catch (Exception e)
            {
                TempData[MessageConstants.MESSAGE] = $"Lỗi khi cập nhật bộ emote: {e.Message}";
                TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.ERROR;
                return RedirectToAction("Details", new { id = dto.Id });
            }
        }

        // API
        [HttpPost]
        public async Task<IActionResult> ToggleEmoteSetActiveStatus(ObjectId emoteSetId)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                await emoteSetService.ToggleEmoteSetActiveStatus(emoteSetId, currentUser.Id);
                return Ok(new EmoteSetResponseDto
                {
                    message = "Thay đổi trạng thái bộ emote thành công",
                    success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new EmoteSetResponseDto
                {
                    message = "Thay đổi trạng thái bộ emote thất bại: " + e.Message,
                    success = false,
                });
            }
        }

        // POST: EmoteSetsController/Delete/5
        // API method
        [HttpPost]
        public async Task<ActionResult> DeleteAsync(ObjectId id)
        {
            try
            {
                var currentUser = await GetCurrentUserAsync();
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                await emoteSetService.DeleteEmoteSetByIdAsync(id);
                return Ok(new EmoteSetResponseDto
                {
                    message = "Xoá bộ emote thành công",
                    success = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new EmoteSetResponseDto
                {
                    message = "Xoá bộ emote thất bại: " + e.Message,
                    success = false,
                });
            }
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            var userId = HttpContext.Session.GetString(UserConstants.UserId);
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }
            var user = await userService.GetUserByIdAsync(ObjectId.Parse(userId));
            return user;
        }
    }
}