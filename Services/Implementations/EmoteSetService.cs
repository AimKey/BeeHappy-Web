using AutoMapper;
using BusinessObjects;
using CommonObjects.DTOs.EmoteSetDTOs;
using CommonObjects.Mappers;
using CommonObjects.ViewModels.EmoteSetVMs;
using DataAccessObjects;
using MongoDB.Bson;
using MongoDB.Driver;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class EmoteSetService(
        IEmoteSetRepository emoteSetRepository,
        IEmoteService emoteService,
        IUserService userService,
        IEmoteRepository emoteRepository,
        IMapper mapper,
        MongoDBContext mongoDBContext) : IEmoteSetService
    {
        public async Task<List<EmoteSetPreviewVM>> GetEmoteSetPreviewsOfUserAsync(ObjectId userId)
        {
            var sets = await GetEmoteSetsAsync(es => es.OwnerId == userId);
            var previews = new List<EmoteSetPreviewVM>();

            foreach (var set in sets)
            {
                var collection = mongoDBContext.Database.GetCollection<Emote>(typeof(Emote).Name);
                var emotes = await collection.Find(e => set.Emotes != null && set.Emotes.Contains(e.Id))
                    .Limit(10)
                    .ToListAsync();

                var preview = EmoteSetMapper.ToPreviewVM(set, emotes);
                previews.Add(preview);
            }

            // Sort ascending by CreatedAt
            previews.Sort((a, b) => a.CreatedAt.CompareTo(b.CreatedAt));
            return previews;
        }

        public async Task<List<EmoteSet>> GetAllEmoteSetsAsync(CancellationToken ct = default)
        {
            return await emoteSetRepository.GetAllAsync(ct);
        }

        public async Task<List<EmoteSet>> GetEmoteSetsAsync(
            System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter, CancellationToken ct = default)
        {
            return await emoteSetRepository.GetAsync(filter, ct);
        }

        public async Task<EmoteSet?> GetEmoteSetByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await emoteSetRepository.GetByIdAsync(id, ct);
        }

        public async Task InsertEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default)
        {
            await emoteSetRepository.InsertAsync(emoteSet, ct);
        }

        public async Task<bool> ReplaceEmoteSetAsync(EmoteSet emoteSet, bool upsert = false,
            CancellationToken ct = default)
        {
            return await emoteSetRepository.ReplaceAsync(emoteSet, upsert, ct);
        }

        public async Task<bool> DeleteEmoteSetByIdAsync(ObjectId id, CancellationToken ct = default)
        {
            return await emoteSetRepository.DeleteByIdAsync(id, ct);
        }

        public async Task<bool> DeleteEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default)
        {
            return await emoteSetRepository.DeleteAsync(emoteSet, ct);
        }

        public async Task<long> CountEmoteSetsAsync(
            System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter = null, CancellationToken ct = default)
        {
            return await emoteSetRepository.CountAsync(filter, ct);
        }

        public async Task<EmoteSetDetailVM> GetEmoteSetDetailByIdAsync(ObjectId id)
        {
            var emoteSet = await GetEmoteSetByIdAsync(id) ?? throw new Exception("Không tìm thấy bộ emote");
            if (emoteSet == null)
            {
                throw new Exception("Không tìm thấy bộ emote");
            }

            var owner = await userService.GetUserByIdAsync(emoteSet.OwnerId);
            if (owner == null)
                throw new Exception("Không tìm thấy chủ sở hữu của bộ emote");
            var emotes = await emoteService.GetEmotesAsync(e => emoteSet.Emotes.Contains(e.Id));
            // Map to the detail VM
            var vm = EmoteSetMapper.ToDetailVM(emoteSet, emotes, owner);
            return vm;
        }

        public async Task InsertEmoteSetAsync(CreateEmoteSetDto createDto)
        {
            var emoteSet = mapper.Map<EmoteSet>(createDto);
            List<string> tagStrings = GetListTagsFromString(createDto.TagsString);

            await InsertEmoteSetAsync(new EmoteSet
            {
                Name = createDto.Name,
                OwnerId = createDto.OwnerId,
                Tags = tagStrings,
                Capacity = createDto.Capacity,
                IsActive = createDto.IsActive,
                Emotes = new List<ObjectId>(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
        }

        public async Task UpdateEmoteSetAsync(EditEmoteSetDto dto)
        {
            var emoteSet = await GetEmoteSetByIdAsync(dto.Id) ?? throw new Exception("Không tìm thấy bộ emote");
            if (emoteSet.OwnerId != dto.OwnerId)
            {
                throw new Exception("Bạn không sở hữu bộ emote này");
            }

            List<string> tagStrings = GetListTagsFromString(dto.TagsString);
            emoteSet.Name = dto.Name;
            emoteSet.Tags = tagStrings;
            emoteSet.Capacity = dto.Capacity;
            emoteSet.IsActive = dto.IsActive;
            emoteSet.UpdatedAt = DateTime.UtcNow;
            var success = await ReplaceEmoteSetAsync(emoteSet);
            if (!success)
            {
                throw new Exception("Cập nhật bộ emote thất bại");
            }
        }

        // Toggle the new emote set and turn off all of the other one

        public async Task ToggleEmoteSetActiveStatus(ObjectId emoteSetId, ObjectId userId)
        {
            var emoteSet = await GetEmoteSetByIdAsync(emoteSetId) ?? throw new Exception("Emote set not found");
            if (emoteSet.OwnerId != userId)
            {
                throw new Exception("You do not own this emote set");
            }

            if (emoteSet.IsActive)
            {
                await ToggleCurrentEmoteSetActiveStatus(emoteSet);
            }
            else
            {
                await ToggleMultipleEmoteSetActiveStatus(userId, emoteSet);
            }
        }

        private async Task ToggleMultipleEmoteSetActiveStatus(ObjectId userId, EmoteSet emoteSet)
        {
            var userEmoteSets = await GetEmoteSetsAsync(es => es.OwnerId == userId && es.IsActive);
            foreach (var set in userEmoteSets)
            {
                set.IsActive = false;
                set.UpdatedAt = DateTime.UtcNow;
                var success = await ReplaceEmoteSetAsync(set);
                if (!success)
                {
                    throw new Exception("Cập nhật bộ emote thất bại");
                }
            }

            emoteSet.IsActive = true;
            emoteSet.UpdatedAt = DateTime.UtcNow;
            await ReplaceEmoteSetAsync(emoteSet);
        }

        private async Task ToggleCurrentEmoteSetActiveStatus(EmoteSet emoteSet)
        {
            emoteSet.IsActive = !emoteSet.IsActive;
            emoteSet.UpdatedAt = DateTime.UtcNow;
            var success = await ReplaceEmoteSetAsync(emoteSet);
            if (!success)
            {
                throw new Exception("Cập nhật bộ emote thất bại");
            }
        }

        private static List<string> GetListTagsFromString(string tagString)
        {
            List<string> tagStrings = new List<string>();
            if (!string.IsNullOrEmpty(tagString))
            {
                tagStrings = tagString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(tag => tag.Trim())
                    .ToList();
            }

            return tagStrings;
        }

        public async Task<bool> AddEmoteToSetAsync(ObjectId emoteSetId, ObjectId emoteId,
            CancellationToken ct = default)
        {
            var set = await emoteSetRepository.GetByIdAsync(emoteSetId, ct);
            if (set == null) 
                throw new Exception("Bộ emote không tồn tại.");

            if (set.Emotes == null)
                set.Emotes = new List<ObjectId>();

            // check duplicate
            if (set.Emotes.Contains(emoteId)) throw new Exception("Emote đã tồn tại trong bộ emote.");
            
            // check capacity
            if (set.Capacity > 0 && set.Emotes.Count >= set.Capacity)
                throw new InvalidOperationException("Bộ emote đã đạt đến sức chứa tối đa.");

            set.Emotes.Add(emoteId);
            return await emoteSetRepository.ReplaceAsync(set, false, ct);
        }

        public Task RemoveEmoteFromSetAsync(ObjectId emoteSetId, ObjectId emoteId)
        {
            var emoteSet = emoteSetRepository.GetByIdAsync(emoteSetId)
                .Result ?? throw new Exception("Bộ emote không tồn tại.");
            if (emoteSet.Emotes == null || !emoteSet.Emotes.Contains(emoteId))
            {
                emoteSet.Emotes = new List<ObjectId>();
                throw new Exception("Emote không tồn tại trong bộ emote.");
            }
            emoteSet.Emotes.Remove(emoteId);
            return emoteSetRepository.ReplaceAsync(emoteSet, false);
        }
    }
}