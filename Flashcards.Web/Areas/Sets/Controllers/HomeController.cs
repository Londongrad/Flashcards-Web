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
        public async Task<IActionResult> Index()
        {
            return View(await setRepository.GetAllAsync());
        }

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

        public async Task<IActionResult> DeleteSet(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            await setRepository.DeleteAsync(id);
            TempData["success"] = "The set has been successfully deleted";
            return RedirectToAction("Index");
        }

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

        public async Task<IActionResult> SelectedSet(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<SetViewModel>(set));
        }

        public IActionResult AddNewWord(int id)
        {
            if (id is 0)
                return NotFound();

            var wordVM = new WordViewModel { SetId = id };

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
    }
}