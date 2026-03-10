using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using AeroVista.Models;
using Microsoft.AspNetCore.Authorization;

namespace AeroVista.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> UserDetails()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Sex = user.Sex,
                DateOfBirth = user.DateOfBirth,
                NewEmail = user.Email ?? ""
            };

            return View("UserDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateEmail(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            if (!ModelState.IsValid)
            {
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Sex = user.Sex;
                model.DateOfBirth = user.DateOfBirth;
                return View("UserDetails", model);
            }

            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.ConfirmPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError("ConfirmPassword", "Incorrect password. Changes not saved.");
                model.FirstName = user.FirstName;
                model.LastName = user.LastName;
                model.Sex = user.Sex;
                model.DateOfBirth = user.DateOfBirth;
                return View("UserDetails", model);
            }

            if (user.Email != model.NewEmail)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("NewEmail", "This email is already taken by another aviator.");
                    model.FirstName = user.FirstName;
                    return View("UserDetails", model);
                }

                user.Email = model.NewEmail;
                user.UserName = model.NewEmail;
                user.NormalizedEmail = model.NewEmail.ToUpper();
                user.NormalizedUserName = model.NewEmail.ToUpper();

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    model.FirstName = user.FirstName;
                    return View("UserDetails", model);
                }

                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Email updated successfully!";
            }

            return RedirectToAction(nameof(UserDetails));
        }
    }
}