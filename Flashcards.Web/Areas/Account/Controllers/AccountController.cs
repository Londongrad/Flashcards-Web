using Flashcards.Infrastructure.Data;
using Flashcards.Web.Areas.Account.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Flashcards.Web.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager) : Controller
    {
        #region [ Login ]

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Username!, model.Password!, model.RememberMe, false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home", new { area = "Sets" });
                }
                else
                {
                    ModelState.AddModelError("", "Email or password is incorrect.");
                    return View(model);
                }
            }
            return View(model);
        }

        #endregion [ Login ]

        #region [ Register ]

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new()
                {
                    UserName = model.Username,
                    Email = model.Email
                };

                var result = await userManager.CreateAsync(user, model.Password!);

                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
            }
            return View(model);
        }

        #endregion [ Register ]

        #region [ Email ]

        [AllowAnonymous]
        [HttpGet]
        public IActionResult VerifyEmail()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email!);

                if (user is null)
                {
                    ModelState.AddModelError("", "Something is wrong!");
                    return View(model);
                }
                else
                {
                    return RedirectToAction("ChangePassword", new { email = user.Email });
                }
            }
            return View(model);
        }

        #endregion [ Email ]

        #region [ Password ]

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ChangePassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("VerifyEmail");
            }
            return View(new ChangePasswordViewModel() { Email = email });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email!);

                if (user is not null)
                {
                    var result = await userManager.RemovePasswordAsync(user);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddPasswordAsync(user, model.NewPassword!);
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }

                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Email not found");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Something went wrong! Try again.");
                return View(model);
            }

        }

        #endregion [ Password ]

        #region [ UserActions ]
        
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Settings()
        {
            var user = await userManager.GetUserAsync(User);

            return View(new UserViewModel() { Id = user!.Id, ImageURL = user!.ImageURL });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(UserViewModel vm)
        {
            var user = await userManager.GetUserAsync(User);
            user!.ImageURL = vm.ImageURL;
            await userManager.UpdateAsync(user);
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(UserViewModel vm)
        {
            var user = await userManager.GetUserAsync(User);
            Response.Cookies.Delete("myAppAuth");
            await userManager.DeleteAsync(user!);
            //await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        #endregion [ UserActions ]
    }
}