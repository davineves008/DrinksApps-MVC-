using DrinksApps.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrinksApps.Controllers.ControllerApi
{
    [ApiController]
    [Route("api/produtos")]
    public class ProdutosApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosApiController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var produtos = await _context.Produtos
                .Where(p => p.Ativo)
                .Select(p => new
                {
                    p.Id,
                    p.Nome,
                    p.Descricao,
                    p.Preco,
                    p.Estoque,
                    p.ImagemUrl,
                    p.CategoriaId
                })
                .ToListAsync();

            return Ok(produtos);
        }
    }
}