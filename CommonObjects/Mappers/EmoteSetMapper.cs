using BusinessObjects;
using CommonObjects.DTOs.EmoteSetDTOs;
using CommonObjects.ViewModels.EmoteSetVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonObjects.DTOs.API.EmoteResponse;
using CommonObjects.DTOs.API.EmoteSetResponse;
using MongoDB.Bson;

namespace CommonObjects.Mappers
{
    public static class EmoteSetMapper
    {
        public static EmoteSetPreviewVM ToPreviewVM(EmoteSet emoteSet, List<Emote> emotes)
        {
            var r = new EmoteSetPreviewVM
            {
                Emotes = emotes.Select(e => new EmoteSetEmotePreviewVM
                {
                    Id = e.Id,
                    Url = e.Files.LastOrDefault().Url,

                }).ToList(),
                EmoteSetId = emoteSet.Id,
                EmoteCapacity = emoteSet.Capacity,
                EmoteCount = emoteSet.Emotes.Count,
                EmoteSetName = emoteSet.Name,
                IsActive = emoteSet.IsActive,
                CreatedAt = emoteSet.CreatedAt,
            };
            return r;
        }

        public static EmoteSetDetailVM ToDetailVM(EmoteSet emoteSet, List<Emote> emotes, User owner)
        {
            var r = new EmoteSetDetailVM
            {
                Emotes = emotes,
                EmoteSet = emoteSet,
                Owner = owner
            };
            return r;
        }

        public static EmoteSetInfoDTO ToInfoDTO(EmoteSet activeSets, List<Emote> emotes, List<User> users)
        {
            var r = new EmoteSetInfoDTO
            {
                Id = activeSets.Id.ToString(),
                Name = activeSets.Name,
                OwnerId = activeSets.OwnerId.ToString(),
                OwnerName = "", // This should be filled in the service layer
                Emotes = emotes.Select(e => new EmoteInfoDTO
                {
                    Id = e.Id.ToString(),
                    Name = e.Name,
                    ByUser = users.FirstOrDefault(u => u.Id == e.OwnerId) ?? new User { Id = ObjectId.Empty, Username = "Unknown" },
                    Files = e.Files,
                    IsOverlaying = e.IsOverlaying,
                    Status = e.Status,
                    Visibility = e.Visibility,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                }).ToList(),
                Capacity = activeSets.Capacity,
                IsActive = activeSets.IsActive,
                CreatedAt = activeSets.CreatedAt,
                UpdatedAt = activeSets.UpdatedAt,
            };
            return r;
        }
    }
}
