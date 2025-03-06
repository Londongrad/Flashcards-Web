using AutoMapper;
using Flashcards.Domain.Entities;
using Flashcards.Web.Areas.Sets.Models;

namespace Flashcards.Web.Profiles
{
    public class WordProfile : Profile
    {
        public WordProfile()
        {
            CreateMap<Word, AddWordViewModel>().ReverseMap();
        }
    }
}
