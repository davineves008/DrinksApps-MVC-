
using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrinksApps.Controllers
{
    public class CadastroController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public CadastroController(
            AppDbContext context,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
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

                // Verifica e-mail duplicado
                if (await EmailExiste(usuario.Email))
                {
                    ModelState.AddModelError(
                        "Email",
                        "Este e-mail já está cadastrado.");

                    return View(usuario);
                }

                // Configurações padrão
                usuario.Ativo = true;
                usuario.Perfil = "Cliente";

                // Gera o hash da senha
                usuario.SenhaHash = _passwordHasher.HashPassword(
                    usuario,
                    usuario.Senha
                );

                // Salva somente o hash
                _context.Usuarios.Add(usuario);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Login");
            }
            catch (Exception)
            {
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

                // Atualiza somente os dados permitidos
                usuarioExistente.Nome = usuario.Nome;
                usuarioExistente.Email = usuario.Email;
                usuarioExistente.Telefone = usuario.Telefone;
                usuarioExistente.Perfil = usuario.Perfil;
                usuarioExistente.Ativo = usuario.Ativo;

                // Preserva a senha hash existente
                await _context.SaveChangesAsync();

                return RedirectToAction("Lista");
            }
            catch
            {
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