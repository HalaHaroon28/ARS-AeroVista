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
                SkyMiles = user.SkyMiles,
                NewEmail = user.Email ?? "",
                Address = user.Address ?? "",
                PhoneNumber = user.PhoneNumber ?? "",
                PreferredCardNumber = user.PreferredCardNumber ?? ""
            };

            return View("UserDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Check password before allowing any changes
            var passwordCheck = await _userManager.CheckPasswordAsync(user, model.ConfirmPassword);
            if (!passwordCheck)
            {
                ModelState.AddModelError("ConfirmPassword", "Incorrect password. Authorization failed.");
                return await ResetViewModelAndReturn(user, model);
            }

            // Update Email if changed
            if (user.Email != model.NewEmail)
            {
                var existingUser = await _userManager.FindByEmailAsync(model.NewEmail);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("NewEmail", "This email is already taken.");
                    return await ResetViewModelAndReturn(user, model);
                }
                user.Email = model.NewEmail;
                user.UserName = model.NewEmail;
            }

            // Update mutable fields
            user.Address = model.Address;
            user.PhoneNumber = model.PhoneNumber;
            user.PreferredCardNumber = model.PreferredCardNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction(nameof(UserDetails));
            }

            foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
            return await ResetViewModelAndReturn(user, model);
        }

        private async Task<IActionResult> ResetViewModelAndReturn(ApplicationUser user, ProfileViewModel model)
        {
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Sex = user.Sex;
            model.DateOfBirth = user.DateOfBirth;
            model.SkyMiles = user.SkyMiles;
            return View("UserDetails", model);
        }
    }
}