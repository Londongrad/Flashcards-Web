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
    public class HomeController(IRepository<Set> repository, IMapper mapper) : Controller
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
        public async Task<IActionResult> AddNewSet(SetViewModel set)
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

            return View(mapper.Map<SetViewModel>(set));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SetViewModel set)
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

        public async Task<IActionResult> CheckSet(string name)
        {
            var sets = await repository.GetAllAsync();

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

            var set = await repository.GetAsync(id);

            if (set is null)
                return NotFound();

            return View(mapper.Map<SetViewModel>(set));
        }
    }
}