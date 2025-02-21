using AutoMapper;
using FlashcardsWEB.Domain.Entities;
using FlashcardsWEB.ViewModels;

namespace FlashcardsWEB.Profiles
{
    public class WordProfile : Profile
    {
        public WordProfile()
        {
            CreateMap<Word, SetViewModel>();
        }
    }
}
