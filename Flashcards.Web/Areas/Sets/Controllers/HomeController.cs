using AutoMapper;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Flashcards.Web.Areas.Sets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Web.Areas.Sets.Controllers
{
    [Authorize]
    [Area("Sets")]
    public class HomeController(DataManager dataManager, IMapper mapper) : Controller
    {
        private static SetViewModel _set = new();
        private static readonly CurrentWordViewModel _wordVM = new();

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = new IndexViewModel
            {
                Sets = mapper.Map<List<SetViewModel>>(await dataManager.SetRepository.GetAllAsync()),
            };
            vm.NewSet.UserId = vm.Sets.First().UserId;
            return View(vm);
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

            return View(_wordVM);
        }

        [HttpGet]
        public IActionResult SwitchWord(int index = 0)
        {
            if (_set.Words!.Count != 0)
            {
                _wordVM.Index = index;
                _wordVM.CurrentWord = _set.Words![_wordVM.Index];
            }
            return View("StudySelectedSet", _wordVM);
        }

        #region [ ADD/EDIT METHODS ]

        #region [ SETS ]

        [HttpPost]
        public async Task<IActionResult> AddSet(IndexViewModel vm)
        {
            await dataManager.SetRepository.AddAsync(mapper.Map<Set>(vm.NewSet));

            TempData["success"] = "The new set has been successfully added";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditSet(SetViewModel set)
        {
            await dataManager.SetRepository.UpdateAsync(mapper.Map<Set>(set));

            TempData["success"] = "The set has been successfully edited";
            return RedirectToAction("SelectedSet", new { id = set.Id });
        }

        #endregion [ SETS ]

        #region [ WORDS ]

        [HttpPost]
        public async Task<IActionResult> AddWord(WordViewModel word)
        {
            await dataManager.WordRepository.AddAsync(mapper.Map<Word>(word));

            TempData["success"] = "The new word has been successfully added";
            return RedirectToAction("SelectedSet", new { id = word.SetId });
        }

        [HttpPost]
        public async Task<IActionResult> EditWord(WordViewModel word)
        {
            await dataManager.WordRepository.UpdateAsync(mapper.Map<Word>(word));

            TempData["success"] = "The word has been successfully edited";
            return RedirectToAction("SelectedSet", new { id = word.SetId });
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

        #region [ CHECK IF EXISTS ]

        [HttpPost]
        public async Task<IActionResult> CheckSet(string name, int id)
        {
            return Json(!await IsSetUnique(name, id));
        }

        private async Task<bool> IsSetUnique(string name, int id)
        {
            var sets = await dataManager.SetRepository.GetAllAsync();
            if (id == 0)
            {
                return sets.Any(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                return sets.Any(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase) && s.Id != id);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckWord(string name, int id)
        {
            return Json(await IsWordUnique(name, id));
        }

        private async Task<bool> IsWordUnique(string name, int id)
        {
            var sets = await dataManager.SetRepository.GetAllAsync();
            if (id == 0)
            {
                foreach (var word in sets.SelectMany(s => s.Words))
                {
                    if (string.Equals(word.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                foreach (var word in sets.SelectMany(s => s.Words))
                {
                    if (string.Equals(word.Name, name, StringComparison.OrdinalIgnoreCase) && word.Id != id)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        #endregion [ CHECK IF EXISTS ]

    }
}