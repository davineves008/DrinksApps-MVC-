
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DrinksApps.Models;
using DrinksApps.Data;

public class ItemPedidoesController : Controller
{
    private readonly AppDbContext _context;

    public ItemPedidoesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: ITEMPEDIDOS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.ItensPedidos.ToListAsync());
    }

    // GET: ITEMPEDIDOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var itempedido = await _context.ItensPedidos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (itempedido == null)
        {
            return NotFound();
        }

        return View(itempedido);
    }

    // GET: ITEMPEDIDOS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: ITEMPEDIDOS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,PedidoId,Pedido,ProdutoId,Produto,Quantidade,PrecoUnitario,Subtotal")] ItemPedido itempedido)
    {
        if (ModelState.IsValid)
        {
            _context.Add(itempedido);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(itempedido);
    }

    // GET: ITEMPEDIDOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var itempedido = await _context.ItensPedidos.FindAsync(id);
        if (itempedido == null)
        {
            return NotFound();
        }
        return View(itempedido);
    }

    // POST: ITEMPEDIDOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,PedidoId,Pedido,ProdutoId,Produto,Quantidade,PrecoUnitario,Subtotal")] ItemPedido itempedido)
    {
        if (id != itempedido.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(itempedido);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ItemPedidoExists(itempedido.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(itempedido);
    }

    // GET: ITEMPEDIDOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var itempedido = await _context.ItensPedidos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (itempedido == null)
        {
            return NotFound();
        }

        return View(itempedido);
    }

    // POST: ITEMPEDIDOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var itempedido = await _context.ItensPedidos.FindAsync(id);
        if (itempedido != null)
        {
            _context.ItensPedidos.Remove(itempedido);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ItemPedidoExists(int? id)
    {
        return _context.ItensPedidos.Any(e => e.Id == id);
    }
}
