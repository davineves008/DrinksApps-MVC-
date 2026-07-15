
using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

public class PedidosController : Controller
{
    private readonly AppDbContext _context;

    public PedidosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: PEDIDOS
    public async Task<IActionResult> Index()
    {
        var perfil = HttpContext.Session.GetString("Perfil");
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

        if (perfil == "Administrador")
        {
            // Administrador vê todos os pedidos
            var pedidos = await _context.Pedidos
                .Include(p => p.Usuario)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return View(pedidos);
        }

        if (!usuarioId.HasValue)
        {
            return RedirectToAction("Index", "Login");
        }

        // Cliente vê somente os próprios pedidos
        var meusPedidos = await _context.Pedidos
            .Include(p => p.Usuario)
            .Where(p => p.UsuarioId == usuarioId.Value)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();

        return View(meusPedidos);
    }


    // GET: PEDIDOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.ItensPedidos)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();

        if (!UsuarioPodeAcessarPedido(pedido))
            return RedirectToAction(nameof(Index));

        return View(pedido);
    }
    // GET: PEDIDOS/Create
    public IActionResult Create()
    {
        ViewBag.Usuarios = new SelectList(_context.Usuarios, "Id", "Nome");
        return View();
    }

    // POST: PEDIDOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,DataPedido,ValorTotal,Status,UsuarioId,ItensPedidos")] Pedido pedido)
    {
        if (ModelState.IsValid)
        {
            _context.Add(pedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(pedido);
    }
    // GET: Pedido/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();


        // Bloqueia edição após confirmação
        if (pedido.Status != "Pendente")
        {
            TempData["Erro"] =
                "Pedido confirmado não pode mais ser editado.";

            return RedirectToAction(nameof(Index));
        }


        return View(pedido);
    }


    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.


    // POST: Pedido/Edit/5
    
    [HttpPost]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pedido pedido)
    {
        if (id != pedido.Id)
            return NotFound();

        var pedidoDb = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedidoDb == null)
            return NotFound();

        // Verifica usando o pedido do banco
        if (!UsuarioPodeAcessarPedido(pedidoDb))
            return RedirectToAction(nameof(Index));

        // Não permite editar pedidos já confirmados
        if (pedidoDb.Status != "Pendente")
        {
            TempData["Erro"] = "Pedido confirmado não pode mais ser editado.";
            return RedirectToAction(nameof(Index));
        }

        // Atualiza apenas os campos permitidos
        pedidoDb.Status = pedido.Status;
        pedidoDb.ValorTotal = pedido.ValorTotal;

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Pedido atualizado com sucesso!";

        return RedirectToAction(nameof(Index));
    }
    // GET: PEDIDOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (pedido == null)
        {
            return NotFound();
        }

        if (!UsuarioPodeAcessarPedido(pedido))
            return RedirectToAction(nameof(Index));

        return View(pedido);
    }

    // POST: PEDIDOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool PedidoExists(int? id)
    {
        return _context.Pedidos.Any(e => e.Id == id);
    }


    // POST: Pedidos/Confirmar/5
    [HttpPost]
    [ValidateAntiForgeryToken]

   
    public async Task<IActionResult> FinalizarPagamento(int id)
    {
        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();

        if (!UsuarioPodeAcessarPedido(pedido))
            return RedirectToAction(nameof(Index));

        if (pedido.Status == "Pendente")
        {
            pedido.Status = "Confirmado";
            await _context.SaveChangesAsync();
        }

        TempData["Sucesso"] = "Pagamento realizado com sucesso!";

        return RedirectToAction(nameof(Index));
    }

    //metodo pro usuario cancela o pedido;
    [HttpPost]
    [ValidateAntiForgeryToken]


   
    public async Task<IActionResult> Cancelar(int id)
    {
        var pedido = await _context.Pedidos
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();

        if (!UsuarioPodeAcessarPedido(pedido))
            return RedirectToAction(nameof(Index));

        if (pedido.Status != "Pendente")
        {
            TempData["Erro"] = "Este pedido não pode mais ser cancelado.";
            return RedirectToAction(nameof(Index));
        }

        pedido.Status = "Cancelado";

        await _context.SaveChangesAsync();

        TempData["Sucesso"] = "Pedido cancelado com sucesso.";

        return RedirectToAction(nameof(Index));
    }

    //Metodo pra pagamento.
    public async Task<IActionResult> Pagamento(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();

        if (!UsuarioPodeAcessarPedido(pedido))
            return RedirectToAction(nameof(Index));

        return View(pedido);
    }
    //apaga todos os pedidos;
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApagarHistorico()
    {
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

        if (!usuarioId.HasValue)
        {
            return Content("UsuarioId não encontrado na sessão.");
        }




        // Busca os pedidos do usuário
        var pedidos = await _context.Pedidos
      .Where(p => p.UsuarioId == usuarioId.Value)
      .ToListAsync();

        if (pedidos.Any())
        {
            var pedidoIds = pedidos.Select(p => p.Id).ToList();

            // Remove os itens dos pedidos
            var itens = await _context.ItensPedidos
                .Where(i => pedidoIds.Contains(i.PedidoId))
                .ToListAsync();

            _context.ItensPedidos.RemoveRange(itens);

            // Remove os pedidos
            _context.Pedidos.RemoveRange(pedidos);

            await _context.SaveChangesAsync();
        }

        TempData["Sucesso"] = "Seu histórico de pedidos foi apagado.";

        return RedirectToAction(nameof(Index));
    }

    //metodo auxiliar 
    private bool UsuarioPodeAcessarPedido(Pedido pedido)
    {
        var perfil = HttpContext.Session.GetString("Perfil");
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

        if (perfil == "Administrador")
            return true;

        return usuarioId.HasValue && pedido.UsuarioId == usuarioId.Value;
    }
}
