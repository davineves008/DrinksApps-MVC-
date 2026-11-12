using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DrinksApps.Controllers.ControllerApi
{
    public class ItemPedidoDto
    {
        public int ProdutoId { get; set; }
        public int Quantidade { get; set; }
    }

    public class CriarPedidoDto
    {
        public List<ItemPedidoDto> Itens { get; set; } = new();

        // Entrega
        public string Rua { get; set; } = "";
        public string Numero { get; set; } = "";
        public string Bairro { get; set; } = "";
        public string Cidade { get; set; } = "";
        public string CEP { get; set; } = "";

        // Pagamento
        public string FormaPagamento { get; set; } = "";
        public string? Observacoes { get; set; }
    }

    [ApiController]
    [Route("api/pedidos")]
    [Authorize]
    public class PedidosApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PedidosApiController(AppDbContext context)
        {
            _context = context;
        }

        private int? ObterUsuarioId()
        {
            var valor = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub");
            return int.TryParse(valor, out var id) ? id : null;
        }

        // GET api/pedidos  -> pedidos do usuário logado
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null) return Unauthorized();

            var pedidos = await _context.Pedidos
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.DataPedido)
                .Select(p => new { p.Id, p.DataPedido, p.ValorTotal, p.Status })
                .ToListAsync();

            return Ok(pedidos);
        }

        // POST api/pedidos  -> cria um pedido a partir do carrinho
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarPedidoDto dto)
        {
            var usuarioId = ObterUsuarioId();
            if (usuarioId == null) return Unauthorized();

            if (dto.Itens == null || dto.Itens.Count == 0)
                return BadRequest("O pedido não tem itens.");

            if (dto.Itens.Any(i => i.Quantidade <= 0))
                return BadRequest("Quantidade inválida.");

            if (string.IsNullOrWhiteSpace(dto.Rua) ||
                string.IsNullOrWhiteSpace(dto.Numero) ||
                string.IsNullOrWhiteSpace(dto.Bairro) ||
                string.IsNullOrWhiteSpace(dto.Cidade) ||
                string.IsNullOrWhiteSpace(dto.CEP))
                return BadRequest("Preencha o endereço de entrega completo.");

            if (string.IsNullOrWhiteSpace(dto.FormaPagamento))
                return BadRequest("Informe a forma de pagamento.");

            var ids = dto.Itens.Select(i => i.ProdutoId).Distinct().ToList();
            var produtos = await _context.Produtos
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            if (produtos.Count != ids.Count)
                return BadRequest("Existe produto inválido no pedido.");

            var pedido = new Pedido
            {
                UsuarioId = usuarioId.Value,
                DataPedido = DateTime.Now,
                Status = "Pendente",
                Rua = dto.Rua.Trim(),
                Numero = dto.Numero.Trim(),
                Bairro = dto.Bairro.Trim(),
                Cidade = dto.Cidade.Trim(),
                CEP = dto.CEP.Trim(),
                FormaPagamento = dto.FormaPagamento.Trim(),
                Observacoes = string.IsNullOrWhiteSpace(dto.Observacoes) ? null : dto.Observacoes.Trim()
            };

            foreach (var item in dto.Itens)
            {
                var produto = produtos[item.ProdutoId];
                pedido.ItensPedidos.Add(new ItemPedido
                {
                    ProdutoId = produto.Id,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = produto.Preco
                });
            }

            pedido.ValorTotal = pedido.ItensPedidos.Sum(i => i.Quantidade * i.PrecoUnitario);

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return Ok(new { pedido.Id, pedido.ValorTotal, pedido.Status });
        }
    }
}