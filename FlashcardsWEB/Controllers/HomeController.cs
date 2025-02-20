using Flashcards.Domain.Entities;
using FlashcardsWEB.Domain.Repositories.Abstract;
using FlashcardsWEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsWEB.Controllers
{
    //[Authorize]
    public class HomeController(IRepository<Set> repository) : Controller
    {
        public async Task<IActionResult> Index()
        {
            return View(await repository.GetAllAsync());
        }

        public IActionResult AddNewSet()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNewSet(NewSetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);

            await repository.UpdateAsync(new Set(0, set.Name!));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await repository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(set);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(NewSetViewModel set)
        {
            if (!ModelState.IsValid)
                return View(set);
                
            await repository.UpdateAsync(new Set(set.Id, set.Name!));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id is 0)
                return NotFound();

            var set = await repository.GetAsync(id);

            if (set is null)
                return NotFound();

            await repository.DeleteAsync(id);

            return RedirectToAction("Index");
        }
    }
}