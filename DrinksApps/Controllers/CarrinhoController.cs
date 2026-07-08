using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
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

        //metodo de checkout final;
        public IActionResult Checkout()
        {
            var carrinho = ObterCarrinho();

            if (!carrinho.Any())
                return RedirectToAction("Index");

            var model = new CheckoutView
            {
                Itens = carrinho,
                Total = carrinho.Sum(x => x.Preco * x.Quantidade)
            };

            return View(model);
        }

        // finaliza o pedido
        [HttpPost]
        public async Task<IActionResult> ConfirmarPedido(CheckoutView model)
        {
            // Valida os dados do formulário
            if (!ModelState.IsValid)
            {
                var carrinhoAtual = ObterCarrinho();

                model.Itens = carrinhoAtual;
                model.Total = carrinhoAtual.Sum(x => x.Preco * x.Quantidade);

                return View("Checkout", model);
            }

            // Obtém o carrinho
            var carrinho = ObterCarrinho();

            if (!carrinho.Any())
            {
                TempData["Erro"] = "Seu carrinho está vazio.";
                return RedirectToAction("Index");
            }

            // Verifica se existe usuário logado
            var usuarioSession = HttpContext.Session.GetString("UsuarioId");

            if (string.IsNullOrEmpty(usuarioSession))
            {
                TempData["Erro"] = "Faça login para finalizar o pedido.";
                return RedirectToAction("Index", "Login");
            }

            int usuarioId = Convert.ToInt32(usuarioSession);

            // Calcula o total do pedido
            decimal total = carrinho.Sum(x => x.Preco * x.Quantidade);

            // Cria o pedido
            var pedido = new Pedido
            {
                DataPedido = DateTime.Now,
                UsuarioId = usuarioId,
                Status = "Pendente",
                ValorTotal = total,

                Rua = model.Rua,
                Numero = model.Numero,
                Bairro = model.Bairro,
                Cidade = model.Cidade,
                CEP = model.CEP,

                FormaPagamento = model.FormaPagamento,
                Observacoes = model.Observacoes
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            // Cria os itens do pedido
            foreach (var item in carrinho)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);

                if (produto == null)
                    continue;

                // Verifica estoque
                if (produto.Estoque < item.Quantidade)
                {
                    TempData["Erro"] = $"Estoque insuficiente para {produto.Nome}.";
                    return RedirectToAction("Checkout");
                }

                // Atualiza o estoque
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

                _context.ItensPedidos.Add(itemPedido);
            }

            // Salva alterações
            await _context.SaveChangesAsync();

            // Limpa o carrinho
            HttpContext.Session.Remove("Carrinho");

            TempData["Sucesso"] = "Pedido realizado com sucesso!";

            // Redireciona para view de pagamento

            return RedirectToAction("Pagamento", "Pedidos", new { id = pedido.Id });

        }

        //Limpa o carrinho.
        public IActionResult Limpar()
        {
            HttpContext.Session.Remove("Carrinho");

            return RedirectToAction("Index");
        }

        


    }
}