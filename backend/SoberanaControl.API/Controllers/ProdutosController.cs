using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Domain.Entities;
using SoberanaControl.Infrastructure.Data;

namespace SoberanaControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ProdutosController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var produtos = await _context.Produtos
            .Include(p => p.Categoria)
            .Include(p => p.UnidadeMedida)
            .ToListAsync();
        return Ok(produtos);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProdutoRequest request)
    {
        var produto = new Produto(request.CodigoInterno, request.Nome, request.CategoriaId, request.UnidadeMedidaId, request.EstoqueMinimo);
        _context.Produtos.Add(produto);
        await _context.SaveChangesAsync();
        return Ok(produto);
    }
}

public record ProdutoRequest(string CodigoInterno, string Nome, Guid CategoriaId, Guid UnidadeMedidaId, decimal EstoqueMinimo);
