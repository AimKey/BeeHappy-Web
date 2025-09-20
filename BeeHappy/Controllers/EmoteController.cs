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

        public EmoteController(IEmoteService emoteService)
        {
            _emoteService = emoteService;
        }

        public async Task<IActionResult> Index()
        {
            var emotes = await _emoteService.GetAllEmotesAsync();
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
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            try
            {                

                // Tạm thời generate OwnerId (sau này lấy từ User.Identity)
                var ownerId = ObjectId.GenerateNewId();

                var emoteId = ObjectId.GenerateNewId();
                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "emotes", emoteId.ToString()
                );
                Directory.CreateDirectory(uploadPath);

                var emote = new Emote
                {
                    Id = emoteId,
                    Name = vm.Name,
                    OwnerId = ownerId,
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

    }
}
