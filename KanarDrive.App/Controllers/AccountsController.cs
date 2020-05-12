using KanarDrive.Common.Entities.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KanarDrive.App.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {
        private UserManager<User> _userManager;

        public AccountsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
    }
}