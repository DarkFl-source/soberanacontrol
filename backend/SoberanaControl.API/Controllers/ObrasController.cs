using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Domain.Entities;
using SoberanaControl.Infrastructure.Data;

namespace SoberanaControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ObrasController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ObrasController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var obras = await _context.Obras.ToListAsync();
        return Ok(obras);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var obra = await _context.Obras.FindAsync(id);
        if (obra == null) return NotFound();
        return Ok(obra);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ObraRequest request)
    {
        var obra = new Obra(request.Nome, request.Endereco);
        _context.Obras.Add(obra);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = obra.Id }, obra);
    }
}

public record ObraRequest(string Nome, string Endereco);
