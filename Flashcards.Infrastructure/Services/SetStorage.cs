using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Flashcards.Infrastructure.Services
{
    public class SetStorage(IHttpContextAccessor httpContextAccessor)
    {
        private static readonly Dictionary<string, Set> _setStorage = [];
        private readonly string _userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        public bool Set(Set set)
        {
            return _setStorage.TryAdd(_userId, set);
        }

        public Set? Get()
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(_userId);
            if (!_setStorage.TryGetValue(_userId, out Set? set))
            {
                return null;
            }
            return set;
        }

        public void Modify(Word word)
        {
            var set = Get();
            if (set != null)
            {
                var index = set.Words.FindIndex(w => w.Id == word.Id);
                if (index != -1) 
                { 
                    set.Words[index] = word;
                }
            }
        }
    }
}
