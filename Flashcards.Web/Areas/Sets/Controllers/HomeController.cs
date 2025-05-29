using AutoMapper;
using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Flashcards.Infrastructure.Services;
using Flashcards.Web.Areas.Sets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Web.Areas.Sets.Controllers
{
    [Authorize]
    [Area("Sets")]
    public class HomeController(DataManager dataManager, IMapper mapper, SetStorage setStorage) : Controller
    {
        private readonly DataManager _dataManager = dataManager;
        private readonly IMapper _mapper = mapper;
        private readonly SetStorage _setStorage = setStorage;

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sets = _mapper.Map<List<SetViewModel>>(await _dataManager.SetRepository.GetAllAsync());
            return View(sets);
        }

        [HttpGet]
        public async Task<IActionResult> SelectedSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = await _dataManager.SetRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(_mapper.Map<SetViewModel>(set));
        }

        [HttpGet]
        public async Task<IActionResult> StudySelectedSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = _mapper.Map<SetViewModel>(await _dataManager.SetRepository.GetAsync(id));

            if (set is null)
                return NotFound();

            _setStorage.Set(_mapper.Map<Set>(set));

            var currentWord = new CurrentWordViewModel()
            {
                Count = set.Words.Count,
                CurrentWord = set.Words[0]
            };

            return View(currentWord);
        }

        [HttpGet]
        public IActionResult SwitchWord(int index = 0)
        {
            var set = _setStorage.Get();
            if (set is not null)
            {
                if (index < 0 || index >= set.Words.Count)
                {
                    return NotFound();
                }
                return Json(set.Words[index]);
            }
            return NotFound();
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

                _setStorage.Modify(word);

                return Json(new { success = true, isFavorite = word.IsFavorite });
            }
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> StudyFavorite(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = _mapper.Map<SetViewModel>(await _dataManager.SetRepository.GetAsync(id));

            if (set is null)
                return NotFound();

            return View("StudySelectedSet", set.Words.Where(w => w.IsFavorite == true).ToList());
        }

        #endregion [ FAVORITE ]

        #region [ ADD/EDIT METHODS ]

        #region [ SETS ]

        [HttpGet]
        public async Task<IActionResult> AddOrEditSet(int id = 0)
        {
            if (id is 0)
            {
                var set = new SetViewModel();
                return PartialView("_AddOrEditSetPartial", set);
            }
            else
            {
                var set = await _dataManager.SetRepository.GetAsync(id);

                if (set is null)
                    return NotFound();

                return PartialView("_AddOrEditSetPartial", _mapper.Map<SetViewModel>(set));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditSet(SetViewModel set)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddOrEditSetPartial", set);

            if (string.Equals(set.OldName, set.Name, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("", "The same name for the set");
                return PartialView("_AddOrEditSetPartial", set);
            }

            if (await _dataManager.SetRepository.IsNotUnique(set.Name, set.Id))
            {
                ModelState.AddModelError("", "Set with this name already exists");
                return PartialView("_AddOrEditSetPartial", set);
            }

            if (set.Id == 0)
            {
                await _dataManager.SetRepository.AddAsync(_mapper.Map<Set>(set));
                TempData["success"] = "Set has been successfully created";
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
            if (setId is 0)
                return BadRequest();

            if (id == 0)
            {
                var wordVM = new WordViewModel { SetId = setId };

                return PartialView("_AddOrEditWordPartial", wordVM);
            }
            else
            {
                var set = await _dataManager.SetRepository.GetAsync(setId);

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
            if (!ModelState.IsValid)
                return PartialView("_AddOrEditWordPartial", word);

            if (await _dataManager.WordRepository.IsNotUnique(word.Name, word.Id))
            {
                ModelState.AddModelError("", "Word with this name already exists");
                return PartialView("_AddOrEditWordPartial", word);
            }

            if (word.Id == 0)
            {
                await _dataManager.WordRepository.AddAsync(_mapper.Map<Word>(word));

                return Json(new { success = true });
            }
            else
            {
                await _dataManager.WordRepository.UpdateAsync(_mapper.Map<Word>(word));

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
            return Json(new { message = "The word has been successfully deleted" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = await _dataManager.SetRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            await _dataManager.SetRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }

        #endregion [ DELETE ACTIONS ]
    }
}