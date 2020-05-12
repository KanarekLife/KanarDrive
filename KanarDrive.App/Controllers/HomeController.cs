using Microsoft.AspNetCore.Mvc;

namespace KanarDrive.App.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        public IActionResult Index()
        {
            return View();
        }
    }
}