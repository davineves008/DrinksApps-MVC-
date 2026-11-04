using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DrinksApps.Controllers.ControllerApi
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest dto)
        {
            var email = dto.Email.Trim();

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email && u.Ativo);

            // Mesma mensagem para e-mail inexistente e senha errada,
            // para não revelar quais e-mails estão cadastrados.
            if (usuario == null || !SenhaConfere(usuario, dto.Senha))
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });

            var (token, expiraEm) = GerarToken(usuario);

            return Ok(new LoginResponse(
                token,
                expiraEm,
                new UsuarioDto(usuario.Id, usuario.Nome, usuario.Email, usuario.Perfil)
            ));
        }

        // GET: api/auth/me  (rota protegida, serve para testar o token)
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                id = User.FindFirstValue(ClaimTypes.NameIdentifier),
                nome = User.FindFirstValue(ClaimTypes.Name),
                email = User.FindFirstValue(ClaimTypes.Email),
                perfil = User.FindFirstValue(ClaimTypes.Role)
            });
        }

        // Hoje a senha está salva em texto puro, igual ao login do site.
        // Quando migrarmos para hash, é só trocar ESTE método.
        private static bool SenhaConfere(Usuario usuario, string senhaDigitada)
        {
            return usuario.Senha == senhaDigitada;
        }

        private (string Token, DateTime ExpiraEm) GerarToken(Usuario usuario)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Perfil)
            };

            var chave = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));

            var credenciais = new SigningCredentials(
                chave, SecurityAlgorithms.HmacSha256);

            var minutos = _config.GetValue("Jwt:ExpireMinutes", 120);
            var expiraEm = DateTime.UtcNow.AddMinutes(minutos);

            var jwt = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: expiraEm,
                signingCredentials: credenciais
            );

            return (new JwtSecurityTokenHandler().WriteToken(jwt), expiraEm);
        }
    }

    public class LoginRequest
    {
        [Required(ErrorMessage = "Informe o e-mail.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Informe a senha.")]
        public string Senha { get; set; } = "";
    }

    // Nunca devolver a senha na resposta.
    public record UsuarioDto(int Id, string Nome, string Email, string Perfil);

    public record LoginResponse(string Token, DateTime ExpiraEm, UsuarioDto Usuario);
}