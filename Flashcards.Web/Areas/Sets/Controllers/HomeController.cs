using AutoMapper;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Flashcards.Web.Areas.Sets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Speech.Synthesis;

namespace Flashcards.Web.Areas.Sets.Controllers
{
    [Authorize]
    [Area("Sets")]
    public class HomeController : Controller
    {
#pragma warning disable CA1416 // Проверка совместимости платформы
        private static SetViewModel _set = new();
        private static readonly CurrentWordViewModel _wordVM = new();
        private readonly SpeechSynthesizer speechSynthesizer;
        private readonly DataManager dataManager;
        private readonly IMapper mapper;

        public HomeController(DataManager dataManager, IMapper mapper)
        {
            this.dataManager = dataManager;
            this.mapper = mapper;
            speechSynthesizer = new SpeechSynthesizer();
            try { speechSynthesizer.SelectVoice("Microsoft Hazel Desktop"); }
            catch (Exception) { }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var sets = mapper.Map<List<SetViewModel>>(await dataManager.SetRepository.GetAllAsync());
            return View(sets);
        }

        [HttpGet]
        public async Task<IActionResult> SelectedSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = await dataManager.SetRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<SetViewModel>(set));
        }

        [HttpGet]
        public async Task<IActionResult> StudySelectedSet(int id)
        {
            if (id is 0)
                return BadRequest();

            _set = mapper.Map<SetViewModel>(await dataManager.SetRepository.GetAsync(id));

            if (_set is null)
                return NotFound();

            _wordVM.Index = 0;
            _wordVM.Count = _set.Words!.Count;
            _wordVM.SetId = id;
            _wordVM.CurrentWord = _set.Words[0];
            await Speak(_wordVM.CurrentWord.Name);
            return View(_wordVM);
        }

        [HttpGet]
        public async Task<IActionResult> SwitchWord(int index = 0)
        {
            if (_set.Words!.Count != 0)
            {
                _wordVM.Index = index;
                _wordVM.CurrentWord = _set.Words![_wordVM.Index];
                await Speak(_wordVM.CurrentWord.Name);
            }

            return View("StudySelectedSet", _wordVM);
        }

        #region [ FAVORITE ]

        [HttpPost]
        public async Task<IActionResult> Favorite(int id)
        {
            var word = await dataManager.WordRepository.GetAsync(id);
            if (word is not null)
            {
                if (word.IsFavorite)
                    word.IsFavorite = false;
                else
                    word.IsFavorite = true;
                await dataManager.WordRepository.UpdateAsync(word);
                return Json(new { success = true, isFavorite = word.IsFavorite });
            }
            //return View("StudySelectedSet", _wordVM);
            return Json(new { success = false });
        }

        [HttpGet]
        public async Task<IActionResult> StudyFavorite(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = mapper.Map<SetViewModel>(await dataManager.SetRepository.GetAsync(id));

            if (_set is null)
                return NotFound();

            _set.Words = set.Words.Where(w => w.IsFavorite == true).ToList();

            _wordVM.Index = 0;
            _wordVM.Count = _set.Words!.Count;
            _wordVM.SetId = id;
            _wordVM.CurrentWord = _set.Words[0];
            await Speak(_wordVM.CurrentWord.Name);

            return View("StudySelectedSet", _wordVM);
        }

        #endregion [ FAVORITE ]

        #region [ TTS Method ]

        private async Task Speak(string name) => await Task.Run(() =>
        {
            speechSynthesizer.SpeakAsyncCancelAll();
            speechSynthesizer.SpeakAsync(name);
        });

        #endregion [ TTS Method ]

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
                var set = await dataManager.SetRepository.GetAsync(id);

                if (set is null)
                    return NotFound();

                return PartialView("_AddOrEditSetPartial", mapper.Map<SetViewModel>(set));
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

            if (await dataManager.SetRepository.IsNotUnique(set.Name, set.Id))
            {
                ModelState.AddModelError("", "Set with this name already exists");
                return PartialView("_AddOrEditSetPartial", set);
            }

            if (set.Id == 0)
            {
                await dataManager.SetRepository.AddAsync(mapper.Map<Set>(set));

                TempData["success"] = "The new set has been successfully added";
                //return RedirectToAction("Index");
                return Json(new { success = true });
            }
            else
            {
                await dataManager.SetRepository.UpdateAsync(mapper.Map<Set>(set));

                TempData["success"] = "The set has been successfully edited";
                //return RedirectToAction("SelectedSet", new { id = set.Id });
                return Json(new { success = true });
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
                var set = await dataManager.SetRepository.GetAsync(setId);

                if (set is null)
                    return NotFound();

                var word = set.Words!.FirstOrDefault(x => x.Id == id);

                if (word is null)
                    return NotFound();

                return PartialView("_AddOrEditWordPartial", mapper.Map<WordViewModel>(word));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditWord(WordViewModel word)
        {
            if (!ModelState.IsValid)
                return PartialView("_AddOrEditWordPartial", word);

            if (await dataManager.WordRepository.IsNotUnique(word.Name, word.Id))
            {
                ModelState.AddModelError("", "Word with this name already exists");
                return PartialView("_AddOrEditWordPartial", word);
            }

            if (word.Id == 0)
            {
                await dataManager.WordRepository.AddAsync(mapper.Map<Word>(word));

                TempData["success"] = "The new word has been successfully added";
                //return RedirectToAction("SelectedSet", new { id = word.SetId });
                return Json(new { success = true });
            }
            else
            {
                await dataManager.WordRepository.UpdateAsync(mapper.Map<Word>(word));

                TempData["success"] = "The word has been successfully edited";
                //return RedirectToAction("SelectedSet", new { id = word.SetId });
                return Json(new { success = true });
            }
        }

        #endregion [ WORDS ]

        #endregion [ ADD/EDIT METHODS ]

        #region [ DELETE ACTIONS ]

        [HttpPost]
        public async Task<IActionResult> DeleteWord(int wordId, int setId)
        {
            if (setId is 0 || wordId is 0)
                return BadRequest();

            await dataManager.WordRepository.DeleteAsync(wordId);
            TempData["success"] = "The word has been successfully deleted";
            return RedirectToAction("SelectedSet", new { id = setId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSet(int id)
        {
            if (id is 0)
                return BadRequest();

            var set = await dataManager.SetRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            await dataManager.SetRepository.DeleteAsync(id);
            TempData["success"] = "The set has been successfully deleted";
            return RedirectToAction("Index");
        }

        #endregion [ DELETE ACTIONS ]
    }
#pragma warning restore CA1416 // Проверка совместимости платформы
}