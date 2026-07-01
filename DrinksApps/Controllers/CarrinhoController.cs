using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DrinksApps.Controllers
{
    public class CarrinhoController : Controller
    {
        private readonly AppDbContext _context;

        public CarrinhoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var carrinho = ObterCarrinho();

            return View(carrinho);
        }

        //Adiciona no carrinho
        public async Task<IActionResult> Adicionar(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
                return NotFound();

            var carrinho = ObterCarrinho();

            var item = carrinho.FirstOrDefault(x => x.ProdutoId == id);

            if (item == null)
            {
                carrinho.Add(new CarrinhoItem
                {
                    ProdutoId = produto.Id,
                    Nome = produto.Nome,
                    Preco = produto.Preco,
                    Quantidade = 1,
                    ImagemUrl = produto.ImagemUrl
                });
            }
            else
            {
                item.Quantidade++;
            }
            SalvarCarrinho(carrinho);

            TempData["Sucesso"] = $"{produto.Nome} foi adicionado ao carrinho!";

            return RedirectToAction("Index");
        }

        //remove do carrinho
        public IActionResult Remover(int id)
        {
            var carrinho = ObterCarrinho();

            var item = carrinho.FirstOrDefault(x => x.ProdutoId == id);

            if (item != null)
            {
                carrinho.Remove(item);
            }

            SalvarCarrinho(carrinho);

            return RedirectToAction("Index");
        }

        private List<CarrinhoItem> ObterCarrinho()
        {
            var json = HttpContext.Session.GetString("Carrinho");

            if (string.IsNullOrEmpty(json))
                return new List<CarrinhoItem>();

            return JsonSerializer.Deserialize<List<CarrinhoItem>>(json)!;
        }

        private void SalvarCarrinho(List<CarrinhoItem> carrinho)
        {
            HttpContext.Session.SetString(
                "Carrinho",
                JsonSerializer.Serialize(carrinho));
        }

        // finaliza o pedido
        [HttpPost]
        public async Task<IActionResult> Finalizar()
        {
            var carrinho = ObterCarrinho();

            if (!carrinho.Any())
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index");
            }

            // Pegando o usuário logado pela Session
            int usuarioId = Convert.ToInt32(HttpContext.Session.GetString("UsuarioId"));

            var pedido = new Pedido
            {
                DataPedido = DateTime.Now,
                Status = "Pendente",
                UsuarioId = usuarioId,
                ValorTotal = 0
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            decimal total = 0;
            foreach (var item in carrinho)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);

                if (produto == null)
                    continue;

                if (produto.Estoque < item.Quantidade)
                {
                    TempData["Erro"] = $"Estoque insuficiente para {produto.Nome}.";
                    return RedirectToAction("Index");
                }

                produto.AtualizarEstoque(item.Quantidade);

                decimal subtotal = item.Preco * item.Quantidade;

                var itemPedido = new ItemPedido
                {
                    PedidoId = pedido.Id,
                    ProdutoId = produto.Id,
                    Quantidade = item.Quantidade,
                    PrecoUnitario = item.Preco,
                    Subtotal = subtotal
                };

                total += subtotal;

                _context.ItensPedidos.Add(itemPedido);
            }

            pedido.ValorTotal = total;

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("Details", "Pedidos", new { id = pedido.Id });
        }

        //Limpa o carrinho.
        public IActionResult Limpar()
        {
            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("Index");
        }
        

    }
}