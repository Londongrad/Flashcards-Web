using AutoMapper;
using FlashcardsWEB.Domain.Entities;
using FlashcardsWEB.ViewModels;

namespace FlashcardsWEB.Profiles
{
    public class SetProfile : Profile
    {
        public SetProfile()
        {
            CreateMap<Set, SetViewModel>();
        }
    }
}
