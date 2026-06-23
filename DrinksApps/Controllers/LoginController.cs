using DrinksApps.Data;
using Microsoft.AspNetCore.Mvc;

namespace DrinksApps.Controllers
{
    public class LoginController : Controller
    {
        private readonly AppDbContext _context;

        public LoginController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string senha)
        {
            var usuarios = _context.Usuarios
                .FirstOrDefault(x =>
                    x.Email == email &&
                    x.Senha == senha &&
                    x.Ativo);

            if (usuarios != null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "Email ou senha inválidos.";

            return View();
        }

    }
}