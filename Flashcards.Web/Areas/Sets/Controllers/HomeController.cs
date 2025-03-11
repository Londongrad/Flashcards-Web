using AutoMapper;
using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Web.Areas.Sets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Web.Areas.Sets.Controllers
{
    [Authorize]
    [Area("Sets")]
    public class HomeController(IRepository<Set> setRepository, IRepository<Word> wordRepository, IMapper mapper) : Controller
    {
        private static SetViewModel _set = new();
        private static CurrentWordViewModel _wordVM = new();
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await setRepository.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> SelectedSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<SetViewModel>(set));
        }

        [HttpGet]
        public async Task<IActionResult> StudySelectedSet(int id)
        {
            if (id is 0)
                return BadRequest();

            _set = mapper.Map<SetViewModel>(await setRepository.GetAsync(id));

            if (_set is null)
                return NotFound();

            if (_set.Words!.Count != 0)
            {
                _wordVM.Index = 0;
                _wordVM.Count = _set.Words!.Count;
                _wordVM.SetId = id;
                _wordVM.CurrentWord = _set.Words[0];
            }
            return View(_wordVM);
        }

        [HttpGet]
        public IActionResult SwitchWord(int index = 0)
        {
            _wordVM.Index = index;
            _wordVM.CurrentWord = _set.Words![_wordVM.Index];
            return View("StudySelectedSet", _wordVM);
        }

        #region [ EDIT METHODS ]

        [HttpGet]
        public async Task<IActionResult> EditSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<SetViewModel>(set));
        }

        [HttpPost]
        public async Task<IActionResult> EditSet(SetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);

            await setRepository.UpdateAsync(new Set(set.Id, set.Name!));
            TempData["success"] = "The set has been successfully edited";
            return RedirectToAction("SelectedSet", new { id = set.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditWord(int id)
        {
            if (id is 0)
                return BadRequest();

            var word = await wordRepository.GetAsync(id);

            if (word is null)
                return NotFound();

            return View(mapper.Map<WordViewModel>(word));
        }

        [HttpPost]
        public async Task<IActionResult> EditWord(WordViewModel word)
        {
            if (!ModelState.IsValid)
                return View(word);

            await wordRepository.UpdateAsync(mapper.Map<Word>(word));
            TempData["success"] = "The set has been successfully edited";
            return RedirectToAction("SelectedSet", new { id = word.SetId });
        }

        #endregion [ EDIT METHODS ]

        #region [ ADD NEW ENTITY ]

        [HttpGet]
        public IActionResult AddNewSet()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSet(SetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);

            await setRepository.UpdateAsync(new Set(0, set.Name!));
            TempData["success"] = "The new set has been successfully added";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult AddNewWord(int setId)
        {
            if (setId is 0)
                return BadRequest();

            var wordVM = new WordViewModel { SetId = setId };

            return View(wordVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewWord(WordViewModel word)
        {
            if (!ModelState.IsValid)
                return View(word);
            word.ImagePath ??= "";
            await wordRepository.UpdateAsync(mapper.Map<Word>(word));
            TempData["success"] = "The new word has been successfully added";
            return RedirectToAction("SelectedSet", new { id = word.SetId });
        }

        #endregion [ ADD NEW ENTITY ]

        #region [ DELETE ACTIONS ]

        [HttpPost]
        public async Task<IActionResult> DeleteWord(int wordId, int setId)
        {
            if (setId is 0 || wordId is 0)
                return BadRequest();

            await wordRepository.DeleteAsync(wordId);
            TempData["success"] = "The word has been successfully deleted";
            return RedirectToAction("SelectedSet", new { id = setId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            await setRepository.DeleteAsync(id);
            TempData["success"] = "The set has been successfully deleted";
            return RedirectToAction("Index");
        }

        #endregion [ DELETE ACTIONS ]

        #region [ CHECK IF EXISTS IN DB ]

        [HttpGet]
        public async Task<IActionResult> CheckSet(string name)
        {
            var sets = await setRepository.GetAllAsync();

            foreach (var set in sets)
            {
                if (set.Name == name)
                    return Json(false);
            }
            return Json(true);
        }

        [HttpGet]
        public async Task<IActionResult> CheckWord(string name)
        {
            var words = await wordRepository.GetAllAsync();

            foreach (var word in words)
            {
                if (word.Name == name)
                    return Json(false);
            }
            return Json(true);
        }

        #endregion [ CHECK IF EXISTS IN DB ]
    }
}