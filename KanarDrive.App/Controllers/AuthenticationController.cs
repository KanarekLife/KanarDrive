using System.Threading.Tasks;
using KanarDrive.App.Models.Authentication;
using KanarDrive.Common.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KanarDrive.App.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public AuthenticationController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet("/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model, string returnUrl = "/")
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("ModelStateIsNotValid", "Model state is invalid!");
                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("UserDataIsWrong",
                    "There is no user with such an email and password in our database!");
                return View();
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.Remember, true);
            if (result.Succeeded) return Redirect(returnUrl);

            ModelState.AddModelError("UserDataIsWrong",
                "There is no user with such an email and password in our database!");
            return View();
        }

        [HttpGet("/logout")]
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }
    }
}