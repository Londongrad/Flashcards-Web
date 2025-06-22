using AutoMapper;
using Flashcards.Domain.Entities;
using Flashcards.Web.Areas.Sets.Models;
using Flashcards.Web.Common;
using Flashcards.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Text.Json;

namespace Flashcards.Web.Areas.Sets.Controllers
{
    [Authorize]
    [Area("Sets")]
    public class HomeController(DataManager dataManager, IMapper mapper, IStringLocalizer<SharedResource> localizer) : BaseCotroller
    {
        private readonly DataManager _dataManager = dataManager;
        private readonly IMapper _mapper = mapper;
        private readonly IStringLocalizer<SharedResource> _localizer = localizer;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            var sets = _mapper.Map<List<SetSummaryViewModel>>(await _dataManager.SetRepository.GetAllSummariesAsync(userId));
            return View(sets);
        }

        [HttpGet]
        public async Task<IActionResult> SelectedSet(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (id is 0)
                return BadRequest();

            var set = await _dataManager.SetRepository.GetAsync(id, userId);

            if (set is null)
                return NotFound();

            return View(_mapper.Map<SetViewModel>(set));
        }

        [HttpGet]
        public async Task<IActionResult> StudySelectedSet(int id, bool flag)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (id is 0)
                return BadRequest();

            var set = _mapper.Map<SetViewModel>(await _dataManager.SetRepository.GetAsync(id, userId));

            if (set is null)
                return NotFound();

            if (flag)
                set.Words = set.Words.Where(w => w.IsFavorite).ToList();

            var studyVM = new StudySetViewModel()
            {
                Count = set.Words.Count,
                FirstWord = set.Words[0],
                WordsJson = JsonSerializer.Serialize(set.Words)
            };

            return View(studyVM);
        }

        #region [ FAVORITE ]

        [HttpPost]
        public async Task<IActionResult> Favorite(int id)
        {
            var word = await _dataManager.WordRepository.GetAsync(id);
            if (word is not null)
            {
                if (word.IsFavorite)
                    word.IsFavorite = false;
                else
                    word.IsFavorite = true;

                await _dataManager.WordRepository.UpdateAsync(word);

                return Json(new { success = true, isFavorite = word.IsFavorite });
            }
            return Json(new { success = false });
        }

        #endregion [ FAVORITE ]

        #region [ ADD/EDIT METHODS ]

        #region [ SETS ]

        [HttpGet]
        public async Task<IActionResult> AddOrEditSet(int id = 0)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (id is 0)
            {
                var set = new SetViewModel();
                return PartialView("_AddOrEditSetPartial", set);
            }
            else
            {
                var set = await _dataManager.SetRepository.GetAsync(id, userId);

                if (set is null)
                    return NotFound();

                return PartialView("_AddOrEditSetPartial", _mapper.Map<SetViewModel>(set));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditSet(SetViewModel set)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return PartialView("_AddOrEditSetPartial", set);

            if (string.Equals(set.OldName, set.Name, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", _localizer["SameSet"]);
                return PartialView("_AddOrEditSetPartial", set);
            }

            if (await _dataManager.SetRepository.IsNotUnique(set.Name, set.Id, userId))
            {
                ModelState.AddModelError("", _localizer["SetExists"]);
                return PartialView("_AddOrEditSetPartial", set);
            }

            if (set.Id == 0)
            {
                set.UserId = userId;
                await _dataManager.SetRepository.AddAsync(_mapper.Map<Set>(set));
                TempData["success"] = _localizer["ToastrNewSetAdded"].Value;
                return Json(new { success = true, isNew = true });
            }
            else
            {
                await _dataManager.SetRepository.UpdateAsync(_mapper.Map<Set>(set));
                return Json(new { success = true, name = set.Name, isNew = false });
            }
        }

        #endregion [ SETS ]

        #region [ WORDS ]

        [HttpGet]
        public async Task<IActionResult> AddOrEditWord(int setId, int id = 0)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (setId is 0)
                return BadRequest();

            if (id == 0)
            {
                var wordVM = new WordViewModel { SetId = setId };

                return PartialView("_AddOrEditWordPartial", wordVM);
            }
            else
            {
                var set = await _dataManager.SetRepository.GetAsync(setId, userId);

                if (set is null)
                    return NotFound();

                var word = set.Words!.FirstOrDefault(x => x.Id == id);

                if (word is null)
                    return NotFound();

                return PartialView("_AddOrEditWordPartial", _mapper.Map<WordViewModel>(word));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditWord(WordViewModel word)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return PartialView("_AddOrEditWordPartial", word);

            if (await _dataManager.WordRepository.IsNotUnique(word.Name, word.Id, userId))
            {
                ModelState.AddModelError("", _localizer["WordExists"]);
                return PartialView("_AddOrEditWordPartial", word);
            }

            if (word.Id == 0)
            {
                await _dataManager.WordRepository.AddAsync(_mapper.Map<Word>(word));
                TempData["success"] = _localizer["ToastrNewWordAdded"].Value;
                return Json(new { success = true });
            }
            else
            {
                await _dataManager.WordRepository.UpdateAsync(_mapper.Map<Word>(word));
                TempData["success"] = _localizer["ToastrWordUpdated"].Value;
                return Json(new { success = true });
            }
        }

        #endregion [ WORDS ]

        #endregion [ ADD/EDIT METHODS ]

        #region [ DELETE ACTIONS ]

        [HttpPost]
        public async Task<IActionResult> DeleteWord(int id)
        {
            if (id is 0)
                return BadRequest();

            await _dataManager.WordRepository.DeleteAsync(id);
            return Json(new { message = _localizer["ToastrDeleteWordSuccess"] });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSet(int id)
        {
            var userId = GetUserId();
            if (userId == null) return Unauthorized();

            if (id is 0)
                return BadRequest();

            var set = await _dataManager.SetRepository.GetAsync(id, userId);

            if (set is null)
                return NotFound();

            await _dataManager.SetRepository.DeleteAsync(id, userId);

            TempData["success"] = _localizer["ToastrDeleteSetSuccess"].Value;

            return Json(new { success = true });
        }

        #endregion [ DELETE ACTIONS ]
    }
}