using Microsoft.AspNetCore.Mvc;

namespace AeroVista.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
