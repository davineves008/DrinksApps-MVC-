using DrinksApps.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrinksApps.Controllers
{
    public class CadastroController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //salva no banco
        [HttpPost]
        public IActionResult Index(Usuario usuarios)
        {
            // salvar no banco aqui
            return RedirectToAction("Index", "Login");
        }
    }
}