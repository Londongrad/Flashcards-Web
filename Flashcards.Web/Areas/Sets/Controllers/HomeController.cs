using AutoMapper;
using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Web.Areas.Sets.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Web.Areas.Sets.Controllers
{
    //[Authorize]
    [Area("Sets")]
    public class HomeController(IRepository<Set> setRepository, IRepository<Word> wordRepository, IMapper mapper) : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await setRepository.GetAllAsync());
        }

        [HttpGet]
        public IActionResult AddNewSet()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSet(AddSetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);

            await setRepository.UpdateAsync(new Set(0, set.Name!));
            TempData["success"] = "The new set has been successfully added";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<AddSetViewModel>(set));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AddSetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);

            await setRepository.UpdateAsync(new Set(set.Id, set.Name!));
            TempData["success"] = "The set has been successfully edited";
            return RedirectToAction("Index");
        }

        [HttpGet]
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
        public async Task<IActionResult> SelectedSet(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await setRepository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<AddSetViewModel>(set));
        }

        [HttpGet]
        public IActionResult AddNewWord(int setId)
        {
            if (setId is 0)
                return BadRequest();

            var wordVM = new AddWordViewModel { SetId = setId };

            return View(wordVM);
        }

        [HttpPost]
        public async Task<IActionResult> AddNewWord(AddWordViewModel word)
        {
            if (!ModelState.IsValid)
                return View(word);
            await wordRepository.UpdateAsync(mapper.Map<Word>(word));
            TempData["success"] = "The new word has been successfully added";
            return RedirectToAction("SelectedSet", new { id = word.SetId });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteWord(int id)
        {
            if (id is 0)
                return BadRequest();

            var word = await wordRepository.GetAsync(id);

            if (word is null)
                return NotFound();

            await wordRepository.DeleteAsync(id);
            TempData["success"] = "The word has been successfully deleted";
            return RedirectToAction("Index");
        }
    }
}