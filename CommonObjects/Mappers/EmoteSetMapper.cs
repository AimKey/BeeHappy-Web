using BusinessObjects;
using CommonObjects.DTOs.EmoteSetDTOs;
using CommonObjects.ViewModels.EmoteSetVMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
