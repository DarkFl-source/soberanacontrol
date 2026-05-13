using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoberanaControl.Application.DTOs;
using SoberanaControl.Application.Interfaces;
using SoberanaControl.Application.UseCases;

namespace SoberanaControl.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovimentacoesController : ControllerBase
{
    private readonly IApplicationDbContext _dbContext;
    private readonly RegistrarMovimentacaoUseCase _registrarUseCase;

    public MovimentacoesController(IApplicationDbContext dbContext, RegistrarMovimentacaoUseCase registrarUseCase)
    {
        _dbContext = dbContext;
        _registrarUseCase = registrarUseCase;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovimentacaoResponseDto>>> Get()
    {
        var movimentacoes = await _dbContext.Movimentacoes
            .Include(m => m.Produto)
            .Include(m => m.ObraOrigem)
            .Include(m => m.ObraDestino)
            .OrderByDescending(m => m.DataHora)
            .Select(m => new MovimentacaoResponseDto
            {
                Id = m.Id,
                ProdutoNome = m.Produto.Nome,
                ObraOrigemNome = m.ObraOrigem != null ? m.ObraOrigem.Nome : null,
                ObraDestinoNome = m.ObraDestino != null ? m.ObraDestino.Nome : null,
                Tipo = m.Tipo.ToString(),
                Quantidade = m.Quantidade,
                ValorUnitario = m.ValorUnitario,
                DataHora = m.DataHora
            })
            .ToListAsync();

        return Ok(movimentacoes);
    }

    [HttpPost]
    public async Task<IActionResult> Post(MovimentacaoCreateDto dto)
    {
        try
        {
            await _registrarUseCase.ExecuteAsync(dto);
            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
