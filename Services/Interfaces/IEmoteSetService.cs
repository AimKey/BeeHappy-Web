using BusinessObjects;
using CommonObjects.DTOs.API.EmoteSetResponse;
using CommonObjects.DTOs.EmoteSetDTOs;
using CommonObjects.ViewModels.EmoteSetVMs;
using MongoDB.Bson;

namespace Services.Interfaces
{
    public interface IEmoteSetService
    {
        Task<List<EmoteSet>> GetAllEmoteSetsAsync(CancellationToken ct = default);
        Task<List<EmoteSet>> GetEmoteSetsAsync(System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter, CancellationToken ct = default);
        Task<EmoteSet?> GetEmoteSetByIdAsync(ObjectId id, CancellationToken ct = default);
        Task InsertEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default);
        Task InsertEmoteSetAsync(CreateEmoteSetDto dto);
        Task<bool> ReplaceEmoteSetAsync(EmoteSet emoteSet, bool upsert = false, CancellationToken ct = default);
        Task<bool> DeleteEmoteSetByIdAsync(ObjectId id, CancellationToken ct = default);
        Task<bool> DeleteEmoteSetAsync(EmoteSet emoteSet, CancellationToken ct = default);
        Task<long> CountEmoteSetsAsync(System.Linq.Expressions.Expression<Func<EmoteSet, bool>>? filter = null, CancellationToken ct = default);
        Task<List<EmoteSetPreviewVM>> GetEmoteSetPreviewsOfUserAsync(ObjectId userId);
        Task<EmoteSetDetailVM> GetEmoteSetDetailByIdAsync(ObjectId id);
        /// <summary>
        /// Update an emote set based on the provided EditEmoteSetDto.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task UpdateEmoteSetAsync(EditEmoteSetDto dto);
        Task ToggleEmoteSetActiveStatus(ObjectId emoteSetId, ObjectId userId);
        Task<bool> AddEmoteToSetAsync(ObjectId emoteSetId, ObjectId emoteId, CancellationToken ct = default);
        Task RemoveEmoteFromSetAsync(ObjectId emoteSetId, ObjectId emoteId);
        Task<EmoteSetInfoDTO?> GetActiveEmoteSetInfoFromUser(User user);
    }
}