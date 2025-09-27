using BeeHappy.ViewModels;
using BusinessObjects;
using CommonObjects.AppConstants;
using CommonObjects.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MongoDB.Bson;
using MongoDB.Driver;
using Services.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace BeeHappy.Controllers
{
    public class EmoteController : Controller
    {
        private readonly IEmoteService _emoteService;

        public EmoteController(IEmoteService emoteService)
        {
            _emoteService = emoteService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 20, string search = "", string tags = "", string[] filters = null)
        {
            ViewBag.Search = search;
            ViewBag.Tags = tags;
            ViewBag.Filters = filters ?? new string[0];
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;

            var emotes = await _emoteService.GetFilteredEmotesAsync(page, pageSize, userId: "", search, tags, filters);

            var vm = emotes.Items.Select(e => new EmoteViewModel
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
                Visibility = e.Visibility ?? new List<string> { "Public"},
                Status = e.Status ?? new List<string> { EmoteStatusConstants.ACTIVE},
                IsOverlaying = e.IsOverlaying,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();

            var pagedResult = new PagedResult<EmoteViewModel>
            {
                Items = vm,
                CurrentPage = emotes.CurrentPage,
                PageSize = emotes.PageSize,
                TotalCount = emotes.TotalCount,
            };

            return View(pagedResult);
        }

        [HttpGet]
        public IActionResult Create() => View();


        [HttpPost]
        public async Task<IActionResult> Create(EmoteViewModel vm)
        {
            if (string.IsNullOrWhiteSpace(vm.Name))
                ModelState.AddModelError("Name", "Name is required.");

            if (vm.Files == null || vm.Files.Count == 0 || vm.Files[0].File == null)
                ModelState.AddModelError("File", "Please upload an image.");

            if (!ModelState.IsValid)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(ModelState);
                return View(vm);
            }

            try
            {
                //var ownerId = ObjectId.GenerateNewId();
                var ownerId = "";
                try
                {
                    ownerId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (string.IsNullOrEmpty(ownerId))
                    {
                        TempData[MessageConstants.MESSAGE] = "Vui lòng đăng nhập";
                        TempData[MessageConstants.MESSAGE_TYPE] = MessageConstants.WARNING;
                        return RedirectToAction("Index", "Home");
                    }
                }
                catch
                {
                    return RedirectToAction(nameof(Index));
                }

                var emoteId = ObjectId.GenerateNewId();

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var maxFileSize = 2 * 1024 * 1024; // 2MB

                var file = vm.Files[0].File;
                var ext = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                    return BadRequest(new { error = "Only JPG, PNG, GIF files are allowed." });

                if (file.Length > maxFileSize)
                    return BadRequest(new { error = "File size cannot exceed 2 MB." });

                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot", "uploads", "emotes", emoteId.ToString()
                );
                Directory.CreateDirectory(uploadPath);

                // handle tags
                var tags = new List<string>();
                if (vm.Tags != null && vm.Tags.Any())
                {
                    tags = vm.Tags
                        .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries)) // tách từng phần tử
                        .Select(t => t.Trim())
                        .ToList();
                }

                var emote = new Emote
                {
                    Id = emoteId,
                    Name = vm.Name,
                    OwnerId = ObjectId.Parse(ownerId),
                    Tags = tags,
                    IsOverlaying = vm.IsOverlaying,
                    Visibility = vm.Visibility,
                    Status = new List<string> { EmoteStatusConstants.ACTIVE},
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Files = new List<BusinessObjects.NestedObjects.EmoteFile>()
                };

                // ✅ Resize thành 4 size
                int[] sizes = { 32, 64, 96, 128 };
                using (var image = await SixLabors.ImageSharp.Image.LoadAsync(file.OpenReadStream()))
                {
                    foreach (var size in sizes)
                    {
                        string fileName = $"{size}x{size}{ext}";
                        string filePath = Path.Combine(uploadPath, fileName);

                        // resize copy để không ảnh hưởng ảnh gốc
                        using (var clone = image.Clone(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
                        {
                            Size = new SixLabors.ImageSharp.Size(size, size),
                            Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop
                        })))
                        {
                            await clone.SaveAsync(filePath);
                        }

                        var fileInfo = new FileInfo(filePath);
                        emote.Files.Add(new BusinessObjects.NestedObjects.EmoteFile
                        {
                            Format = ext.TrimStart('.'),
                            Url = $"/uploads/emotes/{emoteId}/{fileName}",
                            Size = (int)fileInfo.Length
                        });
                    }
                }

                await _emoteService.InsertEmoteAsync(emote);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return Json(new { success = true, emoteId = emote.Id.ToString() });

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                    return BadRequest(new { error = $"Lỗi khi tạo Emote: {ex.Message}" });

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

            // handle tags
            var tags = new List<string>();
            if (vm.Tags != null && vm.Tags.Any())
            {
                tags = vm.Tags
                    .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries)) // tách từng phần tử
                    .Select(t => t.Trim())
                    .ToList();
            }

            var emote = new Emote
            {
                Id = ObjectId.Parse(vm.Id),
                Name = vm.Name,
                Tags = tags,
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


    }
}
