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

     
        // GET: Cadastro (form)
        // =========================
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

 
        // POST: Cadastro
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(usuario);
                }

                // valida e-mail duplicado
                if (EmailExiste(usuario.Email))
                {
                    ModelState.AddModelError("Email", "Este e-mail já está cadastrado.");
                    return View(usuario);
                }

                // define padrão de segurança
                usuario.Ativo = true;
                usuario.Perfil ??= "Funcionario";

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Erro inesperado ao criar usuário.");
                return View(usuario);
            }
        }

        
        // LISTAR USUÁRIOS (ADMIN)
        // =========================
        public IActionResult Lista()
        {
            var perfil = HttpContext.Session.GetString("Perfil");

            if (perfil != "Administrador")
                return RedirectToAction("Index", "Home");

            var usuarios = _context.Usuarios.ToList();
            return View(usuarios);
        }


        // DETALHES
        // =========================
        public IActionResult Details(int id)
        {
            var usuario = BuscarPorId(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        
        // EDITAR (ADMIN)
        // =========================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var perfil = HttpContext.Session.GetString("Perfil");

            if (perfil != "Administrador")
                return RedirectToAction("Index", "Home");

            var usuario = BuscarPorId(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(usuario);

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                return RedirectToAction("Lista");
            }
            catch
            {
                ModelState.AddModelError("", "Erro ao atualizar usuário.");
                return View(usuario);
            }
        }

    
        // EXCLUIR (ADMIN)
        // =========================
        [HttpPost]
        public IActionResult Excluir(int id)
        {
            var perfil = HttpContext.Session.GetString("Perfil");

            if (perfil != "Administrador")
                return RedirectToAction("Index", "Home");

            var usuario = BuscarPorId(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
            }

            return RedirectToAction("Lista");
        }

      
        // AUXILIARES (PRIVADOS)
        // =========================
        private bool EmailExiste(string email)
        {
            return _context.Usuarios.Any(u => u.Email == email);
        }

        private Usuario BuscarPorId(int id)
        {
            return _context.Usuarios.FirstOrDefault(u => u.Id == id);
        }
    }
}