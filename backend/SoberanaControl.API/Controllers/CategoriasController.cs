using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Domain.Entities;
using SoberanaControl.Infrastructure.Data;

namespace SoberanaControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CategoriasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _context.Categorias.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] string nome)
    {
        var categoria = new Categoria(nome);
        _context.Categorias.Add(categoria);
        await _context.SaveChangesAsync();
        return Ok(categoria);
    }
}
