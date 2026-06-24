using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Mvc;

namespace DrinksApps.Controllers
{
    public class CadastroController : Controller
    {
        private readonly AppDbContext _context;

        public CadastroController(AppDbContext context)
        {
            _context = context;
        }

        // Abre a tela de cadastro
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Salva o usuário
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Usuario usuario)
        {
            if (EmailExiste(usuario.Email))
            {
                ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                _context.Usuarios.Add(usuario);
                _context.SaveChanges();

                TempData["Sucesso"] = "Usuário cadastrado com sucesso!";

                return RedirectToAction("Index", "Login");
            }

            return View(usuario);
        }

        // Verifica se email já existe
        public bool EmailExiste(string email)
        {
            return _context.Usuarios.Any(u => u.Email == email);
        }

        // Remove um usuário
        [HttpPost]
        public IActionResult Excluir(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // Busca usuário pelo ID
        public Usuario BuscarPorId(int id)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Id == id);
        }
    }
}