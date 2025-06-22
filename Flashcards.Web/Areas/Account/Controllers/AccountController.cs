using Flashcards.Infrastructure.Data;
using Flashcards.Web.Areas.Account.Models;
using Flashcards.Web.Common;
using Flashcards.Web.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace Flashcards.Web.Areas.Account.Controllers
{
    [Area("Account")]
    [Authorize]
    public class AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, DataManager dataManager) : BaseCotroller
    {
        #region [ Login ]

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login() => View();

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByNameAsync(model.Username!);
            if (user == null || !await userManager.CheckPasswordAsync(user, model.Password!))
            {
                ModelState.AddModelError("", "Email or password is incorrect.");
                return View(model);
            }

            // Генерация нового SessionToken
            user.SessionToken = Guid.NewGuid().ToString();
            await userManager.UpdateAsync(user);

            // Формируем claims вручную
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName!),
                new("SessionToken", user.SessionToken!) // добавляем кастомный токен
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal);

            return RedirectToAction("Index", "Home", new { area = "Sets" });
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
            return View(new PasswordViewModel() { Email = email });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordViewModel model)
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

            return View(new ImageViewModel() { ImageURL = user.ImageURL });
        }

        #region [ Delete account ]

        [HttpGet]
        public async Task<IActionResult> DeleteAccount()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return BadRequest();

            return PartialView("_DeleteAccountPartial", new DeleteAccViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAccount(DeleteAccViewModel vm)
        {
            if (!ModelState.IsValid)
                return PartialView("_DeleteAccountPartial", vm);

            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();

            var isPasswordValid = await userManager.CheckPasswordAsync(user, vm.Password!);

            if (isPasswordValid)
            {
                await signInManager.SignOutAsync();
                await userManager.DeleteAsync(user);
                await dataManager.SetRepository.DeleteAllAsync(user.Id);
                return Json(new { success = true });
            }
            else
            {
                ModelState.AddModelError("", "An error occured during this process. Try again");
                return PartialView("_DeleteAccountPartial", vm);
            }
        }

        [HttpPost]
        public IActionResult ChangeLanguage(string lang)
        {
            if (lang != null)
            {
                var culture = new CultureInfo(lang);
                CultureInfo.CurrentCulture = culture;
                CultureInfo.CurrentUICulture = culture;

                // Добавляем cookie, чтобы запомнить выбранный язык
                Response.Cookies.Append(
                    ".AspNetCore.Culture",
                    $"c={lang}|uic={lang}",
                    new CookieOptions { Expires = DateTime.UtcNow.AddYears(1) });
            }

            return Redirect(Request.Headers.Referer.ToString());
        }

        #endregion [ Delete account ]

        #region [ Change avatar ]

        [HttpGet]
        public async Task<IActionResult> UpdateAvatar()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return BadRequest();

            var avatarVM = new ImageViewModel()
            {
                ImageURL = user.ImageURL
            };

            return PartialView("_ChangeAvatarPartial", avatarVM);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAvatar(ImageViewModel vm)
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return BadRequest();

            if (string.Equals(vm.ImageURL, user.ImageURL))
            {
                ModelState.AddModelError("", "The same image URL");
                return PartialView("_ChangeAvatarPartial");
            }

            user.ImageURL = vm.ImageURL;
            IdentityResult result = await userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["success"] = "User avatar has been successfully updated";
                return Json(new { success = true });
            }
            ModelState.AddModelError("", "An error occured during this process.");
            return Json(new { success = false });
        }

        #endregion [ Avatar ]

        #region [ Change password in settings ]

        [HttpGet]
        public IActionResult ChangePasswordInSettings()
        {
            return PartialView("_ChangePasswordInSettingsPartial", new PasswordInSettingsViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordInSettings(PasswordInSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangePasswordInSettingsPartial");

            var user = await userManager.GetUserAsync(User);

            if (user != null)
            {
                IdentityResult result = await userManager.ChangePasswordAsync(user, model.OldPassword!, model.NewPassword!);
                if (result.Succeeded)
                {
                    TempData["success"] = "Password has been successfully changed";
                    return Json(new { success = true });
                }
                TempData["error"] = "An error occured during this process. Try again";
                return Json(new { success = false });
            }
            else
                return Unauthorized();
        }

        #endregion [ Change password in settings ]

        #region [ Change email ]

        [HttpGet]
        public async Task<IActionResult> ChangeEmail()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return BadRequest();

            return PartialView("_ChangeEmailPartial", new EmailViewModel() { Email = user.Email });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(EmailViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangeEmailPartial");

            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return Unauthorized();

            if (string.Equals(model.Email, user.Email))
            {
                ModelState.AddModelError("", "The same email");
                return PartialView("_ChangeEmailPartial");
            }
            var emailToken = await userManager.GenerateChangeEmailTokenAsync(user, model.Email!);
            IdentityResult result = await userManager.ChangeEmailAsync(user, model.Email!, emailToken);
            if (result.Succeeded)
            {
                TempData["success"] = "Email has been successfully updated";
                return Json(new { success = true });
            }
            ModelState.AddModelError("", "An error occured during this process. It's either your input is incorrect or user with this email already exists.");
            return PartialView("_ChangeEmailPartial", model);

        }

        #endregion [ Change email ]

        #region [ Change username ] 

        [HttpGet]
        public async Task<IActionResult> ChangeUsername()
        {
            var user = await userManager.GetUserAsync(User);

            if (user is null)
                return BadRequest();

            return PartialView("_ChangeUsernamePartial", new UsernameViewModel() { Username = user.UserName });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUsername(UsernameViewModel model)
        {
            if (!ModelState.IsValid)
                return PartialView("_ChangeUsernamePartial");

            var user = await userManager.GetUserAsync(User);
            if (user != null)
            {
                if (string.Equals(model.Username, user!.UserName))
                {
                    ModelState.AddModelError("", "The same username");
                    return PartialView("_ChangeUsernamePartial");
                }
                user.UserName = model.Username;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["success"] = "Username has been successfully changed";
                    return Json(new { success = true });
                }
                ModelState.AddModelError("", "An error occured during this process. Most likely user with this username is already exists");
                return Json(new { success = false });
            }
            else
                return Unauthorized();
        }

        #endregion [ Change username ] 

        #endregion [ UserActions ]

        #region [ IsAuthenticated ]

        [AllowAnonymous]
        [HttpGet]
        public IActionResult IsAuthenticated()
        {
            return User.Identity?.IsAuthenticated == true ? Ok() : Unauthorized();
        }

        #endregion [ IsAuthenticated ]
    }
}