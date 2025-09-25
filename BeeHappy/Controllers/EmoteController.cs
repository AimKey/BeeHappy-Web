using BeeHappy.ViewModels;
using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Services.Interfaces;

namespace BeeHappy.Controllers
{
    public class EmoteController : Controller
    {
        private readonly IEmoteService _emoteService;
        private readonly IUserService _userService;


        public EmoteController(IEmoteService emoteService, IUserService userService)
        {
            _emoteService = emoteService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await GetCurrentUser();
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // Chỉ lấy emotes của user này
            var emotes = await _emoteService.GetEmotesAsync(e => e.OwnerId == currentUser.Id);

            var vm = emotes.Select(e => new EmoteViewModel
            {
                Id = e.Id.ToString(),
                Name = e.Name,
                OwnerId = e.OwnerId.ToString(),
                Files = e.Files?.Select(f => new EmoteFileViewModel
                {
                    Format = f.Format,
                    Url = f.Url,
                    Size = f.Size
                }).ToList(),
                Tags = e.Tags,
                Visibility = e.Visibility,
                Status = e.Status,
                IsOverlaying = e.IsOverlaying,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();

            return View(vm);
        }

        [HttpGet]
        public IActionResult Create() => View();


        [HttpPost]
        public async Task<IActionResult> Create(EmoteViewModel vm)
        {
            // Validate bắt buộc Name
            if (string.IsNullOrWhiteSpace(vm.Name))
            {
                ModelState.AddModelError("Name", "Name is required.");
            }

            // Validate bắt buộc có ít nhất 1 file upload
            if (vm.Files == null || !vm.Files.Any(f => f.File != null && f.File.Length > 0))
            {
                ModelState.AddModelError("Files", "Please upload at least one image (jpg, png, gif).");
            }

            if (!ModelState.IsValid)
            {
                return View(vm);
            }
            try
            {
                var currentUser = await GetCurrentUser();
                if (currentUser == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                

                var emoteId = ObjectId.GenerateNewId();
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var maxFileSize = 2 * 1024 * 1024; // 2MB
                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "emotes", emoteId.ToString()
                );
                Directory.CreateDirectory(uploadPath);

                var emote = new Emote
                {
                    Id = emoteId,
                    Name = vm.Name,
                    OwnerId = currentUser.Id,
                    Tags = vm.Tags ?? new List<string>(), // bind trực tiếp từ form
                    IsOverlaying = vm.IsOverlaying,
                    Visibility = ["public"], // mặc định
                    Status = ["active"],     // mặc định
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Files = new List<BusinessObjects.NestedObjects.EmoteFile>()
                };

                if (vm.Files != null)
                {
                    foreach (var fileVm in vm.Files)
                    {
                        if (fileVm.File != null && fileVm.File.Length > 0)
                        {
                            var ext = Path.GetExtension(fileVm.File.FileName).ToLower();

                            if (!allowedExtensions.Contains(ext))
                            {
                                ModelState.AddModelError("Files", "Only JPG, PNG, GIF files are allowed.");
                                return View(vm);
                            }

                            if (fileVm.File.Length > maxFileSize)
                            {
                                ModelState.AddModelError("Files", "File size cannot exceed 2 MB.");
                                return View(vm);
                            }

                            string fileName = Path.GetFileName(fileVm.File.FileName);
                            string filePath = Path.Combine(uploadPath, fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await fileVm.File.CopyToAsync(stream);
                            }

                            emote.Files.Add(new BusinessObjects.NestedObjects.EmoteFile
                            {
                                Format = Path.GetExtension(fileName).TrimStart('.'),
                                Url = $"/uploads/emotes/{emoteId}/{fileName}",
                                Size = (int)fileVm.File.Length
                            });
                        }
                    }
                }

                await _emoteService.InsertEmoteAsync(emote);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi khi tạo Emote: {ex.Message}");
                return View(vm);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var emote = await _emoteService.GetEmoteByIdAsync(ObjectId.Parse(id));
            if (emote == null) return NotFound();

            var vm = new EmoteViewModel
            {
                Id = emote.Id.ToString(),
                Name = emote.Name,
                Tags = emote.Tags,
                Visibility = emote.Visibility,
                Status = emote.Status,
                IsOverlaying = emote.IsOverlaying
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmoteViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var emote = new Emote
            {
                Id = ObjectId.Parse(vm.Id),
                Name = vm.Name,
                Tags = vm.Tags,
                IsOverlaying = vm.IsOverlaying,
                Visibility = vm.Visibility,
                Status = vm.Status
            };

            await _emoteService.ReplaceEmoteAsync(emote);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var emote = await _emoteService.GetEmoteByIdAsync(ObjectId.Parse(id));
            if (emote == null) return NotFound();

            var vm = new EmoteViewModel
            {
                Id = emote.Id.ToString(),
                Name = emote.Name
            };
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            await _emoteService.DeleteEmoteByIdAsync(ObjectId.Parse(id));
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var emote = await _emoteService.GetEmoteByIdAsync(ObjectId.Parse(id));
            if (emote == null) return NotFound();

            var vm = new EmoteViewModel
            {
                Id = emote.Id.ToString(),
                Name = emote.Name,
                OwnerId = emote.OwnerId.ToString(),
                Tags = emote.Tags,
                Visibility = emote.Visibility,
                Status = emote.Status,
                IsOverlaying = emote.IsOverlaying,
                CreatedAt = emote.CreatedAt,
                UpdatedAt = emote.UpdatedAt,
                Files = emote.Files?.Select(f => new EmoteFileViewModel
                {
                    Format = f.Format,
                    Url = f.Url,
                    Size = f.Size
                }).ToList()
            };

            return View(vm);
        }
        private async Task<User?> GetCurrentUser()
        {
            var userId = HttpContext.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId)) return null;

            return await _userService.GetUserByIdAsync(ObjectId.Parse(userId));
        }

    }
}
