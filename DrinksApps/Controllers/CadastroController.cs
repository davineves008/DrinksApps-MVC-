using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrinksApps.Controllers
{
    public class CadastroController : Controller
    {
        private readonly AppDbContext _context;

        public CadastroController(AppDbContext context)
        {
            _context = context;
        }

        // CADASTRO
        // =========================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Usuario usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(usuario);

                usuario.Email = usuario.Email.Trim();

                // Verifica e-mail duplicado
                if (await EmailExiste(usuario.Email))
                {
                    ModelState.AddModelError(
                        "Email",
                        "Este e-mail já está cadastrado.");

                    return View(usuario);
                }

                // Configurações padrão (o cadastro público nunca define o perfil)
                usuario.Ativo = true;
                usuario.Perfil = "Cliente";

                _context.Usuarios.Add(usuario);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
                // Mostra o motivo real no console durante os testes
                Console.WriteLine(ex);

                ModelState.AddModelError(
                    "",
                    "Erro inesperado ao criar usuário.");

                return View(usuario);
            }
        }

        // LISTAR USUÁRIOS (ADMIN)
        // =========================

        public async Task<IActionResult> Lista()
        {
            var perfil = HttpContext.Session.GetString("Perfil");

            if (perfil != "Administrador")
                return RedirectToAction("Index", "Home");

            var usuarios = await _context.Usuarios.ToListAsync();

            return View(usuarios);
        }

        // DETALHES
        // =========================

        public async Task<IActionResult> Details(int id)
        {
            var usuario = await BuscarPorId(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        // EDITAR (ADMIN)
        // =========================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var perfil = HttpContext.Session.GetString("Perfil");

            if (perfil != "Administrador")
                return RedirectToAction("Index", "Home");

            var usuario = await BuscarPorId(id);

            if (usuario == null)
                return NotFound();

            return View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            var perfil = HttpContext.Session.GetString("Perfil");

            if (perfil != "Administrador")
                return RedirectToAction("Index", "Home");

            if (id != usuario.Id)
                return BadRequest();

            // Não exigir uma nova senha na edição
            ModelState.Remove("Senha");
            ModelState.Remove("ConfirmarSenha");

            if (!ModelState.IsValid)
                return View(usuario);

            try
            {
                var usuarioExistente = await BuscarPorId(id);

                if (usuarioExistente == null)
                    return NotFound();

                // Verifica e-mail duplicado em outro usuário
                bool emailDuplicado = await _context.Usuarios.AnyAsync(
                    u => u.Email == usuario.Email && u.Id != id
                );

                if (emailDuplicado)
                {
                    ModelState.AddModelError(
                        "Email",
                        "Este e-mail já está cadastrado.");

                    return View(usuario);
                }

                // Atualiza somente os dados permitidos (a senha não é alterada aqui)
                usuarioExistente.Nome = usuario.Nome;
                usuarioExistente.Email = usuario.Email;
                usuarioExistente.Telefone = usuario.Telefone;
                usuarioExistente.Perfil = usuario.Perfil;
                usuarioExistente.Ativo = usuario.Ativo;

                await _context.SaveChangesAsync();

                return RedirectToAction("Lista");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                ModelState.AddModelError(
                    "",
                    "Erro ao atualizar usuário.");

                return View(usuario);
            }
        }

        // EXCLUIR (ADMIN)
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Excluir(int id)
        {
            var perfil = HttpContext.Session.GetString("Perfil");

            if (perfil != "Administrador")
                return RedirectToAction("Index", "Home");

            var usuario = await BuscarPorId(id);

            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Lista");
        }

        // MÉTODOS AUXILIARES
        // =========================

        private async Task<bool> EmailExiste(string email)
        {
            return await _context.Usuarios.AnyAsync(
                u => u.Email == email
            );
        }

        private async Task<Usuario?> BuscarPorId(int id)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(
                u => u.Id == id
            );
        }
    }
}