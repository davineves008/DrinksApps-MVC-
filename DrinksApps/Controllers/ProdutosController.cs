using Microsoft.AspNetCore.Http;
using System.IO;
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
    public IActionResult Create()
    {
        ViewBag.Categorias = new SelectList(
            _context.Categorias,
            "Id",
            "Nome");

        return View();
    }

    // POST: Produtos/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    Produto produto,
    IFormFile? imagem,
    string Preco)
    {


        // Remove o erro automático do decimal

        if (!string.IsNullOrEmpty(Preco))
        {
            produto.Preco = decimal.Parse(
                Preco.Replace(",", "."),
                System.Globalization.CultureInfo.InvariantCulture
            );
        }


        if (produto.Estoque <= 0)
        {
            ModelState.AddModelError(
                "Estoque",
                "O estoque deve ser maior que zero."
            );

            ViewBag.Categorias = new SelectList(
                _context.Categorias,
                "Id",
                "Nome",
                produto.CategoriaId
            );

            return View(produto);
        }

        if (imagem != null && imagem.Length > 0)
        {
            string pasta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/images/produtos");

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            string nomeArquivo =
                Guid.NewGuid().ToString() +
                Path.GetExtension(imagem.FileName);

            string caminhoCompleto = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            produto.ImagemUrl = "/images/produtos/" + nomeArquivo;
        }


        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();


        if (produto.CategoriaId == 1)
            return RedirectToAction("Lanches");

        if (produto.CategoriaId == 2)
            return RedirectToAction("Bebidas");

        return RedirectToAction(nameof(Index));
    }
    // GET: Produtos/Edit/5
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

        ViewBag.Categorias = new SelectList(
            _context.Categorias,
            "Id",
            "Nome",
            produto.CategoriaId);

        return View(produto);
    }

    // POST: PRODUTOS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
    int id,
    Produto produto,
    IFormFile? imagem)
    {
        if (id != produto.Id)
            return NotFound();

        var produtoBanco = await _context.Produtos.FindAsync(id);

        if (produtoBanco == null)
            return NotFound();

        if (produto.Estoque <= 0)
        {
            ModelState.AddModelError("Estoque", "O estoque deve ser maior que zero.");

            ViewBag.Categorias = new SelectList(_context.Categorias, "Id", "Nome", produto.CategoriaId);
            return View(produto);
        }

        // Atualiza dados básicos
        produtoBanco.Nome = produto.Nome;
        produtoBanco.Descricao = produto.Descricao;
        produtoBanco.Preco = produto.Preco;
        produtoBanco.Estoque = produto.Estoque;
        produtoBanco.Ativo = produto.Ativo;
        produtoBanco.CategoriaId = produto.CategoriaId;

        // imagem
        if (imagem != null && imagem.Length > 0)
        {
            string pasta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/images/produtos"
            );

            if (!Directory.Exists(pasta))
                Directory.CreateDirectory(pasta);

            if (!string.IsNullOrEmpty(produtoBanco.ImagemUrl))
            {
                var imagemAntiga = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    produtoBanco.ImagemUrl.TrimStart('/')
                );

                if (System.IO.File.Exists(imagemAntiga))
                    System.IO.File.Delete(imagemAntiga);
            }

            string nomeArquivo =
                Guid.NewGuid().ToString() +
                Path.GetExtension(imagem.FileName);

            string caminhoCompleto = Path.Combine(pasta, nomeArquivo);

            using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await imagem.CopyToAsync(stream);
            }

            produtoBanco.ImagemUrl = "/images/produtos/" + nomeArquivo;
        }

        await _context.SaveChangesAsync();

        if (produtoBanco.CategoriaId == 1)
            return RedirectToAction("Lanches");

        if (produtoBanco.CategoriaId == 2)
            return RedirectToAction("Bebidas");

        return RedirectToAction(nameof(Index));
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
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);

        if (produto != null)
        {
            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();
        }

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

