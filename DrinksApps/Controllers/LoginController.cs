using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Http;
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
            TempData.Clear();
            return View();
        }

        [HttpPost]
        public IActionResult Index(string email, string senha)
        {
            var usuario = _context.Usuarios
                .FirstOrDefault(x =>
                    x.Email == email &&
                    x.Senha == senha &&
                    x.Ativo);

            if (usuario != null)
            {
                // Salva informações do usuário na sessão
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("Nome", usuario.Nome);
                HttpContext.Session.SetString("Perfil", usuario.Perfil);

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Erro = "E-mail ou senha inválidos.";

            return View();
        }

        //metodo de sair e limpar a sessão.
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Login");
        }

    }
}