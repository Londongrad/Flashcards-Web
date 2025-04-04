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
                        await userManager.AddPasswordAsync(user, model.NewPassword!);
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
                    ModelState.AddModelError("", "Email is not found");
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

            if (user is null)
                return BadRequest();

            return View(new UserViewModel() { Id = user.Id, ImageURL = user.ImageURL, Username = user.UserName, Email = user.Email });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(UserViewModel vm)
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return BadRequest();

            user.ImageURL = vm.ImageURL;
            await userManager.UpdateAsync(user);
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(UserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);

            if (user is null)
                return BadRequest();
            else
            {
                await signInManager.SignOutAsync();
                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Login");
                }
            }
            return BadRequest();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordInSettings(UserViewModel model)
        {
            if (!(string.IsNullOrEmpty(model.OldPassword) && string.IsNullOrEmpty(model.NewPassword) && string.IsNullOrEmpty(model.Id)))
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    IdentityResult result = await userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Password has been successfully updated";
                        return RedirectToAction("Settings");
                    }
                    TempData["error"] = "An error occured during this process. Try again";
                    return RedirectToAction("Settings");
                }
                else
                    return Unauthorized();
            }
            TempData["error"] = "An error occured during this process";
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(UserViewModel model)
        {
            if (!(string.IsNullOrEmpty(model.Id) && string.IsNullOrEmpty(model.Email)))
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    if (string.Equals(model.Email, user!.Email))
                    {
                        TempData["error"] = "Same email";
                        return RedirectToAction("Settings");
                    }
                    var emailToken = await userManager.GenerateChangeEmailTokenAsync(user, model.Email!);
                    IdentityResult result = await userManager.ChangeEmailAsync(user, model.Email!, emailToken);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Email has been successfully updated";
                        return RedirectToAction("Settings");
                    }
                    TempData["error"] = "An error occured during this process. Most likely user with this email is already exists";
                    return RedirectToAction("Settings");
                }
                else
                    return Unauthorized();
            }
            TempData["error"] = "An error occured during this process";
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUsername(UserViewModel model)
        {
            if (!(string.IsNullOrEmpty(model.Id) && string.IsNullOrEmpty(model.Username)))
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    if (string.Equals(model.Username, user!.UserName))
                    {
                        TempData["error"] = "Same username";
                        return RedirectToAction("Settings");
                    }
                    user.UserName = model.Username;
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["success"] = "Username has been successfully changed";
                        return RedirectToAction("Settings");
                    }
                    TempData["error"] = "An error occured during this process. Most likely user with this username is already exists";
                    return RedirectToAction("Settings");
                }
                else
                    return Unauthorized();
            }
            TempData["error"] = "An error occured during this process";
            return RedirectToAction("Settings");
        }

        #endregion [ UserActions ]
    }
}