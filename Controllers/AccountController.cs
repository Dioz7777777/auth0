using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleMvcApp.ViewModels;
using System.Threading.Tasks;
using SampleMvcApp.Managers;
using SampleMvcApp.Models;

namespace SampleMvcApp.Controllers
{
    public sealed class AccountController(IUserManager userManager) : Controller
    {
        public IActionResult Login() => View();

        [AllowAnonymous]
        public async Task<ActionResult> LoginStepTwo(LoginViewModel model)
        {
            var user = await userManager.GetByName(model.Username);
            if (user == null)
            {
                ViewBag.Error = "User not found";
                return View("Login", model);
            }
            if (!PasswordHelper.VerifyPassword(model.Password, user.PasswordHash))
            {
                ViewBag.Error = "Invalid password";
                return View("Login", model);
            }

            var token = JwtTokenGenerator.GenerateToken(model.Username);
            Response.Headers.Authorization = $"Bearer {token}";

            return Ok("Logged in successfully");
        }

        public IActionResult AccessDenied() => View();

        public IActionResult Registration() => View();

        public async Task<IActionResult> RegisterStepTwo(RegistrationViewModel model)
        {
            var existingUser = await userManager.GetByName(model.Username);
            if (existingUser != null)
            {
                ViewBag.Error = "User already exists";
                return View("Registration");
            }
            var timezone = TimeZoneInfo.Local.Id;
            var user = new User
            {
                Username = model.Username,
                PasswordHash = PasswordHelper.HashPassword(model.Password),
                Timezone = timezone
            };

            await userManager.Create(user);

            return View("Login");
        }
    }
}