using AutoMapper;
using FlashcardsWEB.Domain.Entities;
using FlashcardsWEB.Domain.Repositories.Abstract;
using FlashcardsWEB.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FlashcardsWEB.Controllers
{
    //[Authorize]
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
            TempData["success"] = "New set has been added successfully";
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
            TempData["success"] = "Set has been edited successfully";
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
            TempData["success"] = "Set has been deleted successfully";
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
    }
}