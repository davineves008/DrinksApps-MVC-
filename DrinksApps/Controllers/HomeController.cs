using Microsoft.AspNetCore.Mvc;

namespace DrinksApps.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Lanches()
        {
            return View();
        }

        public IActionResult Bebidas()
        {
            return View();
        }
    }
}