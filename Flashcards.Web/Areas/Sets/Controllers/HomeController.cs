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
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await setRepository.GetAllAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<SetViewModel>(set));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);

            await setRepository.UpdateAsync(new Set(set.Id, set.Name!));
            TempData["success"] = "The set has been successfully edited";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> SelectedSet(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<SetViewModel>(set));
        }

        [HttpGet]
        public async Task<IActionResult> StudySelectedSet(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();            

            return View(mapper.Map<SetViewModel>(set));
        }

        #region [ Add new entity ]

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
            await wordRepository.UpdateAsync(mapper.Map<Word>(word));
            TempData["success"] = "The new word has been successfully added";
            return RedirectToAction("SelectedSet", new { id = word.SetId });
        }

        #endregion [ Add new entity ]

        #region [ Delete ]

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

        #endregion [ Delete ]

        #region [ Check if exists in DB ]

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

        #endregion [ Check if exists in DB ]
    }
}