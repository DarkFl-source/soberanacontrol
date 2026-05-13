using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Domain.Entities;
using SoberanaControl.Infrastructure.Data;

namespace SoberanaControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FornecedoresController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FornecedoresController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var fornecedores = await _context.Fornecedores.ToListAsync();
        return Ok(fornecedores);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] FornecedorRequest request)
    {
        var fornecedor = new Fornecedor(request.Cnpj, request.RazaoSocial, request.Contato, request.Endereco);
        _context.Fornecedores.Add(fornecedor);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = fornecedor.Id }, fornecedor);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(Guid id, [FromBody] FornecedorRequest request)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);
        if (fornecedor == null) return NotFound();
        fornecedor.Atualizar(request.Cnpj, request.RazaoSocial, request.Contato, request.Endereco);
        await _context.SaveChangesAsync();
        return Ok(fornecedor);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var fornecedor = await _context.Fornecedores.FindAsync(id);
        if (fornecedor == null) return NotFound();
        _context.Fornecedores.Remove(fornecedor);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

public record FornecedorRequest(string Cnpj, string RazaoSocial, string Contato, string Endereco);
