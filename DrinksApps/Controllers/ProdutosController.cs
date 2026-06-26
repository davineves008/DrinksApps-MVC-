
using DrinksApps.Data;
using DrinksApps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class ProdutosController : Controller
{
    private readonly AppDbContext _context;

    public ProdutosController(AppDbContext context)
    {
        _context = context;
    }

    // GET: PRODUTOS
    public async Task<IActionResult> Index()
    {
        var produtos = await _context.Produtos
                                     .Include(p => p.Categoria)
                                     .ToListAsync();

        return View(produtos);
    }

    // GET: PRODUTOS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (produto == null)
        {
            return NotFound();
        }

        return View(produto);
    }

    // GET: Produtos/Create
    public IActionResult Create()
    {
        ViewBag.Categorias = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "🍔 Lanches" },
        new SelectListItem { Value = "2", Text = "🥤 Bebidas" }
    };

        return View();
    }
    // POST: Produtos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Nome,Descricao,Preco,Estoque,ImagemUrl,Ativo,CategoriaId")] Produto produto)
    {
        if (ModelState.IsValid)
        {
            _context.Add(produto);
            await _context.SaveChangesAsync();

            var categoria = await _context.Categorias.FindAsync(produto.CategoriaId);

            if (categoria?.Nome == "Lanches")
                return RedirectToAction("Lanches");

            if (categoria?.Nome == "Bebidas")
                return RedirectToAction("Bebidas");

            return RedirectToAction(nameof(Index));
        }

        // Recarrega as categorias caso a validação falhe
        ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nome", produto.CategoriaId);

        return View(produto);
    }
    // GET: PRODUTOS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            return NotFound();
        }
        return View(produto);
    }

    // POST: PRODUTOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Nome,Descricao,Preco,Estoque,ImagemUrl,Ativo,CategoriaId")] Produto produto)
    {
        if (id != produto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(produto);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(produto.Id))
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
        return View(produto);
    }

    // GET: PRODUTOS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var produto = await _context.Produtos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (produto == null)
        {
            return NotFound();
        }

        return View(produto);
    }

    // POST: PRODUTOS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto != null)
        {
            _context.Produtos.Remove(produto);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ProdutoExists(int? id)
    {
        return _context.Produtos.Any(e => e.Id == id);
    }
    //Categoria de lanches 
    public IActionResult Lanches()
    {
        var produtos = _context.Produtos
            .Where(p => p.CategoriaId == 1)
            .ToList();

        return View(produtos);
    }
    //categoria de bebidas
    public IActionResult Bebidas()
    {
        var produtos = _context.Produtos
            .Where(p => p.CategoriaId == 2)
            .ToList();

        return View(produtos);
    }
}

