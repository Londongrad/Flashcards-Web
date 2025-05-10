using AutoMapper;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Flashcards.Web.Areas.Sets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Web.Areas.Sets.Controllers
{
    [Authorize]
    [Area("Sets")]
    public class HomeController(DataManager dataManager, IMapper mapper, UserManager<ApplicationUser> userManager) : Controller
    {
        private static SetViewModel _set = new();

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

            return View(_set.Words);
        }

        [HttpGet]
        public IActionResult SwitchWord(int index = 0)
        {
            if (index < 0 || index >= _set.Words.Count)
            {
                return NotFound();
            }
            return Json(_set.Words[index]);
        }

        #region [ ADD/EDIT METHODS ]

        [HttpGet]
        public async Task<IActionResult> AddOrEditSet(int id = 0)
        {
            if (id is 0)
            {
                var user = await userManager.GetUserAsync(User);

                if (user is null)
                    return BadRequest();

                var set = new SetViewModel { UserId = user.Id };
                return View(set);
            }
            else
            {
                var set = await dataManager.SetRepository.GetAsync(id);

                if (set is null)
                    return NotFound();

                return View(mapper.Map<SetViewModel>(set));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditSet(SetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);

            if (set.Id == 0)
            {
                await dataManager.SetRepository.AddAsync(mapper.Map<Set>(set));

                TempData["success"] = "The new set has been successfully added";
                return RedirectToAction("Index");
            }
            else
            {
                try
                {
                    await dataManager.SetRepository.UpdateAsync(mapper.Map<Set>(set));
                }
                catch (Exception)
                {
                    TempData["error"] = "Set with this name is already exists";
                    return View(set);
                }

                TempData["success"] = "The set has been successfully edited";
                return RedirectToAction("SelectedSet", new { id = set.Id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddOrEditWord(int setId, int id = 0)
        {
            if (setId is 0)
                return BadRequest();

            if (id == 0)
            {
                var set = await dataManager.SetRepository.GetAsync(setId);

                if (set is null)
                    return BadRequest();

                var wordVM = new WordViewModel { SetId = set.Id };

                return View(wordVM);
            }
            else
            {
                var set = await dataManager.SetRepository.GetAsync(setId);

                if (set is null)
                    return NotFound();

                var word = set.Words!.FirstOrDefault(x => x.Id == id);

                if (word is null)
                    return NotFound();

                return View(mapper.Map<WordViewModel>(word));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddOrEditWord(WordViewModel word)
        {
            if (!ModelState.IsValid)
                return View(word);

            if (word.Id == 0)
            {
                await dataManager.WordRepository.AddAsync(mapper.Map<Word>(word));
                TempData["success"] = "The new word has been successfully added";
                return RedirectToAction("SelectedSet", new { id = word.SetId });
            }
            else
            {
                try
                {
                    await dataManager.WordRepository.UpdateAsync(mapper.Map<Word>(word));
                }
                catch (Exception)
                {
                    TempData["error"] = "Word with this name is already exists";
                    return View(word);
                }
                TempData["success"] = "The word has been successfully edited";
                return RedirectToAction("SelectedSet", new { id = word.SetId });
            }
        }

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