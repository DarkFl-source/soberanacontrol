using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Domain.Entities;
using SoberanaControl.Infrastructure.Data;

namespace SoberanaControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UnidadesMedidaController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public UnidadesMedidaController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        return Ok(await _context.UnidadesMedida.ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UnidadeMedidaRequest request)
    {
        var unidade = new UnidadeMedida(request.Sigla, request.Descricao);
        _context.UnidadesMedida.Add(unidade);
        await _context.SaveChangesAsync();
        return Ok(unidade);
    }
}

public record UnidadeMedidaRequest(string Sigla, string Descricao);
