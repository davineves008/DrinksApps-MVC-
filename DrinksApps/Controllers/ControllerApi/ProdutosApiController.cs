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

        // GET: api/produtos
        // GET: api/produtos?categoriaId=2
        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] int? categoriaId)
        {
            var query = _context.Produtos
                .AsNoTracking()
                .Where(p => p.Ativo);

            if (categoriaId.HasValue)
                query = query.Where(p => p.CategoriaId == categoriaId.Value);

            var produtos = await query
                .OrderBy(p => p.Nome)
                .Select(p => new
                {
                    p.Id,
                    p.Nome,
                    p.Descricao,
                    p.Preco,
                    p.Estoque,
                    p.Ativo,
                    p.ImagemUrl,
                    p.CategoriaId
                })
                .ToListAsync();

            // A URL da imagem é montada depois da consulta,
            // porque o EF não consegue traduzir isso para SQL.
            var resultado = produtos.Select(p => new
            {
                p.Id,
                p.Nome,
                Descricao = p.Descricao ?? "",
                p.Preco,
                p.Estoque,
                p.Ativo,
                ImagemUrl = MontarUrlImagem(p.ImagemUrl),
                p.CategoriaId
            });

            return Ok(resultado);
        }

        // GET: api/produtos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var p = await _context.Produtos
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Nome,
                    x.Descricao,
                    x.Preco,
                    x.Estoque,
                    x.Ativo,
                    x.ImagemUrl,
                    x.CategoriaId
                })
                .FirstOrDefaultAsync();

            if (p == null)
                return NotFound();

            return Ok(new
            {
                p.Id,
                p.Nome,
                Descricao = p.Descricao ?? "",
                p.Preco,
                p.Estoque,
                p.Ativo,
                ImagemUrl = MontarUrlImagem(p.ImagemUrl),
                p.CategoriaId
            });
        }

        // No banco a imagem fica como "/images/produtos/arquivo.jpg".
        // O app precisa da URL completa, então juntamos com o endereço do site.
        private string? MontarUrlImagem(string? caminho)
        {
            if (string.IsNullOrWhiteSpace(caminho))
                return null;

            if (caminho.StartsWith("http", StringComparison.OrdinalIgnoreCase))
                return caminho;

            return $"{Request.Scheme}://{Request.Host}{caminho}";
        }
    }
}