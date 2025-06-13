using Flashcards.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Flashcards.Infrastructure.Services
{
    public class SetStorage(IHttpContextAccessor httpContextAccessor)
    {
        private static readonly ConcurrentDictionary<string, Set> _setStorage = [];

        // Current user ID extracted from the HTTP context
        private readonly string _userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        /// <summary> Stores a Set object for the current user.
        /// If a set already exists for the user, it is replaced. </summary>
        public void Set(Set set)
        {
            // Remove old entry (if exists) to ensure clean replacement
            _setStorage.Remove(_userId, out _);
            // Try to add the new set to the dictionary
            _setStorage.TryAdd(_userId, set);
        }

        /// <summary> Retrieves the Set object stored for the current user.
        /// Returns null if no Set is found or user ID is invalid. </summary>
        public Set? Get()
        {
            // Ensure the user ID is valid before attempting lookup
            ArgumentException.ThrowIfNullOrWhiteSpace(_userId);

            // Try to retrieve the set for the current user
            return _setStorage.TryGetValue(_userId, out var set) ? set : null;
        }

        /// <summary> Updates a word within the stored Set for the current user.
        /// Does nothing if the word is not found. </summary>
        public void Modify(Word word)
        {
            var set = Get();
            if (set == null)
                return;

            // Find the index of the word with the matching ID
            var index = set.Words.FindIndex(w => w.Id == word.Id);

            // If found, replace it with the new version
            if (index != -1)
            {
                set.Words[index] = word;
            }
        }
    }
}
