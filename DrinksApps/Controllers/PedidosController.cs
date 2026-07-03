
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
        var pedidos = await _context.Pedidos
            .Include(p => p.Usuario)
            .OrderByDescending(p => p.DataPedido)
            .ToListAsync();

        return View(pedidos);
    }


    // GET: PEDIDOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var pedido = await _context.Pedidos
     .Include(p => p.Usuario)
     .Include(p => p.ItensPedidos)
         .ThenInclude(i => i.Produto)
     .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
            return NotFound();

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

        return View(pedido);
    }


    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

    // POST: PEDIDOS/Edit/5
    // POST: Pedido/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Pedido pedido)
    {
        if (id != pedido.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            return View(pedido);
        }

        var pedidoDb = await _context.Pedidos.FirstOrDefaultAsync(p => p.Id == id);

        if (pedidoDb == null)
            return NotFound();

        // 🔥 AQUI é onde você ajusta o valor
        pedidoDb.ValorTotal = decimal.Parse(
            pedido.ValorTotal.ToString().Replace(",", "."),
            CultureInfo.InvariantCulture
        );

        pedidoDb.Status = pedido.Status;

        _context.Update(pedidoDb);
        await _context.SaveChangesAsync();

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
}
