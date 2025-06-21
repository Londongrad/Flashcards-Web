using AutoMapper;
using Flashcards.Application.DTOs;
using Flashcards.Domain.Entities;
using Flashcards.Web.Areas.Sets.Models;

namespace Flashcards.Web.Profiles
{
    public class SetProfile : Profile
    {
        public SetProfile()
        {
            CreateMap<Set, SetViewModel>()
                .ForMember(w => w.OldName, opt => opt.MapFrom(src => src.Name));

            CreateMap<SetViewModel, Set>();

            CreateMap<SetSummaryViewModel, SetDTO>().ReverseMap();
        }
    }
}