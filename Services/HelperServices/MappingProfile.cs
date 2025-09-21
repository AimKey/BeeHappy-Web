using AutoMapper;
using BusinessObjects;
using CommonObjects.DTOs.EmoteSetDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.HelperServices
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateEmoteSetDto, EmoteSet>().ReverseMap();
            CreateMap<EditEmoteSetDto, EmoteSet>().ReverseMap();
        }
    }
}
